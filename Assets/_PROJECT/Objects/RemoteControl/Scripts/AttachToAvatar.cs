using Oculus.Avatar2;
using System;
using UnityEngine;

public class AttachToAvatar : MonoBehaviour
{
    public string attachJointName;

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

    [Tooltip("If empty, search an object named `Avatar`")]
    public GameObject avatarObject;

    private GameObject attachJoint;


    private void Start()
    {
        if (avatarObject == null)
        {
            if ((avatarObject = GameObject.Find("Avatar")) == null)
            {
                Debug.LogError("### MY | AttachToAvatar | Avatar NOT found");
                return;
            }
        }

        SampleAvatarEntity sae = avatarObject.GetComponent<SampleAvatarEntity>();
        if (sae != null)
        {
            sae.OnDefaultAvatarLoadedEvent.AddListener(OnAvatarLoaded);
            sae.OnFastLoadAvatarLoadedEvent.AddListener(OnAvatarLoaded);
            sae.OnUserAvatarLoadedEvent.AddListener(OnAvatarLoaded);
        }
    }

    public void OnAvatarLoaded(OvrAvatarEntity ae)
    {
        if (attachJoint == null && (attachJoint = findAvatarJoint(attachJointName)))
            attachTo(attachJoint, mode);
    }

    public GameObject findAvatarJoint(string jointName)
    {
        GameObject joint = avatarObject.transform.FindChildRecursive(jointName)?.gameObject;
        if (joint != null)
            Debug.Log("### MY | AttachToAvatar | Joint `" + joint.name + "` found");
        else
            Debug.LogError("### MY | AttachToAvatar | Joint `" + jointName + "` NOT found");
        return joint;
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

        Debug.Log("### MY | AttachToAvatar | `" + name + "` attached to `" + newParent.name + "` mode: " + mode);
    }
}