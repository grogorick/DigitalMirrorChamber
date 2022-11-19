using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TabletButton1 : TriggerAction
{
    [Tooltip("Empty: Regular button\nSelf reference: Toggle\nOther button: XOR toggle")]
    public GameObject toggleOther;

    [Tooltip("Only used if `Toggle Other` is the button itself")]
    public UnityEvent selfToggleReleaseAction;

    [Tooltip("Only used if `Toggle Other` is set")]
    public bool pressed; // pressedIfToggle

    public float releasedPosZ = -.01f;


    private TabletButton1 otherBtn;


    private void Start()
    {
        colliderTag = "ButtonTrigger";

        if (pressed)
            setPressed(true);
        else
            setPressed(false);

        if (toggleOther != null)
        {
            otherBtn = toggleOther.GetComponent<TabletButton1>();
            if (otherBtn == this)
            {
                otherBtn = null;
            }
            else if (otherBtn != null)
            {
                if (otherBtn.toggleOther != gameObject)
                {
                    if (otherBtn.toggleOther == null)
                        Debug.Log("### MY | TabletButton | Button `" + name + "`.toggleOther set to `" + toggleOther.name + "`, which does not reference back. Doing so now");
                    else
                        Debug.LogWarning("### MY | TabletButton | Fixed: Button `" + name + ".toggleOther` set to `" + toggleOther.name + "`, instead of this button. Doing so now");
                    otherBtn.toggleOther = gameObject;
                }
                if (otherBtn.pressed == pressed)
                {
                    Debug.LogWarning("### MY | TabletButton | Fixed: Buttons `" + name + "` and `" + toggleOther.name + "` with identical initial pressed state");
                    otherBtn.setPressed(!otherBtn.pressed);
                }
            }
        }
    }

    protected override void doAction()
    {
        press();
        base.doAction();
    }

    public void press()
    {
        Debug.Log("### MY | TabletButton | Button `" + name + "` press (" + pressed + ")");

        // release other button (if set)
        if (otherBtn != null)
        {
            setPressed(true);
            otherBtn.setPressed(false);
        }

        // release this button (if it's a toggle button)
        else if (toggleOther != null)
        {
            setPressed(!pressed);
        }

        // release this button after a short delay (otherwise)
        else
        {
            setPressed(true);
            releaseBtnAfter();
        }

        vibrate(true, false, .1f);
        vibrate(false, true, 1f, .01f);
    }

    void setPressed(bool press)
    {
        Debug.Log("### MY | TabletButton | Button `" + name + "` setPressed (" + press + ")");

        pressed = press;
        Vector3 pos = transform.localPosition;
        pos.z = press ? 0 : releasedPosZ;
        transform.localPosition = pos;
    }


    void releaseBtnAfter(float seconds = .5f)
    {
        StartCoroutine(_releaseBtnAfter(seconds));
    }

    IEnumerator _releaseBtnAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        setPressed(false);
    }


    void vibrate(bool leftHand, bool rightHand, float amplitude, float seconds = .1f)
    {
        StartCoroutine(_vibrate(leftHand, rightHand, amplitude, seconds));
    }

    IEnumerator _vibrate(bool leftHand, bool rightHand, float amplitude, float seconds)
    {
        float frequency = 1f;

        if (leftHand) OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.LTouch);
        if (rightHand) OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.RTouch);

        yield return new WaitForSeconds(seconds);

        if (rightHand) OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        if (leftHand) OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
    }
}
