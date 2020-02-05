using UnityEngine;
using UnityHelpers;

public class WarningsController : MonoBehaviour
{
    public WarningController warningPrefab;
    public Camera viewingCamera;
    private ObjectPool<WarningController> _warningsPool;
    private ObjectPool<WarningController> WarningsPool { get { if (_warningsPool == null) _warningsPool = new ObjectPool<WarningController>(warningPrefab, 5, false, true, transform, false); return _warningsPool; } }

    public Transform nearestCompass;
    //private List<TrackingData> trackedObjects = new List<TrackingData>();

    void Update()
    {
        AnalyzeTrackedObjects();
    }

    private void AnalyzeTrackedObjects()
    {
        //WarningsPool.ReturnAll();

        float nearestDistance = float.MaxValue;
        //var trackedObjects = FindObjectsOfType<TrackedObject>(); //This costs a lot, very bad in update
        var trackedObjects = TrackedObject.GetTrackedObjects();
        for (int i = trackedObjects.Length - 1; i >= 0; i--)
        {
            Vector3 ray = trackedObjects[i].transform.position - viewingCamera.transform.position;
            float currentDistance = ray.sqrMagnitude;
            if (currentDistance < nearestDistance)
            {
                nearestCompass.forward = ray.normalized;
                nearestDistance = currentDistance;
            }

            //AnalyzeData(trackedObjects[i]);
        }
    }

    private void AnalyzeData(TrackedObject data)
    {
        Vector3 viewPoint = viewingCamera.WorldToViewportPoint(data.transform.position, Camera.MonoOrStereoscopicEye.Mono);
        //Vector3 viewPoint = viewingCamera.WorldToViewportPoint(data.transform.position);
        //Vector3 trackedObjectRay = data.transform.position - transform.position;
        //Vector3 trackedObjectDirection = (data.transform.position - viewingCamera.transform.position).normalized;
        //float trackedObjectDistance = trackedObjectRay.magnitude;
        //float verticalAngle = Vector3.SignedAngle(viewingCamera.transform.forward, trackedObjectDirection, -viewingCamera.transform.right);
        //float horizontalAngle = Vector3.SignedAngle(viewingCamera.transform.forward, trackedObjectDirection, viewingCamera.transform.up);

        //float selfDistance = (transform.position - viewingCamera.transform.position).magnitude;
        //float height = Mathf.Sin(verticalAngle * Mathf.Deg2Rad) * selfDistance;
        //float width = Mathf.Sin(horizontalAngle * Mathf.Deg2Rad) * selfDistance;
        //float height = Mathf.Sin(verticalAngle * Mathf.Deg2Rad) * trackedObjectDistance;
        //float width = Mathf.Sin(horizontalAngle * Mathf.Deg2Rad) * trackedObjectDistance;
        //Vector3 viewPoint = viewingCamera.WorldToViewportPoint(transform.position + viewingCamera.transform.up * height + viewingCamera.transform.right * width);
        //Vector2 viewPoint = new Vector3(horizontalAngle / 90 * 0.5f + 0.5f, verticalAngle / 90 * 0.5f + 0.5f);
        //if (data.TimeLeft() > 0 && !(!data.trackOnScreen && viewPoint.x > 0 && viewPoint.x < 1 && viewPoint.y > 0 && viewPoint.y < 1))
        //{
        if (data.trackOnScreen || viewPoint.x <= 0 || viewPoint.x >= 1 || viewPoint.y <= 0 || viewPoint.y >= 1)
        {
            var warning = WarningsPool.Get();

            BorderController.BorderEdge warningBorder = 0;

            if (viewPoint.x <= 0)
                warningBorder |= BorderController.BorderEdge.left;
            else if (viewPoint.x >= 1)
                warningBorder |= BorderController.BorderEdge.right;
            if (viewPoint.y <= 0)
                warningBorder |= BorderController.BorderEdge.bottom;
            else if (viewPoint.y >= 1)
                warningBorder |= BorderController.BorderEdge.top;

            if (warningBorder == 0)
                warningBorder = BorderController.BorderEdge.bottom | BorderController.BorderEdge.bottomLeft | BorderController.BorderEdge.bottomRight | BorderController.BorderEdge.left | BorderController.BorderEdge.right | BorderController.BorderEdge.top | BorderController.BorderEdge.topLeft | BorderController.BorderEdge.topRight;

            warning.border.SetShownEdges(warningBorder);

            Vector2 halfWarningSize = warning.rectTransform.rect.size / 2f;
            Vector2 halfParentSize = warning.parentRect.rect.size / 2f;
            float warningX = Mathf.Clamp(viewPoint.x * warning.parentRect.rect.size.x - halfParentSize.x, -(halfParentSize.x - halfWarningSize.x), halfParentSize.x - halfWarningSize.x);
            float warningY = Mathf.Clamp(viewPoint.y * warning.parentRect.rect.size.y - halfParentSize.y, -(halfParentSize.y - halfWarningSize.y), halfParentSize.y - halfWarningSize.y);
            warning.rectTransform.localPosition = new Vector3(warningX, warningY);
        }
        //return data.TimeLeft() < 0 || (!data.trackOnScreen && (viewPoint.x > 0 && viewPoint.x < 1 && viewPoint.y > 0 && viewPoint.y < 1));
    }

    //public void TrackObject(Component toTrack, float ttl = 0, bool trackOnscreen = false)
    //{
    //    trackedObjects.Add(new TrackingData(toTrack, WarningsPool.Get(), ttl, trackOnscreen));
    //}
}
