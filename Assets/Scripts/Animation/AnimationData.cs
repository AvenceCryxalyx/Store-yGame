using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StoreyGame.Animations
{
    public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
    {
        public AnimationClipOverrides(int capacity) : base(capacity) { }

        public AnimationClip this[string name]
        {
            get { return this.Find(x => x.Key.name.Equals(name)).Value; }
            set
            {
                int index = this.FindIndex(x => x.Key.name.Equals(name));
                if (index != -1)
                    this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
            }
        }
    }


    public enum AnimationStates
    {
        Idle,
        Walk,
        Run
    }
    [CreateAssetMenu(fileName = "Anim Data", menuName = "Data/Animations")]
    public class AnimationData : ScriptableObject
    {
        [System.Serializable]
        public struct AnimationClipDatum
        {
            public AnimationStates State;
            public Enums.Directions Direction;
            public AnimationClip Clip;
        }

        public string Id;
        public Enums.Bodypart Part;
        public AnimationClipDatum[] ClipDatas;
    }
}

