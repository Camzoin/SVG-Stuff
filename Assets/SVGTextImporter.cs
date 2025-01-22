using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SVGTextImporter : MonoBehaviour
{
    public TextAsset alphabetAsset;

    public Transform textHolder;

    public bool firstLetterStartsOnAdditonMode = true;

    public float additonalLetterOffet = 0;

    public float curWritingXPos = 0;

    public List<List<List<Vector3>>> letterList = new List<List<List<Vector3>>>();

    public List<float> offsets = new List<float>();

    public string displayString = "";

    public float spaceOffset = 500;

    public float textScale = 1;

    public float lineWidth = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("DisplayAlphabetFromList")]
    public void DisplayAlphabetFromList()
    {
        TurnListLettersToLines(displayString);
    }


    [ContextMenu("DisplayAlphabet")]
    public void DisplayAlphabet()
    {
        TurnTextToLines();
    }

    public void TurnTextToLines()
    {
        curWritingXPos = 0;

        offsets = new List<float>();

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

            if (cleanedString[0] == '"' && (cleanedString[1] == 'm'  || cleanedString[1] == 'M'))
            {
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
                        finalSinglePosFloats.Add(float.Parse(finalSingleCoordPositions[j]) * textScale);
                    }
                    else
                    {
                        finalSinglePosFloats.Add(0);
                    }

                }


                int currentStrokeThisPath = 0;



                listOfPathsThisLetter.Add(new List<Vector3>());

                Vector2 lastPosition = Vector2.zero;



                for (int j = 0; j < finalSingleCoordPositions.Count; j++)
                {
                    //If I am a position, place me
                    if (char.IsDigit(finalSingleCoordPositions[j][0]))
                    {
                        //Debug.Log(finalSingleCoordPositions[j]);

                        Vector2 coordinateFloatPair = Vector2.zero;

                        //Move to
                        if (currentDrawCommandIndex == 0 || currentDrawCommandIndex == 1 || currentDrawCommandIndex == 4)
                        {
                            if (currentDrawCommandIndex == 0)
                            {
                                currentStrokeThisPath++;

                                listOfPathsThisLetter.Add(new List<Vector3>());

                                currentDrawCommandIndex = 1;
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



                        if (finalSingleCoordPositions[j][0] == 'l' || finalSingleCoordPositions[j][0] == 'm')
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
                    }



                }

                float offset = 0;

                foreach (List<Vector3> linesToRenderThisLetter in listOfPathsThisLetter)
                {
                    foreach (Vector3 v in linesToRenderThisLetter)
                    {
                        if (offset < v.x)
                        {
                            offset = v.x;
                        }
                    }
                }

                foreach (List<Vector3> linesToRenderThisLetter in listOfPathsThisLetter)
                {

                    //Debug.Log(linesToRenderThisLetter.ToArray().Length + " " + offset);

                    GameObject newLine = new GameObject(curSVGCharCount.ToString());

                    newLine.transform.SetParent(textHolder);

                    LineRenderer lineRenderer = newLine.AddComponent<LineRenderer>();

                    lineRenderer.useWorldSpace = false;

                    lineRenderer.widthMultiplier = lineWidth;

                    lineRenderer.positionCount = linesToRenderThisLetter.ToArray().Length;

                    lineRenderer.SetPositions(linesToRenderThisLetter.ToArray());

                    newLine.transform.position += new Vector3(curWritingXPos, 0, 0);
                }

                offsets.Add(offset);

                curWritingXPos += offset + additonalLetterOffet;

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

        
    }


    public void TurnListLettersToLines(string textToWrite)
    {
        curWritingXPos = 0;

        List<int> letterIndexesToWrite = new List<int>();


        foreach (char c in textToWrite)
        {
            //Debug.Log(c - 64);

            //letterIndexesToWrite.Add(c - 65);

            //Debug.Log(letterList[c - 65].Count);

            letterIndexesToWrite.Add(GetIndexInAlphabet(c));
        }



        //Instead of going thru all letters take in string
        for (int i = 0; i < letterIndexesToWrite.Count; i++)
        {
            //If I am a letter
            if (letterIndexesToWrite[i] < 52)
            {
                for (int j = 0; j < letterList[letterIndexesToWrite[i]].Count; j++)
                {
                    //Debug.Log(penDown.ToArray().Length + " " + offset);

                    GameObject newLine = new GameObject("Letter :)");

                    newLine.transform.SetParent(textHolder);

                    LineRenderer lineRenderer = newLine.AddComponent<LineRenderer>();

                    lineRenderer.useWorldSpace = false;

                    lineRenderer.widthMultiplier = lineWidth;

                    lineRenderer.positionCount = letterList[letterIndexesToWrite[i]][j].ToArray().Length;

                    lineRenderer.SetPositions(letterList[letterIndexesToWrite[i]][j].ToArray());

                    newLine.transform.position += new Vector3(curWritingXPos, 0, 0);
                }

                curWritingXPos += offsets[letterIndexesToWrite[i]] + additonalLetterOffet;
            }
            //add space
            else
            {
                curWritingXPos += spaceOffset;            
            }
        }
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
            return 52;
            //throw new ArgumentOutOfRangeException("value", "This method only accepts standard Latin characters.");
        }

        return (int)upper - (int)'A' + uppserOffset;
    }
}
