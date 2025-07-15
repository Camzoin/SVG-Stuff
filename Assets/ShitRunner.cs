using UnityEngine;

public class ShitRunner : MonoBehaviour
{
    public svgVisual svgVis;

    [ContextMenu("Reset Render Texture Res")]
    public void DoRenderTextureShit()
    {
        svgVis.DoRenderTextureShit();
    }

    [ContextMenu("Make Lines Based on Image")]
    public void GenerateRTWork()
    {
        svgVis.GenerateRTWork();
    }

    //Animation Shit

    [ContextMenu("BuildAnimationCellsFromAnimation")]
    public void BuildAnimationCellsFromAnimation()
    {
        svgVis.animManager.BuildAnimationCellsFromAnimation();
    }

    [ContextMenu("Duplicate Reference Animation Cell For Animation")]
    public void DuplicateReferenceAnimationCellForAnimation()
    {
        svgVis.animManager.DuplicateReferenceAnimationCellForAnimation();
    }
}
