using CarGame;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [SerializeField] private Camera cam;
    [SerializeField] private AudioListener audioListener;

    [Header("Shaker")]
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private float shakeForce = 1f;

    //public Vector3 AudioPosition => audioListener.transform.position;
    public Vector3 AudioPosition => GameManager.Instance.Player.transform.position;

    private void Awake()
    {
        Instance = this;
    }

    public void Shake(float intensity = 1f)
    {
        impulseSource.GenerateImpulse(shakeForce * intensity);
    }
}
