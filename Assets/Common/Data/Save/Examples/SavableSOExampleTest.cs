using UnityEngine;

namespace hhotLib.Save.Example {
    public class SavableSOExampleTest : MonoBehaviour
    {
        [SerializeField] SavableSO[] soData;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SaveLoadSystem.Reset();
            }
        }
    }
}