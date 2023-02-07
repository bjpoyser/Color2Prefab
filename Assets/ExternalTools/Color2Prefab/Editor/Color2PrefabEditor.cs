using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Color2Prefab
{
    [CustomEditor(typeof(ColorDictionaryScriptableObject))]
    public class Color2PrefabEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open Editor"))
            {
                Color2PrefabEditorWindow.Open((ColorDictionaryScriptableObject)target);
            }
        }
    }

    public class AssetHandler
    {
        [OnOpenAsset()]
        public static bool OpenEditor(int instanceId, int line)
        {
            ColorDictionaryScriptableObject colorDictionary = EditorUtility.InstanceIDToObject(instanceId) as ColorDictionaryScriptableObject;
            if(colorDictionary != null)
            {
                Color2PrefabEditorWindow.Open(colorDictionary);
                return true;
            }

            return false;
        }
    }
}
