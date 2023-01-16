using UnityEngine;

public class TabletButtonToggleOther : TabletButton
{
    public TabletButtonToggleOther otherButton;


    protected override void Start()
    {
        bool tmpPressed = pressed;
        base.Start();

        setPressed(tmpPressed);

        if (otherButton != null)
        {
            if (otherButton.otherButton != this)
            {
                if (otherButton.otherButton == null)
                    Debug.Log("### MY | TabletButtonToggleOther | `" + name + "` | `otherButton` set to `" + otherButton.name + "`, which does not reference back. Doing so now");
                else
                    Debug.LogWarning("### MY | TabletButtonToggleOther | `" + name + " | `otherButton` set to `" + otherButton.name + "`, instead of this button. Doing so now");
                otherButton.otherButton = this;
            }
            if (otherButton.pressed == pressed)
            {
                Debug.LogWarning("### MY | TabletButtonToggleOther | `" + name + "` | This and `" + otherButton.name + "` with identical initial pressed state. Inverting the other");
                otherButton.setPressed(!otherButton.pressed);
            }
        }
        else
            Debug.LogWarning("### MY | TabletButtonToggleOther | `" + name + " | `otherButton` not set");
    }

    protected override void press()
    {
        Debug.Log("### MY | TabletButtonToggleOther | `" + name + "` | press (" + pressed + ")");

        if (otherButton != null)
        {
            setPressed(true);
            otherButton.setPressed(false);
        }

        vibrate();
    }
}
