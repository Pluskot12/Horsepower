using UnityEngine;

namespace CarGame
{
    public class Bush : MonoBehaviour
    {
        [SerializeField] private Collider2D visionBlocker;

        private void Start()
        {
            GameManager.Instance.Player.OnIsNoisy += Player_OnIsNoisy;
        }

        private void Player_OnIsNoisy(bool noisy)
        {
            visionBlocker.enabled = !noisy;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var player = TryGetPlayer(collision);

            if (player)
            {
                Debug.Log("enter");
                player.SetHidden(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var player = TryGetPlayer(collision);

            if (player)
            {
                Debug.Log("exit");
                player.SetHidden(false);
            }
        }

        private Player TryGetPlayer(Collider2D collision)
        {
            // MEMO: Vision is now blocked by collider

            return null;
            if (collision.attachedRigidbody != null)
            {
                return collision.attachedRigidbody.GetComponent<Player>();
            }

            return null;
        }
    }
}
