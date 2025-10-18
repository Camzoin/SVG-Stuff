using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneratedTextureLibrary {
    public class GTDEquation : GTData
    {
        const string variables = "xyrgb";

        public string red;
        public string green;
        public string blue;
        public string alpha;

        public override void UpdateTexture()
        {
            string[] tempEquations = new string[] { red, green, blue, alpha };

            //   😈

            var tuplearray = new (string a, string b)[variables.Length*variables.Length];
            int t = 0;
            for (int a = 0; a < variables.Length; a++)
                for (int b = 0; b < variables.Length; b++, t++)
                    tuplearray[t] = (variables[a].ToString(), variables[b].ToString());

            for (int i = 0; i < tempEquations.Length; i++)
                foreach (var tuple in tuplearray)
                    while (tempEquations[i].Contains(tuple.a + tuple.b))
                        tempEquations[i].Replace(tuple.a + tuple.b, $"{tuple.a}*{tuple.b}");



#if UNITY_EDITOR
            for (int x = 0; x < targetTexture.width; x++)
            {
                float tx = targetTexture.width - 1 > 0 ? (float)x / (targetTexture.width - 1) : 0;

                for (int y = 0; y < targetTexture.height; y++)
                {
                    float ty = targetTexture.height - 1 > 0 ? (float)y / (targetTexture.height - 1) : 0;

                    string[] fullEquations = new string[] {tempEquations[0],tempEquations[1],tempEquations[2],tempEquations[3]};

                    for (int i = 0; i < fullEquations.Length; i++)
                        fullEquations[i] = fullEquations[i].Replace("x", tx.ToString());
                    for (int i = 0; i < fullEquations.Length; i++)
                        fullEquations[i] = fullEquations[i].Replace("y", ty.ToString());

                    bool sr = UnityEngine.ExpressionEvaluator.Evaluate(fullEquations[0], out float r);

                    for (int i = 0; i < fullEquations.Length; i++)
                        fullEquations[i] = fullEquations[i].Replace("r", r.ToString());

                    bool sg = UnityEngine.ExpressionEvaluator.Evaluate(fullEquations[1], out float g);

                    for (int i = 0; i < fullEquations.Length; i++)
                        fullEquations[i] = fullEquations[i].Replace("g", g.ToString());

                    bool sb = UnityEngine.ExpressionEvaluator.Evaluate(fullEquations[2], out float b);

                    for (int i = 0; i < fullEquations.Length; i++)
                        fullEquations[i] = fullEquations[i].Replace("b", b.ToString());

                    bool sa = UnityEngine.ExpressionEvaluator.Evaluate(fullEquations[3], out float a);

                    targetTexture.SetPixel(x, y, new Color(r,g,b,a));

                }
            }
            targetTexture.Apply();
#endif



        }

    }
    public static class ExtentionClass
    {
        public static Vector3 Flatten(this Vector3 vector)
        {
            return new Vector3(vector.x, 0f, vector.z);
        }
    }
}