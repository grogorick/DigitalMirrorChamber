using Oculus.Avatar2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RemoteControlScript : MonoBehaviour
{
    [Header("Avatar")]
    public GameObject avatarObject;

    public string tabletAttachPointJointName = "Joint LeftHandIndexMeta";
    private GameObject tabletAttachPointJoint;
    public string tabletTouchFingerJointName = "Joint RightHandIndexDistal";
    private GameObject tabletTouchFingerJoint;

    public string leftHandWristJointName = "Joint LeftHandWrist";
    private GameObject leftHandWristJoint;
    public string rightHandWristJointName = "Joint RightHandWrist";
    private GameObject rightHandWristJoint;
    public GameObject leftHandSkeleton;
    public GameObject rightHandSkeleton;
    public bool updateLeftHandPose = false;
    public bool updateRightHandPose = false;

    [Header("Tablet")]
    public GameObject tabletHandleObject;


    [Serializable]
    public class ToggleBtn
    {
        public GameObject obj;
        public bool pressed;
    }
    [Header("Tablet Buttons")]
    public float releasedPosZ = -.01f;
    public ToggleBtn wall;
    public ToggleBtn clone;
    public ToggleBtn camerasUp;
    public ToggleBtn camerasDown;

    private ToggleBtn[] btns;
    private int buttonWallIdx, buttonCloneIdx, buttonCamerasUpIdx, buttonCamerasDownIdx;
    private Dictionary<int, int> otherBtn = new();


    private void Start()
    {
        btns = new ToggleBtn[] { wall, clone, camerasUp, camerasDown };
        buttonWallIdx = Array.IndexOf(btns, wall);
        buttonCloneIdx = Array.IndexOf(btns, clone);
        buttonCamerasUpIdx = Array.IndexOf(btns, camerasUp);
        buttonCamerasDownIdx = Array.IndexOf(btns, camerasDown);

        otherBtn.Add(buttonWallIdx, buttonCloneIdx);
        otherBtn.Add(buttonCloneIdx, buttonWallIdx);

        for (int i = 0; i < btns.Length; i++)
            if (btns[i].pressed)
                setPressed(i, true);
            else
                setPressed(i, false);

        if (wall.pressed == clone.pressed)
        {
            Debug.LogWarning("### MY | Buttons `Wall` and `Clone` with identical initial pressed state");
            setPressed(buttonCloneIdx, !clone.pressed);
        }


        // cache y-pos of mirror cameras in scene
        foreach (var cam in webcams)
            cam.maxY = cam.cameraHandle.transform.localPosition.y;


        // init animation caches
        for (int i = 0; i < (int)ANIM.COUNT; i++)
            anims.Add((ANIM)i, new());
    }

    OvrAvatarCustomHandPose[] customHandPoses;

    public void AvatarLoaded()
    {
        if (tabletAttachPointJoint == null)
        {
            tabletAttachPointJoint = findAvatarJoint(tabletAttachPointJointName);
            changeParent(tabletHandleObject, tabletAttachPointJoint);

            tabletTouchFingerJoint = findAvatarJoint(tabletTouchFingerJointName);
            changeParent(gameObject, tabletTouchFingerJoint);

            leftHandWristJoint = findAvatarJoint(leftHandWristJointName);

            rightHandWristJoint = findAvatarJoint(rightHandWristJointName);
        }

        customHandPoses = avatarObject.GetComponents<OvrAvatarCustomHandPose>();

        changeParent(leftHandSkeleton, leftHandWristJoint, ChangeParentMode.keepLocalRotation | ChangeParentMode.resetLocalPosition);
        //leftHandSkeleton.transform.position = leftHandWristJoint.transform.position;
        //leftHandSkeleton.transform.rotation = leftHandWristJoint.transform.rotation;
        customHandPoses[0].UpdateHandPose();

        changeParent(rightHandSkeleton, rightHandWristJoint, ChangeParentMode.keepLocalRotation | ChangeParentMode.resetLocalPosition);
        //rightHandSkeleton.transform.position = rightHandWristJoint.transform.position;
        //rightHandSkeleton.transform.rotation = rightHandWristJoint.transform.rotation;
        customHandPoses[1].UpdateHandPose();
    }

    private void Update()
    {
        if (updateLeftHandPose)
        {
            updateLeftHandPose = false;
            customHandPoses[0].UpdateHandPose();
        }
        if (updateRightHandPose)
        {
            updateRightHandPose = false;
            customHandPoses[1].UpdateHandPose();
        }
    }

    private GameObject findAvatarJoint(string jointName)
    {
        GameObject joint = avatarObject.transform.FindChildRecursive(jointName)?.gameObject;
        if (joint != null)
            Debug.Log("### MY | AvatarLoaded: `" + joint.name + "` found");
        else
            Debug.LogError("### MY | AvatarLoaded: `" + jointName + "` NOT found");
        return joint;
    }

    [Flags]
    enum ChangeParentMode
    {
        unchanged = 0x0000,
        resetLocalPosition = 0x0001,
        resetLocalRotation = 0x0010,
        resetLocalPose = 0x0011,
        keepLocalPosition = 0x0100,
        keepLocalRotation = 0x1000,
        keepLocalPose = 0x1100
    }
    private void changeParent(GameObject obj, GameObject newParent, ChangeParentMode mode = ChangeParentMode.resetLocalPose)
    {
        Vector3 tmpPos = obj.transform.localPosition;
        Quaternion tmpRot = obj.transform.localRotation;

        obj.transform.parent = newParent.transform;

        if (ChangeParentMode.unchanged != (mode & ChangeParentMode.keepLocalPosition))
            obj.transform.localPosition = tmpPos;
        if (ChangeParentMode.unchanged != (mode & ChangeParentMode.keepLocalRotation))
            obj.transform.localRotation = tmpRot;

        if (ChangeParentMode.unchanged != (mode & ChangeParentMode.resetLocalPosition))
            obj.transform.localPosition = Vector3.zero;
        if (ChangeParentMode.unchanged != (mode & ChangeParentMode.resetLocalRotation))
            obj.transform.localRotation = Quaternion.identity;
    }

    [Serializable]
    public class ButtonEvent
    {
        public GameObject button;
        public UnityEvent action;
    }
    [Header("Events")]
    public List<ButtonEvent> onButtonClick = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Button"))
        {
            for (int i = 0; i < btns.Length; i++)
            {
                if (btns[i].obj == other.gameObject)
                {
                    Debug.Log("### MY | Button `" + btns[i].obj.name + "` triggered");
                    ButtonEvent evt = onButtonClick.Find(be => be.button == other.gameObject);
                    if (evt != null)
                        evt.action.Invoke();

                    if (!btns[i].pressed)
                        press(i);

                    break;
                }
            }
        }
    }

    [Header("Spinning Platform")]
    public GameObject platformObject;
    public float rotationSpeed = .01f;

    class Anim
    {
        public float alpha = 1, speed = 0, start = 0, end = 0, result = 0;
        public bool changed = false;

        public void animate(float endValue)
        {
            start = result;
            alpha = 0;
            end = endValue;
        }

        public void animateDiff(float diffValue, float clampMin = 0, float clampMax = 1)
        {
            animate(Mathf.Clamp(result + diffValue, clampMin, clampMax));
        }

        public void step()
        {
            if (alpha != 1)
            {
                alpha = Mathf.Min(1, alpha + speed);
                result = Mathf.SmoothStep(start, end, alpha);
                changed = true;
            }
        }
    }
    Dictionary<ANIM, Anim> anims = new();
    enum ANIM { rotatePlatform, moveCameras, COUNT }

    private void FixedUpdate()
    {
        // run all animations
        anims[ANIM.rotatePlatform].speed = rotationSpeed;
        anims[ANIM.moveCameras].speed = moveCameraSpeed;
        foreach (var anim in anims.Values)
            anim.step();

        // platform rotation
        var rot = anims[ANIM.rotatePlatform];
        if (rot.changed)
            platformObject.transform.rotation = Quaternion.Euler(0, rot.result, 0);

        // webcam movement
        var mc = anims[ANIM.moveCameras];
        if (mc.changed)
        {
            foreach (var cam in webcams)
            {
                var pos = cam.cameraHandle.transform.localPosition;
                pos.y = Mathf.Lerp(cam.maxY, cam.minY, mc.result);
                cam.cameraHandle.transform.localPosition = pos;
            }
        }
    }

    [Serializable]
    public class WebCam
    {
        public GameObject cameraHandle;
        public float minY;

        [System.NonSerialized]
        public float maxY;
    }
    [Header("Mirror Webcams")]
    public List<WebCam> webcams;
    public float moveCameraSpeed = .01f;

    public void press(int btnIdx)
    {
        Debug.Log("### MY | Button `" + btns[btnIdx].obj.name + "` press (" + btns[btnIdx].pressed + ")");

        setPressed(btnIdx, true);
        vibrate(true, false, .1f);
        vibrate(false, true, 1f, .01f);

        // release other button (if set)
        if (otherBtn.TryGetValue(btnIdx, out int otherBtnIdx))
            setPressed(otherBtnIdx, false);

        // otherwise release this button
        else
            releaseBtnAfter(btnIdx);


        // actions
        if (btnIdx == buttonWallIdx)
            action_rotateToDisplayWall();
        else if (btnIdx == buttonCloneIdx)
            action_rotateToMirrorAvatar();
        else if (btnIdx == buttonCamerasUpIdx)
            action_moveCamerasUp();
        else if (btnIdx == buttonCamerasDownIdx)
            action_moveCamerasDown();
    }

    void action_rotateToDisplayWall()
    {
        anims[ANIM.rotatePlatform].animate(0);
    }
    void action_rotateToMirrorAvatar()
    {
        anims[ANIM.rotatePlatform].animate(180);
    }

    void action_moveCamerasUp()
    {
        anims[ANIM.moveCameras].animateDiff(-.25f);
    }
    void action_moveCamerasDown()
    {
        anims[ANIM.moveCameras].animateDiff(.25f);
    }

    void setPressed(int btnIdx, bool press)
    {
        Debug.Log("### MY | Button `" + btns[btnIdx].obj.name + "` setPressed (" + press + ")");

        btns[btnIdx].pressed = press;
        Vector3 pos = btns[btnIdx].obj.transform.localPosition;
        pos.z = press ? 0 : releasedPosZ;
        btns[btnIdx].obj.transform.localPosition = pos;
    }

    void releaseBtnAfter(int btnIdx, float seconds = .5f)
    {
        StartCoroutine(_releaseBtn(btnIdx, seconds));
    }

    IEnumerator _releaseBtn(int btnIdx, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        setPressed(btnIdx, false);
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
