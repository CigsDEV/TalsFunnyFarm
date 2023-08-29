using UnityEngine;
using UnityEditor;

public class ConvertPlaneToQuadEditor : EditorWindow
{
    [MenuItem("CigsPlugins/BaldiModding Tools/Meshes/Convert Plane to Quad")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<ConvertPlaneToQuadEditor>("Convert Planes to Quads");
    }

    void OnGUI()
    {
        GUIStyle warningStyle = new GUIStyle(EditorStyles.label);
        warningStyle.normal.textColor = Color.red;
        warningStyle.fontStyle = FontStyle.Bold;

        EditorGUILayout.LabelField("WARNING: This tool is in beta, and may cause issues!\nbackup your scene before using.", warningStyle);
        EditorGUILayout.Space();

        if (GUILayout.Button("Convert Planes to Quads"))
        {
            ConvertPlanesToQuads();
        }

        if (GUILayout.Button("Undo Conversion"))
        {
            Undo.PerformUndo();
        }
    }

    void ConvertPlanesToQuads()
    {
        Mesh quadMesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");

        if (quadMesh == null)
        {
            Debug.LogError("Failed to get the Quad mesh.");
            return;
        }

        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
        {
            MeshFilter mf = obj.GetComponent<MeshFilter>();
            MeshCollider mc = obj.GetComponent<MeshCollider>();

            if (mf && mf.sharedMesh && mf.sharedMesh.name == "Plane")
            {
                Undo.RecordObject(mf, "Change Plane to Quad");
                mf.sharedMesh = quadMesh;

                if (mc)
                {
                    // If the MeshCollider is set to Convex, do not assign the quadMesh.
                    if (!mc.convex)
                    {
                        Undo.RecordObject(mc, "Change Plane Collider to Quad");
                        mc.sharedMesh = quadMesh;
                    }
                    else
                    {
                        Debug.LogWarning("MeshCollider on " + obj.name + " is set to Convex. Skipping Quad assignment.");
                    }
                }

                Vector3 originalRotation = obj.transform.localEulerAngles;

                // Normalize tiny rotations
                if (Mathf.Abs(originalRotation.x) < 0.001f) originalRotation.x = 0;
                if (Mathf.Abs(originalRotation.y) < 0.001f) originalRotation.y = 0;
                if (Mathf.Abs(originalRotation.z) < 0.001f) originalRotation.z = 0;

                // Adjust rotations to keep original appearance
                if (Mathf.Approximately(originalRotation.x, 0))
                {
                    obj.transform.localEulerAngles = new Vector3(0, originalRotation.y, originalRotation.z);
                }
                else if (Mathf.Approximately(originalRotation.x, 90))
                {
                    obj.transform.localEulerAngles = new Vector3(-180, originalRotation.y, originalRotation.z);
                }
                else if (Mathf.Approximately(originalRotation.y, 270))
                {
                    obj.transform.localEulerAngles = new Vector3(originalRotation.x, 90, originalRotation.z);
                }
                else
                {
                    // Keep the original rotation for other cases
                    obj.transform.localEulerAngles = originalRotation;
                }

                // Set scale for all converted quads
                obj.transform.localScale = new Vector3(10, -10, 10);

                // Adjust scale and rotation
                Undo.RecordObject(obj.transform, "Adjust Scale and Rotation for Quad");
            }
        }
    }




}
