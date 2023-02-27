using UnityEngine;
using ZFramework;
using System.Collections.Generic;
using System.Collections;
using System;

public class Movement : MonoBehaviour
{
    public const string 摇杆 = "JoyStickCall";
    public const string 跳跃 = "JumpCall";
    public const string 停止跳跃 = "StopJumpCall";

    /// <summary>
    /// 组件
    /// </summary>
    public GameObject Model => gameObject;
    public Animator Animator;
    public CharacterController Controller;

    /// <summary>
    /// 移动参数
    /// </summary>
    private float MaxSpeed = 1.0f;
    private float MoveAcceleration = 5.0f;
    private float JumpHeight = 1.0f;
    private float Gravity = -9.8f;

    /// <summary>
    /// 移动变量
    /// </summary>
    private float HorizontalSpeed = 0.0f;
    private float VerticalSpeed = 0.0f;
    private Vector3 MoveDirection = Vector3.zero;
    private bool isRun = true;
    private bool isJump = false;
    private bool CanJump = true;

    public Camera mapCamera;

    Vector2 input;


    /// <summary>
    /// 程序运行时启用输入检测,监听输入
    /// </summary>
    private void Start()
    {
        ZEvent.CustomEvent.AddListener<Vector2>(摇杆, JoyCall);
        ZEvent.CustomEvent.AddListener(跳跃, JumpCall);
        ZEvent.CustomEvent.AddListener(停止跳跃, StopJumpCall);

        Controller = gameObject.AddComponent<CharacterController>();
        Controller.center = Vector3.up;
        Animator = gameObject.GetComponent<Animator>();
        Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Anim/New_Player_AC");

        ////若当前平台为PC,请启用以下代码
        ////InputManager.Instance.InputDetectionSwitch(true);
        ////监听移动轴
        //EventManager.Instance.AddEventListener<Vector2>("Movement Axis Update", Move);
        ////监听Run事件
        //EventManager.Instance.AddEventListener<KeyCode>(InputManager.Instance.KeyRun + "Down", Run);
        //EventManager.Instance.AddEventListener<KeyCode>(InputManager.Instance.KeyRun + "Up", StopRun);
        ////监听Jump事件
        //EventManager.Instance.AddEventListener<KeyCode>(InputManager.Instance.KeyJump + "Down", Jump);
        //EventManager.Instance.AddEventListener<KeyCode>(InputManager.Instance.KeyJump + "Up", StopJump);

        mapCamera = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("MapCamera")).GetComponent<Camera>();
        StartCoroutine(SyncIE());

    }

    void JoyCall(CustomEventData<Vector2> eventData)
    {
        input = eventData.Data0;
    }

    void JumpCall(CustomEventData eventData)
    {
        if (Controller.isGrounded && (!isJump) && CanJump)
        {
            VerticalSpeed = Mathf.Sqrt(-2.5f * JumpHeight * Gravity);
            CanJump = false;
            isJump = true;
            this.Animator.SetBool("isJump", true);
        }
    }

    void StopJumpCall(CustomEventData eventData)
    {
        //this.Animator.SetBool("isJump", false);
        //isJump = false;
        //CanJump = true;
    }

    private void Update()
    {
        Move(input);
        mapCamera.transform.position = transform.position + Vector3.up * 5;
    }

    IEnumerator SyncIE()
    {
        WaitForSeconds wfs = new WaitForSeconds(1f / 15);
        while (true)
        {
            yield return wfs;
            Vector3 pos = transform.position;
            Quaternion qua = transform.rotation;
            float anim = Animator.GetFloat("Speed");
            //float anim = Animator.GetFloat("HorizontalSpeed");

            TcpClientComponent.Instance.Send2ServerAsync(new C2S_位置同步()
            {
                roleID = GameManager.Instance.Location.role.id,
                sceneName = GameManager.Instance.Location.mapName,
                x = pos.x,
                y = pos.y,
                z = pos.z,
                a = qua.x,
                b = qua.y,
                c = qua.z,
                d = qua.w,
                animKey = anim,
                time = TcpClientComponent.Instance.rtt_2.Ticks / 10000
            });
        }
    }




    /// <summary>
    /// 移动方法
    /// </summary>
    /// <param name="Axis">水平移动轴值</param>
    public void Move(Vector2 Axis)
    {
        //当此组件不存在时,返回
        if (!this) { return; }
        //对移动向量取模
        var SignalMagnitude = Axis.magnitude;// Mathf.Sqrt((Axis.x * Axis.x) + (Axis.y * Axis.y));
        //根据状态设定移动速度
        HorizontalSpeed = Mathf.MoveTowards(HorizontalSpeed, SignalMagnitude * MaxSpeed * (isRun ? 3.0f : 1.0f), MoveAcceleration * Time.deltaTime);
        //设定动画状态机的参数
        Animator.SetFloat("Speed", SignalMagnitude * Mathf.Lerp(Animator.GetFloat("Speed"), (isRun ? 3.0f : 1.0f), 0.1f));
        //设置水平移动速度
        MoveDirection = HorizontalSpeed * SignalMagnitude * Time.deltaTime * Model.transform.forward;
        //跳跃(当且仅当在地面时)
        //if (Controller.isGrounded && isJump && CanJump)
        //{
        //    VerticalSpeed = Mathf.Sqrt(-2.5f * JumpHeight * Gravity);
        //    CanJump = false;
        //}
        //自由落体
        VerticalSpeed += Gravity * Time.deltaTime;
        if (Controller.isGrounded)
        {
            if (isJump)
            {
                if (VerticalSpeed < 0)
                {
                    VerticalSpeed = 0;
                    isJump = false;
                    CanJump = true;
                    this.Animator.SetBool("isJump", false);
                }
            }
            else
            {
                CanJump = true;
                VerticalSpeed = 0;
            }
        }
        else
        {

        }

        //地面给予的支持力
        //设置垂直移动速度
        MoveDirection += Time.deltaTime * VerticalSpeed * Vector3.up;
        //移动
        Controller.Move(MoveDirection);
        /*
         * 待修改
         */
        //旋转角色模型(限制角色模型垂直旋转)
        var orientation = Axis.x * Camera.main.transform.right + Axis.y * Camera.main.transform.forward;
        orientation.y = 0f;
        if (MoveDirection != Vector3.zero)
        {
            Model.transform.forward = Vector3.Slerp(Model.transform.forward, orientation, 0.25f);
        }
    }

}
