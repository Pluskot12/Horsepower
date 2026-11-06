using UnityEngine;

namespace CarGame
{
    public class RadioStation : MonoBehaviour
    {
        [SerializeField] private RadioStationUI ui;
        [SerializeField] private float maxDistance = 1f;

        private Player player;
        private bool isShowing;

        private void Update()
        {
            if (isShowing && player)
            {
                if (Vector2.Distance(transform.position, player.transform.position) > maxDistance)
                {
                    HideUI();
                }
            }
        }

        public void TryInteract(Player player)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance <= maxDistance)
            {
                if (!isShowing)
                {
                    ShowUI(player);
                }
                else
                {
                    HideUI();
                }
            }
        }

        private void ShowUI(Player player)
        {
            this.player = player;
            isShowing = true;
            ui.Show(player);
        }

        private void HideUI()
        {
            player = null;
            isShowing = false;
            ui.Hide();
        }
    }
}
