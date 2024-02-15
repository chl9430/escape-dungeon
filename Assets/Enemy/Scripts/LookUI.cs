using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookUI : MonoBehaviour
{
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        if (cam == null)
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (cam != null)
        {
            // 카메라가 어딜 보고 있는지와 상관없이 현재 카메라의 전방방향을 바라보고(즉, 나를 바라보고)
            // 카메라가 어딜 보고 있는지와 상관없이 y축만 회전시킨다.
            transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
                cam.transform.rotation * Vector3.up);
        }
    }
}
