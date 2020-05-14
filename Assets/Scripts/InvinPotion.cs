using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvinPotion : Potions
{
    public int invinDist=10;
    
    protected override void PotionPayload()
    {
        base.PotionPayload();
        Debug.Log("INVINCIBLE!");
        GameController.instance.MakePlayerInvinReq(invinDist);
    }
}
