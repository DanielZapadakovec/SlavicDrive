using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    [Header("Light Settings")]
    public Light targetLight;
    public bool isOn = false;

    [Header("Animator (Optional)")]
    public Animator animator;
    public string animatorBoolName = "IsOn";
    public Material lightOnMaterial;
    public Material lightOffMaterial;
    public MeshRenderer lightRenderer;

    private void Start()
    {
        ApplyState();
    }

    public void Toggle()
    {
        isOn = !isOn;
        ApplyState();
    }

    public void TurnOn()
    {
        isOn = true;
        ApplyState();
        lightRenderer.material = lightOnMaterial;
    }

    public void TurnOff()
    {
        isOn = false;
        ApplyState();
        lightRenderer.material = lightOffMaterial;
    }

    void ApplyState()
    {
        if (targetLight != null)
            targetLight.enabled = isOn;

        if (animator != null && !string.IsNullOrEmpty(animatorBoolName))
            animator.SetBool(animatorBoolName, isOn);
    }
}