using System;
using UnityEngine;

namespace deVoid.UIFramework.Examples
{
    [Serializable]
    public class TestPanelProperties : PanelProperties
    {
        public string text;

        public TestPanelProperties(string text)
        {
            this.text = text;
        }
    }

    public class TestPanelController : APanelController<TestPanelProperties>
    {
        protected override void OnPropertiesSet()
        {
            Debug.Log($"{Properties.text}", "TEST");
        }
    }
}