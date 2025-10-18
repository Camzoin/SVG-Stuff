#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GeneratedTextureLibrary
{
    public class GTDWISAssetProcessor : AssetPostprocessor
    {
        static string path;
        void OnPostprocessTexture(Texture2D texture)
        {
            path = assetPath;
            Debug.Log(path);
            EditorApplication.delayCall += DelayedTextureUpdate;
        }
        
        void DelayedTextureUpdate()
        {
            Texture2D target = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            Debug.Log("Test");
            foreach (string guid in AssetDatabase.FindAssets("t:GTLibrary"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AssetDatabase.ImportAsset(path);
                Debug.Log(guid);
                Object[] objects = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (Object obj in objects)
                {
                    if (obj is GTDataWithImportedSample data)
                    {
                        Debug.Log(data.baseTexture + " vs. " + target);
                        if (data.baseTexture == target)
                        {
                            data.UpdateTextureOnImport();
                            AssetDatabase.SaveAssets();
                        }
                    }
                }
            }
            target = null;
        }
    }
}
#endif