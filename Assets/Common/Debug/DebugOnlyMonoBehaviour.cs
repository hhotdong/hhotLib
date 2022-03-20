using UnityEngine;

namespace hhotLib.Build
{
    public abstract class DebugOnlyMonoBehaviour : MonoBehaviour
    {
        public bool DestroyGOInRelease = false;
    }
}