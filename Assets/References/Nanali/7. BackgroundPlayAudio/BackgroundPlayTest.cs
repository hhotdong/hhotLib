using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundPlayTest : MonoBehaviour
{
    //ui.
    public InputField timeField;

    BackgroundPlayAudioManager manager;

    private void Start()
    {
        manager = BackgroundPlayAudioManager.Instance;
        timeField.text = "0";
    }

    public void Play()
    {
        //StreamingAssets 폴더에 있는 파일이름을 사용해야 합니다.
        //하위폴더가 있는 경우, "폴더명/.../파일명" 과 같이 입력하면 됩니다.
        int seconds;
        int.TryParse(timeField.text, out seconds);

        manager.PlayAudio("bgm.ogg", seconds);
    }

    public void Stop()
    {
        manager.StopAudio();
    }
}
