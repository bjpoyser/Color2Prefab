using Color2Prefab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExtendedEditorWindow : EditorWindow
{
    protected SerializedObject serializedObject;
    protected SerializedProperty currentProperty;
    protected SerializedProperty imageProperty;
    protected SerializedProperty boolProperty;

    private string selectedPropertyPath;
    protected SerializedProperty selectedProperty;

    protected void DrawProperties(SerializedProperty prop)
    {
        string lastPropPath = string.Empty;
        foreach (SerializedProperty p in prop)
        {
            if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
            {
                EditorGUILayout.BeginHorizontal();
                p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                EditorGUILayout.EndHorizontal();

                if (p.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    DrawProperties(p);
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) { continue; }
                lastPropPath = p.propertyPath;
                EditorGUILayout.PropertyField(p);
            }
        }
    }

    protected void DrawSidebar(SerializedProperty prop, ColorDictionaryScriptableObject dictionary)
    {
        for (int i = 0; i < prop.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(prop.GetArrayElementAtIndex(i).displayName))
            {
                selectedPropertyPath = prop.GetArrayElementAtIndex(i).propertyPath;
                GUI.FocusControl(null);
            }
            if (GUILayout.Button("x", GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
            {
                YesNoPopup.Open(dictionary, i);
            }
            EditorGUI.BeginChangeCheck();
            dictionary.colorDictionary[i].canBeModified = EditorGUILayout.Toggle(
                new GUIContent("",
                    "Uncheck this if you don’t want to generate or delete the GameObjects linked to this element, otherwise keep it checked."
                ),
                dictionary.colorDictionary[i].canBeModified,
                GUILayout.MaxWidth(15), GUILayout.MaxHeight(20));
            if (EditorGUI.EndChangeCheck())
            {
                Color2PrefabEditorWindow.Open(dictionary);
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button(new GUIContent("Add Element", "Adds a new element to the dictionary")))
        {
            Color2PrefabClass newElement = new Color2PrefabClass();
            newElement.elementName = "New Element";
            newElement.canBeModified = true;
            dictionary.colorDictionary.Add(newElement);
            Color2PrefabEditorWindow.Open(dictionary);
            GUI.FocusControl(null);
        }
        if (!string.IsNullOrEmpty(selectedPropertyPath))
        {
            selectedProperty = serializedObject.FindProperty(selectedPropertyPath);
        }
    }
}

public class YesNoPopup : EditorWindow
{
    private static ColorDictionaryScriptableObject currentDictionary;
    private static int currentIndex;

    public static void Open(ColorDictionaryScriptableObject dictionary, int index)
    {
        currentDictionary = dictionary;
        currentIndex = index;

        YesNoPopup window = CreateInstance<YesNoPopup>();
        window.position = new Rect(Screen.currentResolution.width * 0.3f, Screen.currentResolution.height * 0.4f, 500, 72);
        window.ShowPopup();
    }

    void OnGUI()
    {
        GUIStyle questionTextStyle = new GUIStyle(GUI.skin.label);
        questionTextStyle.alignment = TextAnchor.MiddleCenter;
        questionTextStyle.fontSize = 14;

        //Sets the background color for buttons and fields
        GUI.backgroundColor = new Color(0.4f, 1, 0.01f);
        
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.Space(2f);
        EditorGUILayout.LabelField($"Do you want to remove \"{currentDictionary.colorDictionary[currentIndex].elementName}\" from the dictionary?", questionTextStyle);
        EditorGUILayout.Space(5f);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10f);
        if (GUILayout.Button("Yes"))
        {
            DeleteElement();
            this.Close();
        }
        EditorGUILayout.Space(10f);
        if (GUILayout.Button("No")) this.Close();
        EditorGUILayout.Space(10f);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(10f);
        EditorGUILayout.EndVertical();
    }

    private void DeleteElement()
    {
        currentDictionary.colorDictionary.RemoveAt(currentIndex);
        Color2PrefabEditorWindow.Open(currentDictionary);
        GUI.FocusControl(null);
    }
}

public class OkPopup : EditorWindow
{
    private static string _message;
    public static void Open(string message)
    {
        _message = message;
        OkPopup window = CreateInstance<OkPopup>();
        window.position = new Rect(Screen.currentResolution.width * 0.3f, Screen.currentResolution.height * 0.4f, 500, 72);
        window.ShowPopup();
    }

    void OnGUI()
    {
        GUIStyle questionTextStyle = new GUIStyle(GUI.skin.label);
        questionTextStyle.alignment = TextAnchor.MiddleCenter;
        questionTextStyle.fontSize = 14;

        //Sets the background color for buttons and fields
        GUI.backgroundColor = new Color(0.4f, 1, 0.01f);

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.Space(2f);
        EditorGUILayout.LabelField($"{_message}", questionTextStyle);
        EditorGUILayout.Space(5f);
        if (GUILayout.Button("Ok"))
        {
            this.Close();
        }
        EditorGUILayout.Space(10f);
        EditorGUILayout.EndVertical();
    }
}