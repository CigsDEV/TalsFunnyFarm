using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BaldiPlusLights : MonoBehaviour
{
    public float range = 25f;
    public float updateInterval = 0.5f;
    public float transitionSpeed = 9f;
    public Color defaultColor = Color.white;
    public string TagToIgnore = "Ignore";

    public bool globalMode = false;  // Global setting flag

    private BoxCollider areaOfEffect;
    private List<Renderer> renderersInArea = new List<Renderer>();
    private List<Light> lightsInArea = new List<Light>();

    private void Start()
    {
        areaOfEffect = GetComponent<BoxCollider>();
        areaOfEffect.isTrigger = true; // Make sure it's a trigger

        // Fetch initial lights and renderers inside the collider
        RefreshObjectsInArea();
        StartCoroutine(UpdateColorCoroutine());
    }

    private void RefreshObjectsInArea()
    {

        if (globalMode)
        {
            foreach (var light in FindObjectsOfType<Light>())
            {
                lightsInArea.Add(light);
            }
            foreach (var renderer in FindObjectsOfType<Renderer>())
            {
                if (renderer.gameObject.tag == TagToIgnore)
                {
                    continue;
                }
                else
                {
                    renderersInArea.Add(renderer);
                }

            }
        }
        else
        {
            foreach (var light in FindObjectsOfType<Light>())
            {
                if (areaOfEffect.bounds.Contains(light.transform.position))
                {
                    lightsInArea.Add(light);
                }
            }

            foreach (var renderer in FindObjectsOfType<Renderer>())
            {
                if (areaOfEffect.bounds.Contains(renderer.transform.position) & renderer.gameObject.tag != TagToIgnore)
                {
                    renderersInArea.Add(renderer);
                }
            }
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (globalMode) return;

        var renderer = other.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderersInArea.Add(renderer);
        }

        var light = other.GetComponent<Light>();
        if (light != null)
        {
            lightsInArea.Add(light);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (globalMode) return;

        var renderer = other.GetComponent<Renderer>();
        if (renderer != null)
        {
            foreach (Material mat in renderer.materials)
            {
                Color targetColor;
                if (mat.HasProperty("_Color"))
                {
                    mat.color = Color.white;
                }
                else
                {
                    continue;
                }
            }
            renderersInArea.Remove(renderer);
        }

        var light = other.GetComponent<Light>();
        if (light != null)
        {
            lightsInArea.Remove(light);
        }
    }

    private IEnumerator UpdateColorCoroutine()
    {
        while (true)
        {
            foreach (var renderer in renderersInArea)
            {
                foreach (Material mat in renderer.materials)
                {
                    Color targetColor;
                    if (mat.HasProperty("_Color"))
                    {
                        targetColor = GetClosestLightColor(renderer.transform.position);
                        Color currentColor = mat.color;
                        mat.color = Color.Lerp(currentColor, targetColor, Time.deltaTime * transitionSpeed);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            yield return new WaitForSeconds(updateInterval);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "LightingArea.png", true);
    }

    private Color GetClosestLightColor(Vector3 position)
    {
        Light closestLight = null;
        float closestDistanceSqr = float.MaxValue;

        foreach (var light in lightsInArea)
        {
            float currentDistanceSqr = (light.transform.position - position).sqrMagnitude;
            if (currentDistanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = currentDistanceSqr;
                closestLight = light;
            }
        }

        return closestLight ? closestLight.color : defaultColor;
    }
}
