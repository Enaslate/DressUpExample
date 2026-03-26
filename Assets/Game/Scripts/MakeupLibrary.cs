using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MakeupLibrary", menuName = "Data/Makeup/Library")]
public class MakeupLibrary : ScriptableObject
{
    public List<MakeupItemData> allItems;
}