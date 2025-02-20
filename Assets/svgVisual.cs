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

    public Camera renderCam;

    public float renderScale = 4;

    public Burster burster;

    List<Color> plottedColors = new List<Color>();

    [ContextMenu("SetRenderValues")]
    public void SetRenderValues(List<LineRenderer> lineRenderersToSet, Material matToCopy, Color colorToSet, bool resetLinePositions = false)
    {
        svgParent.position = new Vector3(-(svgSize.x / 2f), -(svgSize.y / 2f), 0);

        Camera.main.orthographicSize = svgSize.y / 2f;

        bgRenderer.material = bgMat;

        bgRenderer.material.SetTexture("_MainTex", paperTextures[paperIndex]);
        bgRenderer.material.SetColor("_BaseColor", bgColor);

        Material lineMat = new Material(matToCopy);

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


        renderCamTexture.Release();
        renderCamTexture.width = (int)((svgSize.x - (clippingSize.x * 2f)) * renderScale);
        renderCamTexture.height = (int)((svgSize.y - (clippingSize.y * 2f)) * renderScale);
        renderCamTexture.Create();


        renderFilterCamTexture.Release();
        renderFilterCamTexture.width = (int)((svgSize.x - (clippingSize.x * 2f)) * renderScale);
        renderFilterCamTexture.height = (int)((svgSize.y - (clippingSize.y * 2f)) * renderScale);
        renderFilterCamTexture.Create();

        //renderCamTexture.width = (int)(svgSize.x - (clippingSize.x * 2f));
        //renderCamTexture.height = (int)(svgSize.y - (clippingSize.y * 2f));


        //Replace 'Generate Work'



        SetRenderValues(lineObjects, unlitMat, Color.black, true);

        //oldPlots.Add(Instantiate(svgParent.gameObject));

        //oldPlots[0].SetActive(true);


        //GenerateDispalySVG(listOfAllPathsThisRun);

        //textImporter.CompileInfoPage();

        if (isFinalRender)
        {
            urlsaver.artName = piecename;
            urlsaver.shortURL = shortURL;

            urlsaver.SubmitFeedback();
        }
    }


    [ContextMenu("GenerateRTWork")]
    public void GenerateRTWork()
    {
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


        Texture2D wholeRenderTexHolder = new Texture2D(renderCamTexture.width, renderCamTexture.height, TextureFormat.RGBA32, false, true);

        //wholeRenderTexHolder.

        RenderTexture.active = renderCamTexture;

        wholeRenderTexHolder.ReadPixels(new Rect(0, 0, renderCamTexture.width, renderCamTexture.height), 0, 0);
        wholeRenderTexHolder.Apply(); // Apply the changes to the Texture2D


        RenderTexture.active = null;



        //Graphics.CopyTexture(renderCamTexture, wholeRenderTexHolder);

        //wholeRenderTexHolder.CopyPixels(renderCamTexture.graphicsTexture);

        //wholeRenderTexHolder.Apply();



        List<Color> diffColors = new List<Color>();


        foreach(Color color in wholeRenderTexHolder.GetPixels())
        {


            if (!diffColors.Contains(color))
            {
                diffColors.Add(color);
                //Debug.Log(color);
            }
        }

        List<List<Vector2>> claimedPixels = new List<List<Vector2>>();

            
        for (int i = 0; i < diffColors.Count; i++)
        {
            //Loop thru the texture for each color. If it's 



            Color thisColor = diffColors[i];
            claimedPixels.Add(new List<Vector2>());


            Color[] pixelArray = wholeRenderTexHolder.GetPixels();

            List<Vector2> listlistlist = burster.HandleJob(pixelArray, thisColor, wholeRenderTexHolder.width);

            claimedPixels[i] = listlistlist;

            //burster.HandleJob(pixelArray, thisColor, wholeRenderTexHolder.width);






            //for (int y = 0; y < pixelArray.Length; y++)
            //{
            //    if (thisColor.Equals(pixelArray[y]))
            //    {
            //        claimedPixels[i].Add(new Vector2(y % wholeRenderTexHolder.width, Mathf.Floor(y / wholeRenderTexHolder.width)));
            //    }
            //}










            //    //Go thru image and get bounds of areas of color
            //for (int y = 0; y < renderCamTexture.height; y++)
            //{
            //    for (int x = 0; x < renderCamTexture.width; x++)
            //    {
            //        //renderTexHolder = new Texture2D(1, 1, TextureFormat.RGBA32, false, true);

            //        // copy the single pixel value from the render texture to the texture2D on the GPU
            //        //Graphics.CopyTexture(renderCamTexture, 0, 0, x, y, 1, 1, renderTexHolder, 0, 0, 0, 0);

            //        Color pixelColor = wholeRenderTexHolder.GetPixel(x, y);

            //        if (thisColor == pixelColor)
            //        {
            //            claimedPixels[i].Add(new Vector2(x,y));
            //        }
            //    }
            //}

        }

        //claimedPixels.Sort((x, y) => x.Count.CompareTo(y.Count));

        Debug.Log(diffColors.Count);

        int colorCountToRemove = 0;

        for (int i = 0; i < diffColors.Count; i++)
        {
            //Debug.Log(i +" " + claimedPixels[i].Count + "   " + diffColors[i]);



            Vector3 thisColorVector = new Vector3(diffColors[i].r, diffColors[i].g, diffColors[i].b);

            bool hasBeenReplaced = false;

            for (int k = 0; k < diffColors.Count; k++)
            {
                if (claimedPixels[k].Count > 10000)
                {
                    Vector3 replacementColor = new Vector3(diffColors[k].r, diffColors[k].g, diffColors[k].b);


                    if (Vector3.Distance(thisColorVector, replacementColor) < 0.025f && thisColorVector != replacementColor && hasBeenReplaced == false)
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



            //If there are not many pixels of this color, move them to the next closest color
           
        }

        Debug.Log("Removed " + colorCountToRemove);

        for (int i = 0; i < claimedPixels.Count; i++)
        {
            if (claimedPixels[i] == new List<Vector2>())
            {
                //claimedPixels.Remove(claimedPixels[i]);
            }


        }


        //for (int i = 0; i < claimedPixels.Count; i++)
        //{
        //    Debug.Log(claimedPixels[i].Count +  " " + diffColors[i]);

        //    //If there are not many pixels of this color, move them to the next closest color
        //    if (claimedPixels[i].Count < 10000)
        //    {
        //        colorCountToRemove++;

        //        //find closest color
        //        Vector3 thisColorVector = new Vector3(diffColors[i].r, diffColors[i].g, diffColors[i].b);

        //        float closestDist = 1000;

        //        int replacementColorIndex = 0;

        //        for (int k = 0; k < diffColors.Count; k++)
        //        {
        //            if (k != i)
        //            {
        //                Vector3 replacementColor = new Vector3(diffColors[k].r, diffColors[k].g, diffColors[k].b);

        //                float colorDist = Vector3.Distance(thisColorVector, replacementColor);

        //                if (colorDist < closestDist)
        //                {
        //                    replacementColorIndex = k;

        //                    closestDist = colorDist;
        //                }
        //            }

        //            claimedPixels[k].AddRange(claimedPixels[i]);
        //        }


        //    }
        //}

        //Debug.Log("Removed "  + colorCountToRemove);

        //claimedPixels.RemoveRange(0, colorCountToRemove);






        //Draw bounds around Colors based on claimed pixels

        List<List<Vector2>> edgePointsPerColor = new List<List<Vector2>>();

        Vector2 curScanPosition = new Vector2();

        for (int i = 0; i < claimedPixels.Count; i++)
        {
            edgePointsPerColor.Add(new List<Vector2>());

            if (claimedPixels[i].Count > 0)
            {
                curScanPosition = claimedPixels[i][0];

                //Line Segments
                for (int q = 0; q < claimedPixels[i].Count; q++)
                {
                    curScanPosition = claimedPixels[i][q];



                    Color col = diffColors[i]; wholeRenderTexHolder.GetPixel((int)curScanPosition.x, (int)curScanPosition.y);

                    //if the pixel to my right is a different color I am an edge, so add to edge point list
                    //color check
                    if (col != wholeRenderTexHolder.GetPixel((int)curScanPosition.x + 1, (int)curScanPosition.y))
                    {
                        edgePointsPerColor[i].Add(curScanPosition);
                    }

                    if (col != wholeRenderTexHolder.GetPixel((int)curScanPosition.x - 1, (int)curScanPosition.y))
                    {
                        edgePointsPerColor[i].Add(curScanPosition);
                    }

                    if (col != wholeRenderTexHolder.GetPixel((int)curScanPosition.x, (int)curScanPosition.y + 1))
                    {
                        edgePointsPerColor[i].Add(curScanPosition);
                    }

                    if (col != wholeRenderTexHolder.GetPixel((int)curScanPosition.x, (int)curScanPosition.y - 1))
                    {
                        edgePointsPerColor[i].Add(curScanPosition);
                    }
                }
            }

           



        }


        List<List<Vector2>> outlinesToAddSplitUpColor = new List<List<Vector2>>();

        for (int i = 0; i < edgePointsPerColor.Count; i++)
        {
            outlinesToAddSplitUpColor.AddRange(FindConcentrations(edgePointsPerColor[i], 1.5f));

        }


        //outlinesToAddSplitUpColor = edgePointsPerColor;

        //split up color areas into islands



        //linesToAddThisColor.Add(new List<Vector2>());

        List<List<Vector2>> outlinesToAddSplitUpColorAndOutlineFix = new List<List<Vector2>>();

        for (int i = 0; i < outlinesToAddSplitUpColor.Count; i++)
        {
            //outlinesToAddSplitUpColor[i] = SimplifyPoints(outlinesToAddSplitUpColor[i], 20);


            outlinesToAddSplitUpColor[i] = SortClockwise(outlinesToAddSplitUpColor[i]);

            //Sort By closest //Fucked up
            //outlinesToAddSplitUpColor[i] = SortPoints(outlinesToAddSplitUpColor[i]);

            List<Vector2> sortedList = SortPoints(outlinesToAddSplitUpColor[i]);

            List<List<Vector2>> sortedSplitList = new List<List<Vector2>>();




            List<Vector2> tempPointList = new List<Vector2>();

            for (int v = 0; v < sortedList.Count - 1; v++)
            {
                tempPointList.Add(sortedList[v]);

                //If this one and the next one are far apart split that boy
                if (Vector2.Distance(sortedList[v], sortedList[v + 1]) > 5)
                {
                    if (Vector2.Distance(tempPointList[tempPointList.Count - 1], tempPointList[0]) < 5)
                    {
                        tempPointList.Add(tempPointList[0]);
                    }



                    sortedSplitList.Add(tempPointList);

                    tempPointList = new List<Vector2>();
                }
            }

            if (tempPointList.Count > 1)
            {
                if (Vector2.Distance(tempPointList[tempPointList.Count - 1], tempPointList[0]) < 5)
                {
                    tempPointList.Add(tempPointList[0]);
                }
            }

            sortedSplitList.Add(tempPointList);

            //sortedList = new List<Vector2>();

            foreach (List<Vector2> lv2 in sortedSplitList)
            {
                float widthOfLineSegment = 0;
                float minX = 100000000;
                float maxX = -10000000;

                foreach (Vector2 lv in lv2)
                {
                    if (lv.x < minX)
                    {
                        minX = lv.x;
                    }

                    if (lv.x > maxX)
                    {
                        maxX = lv.x;
                    }
                }

                widthOfLineSegment = minX - maxX;


                if (Mathf.Abs(widthOfLineSegment) > 1)
                {
                    outlinesToAddSplitUpColorAndOutlineFix.Add(lv2);
                }



                //sortedList.AddRange(lv2);
            }




        }

        List<List<Vector2>> finalLineList = new List<List<Vector2>>();

        for (int i = 0; i < outlinesToAddSplitUpColorAndOutlineFix.Count; i++)
        {
            List<Vector2> scaledPath = new List<Vector2>();


            for (int u = 0; u < outlinesToAddSplitUpColorAndOutlineFix[i].Count; u++)
            {
                scaledPath.Add(outlinesToAddSplitUpColorAndOutlineFix[i][u] * (1f / renderScale));
            }

            finalLineList.Add(scaledPath);
            PlacePath(1, scaledPath, i, transform);
        }

        //place boarder
        List<Vector2> boarderPoints = new List<Vector2>();

        //bottom
        boarderPoints.Add(new Vector3(0 , 0, 0));
        boarderPoints.Add(new Vector3(renderCamTexture.width * (1f / renderScale), 0, 0));
        boarderPoints.Add(new Vector3(renderCamTexture.width * (1f / renderScale), renderCamTexture.height * (1f / renderScale), 0));
        boarderPoints.Add(new Vector3(0, renderCamTexture.height * (1f / renderScale), 0));
        boarderPoints.Add(new Vector3(0, 0, 0));


        //boarderPoints[0] += new Vector2(-48, -48);
        //boarderPoints[1] += new Vector2(-48, -48);
        //boarderPoints[2] += new Vector2(-48, -48);
        //boarderPoints[3] += new Vector2(-48, -48);
        //boarderPoints[4] += new Vector2(-48, -48);

        PlacePath(lineWidth, boarderPoints, 0, transform);

        finalLineList.Add(boarderPoints);


        SetRenderValues(lineObjects, unlitMat, Color.black, true);


        for (int i = 0; i < finalLineList.Count; i++)
        {
            for (int u = 0; u < finalLineList[i].Count; u++)
            {
                finalLineList[i][u] += new Vector2(clippingSize.x, clippingSize.y);
            }
        }


        GenerateSVG(finalLineList,false,false,0,Color.black, svgSize);



        //for (int i = 0; i < pathCount; i++)
        //{
        //    bool addCurPosition = true;

        //    List<Vector2> pointsThisPath = new List<Vector2>();

        //    Vector3 startingPos = Vector3.zero;

        //    Vector2 lastPos = startingPos;

        //    Vector2 curPos = startingPos;

        //    Vector3 realFakePos = new Vector3(((svgSize / 2f) + additionalSpawnOffset).x, ((svgSize / 2f) + additionalSpawnOffset).y, 0);

        //    pointsThisPath.Add(startingPos);


        //    //generate segments
        //    for (int j = 0; j < pathLength; j++)
        //    {
        //        Vector2 moveDir = Vector2.zero;

        //        if (curPos.x <= 0 + clippingSize.x || curPos.x >= svgSize.x - clippingSize.x || curPos.y <= 0 + clippingSize.y || curPos.y >= svgSize.y - clippingSize.y)
        //        {
        //            addCurPosition = false;
        //        }
        //        else
        //        {
        //            addCurPosition = true;
        //        }


        //        for (int k = 0; k < flowFieldPositions.Count; k++)
        //        {
        //            Vector2 thisNodesMovement = flowFieldDirections[k];

        //            thisNodesMovement *= Mathf.Clamp((maxInfluenceDist - Vector2.Distance(curPos, flowFieldPositions[k])), 0, maxInfluenceDist) / maxInfluenceDist;

        //            moveDir += thisNodesMovement;
        //        }


        //        curPos += ((moveDir * flowFieldMovementMulti) + constantMoveValue);


        //        Vector2 finalDir = curPos - lastPos;

        //        Vector2 realFinalPos = lastPos;

        //        int asdasdasd = j;

        //        if (curPos.x <= 0 + clippingSize.x && lastPos.x > 0 + clippingSize.x)
        //        {
        //            j = pathLength - 1;

        //            while (realFinalPos.x > clippingSize.x && realFinalPos.y < svgSize.y - clippingSize.y && realFinalPos.y > clippingSize.y)
        //            {
        //                realFinalPos += finalDir.normalized * 0.1f;
        //            }

        //            curPos = realFinalPos;
        //        }

        //        if (curPos.x >= svgSize.x - clippingSize.x && lastPos.x < svgSize.x - clippingSize.x)
        //        {
        //            j = pathLength - 1;

        //            while (realFinalPos.x < svgSize.x - clippingSize.x && realFinalPos.y < svgSize.y - clippingSize.y && realFinalPos.y > clippingSize.y)
        //            {
        //                realFinalPos += finalDir.normalized * 0.1f;
        //            }

        //            curPos = realFinalPos;
        //        }




        //        if (curPos.y <= 0 + clippingSize.y)
        //        {
        //            j = pathLength - 1;

        //            while (realFinalPos.y > clippingSize.y)
        //            {
        //                realFinalPos += finalDir.normalized * 0.1f;
        //            }

        //            curPos = realFinalPos;
        //        }

        //        if (curPos.y >= svgSize.y - clippingSize.y)
        //        {
        //            j = pathLength - 1;

        //            while (realFinalPos.y < svgSize.y - clippingSize.y)
        //            {
        //                realFinalPos += finalDir.normalized * 0.1f;
        //            }

        //            curPos = realFinalPos;
        //        }


        //        lastPos = curPos;

        //        if (addCurPosition)
        //        {
        //            pointsThisPath.Add(curPos);
        //        }
        //    }


        //    if (dipInPaint)
        //    {
        //        curLineCount++;

        //        PlacePath(lineWidth, paintSpot, i, svgParent);
        //    }

        //    curLineCount++;
        //    PlacePath(lineWidth, pointsThisPath, i, svgParent);
        //}



        //if (drawBounds)
        //{
        //    //place boarder
        //    List<Vector2> boarderPoints = new List<Vector2>();

        //    //bottom
        //    boarderPoints.Add(new Vector3(0 + clippingSize.x, 0 + clippingSize.y, 0));
        //    boarderPoints.Add(new Vector3(svgSize.x - clippingSize.x, 0 + clippingSize.y, 0));
        //    boarderPoints.Add(new Vector3(svgSize.x - clippingSize.x, svgSize.y - clippingSize.y, 0));
        //    boarderPoints.Add(new Vector3(0 + clippingSize.x, svgSize.y - clippingSize.y, 0));
        //    boarderPoints.Add(new Vector3(0 + clippingSize.x, 0 + clippingSize.y, 0));

        //    if (drawSpawnShapes)
        //    {
        //        PlacePath(lineWidth, boarderPoints, pathCount + 1, svgParent);
        //    }
        //    else
        //    {
        //        PlacePath(lineWidth, boarderPoints, pathCount, svgParent);
        //    }
        //}


        //listOfAllPathsThisRun.Add(listOfPaths);


        //GenerateSVG(listOfPaths, false, false, 0, Color.black, svgSize);
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

        if (points == null || points.Count == 0)
        {
            return splitLists; // Return empty list if input is null or empty
        }

        List<Vector2> currentList = new List<Vector2>();
        currentList.Add(points[0]); // Add the first point to the first list
        splitLists.Add(currentList);

        for (int i = 1; i < points.Count; i++)
        {
            bool connected = false;
            foreach (Vector2 pointInCurrentList in currentList)
            {
                if (Vector2.Distance(points[i], pointInCurrentList) <= threshold)
                {
                    connected = true;
                    break;
                }
            }

            if (connected)
            {
                currentList.Add(points[i]);
            }
            else
            {
                currentList = new List<Vector2>(); // Start a new list
                currentList.Add(points[i]);
                splitLists.Add(currentList);
            }
        }

        return splitLists;
    }

    public List<Vector2> SortClockwise(List<Vector2> pointsToSort)
    {
        if (pointsToSort == null || pointsToSort.Count < 3)
        {
            Debug.LogWarning("Cannot sort less than 3 points.");
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

        int curFailCount = 1;

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
                float multi = 0.01f;

                searchOffset = (currentPoint - lastPoint) * (curFailCount * multi);
            }


            Vector2 nextPoint = FindNextClosestPoint(currentPoint + searchOffset, remainingPoints);


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





















    public List<Vector2> SortFuckedUp(List<Vector2> pointsToSort)
    {
        if (pointsToSort == null || pointsToSort.Count < 3)
        {
            Debug.LogWarning("Cannot sort less than 3 points.");
            return pointsToSort; // Return original if less than 3 points or null
        }

        var pointsToSortCopy = pointsToSort;

        List<Vector2> claimedPoints = new List<Vector2>();

        List<Vector2> sortedPoints = new List<Vector2>();

        sortedPoints.Add(pointsToSortCopy[0]);

        Vector2 curPoint = pointsToSortCopy[0];

        for (int i = 0; i < pointsToSort.Count; i++)
        {
            Vector2 targetVector = pointsToSortCopy[0]; // Your target vector

            List<Vector2> vectorList = pointsToSortCopy; // Your list of vectors



            Vector2 closestVector = vectorList[0]; // Initialize with the first vector

            float closestDistance = Mathf.Infinity;



            foreach (Vector2 vec in vectorList)

            {

                float distance = Vector2.Distance(targetVector, vec);

                if (distance < closestDistance)

                {

                    closestDistance = distance;

                    closestVector = vec;

                }

            }

            sortedPoints.Add(closestVector);

            pointsToSortCopy.Remove(closestVector);
        }









        return sortedPoints;
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

                PlacePath(lineWidth, paintSpot, i, svgParent);
            }

            curLineCount++;
            PlacePath(lineWidth, pointsThisPath, i, svgParent);
        }


        if (drawSpawnShapes)
        {
            PlacePath(lineWidth, drawShapeSpawnPoints, pathCount, svgParent);
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
                PlacePath(lineWidth, boarderPoints, pathCount + 1, svgParent);
            }
            else
            {
                PlacePath(lineWidth, boarderPoints, pathCount, svgParent);
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
    public void PlacePath(float width, List<Vector2> points, int IDnum, Transform placeToPlace)
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

        lineRenderer.widthMultiplier = width;

        lineObjects.Add(lineRenderer);

        if (lineObjects[IDnum])
        {
            lineObjects[IDnum].positionCount = points.Count;
            lineObjects[IDnum].SetPositions(realPoints.ToArray());
            lineObjects[IDnum].useWorldSpace = false;
        }
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
            drawLineColor = Color.black;
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

                svgContent.AppendLine(thisPath);
                svgContent.AppendLine($"      id = \"{i}\" />");

                Debug.Log(i + " ID");



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

        Debug.Log("Done");
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