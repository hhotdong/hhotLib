//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using ScriptableObjectArchitecture;

//public enum GrantRewardType
//{
//    NONE                                       = -1,
//    REWARDED_AD_GOLD                           = 0,
//    REWARDED_AD_HEART                          = 1,
//    REWARDED_AD_JEWEL                          = 2,
//    REWARDED_AD_RARE_ANIMAL_RANDOM_BOX         = 3,
//    OFFLINE_REWARD                             = 4,
//    OFFLINE_REWARD_DOUBLE                      = 5,
//    REWARDED_TOUCH_GOLD                        = 6,
//    REWARDED_PICTURE_GOLD                      = 7,
//    ADV_ACCUMULATION                           = 8,
//    GROWTH_TIER_LEVELUP                        = 9,
//    GROWTH_DECREASE_UPGRADE_COST               = 10,
//    GROWTH_INCREASE_GOLD_OUTPUT                = 11,
//    GROWTH_INCREASE_REWARD_REWARDED_AD_GOLD    = 12,
//    GROWTH_INCREASE_REWARD_TOUCH               = 13,
//    GROWTH_INCREASE_ANIMAL_HEART_OUTPUT        = 14,
//    GROWTH_INCREASE_CHANCE_DOUBLEHEART         = 15,
//    GROWTH_INCREASE_REWARD_OFFLINE             = 16,
//    GROWTH_INCREASE_REWARD_PHOTO_MISSION       = 17,
//    QUEST_UPG_ANIMAL_0                         = 18,
//    QUEST_UPG_ANIMAL_1                         = 19,
//    QUEST_UPG_ANIMAL_2                         = 20,
//    QUEST_UPG_ANIMAL_3                         = 21,
//    QUEST_UPG_ANIMAL_4                         = 22,
//    QUEST_UPG_ANIMAL_5                         = 23,
//    QUEST_UPG_ANIMAL_6                         = 24,
//    QUEST_UPG_ANIMAL_7                         = 25,
//    QUEST_UPG_ANIMAL_8                         = 26,
//    QUEST_UPG_ANIMAL_9                         = 27,
//    QUEST_UPG_ANIMAL_10                        = 28,
//    QUEST_UPG_ANIMAL_11                        = 29,
//    QUEST_UPG_ANIMAL_12                        = 30,
//    QUEST_UPG_ANIMAL_13                        = 31,
//    QUEST_UPG_ANIMAL_14                        = 32,
//    QUEST_UPG_LNDMRK_0                         = 33,
//    QUEST_GET_NORMAL_ANIMAL                    = 34,
//    QUEST_CALL_ALBATROSS                       = 35,
//    QUEST_PHOTO_MISSION                        = 36,
//    QUEST_GET_TOUCH_REWARD                     = 37,
//    QUEST_USE_TIME_TICKET                      = 38,
//    QUEST_OPEN_RARE_ANIMAL_BOX                 = 39,
//    QUEST_FEED_ANIMAL                          = 40,
//    QUEST_ANIMAL_HEART                         = 41
//}

//[Serializable]
//public struct PhaseRewardsThreshold
//{
//    public AnimalMaxCountType type;
//    public List<int> thresholds;

//    public PhaseRewardsThreshold(AnimalMaxCountType _type)
//    {
//        type = _type;
//        thresholds = new List<int>();
//    }
//}

//public partial class RewardManager : ScriptableObject, IInitializable, IRemoteInitializable
//{
//    [Header("Data")]
//    [SerializeField] private BigNumberData _GoldRetained;
//    [SerializeField] private BigNumberData _HeartRetained;

//    [Header("Multipliers")]
//    [SerializeField] private double m_RewardedAdGoldMultiplier_Default;
//    [SerializeField] private double m_RewardedAdHeartMultiplier_Default;
//    [SerializeField] private double m_RewardedTouchGoldMultiplier_Default;
//    [SerializeField] private double m_RewardedPictureGoldMultiplier_Default;

