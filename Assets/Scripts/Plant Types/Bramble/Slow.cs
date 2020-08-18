using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : MonoBehaviour
{
    [Tooltip("This is subtracted from Gregor's base movement.")]
    [SerializeField] private float slowAmount = -3; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GregorController gregor = collision.GetComponent<GregorController>();

        if (gregor != null)
            gregor.ChangeMovementSpeed(slowAmount);

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GregorController gregor = collision.GetComponent<GregorController>();

        if (gregor != null)
            gregor.ResetMovementSpeed();
    }
}
