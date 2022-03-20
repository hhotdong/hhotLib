//using UnityEngine;
//using UnityEngine.UI;
//using DG.Tweening;
//using ScriptableObjectArchitecture;

//public class UICollectCoinEffect : MonoBehaviour
//{
//    [Header("Common Infos")]
//    private bool m_IsPlaying = false;
//    private RectTransform tr;
//    private Camera m_UIcam;
//    private RectTransform m_CanvasTr;
//    private Transform m_OriginalParent;
//    private Image[] m_MovingCoinsImg;
//    private RectTransform[] m_MovingCoins;
//    [SerializeField] private float m_PopDuration = 0.5F;
//    [SerializeField] private float m_MoveDuration = 0.7F;
//    [SerializeField] private float m_FadeDuration = 0.25F;
//    [SerializeField] private float m_PopDistance = 30.0F;
//    [SerializeField] private Ease m_PopEase = Ease.OutQuad;
//    [SerializeField] private Ease m_MoveEase = Ease.InBack;
//    private static readonly float RADIUS_MODIFIER = 0.5F;
//    public static Transform[] InitPoints = new Transform[5];
//    public static Transform[] DestPoints = new Transform[4];
    
//    [Header("SO Events")]
//    [SerializeField] private BigNumberGameEvent _OnGoldAdded = default(BigNumberGameEvent);
//    [SerializeField] private BigNumberGameEvent _OnHeartAdded = default(BigNumberGameEvent);
//    [SerializeField] private BigNumberGameEvent _OnJewelAdded = default(BigNumberGameEvent);
//    [SerializeField] private BigNumberGameEvent _OnOtherAdded = default(BigNumberGameEvent);


//    //////////////////////////////////////////
//    // Initialize
//    //////////////////////////////////////////

//    private void Awake()
//    {
//        tr = GetComponent<RectTransform>();
//        m_UIcam = GameObject.Find("UICam").GetComponent<Camera>();
//        m_OriginalParent = ObjectPoolManager.GetFolder(ObjectPoolName.REWARD_COIN_EFFECT);
//        m_MovingCoinsImg = GetComponentsInChildren<Image>();

//        if (m_MovingCoins == null || m_MovingCoins.Length < 1)
//        {
//            m_MovingCoins = new RectTransform[m_MovingCoinsImg.Length];
//            for (int i = 0; i < m_MovingCoinsImg.Length; i++)
//                m_MovingCoins[i] = m_MovingCoinsImg[i].GetComponent<RectTransform>();
//        }
//    }


//    //////////////////////////////////////////
//    // Utilities
//    //////////////////////////////////////////

//    public void Play(int rewardTypeIdx, int initPointTypeIdx, BigNumber reward)
//    {
//        if (m_IsPlaying)
//            return;
//        m_IsPlaying = true;

//        if (!m_CanvasTr)
//            m_CanvasTr = UIManager.MainUICanvas.GetComponent<RectTransform>();

//        SetTween();
//        PlayTween();


//        void SetTween()
//        {
//            Transform destTr = null;
//            switch (rewardTypeIdx)
//            {
//                case RewardType.GOLD:
//                    destTr = DestPoints[0];
//                    _OnGoldAdded.Raise(reward);
//                    break;

//                case RewardType.HEART:
//                    destTr = DestPoints[1];
//                    _OnHeartAdded.Raise(reward);
//                    break;

//                case RewardType.JEWEL:
//                    destTr = DestPoints[2];
//                    _OnJewelAdded.Raise(reward);
//                    break;

//                case RewardType.OTHER:
//                    destTr = DestPoints[3];
//                    _OnOtherAdded.Raise(reward);
//                    break;

//                default:
//                    Debug.Log("RewardTypeIdx is not appropriate!");
//                    return;
//            }

//            Vector3 initPos = Vector3.zero;
//            switch (initPointTypeIdx)
//            {
//                case RewardCoinInitPoint.TOGGLE_BUTTON:
//                    initPos = m_UIcam.WorldToViewportPoint(InitPoints[initPointTypeIdx].position);
//                    break;

//                default:
//                    Debug.Log("InitPointTypeIdx is not appropriate!");
//                    return;
//            }

//            tr.SetParent(m_CanvasTr);
//            tr.localScale = Vector3.one;
//            tr.anchorMin = initPos;
//            tr.anchorMax = tr.anchorMin;
//            tr.anchoredPosition = tr.localPosition = Vector3.zero;
//            Debug.Log(initPos);

//            for (int i = 0; i < m_MovingCoins.Length; i++)
//            {
//                m_MovingCoins[i].SetParent(destTr, true);
//                m_MovingCoinsImg[i].color = HelperManager.WHITE_ALPHA_ZERO;
//            }
//        }

//        void PlayTween()
//        {
//            const float popInterval = 0.05F;
//            for (int i = 0; i < m_MovingCoins.Length; i++)
//            {
//                Vector2 rand = Random.insideUnitCircle * RADIUS_MODIFIER;
//                Sequence seq = DOTween.Sequence()
//                    .Append(m_MovingCoins[i].DOAnchorPos(m_MovingCoins[i].anchoredPosition + rand * m_PopDistance, m_PopDuration).SetEase(m_PopEase))
//                    .Join(m_MovingCoinsImg[i].DOFade(1.0F, m_FadeDuration))
//                    .Append(m_MovingCoins[i].DOAnchorPos(Vector2.zero, m_MoveDuration).SetEase(m_MoveEase))
//                    .PrependInterval(i * popInterval)
//                    .SetRecyclable(true);

//                if (i == m_MovingCoins.Length - 1)
//                    seq.OnComplete(DoComplete);

//                seq.Play();
//            }

//            void DoComplete()
//            {
//                m_IsPlaying = false;
//                for (int i = 0; i < m_MovingCoins.Length; i++)
//                {
//                    m_MovingCoins[i].SetParent(tr, true);
//                    m_MovingCoins[i].anchoredPosition = Vector2.zero;
//                }
//                tr.SetParent(m_OriginalParent);
//                ObjectPoolManager.Free(this.gameObject);
//            }
//        }
//    }
//}
