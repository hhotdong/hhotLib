using UnityEngine;
using hhotLib.Common;

public class TestController : DebugOnlyMonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SoundManager.Instance.PlaySoundEffect(AudioClipName.BUTTON_CLICK_A, 1.0f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SoundManager.Instance.PlaySoundEffect(AudioClipName.BUTTON_CLICK_B, 1.0f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SoundManager.Instance.PlaySoundEffectDelayed(AudioClipName.BUTTON_CLICK_B, 1.0f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SoundManager.Instance.PlayRandomSoundEffect(AudioClipGroupName.BUTTON_A, 1.0f);
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "Load test scene"))
        {
            Debug.Log("Load Battle Scene");
            SceneLoader.Instance.Load("Battle", true, true, "Base");
        }
    }
}
