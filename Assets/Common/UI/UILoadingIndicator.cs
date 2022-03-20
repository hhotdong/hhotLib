using System.Collections;
using UnityEngine;

public class UILoadingIndicator : MonoBehaviour
{
    private Transform tr;
    private Coroutine rotationCoroutine;

    private void Start()
    {
        tr = transform;
    }

    private void OnEnable()
    {
        StopRotating();
        StartCoroutine(Rotate());
    }

    private void OnDisable()
    {
        StopRotating();
    }

    private IEnumerator Rotate()
    {
        const float SPEED = 35.0F;

        while(true)
        {
            yield return null;
            tr.Rotate(Vector3.forward, -Time.deltaTime * SPEED, Space.Self);
        }
    }

    private void StopRotating()
    {
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
            rotationCoroutine = null;
        }
    }
}
