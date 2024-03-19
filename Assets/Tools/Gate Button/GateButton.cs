using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateButton : Tool
{
    [SerializeField] GameObject virtualCamObj;
    [SerializeField] Material activatedMat;
    [SerializeField] GameObject portalObj;
    [SerializeField] GameObject portalPointObj;

    override public void InteractObject()
    {
        // ������ ���� �������
        if (!isUsed)
        {
            isUsed = true;

            // ��ư�� ���ȴٸ�, ������ �����Ѵ�.
            transform.GetComponentInChildren<MeshRenderer>().material = activatedMat;
            StartCoroutine(ActiveGate());
        }
    }

    IEnumerator ActiveGate()
    {
        yield return new WaitForSeconds(2f);

        // ��Ż �������� �ν��Ͻ�ȭ �Ѵ�.
        GameObject portalInstObj = Instantiate(portalObj, portalPointObj.transform);
        virtualCamObj.SetActive(true);

        // �ó׸ӽ� ī�޶� ��Ż�� �ٶ󺸰� �Ѵ�.
        virtualCamObj.GetComponent<CinemachineVirtualCamera>().Follow = portalInstObj.transform.GetChild(1);
        GameManager.instance.IsWatching = true;

        yield return new WaitForSeconds(2f);

        virtualCamObj.SetActive(false);
        GameManager.instance.IsWatching = false;
    }
}
