// Credit: https://www.jacksondunstan.com/articles/3092/comment-page-1
// Credit: https://saens.tistory.com/13
using UnityEngine;

namespace hhotLib.Common.MVP.Example
{
    /// <summary>
    /// Model class derives from ScriptablObejct.
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyModel", menuName = "MVP/Example/EnemyModel")]
    public class EnemyModel : MutableModel
    {
        public BindableValueProperty<int> Hp;
        
        public override void Reset()
        {
            Hp.Dispose();
        }
    }
}