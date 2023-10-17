// Credit: https://www.jacksondunstan.com/articles/3092/comment-page-1
using System;
using UnityEngine;
using UnityEngine.UI;

namespace hhotLib.Common.MVP.Example
{
    /// <summary>
    /// View class derives from MonoBehaviour so that it utilizes direct references of Unity specific components
    /// as view element which receives input or sets output. By implementing interface, it becomes more testable and maintainable.
    /// </summary>
    public class EnemyView : MonoBehaviour, IEnemyView
    {
        [SerializeField] private Text hpText;

        public event Action GetDamageEvent;

        public void UpdateHp(int hp)
        {
            hpText.text = "HP: " + hp.ToString();
        }

        public void GetDamage()
        {
            GetDamageEvent?.Invoke();
        }

        private void OnDestroy()
        {
            GetDamageEvent = null;
        }
    }
}