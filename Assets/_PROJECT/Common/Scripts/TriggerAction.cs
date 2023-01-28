using UnityEngine;
using UnityEngine.Events;

public class TriggerAction : MonoBehaviour
{
    [Header("Trigger Action")]
    public UnityEvent action;

    public enum TriggerEvent { enter, stay, leave }
    public TriggerEvent triggerEvent = TriggerEvent.enter;

    public string colliderTag = string.Empty;

    public Debounce debounce = new ();

    private void OnTriggerEnter(Collider other)
    {
        if (triggerEvent == TriggerEvent.enter && debounce.check())
        {
            Debug.Log("### MY | TriggerAction | `" + name + "` | Enter");

            if (colliderTag == string.Empty || other.CompareTag(colliderTag))
                doAction();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (triggerEvent == TriggerEvent.stay && debounce.check())
        {
            //Debug.Log("### MY | TriggerAction | `" + name + "` | Stay");

            if (colliderTag == string.Empty || other.CompareTag(colliderTag))
                doAction();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggerEvent == TriggerEvent.leave && debounce.check())
        {
            Debug.Log("### MY | TriggerAction | `" + name + "` | Leave");

            if (colliderTag == string.Empty || other.CompareTag(colliderTag))
                doAction();
        }
    }

    protected virtual void doAction()
    {
        if (action.GetPersistentEventCount() > 0)
            action.Invoke();
    }
}