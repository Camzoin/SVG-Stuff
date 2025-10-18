using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEditor;
using System.IO;
using System.Linq;
using Object = UnityEngine.Object;

namespace GeneratedTextureLibrary
{
    [CreateAssetMenu(order = 325,fileName = "New Generated Texture Library", menuName = "Generated Texture Library")]
    public class GTLibrary : ScriptableObject
    {
        public static readonly Color DELETE_COLOR = new Color(1f, .4f, .4f, 1f);
        static readonly Rect UV_DEFAULT = new Rect(0f, 0f, 1f, 1f);
        static readonly Rect UV_THREE = new Rect(-1f, -1f, 3f, 3f);

#if UNITY_EDITOR
        [CustomEditor(typeof(GTLibrary))]
        public class Inspector : Editor
        {
            new GTLibrary target;
            string targetPath;

            private void OnEnable()
            {
                target = (GTLibrary)base.target;
                targetPath = AssetDatabase.GetAssetPath(target);

                InitTextureDataList();

                Undo.undoRedoPerformed += InitTextureDataList;
            }

            private void OnDisable()
            {
                AssetDatabase.SaveAssets();

                if (editors != null)
                    foreach (var editor in editors)
                        if (editor)
                            DestroyImmediate(editor);

                Undo.undoRedoPerformed -= InitTextureDataList;
            }

            List<GTData> textureDatas;
            bool[] datasCanBeResized;
            Editor[] editors;

            void InitTextureDataList()
            {
                if (textureDatas != null)
                    textureDatas.Clear();
                else
                    textureDatas = new List<GTData>();

                Object[] objectsAtPath = AssetDatabase.LoadAllAssetsAtPath(targetPath);

                foreach (Object obj in objectsAtPath)
                {
                    GTData data = obj as GTData;
                    if (data)
                        textureDatas.Add(data);
                }

                if (editors != null)
                    foreach (var editor in editors)
                        if(editor)
                            DestroyImmediate(editor);

                editors = new Editor[textureDatas.Count];
                for (int i = 0; i < editors.Length; i++)
                {
                    editors[i] = Editor.CreateEditor(textureDatas[i]);
                }

                datasCanBeResized = new bool[textureDatas.Count];
                for (int i = 0; i < textureDatas.Count; i++)
                {
                    var attributes = textureDatas[i].GetType().GetCustomAttributes(typeof(GeneratedTextureDataAttribute), false);
                    datasCanBeResized[i] =
                        attributes.Length > 0 &&
                        attributes[0] is GeneratedTextureDataAttribute gtda && 
                        gtda.canBeResized;
                }
            }

