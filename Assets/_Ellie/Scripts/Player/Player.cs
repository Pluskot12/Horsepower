using System;
using System.Collections;
using UnityEngine;

namespace CarGame
{
    public class Player : MonoBehaviour, IDamageable
    {
        [Header("References")]

        [SerializeField] private PlayerController playerController;
        [SerializeField] private CarController carController;
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private Animator trunkAnimator;
        [SerializeField] private AttachmentController attachmentController;
        [SerializeField] private HitEffect hitEffect;
        [SerializeField] private PlayerStatPanelUI statPanel;
        [SerializeField] private Transform lookPosition;

        [SerializeField] private DamageSystem damageSystem;
        [SerializeField] private NoiseGenerator noiseGenerator;

        [Header("Abilities")]
        [SerializeField] private Jump jump;
        [SerializeField] private Dash dash;
        //[SerializeField] private Turbo turbo;

        [Header("Stats")]
        [SerializeField] private int health;
        [SerializeField] private float currentHunger;
        [SerializeField] private float maxHunger;
        [SerializeField] private float currentTurbo;
        [SerializeField] private float maxTurbo;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip hungerLowClip;
        [SerializeField] private AudioClip hungerDamageClip;

        public Action<int> OnAttacked;

        //public Turbo Turbo => turbo;
        public Vector3 Velocity => carController.Body.linearVelocity;

        public CarController CarController => carController;
        public AttachmentController Attachments => attachmentController;
        public Dash Dash => dash;

        public AudioSource AudioSource => audioSource;

        public Transform LookPosition => lookPosition;

        public bool IsDead { get; private set; }

        public int MaxHealth { get => health; set => health = value; }
        public int CurrentHealth { get; set; }

        public float Direction => carController.Direction;

        public float EngineVolume => playerController.EngineVolume;

        private void Awake()
        {
            CurrentHealth = MaxHealth;

            currentHunger = maxHunger;

            baseSpeed = carController.GetMaxSpeed();
            baseHunger = maxHunger;

            baseHealth = MaxHealth;
            baseHorsepower = carController.GetHorsepower();
            baseTurbo = maxTurbo;


            //currentTurbo = maxTurbo;

            statPanel.UpdateHealth(CurrentHealth, MaxHealth, true);
            statPanel.UpdateTurbo(currentTurbo, maxTurbo);
            statPanel.UpdateHunger(currentHunger, maxHunger);
            statPanel.UpdateSpeed(0, 1);

            // gameObject.SetActive(false);

            // StartCoroutine(PlaceOnGround());
            carController.AlignToGround(gameObject);
            //healthText.text = CurrentHealth.ToString();
        }

        IEnumerator PlaceOnGround()
        {
            yield return null;

            carController.AlignToGround(gameObject);

            yield return null;

            gameObject.SetActive(true);

        }



        public void Respawn()
        {
            Vector3 spawnPoint = GameManager.Instance.SpawnPoint;

            body.simulated = true;

            CurrentHealth = MaxHealth;

            currentHunger = maxHunger;
            currentTurbo = 0;

            statPanel.UpdateHealth(CurrentHealth, MaxHealth, true);
            statPanel.UpdateTurbo(currentTurbo, maxTurbo);
            statPanel.UpdateHunger(currentHunger, maxHunger);
            statPanel.UpdateSpeed(0, 1);

            damageSystem.UpdateSprite(100, false);
            damageSystem.Respawn();

            attachmentController.ShowAttachment();

            carController.Teleport(spawnPoint, () =>
            {
                IsDead = false;
            });

            //body.simulated = true;

            //IsDead = false;
        }

