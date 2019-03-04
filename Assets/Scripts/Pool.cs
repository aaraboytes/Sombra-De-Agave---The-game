using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    public static Pool _instance;
    private void Awake()
    {
        _instance = this;
    }
    [System.Serializable]
    public class PooledObj
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
    public List<PooledObj> pools;
    Dictionary<string, Queue<GameObject>> dictionaryPools;
    void Start()
    {
        dictionaryPools = new Dictionary<string, Queue<GameObject>>();
        foreach(PooledObj pool in pools)
        {
            Queue<GameObject> currentPool = new Queue<GameObject>();
            GameObject bucket = new GameObject("Bucket_" + pool.tag);
            for(int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab,bucket.transform);
                obj.SetActive(false);
                currentPool.Enqueue(obj);
            }
            dictionaryPools.Add(pool.tag, currentPool);
        }
    }
    public GameObject SpawnPooledObj(string tag,Vector3 position,Quaternion rotation)
    {
        if (!dictionaryPools.ContainsKey(tag))
        {
            Debug.LogError("The tag " + tag + " doesnt exist");
            return null;
        }
        GameObject obj = dictionaryPools[tag].Dequeue();
        if (obj.activeInHierarchy) {
            dictionaryPools[tag].Enqueue(obj);
            return null;
        }
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);
        dictionaryPools[tag].Enqueue(obj);
        return obj;
    }
    public void HideAllItems(string tag)
    {
        if (!dictionaryPools.ContainsKey(tag))
        {
            Debug.LogError("The tag " + tag + " doesnt exist");
        }
        foreach(GameObject pooledObj in dictionaryPools[tag])
        {
            if (pooledObj.GetComponent<Item>())
            {
                pooledObj.GetComponent<Item>().Reinitialize();
            }else if (pooledObj.GetComponent<Tequila>())
            {
                pooledObj.GetComponent<Tequila>().Reinitialize();
            }
            pooledObj.SetActive(false);
        }
    }
}
