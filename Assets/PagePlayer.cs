using UnityEngine;
using System.Collections.Generic;


//[ExecuteAlways]

public class PagePlayer : MonoBehaviour
{
    public List<Texture2D> pageList = new List<Texture2D>();

    public float timeMulti = 1;

    public float colorChange = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.GetComponent<Renderer>().material.SetTexture("_Texture2D", pageList[Mathf.FloorToInt(((Time.time - (Time.deltaTime * 2)) * timeMulti) % pageList.Count)]);
        this.gameObject.GetComponent<Renderer>().material.SetFloat("_HueAdd", Mathf.FloorToInt(((Time.time - (Time.deltaTime * 2)) * timeMulti)) * colorChange);
    }
}
