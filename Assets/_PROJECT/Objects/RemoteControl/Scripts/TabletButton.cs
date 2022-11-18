using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TabletButton : MonoBehaviour
{
    public UnityEvent action;

    [Tooltip("Regular button, if empty")]
    public GameObject toggleOther;

    public bool pressed; // pressedIfToggle

    public float releasedPosZ = -.01f;


    private void Start()
    {
        if (pressed)
            setPressed(true);
        else
            setPressed(false);

        if (toggleOther != null)
        {
            TabletButton otherBtn = toggleOther.GetComponent<TabletButton>();
            if (otherBtn != null)
            {
                if (otherBtn.toggleOther != gameObject)
                {
                    if (otherBtn.toggleOther == null)
                        Debug.Log("### MY | TabletButton | Button `" + name + "`.toggleOther set to `" + toggleOther.name + "`, which does not reference back. Doing so now");
                    else
                        Debug.LogWarning("### MY | TabletButton | Fixed: Button `" + name + ".toggleOther` set to `" + toggleOther.name + "`, which references another object");
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

    public void press()
    {
        Debug.Log("### MY | TabletButton | Button `" + name + "` press (" + pressed + ")");

        setPressed(true);
        vibrate(true, false, .1f);
        vibrate(false, true, 1f, .01f);

        // release other button (if set)
        if (toggleOther != null) {
            TabletButton otherBtn = toggleOther.GetComponent<TabletButton>();
            if (otherBtn != null)
                otherBtn.setPressed(false);
        }

        // otherwise release this button
        else
            releaseBtnAfter();

        action.Invoke();
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
        StartCoroutine(_releaseBtn(seconds));
    }

    IEnumerator _releaseBtn(float seconds)
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
