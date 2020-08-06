using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public interface Plant
{
    /// <summary>
    /// What happens when Gregor uses the plant? Define "use" here. 
    /// </summary>
    void OnUse();

    /// <summary>
    /// Activate the plant. 
    /// </summary>
    void Activate();

    /// <summary>
    /// Deactivate the plant. 
    /// </summary>
    void Deactivate(); 
}
