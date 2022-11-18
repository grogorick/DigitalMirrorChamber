using UnityEngine;

public class Monitor : MonoBehaviour
{
    public Camera monitorCamera;
    public GameObject monitorDisplayObject;
    public Vector2Int monitorCameraResolution = new (1024, 1024);

    protected RenderTexture renderTexture;
    protected Material mirrorMaterial;

    protected void Start()
    {
        if (monitorDisplayObject == null)
            return;

        renderTexture = new RenderTexture(monitorCameraResolution.x, monitorCameraResolution.y, 32);

        mirrorMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mirrorMaterial.SetTexture("_BaseMap", renderTexture);
        monitorDisplayObject.GetComponent<MeshRenderer>().material = mirrorMaterial;

        monitorCamera.targetTexture = renderTexture;
    }
}
