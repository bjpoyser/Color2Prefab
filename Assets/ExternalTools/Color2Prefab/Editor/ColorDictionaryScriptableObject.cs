using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Color2Prefab
{
    [CreateAssetMenu(menuName = "Color 2 Prefab/Color Dictionary", fileName = "ColorDictionary(0)")]
    public class ColorDictionaryScriptableObject : ScriptableObject
    {
        public enum LevelOrientation
        {
            XY,
            XZ,
            YZ
        }

        [Tooltip("Set a PNG, JPG or PSD image to generate the level")]
        public Texture2D levelImage;
        [Tooltip("Select the axis of your level (XY is recommended for 2D games/ XZ is recommended for 3D games)")]
        public LevelOrientation levelOrientation = LevelOrientation.XY;
        [Tooltip("This is the color 2 prefab dictionary")]
        public List<Color2PrefabClass> colorDictionary = new List<Color2PrefabClass>();
    }

    [Serializable]
    public class Color2PrefabClass
    {
        [Header("Setup")]
        [Tooltip("This is the name of this dictionary's element. If you leave it empty this name will be the gameobject's name")]
        public string elementName;

        [Tooltip("Write down the name of the parent container in the same way it's in the hierarchy")]
        public string parentName;
        [HideInInspector]
        public bool canBeModified;

        [Header("Color and Prefab")]
        [Tooltip("Set a color used in the level image")]
        public Color color;

        [Tooltip("Asign the prefab that'll be generated with the selected color")]
        public GameObject prefab;

        [Header("Adjustments")]
        [Tooltip("Set a position adjustment to these gameObjects")]
        public Vector3 positionAdjustment = Vector3.zero;

        [Tooltip("Set a rotation adjustment to these gameObjects")]
        public Vector3 rotationAdjustment = Vector3.zero;

        [HideInInspector]
        public bool isNotNew;
    }
}
