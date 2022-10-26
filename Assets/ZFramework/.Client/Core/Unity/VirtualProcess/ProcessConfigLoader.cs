using System;

namespace ZFramework
{
    public static class ProcessConfigLoader
    {
        public static ProcessConfig[] Load()
        {
            //客户端  
            ProcessConfig[] output = new ProcessConfig[]
            {
                new ProcessConfig()
                {
                    processClassName = "ClientProcess",//和类名一致
                    parms = "ClientProcess参数"
                }
            };

            return output;
        }
    }
}
