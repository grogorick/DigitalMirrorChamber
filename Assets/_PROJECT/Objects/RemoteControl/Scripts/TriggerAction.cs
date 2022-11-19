using UnityEngine;
using UnityEngine.Events;

public class TriggerAction : MonoBehaviour
{
    [Header("Trigger Action")]
    public UnityEvent action;

    public enum TriggerEvent { enter, leave }
    public TriggerEvent triggerEvent = TriggerEvent.enter;

    public string colliderTag = string.Empty;

    private void OnTriggerEnter(Collider other)
    {
        if (triggerEvent == TriggerEvent.enter)
        {
            Debug.Log("### MY | TriggerAction | Enter `" + name + "`");

            if (colliderTag == string.Empty || other.CompareTag(colliderTag))
                doAction();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggerEvent == TriggerEvent.leave)
        {
            Debug.Log("### MY | TriggerAction | Leave `" + name + "`");

            if (colliderTag == string.Empty || other.CompareTag(colliderTag))
                doAction();
        }
    }

    protected virtual void doAction()
    {
        if (action != null)
            action.Invoke();
    }
}