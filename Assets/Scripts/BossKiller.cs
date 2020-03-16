using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossKiller : MonoBehaviour
{
    public void BossDied()
    {
        BossController.Boss.Die();
    }
}
