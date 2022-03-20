using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using hhotLib.Common;

namespace hhotLib.Tutorial
{
    public enum TutorialType
    {
        NONE                             = -1,
        NEWBIE_SEEDING                   = 0,
        NEWBIE_PURIFY                    = 2,
        NEWBIE_GET_ENERGY                = 3,
        NEWBIE_AUTO_GEN_ENERGY           = 4,
        NEWBIE_ACHIEVE_LEVEL_TEN         = 5,
        MAIN_GET_NEW_VEHICLE             = 5000,
        MAIN_KILL_CHAPTER_ONE_BOSS       = 5001,
        MAIN_GET_HEART                   = 5002,
        MAIN_GET_TOUCH_OBJECT_REWARD     = 5003,
        MAIN_GET_RARE_ANIMAL_RANDOM_BOX  = 5004,
        MAIN_GET_UTILITY_POINT           = 5005,
        MAIN_GET_CLAM_REWARD             = 5006
    }

    public class TutorialManager : Singleton<TutorialManager>
    {
        [Serializable]
        public class TutorialAwaitingAction
        {
            public string AwaitingAction = "";
            public TutorialPage TutorialPage;
        }

        public static bool IsEnabled { get; private set; }

        private readonly Dictionary<TutorialType, TutorialPage> tutorialPages = new Dictionary<TutorialType, TutorialPage>();
        private readonly List<TutorialAwaitingAction> tutorialsAwaitingAction = new List<TutorialAwaitingAction>();

        [SerializeField] private List<TutorialPageData> tutorialPageData;

        [Header("Cached references")]
        [NonSerialized] public UIButtonPressed upgradeButton;
        [NonSerialized] public GameObject navigationPanel;

        public event Action OnCompleteNewbieTutorial;
        public event Action<int, Vector3?> OnTriggerSubTutorial;

        private void Start()
        {
            GenerateTutorialPages();
            CacheReferences();

            void GenerateTutorialPages()
            {
                tutorialPages.Clear();
                for (int i = 0; i < tutorialPageData.Count; i++)
                {
                    if (tutorialPages.ContainsKey(tutorialPageData[i].tutorialType))
                    {
                        Debug.LogError("Duplicate Tutorial Name found: " + tutorialPageData[i].tutorialType.ToString());
                    }
                    else
                    {
                        var pageGO = new GameObject(tutorialPageData[i].tutorialType.ToString());
                        var page = pageGO.AddComponent<TutorialPage>();
                        pageGO.SetActive(false);
                        pageGO.transform.SetParent(this.transform);
                        page.Initialize(tutorialPageData[i]);
                        tutorialPages.Add(tutorialPageData[i].tutorialType, page);
                    }
                }
            }

            void CacheReferences()
            {

            }
        }

        private void OnEnable()
        {
            IsEnabled = true;
        }

        private void OnDisable()
        {
            IsEnabled = false;
            CancelAllTutorials();
        }

        protected override void OnDestroySingleton()
        {
            ClearAllListenersWhenReset();
        }

        public void ShowTutorial(TutorialType type)
        {
            if (!tutorialPages.TryGetValue(type, out TutorialPage page))
            {
                Debug.LogWarning($"Failed to show tutorial({type.ToString()}) : It's not registered!");
                return;
            }

            if (page.gameObject.activeSelf)
            {
                Debug.LogWarning($"Failed to show tutorial({type.ToString()}) : It's gameObject is already active!");
                return;
            }

            var data = page.GetData;
            if (!data)
            {
                Debug.LogWarning($"Failed to show tutorial({type.ToString()}) : Tutorial data is null!");
                return;
            }

            if(data.complete && !data.repetitive)
            {
                Debug.LogWarning($"Failed to show tutorial({type.ToString()}) : Tutorial was already complete!");
                return;
            }

            page.Begin();
        }

        public void HideTutorial(TutorialType type)
        {
            if (tutorialPages.TryGetValue(type, out TutorialPage page))
            {
                page.End();
            }
        }

        public void CancelTutorial(TutorialType type)
        {
            if (tutorialPages.TryGetValue(type, out TutorialPage page))
            {
                page.gameObject.SetActive(false);
                page.Cancel(true);
            }
        }

        public void CancelAllTutorials()
        {
            foreach (TutorialPage tutorialPage in tutorialPages.Values)
            {
                tutorialPage.Cancel(false);
            }
        }

        public void RestoreTriggeredTutorials()
        {
            foreach (var data in tutorialPageData)
            {
                if (data.triggered && !data.complete)
                {
                    ShowTutorial(data.tutorialType);
                }
            }
        }

        public void AddAwaitingActionForTutorial(TutorialPage tutorialPage, string action)
        {
            bool changed = false;
            for (int i = 0; i < tutorialsAwaitingAction.Count; i++)
            {
                if (tutorialsAwaitingAction[i].TutorialPage == tutorialPage)
                {
                    Debug.LogWarning($"Tutorial({tutorialsAwaitingAction[i].TutorialPage.name})'s awaiting action has changed!");
                    tutorialsAwaitingAction[i].AwaitingAction = action;
                    changed = true;
                }
            }

            if (!changed)
            {
                tutorialsAwaitingAction.Add(new TutorialAwaitingAction()
                {
                    TutorialPage = tutorialPage,
                    AwaitingAction = action
                });
            }
        }

        public void BroadcastTutorialAction(string action)
        {
            List<TutorialAwaitingAction> deleteList = new List<TutorialAwaitingAction>();
            for (int i = 0; i < tutorialsAwaitingAction.Count; i++)
            {
                if (tutorialsAwaitingAction[i].AwaitingAction.Equals(action))
                {
                    tutorialsAwaitingAction[i].TutorialPage.End();
                    deleteList.Add(tutorialsAwaitingAction[i]);
                }
            }

            for (int i = 0; i < deleteList.Count; i++)
            {
                tutorialsAwaitingAction.Remove(deleteList[i]);
            }

            deleteList.Clear();
            deleteList = null;
        }

        public void RemoveTutorialActionInAwatingList(TutorialPage tutorialPage)
        {
            for (int i = 0; i < tutorialsAwaitingAction.Count; i++)
            {
                if (tutorialsAwaitingAction[i].TutorialPage == tutorialPage)
                {
                    tutorialsAwaitingAction.Remove(tutorialsAwaitingAction[i]);
                }
            }
        }

        public void SkipNewbieTutorials()
        {
            var newbieTutorials = tutorialPageData.Where(x => x.tutorialType.ToString().Contains("NEWBIE"));
            foreach (var item in newbieTutorials)
            {
                item.complete = true;
            }

            var newbieTutorialPages = tutorialPages.Where(x => x.Value.GetData != null && x.Value.GetData.tutorialType.ToString().Contains("NEWBIE"));
            foreach (var item in newbieTutorialPages)
            {
                item.Value.End();
            }
        }

        public void ClearAllListenersWhenReset()
        {

        }
    }
}
