using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    private Tower placedTower;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (placedTower != null)
        {
            return;
        }

        var tower = collision.GetComponent<Tower>();

        if (tower != null)
        {
            tower.SetPlacePosition(transform.position);
            placedTower = tower;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (placedTower == null)
        {
            return;
        }

        placedTower.SetPlacePosition(null);
        placedTower = null;
    }

}
