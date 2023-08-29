using UnityEngine;
using UnityEditor;
using System.Linq;

public class MassRenameEditor : EditorWindow
{
    public string findString;
    public string replaceWith;
    public bool useNumbers;

    [MenuItem("CigsPlugins/GameObject Modifiers/Mass Rename Objects")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<MassRenameEditor>("Mass Rename Objects");
    }

    void OnGUI()
    {
        GUILayout.Label("Find and Rename Objects", EditorStyles.boldLabel);
        GUILayout.Space(10);

        findString = EditorGUILayout.TextField("Find Name or Part of Name:", findString);
        replaceWith = EditorGUILayout.TextField("Replace With:", replaceWith);
        useNumbers = EditorGUILayout.Toggle("Use Numbers", useNumbers);

        if (GUILayout.Button($"Rename Objects"))
        {
            RenameObjects();
        }
    }

    void RenameObjects()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        var objectsToRename = allObjects.Where(
            obj => obj.name == findString || obj.name.Contains(findString)
        ).ToArray();

        int counter = 1;
        foreach (GameObject obj in objectsToRename)
        {
            if (useNumbers)
            {
                obj.name = $"{replaceWith}-{counter++}";
            }
            else
            {
                obj.name = obj.name.Replace(findString, replaceWith);
            }
        }
    }
}
