using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class BoostPlatform : MonoBehaviour
{
    private BoostPlant _boostPlant;
    private bool _boost;
    private Rigidbody2D _boosting; 
    // Start is called before the first frame update
    void Start()
    {
        _boostPlant = transform.parent.GetComponent<BoostPlant>();
        _boost = false;
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

        if (rigidBody!= null && !gregorGrounded)
        {
            _boost = true;
            _boosting = rigidBody; 
        }
    }

    // do the actual boost here or things will fuck up
    private void FixedUpdate()
    {
        if (_boost)
        {
            //reset velocity first (no mega jumps lol) 
            _boosting.velocity = new Vector2(_boosting.velocity.x, 0);

            _boosting.AddForce(new Vector2(_boosting.velocity.x, _boostPlant.boostForce));
            //Debug.Log(rigidBody.velocity);
            _boostPlant.Deactivate();
            Debug.Log(_boosting.velocity.y);

            _boost = false;
            _boosting = null; 
        }
    }

}
