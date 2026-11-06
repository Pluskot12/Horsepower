using TMPro;
using UnityEngine;

namespace CarGame
{
    public class PlayerDebugUI : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private TextMeshProUGUI noisyLabel;
        [SerializeField] private TextMeshProUGUI visibleLabel;

        private void Start()
        {
            Init(player);
        }

        public void Init(Player player)
        {
            transform.SetParent(null);
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            this.player = player;
        }

        private void Update()
        {
            noisyLabel.text = player.IsNoisy ? "Noisy" : "Quiet";
            visibleLabel.text = player.IsVisible ? "Visible" : "Hidden";
        }
    }
}
