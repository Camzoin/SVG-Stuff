using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneratedTextureLibrary
{
    public abstract class GTDataWithImportedSample : GTData
    {
        [Tooltip("The texture asset that this generated texture will base it's generation on. The generator will run any time this texture is re-imported.")]
        public Texture2D baseTexture;

        public abstract void UpdateTextureOnImport();
    }
}