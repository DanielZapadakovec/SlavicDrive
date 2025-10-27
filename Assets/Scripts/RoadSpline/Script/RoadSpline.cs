using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class RoadSpline : MonoBehaviour
{
    [Header("Spline Points")]
    public List<Vector3> points = new List<Vector3>(); // local-space points

    [Header("Road Settings")]
    public float roadWidth = 4f;
    public float edgeWidth = 0.5f;
    public float uvRepeatPerMeter = 1f;

    [Header("Curve Quality")]
    [Tooltip("Number of samples between each pair of control points")]
    public int curveResolution = 8; // samples per segment
    [Tooltip("0 = Catmull-Rom (smooth), 1 = straighter (higher tension)")]
    [Range(0f, 1f)]
    public float curveTension = 0f; // 0 = Catmull-Rom, up to 1 = straighter

    [Header("Textures & Materials")]
    public Material roadMaterial;
    public Material edgeMaterial;

    [Header("Terrain Integration")]
    public Terrain targetTerrain; // optional, if null will use Terrain.activeTerrain
    [Range(0f, 1f)]
    public float terrainBlend = 0.5f; // how strongly to raise/lower terrain to road height
    public float terrainPadding = 1f; // how far from road center to affect terrain (meters)

    [Header("Generation")]
    public bool autoUpdate = true;

    [HideInInspector]
    public Mesh generatedMesh;

    void OnValidate()
    {
        if (autoUpdate)
            GenerateRoad();
    }

    public void AddPointAtWorldPos(Vector3 worldPos)
    {
        Vector3 local = transform.InverseTransformPoint(worldPos);
        points.Add(local);
        if (autoUpdate) GenerateRoad();
    }

    public void InsertPoint(int index, Vector3 worldPos)
    {
        Vector3 local = transform.InverseTransformPoint(worldPos);
        points.Insert(index, local);
        if (autoUpdate) GenerateRoad();
    }

    public void RemovePoint(int index)
    {
        if (index >= 0 && index < points.Count)
        {
            points.RemoveAt(index);
            if (autoUpdate) GenerateRoad();
        }
    }

    // Public API to regenerate road mesh and optionally apply terrain changes
    public void GenerateRoad(bool applyToTerrain = false)
    {
        if (points == null || points.Count < 2)
        {
            ClearMesh();
            return;
        }

        // Sample curve into list of world-space positions
        List<Vector3> samples = SampleCurveWorldPositions();

        if (samples == null || samples.Count < 2)
        {
            ClearMesh();
            return;
        }

        // Build road verts/tris/uv
        var verts = new List<Vector3>();
        var uvs = new List<Vector2>();
        var tris = new List<int>();

        // Edge geometry
        var edgeVerts = new List<Vector3>();
        var edgeUVs = new List<Vector2>();
        var edgeTris = new List<int>();

        float accumulatedLength = 0f;

        // Precompute distances for UV
        for (int i = 0; i < samples.Count; i++)
        {
            Vector3 p = samples[i];
            if (i > 0)
            {
                accumulatedLength += Vector3.Distance(samples[i - 1], p);
            }

            // forward direction: use tangent between neighbours or next - prev
            Vector3 forward;
            if (i == 0) forward = (samples[i + 1] - p).normalized;
            else if (i == samples.Count - 1) forward = (p - samples[i - 1]).normalized;
            else forward = (samples[i + 1] - samples[i - 1]).normalized;

            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

            float half = roadWidth * 0.5f;
            Vector3 leftInner = p - right * half;
            Vector3 rightInner = p + right * half;

            verts.Add(transform.InverseTransformPoint(leftInner));
            verts.Add(transform.InverseTransformPoint(rightInner));

            uvs.Add(new Vector2(0, accumulatedLength * uvRepeatPerMeter));
            uvs.Add(new Vector2(1, accumulatedLength * uvRepeatPerMeter));

            // edges: leftOuter, leftInner, rightInner, rightOuter (for each sample)
            float outer = half + edgeWidth;
            Vector3 leftOuter = p - right * outer;
            Vector3 rightOuter = p + right * outer;

            edgeVerts.Add(transform.InverseTransformPoint(leftOuter));
            edgeVerts.Add(transform.InverseTransformPoint(leftInner));
            edgeVerts.Add(transform.InverseTransformPoint(rightInner));
            edgeVerts.Add(transform.InverseTransformPoint(rightOuter));

            // for edges we repeat UVs the same way (can be customized)
            edgeUVs.Add(new Vector2(0, accumulatedLength * uvRepeatPerMeter));
            edgeUVs.Add(new Vector2(1, accumulatedLength * uvRepeatPerMeter));
            edgeUVs.Add(new Vector2(0, accumulatedLength * uvRepeatPerMeter));
            edgeUVs.Add(new Vector2(1, accumulatedLength * uvRepeatPerMeter));
        }

        // create tris between samples (two triangles per segment)
        for (int i = 0; i < samples.Count - 1; i++)
        {
            int baseIdx = i * 2;
            tris.Add(baseIdx);
            tris.Add(baseIdx + 2);
            tris.Add(baseIdx + 1);

            tris.Add(baseIdx + 1);
            tris.Add(baseIdx + 2);
            tris.Add(baseIdx + 3);

            int eBase = i * 4;
            // left edge quad (two tris)
            edgeTris.Add(eBase);
            edgeTris.Add(eBase + 4);
            edgeTris.Add(eBase + 1);

            edgeTris.Add(eBase + 1);
            edgeTris.Add(eBase + 4);
            edgeTris.Add(eBase + 5);

            // right edge quad (two tris)
            edgeTris.Add(eBase + 2);
            edgeTris.Add(eBase + 6);
            edgeTris.Add(eBase + 3);

            edgeTris.Add(eBase + 3);
            edgeTris.Add(eBase + 6);
            edgeTris.Add(eBase + 7);
        }

        Mesh roadMesh = new Mesh();
        roadMesh.name = gameObject.name + "_RoadMesh";
        roadMesh.SetVertices(verts);
        roadMesh.SetUVs(0, uvs);
        roadMesh.SetTriangles(tris, 0);
        roadMesh.RecalculateNormals();
        roadMesh.RecalculateBounds();

        Mesh edgeMesh = new Mesh();
        edgeMesh.name = gameObject.name + "_EdgeMesh";
        edgeMesh.SetVertices(edgeVerts);
        edgeMesh.SetUVs(0, edgeUVs);
        edgeMesh.SetTriangles(edgeTris, 0);
        edgeMesh.RecalculateNormals();
        edgeMesh.RecalculateBounds();

        // Assign mesh to child objects for separation (one for road, one for edges)
        Transform roadChild = transform.Find("_RoadMesh");
        if (roadChild == null)
        {
            GameObject rc = new GameObject("_RoadMesh");
            rc.transform.SetParent(transform, false);
            rc.transform.SetSiblingIndex(0);
            rc.AddComponent<MeshFilter>();
            rc.AddComponent<MeshRenderer>();
            roadChild = rc.transform;
        }
        Transform edgeChild = transform.Find("_EdgeMesh");
        if (edgeChild == null)
        {
            GameObject ec = new GameObject("_EdgeMesh");
            ec.transform.SetParent(transform, false);
            ec.AddComponent<MeshFilter>();
            ec.AddComponent<MeshRenderer>();
            edgeChild = ec.transform;
        }

        var roadMF = roadChild.GetComponent<MeshFilter>();
        var roadMR = roadChild.GetComponent<MeshRenderer>();
        var edgeMF = edgeChild.GetComponent<MeshFilter>();
        var edgeMR = edgeChild.GetComponent<MeshRenderer>();

        roadMF.sharedMesh = roadMesh;
        edgeMF.sharedMesh = edgeMesh;

        if (roadMaterial != null) roadMR.sharedMaterial = roadMaterial;
        if (edgeMaterial != null) edgeMR.sharedMaterial = edgeMaterial;

        generatedMesh = roadMesh;

        if (applyToTerrain)
            ApplyToTerrain(samples);
    }

    public void ClearMesh()
    {
        Transform roadChild = transform.Find("_RoadMesh");
        if (roadChild != null) DestroyImmediate(roadChild.gameObject);
        Transform edgeChild = transform.Find("_EdgeMesh");
        if (edgeChild != null) DestroyImmediate(edgeChild.gameObject);
        generatedMesh = null;
    }

    // Sample the whole spline and return world-space positions
    public List<Vector3> SampleCurveWorldPositions()
    {
        var result = new List<Vector3>();

        if (points == null || points.Count < 2) return result;

        // Convert control points to world space
        var worldPts = new List<Vector3>();
        for (int i = 0; i < points.Count; i++) worldPts.Add(transform.TransformPoint(points[i]));

        // For endpoints, we duplicate start and end to have p0/p3 for the spline segments
        for (int i = 0; i < worldPts.Count - 1; i++)
        {
            // fetch p0..p3 (clamped at ends)
            Vector3 p0 = (i - 1 >= 0) ? worldPts[i - 1] : worldPts[i];
            Vector3 p1 = worldPts[i];
            Vector3 p2 = worldPts[i + 1];
            Vector3 p3 = (i + 2 < worldPts.Count) ? worldPts[i + 2] : worldPts[i + 1];

            // sample between p1 and p2
            int steps = Mathf.Max(1, curveResolution);
            for (int s = 0; s < steps; s++)
            {
                float t = (float)s / (float)steps;
                Vector3 pos = GetCardinalSplinePosition(t, p0, p1, p2, p3, curveTension);
                result.Add(pos);
            }
        }

        // add final point explicitly
        result.Add(transform.TransformPoint(points[points.Count - 1]));

        return result;
    }

    // Cardinal (Catmull-Rom when tension=0) spline interpolation between p1 and p2 with tangents influenced by tension
    public Vector3 GetCardinalSplinePosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float tension)
    {
        // tension: 0 -> Catmull-Rom (smooth), 1 -> 0 tangents (linear)
        float t2 = t * t;
        float t3 = t2 * t;

        Vector3 m1 = (1f - tension) * (p2 - p0) * 0.5f;
        Vector3 m2 = (1f - tension) * (p3 - p1) * 0.5f;

        Vector3 a = 2f * p1 - 2f * p2 + m1 + m2;
        Vector3 b = -3f * p1 + 3f * p2 - 2f * m1 - m2;
        Vector3 c = m1;
        Vector3 d = p1;

        return a * t3 + b * t2 + c * t + d;
    }

    // Terrain integration - overload that takes samples for more accurate adjustment
    public void ApplyToTerrain(List<Vector3> samples = null)
    {
        Terrain t = targetTerrain ? targetTerrain : Terrain.activeTerrain;
        if (t == null) return;

        TerrainData td = t.terrainData;
        Vector3 terrainPos = t.transform.position;

        if (samples == null)
            samples = SampleCurveWorldPositions();

        if (samples == null || samples.Count < 2) return;

        int heightmapWidth = td.heightmapResolution;
        int heightmapHeight = td.heightmapResolution;

        float[,] heights = td.GetHeights(0, 0, heightmapWidth, heightmapHeight);

        for (int i = 0; i < samples.Count - 1; i++)
        {
            Vector3 a = samples[i];
            Vector3 b = samples[i + 1];
            float segLen = Vector3.Distance(a, b);
            int steps = Mathf.Max(1, Mathf.CeilToInt(segLen * 2f));
            for (int s = 0; s <= steps; s++)
            {
                float tParam = (float)s / steps;
                Vector3 pos = Vector3.Lerp(a, b, tParam);

                float px = (pos.x - terrainPos.x) / td.size.x;
                float pz = (pos.z - terrainPos.z) / td.size.z;

                if (px < 0 || px > 1 || pz < 0 || pz > 1) continue;

                int hx = Mathf.Clamp(Mathf.RoundToInt(px * (heightmapWidth - 1)), 0, heightmapWidth - 1);
                int hz = Mathf.Clamp(Mathf.RoundToInt(pz * (heightmapHeight - 1)), 0, heightmapHeight - 1);

                int radius = Mathf.CeilToInt((roadWidth * 0.5f + terrainPadding) / td.size.x * heightmapWidth);

                for (int ox = -radius; ox <= radius; ox++)
                {
                    for (int oz = -radius; oz <= radius; oz++)
                    {
                        int nx = hx + ox;
                        int nz = hz + oz;
                        if (nx < 0 || nx >= heightmapWidth || nz < 0 || nz >= heightmapHeight) continue;

                        float wx = terrainPos.x + (float)nx / (heightmapWidth - 1) * td.size.x;
                        float wz = terrainPos.z + (float)nz / (heightmapHeight - 1) * td.size.z;
                        Vector3 worldSample = new Vector3(wx, 0, wz);

                        Vector3 projected = ClosestPointOnSegment(a, b, worldSample + Vector3.up * pos.y);
                        float dist = Vector3.Distance(new Vector3(projected.x, 0, projected.z), new Vector3(worldSample.x, 0, worldSample.z));

                        if (dist <= roadWidth * 0.5f + terrainPadding)
                        {
                            float desiredNorm = (pos.y - terrainPos.y) / td.size.y;
                            desiredNorm = Mathf.Clamp01(desiredNorm);
                            float current = heights[nz, nx];
                            float blended = Mathf.Lerp(current, desiredNorm, terrainBlend * 0.5f);
                            heights[nz, nx] = blended;
                        }
                    }
                }
            }
        }

        td.SetHeights(0, 0, heights);
    }

    // helper
    Vector3 ClosestPointOnSegment(Vector3 a, Vector3 b, Vector3 p)
    {
        Vector3 ab = b - a;
        float denom = Vector3.Dot(ab, ab);
        if (denom == 0f) return a;
        float t = Vector3.Dot(p - a, ab) / denom;
        t = Mathf.Clamp01(t);
        return a + ab * t;
    }
}