        private void Update()
        {
            if (IsDead)
            {
                return;
            }

            if (transform.position.y < -100)
            {
                TryDamage(99999);
            }


            if (Input.GetKeyDown(KeyCode.C))
            {
                //TryDamage(99999);
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                // carController.Teleport(new Vector3(0, -3.38f,0f));
            }
            /*
            if (Input.GetKeyDown(KeyCode.L)) 
            {
                GetComponent<IDamageable>().TryDamage(10);
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                AddHealth(10);
            }
            */
            UpdateSpeed();
            UpdateHunger();
        }
        public float CurrentSpeed => Mathf.Abs(body.linearVelocityX);
        public float MaxSpeed => carController.GetMaxSpeed();
        private void UpdateSpeed()
        {
            Vector2 forward = transform.right;
            float forwardSpeed = Vector2.Dot(body.linearVelocity, forward);
            float speedKmh = Mathf.Abs(forwardSpeed);// * 3.6f;
            statPanel.UpdateSpeed(CurrentSpeed, MaxSpeed);
            //statPanel.UpdateSpeed(speedKmh, carController.GetMaxSpeed());
        }
        int hungerDrain = 5;
        float hungerDrainCooldown = 3;
        float hungerDrainTimer = 0;
        bool triggerHungerSound;
        private void UpdateHunger()
        {
            float hungerPerMinute = 1f;

            if (currentHunger <= 0)
            {
                hungerDrainTimer += Time.deltaTime;
                if (hungerDrainTimer >= hungerDrainCooldown)
                {
                    hungerDrainTimer = 0;
                    audioSource.PlayOneShot(hungerDamageClip);
                    GetComponent<IDamageable>().TryDamage(hungerDrain, gameObject);
                }

                if (triggerHungerSound)
                {
                    audioSource.PlayOneShot(hungerLowClip);
                    triggerHungerSound = false;
                }
            }
            else
            {
                triggerHungerSound = true;
            }

            if (Input.GetAxis("Vertical") != 0)
            {
                float hungerToDrain = (hungerPerMinute / 60f) * Time.deltaTime;
                currentHunger = Mathf.Clamp(currentHunger - hungerToDrain, 0, maxHunger);
            }

            statPanel.UpdateHunger(currentHunger, maxHunger);
        }

        public void OnHit(int damage, bool triggerEffects)
        {
            float percentage = (float)CurrentHealth / MaxHealth * 100f;
            damageSystem.UpdateSprite(percentage);

            HitEffect effect = Instantiate(hitEffect, transform.position, Quaternion.identity);

            statPanel.UpdateHealth(CurrentHealth, MaxHealth);
            //healthText.text = CurrentHealth.ToString();
        }

        public void OnDeath()
        {
            if (!IsDead)
            {
                IsDead = true;
                body.simulated = false;
                damageSystem.OnDeath();
                attachmentController.HideAttachment();

                playerController.OnDeath();

                ResetWorkshopUpgrades();

                PlayerInventory.Instance.DropAllItems();
                PlayerGadgets.Instance.DropAllItems();

                GameManager.Instance.OnPlayerDeath();
            }

            statPanel.UpdateHealth(CurrentHealth, MaxHealth);
        }

        public void OnInventory(bool showing)
        {
            if (showing)
            {
                trunkAnimator.Play("Open");
            }
            else
            {
                trunkAnimator.Play("Close");
            }
        }

        public void Pickup(ItemPickup item)
        {
            // Add item to inventory
            // Debug.LogFormat("Picked up {1} ({0}) Durability: {2}", item.Data.displayName, item.Quantity, item.Durability);
            int leftover = PlayerInventory.Instance.InventoryController.OnItemPickup(item.Data, item.Quantity, item.Durability);

            if (leftover <= 0)
            {
                Destroy(item.gameObject);
            }
            else
            {
                item.Quantity = leftover;
                item.DisablePickup(0.1f);
            }
        }

        public bool CanFit(ItemData data, int quantity)
        {
            return PlayerInventory.Instance.InventoryController.CanFit(data, quantity);
        }
        /*
         public bool TryUseTurbo()
         {
             if (TryUseResource(turbo.GetCost(), true))
             {
                 turbo.Activate();

                 return true;
             }

             turbo.Stop();

             return false;

         }
         */
        /*
         public void StopTurbo()
         {
             turbo.Stop();
         }
        */
        public bool TryUseDash(float direction)
        {
            if (dash.IsDashing || dash.IsDashOnCooldown)
            {
                return false;
            }

            if (TryUseResource(dash.GetCost(), true, true))
            {
                dash.Activate(direction);

                return true;
            }

            return false;
        }

        public bool TryJump()
        {
            if (!jump.CanJump)
            {
                return false;
            }

            if (TryUseResource(jump.GetCost(), true, true))
            {
                jump.Activate();

                return true;
            }

            return false;
        }

        public bool TryUseResource(PlayerResourceCost resource, bool ignoreCost = false, bool useOnZero = false)
        {
            if (resource.resource == PlayerResource.None)
            {
                return true;
            }

            if (ignoreCost && GetResource(resource.resource) > 0f)
            {
                UseResource(resource);
                return true;
            }
            else if (ignoreCost && useOnZero)
            {
                UseResource(resource);
                return true;
            }
            else if (GetResource(resource.resource) >= resource.cost)
            {
                UseResource(resource);
                return true;
            }


            return false;
        }

