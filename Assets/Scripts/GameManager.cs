using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BoardManager _boardManager;

    private Camera _camera;

    #region Helpers
    [ContextMenu("Generate Board 1")]
    public void GenerateBoardHelper1()
    {
        _boardManager.GenerateBoard(Random.Range(0, 3));
    }

    [ContextMenu("Generate Board 2")]
    public void GenerateBoardHelper2()
    {
        _boardManager.GenerateBoard(0);
    }

    [ContextMenu("Flick Board")]
    public void FlickBoard()
    {
        _boardManager.FlickBoard(1.0f);
    }

    [ContextMenu("Clear Board")]
    public void ClearBoard()
    {
        _boardManager.ClearBoard();
    }
    #endregion

    private void Awake()
    {
        if (_boardManager == null)
        {
            _boardManager = GameObject.Find("BoardManager").GetComponent<BoardManager>();
        }
        
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
