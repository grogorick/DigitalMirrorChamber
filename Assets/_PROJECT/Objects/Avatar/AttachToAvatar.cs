using System;
using UnityEngine;

public class AttachToAvatar : FindAvatarJoint
{
    [Flags]
    public enum ChangeParentMode
    {
        unchanged                               = 0x0000,

        resetLocalPosition                      = 0x0001,
        resetLocalRotation                      = 0x0010,
        resetLocalPose                          = 0x0011,

        keepLocalPosition                       = 0x0100,
        keepLocalRotation                       = 0x1000,
        keepLocalPose                           = 0x1100,

        keepLocalRotationAndResetLocalPosition  = 0x1001,
        keepLocalPositionAndResetLocalRotation  = 0x0110
    }
    public ChangeParentMode mode;

    protected override void onAvatarJointFound(GameObject avatarJoint, bool replaced)
    {
        attachTo(avatarJoint, mode);
    }

    public void attachTo(GameObject newParent, ChangeParentMode mode = ChangeParentMode.resetLocalPose)
    {
        Vector3 tmpPos = transform.localPosition;
        Quaternion tmpRot = transform.localRotation;

        transform.parent = newParent.transform;

        if (ChangeParentMode.unchanged != (mode & ChangeParentMode.keepLocalPosition))
        {
            transform.localPosition = tmpPos;
        }
        if (ChangeParentMode.unchanged != (mode & ChangeParentMode.keepLocalRotation))
        {
            transform.localRotation = tmpRot;
        }

        if (ChangeParentMode.unchanged != (mode & ChangeParentMode.resetLocalPosition))
        {
            transform.localPosition = Vector3.zero;
        }
        if (ChangeParentMode.unchanged != (mode & ChangeParentMode.resetLocalRotation))
        {
            transform.localRotation = Quaternion.identity;
        }

        Debug.Log("### MY | AttachToAvatar | `" + name + "` | Attached to `" + newParent.name + "` with mode `" + mode + "`");
    }
}