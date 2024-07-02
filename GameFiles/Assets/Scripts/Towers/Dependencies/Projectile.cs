using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private int dmg;
    private int pierceCap = 100;
    private int pierce = 0;
    private bool canHitCamo;

    /// <summary>
    /// Shoots out a new bullet with the given parameters
    /// </summary>
    /// <param name="speed">speed the bullet travels at</param>
    /// <param name="dmg">damage the bullet deals</param>
    /// <param name="destination">bullet's target (used to find direction)</param>
    /// <param name="pierceCap">self explanatory</param>
    /// <param name="sprite">sprite to set the projectile to</param>
    /// <param name="size">size of the bullet. 1 is default size, 2 is double, 0.5 is half as big, and so on.</param>
    /// <param name="canHitCamo">true if bullet can hit camo, false if not</param>
    public void ShootBullet(float speed, int dmg, Vector3 destination, int pierceCap, Sprite sprite, float size, bool canHitCamo)
    {
        this.dmg = dmg;
        this.pierceCap = pierceCap;
        this.canHitCamo = canHitCamo;

        GetComponent<SpriteRenderer>().sprite = sprite;
        rb = GetComponent<Rigidbody2D>();
        Vector3 direction = destination - transform.position;
        Vector3 rotation = transform.position - destination;
        rb.velocity = new Vector2(direction.x, direction.y).normalized * speed;
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 90);
        transform.localScale = transform.localScale * size;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Collision"))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "Enemy")
        {
            if (collision.gameObject.GetComponent<Enemy>().isCamo && !canHitCamo) return;
            if (pierce < pierceCap)
            {
                collision.GetComponent<Enemy>().DealDamage(dmg);
                pierce++;
            }
            else if (pierce == pierceCap)
            {
                collision.GetComponent<Enemy>().DealDamage(dmg);
                pierce++;
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("how???");
                Destroy(gameObject);
            }
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
