using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Actor
{
    private void Awake()
    {
        base.Awake();
        isPlayer = false;
    }
}