        public void UseResource(PlayerResourceCost resource)
        {
            switch (resource.resource)
            {
                case PlayerResource.None:
                    break;
                case PlayerResource.Health:
                    AddHealth(Mathf.FloorToInt(-resource.cost));
                    break;
                case PlayerResource.Hunger:
                    AddHunger(-resource.cost);
                    break;
                case PlayerResource.Turbo:
                    AddTurbo(-resource.cost);
                    break;
            }
        }

        public float GetResource(PlayerResource resource)
        {
            switch (resource)
            {
                case PlayerResource.None: return 0;
                case PlayerResource.Health: return health;
                case PlayerResource.Hunger: return currentHunger;
                case PlayerResource.Turbo: return currentTurbo;
            }

            return 0;
        }

        public void ApplyItemEffect(ConsumeableItemData itemData)
        {
            audioSource.PlayOneShot(itemData.useClip);

            foreach (var effect in itemData.Effects)
            {
                switch (effect.type)
                {
                    case ConsumeableItemData.EffectType.RestoreHealth:
                        AddHealth(effect.amount);
                        break;
                    case ConsumeableItemData.EffectType.RestoreHunger:
                        AddHunger(effect.amount);
                        break;
                    case ConsumeableItemData.EffectType.RestoreTurbo:
                        AddTurbo(effect.amount);
                        break;
                }
            }
        }

        public void AddHealth(int value, bool playSound = true)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth + value, 0, MaxHealth);
            statPanel.UpdateHealth(CurrentHealth, MaxHealth, true);

