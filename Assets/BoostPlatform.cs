using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class BoostPlatform : MonoBehaviour
{
    private BoostPlant _boostPlant;

    // Start is called before the first frame update
    void Start()
    {
        _boostPlant = transform.parent.GetComponent<BoostPlant>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //if collider is entered, BOOST the gameobject up!
    void OnTriggerEnter2D(Collider2D other)
    {

        Rigidbody2D rigidBody = other.GetComponent<Rigidbody2D>();

        GregorController gregor = other.GetComponent<GregorController>();

        // if gregor is getting boosted, he only is boosted if he jumps (intentional vs just running into it)
        bool gregorJumped = true; 
        if (gregor != null)
            gregorJumped = gregor.hasGregorJumped();

        if (rigidBody!= null && gregorJumped)
        {
            //reset velocity first (no mega jumps lol) 
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
            
            rigidBody.AddForce(new Vector2(rigidBody.velocity.x, _boostPlant.boostForce));
            //Debug.Log(rigidBody.velocity);
            _boostPlant.Deactivate();

        }
    }

}
