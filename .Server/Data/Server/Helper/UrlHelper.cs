using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ZFramework
{
    public class UrlHelper : IUrlHelper
    {
        //目前并没有初始化
        public static void Reg()
        { 
            UrlUtility.coder = new UrlHelper();
        }

        public string Decode(string url)
        {
            return HttpUtility.UrlDecode(url);
        }
    }
}
