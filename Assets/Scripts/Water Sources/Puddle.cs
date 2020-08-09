using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//puddles give u a water
public class Puddle : MonoBehaviour, WaterSource
{
    public int GetWater()
    {
        return 1; 
    }
}
