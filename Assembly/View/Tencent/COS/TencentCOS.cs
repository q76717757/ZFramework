using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using UnityEngine.Networking;

namespace ZFramework
{
    /// <summary>
    /// 腾讯云 对象存储 COS
    /// </summary>
    public class TencentCOS
    {
        public string APPID { get; set; }
        public string SecretId { get; set; }
        public string SecretKey { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        public string Region { get; set; }
        /// <summary>
        /// 存储桶
        /// </summary>
        public string Bucket { get; set; }
        private string Host => $"{Bucket}-{APPID}.cos.{Region}.myqcloud.com";

        public TencentCOS() { }
        public TencentCOS(string appid,string secretId,string secretKey)
        {
            APPID = appid;
            SecretId = secretId;
            SecretKey = secretKey;
        }

        //请求签名过程  文档:https://cloud.tencent.com/document/product/436/7778
        string 请求签名(string UriPathname, string HttpMethod, Dictionary<string, string> paramaMap, Dictionary<string, string> headerMap)
        {
            //步骤1：生成 KeyTime
            long keyTimDuration = 600;
            long keyStartTime = TimestampUtility.GetCurrentTimestamp(TimeUnit.Seconds);
            long keyEndTime = keyStartTime + keyTimDuration;
            string keyTime = $"{keyStartTime};{keyEndTime}";
            //Log.Info("keyTime->" + keyTime);

            //步骤2：生成 SignKey
            string signKey = new HMACSHA1Helper(SecretKey).ComputeHashStringToHexString(keyTime);
            //Log.Info("signKey->" + signKey);

            //步骤3：生成 UrlParamList 和 HttpParameters   (签名请求)
            GenerateListAndMapString(paramaMap, out string UrlParamList, out string HttpParameters);
            //Log.Info("UrlParamList->" + UrlParamList);
            //Log.Info("HttpParameters->" + HttpParameters);

            //步骤4：生成 HeaderList 和 HttpHeaders       (匿名请求)
            GenerateListAndMapString(headerMap, out string HeaderList, out string HttpHeaders);//两种签名方式二选一  目前使用匿名请求的方式
            //Log.Info("HeaderList->" + HeaderList);
            //Log.Info("HttpHeaders->" + HttpHeaders);

            //步骤5：生成 HttpString
            //请求方法小写  get put 
            //请求路径(不带域名)  /或/exampleobject
            string HttpString = $"{HttpMethod.ToLower()}\n/{UriPathname}\n{HttpParameters}\n{HttpHeaders}\n";
            Log.Info("HttpString->" + HttpString);

            //步骤6：生成 StringToSign
            string StringToSign = $"sha1\n{keyTime}\n{new SHA1Helper().ComputeHashStringToHexString(HttpString)}\n";
            //Log.Info("StringToSign->" + StringToSign);

            //步骤7：生成 Signature
            //使用 HMAC-SHA1 以 SignKey 为密钥（字符串形式，非原始二进制），以 StringToSign 为消息
            string Signature = new HMACSHA1Helper(signKey).ComputeHashStringToHexString(StringToSign);
            //Log.Info("Signature->" + Signature);

            //步骤8：生成签名
            //根据 SecretId、KeyTime、HeaderList、UrlParamList 和 Signature 生成签名，格式为：
            string qm =
                "q-sign-algorithm=sha1" +
                $"&q-ak={SecretId}" +
                $"&q-sign-time={keyTime}" +
                $"&q-key-time={keyTime}" +
                $"&q-header-list={HeaderList}" +
                $"&q-url-param-list={UrlParamList}" +
                $"&q-signature={Signature}";
            //Log.Info("TencetCOS 签名->" + qm);
            return qm;
        }
        void GenerateListAndMapString(Dictionary<string, string> headers, out string KeyList, out string Map)
        {
            if (headers == null || headers.Count == 0)
            {
                KeyList = Map = string.Empty;
                return;
            }
            var encodeQuery = new Dictionary<string, string>(headers.Count);
            foreach (var keyValuePair in headers)
            {
                if (!string.IsNullOrEmpty(keyValuePair.Key))
                {
                    encodeQuery[UrlUtility.Encode_ExcludedAllReserved(keyValuePair.Key).ToLower()] = UrlUtility.Encode_ExcludedCustomReserved(keyValuePair.Value, null, Encoding.UTF8);
                }
            }
            //字典序排序
            var list = encodeQuery.Keys.Select((x) => x.ToLower()).ToList();
            list.Sort(delegate (string strA, string strB)
            {
                return StringUtility.Compare(strA, strB, false);
            });
            StringBuilder keylistBuilder = new StringBuilder();
            StringBuilder mapBuilder = new StringBuilder();
            for (int i = 0, size = list.Count; i < size; i++)
            {
                var key = list[i];
                var value = encodeQuery[list[i]];

                keylistBuilder.Append(key);

                mapBuilder.Append(key);
                mapBuilder.Append('=');
                mapBuilder.Append(value);

                if (i + 1 != size)
                {
                    keylistBuilder.Append(';');
                    mapBuilder.Append('&');
                }
            }
            KeyList = keylistBuilder.ToString();
            Map = mapBuilder.ToString();
        }
        UnityWebRequest GetRequest(string host,string path, string method, Dictionary<string, string> paramas, Dictionary<string,string> headers)
        {
            string 签名 = 请求签名(path, method, paramas, headers);
            UnityWebRequest webRequest = new UnityWebRequest();
            string pamarasValue = null;
            if (paramas != null && paramas.Count != 0)
            {
                int size = 0;
                foreach (var item in paramas)
                {
                    size++;
                    pamarasValue += $"{item.Key}={item.Value}";
                    if (size < paramas.Count)
                    {
                        pamarasValue += "&";
                    }
                }
            }
            if (!string.IsNullOrEmpty(pamarasValue)) 
            {
                Log.Info(pamarasValue);
                webRequest.uri = new Uri($"https://{host}/{path}?{pamarasValue}");
            }
            else
            {
                webRequest.uri = new Uri($"https://{host}/{path}");
            }
            //Log.Info("URL->" + webRequest.uri);
            webRequest.method = method;
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Authorization", 签名);
            if (headers != null)
            {
                foreach (var item in headers)
                {
                    webRequest.SetRequestHeader(item.Key, item.Value);
                }
            }
            return webRequest;
        }
        string PrintErrorXML(string xml)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            var code = xmlDoc.SelectSingleNode("Error/Code").InnerText;
            var message = xmlDoc.SelectSingleNode("Error/Message").InnerText;
            var errorString = $"[{code}]{message}";
            Log.Error(errorString);
            return errorString;
        }


