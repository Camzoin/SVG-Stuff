using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace GeneratedTextureLibrary
{
    [GeneratedTextureData("Gradient")]
    public class GTDGradient : GTData
    {
        public UnityEngine.Gradient gradient = new UnityEngine.Gradient();

        public override void UpdateTexture()
        {
            for (int x = 0; x < targetTexture.width; x++)
            {
                float t = targetTexture.width - 1 > 0 ? (float)x / (targetTexture.width - 1) : 0;
                
                for (int y = 0; y < targetTexture.height; y++)
                {
                    targetTexture.SetPixel(x, y, gradient.Evaluate(t));
                }
            }
            targetTexture.Apply();
        }
    }
}