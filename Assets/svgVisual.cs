using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Linq;

public class svgVisual : MonoBehaviour
{
    public FillLineSettings fillSettings;

    public TextFillSettings textFillSettings;

    public OutlineAndLineSettings lineSettings;

    public DocAndColorSettings docSettings;

    public SquiggleLineSettings squiggleLineSettings;

    public SimpleShapeSpamSettings shapeSpamSettings;

    public List<List<Vector2>> listOfPaths = new List<List<Vector2>>();

    public List<List<List<Vector2>>> listOfAllPathsThisRun = new List<List<List<Vector2>>>();

    public List<List<List<Vector2>>> listsOfAllPathsByColor = new List<List<List<Vector2>>>();





    private List<LineRenderer> lineObjects = new List<LineRenderer>();

    public List<Color> potentialColors = new List<Color>();

    public int shapeCount = 1;



    public bool dipInPaint = false;

    public bool isFinalRender = false;

    public float flowFieldMovementMulti = 2;

    private List<Vector2> flowFieldPositions = new List<Vector2>();

    private List<Vector2> flowFieldDirections = new List<Vector2>();

    public int pathCount = 5000;

    public Vector2 circleRadius;

    private int spawnCircleVertCount = 36000;

    public Vector2 constantSpeedMinMax = new Vector2(0.1f, 5f);

    public int pathLength = 250;

    public int flowFieldNodeCount = 400;

    public float maxInfluenceDist = 400;


    public List<int> posibleShapeVertCount = new List<int>();

    public Vector2 shapeCountMinMax;

    private Vector2 constantMoveValue = Vector2.zero;

    public float randomOffsetFromCenter = 0.75f;

    public Vector2 constantFlowDir;

    public bool useRandomContantFlowDir = false;

    public bool useFlowFromSpawn = false;

    public float flowFromCenterMulti = 1;

    public Vector2 additionalSpawnOffset = new Vector2();

    public float shapeRotation = 0;

    public bool useRandomRoation = false;


    private List<Color> plotColors = new List<Color>();







    public float lineWidth = 0.5f;



    public bool redoFlowFieldForNewColors = true;

    public bool randomlyFlipFlowDirectionsPerColor = false;

    public bool randomlyFlipShapePositionPerColor = false;

    public bool flipFlowsAndPositions = false;

    public bool rotateEachShape45 = false;

    public bool fitEachShapeInLast = false;

    public bool drawBounds = true;

    public bool drawSpawnShapes = true;

    public bool removeColorOptionAfterUse = true;

    public Color bgColor;

    public int paperIndex;



    public float chosenSize;


    public string generatorName;

    public string artistName;

    public string recipientName;

    public string versionNumber;

    public int seedValue;

    public string fileURL = "";

    public string shortURL = "";

    public string piecename = "";

    public string yourFileName = "yourFileName";

    public List<GameObject> oldPlots = new List<GameObject>();

    public SVGTextImporter textImporter;

    public Material unlitMat;

    public Material textMat;

    public Material bgMat;

    public List<Texture2D> paperTextures;

    public Renderer bgRenderer;

    public URLSaver urlsaver;

    public Transform svgParent;

    public RenderTexture renderCamTexture;

    public RenderTexture renderFilterCamTexture;

    public RenderTexture renderNormalCamTexture;

    public Camera renderCam;



    public Burster burster;



    private int simpleVerticleFillMod = 4;





    List<Color> plottedColors = new List<Color>();




    public PlotCombiner pc;

    public bool printRunTimes = false;

    public AnimCellManager animManager;









    [ContextMenu("SetRenderValues")]
    public void SetRenderValues(List<LineRenderer> lineRenderersToSet, Material matToCopy, Color colorToSet, bool resetLinePositions = false, bool resetMyAss = true)
    {
        Vector2 svgSize = docSettings.svgSize;

        svgParent.position = new Vector3(-(svgSize.x / 2f), -(svgSize.y / 2f), 0);

        Camera.main.orthographicSize = svgSize.y / 2f;

        bgRenderer.material = bgMat;

        bgRenderer.material.SetTexture("_MainTex", paperTextures[paperIndex]);
        bgRenderer.material.SetColor("_BaseColor", bgColor);

        Material lineMat = matToCopy;

        if (resetMyAss)
        {
            lineMat = new Material(matToCopy);
        }


        lineMat.SetColor("_BaseColor", colorToSet);

        foreach (LineRenderer lr in lineRenderersToSet)
        {
            lr.material = lineMat;

            if (resetLinePositions)
            {
                lr.widthMultiplier = lineWidth;

                lr.transform.localPosition = Vector3.zero;
            }
        }
    }


    [ContextMenu("DoRenderTextureShit")]
    public void DoRenderTextureShit()
    {
        Vector2 svgSize = docSettings.svgSize;
        float renderScale = docSettings.renderScale;
        Vector2 clippingSize = docSettings.clippingSize;
        bool useAnimationSVGOffset = docSettings.useAnimationSVGOffset;
        bool generateArtWithNewSeed = docSettings.generateArtWithNewSeed;
        List<Color> pensToUse = docSettings.pensToUse;


        listOfAllPathsThisRun = new List<List<List<Vector2>>>();

        if (generateArtWithNewSeed)
        {
            seedValue = UnityEngine.Random.Range(-(int.MaxValue / 2), int.MaxValue / 2);
        }

        UnityEngine.Random.InitState(seedValue);

        piecename = textImporter.GenerateRandomName();

        yourFileName = piecename.Replace(" ", "");


        if (useAnimationSVGOffset)
        {
            renderCamTexture.Release();
            renderCamTexture.width = (int)((svgSize.x - (96 / 1.5f)) * renderScale);
            renderCamTexture.height = (int)((svgSize.y - (96 / 1.75f)) * renderScale);
            renderCamTexture.Create();


            renderFilterCamTexture.Release();
            renderFilterCamTexture.width = (int)((svgSize.x - (96 / 1.5f)) * renderScale);
            renderFilterCamTexture.height = (int)((svgSize.y - (96 / 1.75f)) * renderScale);
            renderFilterCamTexture.Create();


            renderNormalCamTexture.Release();
            renderNormalCamTexture.width = (int)((svgSize.x - (96 / 1.5f)) * (renderScale));
            renderNormalCamTexture.height = (int)((svgSize.y - (96 / 1.75f)) * (renderScale));
            renderNormalCamTexture.Create();
        }
        else
        {
            renderCamTexture.Release();
            renderCamTexture.width = (int)((svgSize.x - (clippingSize.x * 2f)) * renderScale);
            renderCamTexture.height = (int)((svgSize.y - (clippingSize.y * 2f)) * renderScale);
            renderCamTexture.Create();


            renderFilterCamTexture.Release();
            renderFilterCamTexture.width = (int)((svgSize.x - (clippingSize.x * 2f)) * renderScale);
            renderFilterCamTexture.height = (int)((svgSize.y - (clippingSize.y * 2f)) * renderScale);
            renderFilterCamTexture.Create();


            renderNormalCamTexture.Release();
            renderNormalCamTexture.width = (int)((svgSize.x - (clippingSize.x * 2f)) * (renderScale));
            renderNormalCamTexture.height = (int)((svgSize.y - (clippingSize.y * 2f)) * (renderScale));
            renderNormalCamTexture.Create();
        }


        SetRenderValues(lineObjects, unlitMat, Color.black, true);
    }




