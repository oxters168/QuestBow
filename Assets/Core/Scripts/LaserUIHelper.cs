using UnityEngine;

public class LaserUIHelper : MonoBehaviour
{
    public LaserPointer laserPointer;
    public LaserPointer.LaserBeamBehavior laserBeamBehavior;

    void Start()
    {
        Debug.Log("Starting LaserUIHelper");
        laserPointer.laserBeamBehavior = laserBeamBehavior;
        OVRRaycaster[] ovrRaycasters = FindObjectsOfType<OVRRaycaster>();
        foreach (var ovrRaycaster in ovrRaycasters)
            ovrRaycaster.pointer = laserPointer.gameObject;
    }
}
