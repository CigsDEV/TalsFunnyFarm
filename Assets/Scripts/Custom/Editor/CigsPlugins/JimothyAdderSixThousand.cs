using UnityEngine;
using UnityEditor;

public class JimothyAdderSixThousand
{
    private const float SCENE_BOUND_SIZE = 100f;  // Assuming a 100x100x100 box

    [MenuItem("CigsPlugins/Joke/Add Jimothy")]
    public static void AddJimothy()
    {
        // Create a new GameObject named "Jimothy"
        GameObject jimothy = new GameObject("Jimothy");
        
        // Set its position to a random position within the scene bounds
        jimothy.transform.position = new Vector3(
            Random.Range(-SCENE_BOUND_SIZE / 2, SCENE_BOUND_SIZE / 2),
            Random.Range(-SCENE_BOUND_SIZE / 2, SCENE_BOUND_SIZE / 2),
            Random.Range(-SCENE_BOUND_SIZE / 2, SCENE_BOUND_SIZE / 2)
        );
        
        // Select Jimothy in the hierarchy for convenience
        Selection.activeGameObject = jimothy;
    }
}
