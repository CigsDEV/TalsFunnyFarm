using UnityEditor;
using UnityEngine;

namespace CigsPlugins
{
    public class BaldiPlusLightsMenu
    {
        [MenuItem("CigsPlugins/BaldiModding Tools/Lights/Create Light Area")]
        static void CreateBaldiPlusLightsGameObject()
        {
            GameObject newObject = new GameObject("Light Area");
            newObject.AddComponent<BaldiPlusLights>();
            BoxCollider boxCollider = newObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(10, 10, 10);

            GameObject childLightObject = new GameObject("Point Light");
            childLightObject.transform.parent = newObject.transform;
            Light light = childLightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 0;

            // Make the new GameObject the active selection
            Selection.activeObject = newObject;
        }

        [MenuItem("CigsPlugins/BaldiModding Tools/Lights/Create Global Light Area")]
        static void CreateGlobalBaldiPlusLightsGameObject()
        {
            GameObject newObject = new GameObject("Light Area");
            BaldiPlusLights BPL = newObject.AddComponent<BaldiPlusLights>();
            BPL.globalMode = true;
            BoxCollider boxCollider = newObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(0, 0, 0);

            GameObject childLightObject = new GameObject("Point Light");
            childLightObject.transform.parent = newObject.transform;
            Light light = childLightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 0;

            // Make the new GameObject the active selection
            Selection.activeObject = newObject;
        }

       
    }
}
