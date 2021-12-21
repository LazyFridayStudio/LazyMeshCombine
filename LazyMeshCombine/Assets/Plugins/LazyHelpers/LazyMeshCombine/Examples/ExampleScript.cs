using System.Linq;
using UnityEngine;

namespace LazyHelper.LazyMeshCombiner
{
    public class ExampleScript : MonoBehaviour
    {
        public MeshCombine myCombiner;
        public GameObject myCombinedObject;
        
        private void PopulateMeshFilters()
        {
            myCombiner.MeshFilters = GetComponentsInChildren<MeshFilter>().ToList();
        }
        
        private void GetCombinedObject()
        {
            myCombinedObject = myCombiner.CombineMeshes();
        }
    }
}