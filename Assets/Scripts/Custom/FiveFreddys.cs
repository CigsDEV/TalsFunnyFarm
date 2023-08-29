using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;

public class FiveFreddys : EditorWindow
{
    private float powerLevel = 100f;
    private bool isLeftDoorClosed = false;
    private bool isRightDoorClosed = false;
    private bool gameStarted = false;
    private float elapsedTime = 0f;
    private int night = 1;

    private Dictionary<string, string> animatronicsUrls = new Dictionary<string, string>()
    {
        {
            "Freddy",
            "https://media.discordapp.net/attachments/1134535902564188221/1143929285669834832/image.png?width=600&height=600"
        },
        {
            "Bonnie",
            "https://media.discordapp.net/attachments/1134535902564188221/1143929476699398144/image.png?width=600&height=600"
        },
        {
            "Chica",
            "https://media.discordapp.net/attachments/1134535902564188221/1143929376153534605/image.png?width=600&height=600"
        },
        {
            "Foxy",
            "https://media.discordapp.net/attachments/1134535902564188221/1143929724364664882/image.png?width=600&height=600"
        }
    };

    private Dictionary<string, Texture2D> animatronicsTextures =
        new Dictionary<string, Texture2D>();
    private string animatronicAtDoor = "";
    private float timeAnimatronicAtDoor = 0f;
    private float animatronicApproachRate;

    [MenuItem("CigsPlugins/i'm/gonna/kill/my/self")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<FiveFreddys>("Five Freddys Mockup");
    }

    void OnEnable()
    {
        foreach (var animatronic in animatronicsUrls)
        {
            LoadTexture(animatronic.Key, animatronic.Value);
        }
    }

    void LoadTexture(string name, string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            // This makes the request synchronous
            www.SendWebRequest();

            while (!www.isDone) { } // Wait until the request is done

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture2D myTexture = DownloadHandlerTexture.GetContent(www);
                animatronicsTextures[name] = myTexture;
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Power Level: " + powerLevel.ToString("F1"), EditorStyles.boldLabel);

        if (gameStarted)
        {
            GUILayout.Space(10);

            isLeftDoorClosed = GUILayout.Toggle(isLeftDoorClosed, "Close Left Door");
            isRightDoorClosed = GUILayout.Toggle(isRightDoorClosed, "Close Right Door");

            GUILayout.Space(10);

            if (
                !string.IsNullOrEmpty(animatronicAtDoor)
                && animatronicsTextures.ContainsKey(animatronicAtDoor)
            )
            {
                GUILayout.Label(
                    animatronicsTextures[animatronicAtDoor],
                    GUILayout.Width(200),
                    GUILayout.Height(200)
                );
            }

            GUILayout.Space(10);

            GUILayout.Label($"Night {night}", EditorStyles.boldLabel);
            GUILayout.Label($"Time: {elapsedTime:F1}s", EditorStyles.boldLabel);

            if (powerLevel <= 0 || timeAnimatronicAtDoor > 10)
            {
                gameStarted = false;
                GUILayout.Label("Game Over", EditorStyles.boldLabel);
            }

            if (elapsedTime >= 60 * night)
            {
                if (night == 5)
                {
                    gameStarted = false;
                    GUILayout.Label("You Win!", EditorStyles.boldLabel);
                }
                else
                {
                    night++;
                    elapsedTime = 0;
                    powerLevel = 100f; // Reset power when night changes
                }
            }
        }
        else
        {
            if (GUILayout.Button("Start Game"))
            {
                StartGame();
            }
        }
    }

    void StartGame()
    {
        gameStarted = true;
        powerLevel = 100f;
        night = 1;
        elapsedTime = 0f;
        animatronicApproachRate = 0.1f * night;
    }

    private void OnInspectorUpdate()
    {
        if (gameStarted)
        {
            elapsedTime += 0.02f; // This simulates Time.deltaTime in Editor mode

            // Decrease power automatically
            powerLevel -= 0.02f * (isLeftDoorClosed || isRightDoorClosed ? 2 : 1);

            if (
                string.IsNullOrEmpty(animatronicAtDoor)
                && Random.Range(0f, 1f) < animatronicApproachRate
            )
            {
                animatronicAtDoor = new List<string>(animatronicsUrls.Keys)[
                    Random.Range(0, animatronicsUrls.Count)
                ];
            }

            if (!string.IsNullOrEmpty(animatronicAtDoor))
            {
                if (isLeftDoorClosed || isRightDoorClosed)
                {
                    timeAnimatronicAtDoor = 0f;
                    animatronicAtDoor = "";
                }
                else
                {
                    timeAnimatronicAtDoor += 0.02f; // Simulating Time.deltaTime again
                }
            }
        }

        Repaint();
    }
}
