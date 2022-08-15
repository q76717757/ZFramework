using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public class VersionAwake : AwakeSystem<VersionComponent>
    {
        public override void OnAwake(VersionComponent entity)
        {
            Log.Info("Awake");
        }
    }

    public static class VersionSystem
    {
        //public static async Task<VersionInfo> GetServerVersionAsync(this VersionComponent component,string versionFileURL)
        //{
        //    var http = Game.Root.GetComponent<HttpComponent>();
        //    if (http != null)
        //    {
        //        var httpcallback = await http.DownloadFileAsync(versionFileURL);
        //        if (httpcallback.success)
        //        {
        //            string info = Encoding.UTF8.GetString(httpcallback.data);
        //            component.serverVersion = new VersionInfo(info);
        //            return component.serverVersion;
        //        }
        //        else
        //        {
        //            Log.Error("GetServerVersion Faill :" + httpcallback.msg);
        //        }
        //    }
        //    return VersionInfo.Zero;
        //}

        //public static VersionInfo GetLocalVersion(this VersionComponent component)
        //{

        //    return new VersionInfo();
        //}
    }
}
