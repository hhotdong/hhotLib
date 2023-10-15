// Credit: https://github.com/yankooliveira/uiframework_examples
// TODO: In/OutTransitionFinished 이벤트로 선언 및 트랜지션 시작/종료 시 이벤트 추가, Property로 콜백 받아오기(또는 임시적인 콜백함수 등록/해제로직 구현)
// TODO: 특정 윈도우가 열렸을 때 펜딩되는 이벤트 수행하는 로직 구현(펜딩된 이벤트가 존재하면 해당 윈도우가 열리지 않고 펜딩된 이벤트부터 호출하기?)
// TODO: 뒤로가기(안드로이드 백버튼) 대응
// TODO: 스크린 프리팹과 아틀라스 비동기 로드
// TODO: 동일한 팝업 윈도우 여러 번 중첩해서 큐에 대기시킬 수 있는 기능(윈도우 닫히는 시점의 이벤트 활용?)
// (테스트 완료) PopUptoWindowSignal 동작 확인(큐 클리어 기능 포함)
// (테스트 완료) 동일한 패널/윈도우 컨트롤러를 사용하지만 서로 다른 이름의 패널/윈도우 프리팹을 만들었을 때 서로 다르게 인식하는지 여부 => 서로 다르게 인식함. 그리고 기존 스크린을 상속하는 것도 가능함.
// (테스트 완료) 동일한 윈도우를 스택에 2개 이상 적재할 수 있는지 확인 => 불가능함
// (테스트 완료) PushWindowSignal, PopWindowSignal, HideAllScreenSignal 동작 확인
// (테스트 완료) ShowPanelSignal, HidePanelSignal 동작 확인
using UnityEngine;
using UnityEngine.SceneManagement;
using deVoid.Utils;
using hhotLib.Common;

namespace deVoid.UIFramework
{
    public class ShowPanelSignal            : ASignal<string, IPanelProperties> { }
    public class HidePanelSignal            : ASignal<string>                   { }
    public class PushWindowSignal           : ASignal<string, IWindowProperties>{ }
    public class PopWindowSignal            : ASignal                           { }
    public class PopToWindowSignal          : ASignal<string, bool, bool>       { }
    public class HideAllScreenSignal        : ASignal<bool, bool, bool>         { }
    public class SaveScreenContextSignal    : ASignal<bool, bool, bool>         { }
    public class RestoreScreenContextSignal : ASignal<bool, bool>               { }

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
            Signals.Get<ShowPanelSignal>()           .AddListener(OnShowPanel);
            Signals.Get<HidePanelSignal>()           .AddListener(OnHidePanel);
            Signals.Get<PushWindowSignal>()          .AddListener(OnPushWindow);
            Signals.Get<PopWindowSignal>()           .AddListener(OnPopWindow);
            Signals.Get<PopToWindowSignal>()         .AddListener(OnPopToWindow);
            Signals.Get<HideAllScreenSignal>()       .AddListener(OnHideAllScreen);
            Signals.Get<SaveScreenContextSignal>()   .AddListener(OnSaveScreenContext);
            Signals.Get<RestoreScreenContextSignal>().AddListener(OnRestoreScreenContext);
        }

        private void RemoveListeners()
        {
            Signals.Get<ShowPanelSignal>()           .RemoveListener(OnShowPanel);
            Signals.Get<HidePanelSignal>()           .RemoveListener(OnHidePanel);
            Signals.Get<PushWindowSignal>()          .RemoveListener(OnPushWindow);
            Signals.Get<PopWindowSignal>()           .RemoveListener(OnPopWindow);
            Signals.Get<PopToWindowSignal>()         .RemoveListener(OnPopToWindow);
            Signals.Get<HideAllScreenSignal>()       .RemoveListener(OnHideAllScreen);
            Signals.Get<SaveScreenContextSignal>()   .RemoveListener(OnSaveScreenContext);
            Signals.Get<RestoreScreenContextSignal>().RemoveListener(OnRestoreScreenContext);
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

        private void OnPopToWindow(string windowId, bool clearWindowQueue, bool animate)
        {
            screenCamUIFrame.PopToWindow(windowId, clearWindowQueue, animate);
        }

        private void OnHideAllScreen(bool includePanel, bool includeWindow, bool animate)
        {
            if (includePanel)
                screenCamUIFrame.HideAllPanels(animate);

            if (includeWindow)
                screenCamUIFrame.CloseAllWindows(animate);
        }

        private void OnSaveScreenContext(bool includePanel, bool includeWindow, bool animate)
        {
            screenCamUIFrame.SaveScreenContext(includePanel, includeWindow, animate);
        }

        private void OnRestoreScreenContext(bool includePanel, bool includeWindow)
        {
            screenCamUIFrame.RestoreScreenContext(includePanel, includeWindow);
        }
    }
}