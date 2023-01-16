using System.Collections.Generic;
using UnityEngine;

public class FocusAwareness : MonoBehaviour
{
    public List<GameObject> objectsToDisableDuringSystemUniversalMenu = new ();

    void Start()
    {
        Debug.Log("### MY | Focus Awareness | Init");
        OVRManager.InputFocusLost += inputFocusLost;
        OVRManager.InputFocusAcquired += inputFocusAcquired;
    }

    private void inputFocusLost()
    {
        Debug.Log("### MY | Focus Awareness | Focus lost | Deactivate:\n-" + string.Join("\n- ", objectsToDisableDuringSystemUniversalMenu.ConvertAll((obj) => obj.name)));
        foreach (var obj in objectsToDisableDuringSystemUniversalMenu)
            obj.SetActive(false);
    }

    private void inputFocusAcquired()
    {
        Debug.Log("### MY | Focus Awareness | Focus acquired | (Re-)activate:\n-" + string.Join("\n- ", objectsToDisableDuringSystemUniversalMenu.ConvertAll((obj) => obj.name)));
        foreach (var obj in objectsToDisableDuringSystemUniversalMenu)
            obj.SetActive(true);
    }
}
