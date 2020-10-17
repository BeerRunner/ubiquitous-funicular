using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridController : MonoBehaviour
{
    [SerializeField] private Grid _grid;
    [SerializeField] private Tilemap _terrainTilemap;
    [SerializeField] private Tilemap _visibilityTilemap;

    [SerializeField] private Tile _invisibleTile;

    private GridFoWComponent _fogOfWarComponent;

    public static GridController Instance { get; private set; }
    
    private void Awake()
    {
        _fogOfWarComponent = new GridFoWComponent(_terrainTilemap);
        _fogOfWarComponent.ChangedVisibleTile += OnChangedVisibleTile;

        Instance = this;
    }

    private void OnChangedVisibleTile(Vector3Axial axial, GridFoWComponent.VisibleState state)
    {
        Debug.Log($"{axial} is now {state}");

        switch (state)
        {
            case GridFoWComponent.VisibleState.Invisible:
                break;
            case GridFoWComponent.VisibleState.Seen:
                _visibilityTilemap.SetTile(axial, _invisibleTile);
                break;
            case GridFoWComponent.VisibleState.Visible:
                _visibilityTilemap.SetTile(axial, null);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            TerrainTile tile = _terrainTilemap.GetTile<TerrainTile>(_grid.WorldToCell(position));
            
            Debug.Log(_grid.WorldToCell(position) +  " " + (Vector3Axial)_grid.WorldToCell(position));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Axial axialPosition = _grid.WorldToCell(position);
            _fogOfWarComponent.Update(axialPosition, 2);
        }
    }
}