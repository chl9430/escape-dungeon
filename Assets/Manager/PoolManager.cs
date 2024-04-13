using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    [SerializeField] GameObject[] prefabs;
    [SerializeField] GameObject[] monObjs;

    List<GameObject>[] objPools;
    List<GameObject>[] monObjList;

    int poolSize = 1;

    void Awake()
    {
        instance = this;

        InitObjPool();
        InitMonObjPool();
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

    void InitMonObjPool()
    {
        monObjList = new List<GameObject>[monObjs.Length];

        for (int i = 0; i < monObjs.Length; i++)
        {
            monObjList[i] = new List<GameObject>();

            GameObject monObj = Instantiate(monObjs[i]);
            monObj.SetActive(false);
            monObjList[i].Add(monObj);
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

    public void SetMonActive(Transform _transform)
    {
        int selectedMonIdx = Random.Range(0, monObjList.Length);

        List<GameObject> selectedMonList = monObjList[selectedMonIdx];

        for (int i = 0; i < selectedMonList.Count; i++)
        {
            // 씬에 비활성화 되어있는게 있다면
            if (!selectedMonList[i].activeInHierarchy)
            {
                selectedMonList[i].transform.parent = _transform;
                selectedMonList[i].transform.position = _transform.position;

                selectedMonList[i].SetActive(true);

                return;
            }
        }

        // 비활성화 되어있는게 없다면
        GameObject monObj = Instantiate(monObjs[selectedMonIdx], _transform);
        monObjList[selectedMonIdx].Add(monObj);
    }
}
