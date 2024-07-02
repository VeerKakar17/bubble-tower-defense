using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using System.Threading;
using Unity.VisualScripting;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.U2D;
using UnityEngine.EventSystems;

public class PlayState : State
{
    private TextMeshProUGUI healthText;
    private TextMeshProUGUI moneyText;
    private TextMeshProUGUI roundText;

    private GameObject towerUpgrader;
    private Button upgrader1;
    private Button upgrader2;
    private Button sell;

    public int Money { get; set; }
    public int Health { get; set; }
    
    public int round;
    private float waitTimer;
    private float waitUntil;
    public bool midRound;
    private List<Wave> waves;
    private string[] currentRoundInfo;
    private int currentWave;
    private bool canReadNextWave;
    public Vector3[] waypoints;
    private List<Enemy> enemiesOnScreen;
    private Tower towerUpgrading = null;

    public override void InitializeState()
    {
        base.InitializeState();
        Debug.Log("PlayState Initialized");
        midRound = false;
        Money = 10000;
        Health = 100;
        round = 0;
        currentWave = 0;
        waitTimer = 0;
        waves = new List<Wave>();
        canReadNextWave = false;
        enemiesOnScreen = new List<Enemy>();

        healthText = GameObject.Find("HealthText").GetComponent<TextMeshProUGUI>();
        roundText = GameObject.Find("RoundText").GetComponent<TextMeshProUGUI>();
        moneyText = GameObject.Find("MoneyText").GetComponent<TextMeshProUGUI>();

        towerUpgrader = GameObject.Find("TowerUpgrader");
        upgrader1 = towerUpgrader.transform.GetChild(0).GetComponent<Button>();
        upgrader2 = towerUpgrader.transform.GetChild(1).GetComponent<Button>();
        sell = towerUpgrader.transform.GetChild(2).GetComponent<Button>();
        CloseTowerUpgrader();


        healthText.color = Color.red;
        moneyText.color = Color.yellow;
        roundText.color = Color.black;
        
        Transform waypointsObject = GameObject.Find("Waypoints").transform;
        waypoints = new Vector3[(int)waypointsObject.childCount];
        for (int i = 0; i < waypointsObject.childCount; i++)
        {
            waypoints[i] = waypointsObject.GetChild(i).transform.position;
        }
    }

