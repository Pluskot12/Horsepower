using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CarGame
{
    public class UIMananger : MonoBehaviour
    {

        public static UIMananger Instance { get; internal set; }
        public static InventoryItem HeldItem { get; internal set; }

        [SerializeField] private AudioSource uiAudioSource;
        [SerializeField] private InventoryPanelUI inventoryPanel;
        [SerializeField] private DeathScreenUI deathScreen;
        [SerializeField] private PlayerStatPanelUI statMeters;
        [SerializeField] private ClockUIPanel clock;
        [SerializeField] private PauseMenuUI pauseMenu;

        [Header("Crafting")]
        [SerializeField] private CraftingMenuUI craftingListPanel;
        [SerializeField] private CraftingRecipePanelUI craftingRecipePanel;

        [Header("Buildings")]
        [SerializeField] private WorkshopUI workshopUI;


        private void Awake()
        {
            Instance = this;
        }

        public void OnEscapeKey()
        {
            if (inventoryPanel.TryClose())
            {
                craftingListPanel.Show(false);
                craftingRecipePanel.Show(false);
            }
            else
            {
                pauseMenu.OnEscapeButton();
            }

            /*
            inventoryPanel.Hide();
            craftingListPanel.Show(false, false);
            craftingRecipePanel.Show(false, false);
            statMeters.Hide();
            clock.Hide();*/
        }

        public void ShowDeathScreen()
        {
            inventoryPanel.SetInteractable(false);

            StartCoroutine(DeathScreenDelayed());
        }

        private IEnumerator DeathScreenDelayed()
        {
            yield return new WaitForSeconds(3f);

            HidePlayerUI();

            deathScreen.Show();

        }

        public void ShowPlayerUI()
        {
            inventoryPanel.SetInteractable(true);
            inventoryPanel.Show();
            //craftingListPanel.Show();
            //craftingRecipePanel.Show();
            statMeters.Show();
            clock.Show();
        }

        public void HidePlayerUI()
        {
            inventoryPanel.Hide();
            craftingListPanel.Show(false, false);
            craftingRecipePanel.Show(false, false);
            statMeters.Hide();
            clock.Hide();
        }

        public static bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject.layer == 6) //5 = UI layer, 6 UI Block layer
                {
                    return true;
                }
            }

            return false;
        }

        public void PlayAudioClip(AudioClip clip)
        {
            uiAudioSource.PlayOneShot(clip);
        }

        public void OnPlayerDeath()
        {
            inventoryPanel.OnDeath();
        }

        public static bool IsHoldingItem;

        #region Buildings



        #endregion

    }
}
