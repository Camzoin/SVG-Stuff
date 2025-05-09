using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Linq;

public class PlotCombiner : MonoBehaviour
{
    public string targetSVGName;
    public string secondSVGName;
    public string thirdSVGName;

    public Transform copierHolder;

    public Vector2 outputSize = new Vector2(9 * 96, 12 * 96);

    public Vector2 infoPageSize = new Vector2(4 * 96, 6 * 96);

    [ContextMenu("SlamTogether")]
    public void SlamTogether(List<List<List<Vector2>>> finalLineList, string namename)
    {
        List<List<Vector2>> allFilesPaths = new List<List<Vector2>>();

        foreach(List<List<Vector2>> llv2 in finalLineList)
        {
            allFilesPaths.AddRange(llv2);
        }


        SaveCombinedFile(allFilesPaths, 1000, outputSize, namename);
    }



    [ContextMenu("ScaleInfoPage")]
    public void ScaleInfoPage()
    {
        List<List<Vector2>> bothFilesPaths = new List<List<Vector2>>();

        if (FileToLineList(targetSVGName, 0) != new List<List<Vector2>>())
        {
            bothFilesPaths.AddRange(ScalePathList(TranslatePathList(FileToLineList(targetSVGName, 1000), new Vector2(0, infoPageSize.y / 4)), 0.5f));

            SaveCombinedFile(bothFilesPaths, 1000, infoPageSize, $"{targetSVGName}{secondSVGName}");
        }
    }


    [ContextMenu("Combine3_4x8")]
    public void Combine3_4x8()
    {
        List<List<Vector2>> bothFilesPaths = new List<List<Vector2>>();

        if (FileToLineList(targetSVGName, 0) != new List<List<Vector2>>())
        {
            bothFilesPaths.AddRange(RotatePathList(FileToLineList(targetSVGName, 0)));

            bothFilesPaths.AddRange(TranslatePathList(RotatePathList(FileToLineList(secondSVGName, 0)), new Vector2(0, 96 * 4)));

            bothFilesPaths.AddRange(TranslatePathList(RotatePathList(FileToLineList(thirdSVGName, 0)), new Vector2(0, 96 * 8)));

            SaveCombinedFile(bothFilesPaths, 0, outputSize, $"{targetSVGName}{secondSVGName}");
        }





        bothFilesPaths = new List<List<Vector2>>();

        if (FileToLineList(targetSVGName, 1) != new List<List<Vector2>>())
        {
            bothFilesPaths.AddRange(RotatePathList(FileToLineList(targetSVGName, 1)));

            bothFilesPaths.AddRange(TranslatePathList(RotatePathList(FileToLineList(secondSVGName, 1)), new Vector2(0, 96 * 4)));

            bothFilesPaths.AddRange(TranslatePathList(RotatePathList(FileToLineList(thirdSVGName, 1)), new Vector2(0, 96 * 8)));

            SaveCombinedFile(bothFilesPaths, 1, outputSize, $"{targetSVGName}{secondSVGName}");
        }


        bothFilesPaths = new List<List<Vector2>>();

        if (FileToLineList(targetSVGName, 2) != new List<List<Vector2>>())
        {
            bothFilesPaths.AddRange(RotatePathList(FileToLineList(targetSVGName, 2)));

            bothFilesPaths.AddRange(TranslatePathList(RotatePathList(FileToLineList(secondSVGName, 2)), new Vector2(0, 96 * 4)));

            bothFilesPaths.AddRange(TranslatePathList(RotatePathList(FileToLineList(thirdSVGName, 2)), new Vector2(0, 96 * 8)));

            SaveCombinedFile(bothFilesPaths, 2, outputSize, $"{targetSVGName}{secondSVGName}");
        }

    }

    [ContextMenu("SaveSVG")]
    public void SaveSVG()
    {






        //bothFilesPaths = new List<List<Vector2>>();

        //if (FileToLineList(targetSVGName, 1) != new List<List<Vector2>>())
        //{
        //    bothFilesPaths.AddRange(RotatePathList(ScalePathList(FileToLineList(targetSVGName, 1), 0.666666f)));

        //    bothFilesPaths.AddRange(TranslatePathList(RotatePathList(ScalePathList(FileToLineList(secondSVGName, 1), 0.666666f)), new Vector2(0, 96 * 6)));

        //    SaveCombinedFile(bothFilesPaths, 1, outputSize, $"{targetSVGName}{secondSVGName}");
        //}


        //bothFilesPaths = new List<List<Vector2>>();

        //if (FileToLineList(targetSVGName, 2) != new List<List<Vector2>>())
        //{
        //    bothFilesPaths.AddRange(RotatePathList(ScalePathList(FileToLineList(targetSVGName, 2), 0.666666f)));

        //    bothFilesPaths.AddRange(TranslatePathList(RotatePathList(ScalePathList(FileToLineList(secondSVGName, 2), 0.666666f)), new Vector2(0, 96 * 6)));

        //    SaveCombinedFile(bothFilesPaths, 2, outputSize, $"{targetSVGName}{secondSVGName}");
        //}





        for (int i = 0; i < 20; i++)
        {
            List<List<Vector2>> bothFilesPaths = new List<List<Vector2>>();

            if (FileToLineList(targetSVGName, i) != new List<List<Vector2>>())
            {


                //bothFilesPaths.AddRange(FileToLineList(targetSVGName, i));

                bothFilesPaths.AddRange(RotatePathList(ScalePathList(FileToLineList(targetSVGName, i), 0.666666f)));

                bothFilesPaths.AddRange(TranslatePathList(RotatePathList(ScalePathList(FileToLineList(secondSVGName, i), 0.666666f)), new Vector2(0, 96 * 6)));

                SaveCombinedFile(bothFilesPaths, i, outputSize, $"{targetSVGName}{secondSVGName}");
            }
        }
    }

