using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        bool isLeftClick = Input.GetMouseButtonDown(0);
        if (isLeftClick || Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("SpecialTile"))
                {
                    SpecialTile tile = hit.transform.gameObject.GetComponent<SpecialTile>();
                    if (isLeftClick)
                        tile.FlickTrueTile(1);
                    else
                        tile.OnTriggered();
                }
            }
        }
    }
}
