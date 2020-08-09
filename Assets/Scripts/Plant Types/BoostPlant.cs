using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPlant : MonoBehaviour, Plant
{
    public float boostForce = 300; 

    private BoxCollider2D _bounceCollider;  
    private SpriteRenderer _spriteRenderer;
    private Animator _animator; 

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = this.GetComponent<SpriteRenderer>();
        _bounceCollider = this.GetComponent<BoxCollider2D>();
        _animator = this.GetComponent<Animator>(); 
    }

    // Update is called once per frame
    void Update()
    {
        //int w = _animator.GetCurrentAnimatorClipInfo(0).Length;
        //string[] clipName = new string[w];
        //for (int i = 0; i < w; i += 1)
        //{
        //    clipName[i] = _animator.GetCurrentAnimatorClipInfo(0)[i].clip.name;
        //    Debug.Log(clipName[i]);
        //}
    }

    public void Activate()
    {

        _animator.SetBool("Activated", true); 

    }

    public void Deactivate()
    {
        _animator.SetBool("Activated", false);

    }

    public void OnUse()
    {
        Deactivate(); 
    }

   
}
