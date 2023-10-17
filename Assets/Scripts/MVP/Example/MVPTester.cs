using UnityEngine;

namespace hhotLib.Common.MVP.Example
{
    public class MVPTester : MonoBehaviour
    {
        [SerializeField] private EnemyModel enemyModel;
        [SerializeField] private GameObject viewPrefab;

        private void Start()
        {
            GameObject viewInstance = Instantiate(viewPrefab, Vector3.zero, Quaternion.identity);
            viewInstance.transform.SetParent(FindAnyObjectByType<Canvas>().transform, false);
            IEnemyView view = viewInstance.GetComponent<EnemyView>();
            new EnemyPresenter(view, enemyModel);
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(0, 0, 100, 100), "Get damage"))
            {
                FindAnyObjectByType<EnemyView>().GetDamage();
            }
        }
    }
}