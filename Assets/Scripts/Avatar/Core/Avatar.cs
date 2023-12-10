using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using StoreyGame.Animations;
using System.Linq;

[RequireComponent(typeof(Animator))]
public class Avatar : MonoBehaviour
{
    [System.Serializable]
    public struct AvatarAnimData
    {
        public Enums.Bodypart Part;
        public AnimationData AnimData;
        public AnimationData PreviousAnimData;

        public void SetData(AnimationData data)
        {
            PreviousAnimData = AnimData;
            AnimData = data;
        }
    }

    public string Id { get; private set; }

    private Animator m_animator;
    private RuntimeAnimatorController m_defaultAnimatorController;

    private AnimationClip m_animationClip;
    private AnimatorOverrideController m_overrideController;

    private AnimationClipOverrides m_defaultOverrides;
    private AnimationClipOverrides m_currentOverrides;

    [SerializeField]
    public AvatarAnimData[] AnimDatas;

    private Dictionary<Enums.Bodypart, AvatarAnimData> m_avatarDatas = new Dictionary<Enums.Bodypart, AvatarAnimData>();

    private void Start()
    {
        foreach(AvatarAnimData data in AnimDatas)
        {
            m_avatarDatas[data.Part] = data;
        }

        m_animator = GetComponent<Animator>();
        m_overrideController = new AnimatorOverrideController(m_animator.runtimeAnimatorController);
        m_animator.runtimeAnimatorController = m_overrideController;

        m_currentOverrides = new AnimationClipOverrides(m_overrideController.overridesCount);
        Debug.LogFormat("override size: {0}", m_currentOverrides.Count);
        m_overrideController.GetOverrides(m_currentOverrides);
        m_animator.runtimeAnimatorController.name = "OverrideController";

        if(AnimDatas.Length != 0)
        {
            UpdateAnimations();
        }
    }

    public void Initialize(AnimationData[] data)
    {
        foreach(AnimationData ad in data)
        {
            AvatarAnimData avaData;
            avaData = m_avatarDatas[ad.Part];
            if (!AnimDatas.Any(x => x.Part == ad.Part))
            {
                m_avatarDatas.Add(ad.Part, avaData);
            }
            avaData.SetData(ad);
        }
        UpdateAnimations();
    }

    public void Move(Vector3 velocity)
    {
        transform.position += velocity;
    }

    public void SetAnimatorController(RuntimeAnimatorController controller)
    {
        m_animator.runtimeAnimatorController = controller;
    }
    void UpdateAnimations()
    {
        foreach (AvatarAnimData aad in m_avatarDatas.Values)
        {
            if(aad.AnimData == null)
            {
                foreach (AnimationData.AnimationClipDatum acd in aad.PreviousAnimData.ClipDatas)
                {
                    string key = string.Format("{0}_0_{1}_{2}", aad.Part.ToString().ToLower(), acd.State.ToString().ToLower(), acd.Direction.ToString().ToLower());
                    Debug.LogFormat("Updated animation key {0} to new clip {1}", key, "");
                    m_currentOverrides[key] = null;
                }
            }
            else
            {
                foreach (AnimationData.AnimationClipDatum acd in aad.AnimData.ClipDatas)
                {
                    string key = string.Format("{0}_0_{1}_{2}", aad.Part.ToString().ToLower(), acd.State.ToString().ToLower(), acd.Direction.ToString().ToLower());
                    Debug.LogFormat("Updated animation key {0} to new clip {1}", key, acd.Clip.name);
                    m_currentOverrides[key] = acd.Clip;
                }
            }

        }

        m_overrideController.ApplyOverrides(m_currentOverrides);
    }
    public void OverrideAnimations(AnimationData data)
    {
        if (!m_avatarDatas.ContainsKey(data.Part))
        {
            return;
        }
        m_avatarDatas[data.Part].SetData(data);
        UpdateAnimations();
    }

    public void SetAnimatorParameter(string id, int value)
    {
        m_animator.SetInteger(id, value);
    }
    public void SetAnimatorParameter(string id, float value)
    {
        m_animator.SetFloat(id, value);
    }
    public void SetAnimatorParameter(string id, bool value)
    {
        m_animator.SetBool(id, value);
    }
}
