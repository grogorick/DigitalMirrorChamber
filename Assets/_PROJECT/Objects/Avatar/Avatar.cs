using UnityEngine;

public class Avatar : SampleAvatarEntity
{
    public void reload()
    {
        Debug.Log("### MY | Avatar `" + name + "` | Reload");
        StartCoroutine(AutoRetry_LoadUser(false));
    }
}
