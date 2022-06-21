
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ZFramework
{
    //生命周期
    public class View_PersonalCenterAwake : AwakeSystem<View_PersonalCenter_Component>
    {
        public override void Awake(View_PersonalCenter_Component component)
        {
            var canvas = component.gameObject.GetComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 10;

            component.sprs = new Sprite[2]
            {
                component.Refs.Get<Sprite>("按钮暗"),
                component.Refs.Get<Sprite>("按钮亮"),
            };
            component.imgs = new Image[2]
            {
                component.Refs.Get<Image>("Btn0"),
                component.Refs.Get<Image>("Btn1"),
            };
            ZEvent.UIEvent.AddListener(component.imgs[0], component.Btn, 0);
            ZEvent.UIEvent.AddListener(component.imgs[1], component.Btn, 1);

        }
    }

    public class View_PersonalCenter_Show : UIShowSystem<View_PersonalCenter_Component>
    {
        public override void OnShow(View_PersonalCenter_Component canvas)
        {
            canvas.gameObject.SetActive(true);
            if (BootStrap.Account == null) return;
            canvas.Refs.Get<TMP_InputField>("姓名").text = BootStrap.Account.realname;
            canvas.Refs.Get<TMP_InputField>("学号").text = BootStrap.Account.realname;
            canvas.Refs.Get<TMP_Dropdown>("性别").value = BootStrap.Account.sex;
            canvas.Refs.Get<TMP_Dropdown>("身份").value = BootStrap.Account.roleid;
            canvas.Refs.Get<TMP_InputField>("班级").text = "";
            canvas.Refs.Get<TMP_InputField>("密码").text = BootStrap.Account.password;
            canvas.Refs.Get<TMP_InputField>("电话").text = BootStrap.Account.phone;
            canvas.Refs.Get<TMP_InputField>("邮箱").text = BootStrap.Account.email;

            canvas.imgs[0].sprite = canvas.sprs[0];
            canvas.imgs[1].sprite = canvas.sprs[0];
        }
    }

    public class View_PersonalCenter_Hide : UIHideSystem<View_PersonalCenter_Component>
    {
        public override void OnHide(View_PersonalCenter_Component canvas)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    //逻辑
    public static class View_PersonalCenter_System
    {
        public static void Btn(this View_PersonalCenter_Component component, UIEventData<int> eventData)
        {
            int btnIndex = eventData.Data0;
            switch (eventData.EventType)
            {
                case UIEventType.Enter:
                    SoundHelper.MouseEnter();
                    component.imgs[btnIndex].sprite = component.sprs[1];
                    break;
                case UIEventType.Exit:
                    component.imgs[btnIndex].sprite = component.sprs[0];
                    break;
                case UIEventType.Click:
                    Game.Root.GetComponent<UIComponent>().Hide(UIType.View_PersonalCenter);
                    
                    if (eventData.Data0 == 0)//保存
                    {
                        var send = new Account()
                        {
                            id = BootStrap.Account.id,
                            realname = component.Refs.Get<TMP_InputField>("姓名").text,
                            email = component.Refs.Get<TMP_InputField>("邮箱").text,
                            roleid = component.Refs.Get<TMP_Dropdown>("身份").value,
                            password = component.Refs.Get<TMP_InputField>("密码").text,
                            phone = component.Refs.Get<TMP_InputField>("邮箱").text,
                            sex = component.Refs.Get<TMP_Dropdown>("性别").value,
                            username = BootStrap.Account.username,
                            postid = component.Refs.Get<TMP_Dropdown>("身份").value,
                        };

                        if (string.IsNullOrEmpty(send.password))
                        {
                            (Game.UI.Show(UIType.View_Message) as View_Message_Component).ShowInfo("错误", "密码不能为空", (Action)null);
                            return;
                        }

                        component.Update(send);
                    }
                    else//取消
                    {

                    }
                    break;
            }
        }

        public static async void Update(this View_PersonalCenter_Component component, Account account)
        {
            var callback = await Game.Root.GetComponent<HttpComponent>().SendPostAsync<HttpCallback>(HttpComponent.HTTPURP + "user/update", account.ToJson());
            Log.Info(callback.msg);
            if (callback.success)
            {
                var jsonData = callback.msg.ToObject();

                if ((int)jsonData["status"] == 200)
                {
                    BootStrap.Account = account;
                }
                else
                {
                    (Game.UI.Show(UIType.View_Message) as View_Message_Component).ShowInfo("错误", (string)jsonData["msg"], (Action)null);
                }
            }
            else
            {
                (Game.UI.Show(UIType.View_Message) as View_Message_Component).ShowInfo("错误", callback.msg, (Action)null);
            }
        }
    }
}
