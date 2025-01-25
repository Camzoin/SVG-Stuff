using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System;

public class svgVisual : MonoBehaviour
{
    public Transform svgParent;

    public List<List<Vector2>> listOfPaths = new List<List<Vector2>>();

    private List<LineRenderer> lineObjects = new List<LineRenderer>();

    public Vector2 svgSize = new Vector2(816, 1056);

    public Texture2D inputTexture;

    public float brightnessScaler = 1;

    public float flowFieldMovementMulti = 2;

    public List<Vector2> flowFieldPositions = new List<Vector2>();

    public List<Vector2> flowFieldDirections = new List<Vector2>();

    public int pathCount = 5000;

    public Vector2 circleRadius;

    public int spawnCircleVertCount = 36000;

    public Vector2 constantSpeedMinMax = new Vector2(0.1f, 5f);

    public int pathLength = 250;

    public int flowFieldNodeCount = 400;

    public float maxInfluenceDist = 400;

    public string yourFileName = "yourFileName";

    public Vector2 clippingSize = new Vector2(0,0);

    public bool drawBounds = true;

    public bool drawSpawnShapes = true;

    public bool dipInPaint = false;

    public List<int> posibleShapeVertCount = new List<int>();

    [SerializeField] private ComputeShader flowFieldCalcShader = null;

    private ComputeBuffer flowFieldNodePostions;
    private ComputeBuffer flowFieldNodeDirections;
    private ComputeBuffer outputMovement;
    private Vector2[] shaderBasedMovement = new Vector2[1];

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

    public Color bgColor;

    public Color lineColor;

    public Material unlitMat;

    public Material textMat;

    public Material bgMat;

    public List<Texture2D> paperTextures;

    public int paperIndex;

    public Renderer bgRenderer;

    public bool incrementFileName = true;

    public bool keepPreviousWorkVisible = false;

    public float lineWidth = 0.5f;

    public string versionNumber;

    public int seedValue;

    public string generatorName;

    public string artistName;

    public string recipientName;

    public SVGTextImporter textImporter;

    public bool generateArtWithNewSeed = false;

    public string fileURL = "";

    public string shortURL = "";

    public string piecename = "";

