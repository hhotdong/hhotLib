// Credit: https://www.jacksondunstan.com/articles/3092/comment-page-1
// Credit: https://saens.tistory.com/13
using UnityEngine;

namespace hhotLib.Common.MVP.Example
{
    /// <summary>
    /// ModelBase class derives from ScriptablObejct
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyModel", menuName = "MVP/Example/EnemyModel")]
    public class EnemyStatModel : ScriptableObjectModel
    {
        public BindableValueProperty<float> BaseMaxHp;
        
        public override void Reset()
        {
            BaseMaxHp.Reset();
        }
    }
}