using System.Collections;
using System.Collections.Generic;

namespace ZFramework
{
    public static class JsonExpand
    {

        //Õÿ’π∑Ω∑®
        public static JsonData ToObject(this string json)
        {
            return Json.ToObject(json);
        }
        public static string ToJson(this object obj, bool pretty = false)
        {
            if (obj == null)
                return "Obj is Null";// new Exception("toJson Fail,obj is null");
            return Json.ToJson(obj, false, pretty);
        }
        public static T ToObject<T>(this string json)
        {
            return Json.ToObject<T>(json);
        }

    }
}
