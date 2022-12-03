# Digital Mirror Chamber
Your digital 360° metaverse avatar fitting room

## Not Included Dependencies
- Oculus Integration (46.0) available via Package Manager
- Meta Avatars SDK (1.50.0) download via MQDH

(Version numbers refer to latest successfulls tested versions)

## Setup
- Clone
- Import/open in UnityHub
  - Some non-store assets will be missing, so choose to *Enter Safe Mode*
- Import *Oculus Integration*
- Import *Meta Avatars SDK*
  - During import uncheck *Oculus/Avatar2/Scripts/AvatarEditorDeeplink/Newtonsoft.Json.dll* (it's already included in Unity and would cause a duplicate error)
- Possible popups:
  - *Update OVR* and potential other plugins
  - "OpenXR Backend" OpenXR is now fully supported by Oculus... &rightarrow; *Use OpenXR* &rightarrow; *Restart*
  - "Enable OculusXR Feature" &rightarrow; *Enable*
  - "Copy SDK Avatars 2.0 Streaming Assets" &rightarrow; *OK*
- In *Project Settings > XR Plug-in Management* enable the *Oculus* Plug-in Provider
- Make sure *Menu > Oculus > Tools > OVR Utilities Plugin*> Set OVRPlugin to OpenXR* is checked
- In *Menu > Oculus > Platform > Edit Settings* enter App-IDs for *Rift* and/or *Quest* applications created in the [DevHub](https://developer.oculus.com/manage)
  - Optionally *Use Standalone Platform* and login with *Test User Email/Password* (from the DevHub again)
