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

    [ContextMenu("ScaleSVGFile")]
    public void ScaleSVGFile()
    {
        //Get SVG file
        string svgFileContent = "";

        var textFile = Resources.Load<TextAsset>($"{targetSVGName}/{targetSVGName}0");

        svgFileContent = textFile.text;

        List<List<List<Vector3>>> svgFileLines = this.gameObject.GetComponent<SVGTextImporter>().TurnTextToLines(svgFileContent, 0.75f, false);

        gameObject.GetComponent<svgVisual>().ResetLineObjects();

        foreach (List<List<Vector3>> sg in svgFileLines)
        {
            Debug.Log(sg.Count);

            foreach(List<Vector3> l in sg)
            {
                List<Vector2> linePoints = new List<Vector2>();

                for (int i = 0; i < l.Count; i++)
                {
                    linePoints.Add(new Vector2(l[i].x, l[i].y));
                }

                Debug.Log("Line points" + linePoints.Count);

                gameObject.GetComponent<svgVisual>().PlacePath(1, linePoints, svgFileLines.IndexOf(sg), copierHolder);
            }



            
        }


    }

    [ContextMenu("RotateSVGFile")]
    public void RotateSVGFile()
    {

    }

    [ContextMenu("CombineSVGSAndFitToPage")]
    public void CombineSVGSAndFitToPage()
    {

    }

}
