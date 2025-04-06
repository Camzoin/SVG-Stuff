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

    public bool generateArtWithNewSeed = false;

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

    public Vector2 svgSize = new Vector2(816, 1056);

    public Vector2 clippingSize = new Vector2(0, 0);

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

    public float renderScale = 4;

    public Burster burster;

    public int simpleVerticleFillMod = 4;

    public bool useAnimationSVGOffset = false;

    public bool useSimpleFillForRender = false;

    public bool fillBGColor = false;

    List<Color> plottedColors = new List<Color>();


    public List<Color> pensToUse = new List<Color>();

    public PlotCombiner pc;

    public bool printRunTimes = false;

    public AnimCellManager animManager;

    public int squiggleLineFillCount = 500;


    [ContextMenu("SetRenderValues")]
    public void SetRenderValues(List<LineRenderer> lineRenderersToSet, Material matToCopy, Color colorToSet, bool resetLinePositions = false, bool resetMyAss = true)
    {
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

            int fillLineCount = squiggleLineFillCount;

            int colorAttempts = 0;

            for (int i = 0; i < fillLineCount; i++)
            {
                int lineSegmentMaxCount = 35;

                List<Vector2> thisFillLine = new List<Vector2>();


                Vector2 additionalOffset = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * 0.4f;


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


                Vector2 curLineWritingPos = lineStartPos;

                //Advance based on normal map colors, if that point is a different color in the color map, end this line
                for (int q = 0; q < lineSegmentMaxCount; q++)
                {
                    startingNormalColor = wholeNormalRenderTexHolder.GetPixel((int)curLineWritingPos.x, (int)curLineWritingPos.y);


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
                                    outlineList.Add(SimplifyPath(scaledPath));
                                }


                            }
                        }
                    }
                }
            }


            //outlineList.AddRange(outlinesToAddSplitUpColorAndOutlineFix);




            fillLineList = PathCrossingRemover.RemoveCrossingPaths(fillLineList, 5);


            outlinesToAddSplitUpColorAndOutlineFix.AddRange(fillLineList);




            bool hasBGFilled = false;


            if (useSimpleFillForRender)
            {

                //Starts at 1 to avoid filling in "empty area", make 0 if you want to fill all colors
                for (int q = 1; q < claimedPixels.Count; q++)
                {
                    if (fillBGColor && hasBGFilled == false)
                    {
                        q = 0;

                        hasBGFilled = true;
                    }


                    List<List<Vector2>> FUICK = GroupVerticalNeighbors(claimedPixels[q]);

                    simpleVertFillLinesByColor.AddRange(FUICK);
                }


                outlinesToAddSplitUpColorAndOutlineFix.AddRange(simpleVertFillLinesByColor);
            }




            for (int q = 0; q < outlinesToAddSplitUpColorAndOutlineFix.Count; q++)
            {
                outlinesToAddSplitUpColorAndOutlineFix[q] = SimplifyPath(outlinesToAddSplitUpColorAndOutlineFix[q]);
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










            //place boarder
            List<Vector2> boarderPoints = new List<Vector2>();

            //bottom
            boarderPoints.Add(new Vector3(0, 0, 0));
            boarderPoints.Add(new Vector3(wholeRenderTexHolder.width * (1f / renderScale), 0, 0));
            boarderPoints.Add(new Vector3(wholeRenderTexHolder.width * (1f / renderScale), wholeRenderTexHolder.height * (1f / renderScale), 0));
            boarderPoints.Add(new Vector3(0, wholeRenderTexHolder.height * (1f / renderScale), 0));
            boarderPoints.Add(new Vector3(0, 0, 0));


            PlacePath(lineWidth, boarderPoints, 0, transform, new Color(10, 10, 10));


            SetRenderValues(lineObjects, unlitMat, Color.white, false, false);

            GameObject holder = Instantiate(new GameObject(), transform);

            foreach (LineRenderer lr in lineObjects)
            {
                lr.transform.SetParent(holder.transform);
            }


            finalLineListByColor[0].Add(boarderPoints);




            //Offset based on cell 4x 3y  using z - animationCell index





            //Offset The lines in the SVG file
            for (int i = 0; i < finalLineListByColor.Count; i++)
            {
                for (int u = 0; u < finalLineListByColor[i].Count; u++)
                {
                    for (int g = 0; g < finalLineListByColor[i][u].Count; g++)
                    {
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









            //Shit way to reduce lines


            //for (int q = 0; q < finalLineListByColor.Count; q++)
            //{
            //    float closeLineSearchRangeE = 8;

            //    List<List<Vector2>> reallySmushedOutlineListE = new List<List<Vector2>>();

            //    List<List<Vector2>> finalLineListByColorHolder = finalLineListByColor[q];



            //    while (finalLineListByColorHolder.Count > 0)
            //    {


            //        if (finalLineListByColorHolder.Count > 1)
            //        {
            //            if (Vector3.Distance(finalLineListByColorHolder[0].Last(), finalLineListByColorHolder[1][0]) < closeLineSearchRangeE)
            //            {
            //                finalLineListByColorHolder[0].AddRange(finalLineListByColorHolder[0 + 1]);

            //                Debug.Log("Merged outlines at distisnace of ");

            //                finalLineListByColorHolder.RemoveAt(1);
            //            }
            //            else
            //            {



            //                reallySmushedOutlineListE.Add(finalLineListByColorHolder[0]);

            //                finalLineListByColorHolder.RemoveAt(0);
            //            }
            //        }
            //        else
            //        {


            //            reallySmushedOutlineListE.Add(finalLineListByColorHolder[0]);

            //            finalLineListByColorHolder.RemoveAt(0);
            //        }
            //    }


            //    Debug.Log("Merged outlines  " + finalLineListByColor[q].Count + " " + reallySmushedOutlineListE.Count);

            //    finalLineListByColor[q] = reallySmushedOutlineListE;

            //}








       














            int lineCountCount = 0;

            for (int q = 0; q < finalLineListByColor.Count; q++)
            {
                for (int k = 0; k < finalLineListByColor[q].Count; k++)
                {
                    if (pensToUse.Count == 0)
                    {
                        PlacePath(lineWidth, finalLineListByColor[q][k], lineCountCount, transform, diffColors[q]);
                    }
                    else
                    {
                        PlacePath(lineWidth, finalLineListByColor[q][k], lineCountCount, transform, pensToUse[q]);
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



        float closeLineSearchRange = 3;

        List<List<Vector2>> reallySmushedOutlineList = new List<List<Vector2>>();

        List<List<Vector2>> outlineListHolder = outlineList;

        while (outlineListHolder.Count > 0)
        {


            if (outlineListHolder[0].Count > 2 && outlineListHolder.Count > 1)
            {
                if (Vector3.Distance(outlineListHolder[0].Last(), outlineListHolder[1][0]) < closeLineSearchRange)
                {
                    outlineListHolder[0].AddRange(outlineListHolder[1]);

                    Debug.Log("Merged outlines at distisnace of ");

                    outlineListHolder.RemoveAt(1);
                }
                else
                {


                    reallySmushedOutlineList.Add(outlineListHolder[0]);


                    outlineListHolder.RemoveAt(0);
                }
            }
            else
            {
                reallySmushedOutlineList.Add(outlineListHolder[0]);

                outlineListHolder.RemoveAt(0);
            }
        }


        outlineList = reallySmushedOutlineList;






        if (pensToUse.Count > 0)
        {
            GenerateSVG(outlineList, false, false,1000, Color.black, svgSize);
        }





        if (pensToUse.Count > 0)
        {
            for (int q = 0; q < pensToUse.Count; q++)
            {
                GenerateSVG(finalCompleteLineListByColor[q], false, false, q, pensToUse[q], svgSize);
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

        pc.SlamTogether(finalCompleteLineListByColor, yourFileName);



        stopwatch.Stop();
        Debug.Log("DONE! TIME = " + stopwatch.Elapsed);
        stopwatch.Start();


        stopwatch.Reset();
    }














    //New AI SHIT







    public List<List<Vector2>> FindPaths(List<Vector2> points, float fixedDistance)
    {
        List<List<Vector2>> allPaths = new List<List<Vector2>>();
        if (points == null || points.Count == 0) return allPaths;

        // Create adjacency list
        List<List<int>> adjacencyList = CreateAdjacencyList(points, fixedDistance);

        // Find connected components
        List<List<int>> connectedComponents = FindConnectedComponents(points, adjacencyList);

        // Generate paths for each component
        foreach (List<int> component in connectedComponents)
        {
            HashSet<int> remainingNodes = new HashSet<int>(component);

            while (remainingNodes.Count > 0)
            {
                List<int> pathIndices = BuildGreedyPath(remainingNodes, adjacencyList);
                allPaths.Add(ConvertIndicesToPoints(pathIndices, points));
            }
        }

        return allPaths;
    }

    private List<List<int>> CreateAdjacencyList(List<Vector2> points, float fixedDistance)
    {
        List<List<int>> adjacencyList = new List<List<int>>();

        for (int i = 0; i < points.Count; i++)
        {
            List<int> neighbors = new List<int>();
            for (int j = 0; j < points.Count; j++)
            {
                if (i != j && Vector2.Distance(points[i], points[j]) == fixedDistance)
                {
                    neighbors.Add(j);
                }
            }
            adjacencyList.Add(neighbors);
        }
        return adjacencyList;
    }

    private List<List<int>> FindConnectedComponents(List<Vector2> points, List<List<int>> adjacencyList)
    {
        List<List<int>> components = new List<List<int>>();
        bool[] visited = new bool[points.Count];

        for (int i = 0; i < points.Count; i++)
        {
            if (!visited[i])
            {
                components.Add(BFS(i, visited, adjacencyList));
            }
        }
        return components;
    }

    private List<int> BFS(int start, bool[] visited, List<List<int>> adjacencyList)
    {
        List<int> component = new List<int>();
        Queue<int> queue = new Queue<int>();
        queue.Enqueue(start);
        visited[start] = true;

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();
            component.Add(current);

            foreach (int neighbor in adjacencyList[current])
            {
                if (!visited[neighbor])
                {
                    visited[neighbor] = true;
                    queue.Enqueue(neighbor);
                }
            }
        }
        return component;
    }

    private List<int> BuildGreedyPath(HashSet<int> remainingNodes, List<List<int>> adjacencyList)
    {
        // Find starting node with least connections
        int startNode = -1;
        int minCount = int.MaxValue;

        foreach (int node in remainingNodes)
        {
            int validNeighbors = CountValidNeighbors(node, remainingNodes, adjacencyList);
            if (validNeighbors < minCount)
            {
                minCount = validNeighbors;
                startNode = node;
            }
        }

        // Build path
        List<int> path = new List<int>();
        int currentNode = startNode;

        while (true)
        {
            path.Add(currentNode);
            remainingNodes.Remove(currentNode);

            // Get valid next nodes
            List<int> nextNodes = new List<int>();
            foreach (int neighbor in adjacencyList[currentNode])
            {
                if (remainingNodes.Contains(neighbor))
                {
                    nextNodes.Add(neighbor);
                }
            }

            if (nextNodes.Count == 0) break;

            // Select next node with least connections
            currentNode = SelectNextNode(nextNodes, remainingNodes, adjacencyList);
        }

        return path;
    }

    private int CountValidNeighbors(int node, HashSet<int> remainingNodes, List<List<int>> adjacencyList)
    {
        int count = 0;
        foreach (int neighbor in adjacencyList[node])
        {
            if (remainingNodes.Contains(neighbor)) count++;
        }
        return count;
    }

    private int SelectNextNode(List<int> candidates, HashSet<int> remainingNodes, List<List<int>> adjacencyList)
    {
        int selected = candidates[0];
        int minNeighbors = int.MaxValue;

        foreach (int node in candidates)
        {
            int count = CountValidNeighbors(node, remainingNodes, adjacencyList);
            if (count < minNeighbors)
            {
                minNeighbors = count;
                selected = node;
            }
        }
        return selected;
    }

    private List<Vector2> ConvertIndicesToPoints(List<int> indices, List<Vector2> points)
    {
        List<Vector2> path = new List<Vector2>();
        foreach (int index in indices)
        {
            path.Add(points[index]);
        }
        return path;
    }































    public List<List<Vector2>> OrderAndSplit(List<Vector2> positions, float adjacencyDistance)
    {
        List<List<Vector2>> result = new List<List<Vector2>>();
        List<Vector2> unprocessed = new List<Vector2>(positions);
        float adjacencySqr = adjacencyDistance * adjacencyDistance; // Precompute squared distance

        while (unprocessed.Count > 0)
        {
            // Start a new path with the first unprocessed point
            Vector2 startPoint = unprocessed[0];
            unprocessed[0] = unprocessed[unprocessed.Count - 1]; // Swap with last for O(1) removal
            unprocessed.RemoveAt(unprocessed.Count - 1);

            List<Vector2> currentPath = new List<Vector2> { startPoint };
            Vector2 currentPoint = startPoint;

            bool foundNext;
            do
            {
                foundNext = false;
                float minSqrDistance = Mathf.Infinity;
                int nearestIndex = -1;

                // Find the nearest unprocessed point using squared distance
                for (int i = 0; i < unprocessed.Count; i++)
                {
                    Vector2 offset = currentPoint - unprocessed[i];
                    float sqrDistance = offset.sqrMagnitude;

                    if (sqrDistance < minSqrDistance)
                    {
                        minSqrDistance = sqrDistance;
                        nearestIndex = i;

                        // Early exit if exact match found
                        if (sqrDistance <= Mathf.Epsilon)
                            break;
                    }
                }

                // Check adjacency using squared distance
                if (nearestIndex != -1 && minSqrDistance <= adjacencySqr)
                {
                    // Swap nearest element with last to remove efficiently
                    Vector2 nearestPoint = unprocessed[nearestIndex];
                    unprocessed[nearestIndex] = unprocessed[unprocessed.Count - 1];
                    unprocessed.RemoveAt(unprocessed.Count - 1);

                    currentPath.Add(nearestPoint);
                    currentPoint = nearestPoint;
                    foundNext = true;
                }
            } while (foundNext);

            result.Add(currentPath);
        }

        return result;
    }





    public List<List<Vector2>> ConnectRegions(List<Vector2> points)
    {
        List<HashSet<Vector2>> regions = FindConnectedRegions(points);
        List<List<Vector2>> result = new List<List<Vector2>>();

        foreach (var regionSet in regions)
        {
            List<Vector2> regionPoints = new List<Vector2>(regionSet);
            List<Vector2> orderedPath = ConnectWithMinimumLines(regionPoints);
            result.Add(orderedPath);
        }

        return result;
    }

    private List<HashSet<Vector2>> FindConnectedRegions(List<Vector2> points)
    {
        HashSet<Vector2> allPoints = new HashSet<Vector2>(points);
        HashSet<Vector2> visited = new HashSet<Vector2>();
        List<HashSet<Vector2>> regions = new List<HashSet<Vector2>>();

        Vector2[] dirs = {
            new Vector2(1, 0), new Vector2(-1, 0),
            new Vector2(0, 1), new Vector2(0, -1)
        };

        foreach (Vector2 point in allPoints)
        {
            if (!visited.Contains(point))
            {
                Queue<Vector2> queue = new Queue<Vector2>();
                HashSet<Vector2> region = new HashSet<Vector2>();
                queue.Enqueue(point);
                visited.Add(point);
                region.Add(point);

                while (queue.Count > 0)
                {
                    Vector2 current = queue.Dequeue();
                    foreach (Vector2 dir in dirs)
                    {
                        Vector2 neighbor = current + dir;
                        if (allPoints.Contains(neighbor) && !visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            region.Add(neighbor);
                            queue.Enqueue(neighbor);
                        }
                    }
                }
                regions.Add(region);
            }
        }
        return regions;
    }

    private List<Vector2> ConnectWithMinimumLines(List<Vector2> regionPoints)
    {
        if (regionPoints.Count == 0)
            return new List<Vector2>();

        HashSet<Vector2> remaining = new HashSet<Vector2>(regionPoints);
        List<Vector2> orderedPath = new List<Vector2>();

        Vector2 current = GetTopLeftMost(remaining);
        orderedPath.Add(current);
        remaining.Remove(current);

        Vector2 currentDirection = Vector2.right;

        Vector2[] directions = {
            Vector2.right, Vector2.up, Vector2.left, Vector2.down
        };

        int directionIndexOffset = 0;

        int lastDirectionIndexOffset = 0;

        while (remaining.Count > 0)
        {
            Vector2 next = current + directions[lastDirectionIndexOffset % 4];
            if (remaining.Contains(next))
            {
                current = next;
                orderedPath.Add(current);
                remaining.Remove(current);
                continue;
            }

            bool foundNext = false;

            directionIndexOffset = lastDirectionIndexOffset;

            foreach (Vector2 dir in directions)
            {
                Vector2 realDir = directions[directionIndexOffset % 4];

                next = current + dir;
                if (remaining.Contains(next))
                {
                    currentDirection = dir;
                    current = next;
                    orderedPath.Add(current);
                    remaining.Remove(current);
                    foundNext = true;

                    lastDirectionIndexOffset = directionIndexOffset;
                    break;
                }

                directionIndexOffset++;
            }


            if (!foundNext)
            {
                foreach (Vector2 pathPoint in orderedPath)
                {
                    foreach (Vector2 dir in directions)
                    {
                        next = pathPoint + dir;
                        if (remaining.Contains(next))
                        {
                            current = next;
                            orderedPath.Add(current);
                            remaining.Remove(current);
                            currentDirection = dir;
                            foundNext = true;
                            break;
                        }
                    }
                    if (foundNext) break;
                }
            }

            if (!foundNext)
                break;
        }

        return orderedPath;
    }

    private Vector2 GetTopLeftMost(HashSet<Vector2> points)
    {
        Vector2 topLeft = new Vector2(float.MaxValue, float.MinValue);
        foreach (Vector2 p in points)
        {
            if (p.x < topLeft.x || (p.x == topLeft.x && p.y > topLeft.y))
                topLeft = p;
        }
        return topLeft;
    }

    private List<Vector2> SimplifyPath(List<Vector2> path)
    {
        if (path.Count < 3)
            return new List<Vector2>(path);

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
    }

    private bool AreCollinear(Vector2 a, Vector2 b, Vector2 c)
    {
        float area = (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
        return Mathf.Abs(area) < 0.001f;
    }





















    public List<List<Vector2>> OrganizeIntoScanLinesWithBreaks(List<Vector2> points)
    {


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
        simpleVerticleFillMod = (int)renderScale;

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
                            uf.Union(current.Index, neighbor.Index);
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







    private List<Vector2> FindMatch(Vector2 point, Dictionary<Vector2, List<Vector2>> endpointMap, float thresholdSquared)
    {
        foreach (var kvp in endpointMap)
        {
            if ((kvp.Key - point).sqrMagnitude <= thresholdSquared)
            {
                return kvp.Value;
            }
        }
        return null;
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



    public List<List<Vector2>> FindConcentrations(List<Vector2> pointsToAnalyze, float radius)
    {
        List<List<Vector2>> concentrations = new List<List<Vector2>>();
        List<Vector2> remainingPoints = new List<Vector2>(pointsToAnalyze); // Copy to avoid modifying original

        while (remainingPoints.Count > 0)
        {
            Vector2 seedPoint = remainingPoints[0]; // Start with the first remaining point
            List<Vector2> currentConcentration = new List<Vector2>();
            currentConcentration.Add(seedPoint);
            remainingPoints.RemoveAt(0);

            List<Vector2> pointsToAdd = new List<Vector2>(); // Points found in the current cluster
            pointsToAdd.Add(seedPoint);

            while (pointsToAdd.Count > 0)
            {
                Vector2 currentPoint = pointsToAdd[0];
                pointsToAdd.RemoveAt(0);

                for (int i = remainingPoints.Count - 1; i >= 0; i--) // Iterate backwards for safe removal
                {
                    if (Vector2.Distance(currentPoint, remainingPoints[i]) <= radius)
                    {
                        currentConcentration.Add(remainingPoints[i]);
                        pointsToAdd.Add(remainingPoints[i]);
                        remainingPoints.RemoveAt(i);
                    }
                }
            }
            concentrations.Add(currentConcentration);
        }

        return concentrations;
    }

    public List<List<Vector2>> SplitPointLists(List<Vector2> points, float threshold)
    {
        List<List<Vector2>> splitLists = new List<List<Vector2>>();

        List<Vector2> curAddList = new List<Vector2>();

        curAddList.Add(points[0]);

        for (int i = 1; i < points.Count; i++)
        {
            if (Vector2.Distance(points[i], points [i - 1]) > renderScale * 2)
            {
                splitLists.Add(curAddList);

                curAddList = new List<Vector2>();

                curAddList.Add(points[i]);

                //new line
            }
            else
            {
                curAddList.Add(points[i]);
            }
        }



        splitLists.Add(curAddList);







        //if (points == null || points.Count == 0)
        //{
        //    return splitLists; // Return empty list if input is null or empty
        //}

        //List<Vector2> currentList = new List<Vector2>();
        //currentList.Add(points[0]); // Add the first point to the first list
        //splitLists.Add(currentList);

        //for (int i = 1; i < points.Count; i++)
        //{
        //    bool connected = false;
        //    foreach (Vector2 pointInCurrentList in currentList)
        //    {
        //        if (Vector2.Distance(points[i], pointInCurrentList) <= threshold)
        //        {
        //            connected = true;
        //            break;
        //        }
        //    }

        //    if (connected)
        //    {
        //        currentList.Add(points[i]);
        //    }
        //    else
        //    {
        //        currentList = new List<Vector2>(); // Start a new list
        //        currentList.Add(points[i]);
        //        splitLists.Add(currentList);
        //    }
        //}

        return splitLists;
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

                PlacePath(lineWidth, paintSpot, i, svgParent, new Color(10, 10, 10));
            }

            curLineCount++;
            PlacePath(lineWidth, pointsThisPath, i, svgParent, new Color(10, 10, 10));
        }


        if (drawSpawnShapes)
        {
            PlacePath(lineWidth, drawShapeSpawnPoints, pathCount, svgParent, new Color(10, 10, 10));
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
                PlacePath(lineWidth, boarderPoints, pathCount + 1, svgParent, new Color(10, 10, 10));
            }
            else
            {
                PlacePath(lineWidth, boarderPoints, pathCount, svgParent, new Color(10, 10, 10));
            }
        }


        listOfAllPathsThisRun.Add(listOfPaths);

        listsOfAllPathsByColor[printIndex % potentialColors.Count].AddRange(listOfPaths); // using System.Linq;


        GenerateSVG(listsOfAllPathsByColor[printIndex % potentialColors.Count], false, false, plotColors.IndexOf(plotColors[printIndex]), plotColors[printIndex], svgSize);
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
    public void PlacePath(float width, List<Vector2> points, int IDnum, Transform placeToPlace, Color col)
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

        lineObjects[lineObjects.Count - 1].positionCount = points.Count;
        lineObjects[lineObjects.Count - 1].SetPositions(realPoints.ToArray());
        lineObjects[lineObjects.Count - 1].useWorldSpace = false;
    }

    [ContextMenu("SaveSVG")]
    public void GenerateSVG(List<List<Vector2>> allPaths, bool saveDisplayCopy, bool isInfoPage, int printColorIndex, Color drawColor, Vector2 svgFileSize)
    {
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
            svgContent.AppendLine($"   style=\"fill:#{ColorUtility.ToHtmlStringRGB(bgColor)};stroke-width:0;stroke-opacity:0\"");
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

        if (!Directory.Exists($"Z:\\Shit\\SVG Stuff\\SVG-Stuff\\Assets\\Resources\\{yourFileName}"))
        {
            AssetDatabase.CreateFolder("Assets/Resources", yourFileName);
        }


        string desktopPath = $"Z:\\Shit\\SVG Stuff\\SVG-Stuff\\Assets\\Resources\\{yourFileName}";
        desktopPath += "\\";

        string filePath = "";

        if (!saveDisplayCopy)
        {
            filePath = Path.Combine(desktopPath, yourFileName + printColorIndex.ToString() + ".txt");
            filePath = Path.Combine(desktopPath, yourFileName + printColorIndex.ToString() + ".svg");
        }
        else
        {
            filePath = Path.Combine(desktopPath, yourFileName + printColorIndex.ToString() + " Display.svg");
            ScreenCapture.CaptureScreenshot(desktopPath + yourFileName + " Screenshot.png");
        }

        if (isInfoPage)
        {
            filePath = Path.Combine(desktopPath, yourFileName + "Info.txt");
        }


        // Write the SVG content to a file
        File.WriteAllText(filePath, svgContent.ToString());

        //Debug.Log("Done");
    }

    public void GenerateDispalySVG(List<List<List<Vector2>>> allPathsToDisplay)
    {
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