        /// <summary>
        /// 存储桶是否存在，是否有权限访问
        /// </summary>
        public async ATask<bool> HEADBucket()
        {
            UnityWebRequest webRequest = GetRequest(Host, "", UnityWebRequest.kHttpVerbHEAD, default, default);
            await webRequest.SendWebRequest();
            switch (webRequest.responseCode)
            {
                case 200:
                    Log.Info("存储桶存在且有读取权限");
                    return true;
                case 403:
                    Log.Error("无存储桶读取权限");
                    return false;
                case 404:
                    Log.Error("存储桶不存在");
                    return false;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 列出该存储桶内的部分或者全部对象
        /// </summary>
        /// <param name="bucket">存储桶的名字(不包括-appid)</param>
        /// <param name="region">区域</param>
        public async ATask<IList<string>> GETBucket()
        {
            List<string> output = new List<string>();
            UnityWebRequest webRequest = GetRequest(Host, "", UnityWebRequest.kHttpVerbGET, default, default);
            DownloadHandler download = await webRequest.SendWebRequest();
            if (!string.IsNullOrEmpty(download.error))
            {
                PrintErrorXML(download.text);
            }
            else
            {
                var xml = download.text;
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                var list = xmlDoc.SelectNodes("ListBucketResult/Contents/Key");
                var count = list.Count;
            
                for (int i = 0; i < count; i++)
                {
                    Log.Info(list.Item(i).InnerText);
                    output.Add(list.Item(i).InnerText);
                }
            }
            return output;
        }

        /// <summary>
        /// 将本地的对象（Object）上传至指定存储桶中  savePath以下划线开头,相对于存储桶的完整路径  如/foloder1/foloder2/*.*
        /// </summary>
        public async ATask<bool> PUTObject(string filePath, byte[] uploadBytes)
        {
            var header = new Dictionary<string, string>() // HTTP 请求头部
            {
                ["Content-Type"] = "application/octet-stream", //image/jpeg  text/html video/mp4 ...  //暂时默认二进制文件
                ["Content-Length"] = uploadBytes.Length.ToString(),
                ["Content-MD5"] = MD5Helper.BytesMD5Base64(uploadBytes),
            };
            UnityWebRequest webRequest = GetRequest(Host, filePath, UnityWebRequest.kHttpVerbPUT, default, header);
            webRequest.uploadHandler = new UploadHandlerRaw(uploadBytes);
            await webRequest.SendWebRequest();
            if (!string.IsNullOrEmpty(webRequest.error))
            {
                PrintErrorXML(webRequest.downloadHandler.text);
                return false;
            }
            else
            {
                Log.Info("PUTObject Success");
                return true;
            }
        }

        /// <summary>
        /// 判断指定对象是否存在和有权限，并在指定对象可访问时获取其元数据
        /// </summary>
        public async ATask<(bool,int)> HEADObject(string filePath)
        {
            UnityWebRequest webRequest = GetRequest(Host, filePath, UnityWebRequest.kHttpVerbHEAD, default, default);
            await webRequest.SendWebRequest();
            switch (webRequest.responseCode)
            {
                case 200:
                    int size = int.Parse(webRequest.GetResponseHeader("Content-Length"));
                    Log.Info($"对象存在且有读取权限,Content-Length={size}");
                    return (true, size);
                case 403:
                    Log.Error("无对象读取权限");
                    break;
                case 404:
                    Log.Error("对象不存在");
                    break;
            }
            return (false, 0);
        }

        /// <summary>
        /// 将 COS 存储桶中的对象（Object）下载至本地
        /// </summary>
        public async ATask<byte[]> GETObject(string filePath)
        {
            UnityWebRequest webRequest = GetRequest(Host, filePath, UnityWebRequest.kHttpVerbGET, default, default);
            await webRequest.SendWebRequest();
            if (!string.IsNullOrEmpty(webRequest.error))
            {
                PrintErrorXML(webRequest.downloadHandler.text);
                return null;
            }
            else
            {
                Log.Info("GETObject Success");
                return webRequest.downloadHandler.data;
            }
        }
        /// <summary>
        /// 分块下载对象  在下载前先head获取对象长度  然后自己分块进行下载  first 和 last 都是基于0开始的偏移量
        /// bytes=-10表示下载对象的最后10个字节的数据。
        /// bytes=10-表示下载对象的第10到最后字节的数据。
        /// bytes=0-表示下载对象的第一个字节到最后一个字节，即完整的文件内容。
        /// </summary>
        public async ATask<byte[]> GETObjectRange(string filePath, string first,string last)
        {
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                ["Range"] = $"bytes={first}-{last}"
            };
            UnityWebRequest webRequest = GetRequest(Host, filePath, UnityWebRequest.kHttpVerbGET, default, header);
            await webRequest.SendWebRequest();
            if (!string.IsNullOrEmpty(webRequest.error))
            {
                PrintErrorXML(webRequest.downloadHandler.text);
                return null;
            }
            else
            {
                Log.Info("GETObjectRange Success");
                Log.Info(webRequest.downloadHandler.data.Length);
                return webRequest.downloadHandler.data;
            }
        }

        /// <summary>
        /// 删除一个指定的对象（Object）
        /// </summary>
        public async ATask<bool> DELETEObject(string filePath)
        {
            string host = $"{Bucket}-{APPID}.cos.{Region}.myqcloud.com";
            UnityWebRequest webRequest = GetRequest(host, filePath, UnityWebRequest.kHttpVerbDELETE, default, default);
            await webRequest.SendWebRequest();
            if (!string.IsNullOrEmpty(webRequest.error))
            {
                PrintErrorXML(webRequest.downloadHandler.text);
                return false;
            }
            else
            {
                Log.Info("DELETEObject Success");
                return true;
            }
        }

        /// <summary>
        /// 一次性删除多个对象  单次最多1000个
        /// </summary>
        public async ATask<bool> DELETEMultipleObjects(string[] keys)
        {
            XmlDocument xmlDoc = new XmlDocument();
            var delete = xmlDoc.CreateElement("Delete");
            xmlDoc.AppendChild(delete);
            var quiet = xmlDoc.CreateElement("Quiet");
            quiet.InnerText = "false";
            delete.AppendChild(quiet);
            for (int i = 0; i < keys.Length; i++)
            {
                var obj = xmlDoc.CreateElement("Object");
                var key = xmlDoc.CreateElement("Key");
                key.InnerText = keys[i];
                obj.AppendChild(key);
                delete.AppendChild(obj);
            }
            Log.Info(xmlDoc.OuterXml);
            byte[] uploadBytes = Encoding.UTF8.GetBytes(xmlDoc.OuterXml);

            var paramas = new Dictionary<string, string>()//请求参数
            {
                ["delete"] = "",
            };
            var header = new Dictionary<string, string>() // HTTP 请求头部
            {
                ["Content-Type"] = "application/xml",
                ["Content-Length"] = uploadBytes.Length.ToString(),
                ["Content-MD5"] = MD5Helper.BytesMD5Base64(uploadBytes),
            };
            UnityWebRequest webRequest = GetRequest(Host, "", UnityWebRequest.kHttpVerbPOST, paramas, header);
            webRequest.uploadHandler = new UploadHandlerRaw(uploadBytes);
            await webRequest.SendWebRequest();
            if (!string.IsNullOrEmpty(webRequest.error))
            {
                PrintErrorXML(webRequest.downloadHandler.text);
                return false;
            }
            else
            {
                var xml = webRequest.downloadHandler.text;
                xmlDoc.LoadXml(xml);
                var list = xmlDoc.SelectNodes("DeleteResult/Deleted/Key");//删除成功 删除一个不存在的对象也会返回成功
                var count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    Log.Info(list.Item(i).InnerText);
                }
                var listError = xmlDoc.SelectNodes("DeleteResult/Error/Key");//删除失败的
                var countError = listError.Count;
                for (int i = 0; i < countError; i++)
                {
                    Log.Info(listError.Item(i).InnerText);
                }
                return true;
            }
        }




