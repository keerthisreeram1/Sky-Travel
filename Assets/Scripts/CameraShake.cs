using System.Collections;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private CinemachineBasicMultiChannelPerlin noise;

    void Awake()
    {
       
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        var vcam = GetComponent<CinemachineVirtualCamera>();
        if (vcam != null)
        {
            noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (noise == null)
            {
                Debug.LogWarning($"Noise extension missing on {vcam.name}. Add it via the Virtual Camera's Inspector -> Add Extension -> Noise.");
            }
        }
        else
        {
            Debug.LogWarning($"CinemachineVirtualCamera not found on {gameObject.name}.");
        }
    }

    public void Shake(float intensity = 3f, float duration = 0.2f)
    {
        if (noise == null)
        {
            Debug.LogWarning("CameraShake: noise component is null. Cannot shake.");
            return;
        }

        StartCoroutine(DoShake(intensity, duration));
    }

    private IEnumerator DoShake(float intensity, float duration)
    {
        noise.m_AmplitudeGain = intensity;
        yield return new WaitForSeconds(duration);
        noise.m_AmplitudeGain = 0f;
    }
}
