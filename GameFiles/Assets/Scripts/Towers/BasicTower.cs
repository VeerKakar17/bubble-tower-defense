using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BasicTower : Tower
{
    private float bulletSpeed;
    private bool tripleShot = false;
    [SerializeField] private Sprite[] bulletSprites;
    private int bulletSpriteNum = 0;
    private float bulletSize = 1f;

    public override void InitializeTower()
    {
        base.InitializeTower();

        CanSeeCamo = false;
        Pierce = 1;
        Range = 0.5f;
        AttackDmg = 1;
        attackCd = 1f;
        bulletSpeed = 5f;

        int[] upgradeCosts1 = {  100, 200, 500, 1000, 5000};
        int[] upgradeCosts2 = { 70, 240, 490, 1200, 6300};

        upgradeCosts = new int[][] { upgradeCosts1, upgradeCosts2 };

    }

    /// <summary>
    /// Shoots at the enemy furthest along the track
    /// </summary>
    protected override void Attack()
    {
        RangeObject.sortCollisions(0);
        float SPREAD = 1f;
        Vector3 target = RangeObject.currentCollisions[0].transform.position;
        Instantiate(GameAssets.instance.bullet, transform.position, transform.rotation).GetComponent<Projectile>().ShootBullet(bulletSpeed, AttackDmg, target, Pierce, bulletSprites[bulletSpriteNum], bulletSize, CanSeeCamo);

        if (tripleShot)
        {
            Instantiate(GameAssets.instance.bullet, transform.position, transform.rotation).GetComponent<Projectile>().ShootBullet(bulletSpeed, AttackDmg, new Vector3(target.x - SPREAD, target.y-SPREAD, target.z), Pierce, bulletSprites[bulletSpriteNum], bulletSize, CanSeeCamo);
            Instantiate(GameAssets.instance.bullet, transform.position, transform.rotation).GetComponent<Projectile>().ShootBullet(bulletSpeed, AttackDmg, new Vector3(target.x+SPREAD, target.y+SPREAD, target.z), Pierce, bulletSprites[bulletSpriteNum], bulletSize, CanSeeCamo);
        }
    }

    public override bool Upgrade(bool path1)
    {
        if (!base.Upgrade(path1)) return false;

        if (path1)
        {
            switch (level[0])
            {
                case 1:
                    attackCd = 0.75f;
                    bulletSpeed += 2;
                    break;
                case 2:
                    attackCd = 0.5f;
                    CanSeeCamo = true;
                    break;
                case 3:
                    // baseball
                    bulletSpriteNum = 1;
                    attackCd = 0.3f;
                    Range += 0.3f;
                    bulletSpeed += 4;
                    break;
                case 4:
                    // tennis ball
                    bulletSpriteNum = 2;
                    attackCd = 0.15f;
                    Range += 0.3f;
                    bulletSpeed += 2;
                    break;
                case 5:
                    tripleShot = true;
                    break;
            }
        }
        else
        {
            switch (level[1])
            {
                case 1:
                    AttackDmg += 1;
                    break;
                case 2:
                    AttackDmg += 1;
                    Pierce += 2;
                    break;
                case 3:
                    // basketball
                    bulletSpriteNum = 3;
                    Pierce += 2;
                    AttackDmg += 2;
                    bulletSize = 2f;
                    break;
                case 4:
                    // Bowling ball
                    bulletSpriteNum = 4;
                    AttackDmg += 2;
                    Pierce += 4;
                    bulletSize = 3f;
                    break;
                case 5:
                    // Add ability for smth
                    break;
            }
        }

        CallOpenUpgrader();
        return true;
    }

}
