using UnityEngine;
using System.Collections.Generic;

public class EnvironmentRandomizer : MonoBehaviour
{
    public Material[] randomMaterials;  // An array of materials you want to randomly assign
    public int seed = 0;                // Seed for consistent randomness
    public int maxWallsToModify = 5;    // Maximum number of walls that can be modified

    private void Start()
    {
        Random.InitState(seed);  // Initialize the random number generator with the given seed

        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        
        // List to store all game objects named "wall"
        List<GameObject> wallObjects = new List<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // Check if the GameObject's name contains "wall" (case-insensitive)
            if (obj.name.ToLower().Contains("wall"))
            {
                wallObjects.Add(obj);
            }
        }

        // Shuffle the list
        for (int i = 0; i < wallObjects.Count; i++)
        {
            GameObject temp = wallObjects[i];
            int randomIndex = Random.Range(i, wallObjects.Count);
            wallObjects[i] = wallObjects[randomIndex];
            wallObjects[randomIndex] = temp;
        }

        // Choose a random number of walls to modify between 0 and the lesser of maxWallsToModify or total number of wall objects
        int numberOfWallsToModify = Random.Range(0, Mathf.Min(maxWallsToModify, wallObjects.Count));

        for (int i = 0; i < numberOfWallsToModify; i++)
        {
            Renderer renderer = wallObjects[i].GetComponent<Renderer>();
            if (renderer != null)
            {
                // Select a random material from the array
                Material newMat = randomMaterials[Random.Range(0, randomMaterials.Length)];

                // Set the primary material to the newly chosen material
                renderer.material = newMat;
            }
        }
    }
}
