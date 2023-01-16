using UnityEngine;
using UnityEngine.Events;

public class TabletButtonToggle : TabletButton
{
    [Tooltip("Only used if `Toggle Other` is the button itself")]
    public UnityEvent buttonReleaseAction;


    protected override void Start()
    {
        bool tmpPressed = pressed;
        base.Start();

        setPressed(tmpPressed);
    }

    protected override void doAction()
    {
        press();
        if (pressed)
        {
            if (action != null)
            {
                Debug.Log("### MY | TabletButtonToggle | `" + name + "` | doAction | action");
                action.Invoke();
            }
            else
                Debug.Log("### MY | TabletButtonToggle | `" + name + "` | doAction | no action");
        }
        else if (buttonReleaseAction != null)
        {
            Debug.Log("### MY | TabletButtonToggle | `" + name + "` | doAction | buttonReleaseAction");
            buttonReleaseAction.Invoke();
        }
    }

    protected override void press()
    {
        Debug.Log("### MY | TabletButtonToggle | `" + name + "` | press (" + pressed + ")");

        setPressed(!pressed);

        vibrate();
    }
}
