// Credit: https://www.jacksondunstan.com/articles/3092/comment-page-1
using System;

namespace hhotLib.Common.MVP.Example
{
    public interface IEnemyView
    {
        /// <summary>
        /// Input event
        /// View passes through the input event to presenter by invoking this event
        /// </summary>
        event EventHandler<DamageTakenEventArgs> DamageTakenEvent;

        /// <summary>
        /// Output function
        /// Presenter calls this function to make changes for the view
        /// </summary>
        void UpdateHp(float hp);
        void UpdateMaxHp(float maxHp);
    }
}