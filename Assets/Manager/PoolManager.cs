using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    [SerializeField] GameObject[] prefabs;

    List<GameObject>[] objPools;
    
    int poolSize = 1;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        InitObjPool();
    }

    void InitObjPool()
    {
        objPools = new List<GameObject>[prefabs.Length];

        for (int i = 0; i < prefabs.Length; i++)
        {
            objPools[i] = new List<GameObject>();

            for (int j = 0; j < poolSize; j++)
            {
                GameObject obj = Instantiate(prefabs[i]);
                obj.SetActive(false);
                objPools[i].Add(obj);
            }
        }
    }

    public GameObject ActiveObj(int _index, Vector3 _pos)
    {
        GameObject obj = null;

        for (int i = 0; i < objPools[_index].Count; i++)
        {
            // 풀 안의 특정 오브젝트가 비활성화 되어있는지 확인한다.
            if (!objPools[_index][i].activeInHierarchy)
            {
                obj = objPools[_index][i];
                obj.transform.position = _pos;
                obj.SetActive(true);
                return obj;
            }
        }

        // 풀 안의 모든 오브젝트가 사용중이라면(활성화 상태) 새로 만들어 풀에 추가한다.
        obj = Instantiate(prefabs[_index]);
        objPools[_index].Add(obj);
        obj.transform.position = _pos;
        obj.SetActive(true);

        return obj;
    }
}
