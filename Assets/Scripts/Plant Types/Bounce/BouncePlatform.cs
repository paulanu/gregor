using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePlatform : MonoBehaviour
{
    // this is to prevent boost stacking
    static bool _delayBoosting = false;
    private float _boostDelayDuration = .2f; // delay until next boost can be registered (in seconds) 
    private float _boostStartTime = -1;

    private BouncePlant _boostPlant;
    private bool _boost;
    private Rigidbody2D _boosting;
    // Start is called before the first frame update
    void Start()
    {
        _boostPlant = transform.parent.GetComponent<BouncePlant>();
        _boost = false;
    }

    private void Update()
    {
        if (_boostStartTime != -1 && (Time.time - _boostStartTime) > _boostDelayDuration)
        {
            _delayBoosting = false;
            _boostStartTime = -1;
        }
    }

    //if collider is entered, BOOST the gameobject up!
    void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rigidBody = other.GetComponent<Rigidbody2D>();

        GregorController gregor = other.GetComponent<GregorController>();

        // if gregor is getting boosted, he only is boosted if he is in the air (NOT if he just runs into it)
        bool gregorGrounded = false;
        if (gregor != null)
            gregorGrounded = gregor.IsGrounded();

        if (rigidBody != null && !gregorGrounded && !_delayBoosting)
        {
            _boost = true;
            _boosting = rigidBody;

            // short delay so that multiple boost plants can't launch gregor into space
            _delayBoosting = true;
        }
    }

    // do the actual boost here or things will fuck up
    private void FixedUpdate()
    {
        if (_boost)
        {
            // set start time here since FixedUpdate() can run seperately from Update() 
            _boostStartTime = Time.time;

            //SHOOT THAT SHIT UP 
            _boosting.AddForce(new Vector2(_boosting.velocity.x, _boostPlant.boostForce));
            // if it's SLAMMING down then BOOST that shit UP like a TRAMPOLINE! 
            float boost = Math.Abs(_boosting.velocity.y / Time.fixedDeltaTime) / 2; //V/time = force???
            _boosting.AddForce(new Vector2(_boosting.velocity.x, Math.Abs(boost)));

            //reset velocity (no mega jumps lol) 
            _boosting.velocity = new Vector2(_boosting.velocity.x, 0);

            _boostPlant.Deactivate();

            _boost = false;
            _boosting = null;
        }
    }
}
