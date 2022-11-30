using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ToolsAndMechanics.ObjectPool
{
    /// <summary>
    /// Пример возврата объекта в пул
    /// </summary>
    public class ExampleObjectRemover : MonoBehaviour
    {
        [SerializeField]
        private float delay = 1f;

        [Inject]
        private ObjectPoolController pool;

        private void OnEnable()
        {
            StartCoroutine(RemoveObjectWithDelay(delay));
        }

        private IEnumerator RemoveObjectWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            pool.ReturnObject(gameObject);
        }
    }
}