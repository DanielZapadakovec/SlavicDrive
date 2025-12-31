using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarAudioSystem : MonoBehaviour
{
    [Header("Wheel setup")]
    public WheelCollider frontLeft;
    public WheelCollider frontRight;
    public WheelCollider rearLeft;
    public WheelCollider rearRight;

    [Header("Surface mappings")]
    public AudioClip[] terrainClips;
    public GameObject[] terrainParticlePrefabs;

    [Header("Non-terrain surfaces")]
    public AudioClip asphaltClip;
    public GameObject asphaltParticlePrefab;

    [Header("Default")]
    public AudioClip defaultSurfaceClip;
    public GameObject defaultParticlePrefab;

    [Header("Audio Sources")]
    [Tooltip("AudioSource pre prehrávanie zvuku terénu")]
    public AudioSource terrainAudioSource;
    public AudioSource skidAudioSource;
    public float skidStartThreshold = 0.25f;
    public float skidMaxPitch = 1.8f;

    [Header("Particles (len zadné kolesá)")]
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;
    public float particleBaseEmission = 1f;
    public float maxParticleScale = 1f;

    [Header("Impact")]
    public AudioSource impactAudioSource;
    public AudioClip[] impactClips;

    private ParticleSystem rearLeftParticles;
    private ParticleSystem rearRightParticles;

    private Terrain terrain;
    private TerrainData terrainData;
    private Rigidbody rb;
    private CarController carController; // voliteľné pre prepojenie s tvojím CarControllerom

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        carController = GetComponent<CarController>();

        terrain = Terrain.activeTerrain;
        if (terrain != null)
            terrainData = terrain.terrainData;

        // Inicializuj particle systémy len pre zadné kolesá
        rearLeftParticles = CreateParticleSystem(rearLeftWheelTransform);
        rearRightParticles = CreateParticleSystem(rearRightWheelTransform);
    }

    void Update()
    {
        float carSpeed = rb.velocity.magnitude * 3.6f; // km/h
        if (carController != null && (!carController.carInteractables.isSeated || !carController.carInteractables.engineRunning))
        {
            StopAllEffects();
            return;
        }

        HandleTerrainAudio(carSpeed);
        HandleRearWheelParticles(carSpeed);
    }

    private void HandleTerrainAudio(float carSpeed)
    {
        // Raycast spod auta (uprostred)
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 2f))
        {
            AudioClip clipToPlay = defaultSurfaceClip;

            if (hit.collider is TerrainCollider && terrain != null)
            {
                int idx = GetMainTextureAtTerrainPosition(hit.point, terrain, terrainData);
                if (idx >= 0 && idx < terrainClips.Length)
                    clipToPlay = terrainClips[idx];
            }
            else
            {
                string tag = hit.collider.tag.ToLower();
                string mat = hit.collider.sharedMaterial ? hit.collider.sharedMaterial.name.ToLower() : "";

                if (tag.Contains("asphalt") || mat.Contains("asphalt"))
                    clipToPlay = asphaltClip;
            }

            if (terrainAudioSource.clip != clipToPlay)
            {
                terrainAudioSource.clip = clipToPlay;
                terrainAudioSource.Play();
            }

            float volume = Mathf.Clamp01(Mathf.InverseLerp(5f, 60f, carSpeed));
            terrainAudioSource.volume = volume * 0.2f;
            terrainAudioSource.pitch = 1f + (carSpeed / 80f);
        }
        else
        {
            if (terrainAudioSource.isPlaying)
                terrainAudioSource.volume = Mathf.MoveTowards(terrainAudioSource.volume, 0f, Time.deltaTime * 1.4f);
        }
    }

    private void HandleRearWheelParticles(float carSpeed)
    {
        HandleSingleWheelParticles(rearLeft, rearLeftParticles, carSpeed);
        HandleSingleWheelParticles(rearRight, rearRightParticles, carSpeed);
    }

    private void HandleSingleWheelParticles(WheelCollider wheel, ParticleSystem ps, float carSpeed)
    {
        if (wheel.GetGroundHit(out WheelHit hit) && carSpeed > 5f)
        {
            float slip = Mathf.Abs(hit.forwardSlip) + Mathf.Abs(hit.sidewaysSlip);

            // intenzita podľa slipu a rýchlosti
            var emission = ps.emission;
            float intensity = Mathf.Clamp01(Mathf.InverseLerp(0f, 40f, carSpeed) + Mathf.Clamp01(slip * 2f));
            emission.rateOverTime = particleBaseEmission * intensity;

            var main = ps.main;
            main.startSize = Mathf.Lerp(0.1f, maxParticleScale, intensity);

            if (!ps.isPlaying) ps.Play();

            // Skid zvuk (šmyk)
            if (skidAudioSource != null && carController.currentSpeed > 10)
            {
                if (slip > skidStartThreshold)
                {
                    if (!skidAudioSource.isPlaying) skidAudioSource.Play();
                    skidAudioSource.volume = Mathf.Clamp01((slip - skidStartThreshold) * 2f);
                    skidAudioSource.pitch = 1f + Mathf.Clamp(carSpeed / 60f, 0f, 0.8f);
                }
                else if (skidAudioSource.isPlaying)
                {
                    skidAudioSource.volume = Mathf.MoveTowards(skidAudioSource.volume, 0f, Time.deltaTime * 2f);
                    if (skidAudioSource.volume <= 0.05f) skidAudioSource.Stop();
                }
            }
        }
        else if (ps.isPlaying)
        {
            ps.Stop();
        }
    }

    private ParticleSystem CreateParticleSystem(Transform wheelTransform)
    {
        GameObject go = new GameObject("WheelParticles_" + wheelTransform.name);
        go.transform.SetParent(wheelTransform, false);
        go.transform.localPosition = Vector3.zero;
        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.loop = true;
        main.playOnAwake = false;
        main.startSize = 0.3f;
        main.startLifetime = 0.5f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        ps.Stop();
        return ps;
    }

    private int GetMainTextureAtTerrainPosition(Vector3 worldPos, Terrain t, TerrainData tData)
    {
        Vector3 terrainPos = worldPos - t.transform.position;
        int mapX = Mathf.FloorToInt((terrainPos.x / tData.size.x) * tData.alphamapWidth);
        int mapZ = Mathf.FloorToInt((terrainPos.z / tData.size.z) * tData.alphamapHeight);
        mapX = Mathf.Clamp(mapX, 0, tData.alphamapWidth - 1);
        mapZ = Mathf.Clamp(mapZ, 0, tData.alphamapHeight - 1);

        float[,,] alphaMaps = tData.GetAlphamaps(mapX, mapZ, 1, 1);
        int numLayers = alphaMaps.GetLength(2);
        float max = 0f;
        int index = 0;
        for (int i = 0; i < numLayers; i++)
        {
            float val = alphaMaps[0, 0, i];
            if (val > max) { max = val; index = i; }
        }
        return index;
    }

    private void StopAllEffects()
    {
        if (terrainAudioSource.isPlaying) terrainAudioSource.Stop();
        if (rearLeftParticles.isPlaying) rearLeftParticles.Stop();
        if (rearRightParticles.isPlaying) rearRightParticles.Stop();
        if (skidAudioSource.isPlaying) skidAudioSource.Stop();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (impactAudioSource == null || impactClips.Length == 0) return;

        float impulse = collision.impulse.magnitude;
        if (impulse < 100f) return;

        int idx = Random.Range(0, impactClips.Length);
        impactAudioSource.pitch = 1f + Mathf.Clamp(impulse / 300f, -0.2f, 1.2f);
        impactAudioSource.volume = Mathf.Clamp01(impulse / 250f);
        impactAudioSource.PlayOneShot(impactClips[idx], impactAudioSource.volume);
    }
}
