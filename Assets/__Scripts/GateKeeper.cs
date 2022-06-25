using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateKeeper : MonoBehaviour
{
    // Индексы тайлов с запертыми дверями
    private const int loockedR = 95;
    private const int loockedUR = 81;
    private const int loockedUL = 80;
    private const int loockedL = 100;
    private const int loockedDL = 101;
    private const int loockedDR = 102;
    
    // Индексы тайлов с отпертыми дверями
    private const int openR = 48;
    private const int openUR = 93;
    private const int openUL = 92;
    private const int openL = 51;
    private const int openDL = 26;
    private const int openDR = 27;

    private IKeyMaster keyMaster;

    private void Awake()
    {
        keyMaster = GetComponent<IKeyMaster>();
    }

    private void OnCollisionStay(Collision other)
    {
        if(keyMaster.keyCount < 1) return;

        Tile tile = other.gameObject.GetComponent<Tile>();
        if(tile == null) return;

        int facing = keyMaster.GetFacing();
        Tile tile2;

        switch (tile.tileNum)
        {
            case loockedR:
                if (facing != 0) return;
                tile.SetTile(tile.x, tile.y, openR);
                break;
            case loockedUR:
                if (facing != 1) return;
                tile.SetTile(tile.x, tile.y, openUR);
                tile2 = TileCamera.TILES[tile.x - 1, tile.y];
                tile2.SetTile(tile2.x, tile2.y, openUL);
                break;
            case loockedUL:
                if (facing != 1) return;
                tile.SetTile(tile.x, tile.y, openUL);
                tile2 = TileCamera.TILES[tile.x + 1, tile.y];
                tile2.SetTile(tile2.x, tile2.y, openUR);
                break;
            case loockedL:
                if (facing != 2) return;
                tile.SetTile(tile.x, tile.y, openL);
                break;
            case loockedDR:
                if (facing != 3) return;
                tile.SetTile(tile.x, tile.y, openDR);
                tile2 = TileCamera.TILES[tile.x - 1, tile.y];
                tile2.SetTile(tile2.x, tile2.y, openDL);
                break;
            case loockedDL:
                if (facing != 3) return;
                tile.SetTile(tile.x, tile.y, openDL);
                tile2 = TileCamera.TILES[tile.x + 1, tile.y];
                tile2.SetTile(tile2.x, tile2.y, openDR);
                break;
            default:
                return;
        }

        keyMaster.keyCount--;
    }
}
