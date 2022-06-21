
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace ZFramework
{
    //生命周期
    public class View_LoginAwake : AwakeSystem<View_Login_Component>
    {
        public override void Awake(View_Login_Component component)
        {
            var btn0 = component.Refs.Get<TextMeshProUGUI>("btn0");
            var btn1 = component.Refs.Get<TextMeshProUGUI>("btn1");
            ZEvent.UIEvent.AddListener(btn0, component.Btn, 0);
            ZEvent.UIEvent.AddListener(btn1, component.Btn, 1);
            component.localMod = -1;

            ZEvent.UIEvent.AddListener(component.Refs.Get<GameObject>("Button0"), component.EnterBtn, 0);
            ZEvent.UIEvent.AddListener(component.Refs.Get<GameObject>("Button1"), component.EnterBtn, 1);
        }
    }

    public class View_Login_Show : UIShowSystem<View_Login_Component>
    {
        public override void OnShow(View_Login_Component canvas)
        {
            canvas.gameObject.SetActive(true);
            canvas.SetMod(0);
        }
    } 

    public class View_Login_Hide : UIHideSystem<View_Login_Component>
    {
        public override void OnHide(View_Login_Component canvas)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    //逻辑
    public static class View_Login_System
    {
        public static void SetMod(this View_Login_Component component, int mod)
        {
            if (component.localMod == mod) return;
            component.localMod = mod;
            component.Refs.Get<TMP_InputField>("input0").gameObject.SetActive(mod == 0);
            component.Refs.Get<TMP_InputField>("input1").gameObject.SetActive(mod == 0);
            component.Refs.Get<TMP_InputField>("input2").gameObject.SetActive(mod == 1);
            component.Refs.Get<TMP_InputField>("input3").gameObject.SetActive(mod == 1);
            component.Refs.Get<TMP_InputField>("input4").gameObject.SetActive(mod == 1);
            component.Refs.Get<GameObject>("Button0").gameObject.SetActive(mod == 0);
            component.Refs.Get<GameObject>("Button1").gameObject.SetActive(mod == 1);

            switch (mod)
            {
                case 0:
                    component.Refs.Get<RectTransform>("line").anchoredPosition = new Vector2(-80, 0);
                    component.Refs.Get<TMP_InputField>("input0").text = "";
                    component.Refs.Get<TMP_InputField>("input1").text = "";
                    
                    break;
                case 1:
                    component.Refs.Get<RectTransform>("line").anchoredPosition = new Vector2(80, 0);
                    component.Refs.Get<TMP_InputField>("input2").text = "";
                    component.Refs.Get<TMP_InputField>("input3").text = "";
                    component.Refs.Get<TMP_InputField>("input4").text = "";
                    break;
            }
        }
        public static void Btn(this View_Login_Component component, UIEventData<int> eventData)
        {
            switch (eventData.EventType)
            {
                case UIEventType.Click:
                    component.SetMod(eventData.Data0);
                    break;
            }
        }

        public static void EnterBtn(this View_Login_Component component, UIEventData<int> eventData)
        {
            switch (eventData.EventType)
            {
                case UIEventType.Click:
                    switch (eventData.Data0)
                    {
                        case 0:
                            {
                                var username = component.Refs.Get<TMP_InputField>("input0").text;
                                var password = component.Refs.Get<TMP_InputField>("input1").text;
                                Login(username, password);
                            }
                            break;
                        case 1:
                            {
                                var username = component.Refs.Get<TMP_InputField>("input2").text;
                                var password = component.Refs.Get<TMP_InputField>("input3").text;
                                var realname = component.Refs.Get<TMP_InputField>("input4").text;
                                Reg(username, password, realname);
                            }
                            break;
                    }
                    break;
            }
        }

        public static async void Login(string username, string password)
        {
            if (Define.IsEditor)
            {
                Game.UI.Hide(UIType.View_Login);
                UnityEngine.SceneManagement.SceneManager.LoadScene("Scene01_Main");
                Game.UI.Show(UIType.View_Top);
                Game.UI.Show(UIType.View_Left);


                return;

                if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
                {
                    Login("admin", "admin");
                    return;
                }
            }

           

            if (string.IsNullOrEmpty(username))
            {
                (Game.Root.GetComponent<UIComponent>().Show(UIType.View_Message) as View_Message_Component).ShowInfo("提示", "用户名不能为空", (Action)null);
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                (Game.Root.GetComponent<UIComponent>().Show(UIType.View_Message) as View_Message_Component).ShowInfo("提示", "密码不能为空", (Action)null);
                return;
            }

            var json = new JsonData() {
                ["username"] = username,
                ["password"] = password,
            }.ToJson();
            var callback = await Game.Root.GetComponent<HttpComponent>().SendPostAsync<HttpCallback>(HttpComponent.HTTPURP + "login",json);

            if (callback.success)
            {
                Log.Info(callback.msg);
                var jsonData = callback.msg.ToObject();
                if ((int)jsonData["status"] == 200)
                {
                    BootStrap.Account = jsonData["data"].ToObject<Account>();

                    Game.UI.Hide(UIType.View_Login);
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Scene01_Main");
                    Game.UI.Show(UIType.View_Top);
                    Game.UI.Show(UIType.View_Left);
                }
                else
                {
                    (Game.Root.GetComponent<UIComponent>().Show(UIType.View_Message) as View_Message_Component).ShowInfo("登录失败", (string)jsonData["msg"], (Action)null);
                }
            }
            else
            {
                (Game.Root.GetComponent<UIComponent>().Show(UIType.View_Message) as View_Message_Component).ShowInfo("登录失败", callback.msg, (Action)null);
            }
        }
        public static async void Reg(string username,string password,string realname) 
        {
            if (string.IsNullOrEmpty(username))
            {
                (Game.Root.GetComponent<UIComponent>().Show(UIType.View_Message) as View_Message_Component).ShowInfo("提示", "用户名不能为空", (Action)null);
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                (Game.Root.GetComponent<UIComponent>().Show(UIType.View_Message) as View_Message_Component).ShowInfo("提示", "密码不能为空", (Action)null);
                return;
            }
            if (string.IsNullOrEmpty(realname))
            {
                (Game.Root.GetComponent<UIComponent>().Show(UIType.View_Message) as View_Message_Component).ShowInfo("提示", "姓名不能为空", (Action)null);
                return;
            }
            var json = new JsonData()
            {
                ["username"] = username,
                ["password"] = password,
                ["realname"] = realname,
                ["departmentid"] = "0",
                ["postid"] = "0",
                ["roleid"] = 0,
                ["sex"] = 0,
            }.ToJson();
            var callback = await Game.Root.GetComponent<HttpComponent>().SendPostAsync<HttpCallback>(HttpComponent.HTTPURP + "user/add", json);

            if (callback.success)
            {
                var jsonData = callback.msg.ToObject();
                if ((int)jsonData["status"] == 200)
                {
                    (Game.Root.GetComponent<UIComponent>().Show(UIType.View_Message) as View_Message_Component).ShowInfo("注册成功", (string)jsonData["msg"], (Action)null);
                }
                else
                {
                    (Game.Root.GetComponent<UIComponent>().Show(UIType.View_Message) as View_Message_Component).ShowInfo("注册失败", (string)jsonData["msg"], (Action)null);
                }
            }
            else
            {
                (Game.Root.GetComponent<UIComponent>().Show(UIType.View_Message) as View_Message_Component).ShowInfo("注册失败", callback.msg, (Action)null);
            }
        }
    }
}

