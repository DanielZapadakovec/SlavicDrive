using UnityEngine;

public class CarAssembly : MonoBehaviour
{
    [System.Serializable]
    public class CarPartSlot
    {
        public ItemType partType;
        public Transform slotTransform;

        [Header("Preview model (transparent)")]
        public GameObject transparentModel;
        public Material previewMaterial;
        public Material installedMaterial;

        public ParticleSystem installParticles;
        public AudioSource installSound;

        public bool isInstalled = false;
        [HideInInspector] public Renderer previewRenderer;
    }

    public CarPartSlot[] carPartSlots;

    private void Awake()
    {
        // Cache renderery pre preview materiál
        foreach (var slot in carPartSlots)
        {
            if (slot.transparentModel != null)
                slot.previewRenderer = slot.transparentModel.GetComponentInChildren<Renderer>();
        }
    }

    public void ShowSlotPreview(ItemType type, bool state)
    {
        // najprv vypni všetky previews
        foreach (var slot in carPartSlots)
        {
            if (slot.transparentModel != null && !slot.isInstalled)
                slot.transparentModel.SetActive(false);
        }

        // potom zapni iba ten správny
        if (state)
        {
            foreach (var slot in carPartSlots)
            {
                if (slot.partType == type && !slot.isInstalled && slot.transparentModel != null)
                {
                    slot.transparentModel.SetActive(true);
                    if (slot.previewRenderer != null && slot.previewMaterial != null)
                    {
                        slot.previewRenderer.sharedMaterial = slot.previewMaterial;
                    }
                }
            }
        }
    }

    public void HideAllPreviews()
    {
        foreach (var slot in carPartSlots)
        {
            if (slot.transparentModel != null && !slot.isInstalled)
                slot.transparentModel.SetActive(false);
        }
    }

    public bool TryInstallPart(ItemType type, RaycastHit hit)
    {
        foreach (var slot in carPartSlots)
        {
            if (slot.partType == type && !slot.isInstalled)
            {
                if (slot.previewRenderer != null && slot.installedMaterial != null)
                {
                  slot.previewRenderer.sharedMaterial = slot.installedMaterial;
                }

                    slot.isInstalled = true;
                if (slot.installParticles != null) slot.installParticles.Play();
                if (slot.installSound != null) slot.installSound.Play();
                return true;
            }
        }
        return false;
    }
    public bool IsPartInstalled(ItemType type)
    {
        foreach (var slot in carPartSlots)
        {
            if (slot.partType == type) return slot.isInstalled;
        }
        return false;
    }
}