    public override void UpdateState()
    {
        // updates text
        healthText.text = Health.ToString();
        moneyText.text = Money.ToString();
        roundText.text = "Round " + round.ToString();

        // Handles round and wave logic here if mid round.
        if (midRound)
        {
            if (waitTimer < waitUntil)
                waitTimer += Time.deltaTime;
            else if (canReadNextWave)
                readNextWave();
            foreach (Wave w in waves.ToList())
                w.Update(Time.deltaTime);
            if (currentWave >= currentRoundInfo.Length && enemiesOnScreen.Count == 0)
            {
                midRound = false;
                Debug.Log("Round Finished");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            StartRound();
        }

        // Closes tower upgrader on escape
        if (towerUpgrader.activeSelf && (Input.GetKeyDown(KeyCode.Escape) || (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()))) {
            CloseTowerUpgrader();
        }
    }

    /// <summary>
    /// Called if enemy reaches the end. subtracts dmg from health.
    /// </summary>
    /// <param name="dmg">Damage for enemy to deal if reaches end</param>
    public void EnemyFinish(int dmg)
    {
        Health -= dmg;
        if (Health <= 0)
        {
            GameOver();
        }
    }

    /// <summary>
    /// Ends the game when hp = 0
    /// </summary>
    private void GameOver()
    {
        Debug.Log("Died");
    }

    /// <summary>
    /// Sets up everything for the next round and splits the round text
    /// </summary>
    private void StartRound()
    {
        Debug.Log("Starting Round " + round);
        midRound = true;
        currentRoundInfo = Rounds.roundInfo[round].Split(" ");
        round++;
        currentWave = 0;
        canReadNextWave = true;
    }

    /// <summary>
    /// reads the next wave text and creates a new wave with info. 
    /// </summary>
    private void readNextWave()
    {
        canReadNextWave = false;
        string wave = currentRoundInfo[currentWave];
        if (wave[0] == 'W')
        {
            waitTimer = 0;
            waitUntil = Int32.Parse(wave.Substring(1, wave.Length-1))/1000;
        }
        else
        {
            int index = wave.IndexOf("x");
            int id = Int32.Parse(wave.Substring(0, index));
            wave = wave.Substring(index + 1, wave.Length - (index + 1));

            index = wave.IndexOf("/");
            int num = Int32.Parse(wave.Substring(0, index ));
            wave = wave.Substring(index + 1, wave.Length - (index + 1));

            index = wave.IndexOf("/");
            int spacing = Int32.Parse(wave.Substring(0, index));
            wave = wave.Substring(index+1, wave.Length - (index+1));


            waves.Add(new Wave(id, num, spacing, wave));
        }
        currentWave++;
        if (currentWave < currentRoundInfo.Length)
        {
            canReadNextWave = true;
        }
    }

    public void RemoveEnemy(Enemy e)
    {
        enemiesOnScreen.Remove(e);
    }

    /// <summary>
    /// Sets up the tower upgrader with the paramaters given a tower.
    /// </summary>
    /// <param name="a">Action to be done when upgrade path 1 clicked</param>
    /// <param name="b">Action to be done when upgrade path 2 clicked</param>
    /// <param name="s">Action to be done when sell button clicked</param>
    /// <param name="cost1">Cost of upgrade path 1</param>
    /// <param name="cost2">Cost of upgrade path 2</param>
    /// <param name="sellCost">Money gained when sold</param>
    /// <param name="s1">Upgrade path 1 button sprite</param>
    /// <param name="s2">Upgrade path 2 button sprite</param>
    /// <param name="towerToUpgrade">Tower which is being upgraded</param>
    public void SetTowerUpgrader(UnityAction a, UnityAction b, UnityAction s, int cost1, int cost2, int sellCost, Sprite s1, Sprite s2, Tower towerToUpgrade)
    {
        Debug.Log("Tower Upgrader Opened");
        CloseTowerUpgrader();
        towerUpgrader.SetActive(true);
        towerUpgrading = towerToUpgrade;

        upgrader1.onClick.RemoveAllListeners();
        upgrader2.onClick.RemoveAllListeners();
        
        if (cost1 > -1)
        {
            upgrader1.onClick.AddListener(a);
            upgrader1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = cost1.ToString();
            upgrader1.GetComponent<SpriteRenderer>().sprite = s1;
        }
        else
        {
            upgrader1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        }

        if (cost2 > -1)
        {
            upgrader2.onClick.AddListener(b);
            upgrader2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = cost2.ToString();
            upgrader2.GetComponent<SpriteRenderer>().sprite = s2;
        }
        else 
        {
            upgrader2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        }

        sell.onClick.RemoveAllListeners();
        sell.onClick.AddListener(s);
        sell.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = sellCost.ToString();
    }

    public void CloseTowerUpgrader()
    {
        if (towerUpgrading != null) { towerUpgrading.HideRange(); }
        towerUpgrading = null;
        towerUpgrader.SetActive(false);
    }

    /// <summary>
    /// Logic for each wave that actually summons the new enemies and all
    /// </summary>
    public class Wave
    {
        private int id;
        private int num;
        private float spacing;
        private float timer;
        private string modifiers;

        private int numSpawned;

        public Wave (int id, int num, int spacing, string modifiers)
        {
            this.id = id;
            this.num = num;
            this.spacing = (float) spacing/1000f;
            this.modifiers = modifiers;
            
            numSpawned = 0;
        }

        /// <summary>
        /// Update function that creates each enemy from round info if it is the right time and removes the wave if current wave is done.
        /// </summary>
        /// <param name="dt"></param>
        public void Update(float dt)
        {
            timer += dt;
            if (timer >= spacing)
            {
                CreateEnemy();
                timer = timer - spacing;
                numSpawned++;
            }

            if (numSpawned >= num)
            {
                GameManager.instance.playState.waves.Remove(this);
            }
        }

        private void CreateEnemy()
        {
            Enemy tempEnemy = Instantiate(GameAssets.instance.enemy, GameManager.instance.playState.waypoints[0], new Quaternion()).GetComponent<Enemy>();
            tempEnemy.Initialize(Rounds.enemies[id].moveSpeed, Rounds.enemies[id].dmg, modifiers, Rounds.enemies[id].health, id, Rounds.enemies[id].moneyWorth, Rounds.enemies[id].spawn);
            GameManager.instance.playState.enemiesOnScreen.Add(tempEnemy);
        }
    }
}
