using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using System.Runtime.CompilerServices;
using EPOOutline;

namespace ZFramework
{
    public class TaskAwake : AwakeSystem<TaskComponent>
    {
        public override void Awake(TaskComponent component)
        {
            component.camIdelPos = new Vector3(6.12f, 1.225f, 1.249f);//  Camera.main.transform.position;
            component.camIdleRot = Quaternion.Euler(5.46f, -90f, 0);// Camera.main.transform.rotation;
            //boot场景add taskcomponent  但是boot场景没有camera main

            component.taskState = TaskState.已结束;

            component.lineValue = new List<float>();
            component.lineValue.AddRange(new float[View_Data_Component.RT0Width]);

            component.dataHZ = new List<float>();
            component.dataHZ.AddRange(new float[View_Data_Component.RT1Width]); //768是目前rawimage2的尺寸  必须是32的倍数
            component.dataSCALE = new List<float>();
            component.dataSCALE.AddRange(new float[View_Data_Component.RT1Width]);

            component.BulidData();
        }
    }

    public static class TaskSystem
    {
        public static async void BulidData(this TaskComponent component)
        {
            float hzP = Mathf.PI * 2 / (float)View_Data_Component.RT1Width;// 抽出一个系数 图片覆盖一个完整正弦周期  实际频率 = X *hzP  X可以直观表示图片包含X个完整周期

            while (Application.isPlaying)
            {
                //Log.Info(component.taskState + ":" + component.dataHZ.Count + ":" + component.dataSCALE.Count);
                switch (component.taskState)
                {
                    case TaskState.已开始:
                        component.dataHZ.Add(100 * hzP);
                        component.dataSCALE.Add(0);

                        component.扭矩 = 0;
                        component.lineValue.Add(0);
                        break;
                    case TaskState.转速加速:
                        component.dataHZ.Add(100 * hzP);
                        for (int i = 0; i < 9; i++)  //0-8
                        {
                            component.dataHZ.Add(100 * hzP);
                            component.dataSCALE.Add(i * 0.1f);
                        }
                        for (int i = 0; i <= 10; i++) //8
                        {
                            component.dataHZ.Add(100 * hzP);
                            component.dataSCALE.Add(0.8f);
                        }
                        for (int i = 8; i >= 1 ; i--) //8-1
                        {
                            component.dataHZ.Add(100 * hzP);
                            component.dataSCALE.Add(i * 0.1f);
                        }
                        for (int i = 0; i <= 33; i++) //33
                        {
                            component.lineValue.Add(控制器.instance.转速 / 1300f / 33f * i);
                        }
                        component.taskState = TaskState.没负载有转速;
                        break;
                    case TaskState.没负载有转速:
                        component.dataHZ.Add(100 * hzP);
                        component.dataSCALE.Add(0.1f);

                        component.扭矩 = UnityEngine.Random.Range(0.01f, 0.06f);
                        component.lineValue.Add(控制器.instance.转速 / 1300f);
                        break;
                    case TaskState.没负载没转速:
                        component.dataHZ.Add(100 * hzP);
                        component.dataSCALE.Add(0.1f);

                        component.扭矩 = UnityEngine.Random.Range(0.01f, 0.06f);
                        component.lineValue.Add(0);
                        break;
                    case TaskState.转速减速:
                        component.dataHZ.Add(100 * hzP);
                        for (int i = 1; i < 9; i++)
                        {
                            component.dataHZ.Add(100 * hzP);
                            component.dataSCALE.Add(i * 0.1f);
                        }
                        for (int i = 0; i < 10; i++)
                        {
                            component.dataHZ.Add(100 * hzP);
                            component.dataSCALE.Add(0.8f);
                        }
                        for (int i = 8; i >= 1; i--)
                        {
                            component.dataHZ.Add(100 * hzP);
                            component.dataSCALE.Add(i * 0.1f);
                        }

                        for (int i = 0; i <= 33; i++) //33
                        {
                            component.lineValue.Add(控制器.instance.转速 / 1300f - 控制器.instance.转速 / 1300f / 33f * i);
                        }
                        component.taskState = TaskState.没负载没转速;
                        break;
                    case TaskState.加入负载://有个凹陷
                        for (int i = 0; i <= 10; i++)
                        {
                            component.dataHZ.Add(100 * hzP);
                            component.dataSCALE.Add(0.8f);
                            component.扭矩 = UnityEngine.Random.Range(0.01f, 0.06f);
                            component.lineValue.Add(控制器.instance.转速 / 1300f * (1 - 0.01f * i));
                        }
                        for (int i = 10; i >= 0; i--)
                        {
                            component.dataHZ.Add(100 * hzP);
                            component.dataSCALE.Add(0.8f);
                            component.扭矩 = UnityEngine.Random.Range(0.01f, 0.06f);
                            component.lineValue.Add(控制器.instance.转速 / 1300f * (1f - 0.01f * i));
                        }
                        component.taskState = TaskState.负载中;
                        break;
                    case TaskState.负载中:
                        component.dataHZ.Add(100 * hzP);
                        component.dataSCALE.Add(0.8f);

                        component.扭矩 = 5 + UnityEngine.Random.Range(0.01f, 0.06f);
                        component.lineValue.Add(控制器.instance.转速 / 1300f);
                        break;
                    case TaskState.减少负载://有个凸起
                        for (int i = 0; i <= 10; i++)
                        {
                            component.dataHZ.Add(100 * hzP);
                            component.dataSCALE.Add(0.1f);
                            component.扭矩 = UnityEngine.Random.Range(0.01f, 0.06f);
                            component.lineValue.Add(控制器.instance.转速 / 1300f * (1 + 0.01f * i));
                        }
                        for (int i = 10; i >= 0; i--)
                        {
                            component.dataHZ.Add(100 * hzP);
                            component.dataSCALE.Add(0.1f);
                            component.扭矩 = UnityEngine.Random.Range(0.01f, 0.06f);
                            component.lineValue.Add(控制器.instance.转速 / 1300f * (1f + 0.01f * i));
                        }
                        component.taskState = TaskState.没负载有转速;
                        break;
                    case TaskState.已结束:
                        break;
                }
                await Task.Delay(20); //0.02s 1像素  每20毫秒一个像素
            }
        }

