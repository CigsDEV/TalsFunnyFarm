using UnityEngine;
using UnityEditor;
using System.Linq;

public class AlignCeilingsEditor : EditorWindow
{
    public Vector3 targetDirection = Vector3.forward; // Default direction is positive Z-axis
    public Vector3 targetScale = new Vector3(0, -1, 0);
    public string ThingToAlign;

    [MenuItem("CigsPlugins/GameObject Modifiers/Align and Adjust Object")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<AlignCeilingsEditor>("Align and Adjust Object");
    }

    void OnGUI()
    {
        GUILayout.Label(
            "This is made for floor or ceiling tiles, using this on walls or doors is not recommended.",
            EditorStyles.boldLabel
        );
        GUILayout.Space(10); // Adds some spacing

        ThingToAlign = EditorGUILayout.TextField("Name or Part of Name:", ThingToAlign);
        targetDirection = EditorGUILayout.Vector3Field("Target Direction", targetDirection);
        targetScale = EditorGUILayout.Vector3Field("Target Scale", targetScale);

        if (GUILayout.Button($"Align {ThingToAlign}s"))
        {
            AlignAllCeilingsToDirection();
        }

        if (GUILayout.Button($"Adjust {ThingToAlign} Scale"))
        {
            AdjustCeilingScale();
        }
    }

    void AlignAllCeilingsToDirection()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        var ceilingObjects = allObjects.Where(
            obj => obj.name == ThingToAlign || obj.name.Contains(ThingToAlign)
        );

        foreach (GameObject ceiling in ceilingObjects)
        {
            ceiling.transform.forward = targetDirection;
        }
    }

    void AdjustCeilingScale()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        var ceilingObjects = allObjects.Where(
            obj => obj.name == ThingToAlign || obj.name.Contains(ThingToAlign)
        );

        foreach (GameObject ceiling in ceilingObjects)
        {
            ceiling.transform.localScale = targetScale;
        }
    }
}
