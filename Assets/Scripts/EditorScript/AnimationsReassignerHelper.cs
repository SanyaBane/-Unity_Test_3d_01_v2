using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Animations;

#endif

public class AnimationsReassignerHelper : MonoBehaviour
{
    // public string SourceAnimFilePrefix = "hyur_m-a0001";
    // public string DestinationAnimFilePrefix = "miqote_f-a0001";

    public GameObject SourceAnimFile;
    public GameObject DestinationAnimFile;

    public void DoWork()
    {
#if UNITY_EDITOR
        var animator = this.GetComponent<Animator>();
        var animatorController = (AnimatorController) animator.runtimeAnimatorController;

        // animatorController.

        // Transform[] children = transform.GetComponentsInChildren<Transform>();
        // foreach (var child in children)
        // {
        //     if (child.name == SelectChildName)
        //     {
        //         Selection.activeGameObject = child.gameObject;
        //     }
        // }


        // var mda = Resources.LoadAll<AnimationClip>("Models/FFXIV/Characters/Ardbert/hyur_m-a0001-bt_common-resident.fbx");

        // ModelImporter modelImporter = (ModelImporter)AssetImporter.GetAtPath("Assets/Models/FFXIV/Characters/Ardbert/hyur_m-a0001-bt_common-resident.fbx");
        // ModelImporterClipAnimation clip = modelImporter.clipAnimations[0]; // get first clip

        // var asObject = clip as UnityEngine.Object;

        // string guid_01;
        // long localId_01;
        // AssetDatabase.TryGetGUIDAndLocalFileIdentifier(SourceAnimFile, out guid_01, out localId_01);

        // foreach (var animClip in animatorController.animationClips)
        // {
        //     string guid_02;
        //     long localId_02;
        //     AssetDatabase.TryGetGUIDAndLocalFileIdentifier(animClip, out guid_02, out localId_02);
        //     
        //     break;
        // }

        // var animations = AnimationUtility.GetAnimationClips(SourceAnimFile);


        Debug.Log("");

        // Resources.Load
#endif
    }
}