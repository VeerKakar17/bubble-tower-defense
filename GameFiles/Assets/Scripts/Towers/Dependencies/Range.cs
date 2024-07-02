using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range : MonoBehaviour
{
    public List<GameObject> currentCollisions = new List<GameObject>();
    public bool canSeeCamo;
    private SpriteRenderer sr;

    /// <summary>
    /// Controls whether this object's sprite renderer is enabled or not
    /// </summary>
    public bool IsVisible { set { sr.enabled = value; } }

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        IsVisible = false;
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        // Add the GameObject collided with to the list.
        Debug.Log("Collided with something");
        if (col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("ITS AN ENEMY OMG");
            if (!canSeeCamo && col.gameObject.GetComponent<Enemy>().isCamo) return;
            currentCollisions.Add(col.gameObject);
        }
    }

    public void OnCollisionExit2D(Collision2D col)
    {
        // Remove the GameObject collided with from the list.
        if (col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            currentCollisions.Remove(col.gameObject);
    }

    public void SetSize(float size)
    {
        transform.localScale = new Vector3 (size, size, size);
    }

    /// <summary>
    /// Sorts all enemys colliding with range by certain order (first = 0, last = 1, strong = 2). 2 NOT IMPLEMENTED YET
    /// </summary>
    /// <param name="sortType">Way to sort. first = 0, last = 1, strong = 2</param>
    public void sortCollisions(int sortType)
    {
        if (sortType == 0)
        {
            currentCollisions.Sort(delegate (GameObject e1, GameObject e2)
            {
                Enemy enemy1 = e1.GetComponent<Enemy>();
                Enemy enemy2 = e2.GetComponent<Enemy>();
                if (enemy1.waypointIndex == enemy2.waypointIndex)
                {
                    return (int)((enemy1.distanceToNextWaypoint() - enemy2.distanceToNextWaypoint()) + 1);
                }
                return (int)((enemy1.waypointIndex - enemy2.waypointIndex) + 1);
            });
            return;
        }
        if (sortType == 1)
        {
            currentCollisions.Sort(delegate (GameObject e1, GameObject e2)
            {
                Enemy enemy2 = e1.GetComponent<Enemy>();
                Enemy enemy1 = e2.GetComponent<Enemy>();
                if (enemy1.waypointIndex == enemy2.waypointIndex)
                {
                    return (int)((enemy1.distanceToNextWaypoint() - enemy2.distanceToNextWaypoint()) + 1);
                }
                return (int)((enemy1.waypointIndex - enemy2.waypointIndex) + 1);
            });
            return;
        }
        if (sortType == 3)
        {
            Debug.Log("WHY SO CRINGE");
        }
    }
}
