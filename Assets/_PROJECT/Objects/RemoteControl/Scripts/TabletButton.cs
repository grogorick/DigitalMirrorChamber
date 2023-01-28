using System.Collections;
using UnityEngine;

public class TabletButton : TriggerAction
{
    public float releasedPosZ = -.01f;
    public bool pressed;
    private AudioSource clickSound;
    private Material material;
    private Color initialEmissionColor;
    public float pressedEmissionFactor = 8f;


#if UNITY_EDITOR
    public bool DEBUG_PRESS;
    private void Update()
    {
        if (DEBUG_PRESS)
        {
            DEBUG_PRESS = false;
            doAction();
        }
    }
#endif


    protected virtual void Start()
    {
        colliderTag = "ButtonTrigger";
        clickSound = GetComponent<AudioSource>();
        material = GetComponent<MeshRenderer>().material;
        initialEmissionColor = material.GetColor("_EmissionColor");
        Debug.Log("### MY | TabletButton | init | `_EmissionColor` = " + initialEmissionColor);

        setPressed(false);
    }


    protected override void doAction()
    {
        Debug.Log("### MY | TabletButton | `" + name + "` | doAction");
        press();
        playClickSound();
        base.doAction();
    }

    protected void playClickSound()
    {
        if (action.GetPersistentEventCount() > 0)
            clickSound.Play();
    }

    protected virtual void press()
    {
        Debug.Log("### MY | TabletButton | `" + name + "` | press (" + pressed + ")");

        setPressed(true);
        releaseBtnAfter();

        vibrate();
    }

    public void setPressed(bool press)
    {
        Debug.Log("### MY | TabletButton | `" + name + "` | setPressed (" + press + ")");

        pressed = press;
        Vector3 pos = transform.localPosition;
        pos.z = press ? 0 : releasedPosZ;
        transform.localPosition = pos;

        Color color = initialEmissionColor * (pressed ? pressedEmissionFactor : 1f);
        Debug.Log("### MY | TabletButton | `_EmissionColor` = " + color);
        material.SetColor("_EmissionColor", color);
    }


    public void releaseBtnAfter(float seconds = .5f)
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
