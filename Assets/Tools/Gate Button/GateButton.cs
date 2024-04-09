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
        // 사용되지 않은 도구라면
        if (!isUsed)
        {
            isUsed = true;

            // 버튼이 눌렸다면, 색상을 변경한다.
            transform.GetComponentInChildren<MeshRenderer>().material = activatedMat;
            GameManager.instance.ActivatePortal();
        }
    }

    // 일정시간 뒤에 게이트버튼의 컷신을 종료한다(시네머신 카메라를 비활성화 한다).
    IEnumerator ActivateGateBtnVirtualCam()
    {
        GameManager.instance.IsWatching = true;

        yield return new WaitForSeconds(2f);

        GetComponentInChildren<CinemachineVirtualCamera>().gameObject.SetActive(false);
        GameManager.instance.IsWatching = false;
    }
}
