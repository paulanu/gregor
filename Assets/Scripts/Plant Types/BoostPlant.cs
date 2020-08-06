using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPlant : MonoBehaviour, Plant
{
    [SerializeField] Sprite on, off;
    [SerializeField] BoxCollider2D bounceCollider;  

    private SpriteRenderer _spriteRenderer; 

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        bounceCollider.enabled = false; 
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Activate()
    {
        Debug.Log("activated!");
        _spriteRenderer.sprite = on;
        bounceCollider.enabled = true;
    }

    public void Deactivate()
    {
        Debug.Log("deactivated!");
        _spriteRenderer.sprite = off;
        bounceCollider.enabled = false;
    }

    public void OnUse()
    {
        Deactivate(); 
    }

   
}
