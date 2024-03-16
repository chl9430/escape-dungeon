using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateButton : Tool
{
    [SerializeField] GameObject virtualCamObj;
    [SerializeField] Material activatedMat;

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

        virtualCamObj.SetActive(true);
        GameManager.instance.IsWatching = true;

        yield return new WaitForSeconds(2f);

        virtualCamObj.SetActive(false);
        GameManager.instance.IsWatching = false;
    }
}
