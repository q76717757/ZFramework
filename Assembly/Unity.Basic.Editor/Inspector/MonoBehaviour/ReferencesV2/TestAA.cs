using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace ZFramework.Editor
{
    public class WeekdaysDropdown : AdvancedDropdown
    {
        public WeekdaysDropdown(AdvancedDropdownState state) : base(state)
        {
        }
        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Fuck");

            var firstHalf = new AdvancedDropdownItem("First half");
            firstHalf.AddSeparator();
            var secondHalf = new AdvancedDropdownItem("Second half");
            var weekend = new AdvancedDropdownItem("Weekend");

            firstHalf.AddChild(new AdvancedDropdownItem("Monday"));
            firstHalf.AddChild(new AdvancedDropdownItem("Tuesday"));
            secondHalf.AddChild(new AdvancedDropdownItem("Wednesday"));
            secondHalf.AddChild(new AdvancedDropdownItem("Thursday"));
            weekend.AddChild(new AdvancedDropdownItem("Friday"));
            weekend.AddChild(new AdvancedDropdownItem("Saturday"));
            weekend.AddChild(new AdvancedDropdownItem("Sunday"));

            root.AddChild(firstHalf);
            root.AddChild(secondHalf);
            root.AddChild(weekend);
            return root;
        }
        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);
            Debug.Log(item.name);
        }
    }

}
