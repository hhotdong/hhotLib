using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace hhotLib.Common
{
    /// <summary>
    /// Removed immediately after initializing.
    /// </summary>
    public abstract class ReferencePoint_Initializer<T> : ReferencePoint where T : ReferencePoint
    {
        protected sealed override void OnAwake()
        {
            isPointUpdatable = false;
            Initialize();
        }

        private void Start()
        {
            Destroy(gameObject);
        }

        protected abstract void Initialize();

        protected sealed override void OnUpdate()   {}
        protected sealed override void OnDispose()  {}
        protected sealed override void Register()   {}
        protected sealed override void Unregister() {}
        protected sealed override void UpdatePoint(Type t) { return; }
    }

    /// <summary>
    /// Allow the same identification of point.
    /// </summary>
    public abstract class ReferencePoint_General<T> : ReferencePoint where T : ReferencePoint
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            if (points.ContainsKey(typeof(T)) == false)
                points.Add(typeof(T), new List<ReferencePoint>());
        }

        protected sealed override void Register()
        {
            if (points[typeof(T)].Contains(this))
            {
                Debug.LogWarning($"Failed to Register! ReferencePoint({gameObject.name}) instance is already registered!");
                return;
            }
            points[typeof(T)].Add(this);

            if (isPointUpdatable)
                OnUpdatePoints += UpdatePoint;
        }

        protected sealed override void Unregister()
        {
            if (points.ContainsKey(typeof(T)) && points[typeof(T)].Contains(this))
                points[typeof(T)].Remove(this);

            if (isPointUpdatable)
                OnUpdatePoints -= UpdatePoint;
        }

        protected sealed override void UpdatePoint(Type t)
        {
            if (t != typeof(T) || isPointUpdatable == false || tr == null)
                return;

            position = tr.position;
            rotation = tr.rotation;
            scale    = tr.localScale;

            OnUpdate();
        }
    }

    /// <summary>
    /// Prohibit the same identification of point.
    /// </summary>
    public abstract class ReferencePoint_Unique<T> : ReferencePoint where T : ReferencePoint
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            if (points.ContainsKey(typeof(T)) == false)
                points.Add(typeof(T), new List<ReferencePoint>());
        }

        protected sealed override void Register()
        {
            if (TryGetPoint(IsSame, out T pt))
            {
                Debug.LogWarning($"Failed to Register! ReferencePoint({gameObject.name}) is already registered with the same information!");
                return;
            }

            if (points[typeof(T)].Contains(this))
            {
                Debug.LogWarning($"Failed to Register! ReferencePoint({gameObject.name}) instance is already registered!");
                return;
            }
            points[typeof(T)].Add(this);

            if (isPointUpdatable)
                OnUpdatePoints += UpdatePoint;
        }

        protected sealed override void Unregister()
        {
            if (points.ContainsKey(typeof(T)) && points[typeof(T)].Contains(this))
                points[typeof(T)].Remove(this);

            if (isPointUpdatable)
                OnUpdatePoints -= UpdatePoint;
        }

        protected abstract bool IsSame(T t);

        protected sealed override void UpdatePoint(Type t)
        {
            if (t != typeof(T) || isPointUpdatable == false || tr == null)
                return;

            position = tr.position;
            rotation = tr.rotation;
            scale    = tr.localScale;

            OnUpdate();
        }
    }

    public abstract class ReferencePoint : MonoBehaviour
    {
        protected Transform  tr;
        protected Vector3    position;
        protected Quaternion rotation;
        protected Vector3    scale;

        public Vector3    Position => position;
        public Quaternion Rotation => rotation;
        public Vector3    Scale    => scale;

        [SerializeField] protected bool isPointUpdatable = true;

        protected static Action<Type> OnUpdatePoints;

        protected static readonly Dictionary<Type, List<ReferencePoint>> points = new Dictionary<Type, List<ReferencePoint>>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BeforeSceneLoad()
        {
            Clear();
        }

        private void Awake()
        {
            tr       = transform;
            position = tr.position;
            rotation = tr.rotation;
            scale    = tr.localScale;
            OnAwake();
            Register();
        }

        private void OnDestroy()
        {
            Unregister();
            OnDispose();
            tr       = null;
            position = Vector3.zero;
            rotation = Quaternion.identity;
            scale    = Vector3.zero;
        }

        protected virtual  void OnAwake()   { }
        protected virtual  void OnUpdate()  { }
        protected virtual  void OnDispose() { }
        protected abstract void Register();
        protected abstract void Unregister();
        protected abstract void UpdatePoint(Type t);

        public static bool TryGetRandomPoint<T>(Func<T, bool> predicate, out T point) where T : ReferencePoint
        {
            if (TryGetPoints(out IEnumerable<T> pts))
            {
                List<T> list = null;
                if (predicate == null)
                {
                    list  = pts.ToList();
                    point = list[UnityEngine.Random.Range(0, list.Count)];
                    return true;
                }

                list = pts.Where(predicate).ToList();
                if (list.Count < 1)
                {
                    point = null;
                    return false;
                }

                point = list[UnityEngine.Random.Range(0, list.Count)];
                return true;
            }
            point = null;
            return false;
        }

        public static bool TryGetPoint<T>(Func<T, bool> predicate, out T point) where T : ReferencePoint
        {
            if (TryGetPoints(out IEnumerable<T> pts))
            {
                if (predicate == null)
                {
                    var arr = pts.ToList();
                    point   = arr[UnityEngine.Random.Range(0, arr.Count)];
                    return true;
                }

                var result = pts.FirstOrDefault(predicate);
                if (result != default(T))
                {
                    point = result;
                    return true;
                }
            }
            point = null;
            return false;
        }

        public static bool TryGetPoints<T>(out IEnumerable<T> pts) where T : ReferencePoint
        {
            if (points.TryGetValue(typeof(T), out List<ReferencePoint> result))
            {
                pts = result.Cast<T>();
                return true;
            }
            pts = null;
            return false;
        }

        public static void UpdateAllPoints()
        {
            foreach(var kvp in points)
            {
                Type type = kvp.Key;
                OnUpdatePoints?.Invoke(type);
            }
        }

        public static void UpdatePoints<T>() where T : ReferencePoint
        {
            OnUpdatePoints?.Invoke(typeof(T));
        }

        public static void Clear()
        {
            Debug.Log($"Clear all ReferencePoints.");
            points.Clear();
        }
    }
}