            public override void OnInspectorGUI()
            {
                for (int i = 0; i < textureDatas.Count; i++)
                {
                    EditorGUI.indentLevel = 0;
                    GTData data = textureDatas[i];

                    bool export = false;
                    GUILayout.BeginVertical("HelpBox");
                    {
                        string newName = EditorGUILayout.DelayedTextField(data.targetTexture.name);
                        if (data.targetTexture.name != newName)
                        {
                            Undo.RecordObject(data, "Generated Texture Renamed");
                            Undo.RecordObject(data.targetTexture, "Generated Texture Renamed");

                            data.name = newName;
                            data.targetTexture.name = newName;

                            Reimport(target);
                        }


                        EditorGUI.BeginChangeCheck();
                        //We sure the built in inspector bnecause it's much better at handling undo's than we are xD
                        editors[i].OnInspectorGUI();
                        if (EditorGUI.EndChangeCheck())
                        {
                            data.UpdateTexture();
                        }

                        

                        EditorGUI.indentLevel = 1;
                        if (data.foldout = EditorGUILayout.Foldout(data.foldout, "Texture Settings"))
                        {
                            EditorGUI.BeginChangeCheck();
                            TextureWrapMode newWrap = (TextureWrapMode)EditorGUILayout.EnumPopup("Wrap Mode", data.targetTexture.wrapMode);
                            FilterMode newFilter = (FilterMode)EditorGUILayout.EnumPopup("Filter Mode", data.targetTexture.filterMode);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(data.targetTexture, "Generated Texture Settings Changed");

                                data.targetTexture.wrapMode = newWrap;
                                data.targetTexture.filterMode = newFilter;

                                Reimport(target);
                            }

                            GUI.enabled = datasCanBeResized[i];
                            EditorGUI.BeginChangeCheck();
                            Vector2Int newRes = new Vector2Int();
                            EditorGUILayout.LabelField("Resolution:");
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Space(14);
                                GUILayout.Label("Width", GUILayout.ExpandWidth(false));
                                newRes.x = Math.Max(1, EditorGUILayout.DelayedIntField(data.targetTexture.width, GUILayout.ExpandWidth(false)));
                                GUILayout.Label("Height", GUILayout.ExpandWidth(false));
                                newRes.y = Math.Max(1, EditorGUILayout.DelayedIntField(data.targetTexture.height, GUILayout.ExpandWidth(false)));
                            }
                            GUILayout.EndHorizontal();
                            GUI.enabled = true;

                            TextureFormat newFormat = (TextureFormat)EditorGUILayout.EnumPopup("Texture Format", data.targetTexture.format);
                            bool isSRGB = EditorGUILayout.Toggle("Is SRGB", GraphicsFormatUtility.IsSRGBFormat(data.targetTexture.graphicsFormat));
                            bool newMip = EditorGUILayout.Toggle("Generate Mip Maps", data.targetTexture.mipmapCount != 1);
                            if (EditorGUI.EndChangeCheck())
                            {
                                GraphicsFormat gFormat = GraphicsFormatUtility.GetGraphicsFormat(newFormat, isSRGB);

                                Undo.RecordObject(data.targetTexture, "Generated Texture Settings Change");

                                data.targetTexture.Reinitialize(newRes.x, newRes.y, gFormat, newMip);
                                data.UpdateTexture();
                            }
                        }
                        EditorGUI.indentLevel = 0;
                        EditorGUI.indentLevel = 1;

                        data.preview = EditorGUILayout.Foldout(data.preview, "Preview");
                        if (data.preview)
                        {
                            GUILayout.BeginVertical("HelpBox");
                            {
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.FlexibleSpace();
                                    data.previewTiled = GUILayout.Toggle(data.previewTiled, "Tiled");
                                    GUI.color = data.previewMask == UnityEngine.Rendering.ColorWriteMask.All ? Color.white : Color.grey;
                                    if (GUILayout.Button("RGB", GUILayout.Width(40)))
                                        data.previewMask = UnityEngine.Rendering.ColorWriteMask.All;
                                    GUI.color = data.previewMask == UnityEngine.Rendering.ColorWriteMask.Red ? Color.white : Color.grey;
                                    if (GUILayout.Button("R", GUILayout.Width(25)))
                                        data.previewMask = UnityEngine.Rendering.ColorWriteMask.Red;
                                    GUI.color = data.previewMask == UnityEngine.Rendering.ColorWriteMask.Green ? Color.white : Color.grey;
                                    if (GUILayout.Button("G", GUILayout.Width(25)))
                                        data.previewMask = UnityEngine.Rendering.ColorWriteMask.Green;
                                    GUI.color = data.previewMask == UnityEngine.Rendering.ColorWriteMask.Blue ? Color.white : Color.grey;
                                    if (GUILayout.Button("B", GUILayout.Width(25)))
                                        data.previewMask = UnityEngine.Rendering.ColorWriteMask.Blue;
                                    GUI.color = data.previewMask == UnityEngine.Rendering.ColorWriteMask.Alpha ? Color.white : Color.grey;
                                    if (GUILayout.Button("A", GUILayout.Width(25)))
                                        data.previewMask = UnityEngine.Rendering.ColorWriteMask.Alpha;
                                    GUI.color = Color.white;
                                }
                                GUILayout.EndHorizontal();
                                float currentViewWidth = EditorGUIUtility.currentViewWidth - 32f;
                                float ratio = currentViewWidth / data.targetTexture.width;

                                Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(data.targetTexture.height * ratio));
                                //GUI.DrawTexture(rect, data.targetTexture);
                                //GUI.DrawTextureWithTexCoords(rect, data.targetTexture, threeByThree ? UV_THREE : UV_DEFAULT);
                                int imagecount = data.previewTiled ? 3 : 1;
                                Vector2 third = new Vector2(Mathf.Ceil(rect.size.x / imagecount), Mathf.Ceil(rect.size.y / imagecount));
                                for (int imgx = 0; imgx < imagecount; imgx++)
                                {
                                    for (int imgy = 0; imgy < imagecount; imgy++)
                                    {
                                        Rect imgRect = new Rect(rect.position + new Vector2(third.x * imgx, third.y*imgy), third);
                                        if (data.previewMask == UnityEngine.Rendering.ColorWriteMask.Alpha)
                                            EditorGUI.DrawTextureAlpha(imgRect, data.targetTexture, ScaleMode.StretchToFill, 0, 0);
                                        else
                                            EditorGUI.DrawPreviewTexture(imgRect, data.targetTexture, mat: null, ScaleMode.StretchToFill, 0, 0, data.previewMask);
                                    }

                                }
                            }
                            GUILayout.EndVertical();
                        }
                        EditorGUI.indentLevel = 0;




                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button("Export to PNG"))
                                export = true;

