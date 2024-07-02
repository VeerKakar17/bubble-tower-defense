using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour
{
    // Initializes this state
    public virtual void InitializeState() { }

    // Logic for when in this state
    public virtual void UpdateState() { }
}
