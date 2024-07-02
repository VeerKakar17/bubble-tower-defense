using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerDragIcon : MonoBehaviour
{
    public List<GameObject> collisions;
    public bool canPlace = true;
    private SpriteRenderer range;
    public bool hasMoney = false;
    private SpriteRenderer st;
    private SpriteRenderer addon1st;
    private SpriteRenderer addon2st;
    private Sprite[] sprites;
    public int towerId = 0;
    private void Awake()
    {
        range = transform.GetChild(0).GetComponent<SpriteRenderer>();
        collisions = new List<GameObject>();
        st = GetComponent<SpriteRenderer>();
        addon1st = transform.GetChild(1).GetComponent<SpriteRenderer>();
        addon2st = transform.GetChild(2).GetComponent<SpriteRenderer>();
        sprites = null;
    }
    private void Update()
    {
        if (sprites == null)
            sprites = GameAssets.instance.towers[towerId].GetComponent<Tower>().GetDefaultSprites();
        st.sprite = sprites[0];
        addon1st.sprite = sprites[1];
        addon2st.sprite = sprites[2];

        transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, transform.position.z);
        if (!hasMoney || collisions.Count > 0)
        {
            canPlace = false;
            range.color = new Color(1,0,0,(101f/255f));
        }
        else
        {
            canPlace = true;
            range.color = new Color(72f/255f, 72f / 255f, 72f/255f, (101f / 255f));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name + " has collided with me :O");
        if (collision.gameObject.tag.Equals("Wall") || collision.gameObject.tag.Equals("NonPlaceableTile") || collision.gameObject.tag.Equals("Tower"))
            collisions.Add(collision.gameObject);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Wall") || collision.gameObject.tag.Equals("NonPlaceableTile") || collision.gameObject.tag.Equals("Tower"))
            collisions.Remove(collision.gameObject);
    }

    public void hide()
    {
        collisions = new List<GameObject>();
        gameObject.SetActive(false);
    }

    private void OnBecameInvisible()
    {
        hide();
    }
}
