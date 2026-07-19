using PrimeTween;
using System;
using UnityEngine;

namespace CarGame
{
    public class AlertNotificationIndicatorUI : MonoBehaviour
    {
        [SerializeField]
        private AlertNotificationPanelUI panel;
        /*
        private bool active;
        public bool Active => active;
        */
        [SerializeField] private RectTransform pivot;
        //[SerializeField] private Image indicator;
        [SerializeField] private RectTransform indicatorRect;
        [SerializeField] private EnemyController target;
        [SerializeField] private float fadeInDuration = 0.5f;
        [SerializeField] private float fadeOutDuration = 0.5f;

        private void Awake()
        {
            indicatorRect.localScale = Vector3.zero;
            //indicator.rectTransform.localScale = Vector3.zero;
        }

        /*
        public void SetTarget(EnemyController target) 
        {
            this.target = target;

            if (target != null)
            {
                SetActive(true);
            }
            else 
            {
                SetActive(false);
            }
        }*/

        [SerializeField] private float rotationSpeed = 1f;
        private void Update()
        {
            if (target)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(target.transform.position);
                Vector3 direction = screenPos - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }

        }
        float vision;
        public void UpdateVision(float vision)
        {
            this.vision = vision;
        }

        public void Activate(EnemyController enemy)
        {
            target = enemy;

            Tween.Scale(indicatorRect, 1f, fadeInDuration);
        }

        public void Deactivate(Action<AlertNotificationIndicatorUI> callback)
        {
            Tween.Scale(indicatorRect, 0f, fadeOutDuration).OnComplete(() => callback.Invoke(this));

        }
    }
}
