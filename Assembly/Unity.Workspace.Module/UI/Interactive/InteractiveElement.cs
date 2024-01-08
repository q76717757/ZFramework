using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ZFramework
{
    //https://zhuanlan.zhihu.com/p/141734324 组件分类
    public abstract class InteractiveElement : UIBehaviour
    {
        [SerializeField]
        private bool m_Interactable = true;
        [SerializeField]
        private bool m_IgnoreModal = false;
        private bool m_GroupsAllowInteraction = true;
        private readonly List<CanvasGroup> m_CanvasGroupCache = new List<CanvasGroup>();
        private UIWindowLinker link;


        public bool InModal
        {
            get
            {
                if (link == null)
                {
                    link = GetComponentInParent<UIWindowLinker>();
                }
                if (link == null)
                {
                    return false;
                }
                return link.window != null && link.window.InModal;
            }
        }
        public bool Interactable
        {
            get
            {
                return m_Interactable;
            }
            set
            {
                if (m_Interactable != value)
                {
                    m_Interactable = value;
                    if (!m_Interactable)
                    {
                        SetSelectedGameObject(false);
                    }
                }
            }
        }
        protected virtual bool CanInteractable()
        {
            return m_GroupsAllowInteraction && m_Interactable && (m_IgnoreModal || !InModal);
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            OnCanvasGroupChanged();
        }
        protected override void OnValidate()
        {
            base.OnValidate();
            if (isActiveAndEnabled)
            {
                if (!m_Interactable)
                {
                    SetSelectedGameObject(false);
                }
            }
        }
        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            OnCanvasGroupChanged();
            link = null;
        }
        protected override void OnCanvasGroupChanged()
        {
            m_GroupsAllowInteraction = ParentGroupAllowsInteraction();
        }
        private bool ParentGroupAllowsInteraction()
        {
            Transform parent = transform;
            while (parent != null)
            {
                parent.GetComponents(m_CanvasGroupCache);
                for (int i = 0; i < m_CanvasGroupCache.Count; i++)
                {
                    if (m_CanvasGroupCache[i].enabled && !m_CanvasGroupCache[i].interactable)
                    {
                        return false;
                    }

                    if (m_CanvasGroupCache[i].ignoreParentGroups)
                    {
                        return true;
                    }
                }
                parent = parent.parent;
            }
            return true;
        }

        protected void SetSelectedGameObject(bool isSelected)
        {
            var current = UnityEngine.EventSystems.EventSystem.current;
            if (current == null)
            {
                return;
            }
            if (isSelected)
            {
                current.SetSelectedGameObject(gameObject);
            }
            else
            {
                if (current.currentSelectedGameObject == gameObject)
                {
                    current.SetSelectedGameObject(null);
                }
            }
        }
    }
}
