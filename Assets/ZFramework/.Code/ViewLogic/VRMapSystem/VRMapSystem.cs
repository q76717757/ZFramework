using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace ZFramework
{
    public class VRMapAwake : OnAwakeImpl<VRMapComponent>
    {
        public override void OnAwake(VRMapComponent entity)
        {
            var pre = RefHelper.References.Get<GameObject>("地图");
            entity.transform = GameObject.Instantiate<GameObject>(pre).transform;

            entity.part = new Transform[5];
            entity.pos = new Vector3[5];
            for (int i = 0; i < 5; i++)
            {
                entity.part[i] = entity.transform.GetChild(i);
                entity.pos[i] = RefHelper.References.Get<Transform>("MapPos" + i).position;

                int index = i;
                entity.part[i].GetComponent<TouchHelper>().OnEnter += (hand) => { entity.Enter(index); };
                entity.part[i].GetComponent<TouchHelper>().OnExit += (hand) => { entity.Exit(index); };
                entity.part[i].GetComponent<TouchHelper>().OnClient += (hand) => { entity.Click(index); };
            }
            entity.Hide();
        }
    }


    public static class VRMapSystem
    {
        public static void Show(this VRMapComponent component)
        {
            if (component.isOn)
            {
                component.Hide();
                return;
            }
            component.isOn = true;
            component.transform.gameObject.SetActive(true);

            var followHead = Valve.VR.InteractionSystem.Player.instance.hmdTransform;
            var forw = followHead.forward*0.5f;
            component.transform.position = followHead.transform.position + new Vector3(forw.x, 0, forw.z) + Vector3.down*0.2f;
            component.transform.LookAt(followHead.transform.position + Vector3.down * 0.2f, Vector3.up);

            component.holding = -1;
            for (int i = 0; i < 5; i++)
            {
                var part = component.part[i];
                part.DOKill();
                part.localPosition = Vector3.zero;
            }
        }

        public static void Hide(this VRMapComponent component)
        {
            component.isOn = false;
            component.transform.gameObject.SetActive(false);
        }

        public static void Enter(this VRMapComponent component, int partIndex)
        {
            if (component.holding != -1)
            {
                component.Exit(component.holding);
            }
            component.holding = partIndex;
            var part = component.part[partIndex];
            part.DOKill();
            part.DOLocalMoveY(0.01f, 0.2f);
        }
        public static void Exit(this VRMapComponent component, int partIndex)
        {
            var part = component.part[partIndex];
            part.DOKill();
            part.DOLocalMoveY(0f, 0.2f);
        }
        public static void Click(this VRMapComponent component,int partIndex)
        {
            if (partIndex == component.holding)
            {
                component.Exit(partIndex);
                component.Process.GetComponent<VRHelperComponent>().Move(component.pos[partIndex]);
                component.holding = -1;
            }
        }

        public static void Rotation(this VRMapComponent component, float ang)
        {
            component.transform.rotation *= Quaternion.Euler(0, ang * Time.deltaTime, 0);
        }
    }
}

