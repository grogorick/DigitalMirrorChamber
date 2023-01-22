using Oculus.Avatar2;
using UnityEngine;
using UnityEngine.Events;

public class FindAvatarJoint : MonoBehaviour
{
    public string jointName;

    [Tooltip("If empty, search object with name `Avatar`")]
    public Avatar avatar;
    [Tooltip("If empty, search object with name `AvatarChangeHandler`")]
    public AvatarChangeHandler avatarChangeHandler;
    public UnityEvent<GameObject, bool> onAvatarJointFoundEvent;

    protected GameObject avatarJoint;
    private bool replaced = false;

    private void Start()
    {
        if (avatar == null)
        {
            GameObject obj = GameObject.Find("Avatar");
            if (obj == null)
            {
                Debug.LogError("### MY | FindAvatarJoint | `" + name + "` | Avatar NOT found");
                return;
            }
            avatar = obj.GetComponent<Avatar>();
        }

        if (avatarChangeHandler == null)
        {
            GameObject obj = GameObject.Find("AvatarChangeHandler");
            if (obj == null)
            {
                Debug.LogError("### MY | FindAvatarJoint | `" + name + "` | AvatarChangeHandler NOT found");
                return;
            }
            avatarChangeHandler = obj.GetComponent<AvatarChangeHandler>();
        }

        avatar.OnDefaultAvatarLoadedEvent.AddListener(onAvatarLoaded);
        avatar.OnFastLoadAvatarLoadedEvent.AddListener(onAvatarLoaded);
        avatar.OnUserAvatarLoadedEvent.AddListener(onAvatarLoaded);

        avatarChangeHandler.onAvatarReplaceEvent.AddListener(onAvatarReplace);
    }

    public void onAvatarReplace(Avatar oldAvatar, Avatar newAvatar)
    {
        Debug.Log("### MY | FindAvatarJoint | `" + name + "` | Avatar replaced");
        avatarJoint = null;
        replaced = true;
        onAvatarLoaded(avatar = newAvatar);
    }

    public void onAvatarLoaded(OvrAvatarEntity ae)
    {
        string msg = "### MY | FindAvatarJoint | `" + name + "` | Avatar loaded (" + ae.CurrentState + ") | ";
        if (avatarJoint == null)
        {
            Debug.Log(msg + "Try to find avatar joint `" + jointName + "`");
            if ((avatarJoint = findAvatarJoint(avatar, jointName)) != null)
            {
                Debug.Log(msg + "Joint `" + avatarJoint.name + "` found");
                onAvatarJointFound(avatarJoint, replaced);
            }
            else
                Debug.LogError(msg + "Joint `" + jointName + "` NOT found");
        }
        Debug.Log(msg + "Avatar joint already found in previous load event. Skip this");
    }

    public static GameObject findAvatarJoint(Avatar avatar, string jointName)
    {
        return avatar.gameObject.transform.FindChildRecursive(jointName)?.gameObject;
    }

    protected virtual void onAvatarJointFound(GameObject avatarJoint, bool replaced) {
        onAvatarJointFoundEvent.Invoke(avatarJoint, replaced);
    }
}
