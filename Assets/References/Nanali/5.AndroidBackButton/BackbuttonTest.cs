//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class BackbuttonTest : MonoBehaviour
//{
//    public Transform UIParent;
//    public BackbuttonTestObject prefab;

//    int indexer = 0;

//    public void Create()
//    {
//        BackbuttonTestObject obj = Instantiate(prefab) as BackbuttonTestObject;
//        obj.transform.SetParent(UIParent);
//        obj.Initialize(indexer, ExitCallback);
//        indexer++;
//    }

//    void ExitCallback()
//    {
//        indexer--;
//    }
//}
