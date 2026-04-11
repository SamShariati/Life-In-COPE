using UnityEngine;
using UnityEditor;

public class MeshCombiner : MonoBehaviour
{
    [ContextMenu("Combine Meshes")]
    public void CombineMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = transform.worldToLocalMatrix * meshFilters[i].transform.localToWorldMatrix;
        }

        foreach (Transform child in transform)
            child.gameObject.SetActive(false);

        MeshFilter mf = gameObject.GetComponent<MeshFilter>();
        if (mf == null) mf = gameObject.AddComponent<MeshFilter>();
        if (gameObject.GetComponent<MeshRenderer>() == null)
            gameObject.AddComponent<MeshRenderer>();

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine);
        mf.sharedMesh = combinedMesh;

        // Save mesh to disk ← this is the new part
        if (!System.IO.Directory.Exists("Assets/CombinedMeshes"))
            AssetDatabase.CreateFolder("Assets", "CombinedMeshes");

        AssetDatabase.CreateAsset(combinedMesh, "Assets/CombinedMeshes/EU-Pall.asset");
        AssetDatabase.SaveAssets();

        gameObject.SetActive(true);
    }
}