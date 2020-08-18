using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

// handles all of Gregor's input and movement. 
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
    
    [Tooltip("Check to toggle a fixed jump.")]
    [SerializeField] private bool useFixedJump;
    

    // components
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private Transform _transform;
    private Animator _animator;
    private WaterActions _waterActions;

    // axis of movement
    private float _movementForce;

    // save a copy of his movement speed in case changes are made in game
    private float _originalMovementSpeed;

    // used for keeping track of gregor's jump
    private bool _jump;
    private bool _inJump;
    private bool _jumpAllowed;

    // pause player control of gregor - used for watering, etc, playing animations
    private bool _freezeControl;
    private float _startFreezeTime;
    private float _freezeDuration; // in seconds


    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _transform = GetComponent<Transform>();
        _animator = GetComponent<Animator>();
        _waterActions = GetComponent<WaterActions>();

        _movementForce = 0f;
        _originalMovementSpeed = movementSpeed;
        _jump = false;
        _inJump = false;
        _jumpAllowed = true;
    }

    // get inputs from player
    void Update()
    {
        // if gregor's controls are frozen, should they be thawed? (lol) 
        if (_freezeControl && (Time.time - _startFreezeTime) > _freezeDuration)
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
            _waterActions.setWater(true); // SEND TO WATER ACTIONS
        }

        // collect water ? 
        if (Input.GetButtonDown("CollectWater"))
        {
            _waterActions.setCollectWater(true); // SEND TO WATER ACTIONS
        }

    }

    // use if you want to remove player control of gregor. 
    public void FreezeControlOfGregor(float freezeDuration)
    {
        // freezes can't stack if you spam buttons !
        if (!_freezeControl)
        {
            _freezeControl = true;
            _startFreezeTime = Time.time;
            _freezeDuration = freezeDuration;
        }
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

    // set gregor's horizontal movement to 0
    public void RemoveHorizontalVelocity()
    {
        _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y); 
    }

    // Check if Gregor is on the ground
    public bool IsGrounded()
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


    // many plants affect Gregor's movement speed.... so here.....
    public void ChangeMovementSpeed(float amount)
    {
        movementSpeed += amount; 
    }

    public void ResetMovementSpeed()
    {
        movementSpeed = _originalMovementSpeed; 
    }
}
