using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Set Dynamically")] 
    public int x;
    public int y;
    public int tileNum;

    private BoxCollider _boxCollider;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    public void SetTile(int eX, int eY, int eTileNum = -1)
    {
        x = eX;
        y = eY;
        transform.localPosition = new Vector3(x, y, 0);
        gameObject.name = x.ToString("D3") + "x" + y.ToString("D3");

        if (eTileNum == -1)
        {
            eTileNum = TileCamera.GET_MAP(x, y);
        }
        else
        {
            TileCamera.SET_MAP(x, y, eTileNum);
        }

        tileNum = eTileNum;
        GetComponent<SpriteRenderer>().sprite = TileCamera.SPRITES[tileNum];
        
        SetCollider();
    }

    void SetCollider()
    {
        _boxCollider.enabled = true;
        char c = TileCamera.COLLISIONS[tileNum];
        switch (c)
        {
            case 'S': // Вся плитка
                _boxCollider.center = Vector3.zero;
                _boxCollider.size = Vector3.one;
                break;
            case 'W': // Верхняя половина
                _boxCollider.center = new Vector3(0, 0.25f, 0);
                _boxCollider.size = new Vector3(1, 0.5f, 1);
                break;
            case 'A': // Левая половина
                _boxCollider.center = new Vector3(-0.25f, 0, 0);
                _boxCollider.size = new Vector3(0.5f, 1, 1);
                break;
            case 'D': // Правая половина
                _boxCollider.center = new Vector3(0.25f, 0, 0);
                _boxCollider.size = new Vector3(0.5f, 1, 1);
                break;
            default:
                _boxCollider.enabled = false;
                break;
        }
    }
}
