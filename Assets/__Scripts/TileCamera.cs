using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TileCamera : MonoBehaviour
{
    static private int W, H;
    static private int[,] MAP;
    static public Sprite[] SPRITES;
    static public Transform TILE_ANCHOR;
    static public Tile[,] TILES;

    [Header("Set in Inspector")] 
    public TextAsset mapData;
    public Texture2D mapTiles;
    public TextAsset mapCollisions;
    public Tile tilePrefab;

    private void Awake()
    {
        LoadMap();
    }

    public void LoadMap()
    {
        GameObject go = new GameObject("TILE_ANCHOR");
        TILE_ANCHOR = go.transform;

        SPRITES = Resources.LoadAll<Sprite>(mapTiles.name);

        string[] lines = mapData.text.Split('\n');
        H = lines.Length;
        string[] tileNums = lines[0].Split(' ');
        W = tileNums.Length;

        System.Globalization.NumberStyles hexNum;
        hexNum = System.Globalization.NumberStyles.HexNumber;
        MAP = new int[W, H];
        for (int j = 0; j < H; j++)
        {
            tileNums = lines[j].Split(' ');
            for (int i = 0; i < W; i++)
            {
                if (tileNums[i] == "..")
                {
                    MAP[i, j] = 0;
                }
                else
                {
                    MAP[i, j] = int.Parse(tileNums[i], hexNum);
                }
            }
        }

        print("Parsed " + SPRITES.Length + " sprites.");
        print("Map size: " + W + " wide by " + H + " high");
        
        ShowMap();
    }

    /// <summary>
    /// Генерирует плитки сразу для всей карты
    /// </summary>
    void ShowMap()
    {
        TILES = new Tile[W, H];
        for (int j = 0; j < H; j++)
        {
            for (int i = 0; i < W; i++)
            {
                if (MAP[i, j] != 0)
                {
                    Tile ti = Instantiate<Tile>(tilePrefab);
                    ti.transform.SetParent(TILE_ANCHOR);
                    ti.SetTile(i,j);
                    TILES[i, j] = ti;
                }
            }
        }
    }
    static public int GET_MAP(int x, int y)
    {
        if (x < 0 || x >= W || y < 0 || y >= H)
        {
            return -1;
        }

        return MAP[x, y];
    }

    static public int GET_MAP(float x, float y)
    {
        int tX = Mathf.RoundToInt(x);
        int tY = Mathf.RoundToInt(y - 0.25f);
        return GET_MAP(tX, tY);
    }

    static public void SET_MAP(int x, int y, int tNum)
    {
        if (x < 0 || x >= W || y < 0 || y >= H)
        {
            return;
        }

        MAP[x, y] = tNum;
    }
}
