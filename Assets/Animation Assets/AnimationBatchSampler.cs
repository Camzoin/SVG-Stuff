#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class AnimationBatchSampler : MonoBehaviour
{
    public static void BatchSampleAnimation(
        AnimationClip clip,
        List<GameObject> targets,
        List<float> times)
    {
        if (clip == null || targets == null || times == null)
        {
            Debug.LogError("Invalid input parameters");
            return;
        }

        if (targets.Count != times.Count)
        {
            Debug.LogError("Targets and times lists must be the same length");
            return;
        }

        // Record undo for all targets
        Undo.RecordObjects(targets.ToArray(), "Batch Sample Animation");

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] == null) continue;

            // Sample animation at specified time
            clip.SampleAnimation(targets[i], times[i]);

            // Mark the object as dirty
            EditorUtility.SetDirty(targets[i]);
        }

        // Force scene view to update
        SceneView.RepaintAll();
    }
}
#endif