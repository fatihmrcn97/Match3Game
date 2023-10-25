using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

public sealed class Board : SingletonMonoBehaviour<Board>
{ 
    public Tile[,] Tiles { get; private set; }

    public int Width => Tiles.GetLength(0);
    public int Height => Tiles.GetLength(1);

    private readonly List<Tile> _selection = new();

    private const float TweenDuration = 0.25f;

    [SerializeField] private Row[] rows;
    [SerializeField] private AudioClip popSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject explosionPref;
    private void Start()
    {
        Tiles = new Tile[rows.Max(row => row.tiles.Length), rows.Length];

        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                var tile = rows[y].tiles[x];

                tile.x = x;
                tile.y = y;
               
                tile.Item = ItemDatabase.Items[Random.Range(0, ItemDatabase.Items.Length)];

                Tiles[x, y] = tile;
            } 
    }


    public async void Select(Tile tile)
    {
        if (!_selection.Contains(tile))
        {
            if (_selection.Count > 0)
            {
                if (Array.IndexOf(_selection[0].Neighbours, tile) != -1)
                {
                    _selection.Add(tile);
                }
            }
            else
            {
                if(tile.Item.isBonus)
                {
                    StartCoroutine(PopAllAround(tile));
                    _selection.Clear();
                    return;
                }
                _selection.Add(tile);
            }

        }



        if (_selection.Count < 2) return;
          
        await Swap(_selection[0], _selection[1]);

        if (CanPop())
        {
            Pop();
        }
        else
            await Swap(_selection[0], _selection[1]);

        _selection.Clear();
    }

    public async Task Swap(Tile tile1 , Tile tile2)
    {
        var icon1 = tile1.icon;
        var icon2 = tile2.icon;

        var icon1Transform = icon1.transform;
        var icon2Transform = icon2.transform;

        var sequence = DOTween.Sequence();

        sequence.Join(icon1Transform.DOMove(icon2Transform.position, TweenDuration))
                .Join(icon2Transform.DOMove(icon1Transform.position, TweenDuration));

        await sequence.Play().AsyncWaitForCompletion();

        icon1Transform.SetParent(tile2.transform);
        icon2Transform.SetParent(tile1.transform);

        tile1.icon = icon2;
        tile2.icon = icon1;

        var tile1ItemTemp = tile1.Item;
        tile1.Item = tile2.Item;
        tile2.Item = tile1ItemTemp;
    }

    private bool CanPop()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (Tiles[x, y].GetConnectedTiles().Skip(1).Count() >= 2) return true;
            }
        }
        return false;
    }

    private async void Pop()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var canBonusCreate=false;
                var tile = Tiles[x, y];

                var connectedTiles = tile.GetConnectedTiles();
                if (connectedTiles.Skip(1).Count() < 2) continue;

                if (connectedTiles.Skip(1).Count() > 2) canBonusCreate = true;

                var deflateSequence = DOTween.Sequence();

                foreach (var conntectedTile in connectedTiles)
                {
                    deflateSequence.Join(conntectedTile.icon.transform.DOScale(Vector3.zero, TweenDuration));
                    conntectedTile.Item.ExlosionEffect(explosionPref, conntectedTile.transform);
                }

                ScoreCounter.Instance.Score += tile.Item.value * connectedTiles.Count;

                audioSource.PlayOneShot(popSound);

                await deflateSequence.Play().AsyncWaitForCompletion();
                 
                var inflateSequence = DOTween.Sequence();

                foreach (var connectedTile in connectedTiles)
                {
                    if(canBonusCreate)
                    {
                        connectedTile.Item = ItemDatabase.BonusItem[0];
                        inflateSequence.Join(connectedTile.icon.transform.DOScale(Vector3.one, TweenDuration));
                        canBonusCreate = false;
                    }
                    else
                    { 
                   
                        connectedTile.Item = ItemDatabase.Items[Random.Range(0, ItemDatabase.Items.Length)];
                        inflateSequence.Join(connectedTile.icon.transform.DOScale(Vector3.one, TweenDuration)); 
                    }
                }

                await inflateSequence.Play().AsyncWaitForCompletion();

                x = 0;
                y = 0;
            }
        }
    }


    private IEnumerator PopAllAround(Tile tile) {

        //FXController.Instance.CreateBombFX(tile.transform.position);

        tile.Item.ExlosionEffect(explosionPref,tile.transform);
        var neigbours = tile.AllNeighbours;
        foreach (var item in neigbours)
        {
            if (item == null) continue;
            Debug.Log(item.name+" "+item.x+" "+item.y);
            item.icon.transform.DOScale(Vector3.zero, TweenDuration); 
            ScoreCounter.Instance.Score += tile.Item.value; 
            audioSource.PlayOneShot(popSound);
            yield return new WaitForSeconds(.5f);
            item.Item = ItemDatabase.Items[Random.Range(0, ItemDatabase.Items.Length)];
            item.icon.transform.DOScale(Vector3.one, TweenDuration);
        } 
        tile.Item = ItemDatabase.Items[Random.Range(0, ItemDatabase.Items.Length)];
    }
}