//    public double RewardedAdGoldMultiplier_Default => m_RewardedAdGoldMultiplier_Default;
//    public double RewardedAdHeartMultiplier_Default => m_RewardedAdHeartMultiplier_Default;
//    public double RewardedTouchGoldMultiplier_Default => m_RewardedTouchGoldMultiplier_Default;
//    public double RewardedPictureGoldMultiplier_Default => m_RewardedPictureGoldMultiplier_Default;

//    [SerializeField] private DoubleReference _Mult_Common_AnimalCount = default(DoubleReference);                // (백분율) 공통 - 동물 증가값
//    [SerializeField] private DoubleReference _Mult_Common_LandmarkShowUpgrade = default(DoubleReference);        // (백분율) 공통 - 진화 횟수 배수

//    [SerializeField] private DoubleReference _Mult_Growth_LandmarkUpgradeCost = default(DoubleReference);        // (백분율) 성장 - 업그레이드 가격 감소값 
//    [SerializeField] private DoubleReference _Mult_Growth_LandmarkGoldOutput = default(DoubleReference);         // (백분율) 성장 - 모든 랜드마크 골드 생산량 증가값
//    [SerializeField] private DoubleReference _Mult_Growth_RewardedAdGold = default(DoubleReference);             // (백분율) 성장 - 알바트로스 보상 증가값
//    [SerializeField] private DoubleReference _Mult_Growth_GoldBox = default(DoubleReference);                    // (백분율) 성장 - 선물상자 보상 증가값
//    [SerializeField] private DoubleReference _Mult_Growth_HeartOutput = default(DoubleReference);                // (백분율) 성장 - 하트 생산량 증가값
//    [SerializeField] private DoubleReference _Mult_Growth_TakeAPicture = default(DoubleReference);               // (백분율) 성장 - 사진 미션 보상 증가값
//    [SerializeField] private DoubleReference _Chance_Growth_DoubleHeart = default(DoubleReference);              // (백분율) 성장 - 하트 두 배 확률
//    [SerializeField] private DoubleReference _Add_Growth_MaxOfflineTime = default(DoubleReference);              //        성장 - 오프라인 보상 시간 증가량  

//    [SerializeField] private DoubleReference _Mult_RareAnimal_LandmarkUpgradeCost = default(DoubleReference);    // (백분율) 희귀동물 - 업그레이드 가격 감소값 
//    [SerializeField] private DoubleReference _Mult_RareAnimal_LandmarkGoldOutput_A = default(DoubleReference);   // (백분율) 희귀동물 - 모든 랜드마크 골드 생산량 증가값 A(1005번 희귀동물 - 섬휘파람새)
//    [SerializeField] private DoubleReference _Mult_RareAnimal_LandmarkGoldOutput_B = default(DoubleReference);   // (백분율) 희귀동물 - 모든 랜드마크 골드 생산량 증가값 B(1009번 희귀동물 - 수리부엉이)
//    [SerializeField] private DoubleReference _Mult_RareAnimal_RewardedAdGold_A = default(DoubleReference);       // (백분율) 희귀동물 - 알바트로스 보상 증가값 A(1001번 희귀동물 - 박새)
//    [SerializeField] private DoubleReference _Mult_RareAnimal_RewardedAdGold_B = default(DoubleReference);       // (백분율) 희귀동물 - 알바트로스 보상 증가값 B(1007번 희귀동물 - 뻐꾸기)
//    [SerializeField] private DoubleReference _Mult_RareAnimal_GoldBox = default(DoubleReference);                // (백분율) 희귀동물 - 선물상자 보상 증가값
//    [SerializeField] private DoubleReference _Mult_RareAnimal_HeartOutput = default(DoubleReference);            // (백분율) 희귀동물 - 하트 생산량 증가값
//    [SerializeField] private DoubleReference _Mult_RareAnimal_TakeAPicture = default(DoubleReference);           // (백분율) 희귀동물 - 사진 미션 보상 증가값
//    [SerializeField] private DoubleReference _Chance_RareAnimal_DoubleHeart = default(DoubleReference);          // (백분율) 희귀동물 - 하트 두 배 확률
//    [SerializeField] private DoubleReference _Add_RareAnimal_MaxOfflineTime = default(DoubleReference);          //        희귀동물 - 오프라인 보상 시간 증가량

