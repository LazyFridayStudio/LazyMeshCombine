#region NameSpaces

using System;
using UnityEngine;
using UnityEditor;
using System.Linq;

#endregion

//================================================================
//							IMPORTANT
//================================================================
//				Copyright LazyFridayStudio
//DO NOT SELL THIS CODE OR REDISTRIBUTE WITH INTENT TO SELL.
//
//Send an email to our support line for any questions or inquirys
//CONTACT: Lazyfridaystudio@gmail.com
//
//Alternatively join our Discord
//DISCORD: https://discord.gg/Z3tpMG
//
//Hope you enjoy the lazy mesh Combine
//designed and made by lazyFridayStudio
//================================================================
[CustomEditor(typeof(MeshCombine))]
[CanEditMultipleObjects]
public class MeshCombineEditor : Editor
{
    private MeshCombine targetMeshCombine;

    #region Constances
    private readonly Color32 headerColorText = new Color32(218, 124, 40, 255);
    private readonly Color32 headerColor = new Color32(26, 26, 26, 255);
    private readonly Color32 headerSeparatorColor = new Color32(242, 242, 242, 255);
    private readonly Color32 itemAreaColor = new Color32(22, 22, 22, 255);
    
    #endregion
    
    #region Textures
    
    private Texture2D headerBackground;
    private Texture2D headerSeparatorBackground;
    private Texture2D itemAreaBackground;
    
    #endregion
    
    #region Styles
    public GUIStyle headerStyleText = new GUIStyle();
    public GUIStyle subMenuStyleText = new GUIStyle();
    #endregion

    #region Propertys

    public SerializedProperty MeshFilters;
    public SerializedProperty combinedInstance;
    public SerializedProperty combinedInstanceMaterial;
    
    #endregion

    private void InitStyleAndTextures()
    {
        // Setup header font
        string path = "Assets/Editor/LazyHelpers/Resources/HeaderFont.ttf";
        Font headerFont = EditorGUIUtility.Load(path) as Font;
        headerStyleText.normal.textColor = headerColorText;
        headerStyleText.fontSize = 16;
        headerStyleText.alignment = TextAnchor.LowerCenter;
        headerStyleText.font = headerFont;
        
        // Setup header font
        subMenuStyleText.normal.textColor = headerColorText;
        subMenuStyleText.fontSize = 14;
        subMenuStyleText.alignment = TextAnchor.MiddleCenter;
    }

    private void InitPropertys()
    {
        MeshFilters = serializedObject.FindProperty ("MeshFilters");
        combinedInstance = serializedObject.FindProperty ("CombinedInstance");
        combinedInstanceMaterial = serializedObject.FindProperty ("CombinedInstanceMaterial");
    }
    
    #region Inspector Drawing Methods
    
    private void OnEnable()
    {
        InitStyleAndTextures();
        InitPropertys();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update ();
        targetMeshCombine = (MeshCombine)target;

        DrawDesign();
        
        serializedObject.ApplyModifiedProperties ();
    }
    
    private void DrawDesign()
    {
        // Draw Header
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space(4);
        GUILayout.Label("LAZY MESH COMBINE", headerStyleText);
        EditorGUILayout.EndVertical();

        // Draw Stats SubHeading
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Stats", subMenuStyleText);
        EditorGUILayout.EndVertical();
        
        // Draw Stats
        EditorGUILayout.BeginVertical();
        EditorGUILayout.HelpBox("Mesh Count: ", MessageType.None, true);
        EditorGUILayout.HelpBox("Vertices Count: ", MessageType.None, true);
        EditorGUILayout.EndVertical();
        
        
        // Draw Separator
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Propertys", subMenuStyleText);
        EditorGUILayout.EndVertical();
        
        // Draw Value Area
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space(4);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Mesh Filters", GUILayout.MaxWidth(150), GUILayout.MinWidth(150));
        EditorGUILayout.PropertyField(MeshFilters);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Combined Instance", GUILayout.MaxWidth(150), GUILayout.MinWidth(150));
        EditorGUILayout.PropertyField(combinedInstance);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Combined Instance Material", GUILayout.MaxWidth(150), GUILayout.MinWidth(150));
        EditorGUILayout.PropertyField(combinedInstanceMaterial);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Unwrap Secondary UVs", GUILayout.MaxWidth(150), GUILayout.MinWidth(150));
       // targetMeshCombine.unwrappedState = (MeshCombine.eState)EditorGUILayout.EnumPopup(targetMeshCombine.unwrappedState);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        
        string HelpBoxText = String.Empty;
        MessageType messageType = MessageType.None;
        if (CountVerticesInMeshes(targetMeshCombine.MeshFilters.ToArray()) > 65535)
        {
            HelpBoxText = "The Combined Mesh will be greater then 65535, This will cause errors in the combining";
            messageType = MessageType.Error;
        }
        
        EditorGUILayout.HelpBox(HelpBoxText, messageType, true);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(200);
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(100);
    }
    #endregion
    
    private static int CountVerticesInMeshes(MeshFilter[] meshFilters)
    {
        //Return count of vertices
        int verticesCount = 0;

        //Count all
        foreach (MeshFilter mf in meshFilters)
        {
            if (mf != null)
            {
                verticesCount += mf.sharedMesh.vertexCount; 
            }
        }

        return verticesCount;
    }
}
