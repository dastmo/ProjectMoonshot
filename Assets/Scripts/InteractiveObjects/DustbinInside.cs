using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustbinInside : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debris debris = collision.GetComponent<Debris>();

        if (debris)
        {
            Dustbin.DebrisCollected?.Invoke(debris.Size);
            Destroy(collision.gameObject);
            return;
        }
    }
}
