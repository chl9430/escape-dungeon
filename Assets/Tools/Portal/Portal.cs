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

    // �����ð� �ڿ� ��Ż�� �ƽ��� �����Ѵ�(�ó׸ӽ� ī�޶� ��Ȱ��ȭ �Ѵ�).
    IEnumerator ActivatePortalVirtualCam()
    {
        GameManager.instance.IsWatching = true;

        yield return new WaitForSeconds(2f);

        GetComponentInChildren<CinemachineVirtualCamera>().gameObject.SetActive(false);
        GameManager.instance.IsWatching = false;
    }
}
