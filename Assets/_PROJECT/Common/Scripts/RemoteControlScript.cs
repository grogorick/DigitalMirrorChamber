using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteControlScript : MonoBehaviour
{
    [Header("Avatar")]
    public string rightIndexFingerJointName = "Joint RightHandIndexDistal";
    public string leftHandJointName = "Joint LeftHandIndexProximal";

    private GameObject rightIndexFingerJoint;
    private GameObject leftHandJoint;


    [Header("Tablet")]
    public GameObject tabletHandleObject;


    [System.Serializable]
    public class Btn
    {
        public GameObject obj;
        public bool pressed;
    }
    [Header("Tablet Buttons")]
    public Btn wall;
    public Btn clone;
    public float releasedPosZ = -.01f;

    private Btn[] btns;
    private int buttonWallIdx, buttonCloneIdx;
    private Dictionary<int, int> otherBtn = new ();


    private void Start()
    {
        btns = new Btn[] { wall, clone };
        buttonWallIdx = Array.IndexOf(btns, wall);
        buttonCloneIdx = Array.IndexOf(btns, clone);

        otherBtn.Add(buttonWallIdx, buttonCloneIdx);
        otherBtn.Add(buttonCloneIdx, buttonWallIdx);

        for (int i = 0; i < btns.Length; i++)
            if (btns[i].pressed)
                setPressed(i, true);
            else
                setPressed(i, false);

        if (wall.pressed == clone.pressed)
            Debug.LogWarning("### MY | Buttons `Wall` and `Clone` with identical initial pressed state");
    }

    public void AvatarLoaded()
    {
        leftHandJoint = GameObject.Find(leftHandJointName);
        if (leftHandJoint != null)
        {
            Debug.Log("### MY | AvatarLoaded: `" + leftHandJoint.name + "` found");

            tabletHandleObject.transform.parent = leftHandJoint.transform;
            tabletHandleObject.transform.localPosition = Vector3.zero;
            tabletHandleObject.transform.localRotation = Quaternion.identity;
        }
        else
            Debug.LogError("### MY | AvatarLoaded: `" + leftHandJointName + "` NOT found");


        rightIndexFingerJoint = GameObject.Find(rightIndexFingerJointName);
        if (rightIndexFingerJoint != null)
        {
            Debug.Log("### MY | AvatarLoaded: `" + rightIndexFingerJoint.name + "` found");

            transform.parent = rightIndexFingerJoint.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
            Debug.LogError("### MY | AvatarLoaded: `" + rightIndexFingerJointName + "` NOT found");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Button"))
        {
            for (int i = 0; i < btns.Length; i++)
            {
                if (btns[i].obj == other.gameObject)
                {
                    //Debug.Log("### MY | Button `" + btns[i].obj.name + "` trigger (" + btns[i].pressed + ")");

                    if (!btns[i].pressed)
                        press(i);

                    break;
                }
            }
        }
    }

    [Header("Spinning Platform")]
    public GameObject platformObject;
    public float rotationSpeed = .1f;

    float rotationStart = 0;
    float rotationTarget = 0;
    float rotationAlpha = 0;
    float rotation = 0;

    private void FixedUpdate()
    {
        if (rotationAlpha != 1)
        {
            rotationAlpha += rotationSpeed;
            if (rotationAlpha > 1)
                rotationAlpha = 1;

            rotation = Mathf.SmoothStep(rotationStart, rotationTarget, rotationAlpha);
            platformObject.transform.rotation = Quaternion.Euler(0, rotation, 0);
        }
    }

    public void press(int btnIdx)
    {
        Debug.Log("### MY | Button `" + btns[btnIdx].obj.name + "` press (" + btns[btnIdx].pressed + ")");

        setPressed(btnIdx, true);
        vibrate(true, false, .1f);
        vibrate(false, true, 1f, .01f);

        // release other button
        if (otherBtn.TryGetValue(btnIdx, out int otherBtnIdx))
            setPressed(otherBtnIdx, false);


        // actions
        if (btnIdx == buttonWallIdx || btnIdx == buttonCloneIdx)
        {
            rotationStart = rotation;
            rotationAlpha = 0;
            if (btnIdx == buttonWallIdx)
                rotationTarget = 0;
            else
                rotationTarget = 180;
        }
    }

    void setPressed(int btnIdx, bool press)
    {
        Debug.Log("### MY | Button `" + btns[btnIdx].obj.name + "` setPressed (" + press + ")");

        btns[btnIdx].pressed = press;
        Vector3 pos = btns[btnIdx].obj.transform.localPosition;
        pos.z = press ? 0 : releasedPosZ;
        btns[btnIdx].obj.transform.localPosition = pos;
    }

    void vibrate(bool leftHand, bool rightHand, float amplitude, float duration = .1f)
    {
        StartCoroutine(_vibrate(leftHand, rightHand, amplitude, duration));
    }

    IEnumerator _vibrate(bool leftHand, bool rightHand, float amplitude, float duration)
    {
        float frequency = 1f;

        if (leftHand) OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.LTouch);
        if (rightHand) OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.RTouch);

        yield return new WaitForSeconds(duration);

        if (rightHand) OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        if (leftHand) OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
    }


    //float source = 0;
    //float target = 0;
    //float rotation = 0;
    //float speed = 1;

    //void FixedUpdate()
    //{
    //    if (OVRInput.GetActiveController() != OVRInput.Controller.Hands)
    //    {
    //        //if (Mathf.Abs(rotation) < rotationStartThresh)
    //        {
    //            OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick);
    //            Vector2 thumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
    //            if (thumbstick.x != 0)
    //            {
    //                source = (rotation += thumbstick.x);
    //            }
    //        }
    //        //else
    //        {
    //            rotation += .1f * (target - rotation);
    //        }
    //    }
    //    transform.rotation = Quaternion.Euler(0, rotation, 0);
    //}
}
