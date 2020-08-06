using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GregorController : MonoBehaviour
{
    [SerializeField] private float jumpForce = 200f;
    [SerializeField] private float movementSpeed = 400f;
    [SerializeField] private Transform groundedCheck;
    [SerializeField] private float groundedRadius = .2f;
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private LayerMask plantLayers;
    [SerializeField] bool useFixedJump;
    [SerializeField] private GameObject plantCheck;

    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private ParticleSystem _waterParticles;
    private Transform _transform; 
    private float _movementForce;
    private bool _jump;
    private bool _water;
    
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _waterParticles = GetComponentInChildren<ParticleSystem>();
        _transform = GetComponent<Transform>(); 
        _movementForce = 0f;
        _jump = false;
        _water = false; 
    }

    // Update is called once per frame
    void Update()
    {
        //get inputs from player
        _movementForce = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            _jump = true; 
        }

        if (Input.GetButtonDown("Water") && IsGrounded()) //WATER ON GROUND ONLY? 
        {
            _water = true;
        }
    }

    private void FixedUpdate()
    {
        //use inputs to move and jump
        Move();
        if (_jump)
        {
            Jump();
        }
        if(_water)
        {
            Water();
        }
    }

    private void Move()
    {
        if (useFixedJump && !IsGrounded())
        {
            //account for direction
            float horizontalVelocity = movementSpeed;

            horizontalVelocity *= _spriteRenderer.flipX == true ?  -1 : 1;

            //apply fixed velocity, no air control 
            _rigidbody.velocity = new Vector2(horizontalVelocity, _rigidbody.velocity.y);
            return; 
        }

        //flip sprite if moving - use scale so colliders/particles also affected
        if (_movementForce < 0 && _transform.localScale.x > 0)
        {
            _transform.localScale = new Vector3(_transform.localScale.x * -1, _transform.localScale.y, _transform.localScale.z); 
        }
        else if (_movementForce > 0 && _transform.localScale.x < 0)
        {
            _transform.localScale = new Vector3(_transform.localScale.x * -1, _transform.localScale.y, _transform.localScale.z);
        }

        //apply that movement
        _rigidbody.velocity = new Vector2(_movementForce * movementSpeed, _rigidbody.velocity.y);
    }

    //literally just jumping
    private void Jump()
    {
        _jump = false; 
        _rigidbody.AddForce(Vector2.up * jumpForce);
    }

    //turn on water particles, check for plants being watered
    private void Water()
    {
        _water = false;       
        _waterParticles.Play();
        //Collider[] hitColliders = Physics.OverlapBox(plantCheck.transform.position, plantCheck.GetComponent<BoxCollider2D>().size, Quaternion.identity, plantLayers);
        //foreach (Collider c in hitColliders)
        //    Debug.Log(c.name);

    }

    private bool IsGrounded()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundedCheck.position, groundedRadius, groundLayers);
     
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                return true;
            }
        }
        return false; 
    }
}
