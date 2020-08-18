
// defines all plants
using UnityEngine;
using CatchCo;

public abstract class Plant : MonoBehaviour
{
    [Header("For level design use only. Set BEFORE hitting play.")]
    [SerializeField] protected bool activated; // THIS IS PURELY FOR USE OF LEVEL DESIGNER

    protected Animator _animator;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _animator = this.GetComponent<Animator>();

        if (activated)
        {
            Activate();
        }
    }

    [ExposeMethodInEditor]
    public virtual void Activate()
    {

        _animator.SetBool("Activated", true);
        activated = true;
    }

    [ExposeMethodInEditor]
    public virtual void Deactivate()
    {
        _animator.SetBool("Activated", false);
        activated = false;
    }

    public virtual void OnUse()
    {
        Deactivate();
    }
}
