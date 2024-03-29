﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hhotLib.Common
{
    public class LocalObjectPool : MonoBehaviour
    {
        public int MaxPoolAmount => maxPoolAmount;

        [SerializeField] private GameObject poolObjectPrefab;
        [SerializeField] private int        initPoolAmount  =  2;
        [SerializeField] private int        shrinkThreshold =  6;
        [SerializeField] private int        maxPoolAmount   = 10;

        private Transform tr;

        private readonly HashSet<GameObject> poolObjects       = new HashSet<GameObject>();
        private readonly Queue<GameObject>   unusedPoolObjects = new Queue<GameObject>();

        public GameObject Get()
        {
            if (unusedPoolObjects.Count < 1)
            {
                if (poolObjects.Count >= maxPoolAmount)
                {
                    Debug.LogWarning($"Pool object count is greater than or same as max({maxPoolAmount})!");
                    return null;
                }
                GameObject newObj = CreatePoolObject(false);
                poolObjects.Add(newObj);
                unusedPoolObjects.Enqueue(newObj);
            }
            var obj = unusedPoolObjects.Dequeue();
            obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            obj.SetActive(true);
            return obj;
        }

        public void Free(GameObject obj)
        {
            if (obj == null)
            {
                Debug.LogError($"Failed to free GameObject! It's null!");
                return;
            }

            if (poolObjects.Contains(obj) == false)
            {
                Debug.LogError($"Failed to free GameObject! It doesn't belong to this pool!");
                return;
            }

            if (poolObjects.Count > shrinkThreshold)
            {
                poolObjects.Remove(obj);
                Destroy(obj);
                return;
            }

            obj.SetActive(false);
            unusedPoolObjects.Enqueue(obj);
        }

        private GameObject CreatePoolObject(bool isInit)
        {
            GameObject obj = Instantiate(poolObjectPrefab, Vector3.zero, Quaternion.identity, tr);
            obj.SetActive(!isInit);
            return obj;
        }

        private void Awake()
        {
            tr = GetComponent<Transform>();
        }

        private IEnumerator Start()
        {
            if (shrinkThreshold < initPoolAmount || shrinkThreshold > maxPoolAmount)
                shrinkThreshold = (initPoolAmount + maxPoolAmount) / 2;

            for (int i = 0; i < initPoolAmount; i++)
            {
                GameObject newObj = CreatePoolObject(true);
                poolObjects.Add(newObj);
                unusedPoolObjects.Enqueue(newObj);
                yield return null;
            }
        }
    }
}