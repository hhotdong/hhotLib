using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ReferecePoint 정보를 기반으로 초기화 수행 후 즉시 제거되는 일회성 클래스
/// </summary>
public abstract class ReferencePoint_Initializer<T> : ReferencePoint where T : ReferencePoint
{
	protected sealed override void OnAwake()
	{
		m_IsPointUpdatable = false;
		Initialize();
	}

	private void Start()
	{
		Destroy(this.gameObject);
	}

	protected abstract void Initialize();
	protected sealed override void OnUpdate() { }
	protected sealed override void OnDispose() { }
	protected sealed override void Register() { }
	protected sealed override void Unregister() { }
	protected sealed override void UpdatePoint(Type t) { return; }
}

/// <summary>
/// 동일한 식별 정보를 지닌 ReferencePoint를 허용하는 클래스
/// </summary>
public abstract class ReferencePoint_General<T> : ReferencePoint where T : ReferencePoint
{
	protected override void OnAwake()
	{
		base.OnAwake();
		if (!points.ContainsKey(typeof(T)))
			points.Add(typeof(T), new List<ReferencePoint>());
	}

	protected sealed override void Register()
	{
		if (points[typeof(T)].Contains(this))
			Debug.LogWarning($"Failed to Register! ReferencePoint({this.name}) instance is already registered!");
		else
			points[typeof(T)].Add(this);

		if (m_IsPointUpdatable)
			OnUpdatePoints += UpdatePoint;
	}

	protected sealed override void Unregister()
	{
		if (points.ContainsKey(typeof(T)) && points[typeof(T)].Contains(this))
			points[typeof(T)].Remove(this);

		if (m_IsPointUpdatable)
			OnUpdatePoints -= UpdatePoint;
	}

	protected sealed override void UpdatePoint(Type t)
	{
		if (t != typeof(T) || !m_IsPointUpdatable || tr == null)
			return;
		position = tr.position;
		rotation = tr.rotation;
		scale = tr.localScale;
		OnUpdate();
	}
}

/// <summary>
/// 동일한 식별 정보를 지닌 ReferencePoint를 허용하지 않는 클래스
/// </summary>
public abstract class ReferencePoint_Unique<T> : ReferencePoint where T : ReferencePoint
{
	protected override void OnAwake()
	{
		base.OnAwake();
		if (!points.ContainsKey(typeof(T)))
			points.Add(typeof(T), new List<ReferencePoint>());
	}

	protected sealed override void Register()
	{
		if (TryGetPoint(IsSame, out T pt))  // 동일한 식별 정보를 지닌 참조 포인트가 이미 존재하는 경우
		{
			Debug.LogWarning($"Failed to Register! ReferencePoint({this.name}) is already registered with the same information!");
			return;
		}

		if (points[typeof(T)].Contains(this))
			Debug.LogWarning($"Failed to Register! ReferencePoint({this.name}) instance is already registered!");
		else
			points[typeof(T)].Add(this);

		if (m_IsPointUpdatable)
			OnUpdatePoints += UpdatePoint;
	}

	protected sealed override void Unregister()
	{
		if (points.ContainsKey(typeof(T)) && points[typeof(T)].Contains(this))
			points[typeof(T)].Remove(this);

		if (m_IsPointUpdatable)
			OnUpdatePoints -= UpdatePoint;
	}

	/// <summary>
	/// ReferencePoint 등록 시 정보 일치여부를 판단하는 함수.
	/// </summary>
	protected abstract bool IsSame(T t);

	protected sealed override void UpdatePoint(Type t)
	{
		if (t != typeof(T) || !m_IsPointUpdatable || tr == null)
			return;
		position = tr.position;
		rotation = tr.rotation;
		scale = tr.localScale;
		OnUpdate();
	}
}

public abstract class ReferencePoint : MonoBehaviour
{
	protected Transform tr;
	protected Vector3 position;
	protected Quaternion rotation;
	protected Vector3 scale;

	// 현재 시점의 Transform 정보가 아니라 의도한 시점에 캐싱된 정보를 반환하는 프로퍼티.
	public Vector3 Position => position;
	public Quaternion Rotation => rotation;
	public Vector3 Scale => scale;

	[SerializeField] protected bool m_IsPointUpdatable = true;  // ReferencePoint에 대한 정보 갱신을 허용할지 여부.

	protected static readonly Dictionary<Type, List<ReferencePoint>> points = new Dictionary<Type, List<ReferencePoint>>();
	protected static Action<Type> OnUpdatePoints;

	private void Awake()
	{
		tr = transform;
		position = tr.position;
		rotation = tr.rotation;
		scale = tr.localScale;
		OnAwake();
		Register();
	}

	private void OnDestroy()
	{
		Unregister();
		OnDispose();
		tr = null;
		position = Vector3.zero;
		rotation = Quaternion.identity;
		scale = Vector3.zero;
	}

	protected virtual void OnAwake() { }
	protected virtual void OnUpdate() { }
	protected virtual void OnDispose() { }
	protected abstract void Register();
	protected abstract void Unregister();

	/// <summary>
	/// ReferencePoint의 위치를 갱신합니다.
	/// </summary>
	protected abstract void UpdatePoint(Type t);

	/// <summary>
	/// 특정 타입 및 조건을 만족하는 컬렉션 중 임의의 ReferencePoint를 반환합니다.
	/// </summary>
	public static bool TryGetRandomPoint<T>(Func<T, bool> predicate, out T point) where T : ReferencePoint
	{
		if (TryGetPoints(out IEnumerable<T> pts))
		{
			List<T> list = null;
			if (predicate == null)
			{
				list = pts.ToList();
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

	/// <summary>
	/// 특정 타입 및 조건을 만족하는 ReferencePoint를 반환합니다.
	/// </summary>
	public static bool TryGetPoint<T>(Func<T, bool> predicate, out T point) where T : ReferencePoint
	{
		if (TryGetPoints(out IEnumerable<T> pts))
		{
			if (predicate == null)
			{
				var arr = pts.ToList();
				point = arr[UnityEngine.Random.Range(0, arr.Count)];
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

	/// <summary>
	/// 특정 타입의 ReferencePoint 컬렉션을 반환합니다.
	/// </summary>
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

	/// <summary>
	/// 모든 ReferencePoint의 위치를 갱신합니다.
	/// </summary>
	public static void UpdateAllPoints()
	{
		foreach (var kvp in points)
		{
			Type type = kvp.Key;
			var list = kvp.Value;
			for (int i = 0; i < list.Count; i++)
				list[i].UpdatePoint(type);
		}
	}

	/// <summary>
	/// 특정 타입 ReferencePoint의 위치 갱신 이벤트를 호출합니다.
	/// </summary>
	public static void UpdatePoints<T>() where T : ReferencePoint
	{
		OnUpdatePoints?.Invoke(typeof(T));
	}

	/// <summary>
	/// 등록된 모든 ReferencePoint를 제거합니다.
	/// 게임 최초 시작 또는 재시작하는 경우 씬이 로드되기 전에 수행되어야 합니다.
	/// </summary>
	public static void Clear()
	{
		Debug.Log($"Clear all ReferencePoints.");
		points.Clear();
	}
}