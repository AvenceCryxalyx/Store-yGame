using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StoreyGame.Animations;
using StoreyGame.Items;

[CreateAssetMenu(fileName = "Clothes Data", menuName = "Data/Clothes")]
public class ClothesData : ItemData
{
    public Enums.Bodypart Part;
    public AnimationData animData;
}
