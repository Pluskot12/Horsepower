using Ellie.Audio;
using System.Collections;
using UnityEngine;

namespace CarGame
{
    public class SeekerRaidManager : MonoBehaviour
    {
        [System.Serializable]
        public class SeekerRaidContext
        {
            public bool IsRaidDay;
            public float RaidTime;
            public int Count;
        }

        [SerializeField] private EnemyData seeker;
        [SerializeField] private SeekerMark seekerMark;
        [SerializeField] private float seekerSpawnDelay = 60;

        [Header("Audio")]
        [SerializeField] private AudioClip seekerWarningSound;
        [SerializeField] private AudioClip seekerMarkSound;

        public SeekerRaidContext context = new();

        private void Start()
        {
            TimeManager.Instance.OnDayChanged += OnDayChanged;


            // Test Data
            return;
            context.IsRaidDay = true;
            context.RaidTime = 1005;


        }

        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("-----------");
                Debug.Log("3 " + GetEnemyCount(3));
                Debug.Log("6 " + GetEnemyCount(6));
                Debug.Log("9 " + GetEnemyCount(9));
                //TimeManager.Instance.SetDay(3);
                //Show();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                TimeManager.Instance.SetDay(2);
                TimeManager.Instance.SetTime(590);
                //Hide();
            }


            if (!context.IsRaidDay)
            {
                return;
            }

            if (TimeManager.Instance.GetTime() >= context.RaidTime)
            {
                context.IsRaidDay = false;
                context.RaidTime = 0;

                StartCoroutine(SeekerCountdown());
            }




        }


        private void OnDayChanged(int day)
        {
            Debug.Log("Day " + day);

            if (context.IsRaidDay)
            {
                return;
            }

            if (day % 3 == 0)
            {
                Debug.Log("Is raid day!");
                SoundManager.PlayUI(seekerWarningSound);

                context.IsRaidDay = true;
                context.RaidTime = RollSeekerRaidTime();
                //context.RaidTime = TimeManager.Instance.GetTime() + 10; // REMOVE, for testing only
                context.Count = GetEnemyCount(day); // + Random.Range(?, ?);

                Debug.Log("Raid set for " + context.RaidTime);
                Debug.Log("Count " + context.Count);
            }
        }

        private float RollSeekerRaidTime()
        {
            float dayStart = seekerSpawnDelay;
            float dayEnd = TimeManager.Instance.DayLength - seekerSpawnDelay;
            float spawnTime = Random.Range(dayStart, dayEnd);

            return spawnTime;
        }

        private int GetEnemyCount(int day)
        {
            int raidNumber = day / 3;
            int enemyCount = (raidNumber + 1) / 2;
            if (raidNumber % 2 == 0 && Random.value < 0.5f)
            {
                enemyCount++;
            }

            return enemyCount;
        }

        private IEnumerator SeekerCountdown()
        {
            Debug.Log("Starting seeker countdown");

            seekerMark.Show();

            SoundManager.PlaySFX(seekerMarkSound, transform.position);

            yield return new WaitForSeconds(seekerSpawnDelay);

            seekerMark.Hide();

            EnemySpawnManager.Instance.SpawnSeekerRaid(seeker, context.Count);

            Debug.Log("Seeker raid starting");
        }



        public SeekerRaidContext GetSaveData()
        {
            Debug.LogWarning("Getting save data " + context.Count);
            return context;
        }
        public void SetSaveData(SeekerRaidContext data)
        {
            Debug.LogWarning("Setting save data " + data.Count);
            context = data;
        }

    }
}
