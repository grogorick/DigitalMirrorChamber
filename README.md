# Digital Mirror Chamber
A digital 360° metaverse avatar fitting room to take a closer look at your customized avatar

![cover image](https://raw.githubusercontent.com/grogorick/DMC-Store-Content/main/cover.png)

## Not Included Dependencies
- *Unity Editor* (2021.3.8f1)
- *Oculus Integration* (49.0) &mdash; available via Package Manager
- *Meta Avatars SDK* (20.1) &mdash; download via [MQDH](https://developer.oculus.com/meta-quest-developer-hub/)

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

## Third-party Components
3D Models
- *4x4 Freestanding Reconfigurable IQ Wall* &mdash; CC BY 4.0 Advanced Visualization Lab - Indiana University via [Sketchfab](https://sketchfab.com/3d-models/4x4-freestanding-reconfigurable-iq-wall-cb68d2f9553b455289e46a94a340c753)
- *FREE Flower Ceramic Vases* &mdash; Monqo Studios via [AssetStore](https://assetstore.unity.com/packages/3d/vegetation/flowers/free-flower-ceramic-vases-187046)
- *Free Statue Pack* &mdash; Storm Bringer Studios via [AssetStore](https://assetstore.unity.com/packages/3d/props/interior/free-statue-pack-152443)
- *HDRP Furniture Pack* &mdash; Tridify via [AssetStore](https://assetstore.unity.com/packages/3d/props/furniture/hdrp-furniture-pack-153946)
- *Logitech Webcam* &mdash; CC BY 4.0 yimni via [Sketchfab](https://sketchfab.com/3d-models/logitech-webcam-ef31d877ff6e4aa4af565cddadf7a4c8)
- *Mario Floor Lamp* &mdash; CC BY 4.0 Andriano Milanovic via [Sketchfab](https://sketchfab.com/3d-models/mario-floor-lamp-15b2782b523b486ea169d6e16e72123f)
- *Sound system* &mdash; CC BY 4.0 yryabchenko via [Sketchfab](https://sketchfab.com/3d-models/sound-system-725273fbdde54a1babaf6ce1c95b96b4)
- *YGS Mugs* &mdash; Rodolfo Rubens via [AssetStore](https://assetstore.unity.com/packages/3d/props/interior/ygs-mugs-96665)
- *Yughues Free Decorative Plants* &mdash; Yughues via [AssetStore](https://assetstore.unity.com/packages/3d/props/interior/yughues-free-decorative-plants-13283)

Environment Map
- *Belfast Sunset (Pure Sky)* &mdash; CC0 Dimitrios Savva, Greg Zaal, Jarod Guest via [Poly Haven](https://polyhaven.com/a/belfast_sunset_puresky)

Materials/Textures
- *Coffee Cup PBR* &mdash; AK STUDIO ART via [AssetStore](https://assetstore.unity.com/packages/3d/props/food/coffee-cup-pbr-224789)
- *Tiling Textures - 3D Microgame Add-Ons* &mdash; Unity Technologies via [AssetStore](https://assetstore.unity.com/packages/2d/textures-materials/tiling-textures-3d-microgame-add-ons-174461)
- *Wooden Floor Materials* &mdash; Casual2D via [AssetStore](https://assetstore.unity.com/packages/2d/textures-materials/wood/wooden-floor-materials-150564)

Music
- *Solo Acoustic #3 (2:06)* &mdash; CC BY 4.0 Jason Shaw via [Audionautix.com](https://audionautix.com/free-music/acoustic)

Other
- *Create-with-VR_2021LTS.zip* &mdash; Unity Technologies via [Unity Learn](https://learn.unity.com/tutorial/vr-project-setup?uv=2021.3)
