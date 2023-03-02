using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.HelpersUnity
{
    public class ObjectPool : MonoBehaviour
    {
        public GameObject objectToPool;
        public int amountToPool;
        public bool CanExpand = true;
        public Transform Parent;

        private List<GameObject> _pooledObjects;

        private void Start()
        {
            Transform parent = Parent == null ? this.gameObject.transform : Parent;

            _pooledObjects = new List<GameObject>();
            for (int i = 0; i < amountToPool; i++)
            {
                CreateObjectForPool(parent);
            }
        }

        public GameObject GetPooledObject()
        {
            for (int i = 0; i < amountToPool; i++)
            {
                if (!_pooledObjects[i].activeInHierarchy)
                {
                    return _pooledObjects[i];
                }
            }

            if (CanExpand)
            {
                Transform parent = Parent == null ? this.gameObject.transform : Parent;
                return CreateObjectForPool(parent);
            }
            else
            {
                return null;
            }
        }

        public List<GameObject> GetPoolActiveObjects()
        {
            var ret = _pooledObjects.Where(x => x.activeInHierarchy).ToList();
            return ret;
        }

        private GameObject CreateObjectForPool(Transform parent)
        {
            GameObject obj = Instantiate(objectToPool, parent);
            obj.SetActive(false);
            _pooledObjects.Add(obj);
            return obj;
        }
    }
}
