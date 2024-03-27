using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacementController : MonoBehaviour
{
    [SerializeField] private LayerMask tileLayerMask;
    [SerializeField] private Tower towerPrefab;
    private Tower towerPreviewInstance;

    private void Start()
    {
        towerPreviewInstance = Instantiate(towerPrefab, new Vector3(0, -100, 0), Quaternion.identity);
        towerPreviewInstance.enabled = false;
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayerMask))
        {
            BuildableTile hitTile = hit.collider.GetComponent<BuildableTile>();

            if (hitTile.isOccupied)
            {
                HidePreviewTower();
                return;
            }
            
            towerPreviewInstance.transform.position = hitTile.towerPlacementTransform.position;
            
            if (Input.GetMouseButtonDown(0))
            {
                Instantiate(towerPrefab, hitTile.towerPlacementTransform.transform.position, Quaternion.identity); // Spawning a tower on the tile
                hitTile.isOccupied = true;
            }
        }
        else
        {
            HidePreviewTower();
        }
    }

    private void HidePreviewTower()
    {
        towerPreviewInstance.transform.position = new Vector3(0, -100, 0);
    }
}
