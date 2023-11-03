using UnityEngine;

namespace hhotLib.Common.MVP.Example
{
    public class MVPTester : MonoBehaviour
    {
        [SerializeField] private EnemyModelBase enemyModelBase;
        [SerializeField] private GameObject     viewPrefab;

        private void Start()
        {
            GameObject viewInstance = Instantiate(viewPrefab, Vector3.zero, Quaternion.identity);
            viewInstance.transform.SetParent(FindAnyObjectByType<Canvas>().transform, false);
            new EnemyPresenter(viewInstance.GetComponent<EnemyView>(), enemyModelBase, new EnemyModel());
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