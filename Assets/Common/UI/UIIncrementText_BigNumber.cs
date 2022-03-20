//using System;
//using UnityEngine;
//using TMPro;
//using DG.Tweening;

//public class UIIncrementText_BigNumber : UIIncrementText<BigNumber>
//{
//    [SerializeField] private bool m_IsIntegerExpression = false;
//    private Tweener m_Tweener;
//    private Action updateTextCallback = null;

//    /// <summary>
//    /// 텍스트 업데이트 전용 데이터
//    /// </summary>
//    [SerializeField] private BigNumberData _Data;

//    /// <summary>
//    /// 텍스트 업데이트 적용값
//    /// </summary>
//    private double m_TempFollowingValueMant;

//    /// <summary>
//    /// 텍스트 업데이트 목표값
//    /// </summary>
//    private double m_TempDestinationValueMant;

//    private static readonly float TEXT_UPDATE_DURATION = 0.75F;


//    //////////////////////////////////////////
//    // Initialize
//    //////////////////////////////////////////
    
//    public override void Initialize()
//    {
//        m_ValueText = GetComponent<TextMeshProUGUI>();
//        m_FollowingValue = m_TargetValue = _Data.Value;
//        m_ValueText.text = m_IsIntegerExpression
//                            ? string.Format(HelperManager.STRING_FORMAT_UNIT_A, m_TargetValue.Mantissa.ToString(HelperManager.STRING_FORMAT_NUMBER_INT), IncrementHelper.GetConvertedExponent(m_TargetValue.Exponent))
//                            : string.Format(HelperManager.STRING_FORMAT_UNIT_A, m_TargetValue.Mantissa.ToString(HelperManager.STRING_FORMAT_NUMBER), IncrementHelper.GetConvertedExponent(m_TargetValue.Exponent));
//        m_IsInitialized = true;
//    }


//    //////////////////////////////////////////
//    // Utilities
//    //////////////////////////////////////////

//    public override void Increment(Action alignCallback = null)
//    {
//        if (!m_IsInitialized)
//            Initialize();

//        bool isSameExponent = false;
//        bool isExpDiffTwoOrBigger = false;
//        string expUnit = IncrementHelper.GetConvertedExponent(_Data.Value.Exponent);

//        if (IncrementHelper.Compare(_Data.Value, m_TargetValue, out isSameExponent, out isExpDiffTwoOrBigger))
//        {
//            m_TempFollowingValueMant = isExpDiffTwoOrBigger == true ? 1.0 : m_FollowingValue.Mantissa;
//            m_TempDestinationValueMant = _Data.Value.Mantissa;

//            if (m_Tweener != null && m_Tweener.IsPlaying())
//            {
//                //Debug.Log("m_Tweener is not null and still playing.");
//                m_TempFollowingValueMant = isExpDiffTwoOrBigger == true ? 1.0 : m_TargetValue.Mantissa;
//            }

//            if (isSameExponent)
//            {
//                if (m_IsIntegerExpression && string.IsNullOrEmpty(expUnit))
//                    updateTextCallback = UpdateText_Int;
//                else
//                    updateTextCallback = UpdateText;

//                void UpdateText_Int() => m_ValueText.text = string.Format(HelperManager.STRING_FORMAT_UNIT_A, m_TempFollowingValueMant.ToString(HelperManager.STRING_FORMAT_NUMBER_INT), expUnit);
//                void UpdateText() => m_ValueText.text = string.Format(HelperManager.STRING_FORMAT_UNIT_A, m_TempFollowingValueMant.ToString(HelperManager.STRING_FORMAT_NUMBER), expUnit);
//            }
//            else
//            {
//                m_TempDestinationValueMant = isExpDiffTwoOrBigger == true
//                                                ? m_TempDestinationValueMant * IncrementHelper.ONE_THOUSAND
//                                                : m_TempDestinationValueMant * Math.Pow(IncrementHelper.ONE_THOUSAND, _Data.Value.Exponent - m_FollowingValue.Exponent);

//                if (m_IsIntegerExpression)
//                    updateTextCallback = UpdateText_Int;
//                else
//                    updateTextCallback = UpdateText;


//                void UpdateText_Int()
//                {
//                    int tempExp = (int)Math.Log(m_TempFollowingValueMant, IncrementHelper.ONE_THOUSAND);
//                    double tempMant = m_TempFollowingValueMant / Math.Pow(IncrementHelper.ONE_THOUSAND, (double)tempExp);
//                    m_ValueText.text = string.Format(HelperManager.STRING_FORMAT_UNIT_A, tempMant.ToString(HelperManager.STRING_FORMAT_NUMBER_INT), expUnit);
//                }

//                void UpdateText()
//                {
//                    int tempExp = (int)Math.Log(m_TempFollowingValueMant, IncrementHelper.ONE_THOUSAND);
//                    double tempMant = m_TempFollowingValueMant / Math.Pow(IncrementHelper.ONE_THOUSAND, (double)tempExp);
//                    m_ValueText.text = string.Format(HelperManager.STRING_FORMAT_UNIT_A, tempMant.ToString(HelperManager.STRING_FORMAT_NUMBER), expUnit);
//                }
//            }

//            if (m_Tweener == null)
//            {
//                m_Tweener = DOTween
//                    .To(() => m_TempFollowingValueMant, x => { m_TempFollowingValueMant = x; }, m_TempDestinationValueMant, TEXT_UPDATE_DURATION)
//                    .OnStart(DoStart)
//                    .OnUpdate(DoUpdate)
//                    .OnComplete(DoComplete)
//                    .SetEase(Ease.OutSine)
//                    .SetAutoKill(false)
//                    .Play();

//                void DoStart() => alignCallback?.Invoke();
//                void DoUpdate() => updateTextCallback.Invoke();
//                void DoComplete()
//                {
//                    m_FollowingValue = new BigNumber(_Data.Value.Mantissa, _Data.Value.Exponent);
//                    updateTextCallback = null;
//                    alignCallback?.Invoke();
//                }
//            }
//            else
//            {
//                m_Tweener
//                    .ChangeValues(m_TempFollowingValueMant, m_TempDestinationValueMant)
//                    .Restart();
//            }
//        }
//        else
//        {
//            //Debug.Log($"Former({_Data.Value.Mantissa}, {_Data.Value.Exponent}) is smaller than Latter({m_TargetValue.Mantissa}, {m_TargetValue.Exponent})!");
//            m_ValueText.text = string.Format(HelperManager.STRING_FORMAT_UNIT_A, _Data.Value.Mantissa.ToString(HelperManager.STRING_FORMAT_NUMBER), expUnit);
//        }

//        m_TargetValue = _Data.Value;
//    }
//}
