using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneratedTextureLibrary
{
    [GeneratedTextureData("Two Curves")]
    public class GTDTwoCurves : GTData
    {
        public AnimationCurve upperCurve = new AnimationCurve();
        public AnimationCurve lowerCurve = new AnimationCurve();

        public override void UpdateTexture()
        {
            for (int x = 0; x < targetTexture.width; x++)
            {
                float tx = targetTexture.width - 1 > 0 ? (float)x / (targetTexture.width - 1) : 0;

                for (int y = 0; y < targetTexture.height; y++)
                {
                    float ty = targetTexture.height - 1 > 0 ? (float)y / (targetTexture.height - 1) : 0;
                    ty = 1f - ty;

                    float f1 = upperCurve.Evaluate(tx);
                    float f2 = lowerCurve.Evaluate(tx);
                    float final = Mathf.Lerp(f1, f2, ty);
                    targetTexture.SetPixel(x, y, new Color(final,final,final));
                }
            }
            targetTexture.Apply();
        }
    }
}