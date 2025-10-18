using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GeneratedTextureLibrary
{
    [GeneratedTextureData("Two Gradients")]
    public class GTDTwoGradients : GTData
    {
        public UnityEngine.Gradient upperGradient = new UnityEngine.Gradient();
        public UnityEngine.Gradient lowerGradient = new UnityEngine.Gradient();

        public override void UpdateTexture()
        {
            for (int x = 0; x < targetTexture.width; x++)
            {
                float tx = targetTexture.width - 1 > 0 ? (float)x / (targetTexture.width - 1) : 0;

                for (int y = 0; y < targetTexture.height; y++)
                {
                    float ty = targetTexture.height - 1 > 0 ? (float)y / (targetTexture.height - 1) : 0;
                    ty = 1f - ty;

                    Color c1 = upperGradient.Evaluate(tx);
                    Color c2 = lowerGradient.Evaluate(tx);
                    Color final = Color.Lerp(c1, c2, ty);
                    targetTexture.SetPixel(x, y, final);
                }
            }
            targetTexture.Apply();
        }
    }
}