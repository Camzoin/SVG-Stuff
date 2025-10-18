using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace GeneratedTextureLibrary
{
    [GeneratedTextureData("Curve")]
    public class GTDCurve : GTData
    {
        public AnimationCurve curve;
        public override void UpdateTexture()
        {
            for (int x = 0; x < targetTexture.width; x++)
            {
                float t = targetTexture.width - 1 > 0 ? (float)x / (targetTexture.width - 1) : 0;

                for (int y = 0; y < targetTexture.height; y++)
                {
                    float v = curve.Evaluate(t);
                    targetTexture.SetPixel(x, y, new Color(v,v,v));
                }
            }
            targetTexture.Apply();
        }
    }
}