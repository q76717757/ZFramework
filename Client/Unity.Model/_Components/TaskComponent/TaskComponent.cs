using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    public enum TaskState//任务的过程 用来生成折线图
    {
        已开始,//0       --    0  //数字代表折线变化的形状

        转速加速,//081   --    08
        没负载没转速,//1   --  0
        没负载有转速,//1   --    8
        转速减速,//181   --    80

        加入负载,//18    --    878
        负载中,//8       --    8
        减少负载,//81    --    898

        已结束,//不画图
    }

    public class TaskComponent:Component
    {
        //接线完成后的实操任务  

        public Vector3 camIdelPos;
        public Quaternion camIdleRot;

        public 电机整体 电机整体 => 电机整体.instance;
        public 控制器 控制器 => 控制器.instance;
        public 扭矩传感器 扭矩传感器 => 扭矩传感器.instance;
        public 三相电源 三相电源 => 三相电源.instance;
        public 示波器 示波器 => 示波器.instance;
        public 直流电源 直流电源 => 直流电源.instance;

        public int stepIndex;
        public TaskState taskState;

        public bool use转速;
        public bool use正反;

        //转速图
        public float 扭矩;
        public List<float> lineValue;

        //波形图系数
        public List<float> dataHZ;
        public List<float> dataSCALE;

        //临时的task
        public Action<bool> callback;
    }
}
