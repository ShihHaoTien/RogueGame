using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potions : MonoBehaviour
{
    public string potionName;
    public string potionExplanation;
    public string potionQuote;
    public bool expireImmediately;
    public GameObject specialEffect;
    public AudioClip soundEffect;
    protected Player player;
    protected SpriteRenderer spriteRenderer;
    //protected Animator animator;
    //FA
    protected enum PotionStates
    {
        InAttractMode,
        IsCollected,
        IsExpiring
    }
    protected PotionStates potionState;

    protected virtual void Awake()
    {
        //animator=GetComponent<Animator>();
        spriteRenderer=GetComponent<SpriteRenderer>();
        //correct position
        gameObject.transform.position=gameObject.transform.position-new Vector3(0,0.3f,0);
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        potionState=PotionStates.InAttractMode;
    }

    //2D
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        PotionCollected(other.gameObject);
    }
    //3D
    protected virtual void OnTriggerEnter(Collider other)
    {
        PotionCollected(other.gameObject);
    }
    protected virtual void PotionCollected(GameObject picker)
    {
        if(picker.tag!="Player") return;
        if(potionState==PotionStates.IsCollected || potionState==PotionStates.IsExpiring) return;
        //state is attract,then can collect
        potionState=PotionStates.IsCollected;
        player=picker.GetComponent<Player>();
        //Move to player
        this.gameObject.transform.SetParent(player.gameObject.transform);
        this.gameObject.transform.position=player.gameObject.transform.position;
        PotionEffect();
        PotionPayload();
        //SEND MESSAGE AND DISABLED
        //.....
        //animator.enabled=false;
        spriteRenderer.enabled=false;
    }

    protected virtual void PotionEffect()
    {
        if(specialEffect!=null)
        {
            Instantiate(specialEffect,transform.position,transform.rotation,transform);
        }
        if(soundEffect!=null)
        {
            SoundManager.instance.PlaySingle(soundEffect);
        }
    }

    protected virtual void PotionPayload()
    {
        Debug.Log("Now potion load:"+gameObject.name);
        if(expireImmediately)
        {
            PotionHasExpired();//expire this potion
        }
    }
    
    protected virtual void PotionHasExpired()
    {
        if(potionState==PotionStates.IsExpiring) return;
        potionState=PotionStates.IsExpiring;
        //SEND Message to loader
        Debug.Log("potion expired");
        this.enabled=false;
        Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
