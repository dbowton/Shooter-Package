using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Wildlife : AI
{
    public override void Die()
    {
        GameManager.Instance.player.playerData.inventory.money += 100;
        base.Die();
    }
}
