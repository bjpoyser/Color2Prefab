using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Color2Prefab
{
    public class Color2PrefabEditorWindow : ExtendedEditorWindow
    {
        const string VERSION_TEXT = "1.6";

        #region Private Variables
        Vector2 sidebarScrollPosition = Vector2.zero;
        Vector2 dictionariesListScrollPosition = Vector2.zero;
        private float _separatorSize = 20;
        private static bool _drawDictionaryList;
        private static ColorDictionaryScriptableObject _colorDictionaryObj;
        #endregion

        #region Serialized Properties
        private SerializedProperty _levelImage;
        private SerializedProperty _levelOrientation;
        private SerializedProperty _colorDictionary;
        #endregion

        #region Editor
        [MenuItem("Tools/Color 2 Prefab")]
        public static void OpenFromMenu()
        {
            Open(_colorDictionaryObj, true);
        }

        public static void Open(ColorDictionaryScriptableObject colorDictionary, bool drawDictionaryList = false)
        {
            Color2PrefabEditorWindow window = GetWindow<Color2PrefabEditorWindow>("Color 2 Prefab");
            if(colorDictionary != null)
            {
                window.serializedObject = new SerializedObject(colorDictionary);
                _colorDictionaryObj = colorDictionary;
                _drawDictionaryList = drawDictionaryList;
            }
        }

        private Rect logoRect;
        private float imageSizePercentaje;
        private Rect portfolioLabelRect, versionLabelRect;

        private void OnGUI()
        {
            //Sets the style for the texts, buttons and window
            #region Styles
            minSize = new Vector2(520, 760);

            GUIStyle titleTextStyle = new GUIStyle();
            titleTextStyle.alignment = TextAnchor.MiddleCenter;
            titleTextStyle.fontStyle = FontStyle.Bold;
            titleTextStyle.fontSize = 30;

            GUIStyle buttonTextStyle = new GUIStyle(GUI.skin.button);
            buttonTextStyle.fontSize = 14;
            buttonTextStyle.padding = new RectOffset(5, 5, 10, 10);

            GUIStyle versionTextStyle = new GUIStyle(GUI.skin.label);
            versionTextStyle.alignment = TextAnchor.MiddleCenter;
            versionTextStyle.fontSize = 14;

            GUIStyle subtitleTextStyle = new GUIStyle(GUI.skin.label);
            subtitleTextStyle.alignment = TextAnchor.MiddleCenter;
            subtitleTextStyle.fontStyle = FontStyle.Bold;
            subtitleTextStyle.fontSize = 14;

            //Sets the background color for buttons and fields
            GUI.backgroundColor = new Color(0.4f, 1, 0.01f);

            //Baner Alignment
            float center = Screen.width / 2.0f;
            logoRect = new Rect(center - 180, 20, 358, 152);
            imageSizePercentaje = 1f;
            portfolioLabelRect = new Rect(center - 130, 170, 100, 30);
            versionLabelRect = new Rect(center + 30, 170, 100, 30);
            #endregion

            //Checks if it should load the dictionary list
            if (serializedObject == null || _drawDictionaryList)
            {
                DrawDictionaryList();
                return;
            }

            //Initialize the tool's properties
            #region Init Props
            _levelImage = serializedObject.FindProperty("levelImage");
            _levelOrientation = serializedObject.FindProperty("levelOrientation");
            _colorDictionary = serializedObject.FindProperty("colorDictionary");

            if (Event.current.type == EventType.MouseUp && portfolioLabelRect.Contains(Event.current.mousePosition))
                Application.OpenURL("https://bjpoyser.me/#/");
            if (Event.current.type == EventType.MouseUp && logoRect.Contains(Event.current.mousePosition))
                Application.OpenURL("https://bjpoyser.me/#/");

            Texture2D logo = Resources.Load<Texture2D>("Media/Logo");
            #endregion

            //If colorDictionary doesn't exist it draws the dictionary list
            if(_colorDictionaryObj == null)
            {
                DrawDictionaryList();
                return;
            }

            //Tool's Information
            #region Banner
            if (logo != null)
            {
                GUI.DrawTextureWithTexCoords(logoRect, logo, new Rect(0.0f, 0.0f, imageSizePercentaje, imageSizePercentaje));
                GUI.Label(portfolioLabelRect, new GUIContent("bjpoyser.me", "Touch to see creator's website"), versionTextStyle);
                GUI.Label(versionLabelRect, $"version { VERSION_TEXT}", versionTextStyle);
                GUILayout.Space(220);
            }
            else
            {
                GUILayout.Space(_separatorSize);
                if (GUILayout.Button(new GUIContent("bjpoyser.me", "Redirects to creator's website"), versionTextStyle))
                {
                    Application.OpenURL("https://bjpoyser.me/#/");
                }
                GUILayout.Label("<color=white>Color</color> <color=#0bfc03>2</color> <color=white>Prefab</color>", titleTextStyle);
                GUILayout.Label($"version {VERSION_TEXT}", versionTextStyle);
                GUILayout.Space(_separatorSize);
            }
            #endregion

            //Action Buttons
            #region Buttons
            EditorGUILayout.BeginVertical("box");
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Actions", subtitleTextStyle);
            GUILayout.Space(5f);
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.Space(5);
            if (GUILayout.Button(new GUIContent("Open Dictionary List", "Displays the dictionary list"), buttonTextStyle))
            {
                Open(_colorDictionaryObj, true);
            }
            GUILayout.Space(5);
            if (GUILayout.Button(new GUIContent("Generate Level", "Generates a level using the color dictionary"), buttonTextStyle))
            {
                Generate();
            }
            GUILayout.Space(5);
            if (GUILayout.Button(new GUIContent("Empty The Containers", "Destroys all the gameobjects of a dictionary element"), buttonTextStyle))
            {
                EmptyContainers();
            }
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            GUILayout.Space(_separatorSize);
            #endregion

            //General Settings for the tool
            #region Settings
            EditorGUILayout.BeginVertical("box");
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Setup", subtitleTextStyle);
            GUILayout.Space(5f);

            //Reference Image of the level
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(5f);
            EditorGUILayout.PropertyField(_levelImage);
            GUILayout.Space(5f);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5f);

            //Change de generation axis from (x, z) to (x, y)
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(5f);
            EditorGUILayout.PropertyField(_levelOrientation, new GUIContent("Axis"));
            GUILayout.Space(3f);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            GUILayout.Space(_separatorSize);
            #endregion

            //Just a Title
            #region Current Dictionary Title
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"{_colorDictionaryObj.name}", subtitleTextStyle);
            EditorGUILayout.EndVertical();
            #endregion

            //Draws the color dictionary elements in the sidebar
            #region Sidebar
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical("box");
            sidebarScrollPosition = GUILayout.BeginScrollView(sidebarScrollPosition, false, false, GUILayout.Width(200), GUILayout.MinHeight(200), GUILayout.MaxHeight(1000), GUILayout.ExpandHeight(true));
            DrawSidebar(_colorDictionary, _colorDictionaryObj);
            GUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            #endregion

            //Draws the config of the selected dictionary element
            #region Dictionary Element Setup
            EditorGUILayout.BeginVertical();
            if (selectedProperty != null)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField(selectedProperty.displayName, subtitleTextStyle);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
            if (selectedProperty != null)
            {
                DrawProperties(selectedProperty);
            }
            else
            {
                if (_colorDictionaryObj.colorDictionary == null || _colorDictionaryObj.colorDictionary.Count == 0)
                    EditorGUILayout.LabelField("The Dictionary is Empty");
                else
                    EditorGUILayout.LabelField("Select a dictionary's element");
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            #endregion

            //Applies the changes as soon as they are made
            Apply();
        }

        private void DrawDictionaryList()
        {
            #region Dictionary's List
            EditorGUILayout.LabelField("Select a Color Dictionary");
            GUILayout.Space(10);
            ColorDictionaryScriptableObject[] colorDictionaries = Resources.LoadAll<ColorDictionaryScriptableObject>("ColorDictionaries");

            EditorGUILayout.BeginVertical();
            dictionariesListScrollPosition = GUILayout.BeginScrollView(dictionariesListScrollPosition, false, false, GUILayout.ExpandHeight(true));

            foreach (var dictionary in colorDictionaries)
            {
                EditorGUILayout.BeginHorizontal("box", GUILayout.Height(30));
                GUILayout.Label(dictionary.name);
                if (GUILayout.Button(new GUIContent("Find", "Looks for the scriptable object in the project"), GUILayout.MaxWidth(70)))
                {
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = dictionary;
                }
                if (GUILayout.Button(new GUIContent("Open", "Opens the dictionary's setup"), GUILayout.MaxWidth(70)))
                {
                    Open(dictionary);
                    GUILayout.Space(_separatorSize);
                }
                EditorGUILayout.EndHorizontal();
            }
            #endregion

            #region New Dictionary
            if (GUILayout.Button(new GUIContent("Create New Dictionary", "Creates a new dictionary")))
            {
                string dictionaryName = "ColorDictionary";
                bool alreadyExists;
                int i = 1;
                do
                {
                    alreadyExists = Resources.Load<ColorDictionaryScriptableObject>($"ColorDictionaries/{dictionaryName}.asset");
                    foreach (var dictionary in colorDictionaries)
                    {
                        if (dictionary.name == dictionaryName)
                        {
                            dictionaryName = $"ColorDictionary({i})";
                            i++;
                            alreadyExists = true;
                            break;
                        }
                    }
                }
                while (alreadyExists);

                ColorDictionaryScriptableObject newDictionary = CreateInstance<ColorDictionaryScriptableObject>();
                AssetDatabase.CreateAsset(newDictionary, $"Assets/ExternalTools/Color2Prefab/Resources/ColorDictionaries/{dictionaryName}.asset");
                AssetDatabase.SaveAssets();

                EditorUtility.FocusProjectWindow();
                Selection.activeObject = newDictionary;
            }
            GUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            #endregion
        }
        #endregion

        #region Logic
        /// <summary>
        /// Goes over the image pixels to generate a prefab from a color
        /// </summary>
        public void Generate()
        {
            if (_colorDictionaryObj.levelImage == null)
            {
                Debug.LogError("Error NF1: Image Not Found");
                return;
            }
            if (_colorDictionaryObj.colorDictionary.Count == 0)
            {
                Debug.LogError("Error NF2: Dictionary Elements List is Empty");
                return;
            }

            CheckImage();

            for (int horizontal = 0; horizontal < _colorDictionaryObj.levelImage.width; horizontal++)
            {
                for (int vertical = 0; vertical < _colorDictionaryObj.levelImage.height; vertical++)
                {
                    SpawnPrefab3DMap(horizontal, vertical);
                }
            }
        }

        private void CheckImage()
        {
            string path = AssetDatabase.GetAssetPath(_colorDictionaryObj.levelImage);

            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);
            importer.alphaIsTransparency = true;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.isReadable = true;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }

        /// <summary>
        /// Spawn the prefab assigned to the color of the received pixel
        /// </summary>
        /// <param name="horizontal">Pixel's position in horizontal axis</param>
        /// <param name="vertical">Pixel's position in vertical axis</param>
        private void SpawnPrefab3DMap(int horizontal, int vertical)
        {
            Color pixelColor = _colorDictionaryObj.levelImage.GetPixel(horizontal, vertical);

            //If alpha is less than one it won't check the color in the dictionary
            if (pixelColor.a < 1 || pixelColor == Color.white || pixelColor == Color.black)
            {
                return;
            }

            for (int i = 0; i < _colorDictionaryObj.colorDictionary.Count; i++)
            {
                var dictionaryElement = _colorDictionaryObj.colorDictionary[i];

                Transform parentTransform;
                GameObject parentGameObject = GameObject.Find(dictionaryElement.parentName);
                if (parentGameObject == null)
                {
                    Debug.LogError($"Error NF5: Couldn't Find {dictionaryElement.elementName} Parent Container Of {_colorDictionaryObj.name} In Scene Hierarchy");
                    return;
                }
                else
                {
                    parentTransform = parentGameObject.transform;
                }

                dictionaryElement.color.a = 1;
                if (dictionaryElement.color == pixelColor)
                {
                    if (dictionaryElement.canBeModified)
                    {
                        //If it is a 2d map the position will be (x,y) if it's a 3D map it wil be (x,z)
                        Vector3 position;

                        switch (_colorDictionaryObj.levelOrientation)
                        {
                            case ColorDictionaryScriptableObject.LevelOrientation.XY: position = new Vector3(horizontal, vertical, 0); break;
                            case ColorDictionaryScriptableObject.LevelOrientation.XZ: position = new Vector3(horizontal, 0, vertical); break;
                            case ColorDictionaryScriptableObject.LevelOrientation.YZ: position = new Vector3(0, vertical, horizontal); break;
                            default: position = new Vector3(horizontal, vertical, 0); break;
                        }

                        //Makes the adjustment
                        position += dictionaryElement.positionAdjustment;

                        //Instantiate the object in the world (gameobject, parent)
                        GameObject newGameObject = (GameObject)PrefabUtility.InstantiatePrefab(dictionaryElement.prefab, parentTransform);

                        //Makes the position & rotation adjustment (when is needed)
                        try
                        {
                            newGameObject.name = $"{dictionaryElement.elementName}/{newGameObject.name}";
                            newGameObject.transform.position = position;
                            newGameObject.transform.rotation = Quaternion.Euler(dictionaryElement.rotationAdjustment);
                        }
                        catch (System.Exception)
                        {
                            Debug.LogError($"Error NF4: Missing Prefab in {dictionaryElement.elementName} Of {_colorDictionaryObj.name}");
                        }
                    }
                    return;
                }
            }
            Debug.LogError($"Error NF3: Color Not Found In Dictionary {pixelColor}");
        }

        /// <summary>
        /// Destroys all the elements inside the parent containers
        /// </summary>
        public void EmptyContainers()
        {
            foreach (var dictionaryElement in _colorDictionaryObj.colorDictionary)
            {
                if (dictionaryElement.canBeModified)
                {
                    GameObject parent = GameObject.Find(dictionaryElement.parentName);
                    if (parent == null)
                    {
                        Debug.LogError($"Error NF5: Couldn't Find Parent Container of {dictionaryElement.elementName} In Scene Hierarchy");
                    }
                    else
                    {
                        Transform parentTransform = parent.transform;
                        int i = 0;
                        if (parentTransform != null)
                        {
                            while (i < parentTransform.childCount)
                            {
                                string[] childName = parentTransform.GetChild(i).name.Split('/');
                                if (childName[0] == dictionaryElement.elementName)
                                {
                                    DestroyImmediate(parentTransform.GetChild(i).gameObject);
                                    i = 0;
                                }
                                else
                                {
                                    i++;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Apply()
        {
            serializedObject.ApplyModifiedProperties();
        }
        #endregion
    }
}
