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

    private List<AnimationCurve> propertyCurves = new List<AnimationCurve>();

    public RenderTexture ppRenderTexture;

    public Vector2 animationCellsXY = new Vector2(4, 3);

    public float animationTimeOffset = 1;

    public float secondsOfAnimation = 1;

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

            newCell.transform.localPosition = Vector3.left * 250 * (k + 1);

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
            colorRT.width = (int)(ppRenderTexture.width / animationCellsXY.x);
            colorRT.height = (int)(ppRenderTexture.height / animationCellsXY.y);
            colorRT.Create();




            normalRT.Release();
            normalRT.width = (int)(ppRenderTexture.width / animationCellsXY.x);
            normalRT.height = (int)(ppRenderTexture.height / animationCellsXY.y);
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
            foreach(Animation animator in referenceCell.transform.GetComponentsInChildren<Animation>())
            {
                //InitializeCurveData(animator.clip);

                //foreach (var data in transformCurves)
                //{
                //    UpdateTransform(data, ((k / ((float)(frameCount) / (float)secondsOfAnimation))) + (animationTimeOffset));
                //}





                foreach (string s in GetPropertyNames(animator.clip))
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


                    GameObject animParent = GameObject.Find(animator.name + $" {k}");

                    Debug.Log(animParent.name);


                    GameObject thisObj = animParent;


                    if (animator.name + $" {k}" != animParent.name)
                    {
                        thisObj = FindDeepChild(animParent.transform, objName + $" {k}").gameObject;
                    }


                    if (FindDeepChild(animParent.transform, objName + $" {k}") != null)
                    {
                        thisObj = FindDeepChild(animParent.transform, objName + $" {k}").gameObject;
                    }



                    //GameObject thisObj = animCells[k].gameObject;

                    float curveValue = ExtractCurve(fieldName + "." + subProperty, animator.clip).Evaluate(((k / ((float)(frameCount) / (float)secondsOfAnimation))) + (animationTimeOffset));




                    if (fieldName == "m_LocalRotation")
                    {
                        propertyCurves.Add(ExtractCurve(fieldName + "." + subProperty, animator.clip));

                    }



                    //Debug.Log(((k / ((float)frameCount / (float)secondsOfAnimation))) + (animationTimeOffset)  + " FUCK " + animator.clip.length);


                    //Debug.Log(ExtractCurve(fieldName + "." + subProperty, clipInfo[0].clip).Evaluate((k / (float)frameCount) * (clipInfo[0].clip.length * clipInfo[0].clip.frameRate)));


                    if (thisObj != null && parts.Length == 3)
                    {
                        //Debug.Log(thisObj.name + " " + curveValue);
                        //Debug.Log(fieldName + "." + subProperty);

                        if (fieldName == "localEulerAnglesRaw")
                        {
                            Debug.Log(fieldName + subProperty + " " + curveValue + " At " + ((k / ((float)(frameCount) / (float)secondsOfAnimation))) + (animationTimeOffset) + " In " + animator.clip.name);

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




                        if (fieldName == "m_LocalRotation")
                        {
                            //float curveValueQX = ExtractCurve(fieldName + ".x", animator.clip).Evaluate(((k / ((float)(frameCount) / (float)secondsOfAnimation))) + (animationTimeOffset));
                            //float curveValueQY = ExtractCurve(fieldName + ".y", animator.clip).Evaluate(((k / ((float)(frameCount) / (float)secondsOfAnimation))) + (animationTimeOffset));
                            //float curveValueQZ = ExtractCurve(fieldName + ".z", animator.clip).Evaluate(((k / ((float)(frameCount) / (float)secondsOfAnimation))) + (animationTimeOffset));
                            //float curveValueQW = ExtractCurve(fieldName + ".w", animator.clip).Evaluate(((k / ((float)(frameCount) / (float)secondsOfAnimation))) + (animationTimeOffset));


                            //Quaternion newRotation = new Quaternion(curveValueQX, curveValueQY, curveValueQZ, curveValueQW);

                            //newRotation.Normalize();

                            //Vector3 rotEuler = newRotation.eulerAngles;

                            ////newRotation = newRotation * Quaternion.Euler(0, 180, 0);



                            //thisObj.transform.localRotation = newRotation;


                            //if (subProperty == "x")
                            //{
                            //    thisObj.transform.localRotation = new Quaternion(curveValue, thisObj.transform.localRotation.y, thisObj.transform.localRotation.z, thisObj.transform.localRotation.w);
                            //}
                            //if (subProperty == "y")
                            //{
                            //    thisObj.transform.localRotation = new Quaternion(thisObj.transform.localRotation.x, curveValue, thisObj.transform.localRotation.z, thisObj.transform.localRotation.w);
                            //}
                            //if (subProperty == "z")
                            //{
                            //    thisObj.transform.localRotation = new Quaternion(thisObj.transform.localRotation.x, thisObj.transform.localRotation.y, curveValue, thisObj.transform.localRotation.w);
                            //}
                            //if (subProperty == "w")
                            //{
                            //    thisObj.transform.localRotation = new Quaternion(thisObj.transform.localRotation.x, thisObj.transform.localRotation.y, thisObj.transform.localRotation.z, curveValue);
                            //}
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
                            Renderer thisRend = thisObj.GetComponent<MeshRenderer>();

                            Material refMat = thisRend.material;

                            Material newMat = new Material(refMat);

                            newMat.SetFloat(subProperty, curveValue);

                            newMat.name = k.ToString();

                            thisRend.material = newMat;



                            Debug.Log("This property not used " + thisRend.name + " " + subProperty + " " + curveValue);


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






    }

























    private List<TransformCurveData> transformCurves = new List<TransformCurveData>();

    [System.Serializable]
    private class TransformCurveData
    {
        public Transform transform;
        public Dictionary<string, AnimationCurve> positionCurves = new Dictionary<string, AnimationCurve>();
        public Dictionary<string, AnimationCurve> rotationCurves = new Dictionary<string, AnimationCurve>();
        public Dictionary<string, AnimationCurve> scaleCurves = new Dictionary<string, AnimationCurve>();
    }

    void InitializeCurveData(AnimationClip animationClip)
    {
        foreach (var binding in AnimationUtility.GetCurveBindings(animationClip))
        {
            if (binding.type != typeof(Transform)) continue;

            Transform targetTransform = FindTransform(binding.path);
            if (targetTransform == null) continue;

            AnimationCurve curve = AnimationUtility.GetEditorCurve(animationClip, binding);
            string[] propertyParts = binding.propertyName.Split('.');

            if (propertyParts.Length != 2) continue;

            string propertyType = propertyParts[0];
            string component = propertyParts[1];

            TransformCurveData data = GetOrCreateTransformData(targetTransform);

            switch (propertyType)
            {
                case "m_LocalPosition":
                    data.positionCurves[component] = curve;
                    break;
                case "m_LocalRotation":
                    data.rotationCurves[component] = curve;
                    break;
                case "m_LocalScale":
                    data.scaleCurves[component] = curve;
                    break;
            }
        }
    }

    TransformCurveData GetOrCreateTransformData(Transform target)
    {
        foreach (var data in transformCurves)
        {
            if (data.transform == target) return data;
        }

        TransformCurveData newData = new TransformCurveData();
        newData.transform = target;
        transformCurves.Add(newData);
        return newData;
    }

    Transform FindTransform(string path)
    {
        if (string.IsNullOrEmpty(path)) return transform;

        Transform result = transform;
        foreach (string childName in path.Split('/'))
        {
            result = result.Find(childName);
            if (result == null) break;
        }
        return result;
    }


    void UpdateTransform(TransformCurveData data, float evalTime)
    {
        // Update position
        Vector3 position = data.transform.localPosition;
        UpdateVectorFromCurves(data.positionCurves, ref position, evalTime);
        data.transform.localPosition = position;

        // Update rotation
        if (data.rotationCurves.Count > 0)
        {
            Quaternion rotation = data.transform.localRotation;
            UpdateQuaternionFromCurves(data.rotationCurves, ref rotation, evalTime);
            data.transform.localRotation = rotation.normalized;
        }

        // Update scale
        Vector3 scale = data.transform.localScale;
        UpdateVectorFromCurves(data.scaleCurves, ref scale, evalTime);
        data.transform.localScale = scale;
    }

    void UpdateVectorFromCurves(Dictionary<string, AnimationCurve> curves, ref Vector3 vector, float evalTime)
    {
        if (curves.TryGetValue("x", out AnimationCurve xCurve)) vector.x = xCurve.Evaluate(evalTime);
        if (curves.TryGetValue("y", out AnimationCurve yCurve)) vector.y = yCurve.Evaluate(evalTime);
        if (curves.TryGetValue("z", out AnimationCurve zCurve)) vector.z = zCurve.Evaluate(evalTime);
    }

    void UpdateQuaternionFromCurves(Dictionary<string, AnimationCurve> curves, ref Quaternion quaternion, float evalTime)
    {
        if (curves.TryGetValue("x", out AnimationCurve xCurve)) quaternion.x = xCurve.Evaluate(evalTime);
        if (curves.TryGetValue("y", out AnimationCurve yCurve)) quaternion.y = yCurve.Evaluate(evalTime);
        if (curves.TryGetValue("z", out AnimationCurve zCurve)) quaternion.z = zCurve.Evaluate(evalTime);
        if (curves.TryGetValue("w", out AnimationCurve wCurve)) quaternion.w = wCurve.Evaluate(evalTime);
    }





















































    public static Transform FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }

            // Recursively search the child's children
            Transform foundChild = FindDeepChild(child, childName);
            if (foundChild != null)
            {
                return foundChild;
            }
        }
        return null; // Return null if not found
    }
















    private AnimationCurve ExtractCurve(string propertyName, AnimationClip clip)
    {
        // Get the curve bindings for the animation clip
        var bindings = AnimationUtility.GetCurveBindings(clip);

        // Find the binding that matches the property name
        foreach (var binding in bindings)
        {
            if (binding.propertyName == propertyName)
            {
                // Extract the curve
                return AnimationUtility.GetEditorCurve(clip, binding);
            }
        }

        Debug.LogWarning($"No curve found for property: {propertyName}");
        return new AnimationCurve();
    }

    public List<string> GetPropertyNames(AnimationClip clip)
    {
        List<string> propertyNames = new List<string>();

        if (clip == null)
        {
            Debug.LogError("Animation Clip is not assigned.");
            return propertyNames;
        }

        // Get all curve bindings in the animation clip
        var bindings = AnimationUtility.GetCurveBindings(clip);

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