        public static async void Run(this TaskComponent component,int stepIndex)
        {
            component.stepIndex = Mathf.Max(stepIndex, 0);

            if (component.stepIndex == 0)
            {
                component.lineValue.Clear();
                component.lineValue.AddRange(new float[View_Data_Component.RT0Width]);

                component.dataHZ.Clear();
                component.dataHZ.AddRange(new float[View_Data_Component.RT1Width]);
                component.dataSCALE.Clear();
                component.dataSCALE.AddRange(new float[View_Data_Component.RT1Width]);

                component.taskState = TaskState.已开始;
            }

            while (true)
            {
                Log.Info("Step->" + component.stepIndex);
                if (await component.RunStep())
                {
                    //Log.Info("Step->Fishsh" + component.stepIndex);
                    component.stepIndex++;
                }
                else
                {
                    component.callback = null;
                    Log.Info("Step-> Break");
                    break;
                }
            }
        }
        public static void End(this TaskComponent component)
        {
            component.callback?.Invoke(false);

            component.taskState = TaskState.已结束;
            Game.UI.Hide(UIType.View_Data);
            Game.UI.Hide(UIType.View_Right);
            Game.UI.Hide(UIType.View_Message);

            component.电机整体.ResetState();
            component.控制器.ResetState();
            component.扭矩传感器.ResetState();
            component.三相电源.ResetState();
            component.示波器.ResetState();
            component.直流电源.ResetState();
            
            component.ResetCamera(false);
        }

