using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateButton : Tool
{
    [SerializeField] GameObject virtualCamObj;
    [SerializeField] Material activatedMat;

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

        virtualCamObj.SetActive(true);
        GameManager.instance.IsWatching = true;

        yield return new WaitForSeconds(2f);

        virtualCamObj.SetActive(false);
        GameManager.instance.IsWatching = false;
    }
}
