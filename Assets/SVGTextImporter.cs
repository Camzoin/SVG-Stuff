using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SVGTextImporter : MonoBehaviour
{
    public TextAsset alphabetAsset;

    public List<List<Vector2>> listOfPaths = new List<List<Vector2>>();

    public Transform textHolder;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("SetRenderValues")]
    public void DisplayAlphabet()
    {
        TurnTextToLines();
    }

    public void TurnTextToLines()
    {
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

            if (cleanedString[0] == '"' && (cleanedString[1] == 'm'  || cleanedString[1] == 'M'))
            {
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




                List<float> finalPosFloats = new List<float>();

                foreach (string finalSinglePos in finalSingleCoordPositions)
                {
                    //If I am a position, place me
                    if (char.IsDigit(finalSinglePos[0]))
                    {
                        finalPosFloats.Add(float.Parse(finalSinglePos));
                    }
                    //If I am a letter, find out which draw command to use
                    //https://www.w3schools.com/graphics/svg_path.asp
                    else
                    {

                    }


                    Debug.Log(finalSinglePos);
                }

                string positionDebug = "";

                foreach (float f in finalPosFloats)
                {
                    positionDebug += f;
                    positionDebug += " ";

                }

                Debug.Log(positionDebug);

                Debug.Log("END CHAR " + curSVGCharCount);

                curSVGCharCount++;

                //Debug.Log(cleanedString);
            }


        }



        
    }
}
