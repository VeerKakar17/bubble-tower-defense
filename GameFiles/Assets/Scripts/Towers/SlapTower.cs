using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlapTower : Tower
{
    private GameObject attackHitbox;
    private Animator anim;

    public override void InitializeTower()
    {
        base.InitializeTower();

        attackHitbox = gameObject.transform.GetChild(0).GetComponent<GameObject>();
        anim = gameObject.GetComponent<Animator>();
        CanSeeCamo = false;
        Pierce = 3;
        Range = 0.2f;
        AttackDmg = 2;
        attackCd = 1f;

        int[] upgradeCosts1 = { 100, 200, 500, 1000, 5000 };
        int[] upgradeCosts2 = { 70, 240, 490, 1200, 6300 };

        upgradeCosts = new int[][] { upgradeCosts1, upgradeCosts2 };

    }

    /// <summary>
    /// Slaps at the enemy furthest along the track 
    /// </summary>
    protected override void Attack()
    {
        RangeObject.sortCollisions(0);
        Vector3 target = RangeObject.currentCollisions[0].transform.position;
        anim.SetTrigger("Attack");
    }

    public override bool Upgrade(bool path1)
    {
        if (!base.Upgrade(path1)) return false;

        if (path1)
        {
            // speed based path. Starts off getting faster, ends up with rapid fire ability and 2 hands
            switch (level[0])
            {
                case 1:
                    
                    break;
                case 2:
                    
                    break;
                case 3:
                    
                    break;
                case 4:
                    
                    break;
                case 5:
                    
                    break;
            }
        }
        else
        {
            // power based path. Gets bigger and stronger hand with more pierce. end up with a stun as well.
            switch (level[1])
            {
                case 1:
                    
                    break;
                case 2:
                    
                    break;
                case 3:
                    
                    break;
                case 4:
                    
                    break;
                case 5:
                   
                    break;
            }
        }

        CallOpenUpgrader();
        return true;
    }
}
