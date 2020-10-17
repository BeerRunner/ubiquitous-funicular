using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridFoWComponent
{
    private HashSet<Vector3Axial> _visibleTiles = new HashSet<Vector3Axial>();
    private HashSet<Vector3Axial> _seenTiles = new HashSet<Vector3Axial>();
    private Tilemap _terrainTilemap;

    public event Action<Vector3Axial, VisibleState> ChangedVisibleTile = (tile, visibleState) => { };

    public enum VisibleState
    {
        Invisible,
        Seen,
        Visible,
    }
    
    public GridFoWComponent(Tilemap terrainTilemap)
    {
        _terrainTilemap = terrainTilemap;
    }

    IEnumerator Show(HashSet<Vector3Axial> area)
    {
        foreach (var areaElement in area)
        {
            ChangeVisibleState(areaElement, VisibleState.Visible);
            yield return new WaitForSeconds(1f);
        }
    }
    
    public void Update(Vector3Axial position, int distance)
    {
        HashSet<Vector3Axial> visible = new HashSet<Vector3Axial>();
        HashSet<Vector3Axial> invisible = new HashSet<Vector3Axial>();
        HashSet<Vector3Axial> seen = new HashSet<Vector3Axial>();
        
        HashSet<Vector3Axial> area = position.GetArea(distance);
        visible.Add(position);
        
        foreach (Vector3Axial element in area)
        {
            HashSet<Vector3Axial> line = position.GetLine(element);

            bool isBlocked = false;
            
            foreach (var lineElement in line)
            {
                TerrainTile tile = _terrainTilemap.GetTile<TerrainTile>(lineElement);
                if (!tile || tile.IsBlockingSight)
                {
                    isBlocked = true;
                    break;
                }
            }

            if (isBlocked)
            {
                if (position.Distance(element) == 1)
                    seen.Add(element);
                else
                    invisible.Add(element);
            }
            else
                visible.Add(element);
        }

        if (visible.Overlaps(invisible))
        {
            Debug.LogError("OVERLAPING");
            foreach (var intersect in visible.Intersect(invisible))
                Debug.LogError(intersect);;
        }
            
        Vector3Axial[] wasVisible = _visibleTiles.Except(visible).ToArray();
        Vector3Axial[] willBeVisible = visible.Except(_visibleTiles).ToArray();
        
        foreach (Vector3Axial elem in wasVisible)
            ChangeVisibleState(elem, VisibleState.Seen);

        foreach (Vector3Axial elem in seen.Except(_seenTiles))
            ChangeVisibleState(elem, VisibleState.Seen);
        
        foreach (Vector3Axial elem in willBeVisible)
            ChangeVisibleState(elem, VisibleState.Visible);
    }

    private void ChangeVisibleState(Vector3Axial position, VisibleState state)
    {
        switch (state)
        {
            case VisibleState.Invisible:
                _visibleTiles.Remove(position);
                _seenTiles.Remove(position);
                
                ChangedVisibleTile(position, VisibleState.Invisible);  
                break;
            case VisibleState.Seen:
                _visibleTiles.Remove(position);
                
                if (_seenTiles.Add(position))
                    ChangedVisibleTile(position, VisibleState.Seen);
                break;
            case VisibleState.Visible:
                _seenTiles.Remove(position);
                
                if (_visibleTiles.Add(position))
                    ChangedVisibleTile(position, VisibleState.Visible);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
    
    public bool IsVisibleTiles(Vector3Axial tileAxial)
    {
        return _visibleTiles.Contains(tileAxial);
    }

    public bool IsSeenTiles(Vector3Axial tileAxial)
    {
        return _seenTiles.Contains(tileAxial);
    }
}