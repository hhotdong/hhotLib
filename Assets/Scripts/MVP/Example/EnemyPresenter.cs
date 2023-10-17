// Credit: https://www.jacksondunstan.com/articles/3092/comment-page-1
namespace hhotLib.Common.MVP.Example
{
    /// <summary>
    /// Presenter class is a plain C# class. It has references to view and model which are provided as parameter in constructor.
    /// </summary>
    public class EnemyPresenter
    {
        // Keep references to the model and view.
        private IEnemyView _view;
        private EnemyModel _model;

        public EnemyPresenter(IEnemyView view, EnemyModel model)
        {
            _view  = view;
            _model = model;

            // Listen to input from the view.
            _view.GetDamageEvent += OnGetDamageEvent;

            // Listen to changes in the model.
            _model.Hp.ChangeValueEvent += OnChangeHp;

            // Set the view's initial state by synching with the model.
            _view.UpdateHp(_model.Hp.Value);
        }

        // Called when the view gets input event.
        private void OnGetDamageEvent()
        {
            _model.Hp.Value -= 1;
        }

        // Called when the model's hp property changes.
        private void OnChangeHp(int hp)
        {
            _view.UpdateHp(hp);
        }
    }
}