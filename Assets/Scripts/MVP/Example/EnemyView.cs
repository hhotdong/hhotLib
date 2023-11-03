// Credit: https://www.jacksondunstan.com/articles/3092/comment-page-1
using System;
using UnityEngine;
using UnityEngine.UI;

namespace hhotLib.Common.MVP.Example
{
    public class DamageTakenEventArgs : EventArgs
    {
        public readonly float Damage;

        public DamageTakenEventArgs(float damage)
        {
            Damage = damage;
        }
    }

    /// <summary>
    /// View class derives from MonoBehaviour so that it utilizes direct references of Unity specific components
    /// as view element which receives input or sets output. By implementing interface, it becomes more testable and maintainable
    /// </summary>
    public class EnemyView : MonoBehaviour, IEnemyView
    {
        [SerializeField] private Text _hpText;
        [SerializeField] private Text _maxHpText;

        public event EventHandler<DamageTakenEventArgs> DamageTakenEvent;

        public void UpdateHp(float hp)
        {
            _hpText.text = "Hp: " + hp.ToString();
        }

        public void UpdateMaxHp(float maxHp)
        {
            _maxHpText.text = "MaxHp: " + maxHp.ToString();
        }

        public void TakeDamage(float damage)
        {
            DamageTakenEvent?.Invoke(this, new DamageTakenEventArgs(damage));
        }

        private void OnDestroy()
        {
            DamageTakenEvent = null;
        }
    }
}