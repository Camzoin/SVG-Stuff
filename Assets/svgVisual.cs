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

    public float minMovemnet = 0.1f;

    public List<Vector2> flowFieldPositions = new List<Vector2>();

    public List<Vector2> flowFieldDirections = new List<Vector2>();

    public int pathCount = 5000;

    public float circleRadius = 200;

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

    private List<Vector2> shaderBasedMovementList = new List<Vector2>();

    [SerializeField] private ComputeShader flowFieldCalcShader = null;

    private ComputeBuffer flowFieldNodePostions;
    private ComputeBuffer flowFieldNodeDirections;
    private ComputeBuffer curLinePositions;
    private ComputeBuffer shaderBasedMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("DoBoth")]
    public void DoBoth()
    {
        GenerateFlowField();
        GenerateWork();
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

    [ContextMenu("GenerateWork")]
    public void GenerateWork()
    {
        //shaderBasedMovement[0] = Vector2.zero;

        //do compute shader shit
        flowFieldNodePostions = CreateComputeBuffer<Vector2>(flowFieldPositions.ToArray(), flowFieldCalcShader, "flowFieldPositions", 0);
        flowFieldNodeDirections = CreateComputeBuffer<Vector2>(flowFieldDirections.ToArray(), flowFieldCalcShader, "flowFieldDirections", 0);
        shaderBasedMovement = CreateComputeBuffer<Vector2>(shaderBasedMovementList.ToArray(), flowFieldCalcShader, "finalMovement", 0);

        flowFieldCalcShader.SetFloat("maxInfluenceDist", maxInfluenceDist);
        flowFieldCalcShader.SetInt("flowFieldNodeCount", flowFieldPositions.Count);

        //List<Vector2> circleSpawnPoints = CalculateCirclePoints(spawnCircleVertCount, svgSize / 2f, circleRadius);
        List<Vector2> circleSpawnPoints = FindPointsOnSimpleShape(spawnCircleVertCount, posibleShapeVertCount[UnityEngine.Random.Range(0, posibleShapeVertCount.Count)], svgSize / 2f, circleRadius);

        //get paint
        List<Vector2> paintSpot = new List<Vector2>();

        //bottom
        paintSpot.Add(new Vector3(svgSize.x - 1, 1, 0));
        paintSpot.Add(new Vector3(svgSize.x - 1, 3, 0));
        paintSpot.Add(new Vector3(svgSize.x - 1, 1, 0));
        paintSpot.Add(new Vector3(svgSize.x - 1, 3, 0));


        listOfPaths = new List<List<Vector2>>();

        foreach (LineRenderer line in lineObjects)
        {
            if (line)
            {
                line.SetPositions(new Vector3[0]);
            }
            
        }

        int curLineCount = 0;

        Vector2 constantMoveValue = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));

        constantMoveValue.Normalize();

        constantMoveValue *= UnityEngine.Random.Range(constantSpeedMinMax.x, constantSpeedMinMax.y);

        for (int i = 0; i < pathLength; i++)
        {
            List<Vector2> pointsThisPath = new List<Vector2>();

            Vector3 startingPos = circleSpawnPoints[(int)((i / (float)pathCount) * circleSpawnPoints.Count)];
            //Vector3 startingPos = new Vector3(UnityEngine.Random.Range(0, svgSize.x), UnityEngine.Random.Range(0, svgSize.y), 0);

            Vector2 curPos = startingPos;

            pointsThisPath.Add(startingPos);

            //generate segments
            //for (int j = 0; j < UnityEngine.Random.Range(100, 250); j++)
            for (int j = 0; j < pathLength; j++)
            {
                Vector2 moveDir = Vector2.zero;

                flowFieldCalcShader.SetVector("curPosition", curPos);

                int kernel = flowFieldCalcShader.FindKernel("FlowField");

                shaderBasedMovement.GetData(shaderBasedMovementList.ToArray());





                curPos += (moveDir * minMovemnet) + constantMoveValue;

                if (curPos.x < 0 + clippingSize.x)
                {
                    j = pathLength;
                    curPos = new Vector3(0 + clippingSize.x, curPos.y, 0);
                }

                if (curPos.x > svgSize.x - clippingSize.x)
                {
                    j = pathLength;
                    curPos = new Vector3(svgSize.x - clippingSize.x, curPos.y, 0);
                }


                if (curPos.y < 0 + clippingSize.y)
                {
                    j = pathLength;
                    curPos = new Vector3(curPos.x, 0 + clippingSize.y, 0);
                }

                if (curPos.y > svgSize.y - clippingSize.y)
                {
                    j = pathLength;
                    curPos = new Vector3(curPos.x, svgSize.y - clippingSize.y, 0);
                }


                pointsThisPath.Add(curPos);
            }



        }

        for (int i = 0; i < listOfPaths.Count; i++)
        {
            if (dipInPaint)
            {
                i++;
                PlacePath(0.5f, paintSpot, i);
            }

            PlacePath(0.5f, listOfPaths[i], i);
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
                PlacePath(0.5f, paintSpot, pathCount);

                PlacePath(0.5f, boarderPoints, pathCount + 1);

                PlacePath(0.5f, paintSpot, pathCount + 2);

                PlacePath(0.5f, boarderPoints, pathCount + 3);
            }
            else
            {
                PlacePath(0.5f, boarderPoints, pathCount);

                PlacePath(0.5f, boarderPoints, pathCount + 1);
            }
        }


        if (drawSpawnShapes)
        {
            if (dipInPaint)
            {
                PlacePath(0.5f, paintSpot, pathCount + 4);

                PlacePath(0.5f, circleSpawnPoints, pathCount + 5);

                PlacePath(0.5f, paintSpot, pathCount + 6);

                PlacePath(0.5f, circleSpawnPoints, pathCount + 7);
            }
            else
            {
                PlacePath(0.5f, circleSpawnPoints, pathCount + 2);

                PlacePath(0.5f, circleSpawnPoints, pathCount + 3);
            }


        }

        GenerateSVG(listOfPaths);
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
    public void GenerateSVG(List<List<Vector2>> allPaths)
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

        for (int i = 0; i < allPaths.Count; i++)
        {
            svgContent.AppendLine("  <path");
            svgContent.AppendLine("     style=\"fill:none;stroke:#000000;stroke-width:1;stroke-opacity:1\"");

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


        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        desktopPath += "\\";

        string filePath = Path.Combine(desktopPath, yourFileName + ".svg");

        // Write the SVG content to a file
        File.WriteAllText(filePath, svgContent.ToString());

        Debug.Log("Done");
    }

    public static List<Vector2> CalculateCirclePoints(int vertexCount, Vector2 center, float radius)
    {
        List<Vector2> points = new List<Vector2>();

        float angleIncrement = 2f * (float)Math.PI / vertexCount;

        for (int i = 0; i < vertexCount; i++)
        {
            float angle = i * angleIncrement;
            float x = center.x + ((float)Math.Cos(angle) * radius);
            float y = center.y + ((float)Math.Sin(angle) * radius);
            points.Add(new Vector2(x, y));
        }

        return points;
    }

    public List<Vector2> FindPointsOnSimpleShape(int pointCount, int shapeVertCount, Vector2 position, float radius)
    {
        List<Vector2> pointsOnShape = new List<Vector2>();

        List<Vector2> shapeVertPoints = CalculateCirclePoints(shapeVertCount, position, radius);

        for (int i = 0; i < pointCount; i++)
        {
            var shapeLineSegment = (int)((i / (float)pointCount) * shapeVertPoints.Count);

            Vector2 pointOnShape = new Vector2();

            //if I need to wrap
            if (shapeLineSegment >= shapeVertPoints.Count - 1)
            {
                pointOnShape = Vector2.Lerp(shapeVertPoints[0], shapeVertPoints[shapeLineSegment], (i / ((float)pointCount / (float)shapeVertPoints.Count)) % 1);
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
