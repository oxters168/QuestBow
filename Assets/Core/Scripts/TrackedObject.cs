using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Attach this to an object that should show up on the HUD
/// </summary>
public class TrackedObject : MonoBehaviour
{
    private static List<TrackedObject> allObjects = new List<TrackedObject>();

    public bool trackOnScreen;
    public float ttl;
    private float birthTime;

    private void OnEnable()
    {
        allObjects.Add(this);
    }
    private void OnDisable()
    {
        allObjects.Remove(this);
    }

    public float TimeLeft()
    {
        return ttl > 0 ? ttl - (Time.time - birthTime) : 0;
    }

    public static TrackedObject[] GetTrackedObjects()
    {
        return allObjects.ToArray();
    }
}
