using UnityEngine;
using System.Collections.Generic;

public class TreeBaker : MonoBehaviour
{
    public Terrain terrain;
    public float chunkSize = 50f;

    void Start()
    {
        BakeTrees();
    }

    void BakeTrees()
    {
        if (terrain == null) terrain = Terrain.activeTerrain;
        if (terrain == null) return;

        TerrainData data = terrain.terrainData;
        TreeInstance[] trees = data.treeInstances;
        TreePrototype[] prototypes = data.treePrototypes;

        Dictionary<int, List<TreeInstance>> treesByProto = new Dictionary<int, List<TreeInstance>>();

        // Roztriedime stromy pod¾a typu
        for (int i = 0; i < prototypes.Length; i++)
            treesByProto[i] = new List<TreeInstance>();

        foreach (var t in trees)
            treesByProto[t.prototypeIndex].Add(t);

        // Pre každý typ stromu
        foreach (var kvp in treesByProto)
        {
            int protoIndex = kvp.Key;
            GameObject prefab = prototypes[protoIndex].prefab;

            MeshFilter prefabMesh = prefab.GetComponent<MeshFilter>();
            if (!prefabMesh)
            {
                Debug.LogWarning("Tree prefab has no MeshFilter: " + prefab.name);
                continue;
            }

            Mesh sourceMesh = prefabMesh.sharedMesh;
            Material mat = prefab.GetComponent<MeshRenderer>().sharedMaterial;

            // Chunkovanie
            Dictionary<Vector2Int, List<Matrix4x4>> chunks = new Dictionary<Vector2Int, List<Matrix4x4>>();

            foreach (var tree in kvp.Value)
            {
                Vector3 worldPos = Vector3.Scale(tree.position, data.size) + terrain.transform.position;

                Vector2Int chunkCoord = new Vector2Int(
                    Mathf.FloorToInt(worldPos.x / chunkSize),
                    Mathf.FloorToInt(worldPos.z / chunkSize)
                );

                if (!chunks.ContainsKey(chunkCoord))
                    chunks[chunkCoord] = new List<Matrix4x4>();

                Matrix4x4 m = Matrix4x4.TRS(
                    worldPos,
                    Quaternion.Euler(0, tree.rotation * Mathf.Rad2Deg, 0),
                    Vector3.one * tree.widthScale
                );

                chunks[chunkCoord].Add(m);
            }

            // Pre každý chunk vytvoríme combined mesh
            foreach (var chunk in chunks)
            {
                CombineInstance[] combine = new CombineInstance[chunk.Value.Count];

                for (int i = 0; i < combine.Length; i++)
                {
                    combine[i].mesh = sourceMesh;
                    combine[i].transform = chunk.Value[i];
                }

                Mesh combined = new Mesh();
                combined.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                combined.CombineMeshes(combine, true, true);

                GameObject baked = new GameObject("BakedTrees_" + prefab.name + "_" + chunk.Key);
                baked.transform.position = Vector3.zero;
                var mf = baked.AddComponent<MeshFilter>();
                var mr = baked.AddComponent<MeshRenderer>();

                mf.sharedMesh = combined;
                mr.sharedMaterial = mat;
            }
        }

        Debug.Log("Tree baking complete.");
    }
}
