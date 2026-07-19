using UnityEngine;
using UnityEngine.UI;

namespace CarGame
{
    public class ProgressBarUI : MonoBehaviour
    {
        [SerializeField] private Image barFill;
        [SerializeField] private float startValue;

        private void Awake()
        {
            Set(startValue);
        }

        public void Set(float progress) 
        {
            barFill.fillAmount = progress;
        }
    }
}
