using System;

namespace ZFramework
{
    public static class ProcessConfigLoader
    {
        public static ProcessConfig[] Load()
        {
            //服务端  去某个地方读表 然后实例出来
            ProcessConfig[] output = new ProcessConfig[]
            {
                new ProcessConfig()
                {
                    processClassName = "DaemonProcess",//和类名一致
                    parms = "DaemonProcess参数"
                }
            };


            return output;
        }
    }
}
