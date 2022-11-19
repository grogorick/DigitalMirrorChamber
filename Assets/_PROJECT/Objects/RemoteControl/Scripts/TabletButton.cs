using System.Collections;
using UnityEngine;

public class TabletButton : TriggerAction
{
    public float releasedPosZ = -.01f;
    public bool pressed;


    protected virtual void Start()
    {
        colliderTag = "ButtonTrigger";

        setPressed(false);
    }


    protected override void doAction()
    {
        Debug.Log("### MY | TabletButton | Button `" + name + "` doAction");
        press();
        base.doAction();
    }

    protected virtual void press()
    {
        Debug.Log("### MY | TabletButton | Button `" + name + "` press (" + pressed + ")");

        setPressed(true);
        releaseBtnAfter();

        vibrate();
    }

    protected void setPressed(bool press)
    {
        Debug.Log("### MY | TabletButton | Button `" + name + "` setPressed (" + press + ")");

        pressed = press;
        Vector3 pos = transform.localPosition;
        pos.z = press ? 0 : releasedPosZ;
        transform.localPosition = pos;
    }


    protected void releaseBtnAfter(float seconds = .5f)
    {
        StartCoroutine(_releaseBtnAfter(seconds));
    }

    private IEnumerator _releaseBtnAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        setPressed(false);
    }


    protected void vibrate()
    {
        vibrate(true, false, .1f);
        vibrate(false, true, 1f, .01f);
    }

    protected void vibrate(bool leftHand, bool rightHand, float amplitude, float seconds = .1f)
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
