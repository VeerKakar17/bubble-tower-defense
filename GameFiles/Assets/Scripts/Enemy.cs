using System.Collections;
using System.Collections.Generic;
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
    protected int spawn;
    public bool isCamo { get; protected set;}

    public void Initialize(float moveSpeed, int dmg, string modifiers, int health, int id, int moneyWorth, int spawn)
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

        // if given modifiers, reads it here and activates them
        foreach (char c in modifiers)
        {
            switch (c)
            {
                case 'C':
                    isCamo = true;
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
    /// Triggers when at end of track. Destroys self and removes dmg health.
    /// </summary>
    /// <param name="dmg">amount of health to remove from player</param>
    public void DealDamage(int dmg)
    {
        Debug.Log("Damage Dealt to Bloon :O");
        health -= dmg;
        if (spawn > -1)
        {
            if (health < 0)
            {
                dmg = health * -1;
                switchEnemy();
                DealDamage(dmg);
            }
            else if (health == 0)
            {
                switchEnemy();
            }
        }
        else
        {
            if (health <= 0)
            {
                removeEnemy();
            }
        }
    }

    protected void removeEnemy()
    {
        GameManager.instance.playState.Money += moneyWorth;
        GameManager.instance.playState.RemoveEnemy(this);
        Destroy(gameObject);
    }

    protected void switchEnemy()
    {
        id = spawn;
        health = Rounds.enemies[id].health;
        dmg = Rounds.enemies[id].dmg;
        moveSpeed = Rounds.enemies[id].moveSpeed;
        moneyWorth = Rounds.enemies[id].moneyWorth;
        spawn = Rounds.enemies[id].spawn;

        GetComponent<SpriteRenderer>().sprite = GameAssets.instance.enemySprites[id];

        SetVelocity();
    }

    public float distanceToNextWaypoint()
    {
        return FindDistance(transform.position, GameManager.instance.playState.waypoints[waypointIndex]);
    }
}