    public void SaveCombinedFile(List<List<Vector2>> allPaths, int colorIndex, Vector2 fileSize, string saveName)
    {
        svgVisual svgVis = gameObject.GetComponent<svgVisual>();

        StringBuilder svgContent = new StringBuilder();

        // SVG header and basic structure
        svgContent.AppendLine("<svg");
        svgContent.AppendLine($"   width=\"{fileSize.x}\"");
        svgContent.AppendLine($"   height = \"{fileSize.y}\"");
        svgContent.AppendLine("   version=\"1.1\"");
        svgContent.AppendLine("   id=\"svg2\"");
        svgContent.AppendLine("   xmlns=\"http://www.w3.org/2000/svg\"");
        svgContent.AppendLine("   xmlns:svg=\"http://www.w3.org/2000/svg\">");
        svgContent.AppendLine("  <defs");
        svgContent.AppendLine("     id=\"defs6\" />");
        svgContent.AppendLine("");

        Color drawLineColor = Color.black;

        if (svgVis.pensToUse.Count > colorIndex)
        {
            drawLineColor = svgVis.pensToUse[colorIndex];
        }


        for (int i = 0; i < allPaths.Count; i++)
        {
            if (allPaths[i].Count >= 2)
            {
                if (new Vector2(allPaths[i][0].x, allPaths[i][0].y) != new Vector2(allPaths[i][1].x, allPaths[i][1].y))
                {
                    //svgContent.AppendLine("  <path");
                    svgContent.AppendLine($"<path  style=\"fill:none;stroke:#{ColorUtility.ToHtmlStringRGB(drawLineColor)};stroke-width:{0.5f};stroke-opacity:{drawLineColor.a}\"");

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

        if (!Directory.Exists($"Z:\\Shit\\SVG Stuff\\SVG-Stuff\\Assets\\Resources\\{saveName}"))
        {
            AssetDatabase.CreateFolder("Assets/Resources", $"{saveName}");
        }


        string desktopPath = $"Z:\\Shit\\SVG Stuff\\SVG-Stuff\\Assets\\Resources\\{saveName}";
        desktopPath += "\\";

        string filePath = "";


        filePath = Path.Combine(desktopPath, $"{saveName}{colorIndex}" + ".svg");
        //filePath = Path.Combine(desktopPath, $"{targetSVGName}{secondSVGName}{colorIndex}" + ".svg");


        //Debug.Log(svgContent);

        // Write the SVG content to a file
        File.WriteAllText(filePath, svgContent.ToString());

        //Debug.Log("Done");
    }


    //[ContextMenu("CombineFiles")]
    //public void CombineFiles()
    //{
    //    DrawLines(RotatePathList(ScalePathList(FileToLineList(targetSVGName), 0.66f)));

    //    DrawLines(TranslatePathList( RotatePathList(ScalePathList(FileToLineList(secondSVGName), 0.66f)), new Vector2(0,96 * 6)));

    //    foreach(Transform t in copierHolder)
    //    {
    //        t.localPosition = Vector3.zero;
    //    }
    //}


    [ContextMenu("FileToLineList")]
    public List<List<Vector2>> FileToLineList(string fileName, int colorInt)
    {
        List<List<Vector2>> allLines = new List<List<Vector2>>();

        //Get SVG file
        string svgFileContent = "";



        var textFile = new TextAsset();

        if (colorInt > 20)
        {
            Debug.Log($"{fileName}/{fileName}Info");
            textFile = Resources.Load<TextAsset>($"{fileName}/{fileName}Info");
            Debug.Log(textFile.name);
        }
        else
        {
            textFile = Resources.Load<TextAsset>($"{fileName}/{fileName}{colorInt}");
            //Debug.Log(textFile.name + "Fuck");
        }

        if (textFile != null)
        {
            Debug.Log(textFile.name);

            svgFileContent = textFile.text;

            List<List<List<Vector2>>> svgFileLines = this.gameObject.GetComponent<SVGTextImporter>().TurnTextToLines(svgFileContent, 1, false);

            gameObject.GetComponent<svgVisual>().ResetLineObjects();

            foreach (List<List<Vector2>> sg in svgFileLines)
            {
                foreach (List<Vector2> l in sg)
                {
                    List<Vector2> linePoints = new List<Vector2>();

                    for (int i = 0; i < l.Count; i++)
                    {
                        linePoints.Add(new Vector2(l[i].x, l[i].y));
                    }

                    allLines.Add(linePoints);
                }
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

            gameObject.GetComponent<svgVisual>().PlacePath(1, linePoints, path.IndexOf(sg), copierHolder, new Color(10,10,10));
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
