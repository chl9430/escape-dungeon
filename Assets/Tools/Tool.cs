using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Tool : MonoBehaviour
{
    protected bool isUsed;

    abstract public void InteractObject();
}
