#region NameSpaces

using UnityEngine;
using UnityEditor;

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

    #region Constants
    
    private readonly Color32 headerColorText = new Color32(218, 124, 40, 255);
    private readonly Color32 headerColor = new Color32(26, 26, 26, 255);
    private readonly Color32 headerSeparatorColor = new Color32(242, 242, 242, 255);
    private readonly Color32 itemAreaColor = new Color32(22, 22, 22, 255);
    
    #endregion

    #region Styles
    
    public GUIStyle headerStyleText = new GUIStyle();
    public GUIStyle subMenuStyleText = new GUIStyle();
    
    #endregion

    #region Propertys
    
    // Populating Method
    private SerializedProperty populatingMethod;
    private SerializedProperty populatingTime;
    private SerializedProperty populatingObject;
    private SerializedProperty MeshFilters;
    
    // Generation
    private SerializedProperty generationMethod;
    private SerializedProperty generationTime;

    // Cleanup
    private SerializedProperty cleanupMethod;
    
    // Combine Settings
    private SerializedProperty combineSetting;
    private SerializedProperty combinedInstance;
    private SerializedProperty combineMaterialSetting;
    private SerializedProperty combinedInstanceMaterial;
    
    // UV Generation
    private SerializedProperty generateSecondaryUvs;
    
    // combine Settings
    private SerializedProperty mergeSubMeshes;
    private SerializedProperty useMatrices;
    private SerializedProperty hasLightMapData;
    

    
    
    #endregion

    #region Setups

    private void InitStyles()
    {
        // Setup header font
        headerStyleText.normal.textColor = headerColorText;
        headerStyleText.fontSize = 16;
        headerStyleText.alignment = TextAnchor.LowerCenter;
        headerStyleText.fontStyle = FontStyle.Bold;

        // Setup header font
        subMenuStyleText.normal.textColor = headerColorText;
        subMenuStyleText.fontSize = 12;
        subMenuStyleText.fontStyle = FontStyle.Bold;
        subMenuStyleText.alignment = TextAnchor.MiddleCenter;
    }

    /// <summary>
    /// Setup the properties for the inspector
    /// </summary>
    private void InitProperties()
    {
        populatingMethod = serializedObject.FindProperty("populatingMethod");
        populatingTime = serializedObject.FindProperty("populatingTime");
        populatingObject = serializedObject.FindProperty("populatingObject");
        MeshFilters = serializedObject.FindProperty("meshFilters");

        // Combine Settings
        combinedInstance = serializedObject.FindProperty("combinedInstance");
        combinedInstanceMaterial = serializedObject.FindProperty("combinedInstanceMaterial");
        combineSetting = serializedObject.FindProperty("combineSetting");
        combineMaterialSetting = serializedObject.FindProperty("combineMaterialSetting");

        // Generation
        generationMethod = serializedObject.FindProperty("generationMethod");
        generationTime = serializedObject.FindProperty("generationTime");

        // Cleanup
        cleanupMethod = serializedObject.FindProperty("cleanupMethod");

        // UV Generation
        generateSecondaryUvs = serializedObject.FindProperty("generateSecondaryUvs");
        
    // combine Settings
    mergeSubMeshes = serializedObject.FindProperty("mergeSubMeshes");
    useMatrices = serializedObject.FindProperty("useMatrices");
    hasLightMapData = serializedObject.FindProperty("hasLightMapData");
    }
    
    #endregion
    
    private void OnEnable()
    {
        InitStyles();
        InitProperties();
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
        
        // Draw Separator
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Core", subMenuStyleText);
        EditorGUILayout.EndVertical();
        
        // Draw Warnings
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        string HelpBoxText = "Thank you for using Lazy Mesh Combine";
        MessageType messageType = MessageType.None;
        if (CountVerticesInMeshes(targetMeshCombine.MeshFilters.ToArray()) > 65535)
        {
            HelpBoxText = "The Combined Mesh will be greater then 65535, This will cause errors in the combining";
            messageType = MessageType.Error;
        }
        EditorGUILayout.HelpBox(HelpBoxText, messageType, true);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.PropertyField(populatingMethod);
        if (populatingMethod.enumValueIndex == (int) MeshCombine.ePopulatingMethod.PopulateFromObjectChildren)
        {
            EditorGUILayout.PropertyField(populatingObject);
        }
        EditorGUILayout.PropertyField(populatingTime);
        EditorGUILayout.EndVertical();

        if (populatingMethod.enumValueIndex == (int) MeshCombine.ePopulatingMethod.Manual)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(MeshFilters,new GUIContent("Meshes"));
            EditorGUILayout.EndHorizontal();
        }

        
        // Draw Stats SubHeading
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Stats", subMenuStyleText);
        EditorGUILayout.EndVertical();
        
        // Draw Stats
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("Mesh Count: " + CountMeshes(targetMeshCombine.MeshFilters.ToArray()));
        EditorGUILayout.LabelField("Vertices Count: " + CountVerticesInMeshes(targetMeshCombine.MeshFilters.ToArray()));
        EditorGUILayout.EndVertical();
        
        // Draw Settings
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Settings", subMenuStyleText);
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical(GUI.skin.box);
        
        // Which type of generation
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(generationMethod);
        EditorGUILayout.EndHorizontal();
        
        if (generationMethod.enumValueIndex != (int) MeshCombine.eGenerationMethod.None)
        {
            if (generationMethod.enumValueIndex == (int) MeshCombine.eGenerationMethod.GenerateAfterTimeAwake ||
                generationMethod.enumValueIndex == (int) MeshCombine.eGenerationMethod.GenerateAfterTimeStart)
            {
                EditorGUILayout.PropertyField(generationTime);
            }

            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(cleanupMethod);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.PropertyField(combineSetting);
            if (combineSetting.enumValueIndex == (int) MeshCombine.eCombineSetting.CombineToObject)
            {
                EditorGUILayout.PropertyField(combinedInstance);
            }

            EditorGUILayout.PropertyField(combineMaterialSetting);
            if (combineMaterialSetting.enumValueIndex == (int) MeshCombine.eCombineMaterialSetting.SharedMaterial)
            {
                EditorGUILayout.PropertyField(combinedInstanceMaterial);
            }
            EditorGUILayout.EndVertical();
            
            // Combining Settings
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.PropertyField(mergeSubMeshes);
            EditorGUILayout.PropertyField(useMatrices);
            EditorGUILayout.PropertyField(hasLightMapData);
            
            if (generationMethod.enumValueIndex == (int)MeshCombine.eGenerationMethod.GenerateByButton)
            {
                // Uv Unwrapping
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.HelpBox("This will only work if generation is done in editor", MessageType.Warning, true);
                EditorGUILayout.PropertyField(generateSecondaryUvs);
            }
            else
            {
                generateSecondaryUvs.boolValue = false;
            }

        }
        EditorGUILayout.EndVertical();
        
        if (generationMethod.enumValueIndex == (int) MeshCombine.eGenerationMethod.GenerateByButton)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            if (GUILayout.Button("Generate Combined mesh"))
            {
                targetMeshCombine.CombineMeshes();
            }
        }
    }
    
    #region Helper Methods

    private int CountMeshes(MeshFilter[] meshFilters)
    {
        return meshFilters.Length;
    }
    
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
   
    #endregion
}
