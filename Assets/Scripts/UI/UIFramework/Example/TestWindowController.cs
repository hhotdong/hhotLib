using System;
using UnityEngine;

namespace deVoid.UIFramework.Examples
{
    [Serializable]
    public class TestWindowProperties : WindowProperties
    {
        public string text;

        public TestWindowProperties(string text)
        {
            this.text = text;
        }
    }

    public class TestWindowController : AWindowController<TestWindowProperties>
    {
        protected override void OnPropertiesSet()
        {
            Debug.Log($"{Properties.text}", "TEST");
        }
    }
}