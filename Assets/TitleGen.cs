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

    public Vector2 pieceSizeInInches = new Vector2(4, 6);

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



        Vector2 docSize = pieceSizeInInches * 96;

        float pieceCircleSize = 10;

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

            List<Vector2> circOne = svgVis.FindPointsOnSimpleShape(piecesInSeries, piecesInSeries, docSize / 2, circle1R, UnityEngine.Random.Range(0f,360f));

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



            svgVis.textImporter.DisplayAlphabet();


            for (int m = 0; m < circTwo.Count; m++)
            {
                Debug.Log(pieceNameAndDash.Length + " " + m + " "  + svgVis.textImporter.GetIndexInAlphabet(pieceNameAndDash[m]));

                if(svgVis.textImporter.GetIndexInAlphabet(pieceNameAndDash[m]) != 1000)
                {
                    List<List<Vector2>> letterLineList = svgVis.textImporter.cachedAlphabet[svgVis.textImporter.GetIndexInAlphabet(pieceNameAndDash[m])];

                    Vector2 offset = svgVis.textImporter.letterSizes[svgVis.textImporter.GetIndexInAlphabet(pieceNameAndDash[m])];

                    List<List<Vector2>> letterLineListFinal = new List<List<Vector2>>();

                    float angle = (((float)m / ((float)circTwo.Count + 1f)) * 360f);

                    angle *= Mathf.Deg2Rad;


                    for (int l = 0; l < letterLineList.Count; l++)
                    {
                        letterLineListFinal.Add(new List<Vector2>());

                        for (int k = 0; k < letterLineList[l].Count; k++)
                        {
                            Vector2 rotatedPoint = (letterLineList[l][k] * new Vector2(0.5f,-0.5f)) - new Vector2(offset.x / 3f, 0);

                            rotatedPoint = new Vector2(rotatedPoint.x * Mathf.Cos(90 * Mathf.Deg2Rad) - rotatedPoint.y * Mathf.Sin(90 * Mathf.Deg2Rad), rotatedPoint.x * Mathf.Sin(90 * Mathf.Deg2Rad) + rotatedPoint.y * Mathf.Cos(90 * Mathf.Deg2Rad));

                            rotatedPoint = new Vector2(rotatedPoint.x * Mathf.Cos(angle) - rotatedPoint.y * Mathf.Sin(angle), rotatedPoint.x * Mathf.Sin(angle) + rotatedPoint.y * Mathf.Cos(angle));




                            letterLineListFinal[l].Add(rotatedPoint + circTwo[m]);
                            //letterLineListFinal[l].Add(rotatedPoint);
                        }
                    }


                    finalCompleteLineList.AddRange(letterLineListFinal);
                }
            }












            //3 spawn in and write artist name and date in a circle












            svgVis.GenerateSVG(finalCompleteLineList, false, false, i, Color.black, docSize, pieceName);
        }


    }
}