    [ContextMenu("SetRenderValues")]
    public void SetRenderValues(List<LineRenderer> lineRenderersToSet, Material matToCopy, bool resetLinePositions = false)
    {
        svgParent.position = new Vector3(-(svgSize.x / 2f), -(svgSize.y / 2f), 0);

        Camera.main.orthographicSize = svgSize.y / 2f;

        bgRenderer.material = bgMat;

        bgRenderer.material.SetTexture("_MainTex", paperTextures[paperIndex]);
        bgRenderer.material.SetColor("_BaseColor", bgColor);

        Material lineMat = new Material(matToCopy);

        lineMat.SetColor("_BaseColor", lineColor);

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
        seedValue = UnityEngine.Random.Range(-(int.MaxValue / 2), int.MaxValue / 2);

        UnityEngine.Random.InitState(seedValue);

        piecename = textImporter.GenerateRandomName();

        yourFileName = piecename.Replace(" ", "");

        GenerateFlowField();
        ChangeConsistentFlow();
        GenerateWork();
        SetRenderValues(lineObjects, unlitMat, true);

        textImporter.CompileInfoPage();

        if (incrementFileName)
        {
            yourFileName += "x";
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
            flowFieldPositions.Add(new Vector2(UnityEngine.Random.Range(0- extraSpace, svgSize.x+ extraSpace), UnityEngine.Random.Range(0- extraSpace, svgSize.y+ extraSpace)));

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
    public void GenerateWork()
    {
        shaderBasedMovement[0] = Vector2.zero;

        //do compute shader shit
        flowFieldNodePostions = CreateComputeBuffer<Vector2>(flowFieldPositions.ToArray(), flowFieldCalcShader, "flowFieldPositions", 0);
        flowFieldNodeDirections = CreateComputeBuffer<Vector2>(flowFieldDirections.ToArray(), flowFieldCalcShader, "flowFieldDirections", 0);
        outputMovement = CreateComputeBuffer<Vector2>(shaderBasedMovement, flowFieldCalcShader, "finalMovement", 0);

        flowFieldCalcShader.SetFloat("maxInfluenceDist", maxInfluenceDist);
        flowFieldCalcShader.SetInt("flowFieldNodeCount", flowFieldPositions.Count);

        

        float spawnOffsetX = ((svgSize.x / 2f) * UnityEngine.Random.Range(-randomOffsetFromCenter, randomOffsetFromCenter)) + additionalSpawnOffset.x;
        float spawnOffsetY = ((svgSize.y / 2f) * UnityEngine.Random.Range(-randomOffsetFromCenter, randomOffsetFromCenter)) + additionalSpawnOffset.y;


        float spawnRad = UnityEngine.Random.Range(circleRadius.x, circleRadius.y);

        //pathCount = (int)spawnRad / 2;



        Vector2 spawnOffset = new Vector2(spawnOffsetX, spawnOffsetY);

        float fakeRot = shapeRotation * Mathf.Deg2Rad;

        if (useRandomRoation)
        {
            float randomRot = UnityEngine.Random.Range(0, 360f);

            fakeRot = randomRot * Mathf.Deg2Rad;

            shapeRotation = randomRot;
        }

        //List<Vector2> circleSpawnPoints = CalculateCirclePoints(spawnCircleVertCount, svgSize / 2f, circleRadius);
        List<Vector2> circleSpawnPoints = FindPointsOnSimpleShape(spawnCircleVertCount, posibleShapeVertCount[UnityEngine.Random.Range(0, posibleShapeVertCount.Count)], (svgSize / 2f) + spawnOffset, spawnRad, fakeRot);

        //get paint
        List<Vector2> paintSpot = new List<Vector2>();

        //bottom
        paintSpot.Add(new Vector3(svgSize.x - 1, 1, 0));
        paintSpot.Add(new Vector3(svgSize.x - 1, 3, 0));
        paintSpot.Add(new Vector3(svgSize.x - 1, 1, 0));
        paintSpot.Add(new Vector3(svgSize.x - 1, 3, 0));

        if (keepPreviousWorkVisible)
        {
            GameObject.Instantiate(svgParent.gameObject);
        }


        listOfPaths = new List<List<Vector2>>();

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
            List<Vector2> pointsThisPath = new List<Vector2>();

            Vector3 startingPos = circleSpawnPoints[(int)((i / (float)pathCount) * circleSpawnPoints.Count)];

            Vector2 lastPos = startingPos;

            //Vector3 startingPos = new Vector3(UnityEngine.Random.Range(0, svgSize.x), UnityEngine.Random.Range(0, svgSize.y), 0);

            Vector2 curPos = startingPos;

            Vector3 realFakePos =  new Vector3(((svgSize / 2f) + spawnOffset).x, ((svgSize / 2f) + spawnOffset).y,0);

            Vector3 dirFromCenter = startingPos - realFakePos;

            dirFromCenter.Normalize();

            pointsThisPath.Add(startingPos);

            //generate segments
            //for (int j = 0; j < UnityEngine.Random.Range(100, 250); j++)
            for (int j = 0; j < pathLength; j++)
            {
                Vector2 moveDir = Vector2.zero;

                flowFieldCalcShader.SetVector("curPosition", curPos);

                int kernel = flowFieldCalcShader.FindKernel("FlowField");

                //ParticleDispatch(flowFieldCalcShader, kernel, flowFieldPositions.Count, 1);

                outputMovement.GetData(shaderBasedMovement);

                //Debug.Log(shaderBasedMovement[0]);

                for (int k = 0; k < flowFieldPositions.Count; k++)
                {
                    Vector2 thisNodesMovement = flowFieldDirections[k];

                    thisNodesMovement *= Mathf.Clamp((maxInfluenceDist - Vector2.Distance(curPos, flowFieldPositions[k])), 0, maxInfluenceDist)  / maxInfluenceDist;

                    moveDir += thisNodesMovement;
                }


                curPos += (moveDir * flowFieldMovementMulti) + constantMoveValue;



                if (useFlowFromSpawn)
                {
                    curPos += new Vector2(dirFromCenter.x, dirFromCenter.y) * flowFromCenterMulti;
                }

                Vector2 finalDir = curPos - lastPos;

                Vector2 realFinalPos = lastPos;



                if (curPos.x < 0 + clippingSize.x)
                {
                    j = pathLength;

                    while (realFinalPos.x > clippingSize.x && realFinalPos.y < svgSize.y - clippingSize.y && realFinalPos.y > clippingSize.y)
                    {
                        realFinalPos += finalDir.normalized * 0.1f;
                    }

                    curPos = realFinalPos;
                }

                if (curPos.x > svgSize.x - clippingSize.x)
                {
                    j = pathLength;

                    while (realFinalPos.x < svgSize.x - clippingSize.x && realFinalPos.y < svgSize.y - clippingSize.y && realFinalPos.y > clippingSize.y)
                    {
                        realFinalPos += finalDir.normalized * 0.1f;
                    }

                    curPos = realFinalPos;
                }




                if (curPos.y < 0 + clippingSize.y)
                {
                    j = pathLength;

                    while (realFinalPos.y > clippingSize.y)
                    {
                        realFinalPos += finalDir.normalized * 0.1f;
                    }

                    curPos = realFinalPos;
                }

                if (curPos.y > svgSize.y - clippingSize.y)
                {
                    j = pathLength;

                    while (realFinalPos.y < svgSize.y - clippingSize.y)
                    {
                        realFinalPos += finalDir.normalized * 0.1f;
                    }

                    curPos = realFinalPos;
                }





                lastPos = curPos;

                pointsThisPath.Add(curPos);
            }


            if (dipInPaint)
            {
                curLineCount++;

                PlacePath(lineWidth, paintSpot, i);
            }

            curLineCount++;
            PlacePath(lineWidth, pointsThisPath, i);
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
                PlacePath(lineWidth, paintSpot, pathCount);

                PlacePath(lineWidth, boarderPoints, pathCount + 1);

                PlacePath(lineWidth, paintSpot, pathCount + 2);

                PlacePath(lineWidth, boarderPoints, pathCount + 3);
            }
            else
            {
                PlacePath(lineWidth, boarderPoints, pathCount);

                PlacePath(lineWidth, boarderPoints, pathCount + 1);
            }
        }


        if (drawSpawnShapes)
        {
            if (dipInPaint)
            {
                PlacePath(lineWidth, paintSpot, pathCount + 4);

                PlacePath(lineWidth, circleSpawnPoints, pathCount + 5);

                PlacePath(lineWidth, paintSpot, pathCount + 6);

                PlacePath(lineWidth, circleSpawnPoints, pathCount + 7);
            }
            else
            {
                PlacePath(lineWidth, circleSpawnPoints, pathCount + 2);

                PlacePath(lineWidth, circleSpawnPoints, pathCount + 3);
            }


        }

        GenerateSVG(listOfPaths, false, false);
        GenerateSVG(listOfPaths, true, false);
    }

