using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoadSpline))]
public class RoadSplineEditor : Editor
{
    RoadSpline road;
    SerializedProperty pointsProp;

    void OnEnable()
    {
        road = (RoadSpline)target;
        pointsProp = serializedObject.FindProperty("points");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Road Spline Editor", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("roadWidth"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("edgeWidth"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("uvRepeatPerMeter"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Curve Quality", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("curveResolution"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("curveTension"));

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("roadMaterial"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("edgeMaterial"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("targetTerrain"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("terrainBlend"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("terrainPadding"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("autoUpdate"));

        EditorGUILayout.Space();
        if (GUILayout.Button("Generate Road Mesh"))
        {
            Undo.RecordObject(road, "Generate Road");
            road.GenerateRoad(false);
            EditorUtility.SetDirty(road);
        }

        if (GUILayout.Button("Generate & Apply to Terrain"))
        {
            Undo.RecordObject(road, "Generate Road + Terrain");
            road.GenerateRoad(true);
            EditorUtility.SetDirty(road);
        }

        if (GUILayout.Button("Clear Road"))
        {
            Undo.RecordObject(road, "Clear Road");
            road.ClearMesh();
            road.points.Clear();
            EditorUtility.SetDirty(road);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Points (local)", EditorStyles.boldLabel);
        if (pointsProp.isArray)
        {
            for (int i = 0; i < pointsProp.arraySize; i++)
            {
                SerializedProperty p = pointsProp.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(p, new GUIContent("" + i));
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    Undo.RecordObject(road, "Remove Point");
                    pointsProp.DeleteArrayElementAtIndex(i);
                    EditorUtility.SetDirty(road);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        if (GUILayout.Button("Add Point at Scene View Center"))
        {
            Vector3 center = SceneView.lastActiveSceneView.camera.transform.position + SceneView.lastActiveSceneView.camera.transform.forward * 10f;
            Undo.RecordObject(road, "Add Point");
            road.AddPointAtWorldPos(center);
            EditorUtility.SetDirty(road);
        }

        serializedObject.ApplyModifiedProperties();
    }

    void OnSceneGUI()
    {
        if (road.points == null) return;

        Handles.color = Color.cyan;

        // Draw and allow moving points
        for (int i = 0; i < road.points.Count; i++)
        {
            Vector3 worldPos = road.transform.TransformPoint(road.points[i]);
            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.PositionHandle(worldPos, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(road, "Move Point");
                road.points[i] = road.transform.InverseTransformPoint(newPos);
                if (road.autoUpdate) road.GenerateRoad();
            }

            // draw labels
            Handles.Label(worldPos + Vector3.up * 0.2f, i.ToString());
        }

        // Draw poly line between control points (not the sampled curve)
        for (int i = 0; i < road.points.Count - 1; i++)
        {
            Vector3 a = road.transform.TransformPoint(road.points[i]);
            Vector3 b = road.transform.TransformPoint(road.points[i + 1]);
            Handles.DrawLine(a, b);
        }

        // draw sampled (smoothed) curve as well
        var samples = road.SampleCurveWorldPositions();
        if (samples != null && samples.Count > 1)
        {
            Handles.color = Color.yellow;
            Vector3[] sArr = samples.ToArray();
            Handles.DrawPolyLine(sArr);
        }

        // Handle adding points with Shift + Left Click
        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0 && e.shift)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Undo.RecordObject(road, "Add Point via Scene");
                road.AddPointAtWorldPos(hit.point);
                if (road.autoUpdate) road.GenerateRoad();
                e.Use();
            }
            else
            {
                // fallback to plane at y=0
                Plane p = new Plane(Vector3.up, Vector3.zero);
                if (p.Raycast(ray, out float enter))
                {
                    Vector3 pos = ray.GetPoint(enter);
                    Undo.RecordObject(road, "Add Point via Scene");
                    road.AddPointAtWorldPos(pos);
                    if (road.autoUpdate) road.GenerateRoad();
                    e.Use();
                }
            }
        }
    }
}
