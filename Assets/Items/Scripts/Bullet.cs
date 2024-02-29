using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Item
{
    public override void Use(GameObject target)
    {
        Pistol pistol = target.GetComponent<PlayerManager>().Pistol;

        pistol.MaxBullet = 10;
    }
}