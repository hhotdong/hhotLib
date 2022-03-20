//using UnityEngine;
//using DG.Tweening;
//using deVoid.Utils;

//public interface IWorldUI
//{
//	void SquashAndStretch();
//	void UpdateScaleAndRotation();
//	void ButtonClicked();
//}

//public abstract class UIGen_World : MonoBehaviour, IWorldUI
//{
//	protected Transform tr;
//	protected SpriteRenderer[] rds;
//	protected Sequence seq;
//	protected Tweener tweener;
//	protected Collider coll;
//	protected Vector3 m_InitScale;
//	protected float m_MinScale;
//	[SerializeField] protected Transform m_Content;
//	[SerializeField] protected bool m_IsScailable = false;


//	//////////////////////////////////////////
//	// Initialize
//	//////////////////////////////////////////

//	protected virtual void Awake()
//	{
//		tr = GetComponent<Transform>();
//		rds = GetComponentsInChildren<SpriteRenderer>();
//		coll = GetComponent<Collider>();
//		m_InitScale = tr.localScale;
//		m_MinScale = m_IsScailable ? 0.45F : 1.0F;

//		Signals.Get<ToggleWorldUIVisible>().AddListener(OnToggleWorldUIVisible);
//		Signals.Get<ToggleWorldUIInteractable>().AddListener(OnToggleWorldUIInteractable);
//	}

//	protected virtual void OnEnable()
//	{
//		m_Content.localScale = Vector3.zero;

//		if (!UIManager.IsWorldUIVisible)
//		{
//			for (int i = 0; i < rds.Length; i++)
//			{
//				Color col = rds[i].color;
//				col.a = 0.0F;
//				rds[i].color = col;
//			}
//		}

//		SquashAndStretch();

//		if (!UIManager.WorldUIs.Contains(this))
//			UIManager.WorldUIs.Add(this);
//	}

//	protected virtual void OnDisable()
//	{
//		if (UIManager.WorldUIs.Contains(this))
//			UIManager.WorldUIs.Remove(this);

//		tweener?.Pause();
//	}

//	protected virtual void OnDestroy()
//	{
//		Signals.Get<ToggleWorldUIVisible>().RemoveListener(OnToggleWorldUIVisible);
//		Signals.Get<ToggleWorldUIInteractable>().RemoveListener(OnToggleWorldUIInteractable);
//	}


//	//////////////////////////////////////////
//	// Listeners
//	//////////////////////////////////////////

//	public virtual void OnToggleWorldUIVisible(bool isOn, bool forceNow)
//	{
//		//Debug.Log($"OnToggleWorldUIVisible : {isOn} , {forceNow}");
//		if (isOn && !forceNow)
//			SquashAndStretch();

//		Toggle(isOn, forceNow);
//	}

//	protected virtual void OnToggleWorldUIInteractable(bool isOn)
//	{
//		coll.enabled = isOn;
//	}

//	public abstract void ButtonClicked();


//	//////////////////////////////////////////
//	// Utilities
//	//////////////////////////////////////////

//	public void SquashAndStretch()
//	{
//		if (seq == null)
//		{
//			seq = UIManager.GetSquashAndStretchSequence(m_Content)
//						  .OnStart(DoActivatingStart)
//						  .OnComplete(DoActivatingComplete)
//						  .SetAutoKill(false)
//						  .SetRecyclable(true)
//						  .Play();
//		}
//		else
//			seq.Restart();
//	}

//	protected virtual void DoActivatingStart()
//	{

//	}

//	protected virtual void DoActivatingComplete()
//	{
//		if (tweener == null)
//			tweener = UIManager.GetFloatingTweener(m_Content).Play();
//		else
//			tweener.Play();
//	}

//	public void UpdateScaleAndRotation()
//	{
//		// 카메라 줌에 따른 스케일 조정
//		tr.localScale = m_InitScale * Mathf.Clamp(CameraHandler.ZoomScaler, m_MinScale, 1.0F);

//		// 빌보드 효과
//		tr.rotation = UIManager.WORLD_UI_BILLBOARD_ROTATION;
//	}

//	public virtual void Toggle(bool isOn, bool forceNow = false)
//	{
//		float duration = forceNow ? 0.01F : 0.5F;
//		float endValue = isOn ? 1.0F : 0.0F;

//		for (int i = 0; i < rds.Length; i++)
//		{
//			if (DOTween.IsTweening(rds[i]))
//				DOTween.Kill(rds[i]);

//			rds[i].DOFade(endValue, duration).Play();
//		}
//	}

//	//public void RefreshButton()
//	//{
//	//    coll.enabled = TouchManager.CurrentTouchContext == TouchContext.CAM_HANDLING ? true : false;
//	//    Toggle(true, true);
//	//}
//}
