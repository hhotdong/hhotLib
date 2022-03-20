using System.Collections;
using UnityEngine;

namespace hhotLib.Tutorial
{
    public class TutorialPage : MonoBehaviour
    {
        private bool isInitialized = false;
        private bool isShowing = false;
        private IEnumerator endAfterTimeCoroutine = null;

        [SerializeField] private TutorialPageData data = null;

        public TutorialPageData GetData
        {
            get
            {
                if(isInitialized && data)
                {
                    return data;
                }
                else
                {
                    Debug.LogWarning($"Failed to get tutorial page data : Not yet initialized({!isInitialized}) or data is null({!data})!");
                    return null;
                }
            }
        }

        public void Initialize(TutorialPageData data)
        {
            isShowing = false;
            if(endAfterTimeCoroutine != null)
            {
                StopCoroutine(endAfterTimeCoroutine);
                endAfterTimeCoroutine = null;
            }
            this.data = data;
            isInitialized = true;
        }

        public void Begin()
        {
            if (!TutorialManager.IsEnabled || !isInitialized)
            {
                return;
            }

            if (!isShowing)
            {
                isShowing = true;

                if (data.toggleActive)
                {
                    gameObject.SetActive(true);
                }

                data.OnBegin();

                if (data.enableTimer)
                {
                    if (endAfterTimeCoroutine != null)
                    {
                        StopCoroutine(endAfterTimeCoroutine);
                    }

                    endAfterTimeCoroutine = EndTutorialAfterTimer();
                    StartCoroutine(endAfterTimeCoroutine);
                }

                if (data.enableAction)
                {
                    TutorialManager.Instance.AddAwaitingActionForTutorial(this, data.awaitingAction);
                }

                if (data.pauseTime)
                {
                    Time.timeScale = 0;
                }
            }


            IEnumerator EndTutorialAfterTimer()
            {
                yield return new WaitForSecondsRealtime(data.timerPeriod);

                endAfterTimeCoroutine = null;
                End();
            }
        }

        public void End()
        {
            if (!TutorialManager.IsEnabled || !isInitialized)
            {
                return;
            }

            ClearProcess(true);
        }

        public void Cancel(bool invokeCallback)
        {
            if (!TutorialManager.IsEnabled || !isInitialized)
            {
                return;
            }

            ClearProcess(false);
            if (invokeCallback)
            {
                data.OnCancel();
            }
        }

        private void ClearProcess(bool shouldCallEndEvent)
        {
            if (isShowing)
            {
                isShowing = false;

                if (data.pauseTime)
                {
                    Time.timeScale = 1;
                }

                if (data.enableTimer)
                {
                    if (endAfterTimeCoroutine != null)
                    {
                        StopCoroutine(endAfterTimeCoroutine);
                    }

                    endAfterTimeCoroutine = null;
                }

                if (data.toggleActive)
                {
                    gameObject.SetActive(false);
                }

                if (shouldCallEndEvent)
                {
                    data.OnEnd();
                }
                else if (data.enableAction)
                {
                    TutorialManager.Instance.RemoveTutorialActionInAwatingList(this);
                }
            }
        }
    }
}