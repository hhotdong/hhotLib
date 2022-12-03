using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace hhotLib.Common
{
    public enum CoinEffectType {
        Gold, Gem,
    }

    [RequireComponent(typeof(LocalObjectPool))]
    public class UICoinEffectGenerator : MonoBehaviour
    {
        [SerializeField] private CoinEffectType coinEffectType   = CoinEffectType.Gold;
        [SerializeField] private int            coinCountsByOnce = 10;
        [SerializeField] private float          popDuration      =  0.5f;
        [SerializeField] private float          moveDuration     =  0.7f;
        [SerializeField] private float          fadeDuration     =  0.25f;
        [SerializeField] private float          playInterval     =  0.05f;
        [SerializeField] private float          coinImageScaler  =  1.0f;
        [SerializeField] private Vector2        popDistanceRange = new Vector2(30.0f, 60.0f);
        [SerializeField] private Ease           popEase          = Ease.OutQuad;
        [SerializeField] private Ease           moveEase         = Ease.InBack;

        private LocalObjectPool pool;

        private void Awake()
        {
            pool = GetComponent<LocalObjectPool>();
        }

        public void Play()
        {
            StartCoroutine(PlayCoroutine());
        }

        private IEnumerator PlayCoroutine()
        {
            var INTERVAL = new WaitForSecondsRealtime(playInterval);
            for (int i = 0; i < coinCountsByOnce; i++)
            {
                var coinAgentGo = pool.Get();
                var coinAgent = coinAgentGo.GetComponent<UICoinEffectAgent>();
                if (coinAgent == null)
                {
                    Debug.LogWarning($"{nameof(UICoinEffectAgent)} of type({coinEffectType}) generated more than max pool amount({pool.MaxPoolAmount})!");
                    yield break;
                }

                coinAgent.SetImage(GetCoinSprite(coinEffectType), coinImageScaler);
                coinAgent.Play(coinEffectType, popDuration, fadeDuration, moveDuration, popDistanceRange, popEase, moveEase, () => {
                    pool.Free(coinAgentGo);
                });

                yield return INTERVAL;
            }
        }

        private static Sprite GetCoinSprite(CoinEffectType type)
        {
            switch (type)
            {
                default: return null;
                case CoinEffectType.Gold: return Resources.Load<Sprite>("UI_GoldCoin");
                case CoinEffectType.Gem : return Resources.Load<Sprite>("UI_GemCoin");
            }
        }
    }
}