        /// <summary>
        /// 初始化  申请一个分片上传的ID
        /// </summary>
        public async ATask<(bool,string)> MultipartUpload_Initiate(string filePath)
        {
            var paramas = new Dictionary<string, string>()
            {
                ["uploads"] = "",
            };
            var header = new Dictionary<string, string>() // HTTP 请求头部
            {
                ["Content-Type"] = "application/octet-stream", //image/jpeg  text/html video/mp4 ...  //暂时默认二进制文件
                ["Content-Length"] = "0",
            };

            UnityWebRequest webRequest = GetRequest(Host, filePath, UnityWebRequest.kHttpVerbPOST, paramas, header);
            await webRequest.SendWebRequest();
            if (!string.IsNullOrEmpty(webRequest.error))
            {
                var error = PrintErrorXML(webRequest.downloadHandler.text);
                return (false, error);
            }
            else
            {
                Log.Info(webRequest.downloadHandler.text);
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(webRequest.downloadHandler.text);
                var uploadID =  xml.SelectSingleNode("InitiateMultipartUploadResult/UploadId").InnerText;
                return (true, uploadID);
            }
        }
        /// <summary>
        /// 分块上传到 COS。最多支持10000分块，每个分块大小为1MB - 5GB  index从1~10000
        /// </summary>
        public async ATask<(bool,string)> UploadPart(string filePath ,string uploadID, int index, byte[] bytes)
        {
            var paramas = new Dictionary<string, string>()
            {
                ["uploadId"] = uploadID,
                ["partNumber"] = index.ToString(),
            };
            var header = new Dictionary<string, string>() // HTTP 请求头部
            {
                ["Content-Type"] = "application/octet-stream", //image/jpeg  text/html video/mp4 ...  //暂时默认二进制文件
                ["Content-Length"] = bytes.Length.ToString(),
                ["Content-MD5"] = MD5Helper.BytesMD5Base64(bytes),
            };
            UnityWebRequest webRequest = GetRequest(Host, filePath, UnityWebRequest.kHttpVerbPUT, paramas, header);
            webRequest.uploadHandler = new UploadHandlerRaw(bytes);
            await webRequest.SendWebRequest();
            if (!string.IsNullOrEmpty(webRequest.error))
            {
                var error =  PrintErrorXML(webRequest.downloadHandler.text);
                return (false, error);
            }
            else
            {
                var eTag = webRequest.GetResponseHeader("ETag");
                Log.Info("ETag->" + eTag);
                return (true, eTag);
            }
        }
        /// <summary>
        /// 完成整个分块上传
        /// </summary>
        public async ATask<bool> CompleteMultipartUpload(string filePath,string uploadID,Dictionary<int,string> etags)
        {
            XmlDocument xmlDoc = new XmlDocument();
            var root = xmlDoc.CreateElement("CompleteMultipartUpload");
            xmlDoc.AppendChild(root);
            foreach (var item in etags)//etags 是编号和分块etag的集合  合并的时候需要提交上去做验证  etags需要按顺序排列!
            {
                var Part = xmlDoc.CreateElement("Part");
                var PartNumberart = xmlDoc.CreateElement("PartNumber");
                PartNumberart.InnerText = item.Key.ToString();
                var ETag = xmlDoc.CreateElement("ETag");
                ETag.InnerText = item.Value;
                Part.AppendChild(PartNumberart);
                Part.AppendChild(ETag);
                root.AppendChild(Part);
            }
            var bytes = Encoding.UTF8.GetBytes(xmlDoc.OuterXml);

            string host = $"{Bucket}-{APPID}.cos.{Region}.myqcloud.com";
            var paramas = new Dictionary<string, string>()
            {
                ["uploadId"] = uploadID,
            };
            var header = new Dictionary<string, string>()
            {
                ["Content-Type"] = "application/xml",
                ["Content-Length"] = bytes.Length.ToString(),
                ["Content-MD5"] = MD5Helper.BytesMD5Base64(bytes),
            };
            UnityWebRequest webRequest = GetRequest(host, filePath, UnityWebRequest.kHttpVerbPOST, paramas, header);
            webRequest.uploadHandler = new UploadHandlerRaw(bytes);
            await webRequest.SendWebRequest();
            if (!string.IsNullOrEmpty(webRequest.error))
            {
                PrintErrorXML(webRequest.downloadHandler.text);
                return false;
            }
            else
            {
                Log.Info(webRequest.downloadHandler.text);
                return true;
            }
        }

