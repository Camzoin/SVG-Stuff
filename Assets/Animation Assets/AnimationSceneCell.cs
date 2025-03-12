using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Linq;

public class AnimationSceneCell : MonoBehaviour
{
    public Camera cellCam;

    public Camera cellNormalCam;

    public Animator cellAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

    }
    [ContextMenu("SetAnimationCell")]
    public void BuildAnimationCellsFromAnimation()
    {

    }
}