        public static async Task<bool> RunStep(this TaskComponent component)
        {
            switch (component.stepIndex)
            {
                case 0: Game.UI.Show(UIType.View_Right); component.ResetCamera(false); return await component.ClickDevice(component.三相电源, "请选择三相电源");
                case 1: return await component.ClickDevicePart(component.三相电源.Refs.Get<CapsuleCollider>("开关"), "请打开三相电源开关");
                case 2: return await component.三相电源开关(true, true);
                case 3: return await component.ClickDevicePart(component.三相电源.Refs.Get<CapsuleCollider>("旋钮"), "请将电压调整到380V");
                case 4: return await component.三相电源旋钮(true, true);
                case 5: return await component.ClickDevicePart(component.三相电源.Refs.Get<BoxCollider>("输出按钮"), "请按下三相电源输出按钮,使电源的输出电压逐渐到设定的额定电压");
                case 6: return await component.ResetCamera();
                case 7: return await component.ClickDevice(component.控制器, "请选择控制器");
                case 8: component.控制器.读数("0000",0); return await component.ClickDevicePart(component.控制器.Refs.Get<BoxCollider>("检查"), "请检查是否上电成功");
                case 9: return await component.ShowMessage("上电成功");
                case 10:return await component.ClickDevicePart(component.控制器.Refs.Get<BoxCollider>("设置"), "请将电机控制模式设定为有速度传感器速度控制模式(FVC)");
                case 11:component.控制器.读数("F1",0); return await component.ClickDevicePart(component.控制器.Refs.Get<BoxCollider>("确认"), "请将电机控制模式设定为有速度传感器速度控制模式(FVC)");
                case 12:component.控制器.读数("01",0); return await component.ClickDevicePart(component.控制器.Refs.Get<BoxCollider>("确认"), "请将电机控制模式设定为有速度传感器速度控制模式(FVC)");
                case 13:component.控制器.读数("F1",0); return await component.ClickDevicePart(component.控制器.Refs.Get<BoxCollider>("复位"), "请将电机控制模式设定为有速度传感器速度控制模式(FVC)");
                case 14:component.控制器.读数("0000",0);  return await component.ShowMessage("设置成功");
                case 15:return await component.控制器旋钮和正反转(true, true, "请设置电机运转方向和运转速度");
                case 16:break;
                case 17:component.taskState = TaskState.转速加速; component.控制器旋钮和正反转(false,false); return await component.电机整体.电机旋转动画(component.控制器.转速);
                case 18:return await component.ShowMessage("电机空载试运行正常");       //放在右下角会看不见  只能放弹窗
                case 19:return await component.SetCamera(component.控制器);
                case 20:return await component.ClickDevicePart(component.控制器.Refs.Get<BoxCollider>("复位"), "请停止运行");
                case 21:component.taskState = TaskState.转速减速; return await component.电机整体.电机旋转动画(0);
                case 22:return await component.ShowMessage("电机已停止运行");
                case 23:return await component.ResetCamera();
                case 24:return await component.ClickDevice(component.直流电源, "请选择直流电源");
                case 25:return await component.ClickDevicePart(component.直流电源.Refs.Get<BoxCollider>("开关"), "请打开直流电源");
                case 26:return await component.直流电源.开关(true, true);
                case 27:return await component.ResetCamera();
                case 28:return await component.ClickDevice(component.控制器, "请选择控制器");
                case 29:return await component.ClickDevicePart(component.控制器.Refs.Get<BoxCollider>("运行"), "请运行电机");
                case 30:component.taskState = TaskState.转速加速; return await component.电机整体.电机旋转动画(component.控制器.转速);
                case 31:return await component.ShowMessage("电机运行正常");
                case 32:return await component.ResetCamera();
                case 33:return await component.ClickDevice(component.直流电源, "请选择直流电源");
                case 34:component.直流电源负载(); return false;//这里选择不同V数 出现分支

                    //选择0-4V  弹个窗
                case 35:return await component.ShowMessage("(待替换文本)只能设置5.6.7V");
                case 36:component.stepIndex = 34; return await component.RunStep();

                    //选择5V  实验一阶段结束
                case 37:component.taskState = TaskState.加入负载; return await component.电机整体.电机加入负载动画();
                case 38:Game.UI.Show(UIType.View_Data); Game.UI.Hide(UIType.View_Right); component.ResetCamera(); return false;

                //选择6-7V  过载
                case 39: component.taskState = TaskState.转速减速; return await component.电机整体.电机旋转动画(0);
                case 40: return await component.ShowMessage("当前为过载情况");
                case 41: return await component.ResetCamera();
                case 42: return await component.ClickDevice(component.直流电源,"请选择直流电源");
                case 43: return await component.ClickDevicePart(component.直流电源.Refs.Get<BoxCollider>("开关"), "请关闭直流电源");
                case 44: return await component.直流电源.开关(false, true);
                case 45: return await component.ResetCamera();
                case 46: return await component.ClickDevice(component.三相电源, "请选择三相电源");
                case 47: return await component.ClickDevicePart(component.三相电源.Refs.Get<CapsuleCollider>("开关"), "请关闭三相电源开关");
                case 48: return await component.三相电源开关(false, true);
                case 49: component.taskState = TaskState.已开始; Game.UI.Show(UIType.View_Data);Game.UI.Hide(UIType.View_Right);component.ResetCamera();return false;

                    //第一阶段结束  第二阶段重新设置转速
                case 100+0: return await component.ShowMessage("请先将负载降为0,并且停止电机运行之后,再重新设置转速");
                case 100+1: Game.UI.Show(UIType.View_Right); return await component.ClickDevice(component.直流电源, "请选择直流电源");
                case 100+2: return await component.ClickDevicePart(component.直流电源.Refs.Get<BoxCollider>("0"), "请设置电机负载为0");
                case 100+3: component.直流电源.Refs.Get<TextMesh>("读数").text = "0"; return await component.ClickDevicePart(component.直流电源.Refs.Get<BoxCollider>("V"), "请设置电机负载为0");
                case 100+4: component.直流电源.Refs.Get<TextMesh>("读数").text = "0V"; return await component.ClickDevicePart(component.直流电源.Refs.Get<BoxCollider>("OK"), "请设置电机负载为0");
                case 100+5: component.taskState = TaskState.减少负载; return await component.电机整体.电机移除负载动画();
                case 100+6:return await component.ResetCamera();
                case 100+7: return await component.ClickDevice(component.控制器, "请选择控制器");
                case 100+8: return await component.ClickDevicePart(component.控制器.Refs.Get<BoxCollider>("复位"), "请停止运行电机");
                case 100+9: component.taskState = TaskState.转速减速; return await component.电机整体.电机旋转动画(0);
                case 100+10: return await component.ShowMessage("电机已经停止运行,请重新设置转速");
                case 100+11:return await component.SetCamera(component.控制器);
                case 100+12:return await component.控制器旋钮和正反转(true, false, "请重新设置电机运转方向和运转速度");
                case 100+13: component.控制器旋钮和正反转(false, false);component.stepIndex = 30; return await component.RunStep();

                default:
                    return false;
            }
            return true;
        }
        public static async void BackStep(this TaskComponent component) //上一步
        {
            switch (component.stepIndex)
            {
                //电机旋转动画的时候直接跳出  有个task bug 临时跳过
                case 17:
                case 21:
                case 30:
                case 39:
                case 109:
                    return;
            }

            Log.Info("Back->" + component.stepIndex);
            component.callback?.Invoke(false);

            switch (component.stepIndex)
            {
                case 0:// Game.UI.Show(UIType.View_Right); component.ResetCamera(false); return await component.ClickDevice(component.三相电源, "请选择三相电源");
                    component.Run(0);
                    break;
                case 1:// return await component.ClickDevicePart(component.三相电源.Refs.Get<CapsuleCollider>("开关"), "请打开三相电源开关");
                    component.CancelClickDevicePart(component.三相电源.Refs.Get<CapsuleCollider>("开关"), "");
                    if (await component.ResetCamera())//如果重复点击上一步 会返回false 不重复执行run0
                    {
                        component.Run(0);
                    }
                    break;
                case 2:// return await component.三相电源开关(true, true);
                    if (await component.三相电源开关(false,true))
                    {
                        component.Run(1);
                    }
                    break;
                case 3:// return await component.ClickDevicePart(component.三相电源.Refs.Get<CapsuleCollider>("旋钮"), "请将电压调整到380V");
                    component.CancelClickDevicePart(component.三相电源.Refs.Get<CapsuleCollider>("旋钮"), "");
                    if (await component.三相电源开关(false, true))
                    {
                        component.Run(1);
                    }
                    break;
                case 4:// return await component.三相电源旋钮(true, true);
                    if (await component.三相电源旋钮(false,true))
                    {
                        component.Run(3);
                    }
                    break;
                case 5:// return await component.ClickDevicePart(component.三相电源.Refs.Get<BoxCollider>("输出按钮"), "请按下三相电源输出按钮,使电源的输出电压逐渐到设定的额定电压");
                    component.CancelClickDevicePart(component.三相电源.Refs.Get<BoxCollider>("输出按钮"), "");
                    if (await component.三相电源旋钮(false, true))
                    {
                        component.Run(3);
                    }
                    break;
                case 6:// return await component.ResetCamera();
                    if (await component.SetCamera(component.三相电源))
                    {
                        component.Run(5);
                    }
                    break;
                case 7:// return await component.ClickDevice(component.控制器, "请选择控制器");
                    component.CancelClickDevice(component.控制器, "");
                    if (await component.SetCamera(component.三相电源))
                    {
                        component.Run(5);
                    }
                    break;
                case 8:// component.控制器.读数("0000", 0); return await component.ClickDevicePart(component.控制器.Refs.Get<BoxCollider>("检查"), "请检查是否上电成功");
                    component.CancelClickDevicePart(component.控制器.Refs.Get<BoxCollider>("检查"), "");
                    if (await component.ResetCamera())
                    {
                        component.Run(7);
                    }
                    break;
                case 9:// return await component.ShowMessage("上电成功");
                    component.Run(9);
                    break;
                case 10:// return await component.ClickDevicePart(component.控制器.Refs.Get<BoxCollider>("设置"), "请将电机控制模式设定为有速度传感器速度控制模式(FVC)");
                    component.CancelClickDevicePart(component.控制器.Refs.Get<BoxCollider>("设置"), "");
                    component.Run(8);
                    break;
                case 11:// component.控制器.读数("F1", 0); return await component.ClickDevicePart(component.控制器.Refs.Get<BoxCollider>("确认"), "请将电机控制模式设定为有速度传感器速度控制模式(FVC)");
                    component.CancelClickDevicePart(component.控制器.Refs.Get<BoxCollider>("确认"), "");
                    component.控制器.读数("0000", 0); 
                    component.Run(10);
                    break;
                case 12:// component.控制器.读数("01", 0); return await component.ClickDevicePart(component.控制器.Refs.Get<BoxCollider>("确认"), "请将电机控制模式设定为有速度传感器速度控制模式(FVC)");
                    component.CancelClickDevicePart(component.控制器.Refs.Get<BoxCollider>("确认"), "");
                    component.控制器.读数("F1", 0);
                    component.Run(11);
                    break;
                case 13:// component.控制器.读数("F1", 0); return await component.ClickDevicePart(component.控制器.Refs.Get<BoxCollider>("复位"), "请将电机控制模式设定为有速度传感器速度控制模式(FVC)");
                    component.CancelClickDevicePart(component.控制器.Refs.Get<BoxCollider>("复位"), "");
                    component.控制器.读数("01", 0);
                    component.Run(12);
                    break;
                case 14:// component.控制器.读数("0000", 0); return await component.ShowMessage("设置成功");
                    component.Run(14);
                    break;
                case 15:// return await component.控制器旋钮和正反转(true, true, "请设置电机运转方向和运转速度");
                    component.控制器旋钮和正反转(false, true);
                    component.Run(13);
                    break;
                case 16:// break;
                    break;
                case 17: //component.taskState = TaskState.转速加速; component.控制器旋钮和正反转(false, false); return await component.电机整体.电机旋转动画(component.控制器.转速);
                    //有bug
                    if (await component.SetCamera(component.控制器))
                    {
                        //component.taskState = TaskState.转速加速;  //转速加速取消.................................................................
                        component.电机整体.设置转速(0);
                        component.电机整体.透明(false);
                        component.Run(15);
                    }
                    break;
                case 18:// return await component.ShowMessage("电机空载试运行正常");       //放在右下角会看不见  只能放弹窗
                    component.Run(18);
                    break;
                case 19:// return await component.SetCamera(component.控制器);
                    component.Run(19);
                    break;
                case 20:// return await component.ClickDevicePart(component.控制器.Refs.Get<BoxCollider>("复位"), "请停止运行");
                    component.CancelClickDevicePart(component.控制器.Refs.Get<BoxCollider>("复位"), "");
                    component.电机整体.设置转速(0);
                    component.电机整体.透明(false);
                    component.Run(15);
                    break;
                case 21:// component.taskState = TaskState.转速减速; return await component.电机整体.电机旋转动画(0);
                    //有BUG的
                    if (await component.SetCamera(component.控制器))
                    {
                        component.电机整体.设置转速(0);//旧转速
                        component.电机整体.透明(false);
                        component.Run(20);
                    }
                    break;
                case 22:// return await component.ShowMessage("电机已停止运行");
                    component.Run(22);
                    break;
                case 23:// return await component.ResetCamera();
                    if (await component.SetCamera(component.控制器))
                    {
                        component.Run(20);
                    }
                    break;
                case 24:// return await component.ClickDevice(component.直流电源, "请选择直流电源");
                    component.CancelClickDevice(component.直流电源, "");
                    if (await component.SetCamera(component.控制器))
                    {
                        component.电机整体.设置转速(component.控制器.转速Backup);
                        component.Run(20);
                    }
                    break;
                case 25:// return await component.ClickDevicePart(component.直流电源.Refs.Get<BoxCollider>("开关"), "请打开直流电源");
                    component.CancelClickDevicePart(component.直流电源.Refs.Get<BoxCollider>("开关"), "");
                    if (await component.ResetCamera())
                    {
                        component.Run(24);
                    }
                    break;
                case 26:// return await component.直流电源.开关(true, true);
                    if (await component.直流电源.开关(false, true))
                    {
                        component.Run(25);
                    }
                    break;
                case 27:// return await component.ResetCamera();
                    component.直流电源.开关(false, false);
                    if (await component.SetCamera(component.直流电源))
                    {
                        component.Run(25);
                    }
                    break;
                case 28:// return await component.ClickDevice(component.控制器, "请选择控制器");
                    component.CancelClickDevice(component.控制器,"");
                    component.直流电源.开关(false, false);
                    if (await component.SetCamera(component.直流电源))
                    {
                        component.Run(25);
                    }
                    break;
                case 29:// return await component.ClickDevicePart(component.控制器.Refs.Get<BoxCollider>("运行"), "请运行电机");
                    component.CancelClickDevicePart(component.控制器.Refs.Get<BoxCollider>("运行"),"");
                    if (await component.ResetCamera())
                    {
                        component.Run(28);
                    }
                    break;
                case 30:// component.taskState = TaskState.转速加速; return await component.电机整体.电机旋转动画(component.控制器.转速);
                    //有BUG的
                    if (await component.SetCamera(component.控制器))
                    {
                        component.电机整体.设置转速(0);
                        component.电机整体.透明(false);
                        component.Run(29);
                    }
                    break;
                case 31:// return await component.ShowMessage("电机运行正常");
                    component.Run(31);
                    break;
                case 32:// return await component.ResetCamera();
                    if (await component.SetCamera(component.控制器))
                    {
                        component.电机整体.设置转速(0);
                        component.电机整体.透明(false);
                        component.Run(29);
                    }
                    break;
                case 33:// return await component.ClickDevice(component.直流电源, "请选择直流电源");
                    component.CancelClickDevice(component.直流电源,"");
                    if (await component.SetCamera(component.控制器))
                    {
                        component.电机整体.设置转速(0);
                        component.电机整体.透明(false);
                        component.Run(29);
                    }
                    break;
                case 34:// component.直流电源负载(); return false;//这里选择不同V数 出现分支
                    component.直流电源负载(false);
                    if (await component.ResetCamera())
                    {
                        component.Run(33);
                    }
                    break;

                //选择0-4V  弹个窗
                case 35:// return await component.ShowMessage("(待替换文本)只能设置5.6.7V");
                    component.Run(35);
                    break;
                case 36:// component.stepIndex = 34; return await component.RunStep();
                    break;

                //选择5V  实验一阶段结束
                case 37:// component.taskState = TaskState.加入负载; Game.UI.Show(UIType.View_Data); Game.UI.Hide(UIType.View_Right); component.ResetCamera(); return false;
                    break;

                //选择6-7V  过载
                case 38:// component.taskState = TaskState.转速减速; return await component.电机整体.电机旋转动画(0);
                    component.直流电源负载(false);
                    if (await component.ResetCamera())
                    {
                        component.Run(33);
                    }
                    break;
                case 39:// break;// return await component.ShowMessage("(待替换文本)过载,关机,结束实验");
                    break;
                case 40:// return await component.ResetCamera();
                    if (await component.SetCamera(component.直流电源))
                    {
                        component.Run(34);
                    }
                    break;
                case 41:// return await component.ClickDevice(component.直流电源, "请选择直流电源");
                    component.CancelClickDevice(component.直流电源,"");
                    if (await component.SetCamera(component.直流电源))
                    {
                        component.Run(34);
                    }
                    break;
                case 42:// return await component.ClickDevicePart(component.直流电源.Refs.Get<BoxCollider>("开关"), "请关闭直流电源");
                    component.CancelClickDevicePart(component.直流电源.Refs.Get<BoxCollider>("开关"),"");
                    if (await component.ResetCamera())
                    {
                        component.Run(41);
                    }
                    break;
                case 43:// return await component.直流电源.开关(false, true);
                    if (await component.直流电源.开关(true, true))
                    {
                        component.Run(42);
                    }
                    break;
                case 44:// return await component.ResetCamera();
                    component.直流电源.开关(true, false);
                    if (await component.SetCamera(component.直流电源))
                    {
                        component.Run(42);
                    }
                    break;
                case 45:// return await component.ClickDevice(component.三相电源, "请选择三相电源");
                    component.CancelClickDevice(component.三相电源,"");
                    component.直流电源.开关(true, false);
                    component.直流电源.Refs.Get<TextMesh>("读数").text = "6V";
                    if (await component.SetCamera(component.直流电源))
                    {
                        component.Run(42);
                    }
                    break;
                case 46:// return await component.ClickDevicePart(component.三相电源.Refs.Get<CapsuleCollider>("开关"), "请关闭三相电源开关");
                    component.CancelClickDevicePart(component.三相电源.Refs.Get<CapsuleCollider>("开关"),"");
                    if (await component.ResetCamera())
                    {
                        component.Run(45);
                    }
                    break;
                case 47:// return await component.三相电源开关(false, true);
                    if (await component.三相电源开关(true, true))
                    {
                        component.三相电源旋钮(true, false);
                        component.Run(46);
                    }
                    break;
                case 48:// component.taskState = TaskState.已开始; Game.UI.Show(UIType.View_Data); Game.UI.Hide(UIType.View_Right); component.ResetCamera(); return false;
                    break;

                //第一阶段结束  第二阶段重新设置转速
                case 100 + 0:// return await component.ShowMessage("请先将负载降为0,并且停止电机运行之后,再重新设置转速");
                    component.Run(100 + 0);
                    break;
                case 100 + 1:// Game.UI.Show(UIType.View_Right); return await component.ClickDevice(component.直流电源, "请选择直流电源");
                    component.Run(100 + 1);
                    break;
                case 100 + 2:// return await component.ClickDevicePart(component.直流电源.Refs.Get<BoxCollider>("0"), "请设置电机负载为0");
                    component.CancelClickDevicePart(component.直流电源.Refs.Get<BoxCollider>("0"),"");
                    if (await component.ResetCamera())
                    {
                        component.Run(100 + 1);
                    }
                    break;
                case 100 + 3:// component.直流电源.Refs.Get<TextMesh>("读数").text = "0"; return await component.ClickDevicePart(component.直流电源.Refs.Get<BoxCollider>("V"), "请设置电机负载为0");
                    component.CancelClickDevicePart(component.直流电源.Refs.Get<BoxCollider>("V"), "");
                    component.直流电源.Refs.Get<TextMesh>("读数").text = "5V";//这是二阶段 一定是5V
                    component.Run(100 + 2);
                    break;
                case 100 + 4:// component.直流电源.Refs.Get<TextMesh>("读数").text = "0V"; return await component.ClickDevicePart(component.直流电源.Refs.Get<BoxCollider>("OK"), "请设置电机负载为0");
                    component.CancelClickDevicePart(component.直流电源.Refs.Get<BoxCollider>("OK"), "");
                    component.直流电源.Refs.Get<TextMesh>("读数").text = "0";
                    component.Run(100 + 3);
                    break;
                case 100 + 5:// component.taskState = TaskState.减少负载; return await component.ResetCamera();
                    if (await component.SetCamera(component.直流电源))
                    {
                        component.Run(100 + 4);
                    }
                    break;
                case 100 + 6:// return await component.ClickDevice(component.控制器, "请选择控制器");
                    component.CancelClickDevice(component.控制器, "");
                    if (await component.SetCamera(component.直流电源))
                    {
                        component.Run(100 + 4);
                    }
                    break;
                case 100 + 7:// return await component.ClickDevicePart(component.控制器.Refs.Get<BoxCollider>("复位"), "请停止运行电机");
                    component.CancelClickDevicePart(component.控制器.Refs.Get<BoxCollider>("复位"),"");
                    if (await component.ResetCamera())
                    {
                        component.Run(100 + 6);
                    }
                    break;
                case 100 + 8:// return await component.电机整体.电机旋转动画(0);
                    //有BUG的
                    if (await component.SetCamera(component.控制器))
                    {
                        component.电机整体.设置转速(component.控制器.转速Backup);
                        component.电机整体.透明(false);
                        component.Run(100 + 7);
                    }
                    break;
                case 100 + 9:// return await component.ShowMessage("电机已经停止运行,请重新设置转速");
                    component.Run(100 + 9);
                    break;
                case 100 + 10:// return await component.SetCamera(component.控制器);
                    component.Run(100 + 10);
                    break;
                case 100 + 11:// return await component.控制器旋钮和正反转(true, false, "请重新设置电机运转方向和运转速度");
                    component.控制器旋钮和正反转(false, false);
                    component.电机整体.设置转速(component.控制器.转速Backup);
                    component.电机整体.透明(false);
                    component.Run(100 + 7);
                    break;
                case 100 + 12:// component.控制器旋钮和正反转(false, false); component.stepIndex = 30; return await component.RunStep();
                    break;

                default: break;
            }
        }

