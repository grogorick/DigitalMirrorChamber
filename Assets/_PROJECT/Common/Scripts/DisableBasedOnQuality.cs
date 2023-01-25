using UnityEngine;

public class DisableBasedOnQuality : MonoBehaviour
{
    public enum Quality
    {
        Low, High
    }

    public Quality onPlatform = Quality.Low;

    void Awake()
    {
        int level = QualitySettings.GetQualityLevel();
        if (
            (onPlatform == Quality.Low && level == 0) ||
            (onPlatform == Quality.High && level == 1))
        {
            Debug.Log("### MY | DisableBasedOnQuality | `" + name + "` | disable for quality level " + level + " `" + QualitySettings.names[level] + "`");
            gameObject.SetActive(false);
        }
    }
}
