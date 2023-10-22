using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class Tile : MonoBehaviour
{

    public int x;
    public int y;

    private Item _item;
    public Item Item
    {
        get => _item;
        set
        {
            if (_item == value) return;
            _item = value;
            icon.sprite = _item.sprite;
        }
    }

    public Image icon;

    public Button button;

    public Tile Left => x > 0 ? Board.Instance.Tiles[x - 1, y] : null;
    public Tile Top => y > 0 ? Board.Instance.Tiles[x, y - 1] : null;
    public Tile Right => x < Board.Instance.Width - 1 ? Board.Instance.Tiles[x + 1, y] : null;
    public Tile Bottom => y < Board.Instance.Height - 1 ? Board.Instance.Tiles[x, y + 1] : null;

    public Tile TileLeftTopCorner => x - 1 > 0 && y - 1 > 0 ? Board.Instance.Tiles[x-1,y-1] : null;
    public Tile TileRightTopCorner => x + 1 < Board.Instance.Width-1 && y > 0 ? Board.Instance.Tiles[x+1,y-1] : null;
    public Tile TileBottomLeftCorner => x - 1 > 0 && y < Board.Instance.Height-1 ? Board.Instance.Tiles[x-1,y+1] : null;
    public Tile TileBottomRightCorner => x + 1 > Board.Instance.Width-1 && y+1 > Board.Instance.Height-1 ? Board.Instance.Tiles[x+1,y+1] : null;


    public Tile[] Neighbours => new[] { Left,Top,Right,Bottom};
    public Tile[] AllNeighbours => new[] { Left, Top, Right, Bottom, TileLeftTopCorner, TileRightTopCorner, TileBottomLeftCorner, TileBottomRightCorner };

    private void Awake()
    {
        button.onClick.AddListener(() => Board.Instance.Select(this));
    }

    public List<Tile> GetConnectedTiles(List<Tile> exclude = null)
    {
        var result = new List<Tile> { this, };

        if (exclude == null)
        {
            exclude = new List<Tile> { this, };
        }
        else
        {
            exclude.Add(this);
        }

        foreach (var neigbour in Neighbours)
        {
            if (neigbour == null || exclude.Contains(neigbour) || neigbour.Item != Item) continue;
            result.AddRange(neigbour.GetConnectedTiles(exclude));
        }

        return result;
    }
}
