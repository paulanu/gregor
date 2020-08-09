
// defines all plants
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
