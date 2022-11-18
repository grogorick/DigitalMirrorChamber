using UnityEngine;

public class TabletButtonTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Button"))
        {
            TabletButton btn = other.gameObject.GetComponent<TabletButton>();
            if (btn != null)
            {
                Debug.Log("### MY | TabletButtonTrigger | Button `" + other.gameObject.name + "` triggered");
                btn.press();
            }
            else
                Debug.LogWarning("### MY | TabletButtonTrigger | Button `" + other.gameObject.name + "` triggered, but `TabletButtonScript` is missing");
        }
    }
}