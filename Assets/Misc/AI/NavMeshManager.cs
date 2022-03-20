using System.Collections.Generic;
using UnityEngine.AI;
using hhotLib.Common;

namespace hhotLib.AI
{
    public class NavMeshManager : Singleton<NavMeshManager>
    {
        private readonly Dictionary<NavMeshType, List<NavMeshSurface>> navMeshSurfaces = new Dictionary<NavMeshType, List<NavMeshSurface>>();

        public static int AREA_MASK_SWIM { get; private set; }

        protected override void OnAwake()
        {
            foreach (var item in navMeshSurfaces) item.Value?.Clear();
            navMeshSurfaces.Clear();

            AREA_MASK_SWIM = 1 << NavMesh.GetAreaFromName("Swim");
        }

        private void OnDestroy()
        {
            foreach (var item in navMeshSurfaces) item.Value?.Clear();
            navMeshSurfaces.Clear();
        }

        public void Register(NavMeshWrapper wrapper)
        {
            if(navMeshSurfaces.TryGetValue(wrapper.NavMeshType, out List <NavMeshSurface> list))
            {
                if (list == null)
                    list = new List<NavMeshSurface>();

                if (list.Contains(wrapper.NavMeshSurface))
                    list.Add(wrapper.NavMeshSurface);
                else
                    Debug.LogWarning($"This NavMeshSurface is already registered!");
            }
        }

        public void Unregister(NavMeshWrapper wrapper)
        {
            if (navMeshSurfaces.TryGetValue(wrapper.NavMeshType, out List<NavMeshSurface> list))
            {
                if (list == null || list.Count < 1)
                {
                    Debug.LogWarning("You tried to remove NavMeshSurface from the list which is null or no item!");
                    return;
                }

                if (list.Contains(wrapper.NavMeshSurface))
                    list.Remove(wrapper.NavMeshSurface);
                else
                    Debug.LogWarning($"Failed to remove this NavMeshSurface : Not registered!");
            }
        }

        public void BakeNavMesh(NavMeshType type)
        {
            if (type == NavMeshType.NONE)
            {
                Debug.LogWarning($"Failed to bake navmesh : Invalid type({type.ToString()})!");
                return;
            }

            if(type == NavMeshType.ALL)
            {
                foreach (var item in navMeshSurfaces)
                {
                    var navMesheslist = item.Value;
                    if (navMesheslist?.Count > 0)
                    {
                        for (int i = 0; i < navMesheslist.Count; i++)
                        {
                            navMesheslist[i].RemoveData();
                            navMesheslist[i].BuildNavMesh();
                        }
                    }
                }
                return;
            }

            if (navMeshSurfaces.TryGetValue(type, out List<NavMeshSurface> list))
            {
                if (list?.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].RemoveData();
                        list[i].BuildNavMesh();
                    }
                }
                else
                    Debug.LogWarning($"The list of type({type.ToString()}) exists but no elements or null!");
            }
            else
                Debug.LogWarning($"The list of type({type.ToString()}) doesn't exist!");
        }
    }
}
