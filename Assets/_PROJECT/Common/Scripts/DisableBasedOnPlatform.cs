using UnityEngine;

public class DisableBasedOnPlatform : MonoBehaviour
{
    public enum Platform
    {
        Mobile, Desktop
    }

    public Platform onPlatform = Platform.Mobile;

    void Awake()
    {
        if (
            (onPlatform == Platform.Mobile && Application.isMobilePlatform) ||
            (onPlatform == Platform.Desktop && !Application.isMobilePlatform))
        {
            Debug.Log("### MY | DisableBasedOnPlatform | `" + name + "`");
            gameObject.SetActive(false);
        }
    }
}