//    public double FinalMult_LandmarkUpgradeCost
//        => _Mult_Growth_LandmarkUpgradeCost.Value
//            * _Mult_RareAnimal_LandmarkUpgradeCost.Value
//            * 0.0001;

//    public double FinalMult_LandmarkGoldOutput
//        => _Mult_Growth_LandmarkGoldOutput.Value
//            * _Mult_Common_LandmarkShowUpgrade.Value
//            * _Mult_Common_AnimalCount.Value
//            * _Mult_RareAnimal_LandmarkGoldOutput_A.Value
//            * _Mult_RareAnimal_LandmarkGoldOutput_B.Value
//            * 0.0000000001;

//    public double FinalMult_RewardedAdGold
//        => m_RewardedAdGoldMultiplier_Default
//            * _Mult_Growth_RewardedAdGold.Value
//            * _Mult_RareAnimal_RewardedAdGold_A.Value
//            * _Mult_RareAnimal_RewardedAdGold_B.Value
//            * 0.000001;

//    public double FinalMult_GoldBox
//        => m_RewardedTouchGoldMultiplier_Default
//            * _Mult_Growth_GoldBox.Value
//            * _Mult_RareAnimal_GoldBox.Value
//            * 0.0001;

//    public double FinalMult_HeartOutput
//        => _Mult_Growth_HeartOutput.Value
//            * _Mult_Common_AnimalCount.Value
//            * _Mult_Common_LandmarkShowUpgrade.Value
//            * _Mult_RareAnimal_HeartOutput.Value
//            * 0.00000001;

//    public double FinalMult_TakeAPicture
//        => m_RewardedPictureGoldMultiplier_Default
//            * _Mult_Growth_TakeAPicture.Value
//            * _Mult_RareAnimal_TakeAPicture.Value
//            * 0.0001;

//    public double FinalChance_DoubleHeart
//        => _Chance_Growth_DoubleHeart.Value
//            + _Chance_RareAnimal_DoubleHeart.Value;

//    public double FinalAdd_MaxOfflineTime
//        => _Add_Growth_MaxOfflineTime.Value
//            + _Add_RareAnimal_MaxOfflineTime.Value;

//    [Header("ETC")]
//    [SerializeField] private GrowthRewardsAmount m_GrowthRewardsAmount;

//    public static readonly int QUEST_REWARD_UTILITY_AMOUNT = 1;
//    public static readonly int ACHIEVEMENT_REWARD_JEWEL_AMOUNT = 10;
//    public static readonly int GROWTH_REWARD_TIER_LEVELUP_JEWEL_AMOUNT = 30;
//    public static readonly int REWARDED_AD_JEWEL_AMOUNT = 30;
//    public static readonly int NEWBIE_TUTORIAL_REWARDS_AD_COUPON_AMOUNT = 5;
//    public static readonly int REWARDED_AD_ACCUMULATION_JEWEL_AMOUNT = 50;
//    public static readonly int NEWBIE_TUTORIAL_REWARDS_RARE_ANIMAL_RANDOM_BOX_AMOUNT = 1;
//    public static readonly BigNumber ACHIEVEMENT_REWARD_JEWEL_AMOUNT_BIGNUM = new BigNumber((double)ACHIEVEMENT_REWARD_JEWEL_AMOUNT, 0);
//    public static readonly BigNumber GROWTH_REWARD_TIER_LEVELUP_JEWEL_AMOUNT_BIGNUM = new BigNumber((double)GROWTH_REWARD_TIER_LEVELUP_JEWEL_AMOUNT, 0);
//    public static readonly BigNumber REWARDED_AD_JEWEL_AMOUNT_BIGNUM = new BigNumber((double)REWARDED_AD_JEWEL_AMOUNT, 0);
//    public static readonly BigNumber REWARDED_AD_ACCUMULATION_JEWEL_AMOUNT_BIGNUM = new BigNumber((double)REWARDED_AD_ACCUMULATION_JEWEL_AMOUNT, 0);

