using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLog : MonoBehaviour
{
    // �α� �ִϸ��̼� �������� ȣ��ȴ�.
    public void RemoveGameLog()
    {
        FindObjectOfType<GameManager>().RemoveGameLogInTheList(gameObject);
        Destroy(gameObject);
    }
}
