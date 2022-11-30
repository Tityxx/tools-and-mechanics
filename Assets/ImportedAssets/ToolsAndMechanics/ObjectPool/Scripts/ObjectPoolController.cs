using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Zenject;

namespace ToolsAndMechanics.ObjectPool
{
    /// <summary>
    /// Контроллер пула объектов
    /// </summary>
    public class ObjectPoolController : MonoBehaviour
    {
        [SerializeField]
        private bool initOnAwake;
        [SerializeField]
        private PoolableObjectData[] dataList;

        [Inject]
        private DiContainer container;

        private Dictionary<PoolableObjectData, Queue<GameObject>> queue = new Dictionary<PoolableObjectData, Queue<GameObject>>();

        private void Awake()
        {
            if (initOnAwake) Init();
        }

        public void Init()
        {
            foreach (PoolableObjectData data in dataList)
            {
                for (int i = 0; i < data.InitCount; i++)
                {
                    GameObject go = CreateObject(data);
                    go.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Получить объект из пула
        /// </summary>
        public GameObject GetObject(PoolableObjectData data, bool active = true, Transform parent = null)
        {
            GameObject go = CreateObject(data);           

            go.transform.SetParent(parent);
            go.SetActive(active);
            return go;
        }

        /// <summary>
        /// Получить объект из пула с указанной позицией
        /// </summary>
        public GameObject GetObject(PoolableObjectData data, Vector3 position, bool isGlobal, bool active = true, Transform parent = null)
        {
            GameObject go = GetObject(data, false, parent);

            if (isGlobal) go.transform.position = position;
            else go.transform.localPosition = position;

            go.SetActive(active);
            return go;
        }

        /// <summary>
        /// Получить объект из пула с указанной позицией и поворотом
        /// </summary>
        public GameObject GetObject(PoolableObjectData data, Vector3 position, bool isGlobalPosition, Vector3 eulerAngles, bool isGlobalRotation, bool active = true, Transform parent = null)
        {
            GameObject go = GetObject(data, false, parent);

            if (isGlobalPosition) go.transform.position = position;
            else go.transform.localPosition = position;
            if (isGlobalRotation) go.transform.eulerAngles = eulerAngles;
            else go.transform.localEulerAngles = eulerAngles;

            go.SetActive(active);
            return go;
        }

        /// <summary>
        /// Вернуть объект в пул
        /// </summary>
        public void ReturnObject(GameObject go)
        {
            go.SetActive(false);
            go.transform.SetParent(null);

            if (go.TryGetComponent(out PoolableObjectInfo info))
            {
                if (queue.ContainsKey(info.Data))
                {
                    queue[info.Data].Enqueue(go);
                }
                else
                {
                    queue.Add(info.Data, new Queue<GameObject>(new[] { go }));
                }
            }
        }

        private GameObject CreateObject(PoolableObjectData data)
        {
            GameObject go;

            if (queue.ContainsKey(data))
            {
                if (queue[data].Count > 0)
                {
                    go = queue[data].Dequeue();
                }
                else
                {
                    go = container.InstantiatePrefab(data.Prefab);
                    AddInfoComponent(data, go);
                }
            }
            else
            {
                queue.Add(data, new Queue<GameObject>());
                go = container.InstantiatePrefab(data.Prefab);
                AddInfoComponent(data, go);
            }

            return go;
        }

        private void AddInfoComponent(PoolableObjectData data, GameObject go)
        {
            go.AddComponent<PoolableObjectInfo>().Data = data;
        }
    }
}