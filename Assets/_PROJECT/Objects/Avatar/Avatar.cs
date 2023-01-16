using Oculus.Avatar2;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Avatar : SampleAvatarEntity
{
    [Header("More Events")]
    public UnityEvent<Avatar> onAvatarChangeDetectedEvent;

    public void checkForChanges()
    {
        StartCoroutine(_checkForChanges());
    }

    private IEnumerator _checkForChanges()
    {
        Debug.Log("### MY | Avatar `" + name + "` | Check for changes");
        var checkTask = HasAvatarChangedAsync();
        while (!checkTask.IsCompleted) yield return null;

        if (checkTask.Result == OvrAvatarManager.HasAvatarChangedRequestResultCode.AvatarHasChanged)
        {
            Debug.Log("### MY | Avatar `" + name + "` | Change detected");
            onAvatarChangeDetectedEvent.Invoke(this);
        }
    }

    public void reload()
    {
        Debug.Log("### MY | Avatar `" + name + "` | Reload");
        StartCoroutine(AutoRetry_LoadUser(false));
    }
}
