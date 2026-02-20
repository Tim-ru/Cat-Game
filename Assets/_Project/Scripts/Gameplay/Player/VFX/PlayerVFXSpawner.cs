using UnityEngine;
public class PlayerVFXSpawner : MonoBehaviour
{
    [SerializeField] private GameObject landingVfxPrefab;
    [SerializeField] private GameObject jumpTakeoffVfxPrefab;
    [SerializeField] private GameObject climbVfxPrefab;
    [SerializeField] private float smallJumpSpeed = 12f;
    [SerializeField] private float referenceLandingSpeed = 20f;

    [SerializeField] private float minLandingSpeedForVfx = 2f;
    [SerializeField] private float referenceJumpForce = 20f;
    public void SpawnLanding(Vector3 position, float impactSpeedY)
    {
        if (landingVfxPrefab == null) return;

        float absSpeed = Mathf.Abs(impactSpeedY);
        if (absSpeed < minLandingSpeedForVfx) return;

        float t = (referenceLandingSpeed - smallJumpSpeed) > 0
            ? Mathf.Clamp01((absSpeed - smallJumpSpeed) / (referenceLandingSpeed - smallJumpSpeed))
            : (absSpeed >= referenceLandingSpeed ? 1f : 0f);
        float intensity = Mathf.Lerp(0.1f, 1f, t * t);
        SpawnOneShot(landingVfxPrefab, position, intensity);
    }

    public void SpawnJumpTakeoff(Vector3 position, float jumpForceMagnitude)
    {
        if (jumpTakeoffVfxPrefab == null) return;

        float intensity = Mathf.Clamp01(jumpForceMagnitude / referenceJumpForce);
        SpawnOneShot(jumpTakeoffVfxPrefab, position, intensity);
    }

    public void SpawnClimb(Vector3 position)
    {
        if (climbVfxPrefab == null) return;

        SpawnOneShot(climbVfxPrefab, position, 1f);
    }

    private void SpawnOneShot(GameObject prefab, Vector3 position, float intensity)
    {
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        var oneShot = instance.GetComponent<OneShotVFX>();
        if (oneShot != null)
            oneShot.ApplyIntensity(intensity);
        else
        {
            var ps = instance.GetComponent<ParticleSystem>();
            if (ps != null)
                ps.Play();
        }
    }
}
