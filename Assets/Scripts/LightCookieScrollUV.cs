using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteAlways] // Run in the editor.
public class LightCookieScrollUV : MonoBehaviour
{
    // In URP, we need to scroll the light cookie.
    // -> lightCookieOffset in URP's UniversalAdditionalLightData component.

    UniversalAdditionalLightData lightData;
    public Vector2 speed;

    void Start()
    {

    }

    void Update()
    {
        // If null, assign lightData.

        if (!lightData)
        {
            lightData = GetComponent<UniversalAdditionalLightData>();
        }

        // Scroll UVs of the light cookie texture.

        lightData.lightCookieOffset = speed * Time.time;
    }
}