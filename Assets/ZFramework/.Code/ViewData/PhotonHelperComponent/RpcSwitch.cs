///**   ZFramework
//* FileName:         RpcSwitch.cs
//* Author:           Rucie
//* Email:            76717757@qq.com
//* CreateTime:       2021-09-24
//* Description:      
//**/

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using ZFramework;

//public class RpcSwitch
//{
//    //这里定义一些协议  0号parms固定为byte协议号  剩下的才是协议参数
//    public static void Switch(RPC协议 code, object[] parms)
//    {
//        switch (code)
//        {
//            case RPC协议.成员请求数据:
//                ZEvent.CustomEvent.Call(code.ToString(), (int)parms[1], (int)parms[2]);//  请求数据的类型 playerid
//                break;
//            case RPC协议.房主发送数据:
//                ZEvent.CustomEvent.Call(code.ToString(), (int)parms[1], (int)parms[2], (string)parms[3]);// 请求数据的类型  playerid  主机发出的json串
//                break;

//            case RPC协议.申请权限:
//                ZEvent.CustomEvent.Call(code.ToString(), (int)parms[1], (string)parms[2], (bool)parms[3]);//playerid , job.tojson , value
//                break;
//            case RPC协议.权限变更通知:
//                ZEvent.CustomEvent.Call(code.ToString(), (string)parms[1], (bool)parms[2]);
//                break;

//            case RPC协议.上传机械臂文件://只有机械臂有文件!
//                ZEvent.CustomEvent.Call(code.ToString(), (int)parms[1], ((int)parms[2], (int)parms[3], (string)parms[4]));//playerid   (userid,dataindex,jxbdata.tojson)
//                break;
//            case RPC协议.确认上传机械臂文件:
//                ZEvent.CustomEvent.Call(code.ToString(), (int)parms[1], ((int)parms[2], (int)parms[3], (string)parms[4]));//playerid   (userid,dataindex,jxbdata.tojson)
//                break;

//            case RPC协议.提交时间轴修改://有人提交
//                ZEvent.CustomEvent.Call(code.ToString(),(int)parms[1], (string)parms[2]); //userid timelineProperty.tojson
//                break;
//            case RPC协议.时间轴修改通知://通知其他人download红点点亮
//                ZEvent.CustomEvent.Call(code.ToString(), (int)parms[1]); //userid
//                break;

//            case RPC协议.聊天室:
//                ZEvent.CustomEvent.Call(code.ToString(), (string)parms[1]);//ChatRoomData.tojson
//                break;
//            case RPC协议.日志:
//                ZEvent.CustomEvent.Call(code.ToString(), (string)parms[1], (string)parms[2], (string)parms[3]);//realname time chatValue
//                break;
//            //case RPC协议.准备指令:
//            //    ZEvent.CustomEvent.Call(code.ToString());
//            //    break;
//            //case RPC协议.准备完成:
//            //    ZEvent.CustomEvent.Call(code.ToString(), (int)parms[1], (bool)parms[2]);//playerid  ready
//            //    break;
//            //case RPC协议.AD关节指令:
//            //    ZEvent.CustomEvent.Call(code.ToString(), (string)parms[1], (float)parms[2],(long)parms[3]);// AD网络标识, 角度增量, ticks
//            //    break;
//            default:
//                Log.Info($"收到意外的rpc调用 code->{code}");
//                break;
//        }
//    }
//}

//public enum RPC协议 : byte
//{
//    成员请求数据,//成员发请求  //这两个是成对的  观众发请求  主机回复json串 用来同步配置信息 如空间站设置/任务设置/清单等  数据以房主为准
//    房主发送数据,//主机发正文

//    申请权限,//包含申请控制 和 申请解除
//    权限变更通知,//成员一人申请  然后房主通知所有人 有权限变更 UI跟着变化

//    上传机械臂文件,
//    确认上传机械臂文件,

//    下载配置,
//    主机发送配置,

//    提交时间轴修改,
//    时间轴修改通知,

//    聊天室,
//    日志,

//    //准备指令,
//    //准备完成,
//    //AD关节指令,
//}

///// <summary>
///// 客户端发送请求的类型   房主收到请求之后把对应的json串发过去
///// </summary>
//public enum DTRequestType { 
//    None,
//    完整配置,

//    同步配置,



//    历史聊天记录,//??
//    历史日志,
//}