    [ContextMenu("ResetLineObjects")]
    public void ResetLineObjects()
    {
        lineObjects = new List<LineRenderer>();

        foreach (Transform child in svgParent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    [ContextMenu("PlacePath")]
    public void PlacePath(float width, List<Vector2> points, int IDnum)
    {
        listOfPaths.Add(points);

        List<Vector3> realPoints = new List<Vector3>();

        foreach(Vector2 p in points)
        {
            realPoints.Add(new Vector3(p.x, p.y, 0));
        }

        if (svgParent.childCount <= IDnum)
        {
            GameObject newLine = new GameObject("line " + IDnum);

            newLine.transform.SetParent(svgParent);

            LineRenderer lineRenderer = newLine.AddComponent<LineRenderer>();
            //lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.widthMultiplier = width;

            lineObjects.Add(lineRenderer);
        }

        if (lineObjects[IDnum])
        {
            lineObjects[IDnum].positionCount = points.Count;
            lineObjects[IDnum].SetPositions(realPoints.ToArray());
            lineObjects[IDnum].useWorldSpace = false;

        }
    }

    [ContextMenu("SaveSVG")]
    public void GenerateSVG(List<List<Vector2>> allPaths, bool saveDisplayCopy, bool isInfoPage)
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

        Color drawLineColor = lineColor;

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
            svgContent.AppendLine("  <path");
            svgContent.AppendLine($"     style=\"fill:none;stroke:#{ColorUtility.ToHtmlStringRGB(drawLineColor)};stroke-width:{lineWidth / 4};stroke-opacity:1\"");

            string thisPath = "     d = \"M " + allPaths[i][0].x + "," + allPaths[i][0].y + " ";

            for (int j = 1; j < allPaths[i].Count; j++)
            {
                thisPath += allPaths[i][j].x + "," + allPaths[i][j].y + " ";
            }

            thisPath += "\"";

            svgContent.AppendLine(thisPath);
            svgContent.AppendLine($"id = \"{i}\" />");
        }



        // Close the SVG
        svgContent.AppendLine("</svg>");


        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Artwork";
        desktopPath += "\\";

        string filePath = "";

        if (!saveDisplayCopy)
        {
            filePath = Path.Combine(desktopPath, yourFileName + ".svg");
        }
        else
        {
            filePath = Path.Combine(desktopPath, yourFileName + " Display.svg");
            ScreenCapture.CaptureScreenshot(desktopPath + yourFileName + " Screenshot.png");
        }

        if (isInfoPage)
        {
            filePath = Path.Combine(desktopPath, yourFileName + " Info.svg");
        }


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

    internal static ComputeBuffer CreateComputeBuffer<T>(T[] data, ComputeShader cs, string computeBufferName, int kernelIndex)
    {
        ComputeBuffer cB;

        int stride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));

        cB = new ComputeBuffer(data.Length, stride);
        cB.SetData(data);

        cs.SetBuffer(kernelIndex, computeBufferName, cB);

        return cB;
    }

    internal static void ParticleDispatch(ComputeShader cs, int kern, int width, int height = 0, int depth = 0)
    {
        cs.GetKernelThreadGroupSizes(kern, out uint x, out uint y, out uint z);

        int nx = Mathf.Max(1, (int)(width / x));
        int ny = Mathf.Max(1, (int)(height / x));
        int nz = Mathf.Max(1, (int)(depth / x));

        cs.Dispatch(kern, nx, ny, nz);
    }
}
