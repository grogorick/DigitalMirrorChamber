using UnityEngine;

public class DynamicWebcam : Monitor
{
    public GameObject monitorCameraHandle;
    public GameObject monitorCameraAnchor;
    public GameObject monitorCameraSlideObject;

    private float mirrorWidth;

    private new void Start()
    {
        base.Start();

        Vector3 size = monitorCameraSlideObject.GetComponent<Collider>().bounds.size;
        Vector3 rightAxis = monitorCameraAnchor.transform.right;
        size.Scale(rightAxis);
        size = monitorCameraAnchor.transform.worldToLocalMatrix.MultiplyVector(size);

        mirrorWidth = size.magnitude / 2;
    }

    private void FixedUpdate()
    {
        Vector3 camPos = monitorCameraHandle.transform.localPosition;
        Vector3 localMainCamPos = monitorCameraAnchor.transform.worldToLocalMatrix.MultiplyPoint(Camera.main.transform.position);

        camPos.x = Mathf.Clamp(localMainCamPos.x, -mirrorWidth, mirrorWidth);

        monitorCameraHandle.transform.localPosition = camPos;
    }
}
