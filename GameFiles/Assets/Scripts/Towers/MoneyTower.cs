using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyTower : Tower
{
    private float roundTimer = 0;
    private int baseAttackDmg;
    private float baseAttackCd;
    private bool justChangedRounds;
    private float roundTimerInc;

    private float attCdDecMidRound;
    private float attDmgIncPerRound;
    private int startRound;
    private const float CHANCE_OF_HEALTH = 0.05f;

    public override void InitializeTower()
    {
        base.InitializeTower();

        // Default params that don't affect this tower.
        CanSeeCamo = false;
        Pierce = 0;

        // Attack dmg and cd are for money income each turn
        AttackDmg = 10;
        baseAttackDmg = 10;
        attackCd = 3f;
        baseAttackCd = attackCd;
        Range = 0.5f;

        justChangedRounds = true;
        roundTimerInc = 3f;
        attCdDecMidRound = 0.1f;
        attDmgIncPerRound = 0.1f;
        startRound = GameManager.instance.playState.round;

        // Adds a game object so range attack cd is always running
        RangeObject.currentCollisions.Add(gameObject);

        int[] upgradeCosts1 = { 1, 2, 3, 4, 5 };
        int[] upgradeCosts2 = { 2, 3, 4, 6, 7 };

        upgradeCosts = new int[][] { upgradeCosts1, upgradeCosts2 };
    }

    protected override void Update()
    {
        base.Update();

        // Handles income increase for after and during each round if bottom path.
        if (level[1] > 2)
        {
            if (GameManager.instance.playState.midRound)
            {
                roundTimer += Time.deltaTime;
                if (roundTimer >= roundTimerInc)
                {
                    roundTimer = 0;
                    attackCd = attackCd - attackCd * attCdDecMidRound;
                }
                justChangedRounds = true;
            }
            else if (justChangedRounds)
            {
                if (level[1] > 3)
                {
                    baseAttackDmg = baseAttackDmg + Mathf.RoundToInt(baseAttackDmg * attDmgIncPerRound);
                }
                roundTimer = 0;
                AttackDmg = baseAttackDmg;
                attackCd = baseAttackCd;
                justChangedRounds = false;
            }
        }
    }

    public override bool Upgrade(bool path1)
    {
        if (!base.Upgrade(path1)) return false;

        if (path1)
        {
            // easy setup money method. Consistently makes good money. 
            switch (level[0])
            {
                case 1:
                    baseAttackDmg += 5;
                    AttackDmg += 5;
                    break;
                case 2:
                    baseAttackDmg += 10;
                    AttackDmg += 10;
                    break;
                case 3:
                    attackCd -= 0.75f;
                    AttackDmg += 30;
                    break;
                case 4:
                    // Money amt inc by a lot.
                    AttackDmg += 40;
                    attackCd -= 0.75f;
                    break;
                case 5:
                    // Small chance to get increased health. Money amt inc.
                    AttackDmg += 50;
                    break;
            }
        }
        else
        {
            // more efficient method, but takes setup. Money ramps longer round has been up and longer its been on field.
            switch (level[1])
            {
                case 1:
                    baseAttackCd -= 0.5f;
                    attackCd = baseAttackCd;
                    break;
                case 2:
                    baseAttackCd -= 0.5f;
                    attackCd = baseAttackCd;
                    break;
                case 3:
                    // Longer round been up for, lower money cd.
                    break;
                case 4:
                    // Money income increases by some % every round. Counts rounds placed before this upgrade bought (too op? might remove)
                    for (int i = startRound; i < GameManager.instance.playState.round; i++)
                    {
                        baseAttackDmg = baseAttackDmg + Mathf.RoundToInt(baseAttackDmg * attDmgIncPerRound);
                    }
                    AttackDmg = baseAttackDmg;
                    break;
                case 5:
                    // ability haha? maybe double income for certain amount of time.
                    break;
            }
        }

        CallOpenUpgrader();
        return true;
    }

    /// <summary>
    /// Gives AttackDmg money each attack
    /// </summary>
    protected override void Attack()
    {
        Debug.Log("Tower inc money by " + AttackDmg);
        GameManager.instance.playState.Money += AttackDmg;
        if (level[0] == 5 && Random.value <= CHANCE_OF_HEALTH)
        {
            GameManager.instance.playState.Health += 1;
        }
    }
}
