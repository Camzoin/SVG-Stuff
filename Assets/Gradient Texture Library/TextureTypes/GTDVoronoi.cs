using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GeneratedTextureLibrary
{
    [GeneratedTextureData("Voronoi", canBeResized = true)]
    public class GTDVoronoi : GTData
    {
        static Vector4[] fillerArray = new Vector4[900];

        public enum DistanceType
        {
            Worley = 0,
            Manhattan = 1
        }

        public enum ImageType
        {
            Packed = 0,
            Distance = 1,
            Edge = 2,
            UV = 3,
            Colors = 4
        }

        [Header("Generation")]
        public DistanceType type = DistanceType.Worley;
        [Range(2,100)]
        public int density = 10;
        [Range(0.1f, 4f)]
        public float distanceOffset = 1f;
        [Range(0, 1)]
        public float randomness = 0f;

        [Header("Output")]
        public ImageType imageType = ImageType.Packed;
        public bool wrapped = false;

        [Space(15)]
        public int seed;

        public override void UpdateTexture()
        {
            float radius = 1f / density;
            //PoissonDiscSampler sampler = new PoissonDiscSampler(1f, 1f, radius, seed, randomness);
            
            //List<Vector4> samples = new List<Vector4>(sampler.V4Samples(wrapped));

            Shader shader = Shader.Find("Hidden/GTLUtilityShader/VoronoiGenShader_Worley");
            if (shader == null)
                return;

            float radiusMult;
            switch (type)
            {
                case DistanceType.Worley:
                    radiusMult = 1.5f;
                    break;
                case DistanceType.Manhattan:
                    radiusMult = 2.25f;
                    break;
                    break;
                default:
                    radiusMult = 1.5f;
                    break;
            }

            Material vMat = new Material(shader);
            //vMat.SetInt("_SampleCount", samples.Count);
            vMat.SetFloat("_MaxDist", radius * radiusMult * distanceOffset);
            vMat.SetInt("_Mode", (int)type);
            vMat.SetInt("_Type", (int)imageType);

            Shader.SetGlobalVectorArray("GTLUtil_Voronoi_DiskSamples", fillerArray);
            //Shader.SetGlobalVectorArray("GTLUtil_Voronoi_DiskSamples", samples);

            RenderTexture rTex = new RenderTexture(targetTexture.width, targetTexture.height, 0);
            Graphics.Blit(rTex, rTex, vMat);

            RenderTexture.active = rTex;
            targetTexture.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            RenderTexture.active = null;
            rTex.DiscardContents();

            targetTexture.Apply();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GTDVoronoi))]
    public class Inpsector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (target is GTDVoronoi data)
            {
                if (GUILayout.Button("Randomize Seed"))
                {
                    data.seed = Random.Range(-100000, 100000);
                }
            }
        }
    }
#endif

}
