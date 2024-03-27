using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacementController : MonoBehaviour
{
    [SerializeField] private LayerMask tileLayerMask;
    [SerializeField] private GameObject towerPrefab;
    private GameObject towerPreviewInstance;

    private void Start()
    {
        towerPreviewInstance = Instantiate(towerPrefab, new Vector3(0, -100, 0), Quaternion.identity);
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayerMask))
        {
            towerPreviewInstance.transform.position = hit.collider.transform.position;
        }
    }
}
