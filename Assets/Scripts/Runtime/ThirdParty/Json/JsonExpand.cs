/** Header
 *  JsonExpand.cs
 *  Json拓展 使其支持unity特有类型 verctor2 verctor3 Rect等
 **/

using System;
using UnityEngine;

namespace ZFramework
{
    /// <summary> Json拓展类 </summary>
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public static class JsonExpand
    {
        private static bool registerd;
        static JsonExpand() { Register(); }
        private static void WriteProperty(this JsonWriter w, string name, int value)
        {
            w.WritePropertyName(name);
            w.WriteValue(value);
        }
        private static void WriteProperty(this JsonWriter w, string name, long value)
        {
            w.WritePropertyName(name);
            w.WriteValue(value);
        }
        private static void WriteProperty(this JsonWriter w, string name, string value)
        {
            w.WritePropertyName(name);
            w.WriteValue(value);
        }
        private static void WriteProperty(this JsonWriter w, string name, bool value)
        {
            w.WritePropertyName(name);
            w.WriteValue(value);
        }
        private static void WriteProperty(this JsonWriter w, string name, double value)
        {
            w.WritePropertyName(name);
            w.WriteValue(value);
        }
        private static void WriteProperty(this JsonWriter w, string name, float value)
        {
            w.WritePropertyName(name);
            w.WriteValue(value);
        }
        private static void Register()
        {
            if (registerd) return;
            registerd = true;
            Json json = new Json();

            //--------反序列化注册

            // 注册整数反序列化为布尔值的Importer
            json.RegisterImporter<int, bool>((i) =>
            {
                return i != 0;
            });

            // 注册Type类型的Importer
            json.RegisterImporter<string, Type>((s) =>
            {
                return Type.GetType(s);
            });


            //--------序列化注册

            // 注册Type类型的Exporter
            json.RegisterExporter<Type>((v, w) =>
            {
                w.WriteValue(v.FullName);
            });

            // 注册Vector2类型的Exporter
            json.RegisterExporter<Vector2>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteObjectEnd();
            });

            // 注册Vector3类型的Exporter
            Action<Vector3, JsonWriter> writeVector3 = (v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("z", v.z);
                w.WriteObjectEnd();
            };

            json.RegisterExporter<Vector3>((v, w) =>
            {
                writeVector3(v, w);
            });

            // 注册Vector4类型的Exporter
            json.RegisterExporter<Vector4>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("z", v.z);
                w.WriteProperty("w", v.w);
                w.WriteObjectEnd();
            });

            // 注册Quaternion类型的Exporter
            json.RegisterExporter<Quaternion>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("z", v.z);
                w.WriteProperty("w", v.w);
                w.WriteObjectEnd();
            });


            // 注册Color类型的Exporter
            Action<Color, JsonWriter> writeColor = (v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("r", v.r);
                w.WriteProperty("g", v.g);
                w.WriteProperty("b", v.b);
                w.WriteProperty("a", v.a);
                w.WriteObjectEnd();
            };
            json.RegisterExporter<Color>((v, w) =>
            {
                writeColor(v, w);
            });

            // 注册Color32类型的Exporter
            json.RegisterExporter<Color32>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("r", v.r);
                w.WriteProperty("g", v.g);
                w.WriteProperty("b", v.b);
                w.WriteProperty("a", v.a);
                w.WriteObjectEnd();
            });

            // 注册Bounds类型的Exporter
            json.RegisterExporter<Bounds>((v, w) =>
            {
                w.WriteObjectStart();

                w.WritePropertyName("center");
                writeVector3(v.center, w);

                w.WritePropertyName("size");
                writeVector3(v.size, w);

                w.WriteObjectEnd();
            });

            // 注册Rect类型的Exporter
            json.RegisterExporter<Rect>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("width", v.width);
                w.WriteProperty("height", v.height);
                w.WriteObjectEnd();
            });

            // 注册RectOffset类型的Exporter
            json.RegisterExporter<RectOffset>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("top", v.top);
                w.WriteProperty("left", v.left);
                w.WriteProperty("bottom", v.bottom);
                w.WriteProperty("right", v.right);
                w.WriteObjectEnd();
            });

            // 注册GradientAlphaKey类型的Exporter
            json.RegisterExporter<GradientAlphaKey>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("time", v.time);
                w.WriteProperty("alpha", v.alpha);
                w.WriteObjectEnd();
            });

            // 注册GradientColorKey类型的Exporter
            json.RegisterExporter<GradientColorKey>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("time", v.time);

                w.WritePropertyName("color");
                writeColor(v.color, w);
                w.WriteObjectEnd();
            });

            // 注册Keyframe类型的Exporter
            Action<Keyframe, JsonWriter> writeKeyfream = (v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("time", v.time);
                w.WriteProperty("value", v.value);
                w.WriteProperty("inTangent", v.inTangent);
                w.WriteProperty("outTangent", v.outTangent);
                w.WriteProperty("inWeight", v.inWeight);
                w.WriteProperty("outWeight", v.outWeight);
                w.WriteProperty("weightedMode", (int)v.weightedMode);
                w.WriteObjectEnd();
            };
            // 注册AnimationCurve类型的Exporter
            json.RegisterExporter<AnimationCurve>((v, w) =>
            {
                w.WriteObjectStart();
                w.WritePropertyName("keys");
                w.WriteArrayStart();
                foreach (var item in v.keys)
                {
                    writeKeyfream(item, w);
                }
                w.WriteArrayEnd();

                w.WriteObjectEnd();
            });
        }

        //拓展方法

        public static JsonData ToObject(this string json) {
            return Json.ToObject(json);
        }
        public static string ToJson(this object obj,bool pretty = false) {
            if (obj == null)
                return "Obj is Null";// new Exception("toJson Fail,obj is null");
            return Json.ToJson(obj,false, pretty);
        }
        public static T ToObject<T>(this string json) {
            return Json.ToObject<T>(json);
        }

    }
}