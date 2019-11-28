using UnityEngine;

public class LaserUIHelper : MonoBehaviour
{
    public LaserPointer laserPointer;
    public LaserPointer.LaserBeamBehavior laserBeamBehavior;
    public GameObject canvas;

    void Start()
    {
        laserPointer.laserBeamBehavior = laserBeamBehavior;
        OVRRaycaster[] ovrRaycasters = canvas.GetComponentsInChildren<OVRRaycaster>(true);
        foreach (var ovrRaycaster in ovrRaycasters)
            ovrRaycaster.pointer = laserPointer.gameObject;
    }
}
