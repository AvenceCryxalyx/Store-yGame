using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StoreyGame.Items;
using StoreyGame.Animations;

public class DebugManager : MonoBehaviour
{
    public AnimationData[] animations;
    public Avatar avatar;

    public void SetRandomOutfit()
    {
        int rand =  Random.Range(0, animations.Length);

        avatar.OverrideAnimations(animations[rand]);
    }
}
