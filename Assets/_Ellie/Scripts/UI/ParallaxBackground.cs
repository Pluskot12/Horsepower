using TriInspector;
using UnityEngine;

namespace CarGame
{
    public class ParallaxBackground : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform container;

        [Header("Settings")]
        [SerializeField] private float smoothTime = 0.15f;
        [SerializeField, Slider(0, 1f)] private float range = 0.5f;

        private Vector2 localMouse;
        private Vector2 velocity;
        private Vector2 maxOffset;

        private void Start()
        {
            float xRange = (background.rect.width - container.rect.width) * 0.5f;
            float yRange = (background.rect.height - container.rect.height) * 0.5f;

            maxOffset = new Vector2(Mathf.Max(0, xRange), Mathf.Max(0, yRange));
        }

        private void Update()
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(container, Input.mousePosition, null, out localMouse);

            Vector2 normalized = new Vector2(
                Mathf.Clamp(localMouse.x / (container.rect.width * 0.5f), -1f, 1f),
                Mathf.Clamp(localMouse.y / (container.rect.height * 0.5f), -1f, 1f)
            );

            Vector2 targetOffset = -normalized * maxOffset * range;

            background.anchoredPosition = Vector2.SmoothDamp(background.anchoredPosition, targetOffset, ref velocity, smoothTime);
        }
    }
}
