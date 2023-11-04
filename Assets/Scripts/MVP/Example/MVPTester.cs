using UnityEngine;

namespace hhotLib.Common.MVP.Example
{
    public class MVPTester : MonoBehaviour
    {
        [SerializeField] private EnemyStatModel enemyStatModel;
        [SerializeField] private GameObject     viewPrefab;

        private void Start()
        {
            GameObject viewInstance = Instantiate(viewPrefab, Vector3.zero, Quaternion.identity);
            viewInstance.transform.SetParent(FindAnyObjectByType<Canvas>().transform, false);
            new EnemyPresenter(viewInstance.GetComponent<EnemyView>(), new EnemyModel(), enemyStatModel);
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(0, 0, 100, 100), "Take damage"))
            {
                FindAnyObjectByType<EnemyView>().TakeDamage(1.0f);
            }
        }
    }
}