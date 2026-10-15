using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MeshCombiner : MonoBehaviour
{
    [ContextMenu("Combine Children Meshes")]
    public void CombineChildren()
    {
        // Group by material
        var materialGroups = new Dictionary<Material, List<CombineInstance>>();

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        var usedFilters = new List<MeshFilter>();

        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.gameObject == gameObject) continue; // skip parent
            if (mf.sharedMesh == null) continue;

            MeshRenderer mr = mf.GetComponent<MeshRenderer>();
            if (mr == null) continue;

            Material[] mats = mr.sharedMaterials;
            Matrix4x4 matrix = transform.worldToLocalMatrix * mf.transform.localToWorldMatrix;
            bool used = false;

            for (int s = 0; s < mf.sharedMesh.subMeshCount && s < mats.Length; s++)
            {
                Material mat = mats[s];
                if (mat == null) continue;

                if (!materialGroups.ContainsKey(mat))
                    materialGroups[mat] = new List<CombineInstance>();

                materialGroups[mat].Add(new CombineInstance
                {
                    mesh = mf.sharedMesh,
                    subMeshIndex = s,   // only this submesh, not the whole mesh
                    transform = matrix
                });
                used = true;
            }

            if (used) usedFilters.Add(mf);
        }

        if (materialGroups.Count == 0)
        {
            Debug.LogWarning("MeshCombiner: nothing to combine.");
            return;
        }

        // One mesh per material
        Mesh[] subMeshes = new Mesh[materialGroups.Count];
        Material[] materials = new Material[materialGroups.Count];
        int i = 0;

        foreach (var kvp in materialGroups)
        {
            Mesh m = new Mesh();
            m.indexFormat = IndexFormat.UInt32;           // <-- allow > 65535 vertices
            m.CombineMeshes(kvp.Value.ToArray(), true, true);
            subMeshes[i] = m;
            materials[i] = kvp.Key;
            i++;
        }

        // Merge the per-material meshes into one mesh with multiple submeshes
        CombineInstance[] finalCombine = new CombineInstance[subMeshes.Length];
        for (int j = 0; j < subMeshes.Length; j++)
        {
            finalCombine[j] = new CombineInstance
            {
                mesh = subMeshes[j],
                transform = Matrix4x4.identity
            };
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.name = gameObject.name + "_Combined";
        combinedMesh.indexFormat = IndexFormat.UInt32;    // <-- here too
        combinedMesh.CombineMeshes(finalCombine, false, false);
        combinedMesh.RecalculateBounds();

        // Temporary per-material meshes are no longer needed
        foreach (Mesh m in subMeshes)
        {
            if (Application.isPlaying) Destroy(m);
            else DestroyImmediate(m);
        }

        // Add MeshFilter / MeshRenderer to the parent if missing
        MeshFilter parentMF = GetComponent<MeshFilter>();
        if (parentMF == null) parentMF = gameObject.AddComponent<MeshFilter>();

        MeshRenderer parentMR = GetComponent<MeshRenderer>();
        if (parentMR == null) parentMR = gameObject.AddComponent<MeshRenderer>();

        parentMF.sharedMesh = combinedMesh;
        parentMR.sharedMaterials = materials;

        // Disable the original children
        foreach (MeshFilter mf in usedFilters)
            mf.gameObject.SetActive(false);

#if UNITY_EDITOR
        const string folder = "Assets/CombinedMeshes";
        if (!AssetDatabase.IsValidFolder(folder))
            AssetDatabase.CreateFolder("Assets", "CombinedMeshes");

        string path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{gameObject.name}.asset");
        AssetDatabase.CreateAsset(combinedMesh, path);
        AssetDatabase.SaveAssets();
#endif

        Debug.Log($"Combined {usedFilters.Count} meshes into 1 mesh with {materials.Length} materials " +
                  $"({combinedMesh.vertexCount} vertices).");
    }
}