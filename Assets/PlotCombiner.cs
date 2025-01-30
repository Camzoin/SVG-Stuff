using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlotCombiner : MonoBehaviour
{
    public string targetSVGName;
    public string secondSVGName;

    public Transform copierHolder;


    [ContextMenu("CombineFiles")]
    public void CombineFiles()
    {
        DrawLines(RotatePathList(ScalePathList(FileToLineList(targetSVGName), 0.66f)));

        DrawLines(TranslatePathList( RotatePathList(ScalePathList(FileToLineList(secondSVGName), 0.66f)), new Vector2(0,96 * 6)));

        foreach(Transform t in copierHolder)
        {
            t.localPosition = Vector3.zero;
        }
    }


    [ContextMenu("FileToLineList")]
    public List<List<Vector2>> FileToLineList(string fileName)
    {
        List<List<Vector2>> allLines = new List<List<Vector2>>();

        //Get SVG file
        string svgFileContent = "";

        var textFile = Resources.Load<TextAsset>($"{fileName}/{fileName}0");

        svgFileContent = textFile.text;

        List<List<List<Vector3>>> svgFileLines = this.gameObject.GetComponent<SVGTextImporter>().TurnTextToLines(svgFileContent, 1, false);

        gameObject.GetComponent<svgVisual>().ResetLineObjects();

        foreach (List<List<Vector3>> sg in svgFileLines)
        {
            foreach (List<Vector3> l in sg)
            {
                List<Vector2> linePoints = new List<Vector2>();

                for (int i = 0; i < l.Count; i++)
                {
                    linePoints.Add(new Vector2(l[i].x, l[i].y));
                }

                allLines.Add(linePoints);
            }
        }

        return allLines;
    }

    public void DrawLines(List<List<Vector2>> path)
    {
        gameObject.GetComponent<svgVisual>().ResetLineObjects();

        foreach (List<Vector2> sg in path)
        {
            Debug.Log(sg.Count);

            List<Vector2> linePoints = new List<Vector2>();

            for (int i = 0; i < sg.Count; i++)
            {
                linePoints.Add(new Vector2(sg[i].x, sg[i].y));
            }

            Debug.Log("Line points" + linePoints.Count);

            gameObject.GetComponent<svgVisual>().PlacePath(1, linePoints, path.IndexOf(sg), copierHolder);
        }
    }


    [ContextMenu("ScalePathList")]
    public List<List<Vector2>> ScalePathList(List<List<Vector2>> svgLines, float scaleMulti)
    {
        List<List<Vector2>> allLines = new List<List<Vector2>>();

        foreach (List<Vector2> sg in svgLines)
        {
            List<Vector2> linePoints = new List<Vector2>();

            for (int i = 0; i < sg.Count; i++)
            {
                linePoints.Add(new Vector2(sg[i].x, sg[i].y) * scaleMulti);
            }

            allLines.Add(linePoints);
        }

        return allLines;
    }

    [ContextMenu("RotatePathList")]
    public List<List<Vector2>> RotatePathList(List<List<Vector2>> svgLines)
    {
        List<List<Vector2>> allLines = new List<List<Vector2>>();

        foreach (List<Vector2> sg in svgLines)
        {
            List<Vector2> linePoints = new List<Vector2>();

            for (int i = 0; i < sg.Count; i++)
            {
                linePoints.Add(new Vector2(sg[i].y, sg[i].x));
            }

            allLines.Add(linePoints);
        }

        return allLines;
    }

    [ContextMenu("TranslatePathList")]
    public List<List<Vector2>> TranslatePathList(List<List<Vector2>> svgLines, Vector2 offset)
    {
        List<List<Vector2>> allLines = new List<List<Vector2>>();

        foreach (List<Vector2> sg in svgLines)
        {
            List<Vector2> linePoints = new List<Vector2>();

            for (int i = 0; i < sg.Count; i++)
            {
                linePoints.Add(new Vector2(sg[i].x, sg[i].y) + offset);
            }

            allLines.Add(linePoints);
        }

        return allLines;
    }
}
