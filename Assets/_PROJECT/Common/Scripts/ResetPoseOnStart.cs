using System.Collections;
using UnityEngine;

public class ResetPoseOnStart : MonoBehaviour
{
    public float waitSeconds = 1;

    void Start()
    {
        StartCoroutine(_recenterPose());
    }
    IEnumerator _recenterPose()
    {
        yield return new WaitForSeconds(waitSeconds);
        recenterPose();
    }

    public void recenterPose()
    {
        Debug.Log("### MY | ResetPose");

        Transform head = Camera.main.transform;
        transform.Rotate(0, -head.eulerAngles.y, 0);
        transform.localPosition += Vector3.Scale(head.localPosition, new Vector3(-1, 0, -1));
    }
}