                            GUI.color = DELETE_COLOR;
                            if (GUILayout.Button("Delete", GUILayout.Width(60)))
                            {
                                GUI.FocusControl(null);
                                Undo.DestroyObjectImmediate(data);
                                Undo.DestroyObjectImmediate(data.targetTexture);
                                Reimport(target);
                                InitTextureDataList();
                            }
                            GUI.color = Color.white;
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();

                    if (export)
                    {
                        string path = EditorUtility.SaveFilePanel($"Save '{data.name}' Gradient as...", AssetDatabase.GetAssetPath(target) + ".png", data.name, "png");
                        if (path.Length != 0)
                        {
                            byte[] bytes = data.targetTexture.EncodeToPNG();
                            File.WriteAllBytes(path, bytes);

                            if (path.Contains(Application.dataPath))
                            {
                                string relativePath = path.Replace(Application.dataPath, "Assets");
                                AssetDatabase.ImportAsset(relativePath);
                            }
                        }
                    }
                }

                if (GUILayout.Button("Add New Gernerated Texture"))
                {
                    GenericMenu creationMenu = new GenericMenu();


                    foreach (var type in GetAllGTDTypes()) 
                    {
                        var attribute = (GeneratedTextureDataAttribute) Attribute.GetCustomAttribute(type, typeof(GeneratedTextureDataAttribute));
                        creationMenu.AddItem(new GUIContent(attribute.displayName), false, MakeNewTexture, type);
                    }
                    creationMenu.ShowAsContext();
                }
            }

            IEnumerable<Type> GetAllGTDTypes()
            {
                return from assembly in AppDomain.CurrentDomain.GetAssemblies()
                       from type in assembly.GetTypes()
                       where type.IsDefined(typeof(GeneratedTextureDataAttribute), false) 
                       where type.IsSubclassOf(typeof(GTData))
                       select type;
            }

            public enum TextureType
            {
                Gradient,
                Curve,
                TwoGradients,
                TwoCurves
            }

            private void MakeNewTexture(object selection)
            {
                Type type = (Type)selection; 
                var attribute = (GeneratedTextureDataAttribute)Attribute.GetCustomAttribute(type, typeof(GeneratedTextureDataAttribute));
                string name = attribute.displayName;

                GTData data = (GTData)ScriptableObject.CreateInstance(type);

                data.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;

                string[] names = new string[textureDatas.Count];
                for (int i = 0; i < textureDatas.Count; i++)
                    names[i] = textureDatas[i].name;

                string newName = ObjectNames.GetUniqueName(names, $"New {name} Texture");

                data.name = newName;

                Texture2D newtex = new Texture2D(64, 64, TextureFormat.ARGB32, false);
                newtex.name = newName;
                AssetDatabase.AddObjectToAsset(newtex, target);

                data.targetTexture = newtex;
                AssetDatabase.AddObjectToAsset(data, newtex);

                Undo.RegisterCreatedObjectUndo(data, "Generated Texture Created");
                Undo.RegisterCreatedObjectUndo(newtex, "Generated Texture Created");

                Reimport(target);
                InitTextureDataList();
            }

            //private void GenerateTexture(GradientTextureLibrary.Data data)
            //{
            //    Texture2D newTexture = new Texture2D((int)data.resolution, 1, data.textureFormat, data.generateMipMaps);
            //    newTexture.name = data.name;
            //    newTexture.filterMode = FilterMode.Bilinear;
            //    newTexture.wrapMode = TextureWrapMode.Clamp;
            //
            //    GradientTextureLibrary.WriteGradientToTexture(newTexture, data.gradient);
            //
            //    data.texture = newTexture;
            //
            //    AssetDatabase.AddObjectToAsset(data.texture, target);
            //    Reimport(target);
            //}


            static bool toReimport;
            public void Reimport(Object target)
            {
                if (toReimport)
                    return;

                toReimport = true;

                //EditorApplication.delayCall += ReimportAsset(target);string assetName = target.name;
                string assetName = target.name;
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(target), assetName + "(PROCESSING)");
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(target), assetName);
                AssetDatabase.Refresh();

                toReimport = false;
            }
            public static EditorApplication.CallbackFunction ReimportAsset(Object target)
            {
                //Black magic to get the project view to refresh
                string assetName = target.name;
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(target), assetName + "(PROCESSING)");
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(target), assetName);
                AssetDatabase.Refresh();

                toReimport = false;
                return null;
            }
        }
#endif
    }
}