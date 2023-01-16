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

    [Tooltip("If empty, search object with name `Avatar`")]
    public Avatar avatar;
    public AvatarChangeHandler avatarChangeHandler;

    private GameObject attachJoint;


    private void Start()
    {
        if (avatar == null)
        {
            GameObject obj = GameObject.Find("Avatar");
            if (obj == null)
            {
                Debug.LogError("### MY | AttachToAvatar | `" + name + "` | Avatar NOT found");
                return;
            }
            avatar = obj.GetComponent<Avatar>();
        }

        if (avatarChangeHandler == null)
        {
            GameObject obj = GameObject.Find("AvatarChangeHandler");
            if (obj == null)
            {
                Debug.LogError("### MY | AttachToAvatar | `" + name + "` | AvatarChangeHandler NOT found");
                return;
            }
            avatarChangeHandler = obj.GetComponent<AvatarChangeHandler>();
        }

        avatarChangeHandler.onAvatarReplaceEvent.AddListener(onAvatarReplace);
        prepareAvatarLoading();
    }

    public void onAvatarReplace(Avatar oldAvatar, Avatar newAvatar)
    {
        Debug.Log("### MY | AttachToAvatar | `" + name + "` | Prepare for replaced avatar");
        transform.parent = (avatar = newAvatar).gameObject.transform;
        prepareAvatarLoading();
    }

    private void prepareAvatarLoading()
    {
        avatar.OnDefaultAvatarLoadedEvent.AddListener(onAvatarLoaded);
        avatar.OnFastLoadAvatarLoadedEvent.AddListener(onAvatarLoaded);
        avatar.OnUserAvatarLoadedEvent.AddListener(onAvatarLoaded);
    }

    public void onAvatarLoaded(OvrAvatarEntity ae)
    {
        string msg = "### MY | AttachToAvatar | `" + name + "` | Avatar loaded (" + ae.CurrentState + ") | ";
        if (attachJoint == null && (attachJoint = findAvatarJoint(attachJointName)))
        {
            Debug.Log(msg + "Try to find avatar joint `" + attachJointName + "`");
            attachTo(attachJoint, mode);
        }
        Debug.Log(msg + "Already attached in previous load event. Skip this");
    }

    public GameObject findAvatarJoint(string jointName)
    {
        GameObject joint = avatar.gameObject.transform.FindChildRecursive(jointName)?.gameObject;
        if (joint != null)
            Debug.Log("### MY | AttachToAvatar | `" + name + "` | Joint `" + joint.name + "` found");
        else
            Debug.LogError("### MY | AttachToAvatar | `" + name + "` | Joint `" + jointName + "` NOT found");
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

        Debug.Log("### MY | AttachToAvatar | `" + name + "` | Attached to `" + newParent.name + "` with mode `" + mode + "`");
    }
}