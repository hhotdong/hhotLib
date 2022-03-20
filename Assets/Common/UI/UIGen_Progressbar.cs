//using UnityEngine;
//using TMPro;
//using DG.Tweening;
//using deVoid.Utils;
//using System;

//public class UIGen_Progressbar : UIGen
//{
//    protected Sequence seq;
//    [SerializeField] protected RectTransform m_ProgressbarBg;
//    [SerializeField] protected RectTransform m_Progressbar;
//    //[SerializeField] private TextMeshProUGUI m_OutputText;
//    [SerializeField] protected Transform m_Content;
//    private static float PROGRESS_MAX;

//    //////////////////////////////////////////
//    // Initialize
//    //////////////////////////////////////////

//    protected override void Awake()
//    {
//        base.Awake();

//        PROGRESS_MAX = m_ProgressbarBg.sizeDelta.x;

//        Signals.Get<ToggleWorldUIVisible>().AddListener(OnToggleWorldUIVisible);
//    }

//    protected override void OnEnable()
//    {
//        base.OnEnable();

//        m_Content.localScale = Vector3.zero;
//        SquashAndStretch();
//    }

//    protected override void OnDestroy()
//    {
//        base.OnDestroy();

//        Signals.Get<ToggleWorldUIVisible>().RemoveListener(OnToggleWorldUIVisible);
//    }


//    //////////////////////////////////////////
//    // Listeners
//    //////////////////////////////////////////

//    private void OnToggleWorldUIVisible(bool isOn, bool forceNow)
//    {
//        if (isOn && !forceNow)
//            SquashAndStretch();
//    }

//    //////////////////////////////////////////
//    // Utilities
//    //////////////////////////////////////////

//    public override void SquashAndStretch()
//    {
//        if (seq == null)
//        {
//            seq = UIManager.GetSquashAndStretchSequence(m_Content)
//                          .SetAutoKill(false)
//                          .SetRecyclable(true)
//                          .Play();
//        }
//        else
//            seq.Restart();
//    }

//    public void UpdateProgress(float progress)
//    {
//        Vector3 currentProgressSize = m_Progressbar.sizeDelta;
//        currentProgressSize.x = progress * PROGRESS_MAX;
//        m_Progressbar.sizeDelta = currentProgressSize;
//    }   

//    //public void UpdateOutputText(string output)
//    //{
//    //    m_OutputText.text = output;
//    //}
//}
