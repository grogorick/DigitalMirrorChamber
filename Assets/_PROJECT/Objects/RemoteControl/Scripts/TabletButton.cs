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
        Debug.Log("### MY | TabletButton | `" + name + "` | doAction");
        press();
        base.doAction();
    }

    protected virtual void press()
    {
        Debug.Log("### MY | TabletButton | `" + name + "` | press (" + pressed + ")");

        setPressed(true);
        releaseBtnAfter();

        vibrate();
    }

    protected void setPressed(bool press)
    {
        Debug.Log("### MY | TabletButton | `" + name + "` | setPressed (" + press + ")");

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
        Vibrate.now(true, false, .1f);
        Vibrate.now(false, true, 1f, .01f);
    }
}
