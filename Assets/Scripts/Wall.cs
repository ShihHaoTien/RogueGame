using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public AudioClip chop1;
    public AudioClip chop2;
    public Sprite dmgSprite;
    public int hp=4;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer=GetComponent<SpriteRenderer>();
    }

    //change hp&img, destory when hp=0
    public void DamageWall(int loss)
    {
        spriteRenderer.sprite=dmgSprite;
        hp-=loss;
        SoundManager.instance.RandomizeSfx(chop1,chop2);
        if(hp<=0)
        {
            gameObject.SetActive(false);
        }
    }
}
