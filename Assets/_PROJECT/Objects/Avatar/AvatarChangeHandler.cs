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

    void Start()
    {
        avatar.onAvatarChangeDetectedEvent.AddListener(onAvatarChangeDetected);
    }

    public void onAvatarChangeDetected(Avatar changedAvatar)
    {
        Debug.Log("### MY | AvatarChangeHandler | Clone and replace changed avatar");

        avatar = Instantiate(avatarCopy, changedAvatar.gameObject.transform.parent);
        avatar.gameObject.name = changedAvatar.gameObject.name;
        changedAvatar.gameObject.SetActive(false);
        changedAvatar.gameObject.name += " (old)";
        avatar.gameObject.SetActive(true);

        onAvatarReplaceEvent.Invoke(changedAvatar, avatar);

        Destroy(changedAvatar.gameObject);

        mirrorAvatar.reload();
    }
}
