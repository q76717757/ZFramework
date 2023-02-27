using System;
using UnityEngine;

namespace ZFramework
{
    public class SyncMovement :MonoBehaviour
    {
        Animator animator;

        Vector3 lastPos;
        Quaternion lastRot;
        long lastTime;

        Vector3 targetPos;
        Quaternion targetQua;
        long targetTime;

        float anim;
        float targetAnim;

        public void Init(Vector3 startPos , Quaternion startRot)
        {
            this.animator = gameObject.GetComponent<Animator>();
            animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Anim/New_Player_AC");

            lastPos = targetPos = startPos;
            lastRot = targetQua = startRot;
            lastTime = targetTime = (DateTime.UtcNow - TcpClientComponent.Instance.rtt_2).Ticks / 10000 - Game.epochTick;

            anim = targetAnim = 0;
        }

        public void Input(Vector3 pos, Quaternion qua,float anim ,long serverTime)
        {
            if (serverTime < lastTime) return;

            lastTime = targetTime;
            lastPos = targetPos;
            lastRot = targetQua;

            targetPos = pos;
            targetQua = qua;
            targetTime = serverTime;

            targetAnim = anim;
        }

        private void Update()
        {
            if (targetTime == lastTime) return;

            var offsetTime = targetTime - lastTime;
            var timeLen = DateTime.UtcNow.Ticks / 10000 - Game.epochTick - lastTime;

            float lerp = (float)((double)timeLen / (double)offsetTime);

            var pos = Vector3.Lerp(lastPos, targetPos, lerp);
            var rot = Quaternion.Lerp(lastRot, targetQua, lerp);

            transform.position = Vector3.Lerp(transform.position, pos, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, 0.1f);

            anim = Mathf.Lerp(anim, targetAnim, 0.5f);
            animator.SetFloat("Speed", anim);
        }
    }
}
