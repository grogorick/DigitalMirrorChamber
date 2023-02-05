using Oculus.Avatar2;
using UnityEngine;
using UnityEngine.Events;

public class AvatarChangeHandler : MonoBehaviour
{
    public Avatar avatar, mirrorAvatar;
    private Avatar avatarCopy, oldAvatar, newAvatar;
    private GameObject newAvatarCamera;

    public UnityEvent<Avatar, Avatar> onAvatarReplaceEvent;

    private void Awake()
    {
        if (avatar.gameObject.activeSelf)
        {
            Debug.LogError("### MY | AvatarChangeHandler | Avatar already `active`. Must be deactivated on startup");
            avatar = null;
            return;
        }
        Debug.Log("### MY | AvatarChangeHandler | Copy avatar");
        avatarCopy = Instantiate(avatar, avatar.gameObject.transform.parent);
        avatar.gameObject.SetActive(true);
    }

    public void reloadAvatar()
    {
        if (avatar != null)
        {
            Debug.Log("### MY | AvatarChangeHandler | Clone avatar to load with changed appearance");

            oldAvatar = avatar;
            newAvatar = Instantiate(avatarCopy, avatarCopy.gameObject.transform.parent);
            newAvatar.gameObject.transform.localScale = Vector3.one * .01f;
            newAvatar.OnUserAvatarLoadedEvent.AddListener(replaceOldAvatarWithComletelyLoadedNewOne);
            newAvatar.gameObject.SetActive(true);
            newAvatarCamera = newAvatar.GetComponentInChildren<Camera>().gameObject;
            newAvatarCamera.SetActive(false);

            mirrorAvatar.reload();
        }
    }

    private void replaceOldAvatarWithComletelyLoadedNewOne(OvrAvatarEntity _)
    {
        Debug.Log("### MY | AvatarChangeHandler | Replace old avatar with new one");

        newAvatarCamera.SetActive(true);
        newAvatar.gameObject.transform.localScale = Vector3.one;
        newAvatar.gameObject.name = oldAvatar.gameObject.name;
        oldAvatar.gameObject.name += " (old)";
        oldAvatar.gameObject.SetActive(false);

        onAvatarReplaceEvent.Invoke(oldAvatar, newAvatar);

        Destroy(oldAvatar.gameObject);
        oldAvatar = null;
        avatar = newAvatar;
        newAvatar = null;
    }
}
