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

            //lr.material
        }
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
                if (randomOffsetFromCenter != 0)
                {
                    randomOffsetFromCenter = chosenSize - ((chosenSize * (1f / Mathf.Sqrt(2))));
                }

                chosenSize *= 1f / Mathf.Sqrt(2);

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

            //Vector3 startingPos = new Vector3(UnityEngine.Random.Range(0, svgSize.x), UnityEngine.Random.Range(0, svgSize.y), 0);

            Vector2 curPos = startingPos;

            Vector3 realFakePos = new Vector3(((svgSize / 2f) + additionalSpawnOffset).x, ((svgSize / 2f) + additionalSpawnOffset).y, 0);

            Vector3 dirFromCenter = startingPos - realFakePos;

            dirFromCenter.Normalize();

            pointsThisPath.Add(startingPos);



            //generate segments
            //for (int j = 0; j < UnityEngine.Random.Range(100, 250); j++)
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

        if (drawBounds)
        {
            //place boarder
            List<Vector2> boarderPoints = new List<Vector2>();

            //bottom
            boarderPoints.Add(new Vector3(0 + clippingSize.x, 0 + clippingSize.y, 0));
            boarderPoints.Add(new Vector3(svgSize.x - clippingSize.x, 0 + clippingSize.y, 0));
            boarderPoints.Add(new Vector3(svgSize.x - clippingSize.x, svgSize.y - clippingSize.y, 0));
            boarderPoints.Add(new Vector3(0 + clippingSize.x, svgSize.y - clippingSize.y, 0));
            boarderPoints.Add(new Vector3(0 + clippingSize.x, 0 + clippingSize.y, 0));

            if (dipInPaint)
            {
                PlacePath(lineWidth, paintSpot, pathCount, svgParent);

                PlacePath(lineWidth, boarderPoints, pathCount + 1, svgParent);
            }
            else
            {
                PlacePath(lineWidth, boarderPoints, pathCount, svgParent);
            }
        }


        if (drawSpawnShapes)
        {
            if (dipInPaint)
            {
                PlacePath(lineWidth, paintSpot, pathCount + 4, svgParent);

                PlacePath(lineWidth, drawShapeSpawnPoints, pathCount + 5, svgParent);
            }
            else
            {
                PlacePath(lineWidth, drawShapeSpawnPoints, pathCount + 1, svgParent);
            }


        }

        listOfAllPathsThisRun.Add(listOfPaths);

        listsOfAllPathsByColor[printIndex % potentialColors.Count].AddRange(listOfPaths); // using System.Linq;




        GenerateSVG(listsOfAllPathsByColor[printIndex % potentialColors.Count], false, false, plotColors.IndexOf(plotColors[printIndex]), plotColors[printIndex]);


        //if (printIndex < plotColors.Count -1)
        //{
        //    if (plotColors[printIndex] != plotColors[printIndex + 1])
        //    {
        //        List<List<Vector2>> pathListThisColor = new List<List<Vector2>>();

        //        foreach (List<List<Vector2>> pathList in listOfAllPathsThisColorChange)
        //        {
        //            foreach(List<Vector2> pointList in pathList)
        //            {
        //                pathListThisColor.Add(pointList);
        //            }
        //        }

        //        GenerateSVG(pathListThisColor, false, false, printIndex, plotColors[printIndex]);

        //        listOfAllPathsThisColorChange = new List<List<List<Vector2>>>();
        //    }
        //    else
        //    {
        //        resetLineListThisShape = false;
        //    }
        //}
        //else
        //{
        //    GenerateSVG(listOfPaths, false, false, printIndex, plotColors[printIndex]);
        //}




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

        if (svgParent.childCount <= IDnum)
        {

        }

        GameObject newLine = new GameObject("line " + IDnum);

        newLine.transform.SetParent(placeToPlace);

        LineRenderer lineRenderer = newLine.AddComponent<LineRenderer>();
        //lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
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
    public void GenerateSVG(List<List<Vector2>> allPaths, bool saveDisplayCopy, bool isInfoPage, int printColorIndex, Color drawColor)
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
                if (new Vector2(allPaths[i][0].x, allPaths[i][0].y) != new Vector2(allPaths[i][1].x, allPaths[i][1].y))
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
            filePath = Path.Combine(desktopPath, yourFileName + "Info.svg");
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