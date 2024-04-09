using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateButton : Tool
{
    [SerializeField] GameObject virtualCamObj;
    [SerializeField] Material activatedMat;

    void Start()
    {
        StartCoroutine(ActivateGateBtnVirtualCam());
    }

    override public void InteractObject()
    {
        // ������ ���� �������
        if (!isUsed)
        {
            isUsed = true;

            // ��ư�� ���ȴٸ�, ������ �����Ѵ�.
            transform.GetComponentInChildren<MeshRenderer>().material = activatedMat;
            GameManager.instance.ActivatePortal();
        }
    }

    // �����ð� �ڿ� ����Ʈ��ư�� �ƽ��� �����Ѵ�(�ó׸ӽ� ī�޶� ��Ȱ��ȭ �Ѵ�).
    IEnumerator ActivateGateBtnVirtualCam()
    {
        GameManager.instance.IsWatching = true;

        yield return new WaitForSeconds(2f);

        GetComponentInChildren<CinemachineVirtualCamera>().gameObject.SetActive(false);
        GameManager.instance.IsWatching = false;
    }
}
