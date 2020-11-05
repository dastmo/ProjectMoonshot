using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fissure : MonoBehaviour
{
    private Debris parentDebris
    {
        get { return GetComponentInParent<Debris>(); }
    }
}
