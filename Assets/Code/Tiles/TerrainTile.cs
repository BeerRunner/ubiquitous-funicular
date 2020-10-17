using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


// ReSharper disable once RequiredBaseTypesConflict
[CreateAssetMenu(menuName = "Create Terrain Tile", fileName = "TerrainTile", order = 0)]
public class TerrainTile : Tile
{
    public int MoveCost;
    public bool IsBlockingSight;
    public List<Item> Content;
    public Building Building;
}