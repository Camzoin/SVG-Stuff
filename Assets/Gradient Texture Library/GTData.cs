using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GeneratedTextureLibrary
{
    public abstract class GTData : ScriptableObject, ISerializationCallbackReceiver
    {
        [System.NonSerialized] internal bool foldout;
        [System.NonSerialized] internal bool preview;
        [System.NonSerialized] internal bool previewTiled;
        [System.NonSerialized] internal UnityEngine.Rendering.ColorWriteMask previewMask = UnityEngine.Rendering.ColorWriteMask.All;

        [HideInInspector,SerializeField] internal Texture2D targetTexture;

        public abstract void UpdateTexture();

        public void OnBeforeSerialize() {}
        public void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += UpdateTexture;
#endif
        }
    }

    public class GeneratedTextureDataAttribute : System.Attribute
    {
        public string displayName;
        public bool canBeResized;
        public GeneratedTextureDataAttribute(string displayName, bool canBeResized = true) 
        { 
            this.displayName = displayName; 
            this.canBeResized = canBeResized; 
        }
    }
}