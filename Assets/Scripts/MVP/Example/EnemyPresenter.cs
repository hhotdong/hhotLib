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
        private EnemyModelBase _modelBase;
        private EnemyModel     _modelInstance;

        public EnemyPresenter(IEnemyView view, EnemyModelBase modelBase, EnemyModel modelInstance)
        {
            _view          = view;
            _modelBase     = modelBase;
            _modelInstance = modelInstance;

            // Listen to input from the view
            _view.DamageTakenEvent += OnDamageTaken;

            // Listen to changes in the model instance
            _modelInstance.Hp   .ValueChangedEvent += OnHpChanged;
            _modelInstance.MaxHp.ValueChangedEvent += OnMaxHpChanged;

            // Listen to changes in the model base
            _modelBase.BaseMaxHp.ValueChangedEvent += OnBaseMaxHpChanged;

            // Initialize model instance
            _modelInstance.Hp   .Value = _modelBase.BaseMaxHp.Value;
            _modelInstance.MaxHp.Value = _modelBase.BaseMaxHp.Value;

            // Set the view's initial state by synching with the model
            _view.UpdateHp   (_modelInstance.Hp   .Value);
            _view.UpdateMaxHp(_modelInstance.MaxHp.Value);
        }

        // Called when the view gets input event
        private void OnDamageTaken(object sender, DamageTakenEventArgs args)
        {
            _modelInstance.Hp.Value -= args.Damage;
        }

        // Called when Hp property of the model instance changed
        private void OnHpChanged(float hp)
        {
            _view.UpdateHp(hp);
        }

        // Called when MaxHp property of the model instance changed
        private void OnMaxHpChanged(float maxHp)
        {
            _view.UpdateMaxHp(maxHp);
        }

        // Called when BaseMaxHp property of the model base changed
        private void OnBaseMaxHpChanged(float baseMaxHp)
        {
            _modelInstance.MaxHp.Value = baseMaxHp;
        }
    }
}