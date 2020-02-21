using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public bool isInQuiver { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Quiver"))
            isInQuiver = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Quiver"))
            isInQuiver = false;
    }
}
