using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// handles Gregor's watering / collecting water actions. 
// It was originally in GregorController but split here for organization.
public class WaterActions : MonoBehaviour
{
    private bool water;
    private bool collectWater;

    // appropriate layers to deal with
    [Tooltip("Which layers are plants?")]
    [SerializeField] private LayerMask plantLayers;
    [Tooltip("Which layers have water?")]
    [SerializeField] private LayerMask waterSourceLayers;
    [Tooltip("Which layers are ground?")]
    [SerializeField] private LayerMask groundLayers;

    // various checks
    [Tooltip("What is used to check for plants, water sources, etc. in front of Gregor.")]
    [SerializeField] private Transform interactCheck;
    [Tooltip("Place in front of Gregor - used to check if he is watering off of a ledge.")]
    [SerializeField] private Transform ledgeCheck;
    [Tooltip("Visual component so you can see :)")]
    [SerializeField] private bool hideCheckVisuals;

    // watering time
    [Tooltip("How long is Gregor paused while he is doing stuff with water? In seconds.")]
    [SerializeField] private float actionDuration = 1f;

    // keep track of watering
    [SerializeField] private int waterQuantity;
    [SerializeField] private int maxWater = 1;

    // getting plant check from child obj 
    private SpriteRenderer _interactCheckVisual;
    private SpriteRenderer _ledgeCheckVisual;

    // components
    private ParticleSystem _waterParticles;
    private Animator _animator;
    private GregorController _gregorController; 

    // Start is called before the first frame update
    void Start()
    {
        _interactCheckVisual = interactCheck.GetComponent<SpriteRenderer>();
        _ledgeCheckVisual = ledgeCheck.GetComponent<SpriteRenderer>();

        waterQuantity = 0; 
        water = false;
        collectWater = false;

        _waterParticles = GetComponentInChildren<ParticleSystem>();
        _animator = GetComponent<Animator>();
        _gregorController = GetComponent<GregorController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (water)
        {
            Water();
        }
        if (collectWater)
        {
            CollectWater();
        }

        // hide check visuals if desired
        if (hideCheckVisuals)
        {
            _interactCheckVisual.color = Color.clear;
            _ledgeCheckVisual.color = Color.clear;
        }
        else
        {
            _interactCheckVisual.color = Color.green;
            _ledgeCheckVisual.color = Color.red;
        }
    }

    // setters
    public void setWater(bool water)
    {
        this.water = water; 
    }

    public void setCollectWater(bool collectWater)
    {
        this.collectWater = collectWater; 
    }

    // check for water sources, get water from them. 
    private void CollectWater()
    {
        // The player can't move Gregor until he is done collecting water
        _gregorController.FreezeControlOfGregor(actionDuration);
        _gregorController.RemoveHorizontalVelocity();

        collectWater = false;

        // check for water in front of gregor
        Collider2D[] hitColliders = InteractWithObjectsInLayer(waterSourceLayers);

        int collectedWater = 0;
        foreach (Collider2D c in hitColliders) // GIT that water 
        {
            collectedWater += c.gameObject.GetComponent<WaterSource>().GetWater();
        }

        // if gregor collected water, update water quantity
        if (collectedWater > 0)
        {
            // if gregor doesn't already have water, play the water anim
            if (waterQuantity == 0)
                _animator.SetBool("CollectedWater", true);

            waterQuantity = Math.Min(collectedWater + waterQuantity, maxWater);

        }
    }

    // turn on water particles, check for plants being watered
    private void Water()
    {
        // The player can't move Gregor until he is done watering
        _gregorController.FreezeControlOfGregor(actionDuration);
        _gregorController.RemoveHorizontalVelocity();

        water = false;

        // only water plants if you have enough water!
        if (waterQuantity > 0)
        {
            waterQuantity--;

            // play watering anim
            _waterParticles.Play();
            _animator.SetBool("CollectedWater", false);

            // check for plants being watered
            Collider2D[] plants = InteractWithObjectsInLayer(plantLayers);

            // account for gregor standing on a ledge and watering plants below
            RaycastHit2D isThereGroundBelow = Physics2D.Raycast(ledgeCheck.position, Vector2.down, 1000, groundLayers);

            if (isThereGroundBelow)
            {
                // I use the waterLedgeCheck scale as my box size so I have a visual aid 
                // sprite is 100x100, 1 unit per 100 pixels, so ledge check scale correlates 1:1 with world scale, BUT multiplied by Gregor's parent scale to affect for that. I think??? 
                float ledgeCheckWorldWidth = Mathf.Abs(ledgeCheck.transform.localScale.x * transform.localScale.x);
                float ledgeCheckWorldHeight = Mathf.Abs(ledgeCheck.transform.localScale.y * transform.localScale.y);

                // now get all colliders in the ledgeCheck bounds
                Collider2D[] ledgePlants = Physics2D.OverlapBoxAll(isThereGroundBelow.point, new Vector2(ledgeCheckWorldWidth, ledgeCheckWorldHeight), 0, plantLayers);

                plants = plants.Concat<Collider2D>(ledgePlants).ToArray();
            }

            foreach (Collider2D plant in plants) //activate those plants
                plant.gameObject.GetComponent<Plant>().Activate();

        }
        else
            Debug.Log("aint got no water!!!!!");

    }
    private Collider2D[] InteractWithObjectsInLayer(LayerMask layerMask)
    {
        // box size based on interactCheck scale so I have a visual aid
        // interactCheck sprite is 100x100, 1 unit per 100 pixels, so interact check scale correlates 1:1 with world scale, BUT multiplied by Gregor's parent scale to affect for that. I think??? 
        float interactCheckWorldWidth = Mathf.Abs(interactCheck.transform.localScale.x * transform.localScale.x);
        float interactCheckWorldHeight = Mathf.Abs(interactCheck.transform.localScale.y * transform.localScale.y);

        // now get all colliders in the interactCheck bounds
        return Physics2D.OverlapBoxAll(interactCheck.transform.position, new Vector2(interactCheckWorldWidth, interactCheckWorldHeight), 0, layerMask);
    }
}
