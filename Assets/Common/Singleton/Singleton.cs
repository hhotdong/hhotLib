using UnityEngine;
using UnityEngine.SceneManagement;

namespace hhotLib.Common
{
    public abstract class Singleton : MonoBehaviour
    {
        // 씬 전환시에도 싱글턴 오브젝트가 유지될지 여부를 설정하는 플래그
        [SerializeField] private bool isPersistent = true;

        // 일반적으로 싱글턴은 앱 종료시 제거된다. 이 때 유니티가 임의의 순서로 오브젝트를 제거하기 때문에 싱글턴 오브젝트가 이미 제거된 시점에
        // 다른 오브젝트가 싱글턴에 접근하면 싱글턴 오브젝트가 다시 생성된다. 따라서 이를 방지하기 위한 플래그를 추가한다.
        protected static bool isQuitting = false;

        // 스레드 세이프를 위한 코드
        //protected static readonly object _lock = new object();

        protected virtual void Awake()
        {
            if (isPersistent)
            {
                DontDestroyOnLoad(this.gameObject);
            }

            OnAwake();
        }

        private void Start()
        {
            // 씬 변경시 파괴되지 않고 유지되는 오브젝트는 Awake, Start 함수를 다시 호출하지 않기 때문에 
            // 변경된 씬에서 싱글턴 클래스의 초기화가 필요한 경우 SceneManager.sceneLoaded 델리게이트를 이용한다.
            // Awake -> OnEnable -> sceneLoaded -> Start 함수순으로 실행되기에 Awake 함수 내에서 델리게이트 연결시
            // 최초 씬에서도 OnSceneLoaded 함수가 호출된다. 따라서 이를 방지하기 위해 Start 함수 내에서 델리게이트를 연결한다.
            if (isPersistent)
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
            }

            OnStart();
        }

        protected virtual void OnDestroy()
        {
            //isQuitting = true;

            if (isPersistent)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }

            OnDestroySingleton();
        }

        private void OnApplicationQuit()
        {
            isQuitting = true;
        }

        protected virtual void OnAwake() { }
        protected virtual void OnStart() { }
        protected virtual void OnDestroySingleton() { }
        protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode) { }
    }

    public abstract class Singleton<T> : Singleton where T : Singleton<T>
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (isQuitting)
                {
                    Debug.LogWarning($"[Singleton] Instance '{typeof(T)}'" +
                        " already destroyed on application quit." +
                        " Won't create again - returning null.");

                    return null;
                }

                //lock (_lock)
                {
                    if (instance != null)
                    {
                        return instance;
                    }
                    var instances = FindObjectsOfType<T>();
                    int count = instances.Length;
                    if (count > 0)
                    {
                        if (count == 1)
                        {
                            return instance = instances[0];
                        }

                        Debug.LogWarning($"[{nameof(Singleton)}<{typeof(T)}>] There should never be " +
                            $"more than one {nameof(Singleton)} of type {typeof(T)} in the scene, " +
                            $"but {count} were found. The first instance found will be used, and all others will be destroyed.");

                        for (int i = 1; i < instances.Length; i++)
                        {
                            Destroy(instances[i]);
                        }
                        return instance = instances[0];
                    }

                    Debug.Log($"[{nameof(Singleton)}<{typeof(T)}>] An instance is needed in the scene " +
                        $"and no existing instances were found, so a new instance will be created.");

                    return instance = new GameObject($"({nameof(Singleton)}){typeof(T)}").AddComponent<T>();
                }
            }
        }

        protected sealed override void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
            }
            else if (instance != this)
            {
                Destroy(this.gameObject);
            }

            base.Awake();
        }

        protected sealed override void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }

            base.OnDestroy();
        }
    }
}