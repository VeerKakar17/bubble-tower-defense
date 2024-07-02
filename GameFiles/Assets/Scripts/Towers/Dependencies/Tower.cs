using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class Tower : MonoBehaviour, IPointerClickHandler
{
    protected Range RangeObject;
    protected float Range { get { return RangeObject.transform.localScale.x; } set { RangeObject.SetSize(value); } }
    protected int AttackDmg;
    protected int Pierce;
    protected bool CanSeeCamo { get { return RangeObject.canSeeCamo; } set { RangeObject.canSeeCamo = value; } }
    protected int[] level = {0, 0};
    protected int[][] upgradeCosts;
    protected int sellCost;
    [SerializeField] protected Sprite[] addon1Sprites;
    [SerializeField] protected Sprite[] addon2Sprites;
    [SerializeField] protected Sprite[] towerSprites;
    protected SpriteRenderer st;
    protected SpriteRenderer addon1st;
    protected SpriteRenderer addon2st;
    protected int targeting = 0;
    protected float attackCd;
    protected float timer = 0;

    private void Start()
    {
        InitializeTower();
    }

    /// <summary>
    /// Initialize the tower with range and stuff. For each child class, set base stats here.
    /// </summary>
    public virtual void InitializeTower()
    {
        // Handle range stuff
        RangeObject = transform.GetChild(0).GetComponent<Range>();
        st = GetComponent<SpriteRenderer>();
        addon1st = transform.GetChild(1).GetComponent<SpriteRenderer>();
        addon2st = transform.GetChild(2).GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Opens upgrade menu if tower is clicked
    /// </summary>
    /// <param name="eventData">idk ngl</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        CallOpenUpgrader();
    }
    
    /// <summary>
    /// Sells the tower by destroying it and gaining sellCost money
    /// </summary>
    public void Sell()
    {
        GameManager.instance.playState.Money += sellCost;
        GameManager.instance.playState.CloseTowerUpgrader();
        Destroy(gameObject);
    }

    /// <summary>
    /// Upgrades the tower with the path specified. In each tower, make switch statement for actual upgrade and do CallOpenUpgrader at the end.
    /// </summary>
    /// <param name="path1">true if upgrading path one, false if path two</param>
    /// <returns>false if not enough money, true if successfully upgraded</returns>
    public virtual bool Upgrade(bool path1)
    {
        // Stops upgrading if not enough money
        if (GameManager.instance.playState.Money < upgradeCosts[path1?0:1][level[path1?0:1]])
        {
            Debug.Log("Not enough money to upgrade");
            return false;
        }

        GameManager.instance.playState.Money -= upgradeCosts[path1 ? 0 : 1][level[path1 ? 0 : 1]];
        if (path1)
        {
            sellCost += Mathf.RoundToInt(upgradeCosts[0][level[0]] * 0.6f);
            level[0]++;
        }
        else
        {
            sellCost += Mathf.RoundToInt(upgradeCosts[1][level[1]] * 0.6f);
            level[1]++;
        }
        Debug.Log("Leveled Up Path " + (path1?1:2) + " to " + level[(path1?0:1)] + " for " + name);
        return true;
    }

    /// <summary>
    /// Opens the upgrader for this tower
    /// </summary>
    protected void CallOpenUpgrader()
    {
        int c1 = level[0] == 5 ? -1 : upgradeCosts[0][level[0]];
        int c2 = level[1] == 5 ? -1 : upgradeCosts[1][level[1]];

        if (level[0] > 2 && level[1] == 2) c2 = -1;
        else if (level[1] > 2 && level[0] == 2) c1 = -1;

        GameManager.instance.playState.SetTowerUpgrader(delegate { Upgrade(true); }, delegate { Upgrade(false); }, delegate { Sell(); }, c1, c2, sellCost, null, null, this);
        RangeObject.IsVisible = true;
    }

    // Makes the range invisible.
    public void HideRange()
    {
        RangeObject.IsVisible = false;
    }

    /// <summary>
    /// Handles sprite stuff. Also handles when to do attacks
    /// </summary>
    protected virtual void Update()
    {
        addon1st.sprite = addon1Sprites[Math.Min(2, level[0])];
        addon2st.sprite = addon2Sprites[Math.Min(2, level[1])];
        
        if (level[0] > 2)
        {
            st.sprite = towerSprites[level[0] - 2];
        }
        else if (level[1] > 2)
        {
            st.sprite = towerSprites[level[1] + 1];
        }

        if (GameManager.instance.currentState == GameState.Play && GameManager.instance.playState.midRound)
        {
            if (timer < attackCd)
                timer += Time.deltaTime;
            RangeObject.currentCollisions.RemoveAll(item => item == null);
            if (timer >= attackCd && RangeObject.currentCollisions.Count > 0)
            {
                timer = 0;
                Attack();
            }
        }
    }

    /// <summary>
    /// Function called to attack. Happens every AttackCd seconds.
    /// </summary>
    protected virtual void Attack() { }

    public Sprite[] GetDefaultSprites()
    {
        return new Sprite[] { towerSprites[0], addon1Sprites[0], addon2Sprites[0] };
    }
}
