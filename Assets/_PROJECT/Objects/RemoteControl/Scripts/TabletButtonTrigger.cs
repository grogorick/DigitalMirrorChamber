using UnityEngine;

public class TabletButtonTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        TabletButton btn = other.gameObject.GetComponent<TabletButton>();
        if (btn != null)
        {
            Debug.Log("### MY | TabletButtonTrigger | Button `" + other.gameObject.name + "` triggered");
            btn.press();
        }

        else
        {
            RemoteControl rc = other.gameObject.GetComponent<RemoteControl>();
            if (rc != null)
            {
                Debug.Log("### MY | TabletButtonTrigger | Hand approach tablet");
                rc.action_approachTablet();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        RemoteControl rc = other.gameObject.GetComponent<RemoteControl>();
        if (rc != null)
        {
            Debug.Log("### MY | TabletButtonTrigger | Hand leave tablet");
            rc.action_leaveTablet();
        }
    }
}