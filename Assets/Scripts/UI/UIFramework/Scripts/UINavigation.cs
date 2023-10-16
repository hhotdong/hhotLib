// Credit: https://github.com/yankooliveira/uiframework_examples
// TODO: 뒤로가기(안드로이드 백버튼) 대응
// TODO: 스크린 프리팹과 아틀라스 비동기 로드
// TODO: 윈도우는 실시간으로 인스턴스 생성하도록 리팩터링
// (DOCS) 동일한 패널/윈도우 컨트롤러를 사용하지만 서로 다른 이름의 패널/윈도우 프리팹을 만들었을 때 서로 다르게 인식하는지 여부 => 서로 다르게 인식함. 그리고 기존 스크린을 상속하는 것도 가능함.
// (DOCS) 동일한 윈도우를 스택에 2개 이상 적재할 수 있는지 확인 => 불가능함
// (DOCS) ScreenTransitionEvent는 HideAll을 통해서도 클리어되지 않으므로 명시적으로 Clear 호출해야 함
using UnityEngine;
using UnityEngine.SceneManagement;
using deVoid.Utils;
using hhotLib.Common;

namespace deVoid.UIFramework
{
    public class ShowPanelSignal                   : ASignal<string, IPanelProperties>      { }
    public class HidePanelSignal                   : ASignal<string>                        { }
    public class PushWindowSignal                  : ASignal<string, IWindowProperties>     { }
    public class PopWindowSignal                   : ASignal                                { }
    public class PopToWindowSignal                 : ASignal<string, bool, bool>            { }
    public class HideAllScreenSignal               : ASignal<bool, bool, bool>              { }
    public class StashScreenContextSignal          : ASignal<bool, bool, bool>              { }
    public class RestoreScreenContextSignal        : ASignal<bool, bool>                    { }
    public class AppendScreenTransitionEventSignal : ASignal<string, ScreenTransitionEvent> { }
    public class ClearScreenTransitionEventSignal  : ASignal<string>                        { }

    [CreateAssetMenu(fileName = "UINavigation", menuName = "deVoid UI/UINavigation")]
    public sealed class UINavigation : SingletonScriptableObject<UINavigation>
    {
        [SerializeField] private UISettings _screenCamSettings;

        private bool    _isInitialized;
        private UIFrame _screenCamUIFrame;

        public void Initialize()
        {
            if (_isInitialized)
                Dispose();

            _screenCamUIFrame = _screenCamSettings.CreateUIInstance(true);
            MoveToUIScene(_screenCamUIFrame.gameObject);
            AddListeners();
            _isInitialized = true;
        }

        public void Dispose()
        {
            if (_screenCamUIFrame != null)
            {
                if (_screenCamUIFrame.gameObject != null)
                    Destroy(_screenCamUIFrame.gameObject);
                _screenCamUIFrame = null;
            }
            RemoveListeners();
            _isInitialized = false;
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
            Signals.Get<ShowPanelSignal>()                  .AddListener(OnShowPanel);
            Signals.Get<HidePanelSignal>()                  .AddListener(OnHidePanel);
            Signals.Get<PushWindowSignal>()                 .AddListener(OnPushWindow);
            Signals.Get<PopWindowSignal>()                  .AddListener(OnPopWindow);
            Signals.Get<PopToWindowSignal>()                .AddListener(OnPopToWindow);
            Signals.Get<HideAllScreenSignal>()              .AddListener(OnHideAllScreen);
            Signals.Get<StashScreenContextSignal>()         .AddListener(OnStashScreenContext);
            Signals.Get<RestoreScreenContextSignal>()       .AddListener(OnRestoreScreenContext);
            Signals.Get<AppendScreenTransitionEventSignal>().AddListener(AppendScreenTransitionEvent);
            Signals.Get<ClearScreenTransitionEventSignal>() .AddListener(ClearScreenTransitionEvent);
        }

        private void RemoveListeners()
        {
            Signals.Get<ShowPanelSignal>()                  .RemoveListener(OnShowPanel);
            Signals.Get<HidePanelSignal>()                  .RemoveListener(OnHidePanel);
            Signals.Get<PushWindowSignal>()                 .RemoveListener(OnPushWindow);
            Signals.Get<PopWindowSignal>()                  .RemoveListener(OnPopWindow);
            Signals.Get<PopToWindowSignal>()                .RemoveListener(OnPopToWindow);
            Signals.Get<HideAllScreenSignal>()              .RemoveListener(OnHideAllScreen);
            Signals.Get<StashScreenContextSignal>()         .RemoveListener(OnStashScreenContext);
            Signals.Get<RestoreScreenContextSignal>()       .RemoveListener(OnRestoreScreenContext);
            Signals.Get<AppendScreenTransitionEventSignal>().RemoveListener(AppendScreenTransitionEvent);
            Signals.Get<ClearScreenTransitionEventSignal>() .RemoveListener(ClearScreenTransitionEvent);
        }

        private void OnShowPanel(string panelId, IPanelProperties properties)
        {
            _screenCamUIFrame.ShowPanel(panelId, properties);
        }

        private void OnHidePanel(string panelId)
        {
            _screenCamUIFrame.HidePanel(panelId);
        }

        private void OnPushWindow(string windowId, IWindowProperties properties)
        {
            _screenCamUIFrame.OpenWindow(windowId, properties);
        }

        private void OnPopWindow()
        {
            _screenCamUIFrame.CloseCurrentWindow();
        }

        private void OnPopToWindow(string windowId, bool clearWindowQueue, bool animate)
        {
            _screenCamUIFrame.PopToWindow(windowId, clearWindowQueue, animate);
        }

        private void OnHideAllScreen(bool includePanel, bool includeWindow, bool animate)
        {
            if (includePanel)
                _screenCamUIFrame.HideAllPanels(animate);

            if (includeWindow)
                _screenCamUIFrame.CloseAllWindows(animate);
        }

        private void OnStashScreenContext(bool includePanel, bool includeWindow, bool animate)
        {
            _screenCamUIFrame.StashScreenContext(includePanel, includeWindow, animate);
        }

        private void OnRestoreScreenContext(bool includePanel, bool includeWindow)
        {
            _screenCamUIFrame.RestoreScreenContext(includePanel, includeWindow);
        }

        private void AppendScreenTransitionEvent(string screenId, ScreenTransitionEvent transitionEvent)
        {
            _screenCamUIFrame.AppendScreenTransitionEvent(screenId, transitionEvent);
        }

        private void ClearScreenTransitionEvent(string screenId)
        {
            _screenCamUIFrame.ClearScreenTransitionEvent(screenId);
        }
    }
}