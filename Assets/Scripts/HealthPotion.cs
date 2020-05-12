using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : Potions
{
    public int addHP=30;
    protected override void PotionPayload()
    {
        base.PotionPayload();
        Debug.Log("REQ ADD HP");
        GameController.instance.AddHPReq(addHP);
    }

}
