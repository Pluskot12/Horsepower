using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ellie.UI
{
    public class Slider : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Transform enemy;
        [SerializeField] private Image slider;
        [SerializeField] private TextMeshProUGUI text;

        private void Start()
        {
            // canvas.transform.SetParent(null);
        }

        private void Update()
        {
            canvas.transform.position = enemy.transform.position + Vector3.up * 1.5f;
            // transform.localScale = Vector3.one * enemy.transform.localScale.x;
        }

        public void SetPercentage(float p)
        {
            if (slider)
                slider.fillAmount = p;
        }

        public void SetColor(Color c)
        {
            slider.color = c;
        }

        public void SetText(string s)
        {
            text.text = s;
        }

        public void Dispose()
        {
            if (this != null)
            {
                Destroy(canvas.gameObject);
            }
        }
    }
}
