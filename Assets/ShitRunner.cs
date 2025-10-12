using UnityEngine;

public class ShitRunner : MonoBehaviour
{
    public svgVisual svgVis;

    public int copies = 1;

    public bool rerollCopies = false;

    public Vector2 copyMinMaxSpawnMulti = new Vector2(1, 1);

    public bool lerpSpawnMulti = false;

    [ContextMenu("Reset Render Texture Res")]
    public void DoRenderTextureShit()
    {
        svgVis.DoRenderTextureShit();
    }

    [ContextMenu("Make Lines Based on Image")]
    public void GenerateRTWork()
    {
        bool ogSetting = svgVis.docSettings.generateArtWithNewSeed;

        for (int i = 0; i < copies; i++)
        {


            svgVis.GenerateRTWork(i);

            if (rerollCopies == true)
            {
                svgVis.docSettings.generateArtWithNewSeed = true;
            }
            else
            {
                svgVis.docSettings.generateArtWithNewSeed = false;
            }


        }

        svgVis.docSettings.generateArtWithNewSeed = ogSetting;
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
