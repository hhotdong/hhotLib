using System.Collections;
using UnityEngine;

namespace hhotLib.Common
{
    public class UILoadingIndicator : MonoBehaviour
    {
        [SerializeField] private float rotSpeed = 35.0F;

        private Transform tr;
        private Coroutine rotateCoroutine;

        private IEnumerator RotateCoroutine()
        {
            while(true)
            {
                yield return null;
                tr.Rotate(Vector3.forward, -Time.unscaledDeltaTime * rotSpeed, Space.Self);
            }
        }

        private void StopRotating()
        {
            if (rotateCoroutine != null)
            {
                StopCoroutine(rotateCoroutine);
                rotateCoroutine = null;
            }
        }

        private void Awake()
        {
            tr = transform;
        }

        private void OnEnable()
        {
            StopRotating();
            StartCoroutine(RotateCoroutine());
        }

        private void OnDisable()
        {
            StopRotating();
        }
    }
}