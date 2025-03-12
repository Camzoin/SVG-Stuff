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

public class AnimCellManager : MonoBehaviour
{
    public AnimationClip animClip;

    public AnimationSceneCell referenceCell;

    public int frameCount = 12;

    public Transform cellHolder;

    public List<AnimationSceneCell> animCells = new List<AnimationSceneCell>();

    public List<AnimationCurve> propertyCurves = new List<AnimationCurve>();

    public RenderTexture ppRenderTexture;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }





    [ContextMenu("Duplicate Reference Animation Cell For Animation")]
    public void DuplicateReferenceAnimationCellForAnimation()
    {
        List<AnimationSceneCell> oldAnimCells = animCells;

        animCells = new List<AnimationSceneCell>();

        for (int k = 0; k < oldAnimCells.Count; k++)
        {
            GameObject newCell = Instantiate(referenceCell.gameObject, Vector3.zero, Quaternion.identity, cellHolder);

            newCell.transform.localPosition = Vector3.left * 500 * (k + 1);

            newCell.name = "Cell " + k;

            foreach(GameObject go in AllChilds(newCell))
            {
                go.name += " " + k;
            }

            AnimationSceneCell thisASC = newCell.GetComponent<AnimationSceneCell>();

            animCells.Add(thisASC);

            thisASC.cellCam.targetTexture = oldAnimCells[k].cellCam.targetTexture;
            thisASC.cellNormalCam.targetTexture = oldAnimCells[k].cellNormalCam.targetTexture;


            RenderTexture colorRT = thisASC.cellCam.targetTexture;
            RenderTexture normalRT = thisASC.cellNormalCam.targetTexture;


            colorRT.Release();
            colorRT.width = (int)(ppRenderTexture.width / 4);
            colorRT.height = (int)(ppRenderTexture.height / 3);
            colorRT.Create();




            normalRT.Release();
            normalRT.width = (int)(ppRenderTexture.width / 4);
            normalRT.height = (int)(ppRenderTexture.height / 3);
            normalRT.Create();
        }

        foreach(AnimationSceneCell asc in oldAnimCells)
        {
            DestroyImmediate(asc.gameObject);
        }
    }





    private List<GameObject> AllChilds(GameObject root)
    {
        List<GameObject> result = new List<GameObject>();
        if (root.transform.childCount > 0)
        {
            foreach (Transform VARIABLE in root.transform)
            {
                Searcher(result, VARIABLE.gameObject);
            }
        }
        return result;
    }

    private void Searcher(List<GameObject> list, GameObject root)
    {
        list.Add(root);
        if (root.transform.childCount > 0)
        {
            foreach (Transform VARIABLE in root.transform)
            {
                Searcher(list, VARIABLE.gameObject);
            }
        }
    }
































    [ContextMenu("BuildAnimationCellsFromAnimation")]
    public void BuildAnimationCellsFromAnimation()
    {
        List<GameObject> cellGameObjects = new List<GameObject>();
        List<float> sampleTimes = new List<float>();

        propertyCurves = new List<AnimationCurve>();


        for (int k = 0; k < animCells.Count; k++)
        {
            foreach (string s in GetPropertyNames())
            {

                // Split the property name into components (e.g., "m_LocalPosition.x")
                string[] parts = s.Split('.');
                if (parts.Length != 3)
                {
                    Debug.LogWarning($"Unsupported property format: {s}");
                    continue;
                }

                string objName = parts[0]; // e.g., "m_LocalPosition"
                string fieldName = parts[1]; // e.g., "x"
                string subProperty = parts[2]; // e.g., "x"



                //propertyCurves.Add();
                //Debug.Log(s);


                GameObject thisObj = GameObject.Find(objName + $" {k}");
                //GameObject thisObj = animCells[k].gameObject;

                float curveValue = ExtractCurve(fieldName + "." + subProperty).Evaluate(k / (float)frameCount);

                if (thisObj != null && parts.Length == 3)
                {
                    //Debug.Log(thisObj.name + " " + curveValue);
                    //Debug.Log(fieldName + "." + subProperty);

                    if (fieldName == "localEulerAnglesRaw")
                    {
                        if (subProperty == "x")
                        {
                            thisObj.transform.localRotation = Quaternion.Euler(new Vector3(curveValue, thisObj.transform.localEulerAngles.y, thisObj.transform.localEulerAngles.z));
                        }
                        if (subProperty == "y")
                        {
                            thisObj.transform.localRotation = Quaternion.Euler(new Vector3(thisObj.transform.localEulerAngles.x, curveValue, thisObj.transform.localEulerAngles.z));
                        }
                        if (subProperty == "z")
                        {
                            thisObj.transform.localRotation = Quaternion.Euler(new Vector3(thisObj.transform.localEulerAngles.x, thisObj.transform.localEulerAngles.y, curveValue));
                        }
                    }


                    else if (fieldName == "m_LocalPosition")
                    {
                        if (subProperty == "x")
                        {
                            thisObj.transform.localPosition = new Vector3(curveValue, thisObj.transform.localPosition.y, thisObj.transform.localPosition.z);
                        }
                        if (subProperty == "y")
                        {
                            thisObj.transform.localPosition = new Vector3(thisObj.transform.localPosition.x, curveValue, thisObj.transform.localPosition.z);
                        }
                        if (subProperty == "z")
                        {
                            thisObj.transform.localPosition = new Vector3(thisObj.transform.localPosition.x, thisObj.transform.localPosition.y, curveValue);
                        }
                    }

                    else if (fieldName == "m_LocalScale")
                    {
                        if (subProperty == "x")
                        {
                            thisObj.transform.localScale = new Vector3(curveValue, thisObj.transform.localScale.y, thisObj.transform.localScale.z);
                        }
                        if (subProperty == "y")
                        {
                            thisObj.transform.localScale = new Vector3(thisObj.transform.localScale.x, curveValue, thisObj.transform.localScale.z);
                        }
                        if (subProperty == "z")
                        {
                            thisObj.transform.localScale = new Vector3(thisObj.transform.localScale.x, thisObj.transform.localScale.y, curveValue);
                        }
                    }
                    else if (fieldName == "material")
                    {
                        Renderer thisRend = thisObj.GetComponent<Renderer>();

                        Material refMat = thisRend.material;

                        Material newMat = new Material(refMat);

                        thisRend.material = newMat;

                        newMat.SetFloat(subProperty, curveValue);
                    }
                    else
                    {
                        Debug.Log("This property not used " + fieldName);
                    }
                }
                else
                {
                    Debug.Log(objName);
                }



                //cellGameObjects.Add(animCells[k].gameObject);
                //sampleTimes.Add(k / frameCount);

                //SampleInEditMode(animClip, animCells[k].gameObject, k / frameCount);
            }
        }





    }
















    private AnimationCurve ExtractCurve(string propertyName)
    {
        // Get the curve bindings for the animation clip
        var bindings = AnimationUtility.GetCurveBindings(animClip);

        // Find the binding that matches the property name
        foreach (var binding in bindings)
        {
            if (binding.propertyName == propertyName)
            {
                // Extract the curve
                return AnimationUtility.GetEditorCurve(animClip, binding);
            }
        }

        Debug.LogWarning($"No curve found for property: {propertyName}");
        return new AnimationCurve();
    }

    public List<string> GetPropertyNames()
    {
        List<string> propertyNames = new List<string>();

        if (animClip == null)
        {
            Debug.LogError("Animation Clip is not assigned.");
            return propertyNames;
        }

        // Get all curve bindings in the animation clip
        var bindings = AnimationUtility.GetCurveBindings(animClip);

        // Extract property names from the bindings
        foreach (var binding in bindings)
        {
            List<string> splitPath = binding.path.Split("/").ToList();

            string combinedString = "";

            foreach(string s in splitPath)
            {
                combinedString = s;
            }


            propertyNames.Add(combinedString + "." + binding.propertyName);
        }

        // Log the property names to the console
        Debug.Log("Property Names in Animation Clip:");
        foreach (var name in propertyNames)
        {
            Debug.Log(name);
        }

        return propertyNames;
    }
}