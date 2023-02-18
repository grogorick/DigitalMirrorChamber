# Digital Mirror Chamber
A digital 360° metaverse avatar fitting room to take a closer look at your customized avatar

## Not Included Dependencies
- *Oculus Integration* (49.0) available via Package Manager
- *Meta Avatars SDK* (20.1) download via [MQDH](https://developer.oculus.com/meta-quest-developer-hub/)

(Version numbers refer to latest successfully tested versions)

## Setup
- Clone
- Import/open in UnityHub
  - Some non-store assets will be missing, so choose to *Enter Safe Mode*
- Import *Oculus Integration*
- Import *Meta Avatars SDK*
  - During import uncheck *Oculus/Avatar2/Scripts/AvatarEditorDeeplink/Newtonsoft.Json.dll* (it's already included in Unity and would cause a duplicate error)
- *Update OVR* and potential other plugins, e.g.:
  - "OpenXR Backend" OpenXR is now fully supported by Oculus... &rightarrow; *Use OpenXR* &rightarrow; *Restart*
  - "Enable OculusXR Feature" &rightarrow; *Enable*
  - "Copy SDK Avatars 2.0 Streaming Assets" &rightarrow; *OK*
- In *Project Settings > XR Plug-in Management* enable the *Oculus* Plug-in Provider
- Make sure *Menu > Oculus > Tools > OVR Utilities Plugin > Set OVRPlugin to OpenXR* is checked
- In *Menu > Oculus > Platform > Edit Settings* enter App-IDs for *Rift* and/or *Quest* applications created in the [DevHub](https://developer.oculus.com/manage)
  - (This is mandatory to use the Avatars SDK)
  - Optionally, for in-Editor-use, activate *Use Standalone Platform* and login with *Test User Email/Password* from the DevHub again
    - (While this enables the Avatars SDK, the *Avatar Editor* still does not work)
