using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class ColorTower : Tower
{
    protected float[] Cooldown = { -1, -1, -1, -1 };
    protected float BulletSpeed;

    protected bool[] CdReady = {false, false, false, false};

    protected int[] colors = new int[9];
    protected int level = 1;
    protected int Income = 0;

    // timer: {Bullet, yellow money, green poison, blue freeze}
    protected float[] timer = new float[4];

    private void Start()
    {
        InitializeTower();
    }

    public override void InitializeTower()
    {
        Range = 3;
        AttackDmg = 1;
        Pierce = 2;
        CanSeeCamo = false;
        BulletSpeed = 6;
        
        base.InitializeTower();
    }

    void Update()
    {
        // TEST STUFF
        if (Input.GetKeyDown(KeyCode.Alpha0))
            LevelUp(Colors.Red);
        if (Input.GetKeyDown(KeyCode.Alpha1))
            LevelUp(Colors.Yellow);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            LevelUp(Colors.Green);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            LevelUp(Colors.Blue);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            LevelUp(Colors.Orange);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            LevelUp(Colors.Purple);
        if (Input.GetKeyDown(KeyCode.Alpha6))
            LevelUp(Colors.Pink);
        if (Input.GetKeyDown(KeyCode.Alpha7))
            LevelUp(Colors.White);
        if (Input.GetKeyDown(KeyCode.Alpha8))
            LevelUp(Colors.Black);
        if (Input.GetKeyDown(KeyCode.E))
            Instantiate(GameAssets.instance.enemy);



        // Handles cds
        for (int i = 0; i < Cooldown.Length; i++)
        {
            if (Cooldown[i] >= 0)
            {
                HandleCooldown(Time.deltaTime, i);
            }
        }

        if (CdReady[(int) Colors.Yellow])
        {
            YellowAttack();
        }

        // Attacks when cd is up and smth is in range
        if (RangeObject.currentCollisions.Count != 0)
        {
            if (CdReady[(int) Colors.Red])
            {
                Attack();
            }
            if (CdReady[(int)Colors.Blue])
            {
                BlueAttack();
            }
            if (CdReady[(int) Colors.Green])
            {
                GreenAttack();    
            }
        }
    }

    protected void HandleCooldown(float dt, int num)
    {
        if (!CdReady[num] && timer[num] >= Cooldown[num])
        {
            timer[num] = 0;
        }
        if (!CdReady[num] && timer[num] < Cooldown[num])
        {
            timer[num] += Time.deltaTime;
            if (timer[num] > Cooldown[num])
            {
                CdReady[num] = true;
            }
        }
    }

    protected void Attack()
    {
        CdReady[(int)Colors.Red] = false;

        Projectile p = Instantiate(GameAssets.instance.bullet, transform.position, transform.rotation).GetComponent<Projectile>();
        //p.ShootBullet(BulletSpeed, AttackDmg, RangeObject.currentCollisions[0].transform.position, Pierce);
    }

    protected void BlueAttack()
    {
        Debug.Log("ATTACKED SMTH FREEZE OMG");
        CdReady[(int)Colors.Blue] = false;
    }
    protected void GreenAttack()
    {
        Debug.Log("ATTACKED SMTH POISON OMG");
        CdReady[(int)Colors.Green] = false;
    }
    protected void YellowAttack()
    {
        GameManager.instance.playState.Money += Income;
        CdReady[(int)Colors.Yellow] = false;
    }

    // returns false if max lvl, true otherwise
    protected bool LevelUp(Colors color)
    {
        if (level == 0)
        {
            colors[(int)color]++;
            level++;
            switch (color)
            {
                case Colors.Red:
                    AttackDmg = 2;
                    Cooldown[0] = 1f;
                    break;
                case Colors.Orange:
                    Cooldown[0] = 0.8f;
                    break;
                case Colors.Yellow:
                    Income = 10;
                    Cooldown[(int)Colors.Yellow] = 2f;
                    break;
                case Colors.Green:
                    Cooldown[(int)Colors.Green] = 1f;
                    break;
                case Colors.Blue:
                    Cooldown[(int)Colors.Blue] = 1f;
                    break;
                case Colors.Pink:
                    Pierce *= 2;
                    break;
                case Colors.White: break;
                default:
                    Range += 1;
                    break;
            }
        }
        else if (level < 6)
        {
            level += 1;
            colors[(int)color]++;
            Debug.Log("Upgraded to lvl " + level);
            switch (color)
            {
                case Colors.Red:
                    AttackDmg = (int) Mathf.Round(1.5f * AttackDmg);
                    if (Cooldown[(int) Colors.Red] < 0)
                    {
                        Cooldown[(int)Colors.Red] = 1f;
                        for (int i = 0; i < colors[(int) Colors.Orange]; i++)
                        {
                            Cooldown[i] *= 0.9f - (0.1f * colors[(int) Colors.Orange]);
                        }
                    }
                    break;
                case Colors.Orange:
                    for (int i = 0; i < 4; i++)
                    {
                        Cooldown[i] *= 0.9f-(0.1f*colors[(int) Colors.Orange]);
                    }
                    BulletSpeed *= 1.5f;
                    break;
                case Colors.Yellow:
                    Income += 20*colors[(int) Colors.Yellow];
                    if (Cooldown[(int)Colors.Yellow] <= 0)
                    {
                        Cooldown[(int)Colors.Yellow] = 2f;
                        for (int i = 0; i < colors[(int)Colors.Orange]; i++)
                        {
                            Cooldown[i] *= 0.9f - (0.1f * colors[(int)Colors.Orange]);
                        }
                    }
                    break;
                case Colors.Green:
                    if (Cooldown[(int)Colors.Green] <= 0)
                    {
                        Cooldown[(int)Colors.Green] = 2f;
                        for (int i = 0; i < colors[(int)Colors.Orange]; i++)
                        {
                            Cooldown[i] *= 0.9f - (0.1f * colors[(int)Colors.Orange]);
                        }
                    }
                    break;
                case Colors.Blue:
                    if (Cooldown[(int)Colors.Blue] <= 0)
                    {
                        Cooldown[(int)Colors.Blue] = 2f;
                        for (int i = 0; i < colors[(int)Colors.Orange]; i++)
                        {
                            Cooldown[i] *= 0.9f - (0.1f * colors[(int)Colors.Orange]);
                        }
                    }
                    break;
                case Colors.Pink:
                    Pierce *= 2;
                    break;
                case Colors.White:
                    CanSeeCamo = true;
                    break;
                default:
                    Range += 1;
                    break;
            }
        }
        else
        {
            return false;
        }
        return true;
    }

    public enum Colors
    {
        Red = 0,
        Yellow = 1,
        Green = 2,
        Blue = 3,
        Orange = 4,
        Purple = 5,
        Pink = 6,
        White = 7,
        Black = 8
    }

}
