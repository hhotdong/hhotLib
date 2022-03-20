using UnityEngine;

namespace hhotLib
{
    public class DestroyOnPlaying : MonoBehaviour
    {
        [SerializeField] private bool destroyWhenNoChildren = false;

        private void Start()
        {
            if (!destroyWhenNoChildren)
                Destroy(this.gameObject);
        }

        private void Update()
        {
            if (!destroyWhenNoChildren)
                Destroy(this.gameObject);
            else if (transform.childCount == 0 && !IsInvoking())
                Invoke("DestroyAfterInterval", 1.0f);
        }

        private void DestroyAfterInterval()
        {
            Destroy(this.gameObject);
        }
    }
}