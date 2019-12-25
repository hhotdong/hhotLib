using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hhotLib
{
    public class MyFunc
    {
        public void RotateAround(Vector3 center, Vector3 axis, float angle)
        {
            Vector3 pos = this.transform.position;
            Quaternion rot = Quaternion.AngleAxis(angle, axis);
            Vector3 dir = pos - center;                                     // find current direction relative to center
            dir = rot * dir;                                                // rotate the direction
            this.transform.position = center + dir;                         // define new position
            transform.rotation *= rot;
        }
    }
}