        //点击设备
        public static TaskComponent ClickDevice(this TaskComponent component, Device device, string tipInfo)
        {
            (Game.UI.Get(UIType.View_Right) as View_Right_Component).ShowTip(tipInfo);

            device.outlinable.enabled = true;
            device.box.enabled = true;
            ZEvent.UnitEvent.AddListener<Device>(device, component.ClickDeviceCallback, device, true);
            return component;
        }
        public static void ClickDeviceCallback(this TaskComponent component, UnitEventData<Device> eventData)
        {
            if (eventData.EventType == UnitEventType.Click)
            {
                eventData.Data0.box.enabled = false;
                eventData.Data0.outlinable.enabled = false;

                var camTran = Camera.main.transform;
                var targetPos = eventData.Data0.camPos;
                camTran.DOKill();
                camTran.DOMove(targetPos.position, 0.5f);
                camTran.DORotateQuaternion(targetPos.rotation, 0.5f).onComplete += () =>
                {
                    component.callback?.Invoke(true);
                };
            }
        }
        public static void CancelClickDevice(this TaskComponent component, Device device, string tipInfo)
        {
            (Game.UI.Get(UIType.View_Right) as View_Right_Component).ShowTip(tipInfo);
            device.outlinable.enabled = false;
            device.box.enabled = false;
            ZEvent.UnitEvent.RemoveListener<Device>(device.gameObject, component.ClickDeviceCallback);
        }

