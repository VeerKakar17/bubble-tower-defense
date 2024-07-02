using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerBuyButton : MonoBehaviour
{
    [SerializeField] private int towerId;
    [SerializeField] private int cost;
    [SerializeField] private TowerDragIcon towerDragIcon;
    private bool isActive = false;
    private void Start()
    {
        isActive = false;
        towerDragIcon.hide();
    }
    public void whenClicked()
    {
            isActive = true;
        towerDragIcon.gameObject.SetActive(true);
        towerDragIcon.towerId = towerId;
    }

    private void Update()
    {
        if (GameManager.instance.playState.Money < cost)
        {
            towerDragIcon.hasMoney = false;
        }
        else
        {
            towerDragIcon.hasMoney = true;
        }
        if (isActive)
        {
            if (Input.GetMouseButtonDown(0) && towerDragIcon.canPlace)
            {
                Instantiate(GameAssets.instance.towers[towerId], towerDragIcon.transform.position, towerDragIcon.transform.rotation);
                towerDragIcon.hide();
                GameManager.instance.playState.Money -= cost;
                isActive = false;
            }
        }
    }
}