            float percentage = (float)CurrentHealth / MaxHealth * 100f;
            damageSystem.UpdateSprite(percentage, playSound);
        }

        public void AddHunger(float value)
        {
            currentHunger = Mathf.Clamp(currentHunger + value, 0, maxHunger);

            statPanel.UpdateHunger(currentHunger, maxHunger);
        }

        public void AddTurbo(float value)
        {
            currentTurbo = Mathf.Clamp(currentTurbo + value, 0, maxTurbo);

            statPanel.UpdateTurbo(currentTurbo, maxTurbo);
        }

        private void UpdateMeters()
        {
            statPanel.UpdateHealth(CurrentHealth, MaxHealth, true);

            float percentage = (float)CurrentHealth / MaxHealth * 100f;
            damageSystem.UpdateSprite(percentage, false);

            statPanel.UpdateHunger(currentHunger, maxHunger);
            statPanel.UpdateTurbo(currentTurbo, maxTurbo);
        }

        public void GenerateNoise(float multiplier)
        {
            noiseGenerator.GenerateNoise(multiplier);
        }

        public void TryDamage(int damage, GameObject attacker = null, bool triggerEffects = true)
        {
            OnAttacked?.Invoke(damage);

            if (!CanTakeDamage())
            {
                return;
            }

            /*
            if (forceShield > 0)
            {
                int absorbed = Mathf.Min(forceShield, damage);
                forceShield -= absorbed;
                damage -= absorbed;

                if (damage <= 0) 
                {
                    return; 
                }
            }
            */
            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);

            if (CurrentHealth <= 0)
            {
                CameraManager.Instance.Shake(3f);
                OnDeath();
            }
            else
            {
                CameraManager.Instance.Shake(1f);
                OnHit(damage, triggerEffects);
            }


        }

        private float speedUpgrade;
        private float fuelUpgrade;
        private float horsePowerUpgrade;

        [SerializeField] private PlayerWorkshopUpgrades workshopUpgrades;
        public int WorkshopUpgradeLevel => workshopUpgrades.UpgradeLevel;
        public PlayerWorkshopUpgrades WorkshopUpgrades => workshopUpgrades;

        public void OnWorkshopUpgrade()
        {
            workshopUpgrades.Upgrade();

            CalculateStats();

            AddHunger(maxHunger);
            AddHealth(MaxHealth);
            AddTurbo(maxTurbo);
        }

        private void ResetWorkshopUpgrades()
        {
            workshopUpgrades.ResetUpgrades();

            CalculateStats();
        }

        private void CalculateStats()
        {
            carController.SetMaxSpeed(CalcMaxSpeed());
            carController.SetHorsepower(CalcMaxHorsepower());
            maxHunger = CalcMaxHunger();
            MaxHealth = CalcMaxHealth();
            maxTurbo = CalcMaxTurbo();
        }

        private float baseSpeed;
        private float baseHunger;

        private int baseHealth;
        private float baseHorsepower;
        private float baseTurbo;


        private float CalcMaxSpeed()
        {
            float speed = baseSpeed;
            speed += workshopUpgrades.CurrentUpgrades.speed / PlayerStatPanelUI.SPEED_MULTI;
            speed += gadgetSpeedBonus / PlayerStatPanelUI.SPEED_MULTI;
            return speed;
        }

        private float CalcMaxHorsepower()
        {
            float horsepower = baseHorsepower;
            horsepower += workshopUpgrades.CurrentUpgrades.horsepower;
            horsepower += gadgetHorsepowerBoost;
            return horsepower;
        }

        private float CalcMaxHunger()
        {
            float hunger = baseHunger;
            hunger += workshopUpgrades.CurrentUpgrades.hunger;
            return hunger;
        }

        private float CalcMaxTurbo()
        {
            float turbo = baseTurbo;
            //turbo += workshopUpgrades.CurrentUpgrades.turbo;
            return turbo;
        }

        private int CalcMaxHealth()
        {
            int health = baseHealth;
            health += workshopUpgrades.CurrentUpgrades.health;
            return health;
        }
        float gadgetSpeedBonus;
        public void SetGadgetTopSpeed(float speed, float duration)
        {
            gadgetSpeedBonus = speed;
            CalculateStats();
            if (duration == 0)
            {
                if (gadgetHorepower != null)
                {
                    StopCoroutine(gadgetSpeed);
                }
            }
            else
            {
                gadgetSpeed = StartCoroutine(GadgetSpeedReset(duration));
            }

        }

        private IEnumerator GadgetSpeedReset(float duration)
        {
            yield return new WaitForSeconds(duration);

            gadgetSpeedBonus = 0f;
            CalculateStats();
        }

        float gadgetHorsepowerBoost;

        Coroutine gadgetSpeed;
        Coroutine gadgetHorepower;
        public void SetHorsepowerBoost(float horsePowerIncrease, float duration)
        {
            gadgetHorsepowerBoost = horsePowerIncrease;
            CalculateStats();

            if (duration == 0)
            {
                if (gadgetHorepower != null)
                {
                    StopCoroutine(gadgetHorepower);
                }
            }
            else
            {
                gadgetHorepower = StartCoroutine(HorsepowerReset(duration));
            }
        }

        private IEnumerator HorsepowerReset(float duration)
        {
            yield return new WaitForSeconds(duration);

            gadgetHorsepowerBoost = 0f;
            CalculateStats();
        }

        private bool hasForceShield;
        public void SetShield(bool hasShield)
        {
            hasForceShield = hasShield;
        }

        private bool CanTakeDamage()
        {
            if (dash.IsImmune)
            {
                return false;
            }

            if (hasForceShield)
            {
                return false;
            }

            return true;
        }

        public PlayerSaveData GetSaveData()
        {
            var data = new PlayerSaveData();

            data.position = transform.position;
            data.scale = transform.localScale;

            data.SpawnPosition = GameManager.Instance.SpawnPoint;

            data.health = CurrentHealth;
            data.turbo = currentTurbo;
            data.hunger = currentHunger;

            data.inventory = PlayerInventory.Instance.GetPlayerInventory();
            data.Gadgets = PlayerGadgets.Instance.GetSaveData();

            data.workshopLevel = workshopUpgrades.UpgradeLevel;
            CalculateStats();

            return data;
        }

        public void OnLoadData(PlayerSaveData data)
        {
            if (data.scale.x == -1)
            {
                carController.FaceLeft();
            }

            if (data.health <= 0)
            {
                // Respawn
                Respawn();
            }
            else
            {
                carController.Teleport(data.position, () => { StartGame(); });

                GameManager.Instance.SetSpawnPoint(null, data.SpawnPosition);

                CurrentHealth = data.health;
                currentTurbo = data.turbo;
                currentHunger = data.hunger;

                UpdateMeters();
            }

            PlayerInventory.Instance.SetPlayerInventory(data.inventory);

            PlayerGadgets.Instance.LoadData(data.Gadgets);

            workshopUpgrades.SetLevel(data.workshopLevel);

            CalculateStats();
        }

        public void StartGame()
        {
            // Enable player
        }
    }

}