using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace GeneratedTextureLibrary
{
    [GeneratedTextureData("Signed Distance Field", canBeResized = true)]
    public class GTDSDF : GTDataWithImportedSample
    {
        enum Channel
        {
            Red,
            Green,
            Blue,
            Alpha
        }

        [Delayed]
        public int distance = 32;

        public bool wrap;

        List<Task> tasks;

        public override void UpdateTexture()
        {
            if (!baseTexture)
                return;

            Shader sdfShader = Shader.Find("Hidden/GTLUtilityShader/SDFGen");
            if (sdfShader == null)
                return;

            Material sdfMat = new Material(sdfShader);
            Vector2 outputTexelSize = new Vector2(1f / targetTexture.width, 1f / targetTexture.height);
            sdfMat.SetVector("_OutputTexelSize", outputTexelSize);
            sdfMat.SetFloat("_Radius", distance);

            RenderTexture rTex = new RenderTexture(targetTexture.width, targetTexture.height, 0);
            rTex.format = RenderTextureFormat.ARGBFloat;
            //if (!UnityEngine.Experimental.Rendering.GraphicsFormatUtility.IsSRGBFormat(targetTexture.graphicsFormat))
            //{
            //}
            Graphics.Blit(baseTexture, rTex, sdfMat);

            RenderTexture.active = rTex;
            targetTexture.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            RenderTexture.active = null;
            rTex.DiscardContents();

            targetTexture.Apply();
        }


        public override void UpdateTextureOnImport()
        {
            UpdateTexture();
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(GTDSDF))]
        public class Inpsector : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if(target is GTDSDF data)
                {
                    if (GUILayout.Button("Generate"))
                    {
                        data.UpdateTextureOnImport();
                    }
                }
            }
        }
#endif

    }
}