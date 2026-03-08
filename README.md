# GL-Gizmos
Tries to use UnityEngine.GL to make Debug.DrawLine/GIzmo's work in builds of Unity games.

# TODO: 
##### (pls help with any of these if u know how to do them)
- make GLGizmos.Patches.GizmosPatch.DrawGizmoIcon actually get the fucking texture omfg unity what the hell are you doing im gonna ckill you
- add support for Gizmos.DrawGUITexture
- add support for Gizmos.probeSize/exposure
- add a harmony manager so we only patch the stuff that actually exist
  
- uhm make another branch for like Scriptable Render Pipeline games since rn only we only support Built-in Render Pipeline due to Camera.OnPostRender not existing in there
- test in more versions of unity/more games cuz rn ive only really tested ultrakill :P
- if any1 knows how please optimise what im doing for invoking MonoBehaviour.OnDrawGizmos/OnDrawGizmosSelected