    [ContextMenu("GenerateRTWork")]
    public void GenerateRTWork()
    {
        Vector2 svgSize = docSettings.svgSize;
        float renderScale = docSettings.renderScale;
        Vector2 clippingSize = docSettings.clippingSize;
        bool useAnimationSVGOffset = docSettings.useAnimationSVGOffset;
        bool generateArtWithNewSeed = docSettings.generateArtWithNewSeed;
        List<Color> pensToUse = docSettings.pensToUse;

        float startTime = Time.time;

        DateTime before = DateTime.Now;

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();



        listOfAllPathsThisRun = new List<List<List<Vector2>>>();

        if (generateArtWithNewSeed)
        {
            seedValue = UnityEngine.Random.Range(-(int.MaxValue / 2), int.MaxValue / 2);
        }

        UnityEngine.Random.InitState(seedValue);

        piecename = textImporter.GenerateRandomName();

        yourFileName = piecename.Replace(" ", "");


        lineObjects = new List<LineRenderer>();

        listOfPaths = new List<List<Vector2>>();

        //get paint
        List<Vector2> paintSpot = new List<Vector2>();

        //bottom
        paintSpot.Add(new Vector3(svgSize.x - 1, 1, 0));
        paintSpot.Add(new Vector3(svgSize.x - 1, 3, 0));
        paintSpot.Add(new Vector3(svgSize.x - 1, 1, 0));
        paintSpot.Add(new Vector3(svgSize.x - 1, 3, 0));




        foreach (LineRenderer line in lineObjects)
        {
            if (line)
            {
                line.SetPositions(new Vector3[0]);
            }
        }




        List<List<List<Vector2>>> finalCompleteLineListByColor = new List<List<List<Vector2>>>();


        List<List<Vector2>> outlineList = new List<List<Vector2>>();

        //for (int z = 0; z < 1; z++)
        for (int z = 0; z < animManager.animCells.Count; z++)
        {
            UnityEngine.Random.InitState(seedValue);




            Texture2D wholeRenderTexHolder = new Texture2D(animManager.animCells[z].cellCam.targetTexture.width, animManager.animCells[z].cellCam.targetTexture.height, TextureFormat.RGBA32, false, true);


            RenderTexture.active = animManager.animCells[z].cellCam.targetTexture;

            wholeRenderTexHolder.ReadPixels(new Rect(0, 0, animManager.animCells[z].cellCam.targetTexture.width, animManager.animCells[z].cellCam.targetTexture.height), 0, 0);
            wholeRenderTexHolder.Apply(); // Apply the changes to the Texture2D


            RenderTexture.active = null;






            Texture2D wholeNormalRenderTexHolder = new Texture2D(animManager.animCells[z].cellNormalCam.targetTexture.width, animManager.animCells[z].cellNormalCam.targetTexture.height, TextureFormat.RGBA32, false, true);


            RenderTexture.active = animManager.animCells[z].cellNormalCam.targetTexture;

            wholeNormalRenderTexHolder.ReadPixels(new Rect(0, 0, animManager.animCells[z].cellNormalCam.targetTexture.width, animManager.animCells[z].cellNormalCam.targetTexture.height), 0, 0);
            wholeNormalRenderTexHolder.Apply(); // Apply the changes to the Texture2D


            RenderTexture.active = null;




            int xCell = z % (int)animManager.animationCellsXY.x;
            int yCell = Mathf.FloorToInt(z / animManager.animationCellsXY.x);
            Vector2 cellOffset = new Vector2(xCell * (wholeRenderTexHolder.width * (1f / renderScale)), yCell * (wholeRenderTexHolder.height * (1f / renderScale)));




            Color[] pixelArray = wholeRenderTexHolder.GetPixels();

            List<Color> diffColors = new List<Color>();




            diffColors = pixelArray.Distinct().ToList();


            List<List<Vector2>> claimedPixels = new List<List<Vector2>>();


            for (int i = 0; i < diffColors.Count; i++)
            {
                Color thisColor = diffColors[i];
                claimedPixels.Add(new List<Vector2>());




                List<Vector2> listlistlist = burster.HandleJob(pixelArray, thisColor, wholeRenderTexHolder.width);

                claimedPixels[i] = listlistlist;
            }


            Debug.Log(diffColors.Count);

            int colorCountToRemove = 0;

            for (int i = 0; i < diffColors.Count; i++)
            {
                Vector3 thisColorVector = new Vector3(diffColors[i].r, diffColors[i].g, diffColors[i].b);

                bool hasBeenReplaced = false;

                for (int k = 0; k < diffColors.Count; k++)
                {
                    if (claimedPixels[k].Count > 10000 * renderScale)
                    {
                        Vector3 replacementColor = new Vector3(diffColors[k].r, diffColors[k].g, diffColors[k].b);


                        if (Vector3.Distance(thisColorVector, replacementColor) < 0.0003f && thisColorVector != replacementColor && hasBeenReplaced == false)
                        if (Vector3.Distance(thisColorVector, replacementColor) < 0.0003f && thisColorVector != replacementColor && hasBeenReplaced == false)
                        {
                            hasBeenReplaced = true;
                            colorCountToRemove++;

                            claimedPixels[k].AddRange(claimedPixels[i]);

                            foreach (Vector2 v2 in claimedPixels[i])
                            {
                                wholeRenderTexHolder.SetPixel((int)v2.x, (int)v2.y, diffColors[k]);

                            }


                            claimedPixels[i] = new List<Vector2>();
                        }
                    }
                }
            }

            Debug.Log("Removed " + colorCountToRemove);

            for (int i = 0; i < claimedPixels.Count; i++)
            {
                if (claimedPixels[i] == new List<Vector2>())
                {
                    claimedPixels.Remove(claimedPixels[i]);
                }
            }

            if (printRunTimes)
            {
                stopwatch.Stop();
                Debug.Log("Cleaned Image Starting To Calculate Lines! TIME = " + stopwatch.Elapsed);
                stopwatch.Start();
            }


            //Draw bounds around Colors based on claimed pixels

            List<List<Vector2>> edgePointsPerColor = new List<List<Vector2>>();


            List<List<Vector2>> simpleVertFillLinesByColor = new List<List<Vector2>>();


            for (int i = 0; i < claimedPixels.Count; i++)
            {
                edgePointsPerColor.Add(new List<Vector2>());


                if (claimedPixels[i].Count > 0)
                {
                    //Line Segments
                    for (int q = 0; q < claimedPixels[i].Count; q++)
                    {
                        if (diffColors[i] != pixelArray[((int)claimedPixels[i][q].x + (((int)claimedPixels[i][q].y * wholeRenderTexHolder.width)) + 1) % (pixelArray.Length - 1)])
                        {
                            edgePointsPerColor[i].Add(claimedPixels[i][q]);

                        }

                        else if (diffColors[i] != pixelArray[Mathf.Abs(((int)claimedPixels[i][q].x + (((int)claimedPixels[i][q].y * wholeRenderTexHolder.width)) - 1)) % (pixelArray.Length - 1)])
                        {
                            edgePointsPerColor[i].Add(claimedPixels[i][q]);

                        }

                        else if (diffColors[i] != pixelArray[((int)claimedPixels[i][q].x + ((((int)claimedPixels[i][q].y + 1) * wholeRenderTexHolder.width))) % (pixelArray.Length - 1)])
                        {
                            edgePointsPerColor[i].Add(claimedPixels[i][q]);
                        }

                        else if (diffColors[i] != pixelArray[Mathf.Abs(((int)claimedPixels[i][q].x + ((((int)claimedPixels[i][q].y - 1) * wholeRenderTexHolder.width)))) % (pixelArray.Length - 1)])
                        {
                            edgePointsPerColor[i].Add(claimedPixels[i][q]);
                        }
                    }
                }
            }


            if (printRunTimes)
            {
                stopwatch.Stop();
                Debug.Log("Found Edges! TIME = " + stopwatch.Elapsed);
                stopwatch.Start();
            }





            List<List<Vector2>> outlinesToAddSplitUpColor = new List<List<Vector2>>();

            for (int i = 0; i < edgePointsPerColor.Count; i++)
            {
                outlinesToAddSplitUpColor.AddRange(FindConcentrationsOptimized(edgePointsPerColor[i], 1.9f * renderScale));

            }



            if (printRunTimes)
            {
                stopwatch.Stop();
                Debug.Log("Found Regions! TIME = " + stopwatch.Elapsed);
                stopwatch.Start();
            }




            List<List<Vector2>> outlinesToAddSplitUpColorAndOutlineFix = new List<List<Vector2>>();

            List<List<Vector2>> fillLineList = new List<List<Vector2>>();



            if (lineSettings.drawColorBoundsOutlines == true)
            {

                for (int i = 0; i < outlinesToAddSplitUpColor.Count; i++)
                {
                    outlinesToAddSplitUpColor[i] = SortClockwise(outlinesToAddSplitUpColor[i]);


                    List<Vector2> sortedList = SortPoints(outlinesToAddSplitUpColor[i]);


                    List<List<Vector2>> sortedSplitList = new List<List<Vector2>>();


                    List<Vector2> tempPointList = new List<Vector2>();


                    for (int v = 0; v < sortedList.Count - 1; v++)
                    {
                        tempPointList.Add(sortedList[v]);

                        //If this one and the next one are far apart split that boy
                        if (Vector2.Distance(sortedList[v], sortedList[v + 1]) > 2)
                        {
                            if (Vector2.Distance(tempPointList[tempPointList.Count - 1], tempPointList[0]) < 2)
                            {
                                tempPointList.Add(tempPointList[0]);
                            }


                            sortedSplitList.Add(tempPointList);

                            tempPointList = new List<Vector2>();
                        }
                    }

                    if (tempPointList.Count > 1)
                    {
                        if (Vector2.Distance(tempPointList[tempPointList.Count - 1], tempPointList[0]) < 2)
                        {
                            tempPointList.Add(tempPointList[0]);
                        }
                    }

                    sortedSplitList.Add(tempPointList);

                    foreach (List<Vector2> lv2 in sortedSplitList)
                    {
                        outlinesToAddSplitUpColorAndOutlineFix.Add(lv2);
                    }
                }
            }
            








            if (printRunTimes)
            {
                stopwatch.Stop();
                Debug.Log("Edge Lines Calculated! TIME = " + stopwatch.Elapsed);
                stopwatch.Start();
            }



            List<List<Vector2>> finalLineList = new List<List<Vector2>>();

            List<List<List<Vector2>>> finalLineListByColor = new List<List<List<Vector2>>>();




            for (int q = 0; q < diffColors.Count; q++)
            {

                finalLineListByColor.Add(new List<List<Vector2>>());
            }




            //Here we spawn in fill lines

            int fillLineCount = squiggleLineSettings.squiggleLineFillCount;

            int colorAttempts = 0;

            for (int i = 0; i < fillLineCount; i++)
            {
                int lineSegmentMaxCount = (int)UnityEngine.Random.Range(squiggleLineSettings.lineSegCountMinMax.x, squiggleLineSettings.lineSegCountMinMax.y);

                List<Vector2> thisFillLine = new List<Vector2>();


                Vector2 additionalOffset = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * 0.4f;

                if (squiggleLineSettings.squiggleUseRandomDir == false)
                {
                    additionalOffset = squiggleLineSettings.squiggleLineDir.normalized * 0.4f;
}


                //Get a random position, if that position has no color on normal map find a different one
                Vector2 lineStartPos = new Vector2(UnityEngine.Random.Range(0, wholeRenderTexHolder.width), UnityEngine.Random.Range(0, wholeRenderTexHolder.height));

                Color startingColor = wholeRenderTexHolder.GetPixel((int)lineStartPos.x, (int)lineStartPos.y);

                Color startingNormalColor = wholeNormalRenderTexHolder.GetPixel((int)lineStartPos.x, (int)lineStartPos.y);

                while (startingNormalColor.maxColorComponent < 0.01f)
                {
                    lineStartPos = new Vector2(UnityEngine.Random.Range(0, wholeRenderTexHolder.width), UnityEngine.Random.Range(0, wholeRenderTexHolder.height));

                    startingNormalColor = wholeNormalRenderTexHolder.GetPixel((int)lineStartPos.x, (int)lineStartPos.y);

                    colorAttempts++;

                    if (colorAttempts > 2000)
                    {
                        startingNormalColor = Color.cyan;
                    }
                }

                float h = 0;

                float s = 0;

                float v = 0;

                Color.RGBToHSV(wholeRenderTexHolder.GetPixel((int)lineStartPos.x, (int)lineStartPos.y), out h, out s, out v);

                bool cancelThis = false;

                if (v >= UnityEngine.Random.Range(0f, 1f) && squiggleLineSettings.useValueAsSquiggleProb == true)
                {
                    cancelThis = true;
                }

                if (cancelThis == false)
                {
                    Vector2 curLineWritingPos = lineStartPos;

                    //Advance based on normal map colors, if that point is a different color in the color map, end this line
                    for (int q = 0; q < lineSegmentMaxCount; q++)
                    {
                        startingNormalColor = wholeNormalRenderTexHolder.GetPixel((int)curLineWritingPos.x, (int)curLineWritingPos.y);

                        if (squiggleLineSettings.squiggleLinesUseSine == true)
                        {
                            additionalOffset += new Vector2(Mathf.Sin(q / (3f * squiggleLineSettings.squiggleSinDir.x)) / 5f, Mathf.Sin(q / (3f * squiggleLineSettings.squiggleSinDir.y)) / 5f);

                        }


                        //If this pixel is the same color
                        if (wholeRenderTexHolder.GetPixel((int)curLineWritingPos.x, (int)curLineWritingPos.y) != startingColor || startingNormalColor.maxColorComponent < 0.01f || curLineWritingPos.x > wholeNormalRenderTexHolder.width || curLineWritingPos.y > wholeNormalRenderTexHolder.height || curLineWritingPos.x < 0 || curLineWritingPos.y < 0)
                        {
                            if (q == 0)
                            {
                                i--;
                            }


                            q = lineSegmentMaxCount;
                        }
                        else
                        {
                            thisFillLine.Add(curLineWritingPos);

                            curLineWritingPos += (new Vector2(startingNormalColor.r - 0.5f, startingNormalColor.g - 0.5f) + additionalOffset).normalized * 3f * (renderScale / 2f) * UnityEngine.Random.Range(0.5f, 1.5f);
                        }


                    }

                    if (thisFillLine.Count > 1)
                    {
                        fillLineList.Add(thisFillLine);
                    }

                }
            }












            



             int characterFillLetterCount = textFillSettings.characterFillLetterCount;

          float valueTextScale = textFillSettings.valueTextScale;

           bool useValueToScaleChars = textFillSettings.useValueToScaleChars;

           bool charsUsePsudoRandomScale = textFillSettings.charsUsePsudoRandomScale;

           bool useValueAsChanceforChars = textFillSettings.useValueAsChanceforChars;

          float minCharScale = textFillSettings.minCharScale;



           //I want to spawn letters to fill in an image
           int fillLineLetterCount = characterFillLetterCount;

            for (int i = 0; i < fillLineLetterCount; i++)
            {
                //Get a random position
                Vector2 lineStartPos = new Vector2(UnityEngine.Random.Range(0, wholeRenderTexHolder.width), UnityEngine.Random.Range(0, wholeRenderTexHolder.height));

                Color startingColor = wholeRenderTexHolder.GetPixel((int)lineStartPos.x, (int)lineStartPos.y);

                float scH = 0;
                float scS = 0;
                float scV = 0;

                Color.RGBToHSV(startingColor, out scH, out scS, out scV);


                float randomVal = UnityEngine.Random.Range(0f, 1f);

                if (scV < randomVal && useValueAsChanceforChars == true)
                {
                    i--;
                }
                else
                {
                    textImporter.DisplayAlphabet();

                    //-23 to remove special chars and numbers
                    //-10 for most of them
                    //Do this better
                    List<List<Vector2>> letterLineList = textImporter.cachedAlphabet[UnityEngine.Random.Range(0, textImporter.cachedAlphabet.Count - 23)];

                    float valDiff = randomVal - scV;

                    float angle = UnityEngine.Random.Range(0f, 360f);



                    if (useValueToScaleChars == false)
                    {
                        valDiff = UnityEngine.Random.Range(minCharScale, 1f);
                    }

                    if(charsUsePsudoRandomScale == false)
                    {
                        valDiff = 1;
                    }


                    foreach (List<Vector2> lines in letterLineList)
                    {
                        List<Vector2> fakeLine = lines;

                        for (int q = 0; q < fakeLine.Count; q++)
                        {
                            fakeLine[q] -= new Vector2(0, 7f);
                      
                            fakeLine[q] = new Vector2(fakeLine[q].x * Mathf.Cos(angle) - fakeLine[q].y * Mathf.Sin(angle), fakeLine[q].x * Mathf.Sin(angle) + fakeLine[q] .y * Mathf.Cos(angle));


                            //    x' = x * cos ? - y * sin ?
                            //    y' = x * sin ? + y * cos ?

                            fakeLine[q] *= valueTextScale;


                            fakeLine[q] *= Mathf.Clamp(valDiff, minCharScale, 1f);
                            


                            fakeLine[q] += lineStartPos;
                        }


                        fillLineList.Add(fakeLine);
                    }
                }
            }











            


            //If I want to spam in simple Shapes





            for (int i = 0; i < shapeSpamSettings.totalShapeCount; i++)
            {
                //Get a random position
                Vector2 lineStartPos = new Vector2(UnityEngine.Random.Range(0, wholeRenderTexHolder.width), UnityEngine.Random.Range(0, wholeRenderTexHolder.height));

                Color startingColor = wholeRenderTexHolder.GetPixel((int)lineStartPos.x, (int)lineStartPos.y);

                float scH = 0;
                float scS = 0;
                float scV = 0;

                Color.RGBToHSV(startingColor, out scH, out scS, out scV);


                float randomVal = UnityEngine.Random.Range(0f, 1f);

                if (scV < randomVal && shapeSpamSettings.useValueAsChanceforShapes == true)
                {
                    i--;
                }
                else
                {
                    float valDiff = randomVal - scV;

                    float angle = 0;


                    if(shapeSpamSettings.useRandomRotation == true)
                    {
                        angle = UnityEngine.Random.Range(0f, 360f);
                    }

                    if (shapeSpamSettings.useRotationFromList == true)
                    {
                        if (shapeSpamSettings.possibleRotations.Count > 0)
                        {
                            angle = shapeSpamSettings.possibleRotations[UnityEngine.Random.Range(0, shapeSpamSettings.possibleRotations.Count)];
                        }
                        
                    }










                    if (shapeSpamSettings.useValueToScaleShapes == false)
                    {
                        valDiff = UnityEngine.Random.Range(shapeSpamSettings.minShapeScale, 1f);
                    }

                    if (shapeSpamSettings.shapesUsePsudoRandomScale == false)
                    {
                        valDiff = 1;
                    }


                    int shapeVertCount = shapeSpamSettings.possibleShapeVertCounts[UnityEngine.Random.Range(0, shapeSpamSettings.possibleShapeVertCounts.Count)];

                    List<Vector2> newShapeList = FindPointsOnSimpleShape(shapeVertCount, shapeVertCount, svgSize / 2, valDiff * shapeSpamSettings.shapeScale, angle * Mathf.Deg2Rad);



                    for (int q = 0; q < newShapeList.Count; q++)
                    {
                        newShapeList[q] += lineStartPos - (svgSize / 2);
                    }

                    newShapeList.Add(newShapeList[0]);


                    fillLineList.Add(newShapeList);
                }
            }


















            //var FAKEoutlineList = finalLineListByColor;


            for (int i = 0; i < outlinesToAddSplitUpColorAndOutlineFix.Count; i++)
            {
                List<Vector2> scaledPath = new List<Vector2>();



                for (int u = 0; u < outlinesToAddSplitUpColorAndOutlineFix[i].Count; u++)
                {
                    Vector2 posToAdd = (outlinesToAddSplitUpColorAndOutlineFix[i][u] * (1f / renderScale));

                    posToAdd = new Vector2(posToAdd.x + (96 / 1.5f) - 5, 5 + (animManager.animCells[z].cellCam.targetTexture.height * (1 /renderScale)) - posToAdd.y);

                    posToAdd += cellOffset;

                    scaledPath.Add(posToAdd);
                }

                //finalLineList.Add(scaledPath);


                if (outlinesToAddSplitUpColorAndOutlineFix[i].Count > 0)
                {
                    Color lineColor = wholeRenderTexHolder.GetPixel((int)outlinesToAddSplitUpColorAndOutlineFix[i][0].x, (int)outlinesToAddSplitUpColorAndOutlineFix[i][0].y);

                    for (int q = 0; q < diffColors.Count; q++)
                    {
                        if (diffColors[q] == lineColor)
                        {
                            if (scaledPath.Count > 1)
                            {
                                float lineDist = 0;

                                for (int c = 0; c < scaledPath.Count - 1; c++)
                                {
                                    lineDist += Vector3.Distance(scaledPath[c], scaledPath[c + 1]);
                                }

                                if (lineDist > 3)
                                {
                                    //outlineList.Add(SimplifyPath(scaledPath));
                                    outlineList.Add(scaledPath);
                                }


                            }
                        }
                    }
                }
            }


            //outlineList.AddRange(outlinesToAddSplitUpColorAndOutlineFix);




            //fillLineList = PathCrossingRemover.RemoveCrossingPaths(fillLineList, 5);


            outlinesToAddSplitUpColorAndOutlineFix.AddRange(fillLineList);




            bool hasBGFilled = false;


            if (fillSettings.useSimpleFillForRender == true)
            {

                //Starts at 1 to avoid filling in "empty area", make 0 if you want to fill all colors
                //fill each color
                for (int q = 1; q < claimedPixels.Count; q++)
                {
                    if (fillSettings.fillBGColor && hasBGFilled == false)
                    {
                        q = 0;

                        hasBGFilled = true;
                    }


                    List<List<Vector2>> FUICK = GroupVerticalNeighbors(claimedPixels[q]);

                    List<List<Vector2>> FUICKCleaned = new List<List<Vector2>>();

                    for (int j = 0; j < FUICK.Count; j++)
                    {
                        List<Vector2> tempLineList = new List<Vector2>();

                        if (fillSettings.useSinWiggle == true)
                        {
                            List<List<Vector2>> listOfLinesThisLine = new List<List<Vector2>>();


                            //Make fill Lines sqiggle
                            for (int h = 0; h < FUICK[j].Count; h++)
                            {


                                Vector2 newPos = FUICK[j][h] + new Vector2(Mathf.Sin(FUICK[j][h].y / fillSettings.ySinDivider) * fillSettings.ySinXMulti, Mathf.Sin(FUICK[j][h].x / fillSettings.xSinDivider) * fillSettings.xSinYMulti);

                                if (h == 0)
                                {
                                    if (lineSettings.ignoreFirstLinePointForWiggle == true)
                                    {
                                        newPos = FUICK[j][h];
                                    }
                                }

                                //if I go OOOOB
                                if (newPos.x / wholeRenderTexHolder.width > 1 || newPos.y / wholeRenderTexHolder.height > 1 || newPos.x < 0 || newPos.y < 0)
                                {

                                }



                                tempLineList.Add(newPos);

                            }
                        }





                        FUICKCleaned.Add(SimplifyPath(tempLineList));
                    }

                    simpleVertFillLinesByColor.AddRange(FUICKCleaned);
                }


                outlinesToAddSplitUpColorAndOutlineFix.AddRange(simpleVertFillLinesByColor);
            }




            for (int q = 0; q < outlinesToAddSplitUpColorAndOutlineFix.Count; q++)
            {
                //outlinesToAddSplitUpColorAndOutlineFix[q] = SimplifyPath(outlinesToAddSplitUpColorAndOutlineFix[q]);
                outlinesToAddSplitUpColorAndOutlineFix[q] = outlinesToAddSplitUpColorAndOutlineFix[q];
            }




            for (int i = 0; i < outlinesToAddSplitUpColorAndOutlineFix.Count; i++)
            {
                List<Vector2> scaledPath = new List<Vector2>();


                for (int u = 0; u < outlinesToAddSplitUpColorAndOutlineFix[i].Count; u++)
                {
                    scaledPath.Add(outlinesToAddSplitUpColorAndOutlineFix[i][u] * (1f / renderScale));
                }

                finalLineList.Add(scaledPath);


                if (outlinesToAddSplitUpColorAndOutlineFix[i].Count > 0)
                {
                    Color lineColor = wholeRenderTexHolder.GetPixel((int)outlinesToAddSplitUpColorAndOutlineFix[i][0].x, (int)outlinesToAddSplitUpColorAndOutlineFix[i][0].y);

                    for (int q = 0; q < diffColors.Count; q++)
                    {
                        if (diffColors[q] == lineColor)
                        {
                            if (scaledPath.Count > 1)
                            {
                                finalLineListByColor[q].Add(scaledPath);
                            }
                        }
                    }
                }
            }










            //asign pen based on render color
            if (pensToUse.Count > 0)
            {
                //use pen colors
                List<List<List<Vector2>>> claimedPixelsByPen = new List<List<List<Vector2>>>();


                for (int u = 0; u < pensToUse.Count; u++)
                {
                    claimedPixelsByPen.Add(new List<List<Vector2>>());
                }



                for (int q = 0; q < diffColors.Count - colorCountToRemove; q++)
                {
                    //go thru all colors in the image, find the pen color that is closest and use that one
                    int penIndex = 0;
                    float colorDistance = 1000000000000000;

                    for (int u = 0; u < pensToUse.Count; u++)
                    {
                        //float thisColorDist = Vector3.Distance(new Vector3(pensToUse[u].r, pensToUse[u].g, pensToUse[u].b), new Vector3(diffColors[q].r, diffColors[q].g, diffColors[q].b));


                        float penH, penS, penV;

                        Color.RGBToHSV(pensToUse[u], out penH, out penS, out penV);


                        float baseH, baseS, baseV;

                        Color.RGBToHSV(diffColors[q], out baseH, out baseS, out baseV);



                        //float thisColorDist = (Mathf.Abs(penH - baseH) * 2.2f) + (Mathf.Abs(penV - baseV) * 2f);

                        float thisColorDist = Vector3.Distance(new Vector3(penH * 2f, penS *.5f, penV), new Vector3(baseH * 2f, baseS *0.5f, baseV));

                        if (thisColorDist < colorDistance)
                        {
                            penIndex = u;
                            colorDistance = thisColorDist;
                        }
                    }

                    claimedPixelsByPen[penIndex].AddRange(finalLineListByColor[q]);

                }

                finalLineListByColor = claimedPixelsByPen;
            }






            if (printRunTimes)
            {
                stopwatch.Stop();
                Debug.Log("Start Drawing Lines! TIME = " + stopwatch.Elapsed);
                stopwatch.Start();
            }




            //for (int q = 0; q < finalLineListByColor.Count; q++)
            //{
            //    //finalLineListByColor[q] = PathCombiner.CombineNearbyLines(finalLineListByColor[q], 2f);
            //}












            if (drawBounds == true)
            {
                //place boarder
                List<Vector2> boarderPoints = new List<Vector2>();

                //bottom
                boarderPoints.Add(new Vector3(0, 0, 0));
                boarderPoints.Add(new Vector3(wholeRenderTexHolder.width * (1f / renderScale), 0, 0));
                boarderPoints.Add(new Vector3(wholeRenderTexHolder.width * (1f / renderScale), wholeRenderTexHolder.height * (1f / renderScale), 0));
                boarderPoints.Add(new Vector3(0, wholeRenderTexHolder.height * (1f / renderScale), 0));
                boarderPoints.Add(new Vector3(0, 0, 0));

                PlacePath(lineWidth, boarderPoints, 0, transform, new Color(10, 10, 10), true);

                SetRenderValues(lineObjects, unlitMat, Color.white, false, false);

                GameObject holder = Instantiate(new GameObject(), transform);

                foreach (LineRenderer lr in lineObjects)
                {
                    lr.transform.SetParent(holder.transform);
                }


                finalLineListByColor[0].Add(boarderPoints);
            }








            //Offset based on cell 4x 3y  using z - animationCell index





            //Offset The lines in the SVG file
            for (int i = 0; i < finalLineListByColor.Count; i++)
            {
                for (int u = 0; u < finalLineListByColor[i].Count; u++)
                {
                    for (int g = 0; g < finalLineListByColor[i][u].Count; g++)
                    {
                        if (finalLineListByColor[i][u][g].x > wholeRenderTexHolder.width / renderScale)
                        {
                            finalLineListByColor[i][u][g] = new Vector2(wholeRenderTexHolder.width / renderScale, finalLineListByColor[i][u][g].y);
                        }

                        if (finalLineListByColor[i][u][g].x < 0)
                        {
                            finalLineListByColor[i][u][g] = new Vector2(0, finalLineListByColor[i][u][g].y);
                        }



                        if (finalLineListByColor[i][u][g].y > wholeRenderTexHolder.height / renderScale)
                        {
                            finalLineListByColor[i][u][g] = new Vector2(finalLineListByColor[i][u][g].x, wholeRenderTexHolder.height / renderScale);
                        }

                        if (finalLineListByColor[i][u][g].y < 0)
                        {
                            finalLineListByColor[i][u][g] = new Vector2(finalLineListByColor[i][u][g].x, 0);
                        }




                        finalLineListByColor[i][u][g] = new Vector2(finalLineListByColor[i][u][g].x, (wholeRenderTexHolder.height / renderScale) - finalLineListByColor[i][u][g].y);

                        if (useAnimationSVGOffset)
                        {
                            finalLineListByColor[i][u][g] -= new Vector2(-(96 / 1.5f) + 5, -5);
                        }
                        else
                        {
                            finalLineListByColor[i][u][g] += new Vector2(clippingSize.x, clippingSize.y);
                        }







                        finalLineListByColor[i][u][g] += cellOffset;
                    }
                }

                finalCompleteLineListByColor.Add(new List<List<Vector2>>());

                finalCompleteLineListByColor[i].AddRange(finalLineListByColor[i]);
            }






            if (lineSettings.combineLineDist > 0)
            {
                var finalCompleteLineListByColorCopy = finalCompleteLineListByColor;

                List<List<List<Vector2>>> realFinalLineList = new List<List<List<Vector2>>>();


                bool giveUp = false;


                //Better way to reduce lines
                for (int i = 0; i < finalCompleteLineListByColorCopy.Count; i++)
                {
                    realFinalLineList.Add(new List<List<Vector2>>());

                    if (finalCompleteLineListByColorCopy[i].Count > 0)
                    {
                        int linesAdded = 1;

                        List<Vector2> curCombineLine = finalCompleteLineListByColorCopy[i][0];

                        finalCompleteLineListByColorCopy[i].Remove(finalCompleteLineListByColorCopy[i][0]);

                        List<int> usedLineIndecies = new List<int>();

                        usedLineIndecies.Add(0);

                        //While I have not added all line content to combined line list
                        while (finalCompleteLineListByColorCopy[i].Count > 0)
                        {
                            bool lineCanCombine = false;

                            List<Vector2> curClosestList = finalCompleteLineListByColorCopy[i][0];

                            float distToNextLineSeg = Vector2.Distance(curCombineLine.Last(), curClosestList[0]);

                            int lineCombIndex = 0;

                            bool addLineReversed = false;

                            for (int q = 0; q < finalCompleteLineListByColorCopy[i].Count; q++)
                            {
                                float thisSegDistToCurLine = Vector2.Distance(curCombineLine.Last(), finalCompleteLineListByColorCopy[i][q][0]);

                                if (thisSegDistToCurLine < distToNextLineSeg)
                                {
                                    curClosestList = finalCompleteLineListByColorCopy[i][q];

                                    lineCombIndex = q;

                                    distToNextLineSeg = thisSegDistToCurLine;

                                    addLineReversed = false;
                                }
                            }


                            for (int q = 0; q < finalCompleteLineListByColorCopy[i].Count; q++)
                            {
                                float thisSegDistToCurLine = Vector2.Distance(curCombineLine[0], finalCompleteLineListByColorCopy[i][q].Last());

                                if (thisSegDistToCurLine < distToNextLineSeg)
                                {
                                    curClosestList = finalCompleteLineListByColorCopy[i][q];

                                    //curClosestList.Reverse();

                                    lineCombIndex = q;

                                    distToNextLineSeg = thisSegDistToCurLine;

                                    addLineReversed = true;
                                }
                            }





                            if (curCombineLine.Count > 0 && curClosestList.Count > 0)
                            {

                                if (distToNextLineSeg < lineSettings.combineLineDist)
                                {
                                    if (addLineReversed == false)
                                    {
                                        lineCanCombine = true;

                                        usedLineIndecies.Add(lineCombIndex);

                                        curCombineLine.AddRange(curClosestList);

                                        finalCompleteLineListByColorCopy[i].Remove(finalCompleteLineListByColorCopy[i][lineCombIndex]);

                                        linesAdded++;
                                    }
                                    else
                                    {
                                        lineCanCombine = true;

                                        usedLineIndecies.Add(lineCombIndex);

                                        curCombineLine.InsertRange(0, curClosestList);

                                        finalCompleteLineListByColorCopy[i].Remove(finalCompleteLineListByColorCopy[i][lineCombIndex]);

                                        linesAdded++;
                                    }



                                }
                            }





                            if (lineCanCombine == false)
                            {
                                realFinalLineList[i].Add(curCombineLine);

                                curCombineLine = finalCompleteLineListByColorCopy[i][0];

                                finalCompleteLineListByColorCopy[i].Remove(finalCompleteLineListByColorCopy[i][0]);
                            }


                            stopwatch.Stop();
                            if (stopwatch.Elapsed.TotalMinutes > 5)
                            {
                                giveUp = true;
                            }
                            stopwatch.Start();


                            if (giveUp == true)
                            {
                                break;
                            }
                        }

                        realFinalLineList[i].Add(curCombineLine);
                    }





                }

                finalCompleteLineListByColor = realFinalLineList;
            }












            if (finalCompleteLineListByColor.Count > 15000)
            {
                Debug.LogError("This is a lot of lines");
            }

            if (finalCompleteLineListByColor.Count > 10000 && lineSettings.combineLineDist == 0)
            {
                Debug.LogError("This is a lot of lines, and you are not even trying to merge them dummy!");
            }

            if (lineSettings.combineLineDist == 0)
            {
                Debug.LogError("You probably meant to merge lines dummy");
            }












            //Split lines if they have too many points. I think if they are too long they just don't load :(
            //But maybe I was wrong? Removed for now
            //List<List<List<Vector2>>> splitLines = new List<List<List<Vector2>>>();


            //foreach (List<List<Vector2>> listOfLines in finalCompleteLineListByColor)
            //{
            //    splitLines.Add(new List<List<Vector2>>());

            //    foreach (List<Vector2> listOfPoints in listOfLines)
            //    {
            //        if (listOfPoints.Count > 500)
            //        {
            //            Debug.Log("Long boy" + listOfPoints.Count);

            //            List<List<Vector2>> splitLongBoy = new List<List<Vector2>>();

            //            List<Vector2> runningSplitList = new List<Vector2>();

            //            for (int j = 0; j < listOfPoints.Count; j++)
            //            {
            //                if (j % 100 == 0)
            //                {
            //                    if (runningSplitList != new List<Vector2>())
            //                    {
            //                        splitLongBoy.Add(runningSplitList);
            //                    }


            //                    runningSplitList = new List<Vector2>();
            //                }

            //                runningSplitList.Add(listOfPoints[j]);
            //            }


            //            splitLongBoy.Add(runningSplitList);


            //            foreach (List<Vector2> listOfPointsSPLIT in splitLongBoy)
            //            {
            //                splitLines.Last().Add(SimplifyPath(listOfPointsSPLIT));
            //            }
            //        }
            //        else
            //        {
            //            splitLines.Last().Add(SimplifyPath(listOfPoints));
            //        }
            //    }
            //}





            //finalCompleteLineListByColor = splitLines;




















            int lineCountCount = 0;

            for (int q = 0; q < finalCompleteLineListByColor.Count; q++)
            {
                for (int k = 0; k < finalCompleteLineListByColor[q].Count; k++)
                {
                    if (pensToUse.Count == 0)
                    {
                        PlacePath(lineWidth, finalCompleteLineListByColor[q][k], lineCountCount, transform, diffColors[q], false);
                    }
                    else
                    {
                        PlacePath(lineWidth, finalCompleteLineListByColor[q][k], lineCountCount, transform, pensToUse[q], false);
                    }


                    lineCountCount++;
                }
            }



            if (printRunTimes)
            {
                stopwatch.Stop();
                Debug.Log("Start SVG Saving! TIME = " + stopwatch.Elapsed);
                stopwatch.Start();
            }
        }



       






        if (pensToUse.Count > 0)
        {
            //GenerateSVG(outlineList, false, false,1000, Color.black, svgSize, yourFileName);
        }





        if (pensToUse.Count > 0)
        {
            //Debug.Log("shit " + finalCompleteLineListByColor.Count + " " + pensToUse.Count + " " + q);

            for (int q = 0; q < pensToUse.Count; q++)
            {
                Debug.Log("shit " + finalCompleteLineListByColor.Count + " " + pensToUse.Count + " " + q);

                //GenerateSVG(finalCompleteLineListByColor[q], false, false, q, pensToUse[q], svgSize);
                GenerateSVG(finalCompleteLineListByColor[q], false, false, q, Color.black, svgSize, yourFileName);
            }
        }
        else
        {
            //You have to preselect pen colors for now

            ////Claimed pixels = [Color][Position], we remove the cleaned colors from the claimedPixels but not anywhere else like diff colors 
            //for (int q = 0; q < diffColors.Count - colorCountToRemove; q++)
            //{
            //    GenerateSVG(finalCompleteLineListByColor[q], false, false, q, diffColors[q], svgSize);
            //}
        }





        //pc.SlamTogether(finalCompleteLineListByColor, yourFileName);



        stopwatch.Stop();
        Debug.Log("DONE! TIME = " + stopwatch.Elapsed);
        stopwatch.Start();


        stopwatch.Reset();
    }












