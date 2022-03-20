//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class BackbuttonTestObject : MonoBehaviour
//{
//    public Text Counter;
//    Action ExitCallback;

//    public void Initialize(int index, Action exitCallback)
//    {
//        ExitCallback = exitCallback;
//        transform.localPosition = Vector3.zero;

//        Counter.text = (index + 1).ToString("N0");

//        //BackbuttonManager에 등록.
//        BackbuttonManager.Instance?.Push(Die);
//    }

//    public void ExitBtn()
//    {
//        BackbuttonManager.Instance?.Pop();
//        Die();
//    }

//    void Die()
//    {
//        ExitCallback();
//        Destroy(gameObject);
//    }
//}
