//using System;
//using System.Collections.Generic;
//using UnityEngine;

//public class TO_TestObject : TouchObject
//{
//    public void Initialize(TouchObjectPosition position, Action<string> touchCallback)
//    {
//        //터치 콜백 등록.
//        OnTouch = touchCallback;
//        //코드 등록.
//        SetPositionCode(position.Code);
//        //객체 위치 수정.
//        transform.position = position.transform.position;
//    }

//    //유저가 터치하였음.
//    public void UserTouchListener()
//    {
//        //base에 전달.
//        Touched();

//        //보상제공.

//        //삭제.
//        Destroy(gameObject);
//    }
//}
