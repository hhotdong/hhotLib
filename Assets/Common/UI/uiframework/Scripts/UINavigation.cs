// TODO: 뒤로가기(안드로이드 백버튼) 대응
// TODO: 스크린 프리팹과 아틀라스 비동기 로드
using UnityEngine;
using hhotLib.Common;

namespace deVoid.UIFramework
{
    [CreateAssetMenu(fileName = "UINavigation", menuName = "deVoid UI/UINavigation")]
    public class UINavigation : SingletonScriptableObject<UINavigation>
    {
        [SerializeField] private UISettings screenCamSettings;

        public void Initialize()
        {
            screenCamSettings.CreateUIInstance(true, true);
        }
    }
}