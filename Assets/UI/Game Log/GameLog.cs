using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLog : MonoBehaviour
{
    // 로그 애니메이션 마지막에 호출된다.
    public void RemoveGameLog()
    {
        FindObjectOfType<GameManager>().RemoveGameLogInTheList(gameObject);
        Destroy(gameObject);
    }
}
