using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hhotLib.Common
{
    public class DependentSceneLoader : MonoBehaviour
    {
        [SerializeField] private List<string> sceneNames;

        private IEnumerator Start()
        {
            for (int i = 0; i < sceneNames.Count; i++)
            {
#if !(UNITY_EDITOR || DEVELOPMENT_BUILD)
                if (sceneNames[i].Contains(SceneName.DEBUG))
                    continue;
#endif
                SceneLoader.Instance.Load(sceneNames[i], false);
                yield return new WaitUntil(() => SceneLoader.Instance.IsLoading == false);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            for (int i = 0; i < sceneNames.Count; i++)
                Debug.Assert(string.IsNullOrEmpty(sceneNames[i]) == false);
        }
#endif
    }
}