        //点击设备上的按钮
        public static TaskComponent ClickDevicePart(this TaskComponent component, Collider collider, string tipinfo)
        {
            collider.enabled = true;
            collider.GetComponent<Outlinable>().enabled = true;

            ZEvent.UnitEvent.AddListener<Collider>(collider, component.ClickDevicePartCallback, collider, true);
            (Game.UI.Get(UIType.View_Right) as View_Right_Component).ShowTip(tipinfo);
            return component;
        }
        public static void CancelClickDevicePart(this TaskComponent component, Collider collider, string tipinfo)
        {
            collider.enabled = false;
            collider.GetComponent<Outlinable>().enabled = false;

            ZEvent.UnitEvent.RemoveListener<Collider>(collider.gameObject, component.ClickDevicePartCallback);
            (Game.UI.Get(UIType.View_Right) as View_Right_Component).ShowTip(tipinfo);
        }
        public static void ClickDevicePartCallback(this TaskComponent component, UnitEventData<Collider> eventData)
        {
            if (eventData.EventType == UnitEventType.Click)
            {
                eventData.Data0.enabled = false;
                eventData.Data0.GetComponent<Outlinable>().enabled = false;

                component.callback?.Invoke(true);
            }
        }

        //移动相机到设备
        public static TaskComponent SetCamera(this TaskComponent component, Device device)
        {
            device.box.enabled = false;
            device.outlinable.enabled = false;

            var camTran = Camera.main.transform;
            var targetPos = device.camPos;
            camTran.DOKill();
            camTran.DOMove(targetPos.position, 0.5f);
            camTran.DORotateQuaternion(targetPos.rotation, 0.5f).onComplete += () =>
            {
                component.callback?.Invoke(true);
            };
            return component;
        }
        //重置相机
        public static TaskComponent ResetCamera(this TaskComponent component,bool useAnim = true)
        {
            var camTran = Camera.main.transform;
            camTran.DOKill();
            
            if (useAnim)
            {
                camTran.DOMove(component.camIdelPos, 0.5f);
                camTran.DORotateQuaternion(component.camIdleRot, 0.5f).onComplete += () =>
                {
                    component.callback?.Invoke(true);
                };
            }
            else
            {
                camTran.position = component.camIdelPos;
                camTran.rotation = component.camIdleRot;
            }
            return component;
        }
        //弹窗
        public static TaskComponent ShowMessage(this TaskComponent component,string info)
        {
            (Game.UI.Show(UIType.View_Message) as View_Message_Component).ShowInfo("", info, () =>
            {
                component.callback?.Invoke(true);
            });
            return component;
        }

