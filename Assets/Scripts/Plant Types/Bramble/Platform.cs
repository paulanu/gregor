using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    Bramble _parent; 

    // Start is called before the first frame update
    void Start()
    {
        _parent = GetComponentInParent<Bramble>(); 
    }

    //https://answers.unity.com/questions/783377/detect-side-of-collision-in-box-collider-2d.html
    //if something stands on and exits the top of the platform, it is all used up :'( 
    private void OnTriggerExit2D(Collider2D collision)
    {
        _parent.Deactivate();
    }
}
