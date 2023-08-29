using UnityEngine;
using UnityEditor;
using System.Linq;

public class MaterialChanger : EditorWindow
{
    public string findString;
    public Material targetMaterial;
    private Material findMaterial;
    private Material[] allMaterials;
    private int targetMaterialIndex;
    private int findMaterialIndex;

    public bool replaceByGameObjectName = true;
    public bool replaceByMaterialName;
    public bool addMaterialOverlay;

    [MenuItem("CigsPlugins/GameObject Modifiers/Material Changer")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<MaterialChanger>("Material Changer");
    }

    private void OnEnable()
    {
        allMaterials = Resources.FindObjectsOfTypeAll<Material>();
    }

    void OnGUI()
    {
        GUILayout.Label("Find and Change Materials on Objects", EditorStyles.boldLabel);
        GUILayout.Space(10);

        replaceByGameObjectName = EditorGUILayout.Toggle("By GameObject Name", replaceByGameObjectName);
        if (replaceByGameObjectName)
        {
            replaceByMaterialName = false;
            findString = EditorGUILayout.TextField("Find Name or Part of Name:", findString);
        }

        replaceByMaterialName = EditorGUILayout.Toggle("By Material Name", replaceByMaterialName);
        if (replaceByMaterialName)
        {
            replaceByGameObjectName = false;
            findMaterialIndex = EditorGUILayout.Popup("Find Material", findMaterialIndex, allMaterials.Select(m => m.name).ToArray());
            findMaterial = allMaterials[findMaterialIndex];
        }

        targetMaterialIndex = EditorGUILayout.Popup("Target Material", targetMaterialIndex, allMaterials.Select(m => m.name).ToArray());
        targetMaterial = allMaterials[targetMaterialIndex];

        addMaterialOverlay = EditorGUILayout.Toggle("Add As Overlay", addMaterialOverlay);

        if (GUILayout.Button($"Change Materials"))
        {
            ChangeMaterials();
        }
    }

    void ChangeMaterials()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        var objectsToChange = allObjects.Where(obj =>
        {
            var renderer = obj.GetComponent<Renderer>();
            if (!renderer || renderer is SpriteRenderer) return false;

            if (replaceByGameObjectName)
            {
                return obj.name.Contains(findString);
            }
            else if (replaceByMaterialName)
            {
                return renderer.sharedMaterials.Contains(findMaterial);
            }
            return false;
        });

        foreach (GameObject obj in objectsToChange)
        {
            var renderer = obj.GetComponent<Renderer>();
            if (addMaterialOverlay)
            {
                Undo.RecordObject(renderer, "Add Material Overlay");  // Add undo for material addition
                var materialsList = renderer.sharedMaterials.ToList();
                materialsList.Add(targetMaterial);
                renderer.sharedMaterials = materialsList.ToArray();
            }
            else
            {
                Undo.RecordObject(renderer, "Change Material");  // Add undo for material change
                renderer.sharedMaterial = targetMaterial;
            }
        }
    }
}
