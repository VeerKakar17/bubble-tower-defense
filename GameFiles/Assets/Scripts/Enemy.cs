using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Enemy : MonoBehaviour
{
    protected float moveSpeed = 2f;
    public int waypointIndex = 0;
    protected Rigidbody2D rb;
    protected int dmg = 1;
    protected int health = 1;
    protected int id;
    protected int moneyWorth;
    protected int[] spawn;
    protected bool scales;
    protected bool bouncing;
    protected string modifiers;
    public bool isCamo { get; protected set;}

    public void Initialize(float moveSpeed, int dmg, string modifiers, int health, int id, int moneyWorth, int[] spawn, bool scales, float size)
    {
        this.health = health;
        this.id = id;
        this.dmg = dmg;
        this.moveSpeed = moveSpeed;
        this.spawn = spawn;
        isCamo = false;
        transform.position = GameManager.instance.playState.waypoints[waypointIndex];
        waypointIndex++;
        this.moneyWorth = moneyWorth;
        GetComponent<SpriteRenderer>().sprite = GameAssets.instance.enemySprites[id];
        gameObject.transform.localScale = new Vector3(size, size, size);
        this.scales = scales;

        this.modifiers = modifiers;
        // if given modifiers, reads it here and activates them
        foreach (char c in modifiers)
        {
            switch (c)
            {
                case 'C':
                    isCamo = true;
                    break;
                case 'B':
                    bouncing = true;
                    break;
                default:
                    break;
            }
        }

        rb = GetComponent<Rigidbody2D>();
        SetVelocity();
    }

    protected void Update()
    {
        // If reaches the next waypoint on the track, moves towards the next one. If at end, destroys itself.
        if (FindDistance(transform.position, GameManager.instance.playState.waypoints[waypointIndex]) < (moveSpeed/100))
        {
            transform.position = GameManager.instance.playState.waypoints[waypointIndex];
            if (waypointIndex == GameManager.instance.playState.waypoints.Length - 1)
            {
                GameManager.instance.playState.EnemyFinish(dmg);
                GameManager.instance.playState.RemoveEnemy(this);
                Destroy(gameObject);
            }
            else
            {
                waypointIndex++;
                SetVelocity();
            }

        }
    }

    /// <summary>
    /// sets velocity of enemy in the direction of the next waypoint.
    /// </summary>
    protected void SetVelocity()
    {
        Vector2 direction = GameManager.instance.playState.waypoints[waypointIndex] - transform.position;
        float theta = Mathf.Atan2(direction.y, direction.x);
        rb.velocity = new Vector2(moveSpeed * Mathf.Cos(theta), moveSpeed * Mathf.Sin(theta));    
    }

    /// <summary>
    /// Finds distance between pos1 and pos2 
    /// </summary>
    /// <param name="pos1">first position</param>
    /// <param name="pos2">second position</param>
    /// <returns>distance between the 2</returns>
    protected float FindDistance(Vector3 pos1, Vector3 pos2)
    {
        float x = pos1.x - pos2.x;
        x *= x;
        float y = pos1.y - pos2.y;
        y *= y;
        return Mathf.Sqrt(x + y);
    }

    /// <summary>
    /// Triggers when bubble takes damage. Deletes bubble if health is less than 0. Scales down and gives money if scaleable bubble.
    /// </summary>
    /// <param name="dmg">amount of health to remove from enemy</param>
    public void DealDamage(int dmgTaken)
    {
        Debug.Log("Damage Dealt to Bloon :O");
        health -= dmgTaken;
        if (scales && health > 0)
        {
            GameManager.instance.playState.Money += moneyWorth * dmgTaken;
            dmg -= dmgTaken;
            moveSpeed -= 1f * dmgTaken;
            float size = gameObject.transform.localScale.x - (0.5f * dmgTaken);
            id -= dmgTaken;
            GetComponent<SpriteRenderer>().sprite = GameAssets.instance.enemySprites[id];
            gameObject.transform.localScale = new Vector3(size, size, size);
            SetVelocity();
        }


        if (spawn != null && spawn.Length > 0)
        {
            if (health < 0)
            {
                dmgTaken = health * -1;
                switchEnemy();
                DealDamage(dmgTaken);
            }
            else if (health == 0)
            {
                GameManager.instance.playState.Money += moneyWorth;
                switchEnemy();
            }
        }
        else
        {
            if (health <= 0)
            {
                if (scales)
                    GameManager.instance.playState.Money += moneyWorth*(dmg+health);
                else
                    GameManager.instance.playState.Money += moneyWorth;

                removeEnemy();
            }
        }
    }

    protected void removeEnemy()
    {
        GameManager.instance.playState.RemoveEnemy(this);
        Destroy(gameObject);
    }

    /// <summary>
    /// Summons enemy spawns when destroyed
    /// </summary>
    protected void switchEnemy()
    {
        foreach (int i in spawn)
        {
            id = i;
            Enemy tempEnemy = Instantiate(GameAssets.instance.enemy, GameManager.instance.playState.waypoints[0], new Quaternion()).GetComponent<Enemy>();
            tempEnemy.Initialize(Rounds.enemies[id].moveSpeed, Rounds.enemies[id].dmg, modifiers, Rounds.enemies[id].health, id, Rounds.enemies[id].moneyWorth, Rounds.enemies[id].spawn, Rounds.enemies[id].scales, Rounds.enemies[id].size);
            GameManager.instance.playState.enemiesOnScreen.Add(tempEnemy);
            tempEnemy.wasInstantiated(waypointIndex, transform.position);
        }

        removeEnemy();
    }

    public float distanceToNextWaypoint()
    {
        return FindDistance(transform.position, GameManager.instance.playState.waypoints[waypointIndex]);
    }

    public void wasInstantiated(int waypoint, Vector3 pos)
    {
        Debug.Log("IS INSTANTIATED");
        waypointIndex = waypoint;
        transform.position = pos;
        SetVelocity();
    }
}
