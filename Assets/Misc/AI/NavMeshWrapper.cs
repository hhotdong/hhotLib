using UnityEngine;
using UnityEngine.AI;

namespace hhotLib.AI
{
    public enum NavMeshType
    {
        NONE       = -2,
        ALL        = -1,
        NAVMESH_0  = 0,
        NAVMESH_1  = 1,
        NAVMESH_2  = 2,
        NAVMESH_3  = 3,
        NAVMESH_4  = 4,
        NAVMESH_5  = 5,
        NAVMESH_6  = 6
    }

    [RequireComponent(typeof(NavMeshSurface))]
    public class NavMeshWrapper : MonoBehaviour
    {
        [SerializeField] private NavMeshType navMeshType = NavMeshType.NONE;
        public NavMeshType NavMeshType => navMeshType;

        private NavMeshSurface navMeshSurface;
        public NavMeshSurface NavMeshSurface
        {
            get
            {
                if (navMeshSurface == null)
                    navMeshSurface = GetComponent<NavMeshSurface>();
                return navMeshSurface;
            }
        }

        private void OnEnable()
        {
            NavMeshManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            NavMeshManager.Instance.Unregister(this);
        }
    }
}