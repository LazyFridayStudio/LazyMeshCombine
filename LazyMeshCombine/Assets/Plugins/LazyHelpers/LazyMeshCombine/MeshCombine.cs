using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class MeshCombine : MonoBehaviour
{
    #region Enums

    public enum ePopulatingMethod
    {
        Manual,
        PopulateFromChildren,
        PopulateFromObjectChildren,
    }
    
    public enum ePopulatingTime
    {
        None,
        OnCombine,
        OnAwake,
        OnStart,
    }
    
    public enum eGenerationMethod
    {
        None,
        GenerateByButton,
        GenerateOnStart,
        GenerateOnAwake,
        GenerateAfterTimeStart,
        GenerateAfterTimeAwake
    }

    public enum eCleanupMethod
    {
        None,
        TurnOffMeshRenderer,
        TurnOffGameObject,
        DeleteMeshFilterAndRenderer,
        DeleteObjects,
    }

    public enum eCombineSetting
    {
        CombineToNewObject,
        CombineToObject,
        CombineToFirstMesh
    }

    public enum eCombineMaterialSetting
    {
        SharedMaterial,
        FirstMeshMaterial
    }

    #endregion

    // Generation Values
    [SerializeField] private eGenerationMethod generationMethod;
    [SerializeField] private float generationTime;

    // Cleanup Values
    [SerializeField] private eCleanupMethod cleanupMethod;

    // Combine
    [SerializeField] private eCombineSetting combineSetting;
    [SerializeField] private GameObject combinedInstance;
    [SerializeField] private eCombineMaterialSetting combineMaterialSetting;
    [SerializeField] private Material combinedInstanceMaterial;

    // Uv Unwrapping
    [SerializeField] private bool generateSecondaryUvs;

    // Mesh combining settings
    [SerializeField] private bool mergeSubMeshes;
    [SerializeField] private bool useMatrices;
    [SerializeField] private bool hasLightMapData;
    
    // Populating Method
    [SerializeField] private List<MeshFilter> meshFilters;
    [SerializeField] private ePopulatingMethod populatingMethod;
    [SerializeField] private ePopulatingTime populatingTime;
    [SerializeField] private GameObject populatingObject;
    
    public List<MeshFilter> MeshFilters
    {
        get => meshFilters;
        set => meshFilters = value;
    }

    private void Awake()
    {
        if (populatingTime == ePopulatingTime.OnAwake)
        {
            populateMeshes();
        }
        
        if (generationMethod == eGenerationMethod.GenerateOnAwake)
        {
            CombineMeshes();
        }
        else if (generationMethod == eGenerationMethod.GenerateAfterTimeAwake)
        {
            GenerateAfterTime();
        }
    }

    private void Start()
    {
        if (populatingTime == ePopulatingTime.OnStart)
        {
            populateMeshes();
        }
        
        if (generationMethod == eGenerationMethod.GenerateOnStart)
        {
            CombineMeshes();
        }
        else if (generationMethod == eGenerationMethod.GenerateAfterTimeStart)
        {
            GenerateAfterTime();
        }
    }

    private void populateMeshes()
    {
        switch (populatingMethod)
        {
            case ePopulatingMethod.PopulateFromChildren:
                meshFilters = GetComponentsInChildren<MeshFilter>().ToList();
                break;
            case ePopulatingMethod.PopulateFromObjectChildren:
                meshFilters = populatingObject.GetComponentsInChildren<MeshFilter>().ToList();
                break;
        }
    }

    private async Task GenerateAfterTime()
    {
        await Task.Delay(TimeSpan.FromSeconds(generationTime));
        CombineMeshes();
    }

    public void CombineMeshes(List<MeshFilter> _meshFilters, eCleanupMethod _cleanupMethod)
    {
        cleanupMethod = _cleanupMethod;
        MeshFilters = _meshFilters;

        generationMethod = eGenerationMethod.None;
        combineSetting = eCombineSetting.CombineToNewObject;
        combineMaterialSetting = eCombineMaterialSetting.FirstMeshMaterial;
        generateSecondaryUvs = false;
        mergeSubMeshes = true;
        useMatrices = true;
        hasLightMapData = false;
        populatingMethod = ePopulatingMethod.Manual;
        populatingTime = ePopulatingTime.None;
        
        CombineMeshes();
    }
    
    public void CombineMeshes()
    {
        if (populatingTime == ePopulatingTime.OnCombine)
        {
            populateMeshes();
        }
        
        // Combine lists
        CombineInstance[] combine = new CombineInstance[meshFilters.Count];

        // Cleans if there is no mesh
        meshFilters.RemoveAll(MeshFilter => MeshFilter == null);

        // adds the meshes to the combining list
        for (int i = 0; i < meshFilters.Count; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        // Combine Settings
        GameObject meshObject = null;
        switch (combineSetting)
        {
            case eCombineSetting.CombineToNewObject:
                meshObject = new GameObject("New Mesh Combine Instance");
                break;
            case eCombineSetting.CombineToObject:
                meshObject = combinedInstance;
                break;
            case eCombineSetting.CombineToFirstMesh:
                meshObject = meshFilters[0].gameObject;
                break;
        }

        MeshFilter CIMF = null;
        MeshRenderer CIMR = null;

        // Add in Mesh Filter
        CIMF = meshObject.GetComponent<MeshFilter>() == null ? meshObject.AddComponent<MeshFilter>() : meshObject.GetComponent<MeshFilter>();

        // Add in Mesh Renderer
        CIMR = meshObject.GetComponent<MeshRenderer>() == null ? meshObject.AddComponent<MeshRenderer>() : meshObject.GetComponent<MeshRenderer>();

        // Combine Meshes
        CIMF.mesh = new Mesh();
        CIMF.sharedMesh.CombineMeshes(combine, mergeSubMeshes, useMatrices, hasLightMapData);

        switch (combineMaterialSetting)
        {
            // Setup Materials
            case eCombineMaterialSetting.SharedMaterial:
                CIMR.sharedMaterial = combinedInstanceMaterial;
                break;
            case eCombineMaterialSetting.FirstMeshMaterial:
                CIMR.sharedMaterial = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;
                break;
        }

#if UNITY_EDITOR
        // Should it generate Secondary Uv's
        if (generateSecondaryUvs)
        {
            Unwrapping.GenerateSecondaryUVSet(CIMF.sharedMesh);
        }
#endif

        // Clean up Method
        switch (cleanupMethod)
        {
            case eCleanupMethod.DeleteObjects:
            {
                foreach (var MF in meshFilters)
                {
                    DestroyImmediate(MF.gameObject);
                }

                break;
            }
            case eCleanupMethod.DeleteMeshFilterAndRenderer:
            {
                foreach (var MF in meshFilters)
                {
                    if (MF.GetComponent<MeshRenderer>() != null)
                    {
                        DestroyImmediate(MF.GetComponent<MeshRenderer>());
                    }

                    DestroyImmediate(MF);
                }

                break;
            }
            case eCleanupMethod.TurnOffMeshRenderer:
            {
                foreach (var MF in meshFilters)
                {
                    if (MF.GetComponent<MeshRenderer>() != null)
                    {
                        MF.GetComponent<MeshRenderer>().enabled = false;
                    }
                }

                break;
            }
            case eCleanupMethod.TurnOffGameObject:
            {
                foreach (var MF in meshFilters)
                {
                    MF.gameObject.SetActive(false);
                }

                break;
            }
        }
    }
}