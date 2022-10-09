using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Cysharp.Threading.Tasks
{
    public class UnityWebRequestException : Exception
    {
        public UnityWebRequest UnityWebRequest { get; }
        public UnityWebRequest.Result Result { get; }
        public string Error { get; }
        public string Text { get; }
        public long ResponseCode { get; }
        public Dictionary<string, string> ResponseHeaders { get; }

        string msg;

        public UnityWebRequestException(UnityWebRequest unityWebRequest)
        {
            this.UnityWebRequest = unityWebRequest;
            this.Result = unityWebRequest.result;
            this.Error = unityWebRequest.error;
            this.ResponseCode = unityWebRequest.responseCode;
            if (UnityWebRequest.downloadHandler != null)
            {
                if (unityWebRequest.downloadHandler is DownloadHandlerBuffer dhb)
                {
                    this.Text = dhb.text;
                }
            }
            this.ResponseHeaders = unityWebRequest.GetResponseHeaders();
        }

        public override string Message
        {
            get
            {
                if (msg == null)
                {
                    if(!string.IsNullOrWhiteSpace(Text))
                    {
                        msg = Error + Environment.NewLine + Text;
                    }
                    else
                    {
                        msg = Error;
                    }
                }
                return msg;
            }
        }
    }
}