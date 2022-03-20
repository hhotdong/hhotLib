//using System;
//using UnityEngine;
//using ScriptableObjectArchitecture;
//using deVoid.Utils;

//public class UIGen_Gold : UIGen_World
//{
//    public int LandmarkTypeIdx { get; set; }
//    public Action ButtonClickedCallback { get; set; }
//    [SerializeField] private GameObject m_GoldButtonEffect;
//    [SerializeField] private ParticleSystem m_GoldButtonShowEffect;
//    [SerializeField] private IntGameEvent _OnClickGoldGenButton = default(IntGameEvent);

//    private bool m_IsToggledOn = true;
//    private bool IsVisible => gameObject.activeSelf && m_IsToggledOn;


//    //////////////////////////////////////////
//    // Initialize
//    //////////////////////////////////////////

//    protected override void OnEnable()
//    {
//        base.OnEnable();
//        if (UIManager.IsWorldUIVisible && IsVisible)
//            m_GoldButtonShowEffect.Play();
//    }

//    protected override void OnDisable()
//    {
//        base.OnDisable();
//        m_GoldButtonShowEffect.Stop();
//    }


//    //////////////////////////////////////////
//    // Listeners
//    //////////////////////////////////////////

//    public override void ButtonClicked()
//    {
//        SoundManager.PlaySoundEffect(SoundType.BUTTON_BUBBLE);

//        ButtonClickedCallback?.Invoke();

//        if (UIManager.IsWorldUIVisible && IsVisible)
//            ObjectPoolManager.Get(ObjectPoolName.EFFECT_GOLD_POP).transform.position = tr.position;

//        _OnClickGoldGenButton.Raise(LandmarkTypeIdx);

//        this.gameObject.SetActive(false);
//    }

//    public void DoGoldButtonReady()
//    {
//        if (UIManager.IsWorldUIVisible && IsVisible)
//            m_GoldButtonShowEffect.Play();
//    }


//    //////////////////////////////////////////
//    // Utilities
//    //////////////////////////////////////////

//    //public override void Toggle(bool isOn, bool forceNow = false)
//    //{
//    //    //float duration = forceNow ? 0.01F : 0.5F;
//    //    //float endValue = isOn ? 1.0F : 0.0F;
//    //    m_IsToggledOn = isOn;
//    //    m_GoldButtonEffect.SetActive(isOn);
//    //}

//    public void ForceToggle(bool toggle)
//    {
//        if (m_IsToggledOn == toggle)
//            return;

//        if (toggle)
//        {
//            m_IsToggledOn = true;
//            m_GoldButtonEffect.SetActive(true);
//            Signals.Get<ToggleWorldUIVisible>().AddListener(OnToggleWorldUIVisible);
//            Signals.Get<ToggleWorldUIInteractable>().AddListener(OnToggleWorldUIInteractable);
//        }
//        else
//        {
//            m_IsToggledOn = false;
//            m_GoldButtonEffect.SetActive(false);
//            Signals.Get<ToggleWorldUIVisible>().RemoveListener(OnToggleWorldUIVisible);
//            Signals.Get<ToggleWorldUIInteractable>().RemoveListener(OnToggleWorldUIInteractable);
//        }
//    }
//}