        //控制器
        public static TaskComponent 控制器旋钮和正反转(this TaskComponent component, bool isOpen,bool reset旋钮,string info = null)
        {
            (Game.UI.Get(UIType.View_Right) as View_Right_Component).ShowTip(info);

            var a = component.控制器.Refs.Get<BoxCollider>("正反转");
            var b = component.控制器.Refs.Get<CapsuleCollider>("运转速度");

            component.use正反 = false;
            component.use转速 = false;
            component.CancelClickDevicePart(component.控制器.Refs.Get<BoxCollider>("运行"), "");

            component.控制器.打开旋钮和正反转(isOpen, reset旋钮);
            if (isOpen)
            {
                ZEvent.UnitEvent.AddListener<string>(a, component.正反转, info);
                ZEvent.UnitEvent.AddListener<string>(b, component.刻度, info);
            }
            else
            {
                ZEvent.UnitEvent.RemoveListener<string>(a.gameObject, component.正反转);
                ZEvent.UnitEvent.RemoveListener<string>(b.gameObject, component.刻度);
            }
            return component;
        }
        public static void 正反转(this TaskComponent component, UnitEventData<string> eventData)
        {
            var point1 = component.控制器.Refs.Get<GameObject>("Point1");
            var point2 = component.控制器.Refs.Get<GameObject>("Point2");
            if (eventData.EventType == UnitEventType.Click)
            {
                if (point1.activeSelf)
                {
                    point1.SetActive(false);
                    point2.SetActive(true);
                }
                else
                {
                    point1.SetActive(true);
                    point2.SetActive(false);
                }
                component.use正反 = true;
                if (component.use转速)
                {
                    component.ClickDevicePart(component.控制器.Refs.Get<BoxCollider>("运行"), eventData.Data0);
                }
                else
                {
                    component.CancelClickDevicePart(component.控制器.Refs.Get<BoxCollider>("运行"), eventData.Data0);
                }
            }
        }
        public static void 刻度(this TaskComponent component, UnitEventData<string> eventData)
        {
            switch (eventData.EventType)
            {
                case UnitEventType.Drag:
                    var scenepos = Camera.main.WorldToScreenPoint(eventData.Target.transform.position);
                    var dir = (eventData.Position - new Vector2(scenepos.x, scenepos.y)).normalized;
                    var qua = Quaternion.FromToRotation(dir, Vector2.up);

                    var z = qua.eulerAngles.z;
                    if (z > 140 && z < 180)
                    {
                        z = 140;
                    }
                    if (z < 220 && z >= 180)
                    {
                        z = 220;
                    }
                    eventData.Target.transform.localRotation = Quaternion.Euler(0, 0, z);
                    float value;
                    if (z > 180)//左边
                    {
                        var lerp = (360 - z) / 140f;
                        value = Mathf.Lerp(600, 0, lerp);
                    }
                    else//右边
                    {
                        var lerp = z / 140f;
                        value = Mathf.Lerp(600, 1200, lerp);
                    }

                    int 读数 = (int)(value / 100) * 100;

                    component.控制器.读数($"{读数:D4}", 读数);

                    component.use转速 = 读数 != 0;
                    if (component.use正反 == true)
                    {
                        if (component.use转速)
                        {
                            component.ClickDevicePart(component.控制器.Refs.Get<BoxCollider>("运行"), eventData.Data0);
                        }
                        else
                        {
                            component.CancelClickDevicePart(component.控制器.Refs.Get<BoxCollider>("运行"), eventData.Data0);
                        }
                    }
                    break;
            }
        }

