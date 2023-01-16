using UnityEngine;
using UnityEngine.Events;

public class TriggerActions : MonoBehaviour
{
    [Header("Trigger Actions")]
    public UnityEvent enterAction;
    public UnityEvent stayAction;
    public UnityEvent leaveAction;

    public string colliderTag = string.Empty;

    public Debounce debounce = new ();

    private void OnTriggerEnter(Collider other)
    {
        if (enterAction.GetPersistentEventCount() > 0 && debounce.check())
        {
            Debug.Log("### MY | TriggerActions | `" + name + "` | Enter");

            if (colliderTag == string.Empty || other.CompareTag(colliderTag))
                enterAction.Invoke();

            if (leaveAction != null)
                debounce.reset();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (stayAction.GetPersistentEventCount() > 0 && debounce.check())
        {
            Debug.Log("### MY | TriggerActions | `" + name + "` | Stay");

            if (colliderTag == string.Empty || other.CompareTag(colliderTag))
                stayAction.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (leaveAction.GetPersistentEventCount() > 0)
        {
            Debug.Log("### MY | TriggerActions | `" + name + "` | Leave");

            if (colliderTag == string.Empty || other.CompareTag(colliderTag))
                leaveAction.Invoke();
        }
    }
}