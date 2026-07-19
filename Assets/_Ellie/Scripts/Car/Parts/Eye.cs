using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace CarGame
{
    public class Eye : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private Transform eyeParent;
        [SerializeField] private Transform eyeCenter;

        [Header("Settings")]
        [SerializeField] private bool isPlayer;
        [SerializeField] private float minBlink = 1;
        [SerializeField] private float maxBlink = 3;
        [SerializeField] private float maxDistance = 0.1f;
        [SerializeField] private bool shouldBlink = true;
        [SerializeField] private bool shouldFollow = false;
        [SerializeField] private Transform target;

        private Vector3 startPosition;
        private Camera mainCamera;



        private void Start()
        {
            mainCamera = Camera.main;
            startPosition = eyeParent.localPosition;

            if (animator != null)
            {
                StartCoroutine(Blink());
            }
        }

        private void OnEnable()
        {
            if (isPlayer)
            {
                CinemachineCore.CameraUpdatedEvent.AddListener(OnCinemachineUpdated);
            }
        }

        private void OnDisable()
        {
            if (isPlayer)
            {
                CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCinemachineUpdated);
            }
        }

        private void OnCinemachineUpdated(CinemachineBrain brain)
        {
            if (GameManager.GamePaused)
            {
                return;
            }

            if (shouldFollow && target == null)
            {
                FollowMouse();
            }
        }

        private void LateUpdate()
        {
            if (shouldFollow)
            {
                if (target != null)
                {
                    FollowTarget();
                }
            }
            else
            {
                eyeParent.localPosition = startPosition;
            }

        }

        public void SetFollow(bool follow)
        {
            shouldFollow = follow;
        }

        private void FollowMouse()
        {
            if (!mainCamera) { return; }
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;

            // Convert mouse to local space of the socket — origin is always (0,0) regardless of player movement
            Vector3 localMouse = eyeParent.parent.InverseTransformPoint(mouseWorld);
            localMouse.z = 0f;

            // Clamp in local space
            if (localMouse.magnitude > maxDistance)
            {
                localMouse = localMouse.normalized * maxDistance;
            }

            eyeParent.localPosition = localMouse;
        }

        private void FollowTarget()
        {
            Vector3 direction = target.transform.position - eyeCenter.position;

            if (direction.magnitude > maxDistance)
            {
                direction = direction.normalized * maxDistance;
            }

            Vector3 targetWorldPos = eyeCenter.position + direction;

            eyeParent.localPosition = transform.InverseTransformPoint(targetWorldPos);
        }

        IEnumerator Blink()
        {
            while (shouldBlink)
            {
                float random = Random.Range(minBlink, maxBlink);

                yield return new WaitForSeconds(random);

                animator.Play("Blink");
            }
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }
    }
}