using System.Collections.Generic;
using UnityEngine;

namespace CarGame
{
    public class PlayerTrailer : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private Trailer trailer;
        [SerializeField] private Transform trailerAnchor;
        [SerializeField] private List<Trailer.TrailerVariant> variants;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                trailer.Setup(player.CarController, trailerAnchor);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                trailer.Setup(player.CarController, trailerAnchor);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                trailer.Setup(player.CarController, trailerAnchor);
            }
        }
    }
}
