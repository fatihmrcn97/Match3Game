using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName ="Match-3/Item")]
public sealed class Item : ScriptableObject
{
    public int value;

    public Sprite sprite;

    public bool isBonus;

    public List<Sprite> explosionSprites;

    public int explosionForce = 50;
    public void ExlosionEffect(GameObject explosionPref, Transform tileTransform)
    {
        foreach (var item in explosionSprites)
        {
            var randomNumber = Random.Range(-60, 60);
            var explosionPiece = Instantiate(explosionPref, tileTransform.position+new Vector3(randomNumber, 0), Quaternion.identity, tileTransform.parent.parent.parent);
            explosionPiece.GetComponent<Image>().sprite = item;
            explosionPiece.GetComponent<Rigidbody2D>().AddForce(new Vector2(randomNumber, 20) * explosionForce); 
            Destroy(explosionPiece, 5f);
        }
    }
}
