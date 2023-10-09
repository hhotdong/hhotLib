// Credit: https://github.com/yankooliveira/uiframework_examples

// TEST: PopUptoWindowSignal 동작 확인(큐 클리어 기능 포함)
// (테스트완료) 동일한 패널/윈도우 컨트롤러를 사용하지만 서로 다른 이름의 패널/윈도우 프리팹을 만들었을 때 서로 다르게 인식하는지 여부 => 서로 다르게 인식함. 그리고 기존 스크린을 상속하는 것도 가능함.
// (테스트완료) 동일한 윈도우를 스택에 2개 이상 적재할 수 있는지 확인 => 불가능함
// (테스트완료) PushWindowSignal, PopWindowSignal, HideAllScreenSignal 동작 확인
// (테스트완료) ShowPanelSignal, HidePanelSignal 동작 확인

// TODO: 현재 트랜지션중인 윈도우가 있을 때 윈도우 오픈 또는 닫으려고 하면 큐에 넣고 트랜지션 종료된 후에 실행하기 또는 매개변수에 따라 즉시 트랜지션을 종료시키고 윈도우 바로 열기
// TODO: In/OutTransitionFinished 이벤트로 선언 및 트랜지션 시작/종료 시 이벤트 추가, Property로 콜백 받아오기(또는 임시적인 콜백함수 등록/해제로직 구현)
// TODO: ForceHide 구현 여부
// TODO: 스크립터블 트랜지션 생성
// TODO: PopupWindow RefreshDarken, OnCloseRequestedByWindow 동작 확인
// FIXME: 특정 윈도우가 열렸을 때 펜딩되는 이벤트 수행하는 로직 구현
// FIXME: 뒤로가기(안드로이드 백버튼) 대응
// FIXME: 스크린 프리팹과 아틀라스 비동기 로드
using UnityEngine;
using UnityEngine.SceneManagement;
using deVoid.Utils;
using hhotLib.Common;

namespace deVoid.UIFramework
{
    public class ShowPanelSignal     : ASignal<string, IPanelProperties> { }
    public class HidePanelSignal     : ASignal<string>                   { }
    public class PushWindowSignal    : ASignal<string, IWindowProperties>{ }
    public class PopWindowSignal     : ASignal                           { }
    public class PopToWindowSignal   : ASignal<string, bool, bool>       { }
    public class HideAllScreenSignal : ASignal<bool, bool, bool>         { }

    [CreateAssetMenu(fileName = "UINavigation", menuName = "deVoid UI/UINavigation")]
    public sealed class UINavigation : SingletonScriptableObject<UINavigation>
    {
        [SerializeField] private UISettings screenCamSettings;

        private bool    isInitialized;
        private UIFrame screenCamUIFrame;

        public void Initialize()
        {
            if (isInitialized)
                Dispose();

            screenCamUIFrame = screenCamSettings.CreateUIInstance(true);
            MoveToUIScene(screenCamUIFrame.gameObject);
            AddListeners();
            isInitialized = true;
        }

        public void Dispose()
        {
            if (screenCamUIFrame != null)
            {
                if (screenCamUIFrame.gameObject != null)
                    Destroy(screenCamUIFrame.gameObject);
                screenCamUIFrame = null;
            }
            RemoveListeners();
            isInitialized = false;
        }

        private void MoveToUIScene(GameObject go)
        {
            Scene uiScene = SceneManager.GetSceneByName(name);
            if (uiScene.isLoaded == false)
                uiScene = SceneManager.CreateScene(name);
            SceneManager.MoveGameObjectToScene(go, uiScene);
        }

        private void AddListeners()
        {
            Signals.Get<ShowPanelSignal>()    .AddListener(OnShowPanel);
            Signals.Get<HidePanelSignal>()    .AddListener(OnHidePanel);
            Signals.Get<PushWindowSignal>()   .AddListener(OnPushWindow);
            Signals.Get<PopWindowSignal>()    .AddListener(OnPopWindow);
            Signals.Get<PopToWindowSignal>()  .AddListener(OnPopToWindow);
            Signals.Get<HideAllScreenSignal>().AddListener(OnHideAllScreen);
        }

        private void RemoveListeners()
        {
            Signals.Get<ShowPanelSignal>()    .RemoveListener(OnShowPanel);
            Signals.Get<HidePanelSignal>()    .RemoveListener(OnHidePanel);
            Signals.Get<PushWindowSignal>()   .RemoveListener(OnPushWindow);
            Signals.Get<PopWindowSignal>()    .RemoveListener(OnPopWindow);
            Signals.Get<PopToWindowSignal>()  .RemoveListener(OnPopToWindow);
            Signals.Get<HideAllScreenSignal>().RemoveListener(OnHideAllScreen);
        }

        private void OnShowPanel(string panelId, IPanelProperties properties)
        {
            screenCamUIFrame.ShowPanel(panelId, properties);
        }

        private void OnHidePanel(string panelId)
        {
            screenCamUIFrame.HidePanel(panelId);
        }

        private void OnPushWindow(string windowId, IWindowProperties properties)
        {
            screenCamUIFrame.OpenWindow(windowId, properties);
        }

        private void OnPopWindow()
        {
            screenCamUIFrame.CloseCurrentWindow();
        }

        private void OnPopToWindow(string windowId, bool clearWindowQueue, bool shouldAnimate)
        {
            screenCamUIFrame.PopToWindow(windowId, clearWindowQueue, shouldAnimate);
        }

        private void OnHideAllScreen(bool includePanel, bool includeWindow, bool shouldAnimate)
        {
            if (includePanel)
                screenCamUIFrame.HideAllPanels(shouldAnimate);

            if (includeWindow)
                screenCamUIFrame.CloseAllWindows(shouldAnimate);
        }
    }
}