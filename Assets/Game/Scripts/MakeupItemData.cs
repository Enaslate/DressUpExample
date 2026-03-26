using UnityEngine;

[CreateAssetMenu(fileName = "MakeupItem", menuName = "Data/Makeup/Item")]
public class MakeupItemData : ScriptableObject
{
    [field: SerializeField] public MakeupType Type { get; private set; }
    [field: SerializeField] public Sprite ItemSprite { get; private set; }
    [field: SerializeField] public Sprite ResultSprite { get; private set; }
}