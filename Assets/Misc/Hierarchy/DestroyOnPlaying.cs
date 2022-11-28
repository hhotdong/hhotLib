using System.Collections;
using UnityEngine;

namespace hhotLib.Common
{
    public class DestroyOnPlaying : MonoBehaviour
    {
        [SerializeField] private bool destroyWhenNoChildren = false;

        private IEnumerator Start()
        {
            if (destroyWhenNoChildren == false)
            {
                Destroy(gameObject);
                yield break;
            }

            while (transform.childCount > 0)
                yield return null;

            Destroy(gameObject);
        }
    }
}