using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class TowerUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image towerIcon;

    [SerializeField] private Tower towerPrefab;
    private Tower currentSpawnedTower;

    public void OnBeginDrag(PointerEventData eventData)
    {
        var newTowerObj = Instantiate(towerPrefab.gameObject);
        currentSpawnedTower = newTowerObj.GetComponent<Tower>();
        currentSpawnedTower.ToggleOrderInLayer(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        var mainCamera = Camera.main;
        var mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z;
        var targetPos = Camera.main.ScreenToWorldPoint(mousePos);

        currentSpawnedTower.transform.position = targetPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentSpawnedTower.PlacePosition == null)
        {
            Destroy(currentSpawnedTower.gameObject);
        }
        else
        {
            currentSpawnedTower.LockPlacement();
            currentSpawnedTower.ToggleOrderInLayer(false);
            LevelManager.Instance.RegisterSpawnedTower(currentSpawnedTower);
            currentSpawnedTower = null;
        }
    }

    public void SetTowerPrefab(Tower tower)
    {
        towerPrefab = tower;
        towerIcon.sprite = tower.GetTowerHeadIcon();
    }
}
