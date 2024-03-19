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
        // 사용되지 않은 도구라면
        if (!isUsed)
        {
            isUsed = true;

            // 버튼이 눌렸다면, 색상을 변경한다.
            transform.GetComponentInChildren<MeshRenderer>().material = activatedMat;
            StartCoroutine(ActiveGate());
        }
    }

    IEnumerator ActiveGate()
    {
        yield return new WaitForSeconds(2f);

        // 포탈 프리팹을 인스턴스화 한다.
        GameObject portalInstObj = Instantiate(portalObj, portalPointObj.transform);
        virtualCamObj.SetActive(true);

        // 시네머신 카메라가 포탈을 바라보게 한다.
        virtualCamObj.GetComponent<CinemachineVirtualCamera>().Follow = portalInstObj.transform.GetChild(1);
        GameManager.instance.IsWatching = true;

        yield return new WaitForSeconds(2f);

        virtualCamObj.SetActive(false);
        GameManager.instance.IsWatching = false;
    }
}