//    public static readonly int PRODUCT_PRICE_GOLD_TICKET_1H   = 30;
//    public static readonly int PRODUCT_PRICE_GOLD_TICKET_6H   = 160;
//    public static readonly int PRODUCT_PRICE_GOLD_TICKET_12H  = 300;
//    public static readonly int PRODUCT_PRICE_HEART_TICKET_1H  = 50;
//    public static readonly int PRODUCT_PRICE_HEART_TICKET_6H  = 250;
//    public static readonly int PRODUCT_PRICE_HEART_TICKET_12H = 500;
//    public static readonly int PRODUCT_PRICE_RARE_BOX = 200;

//    [Serializable]
//    private class GrowthRewardsAmount
//    {
//        public double GROWTH_REWARD_MULTIPLIER_RATE_DECREASE_UPGRADE_COST;
//        public double GROWTH_REWARD_MULTIPLIER_RATE_INCREASE_LANDMARK_GOLD_OUTPUT;
//        public double GROWTH_REWARD_MULTIPLIER_RATE_INCREASE_REWARD_REWARDED_AD_GOLD;
//        public double GROWTH_REWARD_MULTIPLIER_RATE_INCREASE_REWARD_GOLDBOX;
//        public double GROWTH_REWARD_MULTIPLIER_RATE_INCREASE_ANIMAL_HEART_OUTPUT;
//        public double GROWTH_REWARD_ADDITIVE_RATE_INCREASE_CHANCE_DOUBLEHEART;
//        public double GROWTH_REWARD_ADDITIVE_RATE_INCREASE_REWARD_OFFLINE;
//        public double GROWTH_REWARD_MULTIPLIER_RATE_INCREASE_REWARD_TAKE_A_PICTURE;
//    }

//    public double GetGrowthRewardRateToDisplay(GrowthType type)
//    {
//        switch (type)
//        {
//            case GrowthType.DECREASE_UPGRADE_COST: return 100 - (m_GrowthRewardsAmount.GROWTH_REWARD_MULTIPLIER_RATE_DECREASE_UPGRADE_COST * 100.0);
//            case GrowthType.INCREASE_GOLD_OUTPUT: return m_GrowthRewardsAmount.GROWTH_REWARD_MULTIPLIER_RATE_INCREASE_LANDMARK_GOLD_OUTPUT * 100.0;
//            case GrowthType.INCREASE_REWARD_REWARDED_AD_GOLD: return m_GrowthRewardsAmount.GROWTH_REWARD_MULTIPLIER_RATE_INCREASE_REWARD_REWARDED_AD_GOLD * 100.0;
//            case GrowthType.INCREASE_REWARD_TOUCH: return m_GrowthRewardsAmount.GROWTH_REWARD_MULTIPLIER_RATE_INCREASE_REWARD_GOLDBOX * 100.0;
//            case GrowthType.INCREASE_ANIMAL_HEART_OUTPUT: return m_GrowthRewardsAmount.GROWTH_REWARD_MULTIPLIER_RATE_INCREASE_ANIMAL_HEART_OUTPUT * 100.0;
//            case GrowthType.INCREASE_CHANCE_DOUBLEHEART: return m_GrowthRewardsAmount.GROWTH_REWARD_ADDITIVE_RATE_INCREASE_CHANCE_DOUBLEHEART;
//            case GrowthType.INCREASE_REWARD_OFFLINE: return m_GrowthRewardsAmount.GROWTH_REWARD_ADDITIVE_RATE_INCREASE_REWARD_OFFLINE / 60.0;
//            case GrowthType.INCREASE_REWARD_PHOTO_MISSION: return m_GrowthRewardsAmount.GROWTH_REWARD_MULTIPLIER_RATE_INCREASE_REWARD_TAKE_A_PICTURE * 100.0;
//            default:
//                Debug.Log($"Failed to get GrowthRewardRate because type({type.ToString()}) is not appropriate!");
//                return -1.0;
//        }
//    }
//}
