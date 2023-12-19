using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ZFramework
{
	public static class NetHelper
	{
		public static string[] GetAddressIPs()
		{
			//获取本地的IP地址
			List<string> addressIPs = new List<string>();

            Log.Info(Dns.GetHostName());
            foreach (IPAddress address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
			{
				addressIPs.Add(address.ToString());
                Log.Info(address.ToString());
			}
			return addressIPs.ToArray();
		}

        public static string GetIP()
        {
            string output = "";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                NetworkInterfaceType _type1 = NetworkInterfaceType.Wireless80211;//无线局域网适配器   现场用的无线网
                if ((item.NetworkInterfaceType == _type1) && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                            return output;
                        }
                    }
                }

                _type1 = NetworkInterfaceType.Ethernet;
                if ((item.NetworkInterfaceType == _type1) && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                            return output;
                        }
                    }
                }
            }
            return output;
        }
    }
}
