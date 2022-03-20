//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using DG.Tweening;

//public class UIGen_Text : UIGen
//{
//    [SerializeField] private bool m_ShouldFreeAfterDisplay = true;
//    [SerializeField] private TextMeshProUGUI m_UIText;
//    [SerializeField] private Image m_UIImage;
//    private float m_ScaleModifier = 0.0F;
//    protected Sequence m_MoveSequence;
//    protected Ease m_MoveEase = Ease.OutQuad;
//    protected Ease m_ScaleEase = Ease.OutBack;
//    protected static readonly float MOVE_OFFSET = 2.0F;
//    protected static readonly float MOVE_DURATION = 1.5F;
//    protected static readonly float BLANK_SPACE_OFFSET = 0.05F;


//    //////////////////////////////////////////
//    // Initialize & Reset
//    //////////////////////////////////////////

//    public override void Initialize(Vector3 initPos, BigNumber num)
//    {
//        m_CanvasGroup.alpha = 0.0F;
//        m_UIText.text = string.Format(HelperManager.STRING_FORMAT_PLUS_SIGN, IncrementHelper.GetFormattedNumber(num, false));
//        HelperManager.AlignWithSameDistanceFromCenter(m_UIText, m_UIImage, BLANK_SPACE_OFFSET, true);
//        tr.position = initPos;
//    }

//    public void Move(BigNumber num)
//    {
//        m_CanvasGroup.alpha = 1.0F;
//        m_ScaleModifier = 0.0F;
//        m_UIText.text = string.Format(HelperManager.STRING_FORMAT_PLUS_SIGN, IncrementHelper.GetFormattedNumber(num, false));
//        HelperManager.AlignWithSameDistanceFromCenter(m_UIText, m_UIImage, BLANK_SPACE_OFFSET, true);
//        tr.localScale = m_IsScailable ? Vector3.zero : Vector3.one;

//        // 텍스트 생성이 시작되는 지점의 높이값이 계속 변화하는 경우(ex. 하트)
//        if (m_ShouldFreeAfterDisplay)
//        {
//            DOTween.Sequence()
//                .Append(tr.DOLocalMoveY(tr.localPosition.y + MOVE_OFFSET, MOVE_DURATION).SetEase(m_MoveEase))
//                .Insert(0.0F, DOTween.To(() => m_ScaleModifier, x => m_ScaleModifier = x, 1.0F, MOVE_DURATION * 0.35F).SetEase(m_ScaleEase))
//                .Insert(MOVE_DURATION * 0.8F, m_CanvasGroup.DOFade(0.0F, MOVE_DURATION * 0.2F))
//                .SetUpdate(UpdateType.Late)
//                .SetRecyclable(true)
//                .OnComplete(DoComplete)
//                .Play();
//            return;


//            void DoComplete() => UIObjectPoolManager.Free(this.gameObject);
//        }

//        if (m_MoveSequence == null)
//        {
//            SetSequence();
//            m_MoveSequence.Play();
//        }
//        else
//            m_MoveSequence.Restart();

//        // 텍스트 생성이 시작되는 지점의 높이값이 변하지 않는 경우(ex. 골드)
//        void SetSequence()
//        {
//            m_MoveSequence = DOTween.Sequence()
//                                .Append(tr.DOLocalMoveY(tr.localPosition.y + MOVE_OFFSET, MOVE_DURATION).SetEase(m_MoveEase))
//                                .Insert(0.0F, DOTween.To(() => m_ScaleModifier, x => m_ScaleModifier = x, 1.0F, MOVE_DURATION * 0.35F).SetEase(m_ScaleEase))
//                                .Insert(MOVE_DURATION * 0.8F, m_CanvasGroup.DOFade(0.0F, MOVE_DURATION * 0.2F))
//                                .SetUpdate(UpdateType.Late)
//                                .SetAutoKill(false)
//                                .SetRecyclable(true)
//                                .Play();
//        }
//    }
//}
