using System.Collections;
using UnityEngine;

public class Vibrate : MonoBehaviour
{
    static public void now(bool leftHand, bool rightHand, float amplitude = 1f, float seconds = .1f)
    {
        getOrFindInstanceInScene().vibrate(leftHand, rightHand, amplitude, seconds);
    }


    static private Vibrate instance;

    private static Vibrate getOrFindInstanceInScene()
    {
        if (instance == null)
            return (instance = FindObjectOfType<Vibrate>());
        return instance;
    }

    private void vibrate(bool leftHand, bool rightHand, float amplitude = 1f, float seconds = .1f)
    {
        StartCoroutine(_vibrate(leftHand, rightHand, amplitude, seconds));
    }

    private IEnumerator _vibrate(bool leftHand, bool rightHand, float amplitude, float seconds)
    {
        float frequency = 1f;

        if (leftHand) OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.LTouch);
        if (rightHand) OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.RTouch);

        yield return new WaitForSeconds(seconds);

        if (rightHand) OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        if (leftHand) OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
    }
}
