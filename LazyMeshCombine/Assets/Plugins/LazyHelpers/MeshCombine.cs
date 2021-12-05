using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombine : MonoBehaviour
{
    public enum eState
    {
        False,
        True
    }

    // Generation Values
    public eState TurnOffMeshFilters = eState.True;
    public eState DeleteObjectsOnCombine = eState.False;
    public eState GenerateOnStart = eState.False;
    
    public GameObject CombinedInstance;
    public Material CombinedInstanceMaterial;
    public List<MeshFilter> MeshFilters;
    
    private void Start()
    {
        if (GenerateOnStart == eState.True)
        {
            CombineMeshes();
        }
    }

    public void CombineMeshes()
    {
        // Combine lists
        CombineInstance[] combine = new CombineInstance[MeshFilters.Count];

        // Cleans if there is no mesh
        MeshFilters.RemoveAll(MeshFilter => MeshFilter == null);

        // adds the meshes to the combining list
        for (int i = 0; i < MeshFilters.Count; i++)
        {
            combine[i].mesh = MeshFilters[i].sharedMesh;
            combine[i].transform = MeshFilters[i].transform.localToWorldMatrix;
            MeshFilters[i].gameObject.SetActive(false);
        }

        // Destroys the MeshFilter mesh if it already exists, Cleans memory up
        if (CombinedInstance.GetComponent<MeshFilter>() != null)
        {
            DestroyImmediate(CombinedInstance.GetComponent<MeshFilter>().mesh);
        }

        // Adds the components
        MeshFilter CIMF = CombinedInstance.AddComponent<MeshFilter>();
        CIMF.mesh = new Mesh();
        CIMF.mesh.CombineMeshes(combine);

        MeshRenderer CIMR = CombinedInstance.AddComponent<MeshRenderer>();
        CIMR.sharedMaterial = CombinedInstanceMaterial;

        if (DeleteObjectsOnCombine == eState.True)
        {
            foreach (var MF in MeshFilters)
            {
                Destroy(MF.gameObject);
            }
        }
        else
        {
            if (TurnOffMeshFilters == eState.True)
            {
                foreach (var MF in MeshFilters)
                {
                    MF.gameObject.SetActive(false);
                }
            }
        }
    }
}