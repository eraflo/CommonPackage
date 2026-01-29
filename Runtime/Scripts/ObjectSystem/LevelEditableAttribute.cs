using System;

namespace Eraflo.Common.ObjectSystem
{
    [AttributeUsage(AttributeTargets.Field)]
    public class LevelEditableAttribute : Attribute
    {
        public bool ShowInInspector { get; set; } = true;

        public LevelEditableAttribute(bool showInInspector = true)
        {
            ShowInInspector = showInInspector;
        }
    }
}