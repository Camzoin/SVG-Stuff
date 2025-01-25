using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class SVGTextImporter : MonoBehaviour
{
    public TextAsset alphabetAsset;

    public bool firstLetterStartsOnAdditonMode = true;

    public float additonalLetterOffet = 0;

    public float curWritingXPos = 0;

    public List<List<List<Vector3>>> letterList = new List<List<List<Vector3>>>();

    public List<Vector2> letterSizes = new List<Vector2>();

    public string displayString = "";

    public float spaceOffset = 500;

    public float textScale = 1;

    public float lineWidth = 2;

    public bool underlineTestText = false;

    public TextAsset adjList;

    public TextAsset nounList;

    public svgVisual svgVis;

    private List<LineRenderer> pastWrittenLineRendererList = new List<LineRenderer>();

    private List<List<Vector2>> pastWrittenLinePositionLists = new List<List<Vector2>>();

    public GameObject lastTextHolder;

    [ContextMenu("DisplayAlphabetFromList")]
    public void DisplayAlphabetFromList()
    {
        TurnTextToLines(true, textScale);

        pastWrittenLineRendererList = new List<LineRenderer>();

        WriteText(displayString, Vector3.up * 100, underlineTestText);
    }


    [ContextMenu("DisplayAlphabet")]
    public void DisplayAlphabet()
    {
        TurnTextToLines(false, textScale);
    }

    [ContextMenu("CompileInfoPage")]
    public void CompileInfoPage()
    {
        if (lastTextHolder)
        {
            DestroyImmediate(lastTextHolder);
        }

        lastTextHolder = new GameObject("Text Holder");

        pastWrittenLineRendererList = new List<LineRenderer>();

        pastWrittenLinePositionLists = new List<List<Vector2>>();

        float curInfoSegmentY = 0;



        //Write Title Block
        curInfoSegmentY = -(svgVis.svgSize.y / 2) + svgVis.clippingSize.y + 120;
        TurnTextToLines(true, textScale * 2.7f);
        WriteText(svgVis.piecename, new Vector3(0, curInfoSegmentY, 0), true);

        curInfoSegmentY += 37.5f;
        TurnTextToLines(true, textScale * 0.75f);
        WriteText(svgVis.generatorName, new Vector3(0, curInfoSegmentY, 0), false);

        curInfoSegmentY += 25;
        WriteText("Seed: " + svgVis.seedValue.ToString() + "      Version: " + svgVis.versionNumber + "      Date: " + GetDate(), new Vector3(0, curInfoSegmentY, 0), false);

        //curInfoSegmentY += 25;
        //WriteText("      Version: " + svgVis.versionNumber, new Vector3(0, curInfoSegmentY, 0), false);

        curInfoSegmentY += 25;
        WriteText("Shortened URL: " + svgVis.shortURL, new Vector3(0, curInfoSegmentY, 0), false);

        curInfoSegmentY += 25;
        WriteText("File Host URL: " + svgVis.fileURL, new Vector3(0, curInfoSegmentY, 0), false);

        //curInfoSegmentY += 25;
        //WriteText("      Date: " + GetDate(), new Vector3(0, curInfoSegmentY, 0), false);

        //Write Param Block
        curInfoSegmentY += 65;
        TurnTextToLines(true, textScale * 2f);
        WriteText("Parameters", new Vector3(0, curInfoSegmentY, 0), true);

        curInfoSegmentY += 25 + 12.5f;
        TurnTextToLines(true, textScale * 0.75f);
        WriteText("Size = " + svgVis.svgSize.ToString(), new Vector3(0, curInfoSegmentY, 0), false);

        curInfoSegmentY += 25;
        WriteText("Bounding Box Indent = " + svgVis.clippingSize.ToString(), new Vector3(0, curInfoSegmentY, 0), false);

        curInfoSegmentY += 25;
        WriteText("Draw Bounds = " + svgVis.drawBounds.ToString(), new Vector3(0, curInfoSegmentY, 0), false);




        curInfoSegmentY += 25f;
        WriteText("Flow Field Node Count = " + svgVis.flowFieldNodeCount.ToString(), new Vector3(0, curInfoSegmentY, 0), false);

        curInfoSegmentY += 25;
        WriteText("Flow Field Node Influence Distance = " + svgVis.maxInfluenceDist.ToString(), new Vector3(0, curInfoSegmentY, 0), false);

        curInfoSegmentY += 25;
        WriteText("Flow Field Movement Multiplier = " + svgVis.flowFieldMovementMulti.ToString(), new Vector3(0, curInfoSegmentY, 0), false);

        curInfoSegmentY += 25;
        WriteText("New Flow Field Per Shape = " + svgVis.redoFlowFieldForNewColors.ToString(), new Vector3(0, curInfoSegmentY, 0), false);

        curInfoSegmentY += 25;
        WriteText("Use Random Constant Flow Direction = " + svgVis.useRandomContantFlowDir.ToString(), new Vector3(0, curInfoSegmentY, 0), false);

        curInfoSegmentY += 25;
        WriteText("Planned Constant Flow Direction = " + svgVis.constantFlowDir.ToString(), new Vector3(0, curInfoSegmentY, 0), false);

        curInfoSegmentY += 25;
        WriteText("Constant Flow Speed Min Max = " + svgVis.constantSpeedMinMax.ToString(), new Vector3(0, curInfoSegmentY, 0), false);

        curInfoSegmentY += 25;
        WriteText("Flow From Shape = " + svgVis.useFlowFromSpawn.ToString(), new Vector3(0, curInfoSegmentY, 0), false);

        curInfoSegmentY += 25;
        WriteText("Flow From Shape Speed = " + svgVis.flowFromCenterMulti.ToString(), new Vector3(0, curInfoSegmentY, 0), false);




        curInfoSegmentY += 25f;
        WriteText("Shape Point Count = " + svgVis.posibleShapeVertCount[0].ToString(), new Vector3(0, curInfoSegmentY, 0), false);

        curInfoSegmentY += 25;
        WriteText("Shape Count = " + svgVis.colorCount.ToString(), new Vector3(0, curInfoSegmentY, 0), false);

        curInfoSegmentY += 25;
        WriteText("Shape Position = " + svgVis.additionalSpawnOffset.ToString(), new Vector3(0, curInfoSegmentY, 0), false);

        curInfoSegmentY += 25;
        WriteText("Shape Radius Min Max = " + svgVis.circleRadius.ToString(), new Vector3(0, curInfoSegmentY, 0), false);

        curInfoSegmentY += 25;
        WriteText("Line Count = " + svgVis.pathCount.ToString(), new Vector3(0, curInfoSegmentY, 0), false);

        curInfoSegmentY += 25;
        WriteText("Max Line Segments = " + svgVis.pathLength.ToString(), new Vector3(0, curInfoSegmentY, 0), false);

        curInfoSegmentY += 25;
        WriteText("Draw Spawn Shape = " + svgVis.drawSpawnShapes.ToString(), new Vector3(0, curInfoSegmentY, 0), false);

        curInfoSegmentY += 25;
        WriteText("Use Random Shape Rotation = " + svgVis.useRandomRoation.ToString(), new Vector3(0, curInfoSegmentY, 0), false);

        curInfoSegmentY += 25;
        WriteText("Shape Rotation = " + svgVis.shapeRotation.ToString(), new Vector3(0, curInfoSegmentY, 0), false);



        if (svgVis.recipientName != "")
        {
            //Write Signature Block
            curInfoSegmentY = (svgVis.svgSize.y / 2) - svgVis.clippingSize.y - 125;
            TurnTextToLines(true, textScale * 2.25f);
            WriteText(svgVis.artistName, new Vector3(0, curInfoSegmentY, 0), true);

            curInfoSegmentY += 35;
            TurnTextToLines(true, textScale * 0.75f);
            WriteText("for", new Vector3(0, curInfoSegmentY, 0), false);

            curInfoSegmentY += 40;
            TurnTextToLines(true, textScale * 1.85f);
            WriteText(svgVis.recipientName, new Vector3(0, curInfoSegmentY, 0), true);
        }
        else
        {
            //Write Signature Block
            curInfoSegmentY = (svgVis.svgSize.y / 2) - svgVis.clippingSize.y - 60;
            TurnTextToLines(true, textScale * 3f);
            WriteText(svgVis.artistName, new Vector3(0, curInfoSegmentY, 0), true);
        }

        svgVis.SetRenderValues(pastWrittenLineRendererList, svgVis.textMat, Color.black);

        lastTextHolder.transform.position = Vector3.up * svgVis.svgSize.y * 4;

        svgVis.GenerateSVG(pastWrittenLinePositionLists, false, true, 0);

        StartCoroutine(waiter());
    }

    IEnumerator waiter()
    {
        yield return new WaitForSeconds(1f);

        Camera.main.transform.position += Vector3.up * svgVis.svgSize.y * 4;

        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Artwork";
        desktopPath += "\\";

        ScreenCapture.CaptureScreenshot(desktopPath + svgVis.yourFileName + " Info Screenshot.png");

        //Wait for 2 seconds
        yield return new WaitForSeconds(1f);

        Camera.main.transform.position -= Vector3.up * svgVis.svgSize.y * 4;
    }



    public void TurnTextToLines(bool clearAlphabetAfterSetup, float thisAlphabetsScale)
    {
        spaceOffset = thisAlphabetsScale * 300;

        GameObject alphabetHolder = new GameObject("AlphabetHolder");

        curWritingXPos = 0;

        letterSizes = new List<Vector2>();

        letterList = new List<List<List<Vector3>>>();

        string wholeFile = alphabetAsset.ToString();

        char[] escapeChars = new char[2];

        escapeChars[0] = ' ';
        escapeChars[0] = ',';

        string[] brokenString = wholeFile.Split("d=");

        int curSVGCharCount = 0;

        //For each letter (or path) in the text file
        for (int i = 0; i < brokenString.Length; i++)
        {
            string cleanedString = brokenString[i];

            List<List<Vector3>> listOfPathsThisLetter = new List<List<Vector3>>();

            Vector2 lastPosition = Vector2.zero;

            if (cleanedString[0] == '"' && (cleanedString[1] == 'm'  || cleanedString[1] == 'M'))
            {
                int currentStrokeThisPath = 0;

                int currentDrawCommandIndex = 1;

                if (cleanedString[1] == 'm')
                {
                    currentDrawCommandIndex = 4;
                }

                cleanedString = cleanedString.Substring(3);

                cleanedString = cleanedString.Split('"')[0];

                string[] linePositions1 = cleanedString.Split(' ');

                List<string> finalSingleCoordPositions = new List<string>();

                foreach (string thisPos in linePositions1)
                {
                    if (thisPos.Contains(","))
                    {
                        string[] splitCoords = thisPos.Split(',');

                        foreach (string splitSingle in splitCoords)
                        {
                            finalSingleCoordPositions.Add(splitSingle);
                        }
                    }
                    else
                    {
                        finalSingleCoordPositions.Add(thisPos);
                    }                 
                }

                List<float> finalSinglePosFloats = new List<float>();

                for (int j = 0; j < finalSingleCoordPositions.Count; j++)
                {
                    if (char.IsDigit(finalSingleCoordPositions[j][0]) || finalSingleCoordPositions[j][0] == '-')
                    {
                        finalSinglePosFloats.Add(float.Parse(finalSingleCoordPositions[j]) * thisAlphabetsScale);
                    }
                    else
                    {
                        finalSinglePosFloats.Add(0);
                    }

                }






                listOfPathsThisLetter.Add(new List<Vector3>());





                for (int j = 0; j < finalSingleCoordPositions.Count; j++)
                {
                    //If I am a position, place me
                    if (char.IsDigit(finalSingleCoordPositions[j][0]) || finalSingleCoordPositions[j][0] == '-')
                    {
                        //Debug.Log(finalSingleCoordPositions[j]);

                        Vector2 coordinateFloatPair = Vector2.zero;

                        //Move to
                        if (currentDrawCommandIndex == 0 || currentDrawCommandIndex == 1 || currentDrawCommandIndex == 4 || currentDrawCommandIndex == 7)
                        {
                            if (currentDrawCommandIndex == 0)
                            {
                                currentStrokeThisPath++;

                                listOfPathsThisLetter.Add(new List<Vector3>());

                                currentDrawCommandIndex = 1;
                            }

                            if (currentDrawCommandIndex == 7)
                            {
                                currentStrokeThisPath++;

                                listOfPathsThisLetter.Add(new List<Vector3>());

                                currentDrawCommandIndex = 4;
                            }

                            if (currentDrawCommandIndex == 4)
                            {
                                coordinateFloatPair = new Vector3(finalSinglePosFloats[j] + lastPosition.x, finalSinglePosFloats[j + 1] + lastPosition.y, 0);
                            }
                            else
                            {
                                coordinateFloatPair = new Vector3(finalSinglePosFloats[j], finalSinglePosFloats[j + 1], 0);
                            }



                            listOfPathsThisLetter[currentStrokeThisPath].Add(coordinateFloatPair);
                            j++;
                        }

                        //Horizontal
                        if (currentDrawCommandIndex == 2 || currentDrawCommandIndex == 5)
                        {
                            if (currentDrawCommandIndex == 5)
                            {
                                coordinateFloatPair = new Vector3(finalSinglePosFloats[j] + lastPosition.x, lastPosition.y, 0);
                            }
                            else
                            {
                                coordinateFloatPair = new Vector3(finalSinglePosFloats[j], lastPosition.y, 0);
                            }

                            listOfPathsThisLetter[currentStrokeThisPath].Add(coordinateFloatPair);
                        }

                        //Vertical
                        if (currentDrawCommandIndex == 3 || currentDrawCommandIndex == 6)
                        {
                            if (currentDrawCommandIndex == 6)
                            {
                                coordinateFloatPair = new Vector3(lastPosition.x, finalSinglePosFloats[j] + lastPosition.y, 0);
                            }
                            else
                            {
                                coordinateFloatPair = new Vector3(lastPosition.x, finalSinglePosFloats[j], 0);
                            }

                            listOfPathsThisLetter[currentStrokeThisPath].Add(coordinateFloatPair);
                        }

                        //Debug.Log(coordinateFloatPair);

                        lastPosition = coordinateFloatPair;
                    }
                    //If I am a letter, find out which draw command to use
                    //https://www.w3schools.com/graphics/svg_path.asp
                    else
                    {
                        //Debug.Log(finalSingleCoordPositions[j]);

                        if (finalSingleCoordPositions[j][0] == 'M')
                        {
                            currentDrawCommandIndex = 0;
                        }

                        if (finalSingleCoordPositions[j][0] == 'L')
                        {
                            currentDrawCommandIndex = 1;
                        }

                        if (finalSingleCoordPositions[j][0] == 'H')
                        {
                            currentDrawCommandIndex = 2;
                        }

                        if (finalSingleCoordPositions[j][0] == 'V')
                        {
                            currentDrawCommandIndex = 3;
                        }



                        if (finalSingleCoordPositions[j][0] == 'l')
                        {
                            currentDrawCommandIndex = 4;
                        }

                        if (finalSingleCoordPositions[j][0] == 'h')
                        {
                            currentDrawCommandIndex = 5;
                        }

                        if (finalSingleCoordPositions[j][0] == 'v')
                        {
                            currentDrawCommandIndex = 6;
                        }
                        if (finalSingleCoordPositions[j][0] == 'm')
                        {
                            currentDrawCommandIndex = 7;
                        }
                    }



                }

                Vector2 offset = Vector2.zero;

                foreach (List<Vector3> linesToRenderThisLetter in listOfPathsThisLetter)
                {
                    foreach (Vector3 v in linesToRenderThisLetter)
                    {
                        if (offset.x < v.x)
                        {
                            offset.x = v.x;
                        }

                        if (offset.y > v.y)
                        {
                            offset.y = v.y;
                        }
                    }
                }

                foreach (List<Vector3> linesToRenderThisLetter in listOfPathsThisLetter)
                {

                    //Debug.Log(linesToRenderThisLetter.ToArray().Length + " " + offset);

                    GameObject newLine = new GameObject(curSVGCharCount.ToString());

                    newLine.transform.SetParent(alphabetHolder.transform);

                    LineRenderer lineRenderer = newLine.AddComponent<LineRenderer>();

                    lineRenderer.useWorldSpace = false;

                    lineRenderer.widthMultiplier = lineWidth;

                    lineRenderer.positionCount = linesToRenderThisLetter.ToArray().Length;

                    lineRenderer.SetPositions(linesToRenderThisLetter.ToArray());

                    newLine.transform.position += new Vector3(curWritingXPos, 0, 0);
                }

                letterSizes.Add(offset);

                curWritingXPos += offset.x + additonalLetterOffet;

                //foreach (List<Vector2> pathPoints in listOfPathsThisLetter)
                //{
                //    Debug.Log(pathPoints);
                //}

                Debug.Log("END CHAR " + curSVGCharCount + " TOOK " + currentStrokeThisPath + " LINES " + listOfPathsThisLetter.Count);

                curSVGCharCount++;

                //Debug.Log(cleanedString);
            }



            letterList.Add(listOfPathsThisLetter);

            Debug.Log(letterList.Count + " " + letterList[letterList.Count - 1].Count);

            if (letterList[letterList.Count - 1].Count == 0)
            {
                letterList.RemoveAt(letterList.Count - 1);
            }
        }

        foreach(List<List<Vector3>> letters in letterList)
        {
            Debug.Log("Letter" + letters.Count);
        }

        if (clearAlphabetAfterSetup)
        {
            DestroyImmediate(alphabetHolder);
        }
    }


    public void WriteText(string textToWrite, Vector3 textOffset, bool underlineThisText)
    {
        curWritingXPos = 0;

        List<int> letterIndexesToWrite = new List<int>();

        GameObject newTextHolder = new GameObject(textToWrite);

        if (lastTextHolder == null)
        {
            lastTextHolder = new GameObject("Text Holder");
        }

        newTextHolder.transform.parent = lastTextHolder.transform;


        foreach (char c in textToWrite)
        {
            //Debug.Log(c - 64);

            //letterIndexesToWrite.Add(c - 65);

            //Debug.Log(letterList[c - 65].Count);

            if (char.IsLetter(c))
            {
                letterIndexesToWrite.Add(GetIndexInAlphabet(c));
            }
            else
            {
                if (char.IsDigit(c))
                {
                    letterIndexesToWrite.Add(int.Parse(c.ToString()) + 52);
                }
                //.!?,-+*/=_:;' other supported character

                if (c == '.')
                {
                    letterIndexesToWrite.Add(62);
                }

                if (c == '!')
                {
                    letterIndexesToWrite.Add(63);
                }

                if (c == '?')
                {
                    letterIndexesToWrite.Add(64);
                }

                if (c == ',')
                {
                    letterIndexesToWrite.Add(65);
                }

                if (c == '-')
                {
                    letterIndexesToWrite.Add(66);
                }

                if (c == '+')
                {
                    letterIndexesToWrite.Add(67);
                }

                if (c == '*')
                {
                    letterIndexesToWrite.Add(68);
                }

                if (c == '/')
                {
                    letterIndexesToWrite.Add(69);
                }

                if (c == '=')
                {
                    letterIndexesToWrite.Add(70);
                }

                if (c == '_')
                {
                    letterIndexesToWrite.Add(71);
                }

                if (c == ':')
                {
                    letterIndexesToWrite.Add(72);
                }

                if (c == ';')
                {
                    letterIndexesToWrite.Add(73);
                }

                if (c == '\'')
                {
                    letterIndexesToWrite.Add(74);
                }

                if (c == ' ')
                {
                    letterIndexesToWrite.Add(1000);
                }
            }
        }

        float lowestLetterY = 0;

        //Instead of going thru all letters take in string
        for (int i = 0; i < letterIndexesToWrite.Count; i++)
        {
            //If I am a letter
            if (letterIndexesToWrite[i] < 75)
            {
                for (int j = 0; j < letterList[letterIndexesToWrite[i]].Count; j++)
                {
                    //Debug.Log(penDown.ToArray().Length + " " + offset);

                    GameObject newLine = new GameObject(letterIndexesToWrite[i].ToString());

                    newLine.transform.SetParent(newTextHolder.transform);

                    LineRenderer lineRenderer = newLine.AddComponent<LineRenderer>();

                    lineRenderer.useWorldSpace = false;

                    lineRenderer.widthMultiplier = lineWidth;

                    lineRenderer.alignment = LineAlignment.TransformZ;

                    lineRenderer.positionCount = letterList[letterIndexesToWrite[i]][j].ToArray().Length;

                    List<Vector2> vec2Positions = new List<Vector2>();

                    foreach(Vector3 v in letterList[letterIndexesToWrite[i]][j])
                    {
                        vec2Positions.Add(new Vector2(v.x + curWritingXPos + textOffset.x + (svgVis.clippingSize.x * 1.5f), -v.y + lineRenderer.transform.position.y + textOffset.y + (svgVis.svgSize.y / 2)));
                    }

                    pastWrittenLinePositionLists.Add(vec2Positions);

                    lineRenderer.SetPositions(letterList[letterIndexesToWrite[i]][j].ToArray());

                    pastWrittenLineRendererList.Add(lineRenderer);


                    newLine.transform.localPosition += new Vector3(curWritingXPos - (svgVis.svgSize.x / 4), 0, 0);
                }

                if (lowestLetterY > letterSizes[letterIndexesToWrite[i]].y)
                {
                    lowestLetterY = letterSizes[letterIndexesToWrite[i]].y;
                }

                curWritingXPos += letterSizes[letterIndexesToWrite[i]].x + additonalLetterOffet;
            }
            //add space
            else
            {
                curWritingXPos += spaceOffset;
            }
            
        }

        if (underlineThisText)
        {
            GameObject newLine = new GameObject("Underline");

            newLine.transform.SetParent(newTextHolder.transform);

            LineRenderer lineRenderer = newLine.AddComponent<LineRenderer>();

            lineRenderer.useWorldSpace = false;

            lineRenderer.widthMultiplier = lineWidth;

            lineRenderer.alignment = LineAlignment.TransformZ;

            lineRenderer.positionCount = 2;

            List<Vector3> underlinePoints = new List<Vector3>();

            underlinePoints.Add(Vector3.zero);

            underlinePoints.Add(Vector3.right * curWritingXPos);

            List<Vector2> vec2Positions = new List<Vector2>();

            foreach (Vector3 v in underlinePoints)
            {
                vec2Positions.Add(new Vector2(v.x + textOffset.x + (svgVis.clippingSize.x * 1.5f), -v.y + lineRenderer.transform.position.y + textOffset.y + (svgVis.svgSize.y / 2) - lowestLetterY + 2.5f));
            }

            pastWrittenLinePositionLists.Add(vec2Positions);

            lineRenderer.SetPositions(underlinePoints.ToArray());

            pastWrittenLineRendererList.Add(lineRenderer);


            newLine.transform.localPosition += new Vector3(-(svgVis.svgSize.x / 4), lowestLetterY - 2.5f, 0);
        }

        newTextHolder.transform.position -= textOffset;
    }


    private static int GetIndexInAlphabet(char value)
    {
        int uppserOffset = 0;

        if (char.IsUpper(value))
        {
            uppserOffset += 26;
        }

        // Uses the uppercase character unicode code point. 'A' = U+0042 = 65, 'Z' = U+005A = 90
        char upper = char.ToUpper(value);
        if (upper < 'A' || upper > 'Z')
        {
            return 1000;
            //throw new ArgumentOutOfRangeException("value", "This method only accepts standard Latin characters.");
        }

        return (int)upper - (int)'A' + uppserOffset;
    }

    [ContextMenu("GetDate")]
    public string GetDate()
    {
        DateTime today = DateTime.Today;

        string date = "";

        string[] dateStings = today.Date.ToString().Split(' ');

        date += dateStings[0];

        Debug.Log(date);

        return date;
    }


    [ContextMenu("RandomName")]
    public string GenerateRandomName()
    {
        string finalName = "";

        string finalAdj = "";

        string finalNoun = "";

        string[] adjListText = adjList.text.Split($"{Environment.NewLine}");

        int adjNum = UnityEngine.Random.Range(0, adjListText.Length);


        for (int i = 0; i < adjListText[adjNum].Length; i++)
        {
            if (i == 0)
            {
                finalAdj += char.ToUpper(adjListText[adjNum][i]).ToString();
            }
            else
            {
                finalAdj += adjListText[adjNum][i].ToString();
            }
        }

        char.ToUpper(adjListText[adjNum][0]);

        finalName += finalAdj + " ";


        string[] nounListText = nounList.text.Split('\n');

        int nounNum = UnityEngine.Random.Range(0, nounListText.Length);

        bool previousCharWasSpecial = false;

        for (int i = 0; i < nounListText[nounNum].Length; i++)
        {
            if (i == 0 || previousCharWasSpecial)
            {
                finalNoun += char.ToUpper(nounListText[nounNum][i]).ToString();

                previousCharWasSpecial = false;
            }
            else
            {
                if (!char.IsLetter(nounListText[nounNum][i]))
                {
                    previousCharWasSpecial = true;
                }

                finalNoun += nounListText[nounNum][i].ToString();
            }
        }

        finalName += finalNoun;

        Debug.Log(finalName + " " + nounListText.Length + " " + adjListText.Length);

        return finalName;
    }


    public void FixAdjList()
    {
        List<string> allAdjs = new List<string>();

        string[] firstSplit = adjList.text.Split(", ");

        foreach (string s in firstSplit)
        {
            string newS = "";

            for (int i = 0; i < s.Length - 1; i++)
            {
                if (char.IsLetter(s[i]))
                {
                    newS += s[i];
                }
            }

            allAdjs.Add(newS);
        }

        string newAdjFile = "";
        for (int i = 0; i < allAdjs.Count - 1; i++)
        {
            Debug.Log(i + " / " + allAdjs.Count +  " " + allAdjs[i]);

            newAdjFile += allAdjs[i] + $"{Environment.NewLine}";


        }

        Debug.Log(newAdjFile);

        TextEditor te = new TextEditor();
        te.text = newAdjFile;
        te.SelectAll();
        te.Copy();
    }
}
