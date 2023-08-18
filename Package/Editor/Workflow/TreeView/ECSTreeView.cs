using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ZFramework.Editor
{
    public class ECSTreeView : TreeView
    {
        public ECSTreeView(TreeViewState treeViewState) : base(treeViewState)
        {
            Reload();
        }

        ECSTreeViewItem root;

        protected override TreeViewItem BuildRoot()
        {
            if (root == null) 
            {
                root = new ECSTreeViewItem { id = 0, depth = -1, displayName = "Root" };
                var allItems = new List<TreeViewItem>
                {
                    new ECSTreeViewItem {id = 1, depth = 0, displayName = "Animals"},
                    new ECSTreeViewItem {id = 2, depth = 1, displayName = "Mammals"},
                    new ECSTreeViewItem {id = 3, depth = 2, displayName = "Tiger"},
                    new ECSTreeViewItem {id = 4, depth = 2, displayName = "Elephant"},
                    new ECSTreeViewItem {id = 5, depth = 2, displayName = "Okapi"},
                    new ECSTreeViewItem {id = 6, depth = 2, displayName = "Armadillo"},
                    new ECSTreeViewItem {id = 7, depth = 1, displayName = "Reptiles"},
                    new ECSTreeViewItem {id = 8, depth = 2, displayName = "Crocodile"},
                    new ECSTreeViewItem {id = 9, depth = 2, displayName = "Lizard"},
                };
                SetupParentsAndChildrenFromDepths(root, allItems);
            }
            return root;
        }
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            return base.BuildRows(root);
        }

        //一堆事件
        protected override void ContextClickedItem(int id)
        {
            base.ContextClickedItem(id);
        }
        protected override void SelectionChanged(IList<int> selectedIds)
        {
            foreach (var item in selectedIds)
            {
                Log.Info(item);
            }
            base.SelectionChanged(selectedIds);
        }
        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }
        protected override void ContextClicked()
        {
            base.ContextClicked();
            Log.Info(Event.current.button);
            if (Event.current.button == 1)//1是鼠标右键
            {
                ShowGenericMenu();
            }
        }
        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            Log.Info(id);
        }
        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            Log.Info(args.dragAndDropPosition);
            return DragAndDropVisualMode.Move;// base.HandleDragAndDrop(args);
        }
        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            Log.Info(args.draggedItemIDs.Count);
            //base.SetupDragAndDrop(args);
        }
        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            return true;
            return base.CanStartDrag(args);
        }
        protected override void DoubleClickedItem(int id)
        {
            Log.Info("double click:" + id);
        }
        protected override bool CanRename(TreeViewItem item)
        {
            return true;
            return base.CanRename(item);
        }
        protected override void RenameEnded(RenameEndedArgs args)
        {
            Log.Info(args.newName);
            //base.RenameEnded(args);
        }
        protected override void SearchChanged(string newSearch)
        {
            base.SearchChanged(newSearch);
        }
        protected override bool DoesItemMatchSearch(TreeViewItem item, string search)
        {
            return base.DoesItemMatchSearch(item, search);
        }
        protected override void CommandEventHandling()
        {
            //Log.Info(1);
            base.CommandEventHandling();
        }
        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);
        }
        protected override void KeyEvent()
        {
            //Log.Info(Event.current.keyCode = Event.current.keyCode);
            base.KeyEvent();
        }


        void ShowGenericMenu()
        {
            GenericMenu menu = new GenericMenu(); //初始化GenericMenu 
            menu.AddItem(new GUIContent("Red"), false, ChangeColorRed); //向菜单中添加菜单项
            menu.AddItem(new GUIContent("Blue"), false, ChangeColorBlue);
            menu.AddItem(new GUIContent("Yellow"), false, ChangeColorYellow);
            menu.AddSeparator(""); //分隔符
            menu.AddItem(new GUIContent("11/22"), false, A, "123");
            menu.AddItem(new GUIContent("11/33"), false, A, "456");


            menu.ShowAsContext(); //显示菜单
        }

        void ChangeColorRed()
        {
        }
        void ChangeColorBlue()
        {
        }
        void ChangeColorYellow()
        {
        }
        void A(object e)
        { 

        }
    }
}
