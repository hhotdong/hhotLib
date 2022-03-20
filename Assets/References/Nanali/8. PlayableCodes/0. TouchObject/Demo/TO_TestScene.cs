//    using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TO_TestScene : MonoBehaviour
//{
//    public int myLevel = 3; //랜드마크 레벨. (테스트)

//    public TO_TestObject objectPrefab;
//    TouchObjectManager manager;

//    //test ui.
//    public TO_TestProgress pgBar;
//    public GameObject[] Go_MapActive;

//    //initialize.
//    private void Start()
//    {
//        manager = TouchObjectManager.Instance;

//        //맵 세팅.
//        for (int i = 0; i < Go_MapActive.Length; i++)
//            Go_MapActive[i].SetActive(i < myLevel);

//        //매니저 초기화.
//        manager.Initialize(myLevel);

//        //데이터 로드.
//        manager.Load(Create, OnProgress, (positions, remainSeconds) =>
//        {
//            for (int i = 0; i < positions.Length; i++)
//            {
//                //객체 생성.
//                Create(positions[i]);
//            }
//        });
//    }

//    //progress.
//    void OnProgress(float percent)
//    {
//        pgBar.SetProgressValue(percent);
//    }

//    //객체 생성.
//    void Create(TouchObjectPosition position, bool autoCreate = false)
//    {
//        //생성할 자리가 있을때만 진행.
//        if (position == null)
//            return;
//        //객체 생성.
//        TO_TestObject obj = Instantiate(objectPrefab) as TO_TestObject;
//        //객체에 포지션 키값 부여.
//        obj.Initialize(position, UserTouchCallback);
//        //매니저에 객체 부여.
//        //manager.SetObject(position.Code, obj, autoCreate);
//    }

//    //객체 터치시 콜백.
//    public void UserTouchCallback(string code)
//    {
//        //매니저에 터치여부 전달.
//        manager.Touch(code);
//    }

//    //test.
//    public void LevelUp()
//    {
//        myLevel++;
//        manager.RefreshPositions(myLevel);

//        //맵 세팅.
//        for (int i = 0; i < Go_MapActive.Length; i++)
//            Go_MapActive[i].SetActive(i < myLevel);
//    }
//}