        /// <summary>
        /// 查询正在进行中的分块上传任务 最多列出1000个
        /// </summary>
        public async ATask<IList<string>> ListMultipartUploads()
        {
            List<string> output = new List<string>();
            string host = $"{Bucket}-{APPID}.cos.{Region}.myqcloud.com";
            var paramas = new Dictionary<string, string>()
            {
                ["uploads"] = "",
            };
            UnityWebRequest webRequest = GetRequest(host, "", UnityWebRequest.kHttpVerbGET, paramas, default);
            await webRequest.SendWebRequest();
            if (!string.IsNullOrEmpty(webRequest.error))
            {
                PrintErrorXML(webRequest.downloadHandler.text);
            }
            else
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(webRequest.downloadHandler.text);
                var list = xmlDoc.SelectNodes("ListMultipartUploadsResult/Upload");
                if (list.Count == 0)
                {
                    Log.Info("没有正在进行的分块上传任务");
                }
                else
                {
                    foreach (XmlNode node in list)
                    {
                        var key = node.SelectSingleNode("Key").InnerText;
                        var uploadID = node.SelectSingleNode("UploadId").InnerText;
                        Log.Info(key + " : " + uploadID);
                        output.Add(uploadID);
                    }
                }
            }
            return output;
        }

        /// <summary>
        /// 舍弃一个分块上传并删除已上传的块
        /// </summary>
        public async ATask AbortMultipartUpload(string filePath ,string uploadID)
        {
            var paramas = new Dictionary<string, string>()
            {
                ["uploadId"] = uploadID,
            };
            UnityWebRequest webRequest = GetRequest(Host, filePath, UnityWebRequest.kHttpVerbDELETE, paramas, default);
            await webRequest.SendWebRequest();
            if (!string.IsNullOrEmpty(webRequest.error))
            {
                PrintErrorXML(webRequest.downloadHandler.text);
            }
            else
            {
                Log.Info("AbortMultipartUpload Success");
            }
        }
        /// <summary>
        /// 查询特定分块上传中的已上传的块  列出所有已上传成功的
        /// </summary>
        public async ATask ListParts(string filePath,string uploadID)
        {
            string host = $"{Bucket}-{APPID}.cos.{Region}.myqcloud.com";
            var paramas = new Dictionary<string, string>()
            {
                ["uploadId"] = uploadID,
            };
            UnityWebRequest webRequest = GetRequest(host, filePath, UnityWebRequest.kHttpVerbGET, paramas, default);
            await webRequest.SendWebRequest();
            if (!string.IsNullOrEmpty(webRequest.error))
            {
                PrintErrorXML(webRequest.downloadHandler.text);
            }
            else
            {
            }
            Log.Info(webRequest.downloadHandler.text);
            //XML... 只返回成功的part
            //ListPartsResult/Part/PartPartNumber
            //ListPartsResult/Part/ETag
            //ListPartsResult/Part/Size
        }
    }
}
