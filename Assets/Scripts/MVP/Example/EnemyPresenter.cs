// Credit: https://www.jacksondunstan.com/articles/3092/comment-page-1
namespace hhotLib.Common.MVP.Example
{
    /// <summary>
    /// Presenter class is a plain C# class. It has references to view and model which are provided as parameter in constructor
    /// </summary>
    public class EnemyPresenter
    {
        // Keep references to view and models
        private IEnemyView     _view;
        private EnemyModel     _model;
        private EnemyStatModel _statModel;

        public EnemyPresenter(IEnemyView view, EnemyModel model, EnemyStatModel statModel)
        {
            _view      = view;
            _model     = model;
            _statModel = statModel;

            // Listen to input from the view
            _view.DamageTakenEvent += OnDamageTaken;

            // Listen to changes in the model
            _model.Hp   .ValueChangedEvent += OnHpChanged;
            _model.MaxHp.ValueChangedEvent += OnMaxHpChanged;

            // Listen to changes in the stat model
            _statModel.BaseMaxHp.ValueChangedEvent += OnBaseMaxHpChanged;

            // Initialize model
            _model.Hp   .Value = _statModel.BaseMaxHp.Value;
            _model.MaxHp.Value = _statModel.BaseMaxHp.Value;

            // Set the view's initial state by synching with the model
            _view.UpdateHp   (_model.Hp   .Value);
            _view.UpdateMaxHp(_model.MaxHp.Value);
        }

        // Called when the view gets input event
        private void OnDamageTaken(object sender, DamageTakenEventArgs args)
        {
            _model.Hp.Value -= args.Damage;
        }

        // Called when Hp property of the model changed
        private void OnHpChanged(float hp)
        {
            _view.UpdateHp(hp);
        }

        // Called when MaxHp property of the model changed
        private void OnMaxHpChanged(float maxHp)
        {
            _view.UpdateMaxHp(maxHp);
        }

        // Called when BaseMaxHp property of the stat model changed
        private void OnBaseMaxHpChanged(float baseMaxHp)
        {
            _model.MaxHp.Value = baseMaxHp;
        }
    }
}