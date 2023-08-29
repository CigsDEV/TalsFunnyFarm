using UnityEngine;
using UnityEditor;

public class GameObjectGrouper : EditorWindow
{
    private string newParentName = "NewParent";

    [MenuItem("CigsPlugins/GameObject Modifiers/Group Selected Objects")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(GameObjectGrouper), false, "Group Objects");
    }

    void OnGUI()
    {
        GUILayout.Label("Group Selected GameObjects", EditorStyles.boldLabel);
        GUILayout.Space(10);
        newParentName = EditorGUILayout.TextField("New Parent Name:", newParentName);

        if (GUILayout.Button("Group"))
        {
            GroupSelectedObjects();
        }
    }

    void GroupSelectedObjects()
    {
        if (Selection.gameObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("No GameObjects selected", "Please select one or more GameObjects in the hierarchy.", "Okay");
            return;
        }

        GameObject newParent = new GameObject(newParentName);
        Vector3 centerPosition = Vector3.zero;

        foreach (GameObject obj in Selection.gameObjects)
        {
            centerPosition += obj.transform.position;
        }

        centerPosition /= Selection.gameObjects.Length;
        newParent.transform.position = centerPosition;

        foreach (GameObject obj in Selection.gameObjects)
        {
            Undo.SetTransformParent(obj.transform, newParent.transform, "Group GameObjects");
        }

        Selection.activeGameObject = newParent;
    }
}
