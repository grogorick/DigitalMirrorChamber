using UnityEngine;

public class MonitorDynamic : Monitor
{
    public Camera mainCamera;

    private Matrix4x4 mirrorZMat = Matrix4x4.Scale(new Vector3(1, 1, -1));
    protected new void Start()
    {
        base.Start();
        mainCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        Transform playerTr = mainCamera.transform;
        Vector3 mirroredPos = (transform.localToWorldMatrix * mirrorZMat * transform.worldToLocalMatrix).MultiplyPoint(playerTr.position);
        Vector3 mirroredForwardVec = Vector3.Reflect(playerTr.forward, transform.TransformVector(Vector3.forward));
        Quaternion mirroredRot = Quaternion.LookRotation(mirroredForwardVec, Vector3.up);

        monitorCamera.transform.SetPositionAndRotation(mirroredPos, mirroredRot);
    }
}
