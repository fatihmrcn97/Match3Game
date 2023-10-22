using UnityEngine;

public static class ItemDatabase
{ 
    public static Item[] Items { get; private set; }

    public static Item[] BonusItem { get; private set; }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] private static void Initialize() => Items = Resources.LoadAll<Item>("Items/");
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] private static void InitializeBonus() => BonusItem = Resources.LoadAll<Item>("Bonus/");
}
