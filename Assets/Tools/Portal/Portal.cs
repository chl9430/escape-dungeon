using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Tool
{
    void Start()
    {
        StartCoroutine(ActivatePortalVirtualCam());
    }

    override public void InteractObject()
    {
        GameManager.instance.SetGameClearUI();
    }

    // 일정시간 뒤에 포탈의 컷신을 종료한다(시네머신 카메라를 비활성화 한다).
    IEnumerator ActivatePortalVirtualCam()
    {
        GameManager.instance.IsWatching = true;

        yield return new WaitForSeconds(2f);

        GetComponentInChildren<CinemachineVirtualCamera>().gameObject.SetActive(false);
        GameManager.instance.IsWatching = false;
    }
}
