using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace deVoid.UIFramework.Examples
{
    public class TestPanelChildController : TestPanelController
    {
        protected override void OnPropertiesSet()
        {
            Debug.Log($"{Properties.text}", "TEST");
        }
    }
}