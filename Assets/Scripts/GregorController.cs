using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GregorController : MonoBehaviour
{
    [Tooltip("Controlling Gregor's jump height")]
    [SerializeField] private float jumpForce = 200f;
    [Tooltip("Controlling Gregor's movement speed")]
    [SerializeField] private float movementSpeed = 400f;
    
    [Tooltip("Input what is used as a Gregor's ground check")]
    [SerializeField] private Transform groundedCheck;
    [Tooltip("How many units around the ground check to search for ground.")]
    [SerializeField] private float groundedRadius = .2f;
    
    [Tooltip("Which layers are ground?")]
    [SerializeField] private LayerMask groundLayers;
    [Tooltip("Which layers are plants?")]
    [SerializeField] private LayerMask plantLayers;
    [Tooltip("Which layers have water?")]
    [SerializeField] private LayerMask waterSourceLayers;
    
    [Tooltip("Check to toggle a fixed jump.")]
    [SerializeField] private bool useFixedJump;
    
    [Tooltip("What is used to check for plants, water sources, etc. in front of Gregor.")]
    [SerializeField] private Transform interactCheck;
    [Tooltip("Visual component so you can see :)")]
    [SerializeField] private bool hideInteractCheckVisual;
    
    [Tooltip("How long is Gregor paused while he is watering? In seconds.")]
    [SerializeField] private float wateringTime = 1f; 

    // components
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private ParticleSystem _waterParticles;
    private Transform _transform;
    private Animator _animator; 

    // axis of movement
    private float _movementForce;

    // used for keeping track of gregor's jump
    private bool _jump;
    private bool _inJump;
    private bool _jumpAllowed;

    // keep track of watering
    [SerializeField] private int waterQuantity;
    [SerializeField] private int maxWater = 1; 
    private bool _water;
    private bool _collectWater; 

    // pause player control of gregor - used for watering, etc, playing animations
    private bool _freezeControl;
    private float _startFreezeTime;

    // getting plant check from child obj
    private SpriteRenderer _plantCheckVisual;


    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _waterParticles = GetComponentInChildren<ParticleSystem>();
        _transform = GetComponent<Transform>();
        _animator = GetComponent<Animator>(); 
        _movementForce = 0f;
        _jump = false;
        _inJump = false;
        _jumpAllowed = true;
        _water = false;
        _collectWater = false; 
        waterQuantity = 0; 
        _plantCheckVisual = interactCheck.GetComponent<SpriteRenderer>(); 
    }

    // get inputs from player
    void Update()
    {
        // if gregor's controls are frozen, should they be thawed? (lol) 
        if ((Time.time - _startFreezeTime) > wateringTime)
        {
            _freezeControl = false;
        }

        // get direction of movement
        _movementForce = Input.GetAxis("Horizontal");

        // jump ? 
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            _jump = true; 
        }

        // water ? 
        if (Input.GetButtonDown("Water") && IsGrounded()) //WATER ON GROUND ONLY? 
        {
            _water = true;
        }

        // collect water ? 
        if (Input.GetButtonDown("CollectWater"))
        {
            _collectWater = true;
        }

        // hide plant check visual if desired
        if (hideInteractCheckVisual) _plantCheckVisual.color = Color.clear;
        else _plantCheckVisual.color = Color.green; 
    }

    // use inputs to do appropriate actions
    private void FixedUpdate()
    {
        if (!_freezeControl)
            Move();
        if (_jump && !_freezeControl) 
        {
            Jump();
        }
        if (_water)
        {
            Water();
        }
        if (_collectWater)
        {
            CollectWater(); 
        }
    }

    private void Move()
    {
        // fixed jump 
        if (useFixedJump && !IsGrounded() && _inJump)
        {
            // account for direction
            float horizontalVelocity = movementSpeed;

            horizontalVelocity *= _transform.localScale.x < 0 ?  -1 : 1;

            // apply fixed velocity, no air control 
            _rigidbody.velocity = new Vector2(horizontalVelocity, _rigidbody.velocity.y);
            return; 
        }

        // flip sprite if moving - use scale so colliders/particles also affected
        if (_movementForce < 0 && _transform.localScale.x > 0)
        {
            _transform.localScale = new Vector3(_transform.localScale.x * -1, _transform.localScale.y, _transform.localScale.z); 
        }
        else if (_movementForce > 0 && _transform.localScale.x < 0)
        {
            _transform.localScale = new Vector3(_transform.localScale.x * -1, _transform.localScale.y, _transform.localScale.z);
        }

        // apply that movement
        _rigidbody.velocity = new Vector2(_movementForce * movementSpeed, _rigidbody.velocity.y);
    }

    // literally just jumping
    private void Jump()
    {
        _jump = false;
        _inJump = true;

        // only jump if nothing is currently boosting gregor up (No Super Jumps!!!!)
        if (_rigidbody.velocity.y <= 0)
            _rigidbody.AddForce(Vector2.up * jumpForce);
    }

    // returns if Gregor has jumped lol
    public bool hasGregorJumped()
    {
        return _inJump; 
    }

    // this is used when an outside force manipulates Gregor's movement, and you don't want jumping to fuck it up
    // for example: when both the force and the jump are registered at the same time, then they can multiply if the jump comes after
    public void pauseJump()
    {
        _jump = false;
        _inJump = false;
        _jumpAllowed = false; 
    }

    private void CollectWater()
    {
        // The player can't move Gregor until he is done collecting water
        FreezeControlOfGregor();
        RemoveHorizontalVelocity();

        _collectWater = false; 

        //check for water in front of gregor
        Collider2D[] hitColliders = InteractWithObjectsInLayer(waterSourceLayers);

        int collectedWater = 0; 
        foreach (Collider2D c in hitColliders) //GIT that water 
        {
            collectedWater += c.gameObject.GetComponent<WaterSource>().GetWater();
        }

        // if gregor has collected water (and doesn't already have water), play the water anim
        if (collectedWater > 0 && waterQuantity == 0)
            _animator.SetBool("CollectedWater", true);

        // now change the water quantity 
        waterQuantity = Math.Min(collectedWater, maxWater);
    }

    // turn on water particles, check for plants being watered
    private void Water()
    {
        // The player can't move Gregor until he is done watering
        FreezeControlOfGregor(); 
        RemoveHorizontalVelocity(); 

        _water = false;

        if (waterQuantity > 0)
        {
            waterQuantity--; 
            _waterParticles.Play();

            // play watering anim
            _animator.SetBool("CollectedWater", false);

            // check for plants being watered
            Collider2D[] hitColliders = InteractWithObjectsInLayer(plantLayers);

            foreach (Collider2D c in hitColliders) //activate those plants
                c.gameObject.GetComponent<Plant>().Activate();
        }
        else
            Debug.Log("aint got no water!!!!!");

    }

    private Collider2D[] InteractWithObjectsInLayer(LayerMask layerMask)
    {
        // interactCheck sprite is 100x100, 1 unit per 100 pixels, so interact check scale correlates 1:1 with world scale, BUT multiplied by Gregor's parent scale to affect for that. I think??? 
        float interactCheckWorldWidth = Mathf.Abs(interactCheck.transform.localScale.x * transform.localScale.x);
        float interactCheckWorldHeight = Mathf.Abs(interactCheck.transform.localScale.y * transform.localScale.y);

        // now get all colliders in the interactCheck bounds, and activate them
        return Physics2D.OverlapBoxAll(interactCheck.transform.position, new Vector2(interactCheckWorldWidth, interactCheckWorldHeight), 0, layerMask);
    }

    // use if you want to remove player control of gregor. 
    public void FreezeControlOfGregor()
    {
        _freezeControl = true;
        _startFreezeTime = Time.time;
    }

    // set gregor's horizontal movement to 0
    private void RemoveHorizontalVelocity()
    {
        _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y); 
    }

    // Check if Gregor is on the ground
    private bool IsGrounded()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundedCheck.position, groundedRadius, groundLayers);
     
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                //grounded
                _inJump = false; 
                return true;
            }
        }
        return false; 
    }
}
