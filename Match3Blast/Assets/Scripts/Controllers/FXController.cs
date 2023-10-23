using UnityEngine;
using DG.Tweening;
public class FXController : SingletonMonoBehaviour<FXController>
{

    [SerializeField] private GameObject bombFxPrefab;
    
    
    public void CreateBombFX(Vector3 position)
    {
        var bombfx = Instantiate(bombFxPrefab, transform);
        bombfx.transform.position = position;
        Destroy(bombfx, 1f);
    }

   
}
