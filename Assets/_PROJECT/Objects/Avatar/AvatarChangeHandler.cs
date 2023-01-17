using Oculus.Avatar2;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AvatarChangeHandler : MonoBehaviour
{
    public Avatar avatar, mirrorAvatar;
    private Avatar avatarCopy;

    public UnityEvent<Avatar, Avatar> onAvatarReplaceEvent;

    private void Awake()
    {
        if (avatar.gameObject.activeSelf)
        {
            Debug.LogError("### MY | AvatarChangeHandler | Avatar already `active`. Must be deactivated on startup");
            return;
        }
        Debug.Log("### MY | AvatarChangeHandler | Copy avatar");
        avatarCopy = Instantiate(avatar, avatar.gameObject.transform.parent);
        avatar.gameObject.SetActive(true);
    }

    public void reloadAvatar()
    {
        Debug.Log("### MY | AvatarChangeHandler | Clone and replace changed avatar");

        Avatar oldAvatar = avatar;
        avatar = Instantiate(avatarCopy, oldAvatar.gameObject.transform.parent);
        avatar.gameObject.name = oldAvatar.gameObject.name;
        oldAvatar.gameObject.SetActive(false);
        oldAvatar.gameObject.name += " (old)";
        avatar.gameObject.SetActive(true);

        onAvatarReplaceEvent.Invoke(oldAvatar, avatar);

        Destroy(oldAvatar.gameObject);

        mirrorAvatar.reload();
    }
}
