using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOrbitObject : MonoBehaviour
{
    private void Update()
    {
        AlignWithEarth();
    }

    private void AlignWithEarth()
    {
        Vector2 diff = Vector2.zero - (Vector2)transform.position;
        diff.Normalize();

        float rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + 90f);
    }
}
