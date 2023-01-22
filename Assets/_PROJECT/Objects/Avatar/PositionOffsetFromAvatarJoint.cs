using UnityEngine;

public class PositionOffsetFromAvatarJoint : FindAvatarJoint
{
    public Vector3 axesFactors;
    private Vector3 initObjPos;

    private void Awake()
    {
        initObjPos = transform.localPosition;
    }

    void Update()
    {
        if (avatarJoint != null)
            transform.localPosition = initObjPos + Vector3.Scale(avatarJoint.transform.localPosition, axesFactors);
    }
}