        //直流电源
        public static void 直流电源负载(this TaskComponent component,bool isOn = true)
        {
            for (int i = 0; i < 8; i++)
            {
                component.直流电源负载按钮开关(i.ToString(), isOn);
            }
            if (!isOn)
            {
                component.直流电源.Refs.Get<TextMesh>("读数").text = "";
                component.直流电源负载按钮开关("V", false);
                component.直流电源负载按钮开关("OK", false);
            }
        } 
        public static void 直流电源负载按钮开关(this TaskComponent component, string btnName,bool isOn)
        {
            var btn = component.直流电源.Refs.Get<BoxCollider>(btnName);
            if (btn == null) return;
            btn.enabled = isOn;
            btn.GetComponent<Outlinable>().enabled = isOn;
            if (isOn)
            {
                ZEvent.UnitEvent.AddListener<string>(btn, component.直流电源负载按钮回调, btnName);
            }
            else
            {
                ZEvent.UnitEvent.RemoveListener<string>(btn.gameObject, component.直流电源负载按钮回调);
            }
        }
        public static void 直流电源负载按钮回调(this TaskComponent component, UnitEventData<string> eventData)
        {
            if (eventData.EventType == UnitEventType.Click)
            {
                switch (eventData.Data0)
                {
                    case "0": 
                    case "1": 
                    case "2": 
                    case "3": 
                    case "4": 
                    case "5": 
                    case "6": 
                    case "7": 
                        component.直流电源.Refs.Get<TextMesh>("读数").text = eventData.Data0;
                        component.直流电源负载按钮开关("V", true);
                        component.直流电源负载按钮开关("OK", false);
                        break;
                    case "V":
                        component.直流电源.Refs.Get<TextMesh>("读数").text += "V";
                        component.直流电源负载按钮开关("V", false);
                        component.直流电源负载按钮开关("OK", true);
                        break;
                    case "OK":
                        string value = component.直流电源.Refs.Get<TextMesh>("读数").text;
                        component.直流电源负载按钮开关("V", false);
                        component.直流电源负载按钮开关("OK", false);
                        for (int i = 0; i < 8; i++)
                        {
                            component.直流电源负载按钮开关(i.ToString(), false);
                        }
                        switch (value)
                        {
                            case "0V":
                            case "1V": 
                            case "2V": 
                            case "3V": 
                            case "4V":
                                component.Run(35);
                                break;
                            case "5V":
                                component.Run(37);
                                break;
                            case "6V": //过载
                            case "7V":
                                component.Run(39);
                                break;
                            default:
                                break;
                        }
                        break;
                }
            }
        }

