//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TO_TestController : MonoBehaviour
//{
//    private void Update()
//    {
//        if (Input.GetMouseButton(0))
//        {
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            RaycastHit hit;
//            if (Physics.Raycast(ray.origin, ray.direction, out hit, 100))
//            {
//                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("UI"))
//                {
//                    TO_TestObject obj = hit.collider.GetComponent<TO_TestObject>();
//                    if (obj != null)
//                        obj.UserTouchListener();
//                }
//            }
//        }
//    }
//}
