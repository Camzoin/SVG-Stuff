using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Reflection;

public class TitleGen : MonoBehaviour
{
    public string artistName = "Argyle Smith";

    public string yearString = "2025";

    public string pieceName = "Yo Mama";

    public List<Vector2> pieceSizeInInches = new List<Vector2>();

    public int piecesInSeries = 4;

    public svgVisual svgVis;

    public float circle1R = 15;
    public float circle2R = 40;
    public float circle3R = 6;

    public bool generateNewPieceName = true;


    [ContextMenu("Create Title Cards")]
    public void MakeTitleCardsForSeries()
    {
        if (generateNewPieceName == true)
        {
            pieceName = svgVis.textImporter.GenerateRandomName();
        }

        foreach(Vector2 size in pieceSizeInInches)
        {
            Vector2 docSize = size * 96;

            float pieceCircleSize = 6;

            List<List<Vector2>> veryRoundCircles = new List<List<Vector2>>();

            for (int q = 0; q < pieceCircleSize; q++)
            {
                veryRoundCircles.Add(svgVis.FindPointsOnSimpleShape(720, 720, docSize / 2, q, UnityEngine.Random.Range(0f, 360f)));
            }


            //For each piece of this series

            for (int i = 0; i < piecesInSeries; i++)
            {
                List<List<Vector2>> finalCompleteLineList = new List<List<Vector2>>();

                //Make the circles of the radius'

                //For each cricle do what you need to
                //1 spawn in and fill in dots depending on piece index

                List<Vector2> circOne = svgVis.FindPointsOnSimpleShape(piecesInSeries, piecesInSeries, docSize / 2, circle1R, UnityEngine.Random.Range(0f, 360f));

                //Offset Center Circle Lines and spawn in circles based on piece number
                for (int q = 0; q < piecesInSeries; q++)
                {
                    List<Vector2> veryRoundCircleOffset = new List<Vector2>();




                    if (q <= i)
                    {
                        foreach (List<Vector2> listOfCircPoints in veryRoundCircles)
                        {
                            foreach (Vector2 point in listOfCircPoints)
                            {
                                veryRoundCircleOffset.Add(point + circOne[q] - docSize / 2);
                            }
                        }
                    }
                    else
                    {
                        foreach (Vector2 point in veryRoundCircles[(int)pieceCircleSize - 1])
                        {
                            veryRoundCircleOffset.Add(point + circOne[q] - docSize / 2);
                        }
                    }

                    svgVis.PlacePath(1, veryRoundCircleOffset, 0, this.transform, Color.black, false);

                    finalCompleteLineList.Add(veryRoundCircleOffset);
                }






                //2 spawn in and write piece name in a circle

                string pieceNameAndDash = pieceName + "    " + pieceName + "    ";

                List<Vector2> circTwo = svgVis.FindPointsOnSimpleShape(pieceNameAndDash.Length, 1000, docSize / 2, circle2R, 0);

                int mOffsetTitle = UnityEngine.Random.Range(0, circTwo.Count);

                svgVis.textImporter.DisplayAlphabet();


                for (int m = 0; m < circTwo.Count; m++)
                {
                    mOffsetTitle++;

                    mOffsetTitle %= circTwo.Count;

                    Debug.Log(pieceNameAndDash.Length + " " + m + " " + svgVis.textImporter.GetIndexInAlphabet(pieceNameAndDash[m]));

                    if (svgVis.textImporter.GetIndexInAlphabet(pieceNameAndDash[m]) != 1000)
                    {
                        List<List<Vector2>> letterLineList = svgVis.textImporter.cachedAlphabet[svgVis.textImporter.GetIndexInAlphabet(pieceNameAndDash[m])];

                        Vector2 offset = svgVis.textImporter.letterSizes[svgVis.textImporter.GetIndexInAlphabet(pieceNameAndDash[m])];

                        List<List<Vector2>> letterLineListFinal = new List<List<Vector2>>();

                        float angle = (((float)mOffsetTitle / ((float)circTwo.Count)) * 360f);

                        angle *= Mathf.Deg2Rad;


                        for (int l = 0; l < letterLineList.Count; l++)
                        {
                            letterLineListFinal.Add(new List<Vector2>());

                            for (int k = 0; k < letterLineList[l].Count; k++)
                            {
                                Vector2 rotatedPoint = (letterLineList[l][k] * new Vector2(0.7f, -0.7f)) - new Vector2(offset.x / 4f, 0);

                                rotatedPoint = new Vector2(rotatedPoint.x * Mathf.Cos(90 * Mathf.Deg2Rad) - rotatedPoint.y * Mathf.Sin(90 * Mathf.Deg2Rad), rotatedPoint.x * Mathf.Sin(90 * Mathf.Deg2Rad) + rotatedPoint.y * Mathf.Cos(90 * Mathf.Deg2Rad));

                                rotatedPoint = new Vector2(rotatedPoint.x * Mathf.Cos(angle) - rotatedPoint.y * Mathf.Sin(angle), rotatedPoint.x * Mathf.Sin(angle) + rotatedPoint.y * Mathf.Cos(angle));




                                letterLineListFinal[l].Add(rotatedPoint + circTwo[mOffsetTitle]);
                                //letterLineListFinal[l].Add(rotatedPoint);
                            }
                        }


                        finalCompleteLineList.AddRange(letterLineListFinal);
                    }
                }












                //3 spawn in and write artist name and date in a circle

                string artistNameAndYear = artistName + "    " + yearString + "    ";

                List<Vector2> circThree = svgVis.FindPointsOnSimpleShape(artistNameAndYear.Length, 1000, docSize / 2, circle3R, 0);

                int mOffset = UnityEngine.Random.Range(0, circThree.Count);

                //mOffset *= 5;

                for (int m = 0; m < circThree.Count; m++)
                {
                    mOffset++;

                    mOffset %= circThree.Count;

                    bool isNum = false;

                    int intIndex = 0;

                    if (char.IsDigit(artistNameAndYear[m]))
                    {
                        isNum = true;

                        intIndex = int.Parse(artistNameAndYear[m].ToString()) + 52;
                    }
                    else
                    {
                        intIndex = svgVis.textImporter.GetIndexInAlphabet(artistNameAndYear[m]);
                    }




                    if (svgVis.textImporter.GetIndexInAlphabet(artistNameAndYear[m]) != 1000 || isNum == true)
                    {
                        Debug.Log(artistNameAndYear.Length + " " + m + " " + intIndex + "   " + svgVis.textImporter.cachedAlphabet[intIndex].Count);

                        List<List<Vector2>> letterLineList = new List<List<Vector2>>();

                        Vector2 offset = new Vector2();

                        if (isNum == true)
                        {
                            letterLineList = svgVis.textImporter.cachedAlphabet[intIndex];

                            offset = svgVis.textImporter.letterSizes[intIndex];
                        }
                        else
                        {
                            letterLineList = svgVis.textImporter.cachedAlphabet[intIndex];

                            offset = svgVis.textImporter.letterSizes[intIndex];
                        }



                        List<List<Vector2>> letterLineListFinal = new List<List<Vector2>>();

                        float angle = (((float)mOffset / ((float)circThree.Count)) * 360f);

                        angle *= Mathf.Deg2Rad;


                        for (int l = 0; l < letterLineList.Count; l++)
                        {
                            letterLineListFinal.Add(new List<Vector2>());

                            for (int k = 0; k < letterLineList[l].Count; k++)
                            {
                                Vector2 rotatedPoint = (letterLineList[l][k] * new Vector2(0.5f, -0.5f)) - new Vector2(offset.x / 4f, 0);

                                rotatedPoint = new Vector2(rotatedPoint.x * Mathf.Cos(90 * Mathf.Deg2Rad) - rotatedPoint.y * Mathf.Sin(90 * Mathf.Deg2Rad), rotatedPoint.x * Mathf.Sin(90 * Mathf.Deg2Rad) + rotatedPoint.y * Mathf.Cos(90 * Mathf.Deg2Rad));

                                rotatedPoint = new Vector2(rotatedPoint.x * Mathf.Cos(angle) - rotatedPoint.y * Mathf.Sin(angle), rotatedPoint.x * Mathf.Sin(angle) + rotatedPoint.y * Mathf.Cos(angle));




                                letterLineListFinal[l].Add(rotatedPoint + circThree[mOffset]);
                                //letterLineListFinal[l].Add(rotatedPoint);
                            }
                        }


                        finalCompleteLineList.AddRange(letterLineListFinal);
                    }
                }










                svgVis.GenerateSVG(finalCompleteLineList, false, false, i, Color.black, docSize,  pieceName.Replace(" ",""), "_TitleCard_" + size.x + "x" + size.y + "_", false);
            }
        }

        


    }
}
