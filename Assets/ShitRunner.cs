using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ShitRunner : MonoBehaviour
{
    public svgVisual svgVis;

    public int copies = 1;

    public bool rerollCopies = false;

    public Vector2 copyMinMaxSpawnMulti = new Vector2(1, 1);

    public bool lerpSpawnMulti = false;

    public int frames = 12;

    public int targetFPS = 12;

    public Camera firstCellCam;

    public Animator firstCellAnimator;

    [ContextMenu("Reset Render Texture Res")]
    public void DoRenderTextureShit()
    {
        svgVis.DoRenderTextureShit();
    }

    [ContextMenu("Make Lines Based on Image")]
    public void GenerateRTWork()
    {
        if (firstCellCam == null)
        {
            firstCellCam = svgVis.animManager.animCells[0].cellCam;
        }

        if (firstCellAnimator == null)
        {
            firstCellAnimator = svgVis.animManager.animCells[0].cellAnimator;
        }


        bool ogSetting = svgVis.docSettings.generateArtWithNewSeed;

        // 1. Fixed Timestep: Dictates the duration of one 'Step' in seconds.
        // For 60 FPS, this should be ~0.01666s.
        Time.fixedDeltaTime = 1f / targetFPS;

        // 2. Maximum Allowed Timestep: Prevents physics "death spirals."
        // Usually set to 2x or 3x the fixed timestep.
        Time.maximumDeltaTime = Time.fixedDeltaTime * 2f;


        string pieceName = "";

        for (int f = 0; f < frames; f++)
        {
            for (int i = 0; i < copies; i++)
            {
                if (pieceName == "")
                {
                    pieceName = svgVis.GenerateRTWork(i, f);
                }
                else

                {
                   svgVis.GenerateRTWork(i, f, pieceName);
                }


                if (rerollCopies == true)
                {
                    svgVis.docSettings.generateArtWithNewSeed = true;
                }
                else
                {
                    svgVis.docSettings.generateArtWithNewSeed = false;
                }


            }

            firstCellAnimator.Update(Time.fixedDeltaTime);

            EditorApplication.Step();
            SceneView.RepaintAll();



            var request = new RenderPipeline.StandardRequest();

            // 3. Execute the Request
            // This is the modern replacement for RenderSingleCamera
            RenderPipeline.SubmitRenderRequest(firstCellCam, request);




            Camera.main.Render();
            firstCellCam.Render();
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