        //三相电源
        public static TaskComponent 三相电源开关(this TaskComponent component,bool isOn,bool useAnim)
        {
            var caps = component.三相电源.Refs.Get<CapsuleCollider>("开关").transform;
            caps.DOKill();

            component.三相电源.Refs.Get<TextMesh>("电压").text = isOn ? "000V" : "";
            if (useAnim)
            {
                caps.DOLocalRotateQuaternion(Quaternion.Euler(0, -90, isOn ? -200 : -100), 0.5f).onComplete += () =>
                {
                    component.callback?.Invoke(true);
                };
            }
            else
            {
                caps.localRotation = Quaternion.Euler(0, -90, isOn ? -200 : -100);
            }
            if (!isOn)
            {
                component.三相电源旋钮(false,false);
            }

            return component;
        }
        public static TaskComponent 三相电源旋钮(this TaskComponent component, bool isOn, bool useAnim)
        {
            var caps = component.三相电源.Refs.Get<CapsuleCollider>("旋钮").transform;
            caps.DOKill();

            component.三相电源.Refs.Get<TextMesh>("电压").text = isOn ? "380V" : "000V";
            if (useAnim)
            {
                caps.DOLocalRotateQuaternion(Quaternion.Euler(isOn ? -220 : -60, -90, 90), 0.5f).onComplete += () => 
                { 
                    component?.callback?.Invoke(true);
                };
            }
            else
            {
                caps.localRotation = Quaternion.Euler(isOn ? -220 : -60, -90, 90);
            }
            return component;
        }












        public static TaskAwaiter<bool> GetAwaiter(this TaskComponent task)
        {
            var tcs = new TaskCompletionSource<bool>();
            task.callback = (b) => 
            {
                tcs.SetResult(b);
            };
            return tcs.Task.GetAwaiter();
        }

    }
}
