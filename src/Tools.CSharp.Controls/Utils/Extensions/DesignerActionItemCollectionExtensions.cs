using System;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Tools.CSharp.Controls.Extensions
{
    public static class DesignerActionItemCollectionExtensions
    {
        public static void AddActionPropertyItem(this DesignerActionItemCollection items, string memberName, PropertyDescriptor controlPropertyDescriptor, string category)
        {
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }
            if (controlPropertyDescriptor == null)
            { throw new ArgumentNullException(nameof(controlPropertyDescriptor)); }

            items.Add(new DesignerActionPropertyItem(memberName, controlPropertyDescriptor.DisplayName, category, controlPropertyDescriptor.Description));
        }
    }
}