    private List<Vector2> SimplifyPath(List<Vector2> path)
    {
        if (path.Count < 3)
        {
            return path;
        }


        List<Vector2> simplified = new List<Vector2> { path[0] };
        Vector2 a = path[0];
        Vector2 b = path[1];

        for (int i = 2; i < path.Count; i++)
        {
            Vector2 c = path[i];
            if (!AreCollinear(a, b, c))
            {
                simplified.Add(b);
                a = b;
            }
            b = c;
        }


        simplified.Add(b);
        return simplified;
        //return path;
    }

    private bool AreCollinear(Vector2 a, Vector2 b, Vector2 c)
    {
        float area = (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
        return Mathf.Abs(area) < 0.001f;
    }
















    public List<List<Vector2>> OrganizeIntoScanLinesWithBreaks(List<Vector2> points)
    {
        Vector2 svgSize = docSettings.svgSize;
        float renderScale = docSettings.renderScale;
        Vector2 clippingSize = docSettings.clippingSize;
        bool useAnimationSVGOffset = docSettings.useAnimationSVGOffset;
        bool generateArtWithNewSeed = docSettings.generateArtWithNewSeed;
        List<Color> pensToUse = docSettings.pensToUse;

        // First organize into standard scan lines (grouped by Y)
        Dictionary<float, List<Vector2>> scanLineDict = new Dictionary<float, List<Vector2>>();

        foreach (Vector2 point in points)
        {
            if (!scanLineDict.ContainsKey(point.y))
            {
                scanLineDict[point.y] = new List<Vector2>();
            }
            scanLineDict[point.y].Add(point);
        }

        // Process each scan line to split into continuous segments
        List<List<Vector2>> result = new List<List<Vector2>>();
        List<float> sortedYs = new List<float>(scanLineDict.Keys);
        sortedYs.Sort((a, b) => b.CompareTo(a)); // Top to bottom

        float xRenderScale = Mathf.Clamp(renderScale, 1, 4);

        foreach (float y in sortedYs)
        {
            List<Vector2> line = scanLineDict[y];
            line.Sort((a, b) => a.x.CompareTo(b.x)); // Left to right

            List<List<Vector2>> subLines = new List<List<Vector2>>();
            List<Vector2> currentSubLine = new List<Vector2>();

            for (int i = 0; i < line.Count; i++)
            {
                if (currentSubLine.Count == 0)
                {
                    currentSubLine.Add(line[i]);
                }
                else
                {
                    // Check if this point is adjacent to the previous point
                    float prevX = currentSubLine[currentSubLine.Count - 1].x;
                    if (line[i].x == prevX + 2)
                    {
                        currentSubLine.Add(line[i]);
                    }
                    else
                    {
                        // Gap detected, finalize current sub-line
                        subLines.Add(currentSubLine);

                        currentSubLine = new List<Vector2> { line[i] };
                    }
                }
            }

            simpleVerticleFillMod = (int)renderScale * fillSettings.simpleVerticleFillModMod;

            // Add the last sub-line
            if (currentSubLine.Count > 0)
            {
                if (currentSubLine[0].y % simpleVerticleFillMod == 0)
                {
                    subLines.Add(currentSubLine);
                }

            }

            result.AddRange(subLines);
        }

        return result;
    }












    public static class PathCrossingRemover
    {
        public static List<List<Vector2>> RemoveCrossingPaths(List<List<Vector2>> paths, int maxCrossings = 10)
        {
            List<List<Vector2>> filteredPaths = new List<List<Vector2>>(paths);
            List<List<Vector2>> pathsToRemove = new List<List<Vector2>>();

            for (int i = 0; i < filteredPaths.Count; i++)
            {
                for (int j = i + 1; j < filteredPaths.Count; j++)
                {
                    int crossingCount = CountCrossings(filteredPaths[i], filteredPaths[j]);

                    if (crossingCount > maxCrossings)
                    {
                        if (!pathsToRemove.Contains(filteredPaths[i]))
                        {
                            pathsToRemove.Add(filteredPaths[i]);
                        }
                        if (!pathsToRemove.Contains(filteredPaths[j]))
                        {
                            pathsToRemove.Add(filteredPaths[j]);
                        }
                    }
                }
            }

            foreach (var pathToRemove in pathsToRemove)
            {
                filteredPaths.Remove(pathToRemove);
                //Debug.Log("FUCK REMOVED");
            }

            return filteredPaths;
        }

        private static int CountCrossings(List<Vector2> path1, List<Vector2> path2, float tolerance = 5f)
        {
            int crossingCount = 0;
            HashSet<Vector2> crossedPoints = new HashSet<Vector2>();

            for (int i = 0; i < path1.Count; i++)
            {
                for (int j = 0; j < path2.Count; j++)
                {
                    if (Vector2.Distance(path1[i], path2[j]) <= tolerance)
                    {
                        // Check if we've already counted this crossing
                        bool alreadyCrossed = false;
                        foreach (var crossedPoint in crossedPoints)
                        {
                            if (Vector2.Distance(path1[i], crossedPoint) <= tolerance)
                            {
                                alreadyCrossed = true;
                                break;
                            }
                        }

                        if (!alreadyCrossed)
                        {
                            crossingCount++;
                            crossedPoints.Add(path1[i]);
                        }
                    }
                }
            }

            return crossingCount;
        }
    }







    public List<List<Vector2>> GroupVerticalNeighbors(List<Vector2> points)
    {
        Vector2 svgSize = docSettings.svgSize;
        float renderScale = docSettings.renderScale;
        Vector2 clippingSize = docSettings.clippingSize;
        bool useAnimationSVGOffset = docSettings.useAnimationSVGOffset;
        bool generateArtWithNewSeed = docSettings.generateArtWithNewSeed;
        List<Color> pensToUse = docSettings.pensToUse;

        simpleVerticleFillMod = (int)renderScale * fillSettings.simpleVerticleFillModMod;

        List<Vector2> pointsModded = new List<Vector2>();

        float xRenderScale = Mathf.Clamp(renderScale, 1, 4);

        for (int i = 0; i < points.Count; i++)
        {
            if (points[i].y % simpleVerticleFillMod == 0 && points[i].x % 2 == 0)
            {
                pointsModded.Add(points[i]);
            }

            //if (points[i].y % simpleVerticleFillMod == 0 && points[i].x % renderScale == 0)
            //{
            //    pointsModded.Add(points[i]);
            //}
        }


        // 1. Get initial scan line segments with X-gap splits
        List<List<Vector2>> segments = OrganizeIntoScanLinesWithBreaks(pointsModded);

        // 2. Create data structures for quick lookups
        Dictionary<float, List<SegmentData>> yMap = new Dictionary<float, List<SegmentData>>();
        List<SegmentData> allSegments = new List<SegmentData>();

        // Populate segment data
        for (int i = 0; i < segments.Count; i++)
        {
            List<Vector2> seg = segments[i];
            float y = seg[0].y;
            float minX = seg[0].x;
            float maxX = seg[^1].x;

            SegmentData data = new SegmentData(i, y, minX, maxX);
            allSegments.Add(data);

            if (!yMap.ContainsKey(y)) yMap[y] = new List<SegmentData>();
            yMap[y].Add(data);
        }




        // 3. Union-Find to connect vertical neighbors
        UnionFind uf = new UnionFind(segments.Count);

        foreach (SegmentData current in allSegments)
        {
            // Check Y+1 and Y-1
            foreach (int yOffset in new[] { -simpleVerticleFillMod, simpleVerticleFillMod })
            {
                float neighborY = current.Y + yOffset;
                if (yMap.TryGetValue(neighborY, out List<SegmentData> neighbors))
                {
                    foreach (SegmentData neighbor in neighbors)
                    {
                        // Check X overlap
                        if (current.MaxX >= neighbor.MinX && current.MinX <= neighbor.MaxX && Mathf.Abs(current.MinX - neighbor.MinX) < 2.1f * renderScale && Mathf.Abs(current.MaxX - neighbor.MaxX) < 2.1f * renderScale)
                        {
                            //uf.Union(current.Index, neighbor.Index);
                        }
                    }
                }
            }
        }

        bool reverseAdd = true;

        // 4. Merge connected segments into final groups
        Dictionary<float, List<Vector2>> groups = new Dictionary<float, List<Vector2>>();
        for (int i = 0; i < segments.Count; i++)
        {
            int root = uf.Find(i);
            if (!groups.ContainsKey(root)) groups[root] = new List<Vector2>();

            if (segments[i][0].y % (simpleVerticleFillMod * 2) == 0)
            {
                groups[root].AddRange(segments[i]);
            }
            else
            {
                segments[i].Reverse();

                groups[root].AddRange(segments[i]);
            }

            reverseAdd = !reverseAdd;
        }




        // 5. Sort points in final groups (top-to-bottom, left-to-right)
        List<List<Vector2>> result = new List<List<Vector2>>();
        foreach (var group in groups.Values)
        {
            //group.Sort((a, b) => b.y != a.y ? b.y.CompareTo(a.y) : a.x.CompareTo(b.x));
            result.Add(group);
        }

        return result;
    }


    // Helper classes
    class SegmentData
    {
        public int Index;
        public float Y;
        public float MinX;
        public float MaxX;

        public SegmentData(int index, float y, float minX, float maxX)
        {
            Index = index;
            Y = y;
            MinX = minX;
            MaxX = maxX;
        }
    }

    class UnionFind
    {
        private int[] parent;

        public UnionFind(int size)
        {
            parent = new int[size];
            for (int i = 0; i < size; i++) parent[i] = i;
        }

        public int Find(int x) => parent[x] == x ? x : parent[x] = Find(parent[x]);

        public void Union(int x, int y)
        {
            int rootX = Find(x);
            int rootY = Find(y);
            if (rootX != rootY) parent[rootY] = rootX;
        }
    }












    public List<List<Vector2>> FindConcentrationsOptimized(List<Vector2> pointsToAnalyze, float radius)
    {
        List<List<Vector2>> concentrations = new List<List<Vector2>>();
        if (pointsToAnalyze.Count == 0) return concentrations;

        // Precompute squared radius to avoid sqrt in distance checks
        float radiusSq = radius * radius;

        // Create a grid to partition points into cells of size 'radius'
        Dictionary<Vector2Int, List<Vector2>> grid = new Dictionary<Vector2Int, List<Vector2>>();
        HashSet<Vector2> visited = new HashSet<Vector2>();

        // Populate the grid
        foreach (Vector2 point in pointsToAnalyze)
        {
            Vector2Int cell = new Vector2Int(
                Mathf.FloorToInt(point.x / radius),
                Mathf.FloorToInt(point.y / radius)
            );

            if (!grid.ContainsKey(cell))
                grid.Add(cell, new List<Vector2>());
            grid[cell].Add(point);
        }

        foreach (Vector2 point in pointsToAnalyze)
        {
            if (visited.Contains(point)) continue;

            Queue<Vector2> queue = new Queue<Vector2>();
            List<Vector2> cluster = new List<Vector2>();

            queue.Enqueue(point);
            visited.Add(point);
            cluster.Add(point);

            while (queue.Count > 0)
            {
                Vector2 current = queue.Dequeue();
                Vector2Int currentCell = new Vector2Int(
                    Mathf.FloorToInt(current.x / radius),
                    Mathf.FloorToInt(current.y / radius)
                );

                // Check all 9 adjacent cells (including current cell)
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        Vector2Int neighborCell = new Vector2Int(currentCell.x + x, currentCell.y + y);
                        if (grid.TryGetValue(neighborCell, out List<Vector2> cellPoints))
                        {
                            // Check each point in the neighboring cell
                            foreach (Vector2 neighbor in cellPoints)
                            {
                                if (!visited.Contains(neighbor))
                                {
                                    // Calculate squared distance
                                    float dx = current.x - neighbor.x;
                                    float dy = current.y - neighbor.y;
                                    float distSq = dx * dx + dy * dy;

                                    if (distSq <= radiusSq)
                                    {
                                        visited.Add(neighbor);
                                        cluster.Add(neighbor);
                                        queue.Enqueue(neighbor);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            concentrations.Add(cluster);
        }

        return concentrations;
    }




    


    public List<Vector2> SortClockwise(List<Vector2> pointsToSort)
    {
        if (pointsToSort == null || pointsToSort.Count < 3)
        {
            //Debug.LogWarning("Cannot sort less than 3 points.");
            return pointsToSort; // Return original if less than 3 points or null
        }

        // 1. Find the center point (centroid)
        Vector2 center = Vector2.zero;
        foreach (Vector2 point in pointsToSort)
        {
            center += point;
        }
        center /= pointsToSort.Count;

        // 2. Calculate angles relative to the center
        List<Tuple<Vector2, float>> pointsWithAngles = new List<Tuple<Vector2, float>>();
        foreach (Vector2 point in pointsToSort)
        {
            float angle = Mathf.Atan2(point.y - center.y, point.x - center.x);
            pointsWithAngles.Add(new Tuple<Vector2, float>(point, angle));
        }

        // 3. Sort by angle
        pointsWithAngles = pointsWithAngles.OrderBy(p => p.Item2).ToList();

        // 4. Extract the sorted points
        List<Vector2> sortedPoints = pointsWithAngles.Select(p => p.Item1).ToList();

        return sortedPoints;
    }



















    public List<Vector2> SortPoints(List<Vector2> points)
    {
        if (points == null || points.Count == 0)
            return new List<Vector2>();

        List<Vector2> sortedPoints = new List<Vector2>();
        List<Vector2> remainingPoints = new List<Vector2>(points);

        // Start with the first point
        Vector2 currentPoint = remainingPoints[0];
        sortedPoints.Add(currentPoint);
        remainingPoints.RemoveAt(0);

        int pointsToRemove = 0;

        Vector2 lastPoint = currentPoint;

        // Loop through the remaining points
        while (remainingPoints.Count > pointsToRemove)
        {
            Vector2 searchOffset = Vector2.zero;

            

            if (sortedPoints.Count == 1)
            {
                searchOffset = (Vector2.right * 0.01f);
            }
            else
            {
                searchOffset = Vector2.zero;
            }


            Vector2 nextPoint = FindNextClosestPoint(currentPoint + searchOffset, remainingPoints);

            lastPoint = currentPoint;

            sortedPoints.Add(nextPoint);
            remainingPoints.Remove(nextPoint);
            currentPoint = nextPoint;
        }

        return sortedPoints;
    }

    private Vector2 FindNextClosestPoint(Vector2 currentPoint, List<Vector2> points)
    {
        Vector2 closestPoint = Vector2.zero;
        float closestDistance = float.MaxValue;

        foreach (Vector2 point in points)
        {
            float distance = Vector2.Distance(currentPoint, point);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPoint = point;
            }
        }

        return closestPoint;
    }










    

















    [ContextMenu("DoBoth")]
    public void DoBoth()
    {
        Vector2 svgSize = docSettings.svgSize;
        float renderScale = docSettings.renderScale;
        Vector2 clippingSize = docSettings.clippingSize;
        bool useAnimationSVGOffset = docSettings.useAnimationSVGOffset;
        bool generateArtWithNewSeed = docSettings.generateArtWithNewSeed;
        List<Color> pensToUse = docSettings.pensToUse;

        plottedColors = new List<Color>();

        List<Color> OGPotColors = potentialColors;

        listOfAllPathsThisRun = new List<List<List<Vector2>>>();

        Vector2 OGadditionalSpawnOffset = additionalSpawnOffset;

        float OGrandomOffsetFromCenter = randomOffsetFromCenter;

        int ogPathCount = pathCount;

        listsOfAllPathsByColor = new List<List<List<Vector2>>>();

        listOfPaths = new List<List<Vector2>>();

        plotColors = new List<Color>();


        for (int i = 0; i < potentialColors.Count; i++)
        {
            listsOfAllPathsByColor.Add(new List<List<Vector2>>());
        }



        for (int i = 0; i < shapeCount; i++)
        {
            int randomColorIndex = 0;

            if (removeColorOptionAfterUse)
            {
                randomColorIndex = (i) % (potentialColors.Count);
            }
            else
            {
                randomColorIndex = UnityEngine.Random.Range(0, potentialColors.Count);
            }


            plotColors.Add(potentialColors[randomColorIndex]);
        }




        for (int i = 0; i < oldPlots.Count; i++)
        {
            DestroyImmediate(oldPlots[i]);
        }

        oldPlots = new List<GameObject>();

        svgParent.gameObject.SetActive(false);

        if (generateArtWithNewSeed)
        {
            seedValue = UnityEngine.Random.Range(-(int.MaxValue / 2), int.MaxValue / 2);
        }

        UnityEngine.Random.InitState(seedValue);

        piecename = textImporter.GenerateRandomName();

        yourFileName = piecename.Replace(" ", "");

        GenerateFlowField();

        float ogRotation = shapeRotation;

        Vector2 ogSizeMinMax = circleRadius;

        chosenSize = UnityEngine.Random.Range(circleRadius.x, circleRadius.y);

        for (int i = 0; i < plotColors.Count; i++)
        {
            for (int j = 0; j < lineObjects.Count; j++)
            {
                DestroyImmediate(lineObjects[j].gameObject);
            }

            lineObjects = new List<LineRenderer>();



            if (randomlyFlipFlowDirectionsPerColor)
            {
                constantFlowDir.x -= (constantFlowDir.x * 2) * UnityEngine.Random.Range(0, 2);

                constantFlowDir.y -= (constantFlowDir.y * 2) * UnityEngine.Random.Range(0, 2);

                flowFromCenterMulti -= (flowFromCenterMulti * 2) * UnityEngine.Random.Range(0, 2);
            }

            if (randomlyFlipShapePositionPerColor)
            {
                additionalSpawnOffset.x -= (additionalSpawnOffset.x * 2) * UnityEngine.Random.Range(0, 2);

                additionalSpawnOffset.y -= (additionalSpawnOffset.y * 2) * UnityEngine.Random.Range(0, 2);
            }


            if (flipFlowsAndPositions)
            {
                constantFlowDir.x -= (constantFlowDir.x * 2);

                constantFlowDir.y -= (constantFlowDir.y * 2);

                additionalSpawnOffset.x -= (additionalSpawnOffset.x * 2);

                additionalSpawnOffset.y -= (additionalSpawnOffset.y * 2);
            }

            if (fitEachShapeInLast)
            {

            }

            ChangeConsistentFlow();

            GenerateWork(i);

            if (fitEachShapeInLast)
            {


                chosenSize *= 1f / Mathf.Sqrt(2);

                if (randomOffsetFromCenter != 0)
                {
                    randomOffsetFromCenter = chosenSize - ((chosenSize * (1f / Mathf.Sqrt(2))));
                }

                pathCount = (int)(pathCount * (1f / Mathf.Sqrt(2)));
            }

            if (redoFlowFieldForNewColors)
            {
                GenerateFlowField();
            }

            if (rotateEachShape45)
            {
                shapeRotation += 45;
            }

            SetRenderValues(lineObjects, unlitMat, plotColors[i], true);

            oldPlots.Add(Instantiate(svgParent.gameObject));

            oldPlots[i].SetActive(true);
        }

        GenerateDispalySVG(listOfAllPathsThisRun);


        shapeRotation = ogRotation;

        circleRadius = ogSizeMinMax;

        pathCount = ogPathCount;

        additionalSpawnOffset = OGadditionalSpawnOffset;

        randomOffsetFromCenter = OGrandomOffsetFromCenter;

        potentialColors = OGPotColors;

        textImporter.CompileInfoPage();

        if (isFinalRender)
        {
            urlsaver.artName = piecename;
            urlsaver.shortURL = shortURL;

            urlsaver.SubmitFeedback();
        }
    }

    [ContextMenu("GenerateFlowField")]
    public void GenerateFlowField()
    {
        Vector2 svgSize = docSettings.svgSize;
        float renderScale = docSettings.renderScale;
        Vector2 clippingSize = docSettings.clippingSize;
        bool useAnimationSVGOffset = docSettings.useAnimationSVGOffset;
        bool generateArtWithNewSeed = docSettings.generateArtWithNewSeed;
        List<Color> pensToUse = docSettings.pensToUse;

        flowFieldPositions = new List<Vector2>();

        flowFieldDirections = new List<Vector2>();

        float extraSpace = 75;

        for (int i = 0; i < flowFieldNodeCount; i++)
        {
            flowFieldPositions.Add(new Vector2(UnityEngine.Random.Range(0 - extraSpace, svgSize.x + extraSpace), UnityEngine.Random.Range(0 - extraSpace, svgSize.y + extraSpace)));

            Vector2 dir = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));

            dir.Normalize();

            flowFieldDirections.Add(dir);
        }
    }

    [ContextMenu("ChangeConsistentFlow")]
    public void ChangeConsistentFlow()
    {
        if (useRandomContantFlowDir)
        {
            constantMoveValue = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
        }

        else
        {
            constantMoveValue = constantFlowDir;
        }

        constantMoveValue.Normalize();


        constantMoveValue *= UnityEngine.Random.Range(constantSpeedMinMax.x, constantSpeedMinMax.y);
    }

    [ContextMenu("GenerateWork")]
    public void GenerateWork(int printIndex)
    {
        Vector2 svgSize = docSettings.svgSize;
        float renderScale = docSettings.renderScale;
        Vector2 clippingSize = docSettings.clippingSize;
        bool useAnimationSVGOffset = docSettings.useAnimationSVGOffset;
        bool generateArtWithNewSeed = docSettings.generateArtWithNewSeed;
        List<Color> pensToUse = docSettings.pensToUse;

        lineObjects = new List<LineRenderer>();

        listOfPaths = new List<List<Vector2>>();

        float spawnOffsetX = UnityEngine.Random.Range(-randomOffsetFromCenter, randomOffsetFromCenter);
        float spawnOffsetY = UnityEngine.Random.Range(-randomOffsetFromCenter, randomOffsetFromCenter);


        float spawnRad = chosenSize;


        Vector2 spawnOffset = new Vector2(spawnOffsetX, spawnOffsetY);



        float fakeRot = shapeRotation * Mathf.Deg2Rad;

        if (useRandomRoation)
        {
            float randomRot = UnityEngine.Random.Range(0, 360f);

            fakeRot = randomRot * Mathf.Deg2Rad;

            shapeRotation = randomRot;
        }

        int vertCount = posibleShapeVertCount[UnityEngine.Random.Range(0, posibleShapeVertCount.Count)];

        List<Vector2> circleSpawnPoints = FindPointsOnSimpleShape(spawnCircleVertCount, vertCount, (svgSize / 2f) + spawnOffset + additionalSpawnOffset, spawnRad, fakeRot);
        List<Vector2> drawShapeSpawnPoints = FindPointsOnSimpleShape(spawnCircleVertCount / 10, vertCount, (svgSize / 2f) + spawnOffset + additionalSpawnOffset, spawnRad, fakeRot);

        drawShapeSpawnPoints.Add(drawShapeSpawnPoints[0]);

        if (fitEachShapeInLast)
        {
            additionalSpawnOffset += spawnOffset;
        }

        //get paint
        List<Vector2> paintSpot = new List<Vector2>();

        //bottom
        paintSpot.Add(new Vector3(svgSize.x - 1, 1, 0));
        paintSpot.Add(new Vector3(svgSize.x - 1, 3, 0));
        paintSpot.Add(new Vector3(svgSize.x - 1, 1, 0));
        paintSpot.Add(new Vector3(svgSize.x - 1, 3, 0));





        foreach (LineRenderer line in lineObjects)
        {
            if (line)
            {
                line.SetPositions(new Vector3[0]);
            }

        }

        int curLineCount = 0;



        for (int i = 0; i < pathCount; i++)
        {
            bool addCurPosition = true;

            List<Vector2> pointsThisPath = new List<Vector2>();

            Vector3 startingPos = circleSpawnPoints[(int)((i / (float)pathCount) * circleSpawnPoints.Count)];

            Vector2 lastPos = startingPos;

            Vector2 curPos = startingPos;

            Vector3 realFakePos = new Vector3(((svgSize / 2f) + additionalSpawnOffset).x, ((svgSize / 2f) + additionalSpawnOffset).y, 0);

            Vector3 dirFromCenter = startingPos - realFakePos;

            dirFromCenter.Normalize();

            pointsThisPath.Add(startingPos);


            //generate segments
            for (int j = 0; j < pathLength; j++)
            {
                Vector2 moveDir = Vector2.zero;

                if (curPos.x <= 0 + clippingSize.x || curPos.x >= svgSize.x - clippingSize.x || curPos.y <= 0 + clippingSize.y || curPos.y >= svgSize.y - clippingSize.y)
                {
                    addCurPosition = false;
                }
                else
                {
                    addCurPosition = true;
                }


                for (int k = 0; k < flowFieldPositions.Count; k++)
                {
                    Vector2 thisNodesMovement = flowFieldDirections[k];

                    thisNodesMovement *= Mathf.Clamp((maxInfluenceDist - Vector2.Distance(curPos, flowFieldPositions[k])), 0, maxInfluenceDist) / maxInfluenceDist;

                    moveDir += thisNodesMovement;
                }


                curPos += ((moveDir * flowFieldMovementMulti) + constantMoveValue);


                if (useFlowFromSpawn)
                {
                    curPos += (new Vector2(dirFromCenter.x, dirFromCenter.y) * flowFromCenterMulti);
                }

                Vector2 finalDir = curPos - lastPos;

                Vector2 realFinalPos = lastPos;

                int asdasdasd = j;

                if (curPos.x <= 0 + clippingSize.x && lastPos.x > 0 + clippingSize.x)
                {
                    j = pathLength - 1;

                    while (realFinalPos.x > clippingSize.x && realFinalPos.y < svgSize.y - clippingSize.y && realFinalPos.y > clippingSize.y)
                    {
                        realFinalPos += finalDir.normalized * 0.1f;
                    }

                    curPos = realFinalPos;
                }

                if (curPos.x >= svgSize.x - clippingSize.x && lastPos.x < svgSize.x - clippingSize.x)
                {
                    j = pathLength - 1;

                    while (realFinalPos.x < svgSize.x - clippingSize.x && realFinalPos.y < svgSize.y - clippingSize.y && realFinalPos.y > clippingSize.y)
                    {
                        realFinalPos += finalDir.normalized * 0.1f;
                    }

                    curPos = realFinalPos;
                }




                if (curPos.y <= 0 + clippingSize.y)
                {
                    j = pathLength - 1;

                    while (realFinalPos.y > clippingSize.y)
                    {
                        realFinalPos += finalDir.normalized * 0.1f;
                    }

                    curPos = realFinalPos;
                }

                if (curPos.y >= svgSize.y - clippingSize.y)
                {
                    j = pathLength - 1;

                    while (realFinalPos.y < svgSize.y - clippingSize.y)
                    {
                        realFinalPos += finalDir.normalized * 0.1f;
                    }

                    curPos = realFinalPos;
                }



                if (asdasdasd != j && asdasdasd == 1)
                {
                    Debug.Log("FUCK");
                }

                lastPos = curPos;

                if (addCurPosition)
                {
                    pointsThisPath.Add(curPos);
                }
            }


            if (dipInPaint)
            {
                curLineCount++;

                PlacePath(lineWidth, paintSpot, i, svgParent, new Color(10, 10, 10), true);
            }

            curLineCount++;
            PlacePath(lineWidth, pointsThisPath, i, svgParent, new Color(10, 10, 10), true);
        }


        if (drawSpawnShapes)
        {
            PlacePath(lineWidth, drawShapeSpawnPoints, pathCount, svgParent, new Color(10, 10, 10), true);
        }


        if (drawBounds && printIndex >= shapeCount - potentialColors.Count)
        {
            //place boarder
            List<Vector2> boarderPoints = new List<Vector2>();

            //bottom
            boarderPoints.Add(new Vector3(0 + clippingSize.x, 0 + clippingSize.y, 0));
            boarderPoints.Add(new Vector3(svgSize.x - clippingSize.x, 0 + clippingSize.y, 0));
            boarderPoints.Add(new Vector3(svgSize.x - clippingSize.x, svgSize.y - clippingSize.y, 0));
            boarderPoints.Add(new Vector3(0 + clippingSize.x, svgSize.y - clippingSize.y, 0));
            boarderPoints.Add(new Vector3(0 + clippingSize.x, 0 + clippingSize.y, 0));

            if (drawSpawnShapes)
            {
                PlacePath(lineWidth, boarderPoints, pathCount + 1, svgParent, new Color(10, 10, 10), true);
            }
            else
            {
                PlacePath(lineWidth, boarderPoints, pathCount, svgParent, new Color(10, 10, 10), true);
            }
        }


        listOfAllPathsThisRun.Add(listOfPaths);

        listsOfAllPathsByColor[printIndex % potentialColors.Count].AddRange(listOfPaths); // using System.Linq;


        GenerateSVG(listsOfAllPathsByColor[printIndex % potentialColors.Count], false, false, plotColors.IndexOf(plotColors[printIndex]), plotColors[printIndex], svgSize, yourFileName);
    }

    [ContextMenu("ResetLineObjects")]
    public void ResetLineObjects()
    {
        lineObjects = new List<LineRenderer>();

        foreach (Transform child in svgParent)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }
    }

    [ContextMenu("PlacePath")]
    public void PlacePath(float width, List<Vector2> points, int IDnum, Transform placeToPlace, Color col, bool spawnLineRenderer)
    {
        listOfPaths.Add(points);

        List<Vector3> realPoints = new List<Vector3>();

        foreach (Vector2 p in points)
        {
            realPoints.Add(new Vector3(p.x, p.y, 0));
        }

        //if (svgParent.childCount <= IDnum)
        //{

        //}

        if (spawnLineRenderer == true)
        {
            GameObject newLine = new GameObject("line " + IDnum);

            newLine.transform.SetParent(placeToPlace);

            LineRenderer lineRenderer = newLine.AddComponent<LineRenderer>();

            //new Color(10,10,10) == escape color - do not color lines right now
            if (col != new Color(10, 10, 10))
            {
                //lineRenderer.material = new Material(unlitMat);

                //lineRenderer.material.SetColor("_BaseColor", col);

                lineRenderer.endColor = col;
                lineRenderer.startColor = col;
            }




            lineRenderer.widthMultiplier = width;

            lineObjects.Add(lineRenderer);

            //if (lineObjects[IDnum])
            //{
            //    lineObjects[lineObjects.Count - 1].positionCount = points.Count;
            //    lineObjects[lineObjects.Count - 1].SetPositions(realPoints.ToArray());
            //    lineObjects[lineObjects.Count - 1].useWorldSpace = false;
            //}

            //lineObjects[lineObjects.Count - 1].positionCount = points.Count;
            //lineObjects[lineObjects.Count - 1].SetPositions(realPoints.ToArray());
            //lineObjects[lineObjects.Count - 1].useWorldSpace = false;


            lineObjects[lineObjects.Count - 1].positionCount = points.Count;
            lineObjects[lineObjects.Count - 1].SetPositions(realPoints.ToArray());
            lineObjects[lineObjects.Count - 1].useWorldSpace = false;
        }

        
    }

    [ContextMenu("SaveSVG")]
    public void GenerateSVG(List<List<Vector2>> allPaths, bool saveDisplayCopy, bool isInfoPage, int printColorIndex, Color drawColor, Vector2 svgFileSize, string fileNameToUse, string titleExtention = "", bool saveTxtCopy = true)
    {
        Vector2 svgSize = docSettings.svgSize;
        float renderScale = docSettings.renderScale;
        Vector2 clippingSize = docSettings.clippingSize;
        bool useAnimationSVGOffset = docSettings.useAnimationSVGOffset;
        bool generateArtWithNewSeed = docSettings.generateArtWithNewSeed;
        List<Color> pensToUse = docSettings.pensToUse;

        StringBuilder svgContent = new StringBuilder();

        // SVG header and basic structure
        svgContent.AppendLine("<svg");
        svgContent.AppendLine($"   width=\"{svgFileSize.x}\"");
        svgContent.AppendLine($"   height = \"{svgFileSize.y}\"");
        svgContent.AppendLine("   version=\"1.1\"");
        svgContent.AppendLine("   id=\"svg2\"");
        svgContent.AppendLine("   xmlns=\"http://www.w3.org/2000/svg\"");
        svgContent.AppendLine("   xmlns:svg=\"http://www.w3.org/2000/svg\">");
        svgContent.AppendLine("  <defs");
        svgContent.AppendLine("     id=\"defs6\" />");
        svgContent.AppendLine("");

        Color drawLineColor = drawColor;

        if (saveDisplayCopy)
        {
            svgContent.AppendLine("<rect");

            if (drawColor == Color.white)
            {
                svgContent.AppendLine($"   style=\"fill:#{ColorUtility.ToHtmlStringRGB(Color.black)};stroke-width:0;stroke-opacity:0\"");
            }
            else
            {
                svgContent.AppendLine($"   style=\"fill:#{ColorUtility.ToHtmlStringRGB(bgColor)};stroke-width:0;stroke-opacity:0\"");
            }


            svgContent.AppendLine($"   id=\"Background\"");
            svgContent.AppendLine($"   width=\"{svgSize.x}\"");
            svgContent.AppendLine($"   height=\"{svgSize.y}\"");
            svgContent.AppendLine($"   x=\"0\"");
            svgContent.AppendLine($"   y=\"0\"");
            svgContent.AppendLine("   rx=\"0\" />");
        }
        else
        {
            //drawLineColor = Color.black;
        }

        for (int i = 0; i < allPaths.Count; i++)
        {
            if (allPaths[i].Count >= 2)
            {
                //svgContent.AppendLine("  <path");
                svgContent.AppendLine($"<path  style=\"fill:none;stroke:#{ColorUtility.ToHtmlStringRGB(drawLineColor)};stroke-width:{lineWidth / 4};stroke-opacity:{drawLineColor.a}\"");

                string thisPath = "       d=\"M " + allPaths[i][0].x + "," + allPaths[i][0].y + " ";

                for (int j = 1; j < allPaths[i].Count; j++)
                {
                    thisPath += allPaths[i][j].x + "," + allPaths[i][j].y + " ";
                }

                thisPath += "\"";

                svgContent.AppendLine(thisPath + $"      id = \"{i}\" />");
                //svgContent.AppendLine();

                //Debug.Log(i + " ID");



                //if (new Vector2(allPaths[i][0].x, allPaths[i][0].y) != new Vector2(allPaths[i][1].x, allPaths[i][1].y))
                //{
                //    //svgContent.AppendLine("  <path");
                //    svgContent.AppendLine($"<path  style=\"fill:none;stroke:#{ColorUtility.ToHtmlStringRGB(drawLineColor)};stroke-width:{lineWidth / 4};stroke-opacity:{drawLineColor.a}\"");

                //    string thisPath = "       d=\"M " + allPaths[i][0].x + "," + allPaths[i][0].y + " ";

                //    for (int j = 1; j < allPaths[i].Count; j++)
                //    {
                //        thisPath += allPaths[i][j].x + "," + allPaths[i][j].y + " ";
                //    }

                //    thisPath += "\"";

                //    svgContent.AppendLine(thisPath);
                //    svgContent.AppendLine($"      id = \"{i}\" />");

                //    Debug.Log(i + " ID");
                //}
            }
        }


        // Close the SVG
        svgContent.AppendLine("</svg>");

        if (!Directory.Exists($"Z:\\Shit\\SVG Stuff\\SVG-Stuff\\Assets\\Resources\\{fileNameToUse}"))
        {
            AssetDatabase.CreateFolder("Assets/Resources", fileNameToUse);
        }


        string desktopPath = $"Z:\\Shit\\SVG Stuff\\SVG-Stuff\\Assets\\Resources\\{fileNameToUse}";
        desktopPath += "\\";

        string filePath = "";
        string filePathTXT = "";

        if (!saveDisplayCopy)
        {
            filePathTXT = Path.Combine(desktopPath, fileNameToUse + titleExtention + printColorIndex.ToString() + ".txt");

            filePath = Path.Combine(desktopPath, fileNameToUse + titleExtention + printColorIndex.ToString() + ".svg");
        }
        else
        {
            filePath = Path.Combine(desktopPath, fileNameToUse + titleExtention + printColorIndex.ToString() + " Display.svg");
            ScreenCapture.CaptureScreenshot(desktopPath + fileNameToUse + " Screenshot.png");
        }

        if (isInfoPage)
        {
            filePath = Path.Combine(desktopPath, fileNameToUse + "Info.txt");
        }


        // Write the SVG content to a file
        File.WriteAllText(filePath, svgContent.ToString());

        if (saveTxtCopy == true)
        {
            File.WriteAllText(filePathTXT, svgContent.ToString());
        }

        //Debug.Log("Done");
    }

    public void GenerateDispalySVG(List<List<List<Vector2>>> allPathsToDisplay)
    {
        Vector2 svgSize = docSettings.svgSize;
        float renderScale = docSettings.renderScale;
        Vector2 clippingSize = docSettings.clippingSize;
        bool useAnimationSVGOffset = docSettings.useAnimationSVGOffset;
        bool generateArtWithNewSeed = docSettings.generateArtWithNewSeed;
        List<Color> pensToUse = docSettings.pensToUse;

        StringBuilder svgContent = new StringBuilder();

        // SVG header and basic structure
        svgContent.AppendLine("<svg");
        svgContent.AppendLine($"   width=\"{svgSize.x}\"");
        svgContent.AppendLine($"   height = \"{svgSize.y}\"");
        svgContent.AppendLine("   version=\"1.1\"");
        svgContent.AppendLine("   id=\"svg2\"");
        svgContent.AppendLine("   xmlns=\"http://www.w3.org/2000/svg\"");
        svgContent.AppendLine("   xmlns:svg=\"http://www.w3.org/2000/svg\">");
        svgContent.AppendLine("  <defs");
        svgContent.AppendLine("     id=\"defs6\" />");
        svgContent.AppendLine("");


        svgContent.AppendLine("<rect");
        svgContent.AppendLine($"   style=\"fill:#{ColorUtility.ToHtmlStringRGB(bgColor)};stroke-width:0;stroke-opacity:0\"");
        svgContent.AppendLine($"   id=\"Background\"");
        svgContent.AppendLine($"   width=\"{svgSize.x}\"");
        svgContent.AppendLine($"   height=\"{svgSize.y}\"");
        svgContent.AppendLine($"   x=\"0\"");
        svgContent.AppendLine($"   y=\"0\"");
        svgContent.AppendLine("   rx=\"0\" />");

        for (int k = 0; k < allPathsToDisplay.Count; k++)
        {
            Color drawLineColor = plotColors[k];

            for (int i = 0; i < allPathsToDisplay[k].Count; i++)
            {
                if (allPathsToDisplay[k][i].Count >= 2)
                {
                    if (new Vector2(allPathsToDisplay[k][i][0].x, allPathsToDisplay[k][i][0].y) != new Vector2(allPathsToDisplay[k][i][1].x, allPathsToDisplay[k][i][1].y))
                    {
                        svgContent.AppendLine("  <path");
                        svgContent.AppendLine($"     style=\"fill:none;stroke:#{ColorUtility.ToHtmlStringRGB(drawLineColor)};stroke-width:{lineWidth / 4};stroke-opacity:{drawLineColor.a}\"");

                        string thisPath = "     d = \"M " + allPathsToDisplay[k][i][0].x + "," + allPathsToDisplay[k][i][0].y + " ";

                        for (int j = 1; j < allPathsToDisplay[k][i].Count; j++)
                        {
                            thisPath += allPathsToDisplay[k][i][j].x + "," + allPathsToDisplay[k][i][j].y + " ";
                        }

                        thisPath += "\"";

                        svgContent.AppendLine(thisPath);
                        svgContent.AppendLine($"id = \"{i}\" />");
                    }
                }

            }
        }

        // Close the SVG
        svgContent.AppendLine("</svg>");

        if (!Directory.Exists($"Z:\\Shit\\SVG Stuff\\SVG-Stuff\\Assets\\Resources\\{yourFileName}"))
        {
            AssetDatabase.CreateFolder("Assets/Resources", yourFileName);
        }

        string desktopPath = $"Z:\\Shit\\SVG Stuff\\SVG-Stuff\\Assets\\Resources\\{yourFileName}";
        desktopPath += "\\";

        string filePath = "";

        filePath = Path.Combine(desktopPath, yourFileName + " Display.svg");
        ScreenCapture.CaptureScreenshot(desktopPath + yourFileName + " Screenshot.png");

        // Write the SVG content to a file
        File.WriteAllText(filePath, svgContent.ToString());

        Debug.Log("Done");
    }







    







    public static List<Vector2> CalculateCirclePoints(int vertexCount, Vector2 center, float radius, float rotation)
    {
        List<Vector2> points = new List<Vector2>();

        float angleIncrement = 2f * (float)Math.PI / vertexCount;

        for (int i = 0; i < vertexCount; i++)
        {
            float angle = (i * angleIncrement) + rotation;
            float x = center.x + ((float)Math.Cos(angle) * radius);
            float y = center.y + ((float)Math.Sin(angle) * radius);
            points.Add(new Vector2(x, y));
        }

        return points;
    }

    public List<Vector2> FindPointsOnSimpleShape(int pointCount, int shapeVertCount, Vector2 position, float radius, float rotation)
    {
        Vector2 svgSize = docSettings.svgSize;
        float renderScale = docSettings.renderScale;
        Vector2 clippingSize = docSettings.clippingSize;
        bool useAnimationSVGOffset = docSettings.useAnimationSVGOffset;
        bool generateArtWithNewSeed = docSettings.generateArtWithNewSeed;
        List<Color> pensToUse = docSettings.pensToUse;

        List<Vector2> pointsOnShape = new List<Vector2>();

        List<Vector2> shapeVertPoints = CalculateCirclePoints(shapeVertCount, position, radius, rotation);

        for (int i = 0; i < pointCount; i++)
        {
            var shapeLineSegment = (int)((i / (float)pointCount) * shapeVertPoints.Count);

            Vector2 pointOnShape = new Vector2();

            //if I need to wrap
            if (shapeLineSegment >= shapeVertPoints.Count - 1)
            {
                pointOnShape = Vector2.Lerp(shapeVertPoints[0], shapeVertPoints[shapeLineSegment], 1 - ((i / ((float)pointCount / (float)shapeVertPoints.Count)) % 1));
            }
            else
            {
                pointOnShape = Vector2.Lerp(shapeVertPoints[shapeLineSegment], shapeVertPoints[shapeLineSegment + 1], (i / ((float)pointCount / (float)shapeVertPoints.Count)) % 1);
            }

            pointsOnShape.Add(pointOnShape);
        }

        for (int i = 0; i < pointCount; i++)
        {

            if (pointsOnShape[i].x < 0 + clippingSize.x)
            {
                pointsOnShape[i] = new Vector2(0 + clippingSize.x, pointsOnShape[i].y);
            }

            if (pointsOnShape[i].x > svgSize.x - clippingSize.x)
            {
                pointsOnShape[i] = new Vector2(svgSize.x - clippingSize.x, pointsOnShape[i].y);
            }


            if (pointsOnShape[i].y < 0 + clippingSize.y)
            {
                pointsOnShape[i] = new Vector2(pointsOnShape[i].x, 0 + clippingSize.y);
            }

            if (pointsOnShape[i].y > svgSize.y - clippingSize.y)
            {
                pointsOnShape[i] = new Vector2(pointsOnShape[i].x, svgSize.y - clippingSize.y);
            }
        }

        return pointsOnShape;
    }









}


public static class VectorListExtensions
{
    public static List<List<Vector2>> SortByDistanceToPoint(this List<List<Vector2>> listOfVectorLists, Vector2 point)
    {
        // Create a copy to avoid modifying the original list
        List<List<Vector2>> sortedList = listOfVectorLists.ToList();

        // Sort the copy using the custom comparison delegate
        sortedList.Sort((list1, list2) =>
        {
            // Calculate the average position of the first list
            Vector2 avg1 = Vector2.zero;
            if (list1.Count > 0)
            {
                foreach (Vector2 v in list1)
                {
                    avg1 += v;
                }
                avg1 /= list1.Count;
            }

            // Calculate the average position of the second list
            Vector2 avg2 = Vector2.zero;
            if (list2.Count > 0)
            {
                foreach (Vector2 v in list2)
                {
                    avg2 += v;
                }
                avg2 /= list2.Count;
            }

            // Compare the squared magnitudes of the distances from the point to the average positions
            float dist1 = (avg1 - point).sqrMagnitude;
            float dist2 = (avg2 - point).sqrMagnitude;
            return dist1.CompareTo(dist2);
        });

        return sortedList;
    }
}