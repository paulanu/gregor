using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatchCo;
public class BoostPlant : MonoBehaviour, Plant
{
    [Header("How much gregor go boing")]
    public float boostForce = 300;

    [Header("For level design use only. Set BEFORE hitting play.")]
    [SerializeField] private bool activated; // THIS IS PURELY FOR USE OF LEVEL DESIGNER

    private BoxCollider2D _bounceCollider;  
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;


    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = this.GetComponent<SpriteRenderer>();
        _bounceCollider = this.GetComponent<BoxCollider2D>();
        _animator = this.GetComponent<Animator>();
        
        if (activated)
        {
            Activate(); 
        }
    }

   
    [ExposeMethodInEditor]
    public void Activate()
    {

        _animator.SetBool("Activated", true);
        activated = true;
    }

    [ExposeMethodInEditor]
    public void Deactivate()
    {
        _animator.SetBool("Activated", false);
        activated = false;
    }

    public void OnUse()
    {
        Deactivate(); 
    }

   
}
