using UnityEngine;
using System.Collections.Generic;


[ExecuteAlways]

public class PagePlayer : MonoBehaviour
{
    public List<Texture2D> pageList = new List<Texture2D>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.GetComponent<Renderer>().material.SetTexture("_Texture2D", pageList[Mathf.FloorToInt(Time.time % pageList.Count)]);
    }
}
