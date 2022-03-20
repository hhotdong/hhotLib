//using UnityEngine;
//using UnityEngine.UI;
//using ScriptableObjectArchitecture;

//[RequireComponent(typeof(Button))]
//public class UIButtonClicked : MonoBehaviour
//{
//    public int Parameter { get; set; }

//    [Header("SO Events")]
//    [SerializeField] private IntGameEvent _OnButtonClicked = default(IntGameEvent);

//    private void Awake()
//    {
//        GetComponent<Button>().onClick.AddListener(OnClickButton);
//    }

//    private void OnDestroy()
//    {
//        GetComponent<Button>().onClick.RemoveListener(OnClickButton);
//    }

//    protected virtual void OnClickButton()
//    {
//        SoundManager.PlaySoundEffect(SoundType.BUTTON_DEFAULT, 0.5F);
//        _OnButtonClicked?.Raise(Parameter);
//    }
//}
