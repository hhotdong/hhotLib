using System;

namespace deVoid.UIFramework {
    /// <summary>
    /// Interface that all UI Screens must implement directly or indirectly
    /// </summary>
    public interface IUIScreenController {
        string ScreenId { get; set; }
        bool IsVisible { get; }
        bool IsTransitioning { get; }

        void Show(IScreenProperties props = null);
        void Hide(bool animate = true);
        void StopTransition();

        event Action<IUIScreenController> InTransitionStarted;
        event Action<IUIScreenController> InTransitionFinished;
        event Action<IUIScreenController> OutTransitionStarted;
        event Action<IUIScreenController> OutTransitionFinished;
        event Action<IUIScreenController> CloseRequest;
        event Action<IUIScreenController> ScreenDestroyed;
    }

    /// <summary>
    /// Interface that all Windows must implement
    /// </summary>
    public interface IWindowController : IUIScreenController {
        bool HideOnForegroundLost { get; }
        bool IsPopup { get; }
        WindowPriority WindowPriority { get; }
    }

    /// <summary>
    /// Interface that all Panels must implement
    /// </summary>
    public interface IPanelController : IUIScreenController {
        PanelPriority Priority { get; }
        bool CanReopenWhileVisible { get; }
    }
}
