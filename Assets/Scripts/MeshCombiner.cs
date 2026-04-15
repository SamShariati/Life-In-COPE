using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MeshCombiner : MonoBehaviour
{
    [ContextMenu("Combine Children Meshes")]
    public void CombineChildren()
    {
        // Group MeshFilters by material
        Dictionary<Material, List<CombineInstance>> materialGroups = new Dictionary<Material, List<CombineInstance>>();

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.gameObject == gameObject) continue; // skip parent

            MeshRenderer mr = mf.GetComponent<MeshRenderer>();
            if (mr == null) continue;

            foreach (Material mat in mr.sharedMaterials)
            {
                if (!materialGroups.ContainsKey(mat))
                    materialGroups[mat] = new List<CombineInstance>();

                CombineInstance ci = new CombineInstance
                {
                    mesh = mf.sharedMesh,
                    transform = transform.worldToLocalMatrix * mf.transform.localToWorldMatrix
                };
                materialGroups[mat].Add(ci);
            }
        }

        // Build one sub-mesh per material
        Mesh[] subMeshes = new Mesh[materialGroups.Count];
        Material[] materials = new Material[materialGroups.Count];
        int i = 0;

        foreach (var kvp in materialGroups)
        {
            Mesh m = new Mesh();
            m.CombineMeshes(kvp.Value.ToArray(), true);
            subMeshes[i] = m;
            materials[i] = kvp.Key;
            i++;
        }

        // Combine sub-meshes into one mesh (false = keep as sub-meshes)
        CombineInstance[] finalCombine = new CombineInstance[subMeshes.Length];
        for (int j = 0; j < subMeshes.Length; j++)
        {
            finalCombine[j] = new CombineInstance { mesh = subMeshes[j], transform = Matrix4x4.identity };
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(finalCombine, false);
        combinedMesh.RecalculateNormals();
        combinedMesh.RecalculateBounds();

        // Explicitly add MeshFilter and MeshRenderer if missing
        MeshFilter parentMF = GetComponent<MeshFilter>();
        if (parentMF == null) parentMF = gameObject.AddComponent<MeshFilter>();

        MeshRenderer parentMR = GetComponent<MeshRenderer>();
        if (parentMR == null) parentMR = gameObject.AddComponent<MeshRenderer>();

        parentMF.sharedMesh = combinedMesh;
        parentMR.sharedMaterials = materials;

        // Disable children
        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.gameObject != gameObject)
                mf.gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        AssetDatabase.CreateAsset(combinedMesh, "Assets/CombinedMeshes/" + gameObject.name + ".asset");
        AssetDatabase.SaveAssets();
#endif

        Debug.Log($"Combined into {subMeshes.Length} sub-meshes with {subMeshes.Length} materials.");
    }
}