using UnityEngine;
using TMPro;

public sealed class ScoreCounter : MonoBehaviour
{
   
    public static ScoreCounter Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private int _score;

    public int Score
    {
        get => _score;
        set
        {
            if (_score == value) return;
            _score = value;
            scoreTxt.SetText($"Score ={_score}");
        }
    }

    [SerializeField] private TextMeshProUGUI scoreTxt;

}
