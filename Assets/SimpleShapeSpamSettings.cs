using UnityEngine;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Linq;

public class SimpleShapeSpamSettings : MonoBehaviour
{
    public List<int> possibleShapeVertCounts = new List<int>();

    public bool useRandomRotation = true;

    public bool useRotationFromList = false;

    public List<float> possibleRotations = new List<float>();

    public int totalShapeCount = 2000;

    public float shapeScale = 1;

    public bool useValueToScaleShapes = true;

    public bool shapesUsePsudoRandomScale = false;

    public bool useValueAsChanceforShapes = true;

    public float minShapeScale = 0.05f;
}
