using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using StoreyGame.Animations;

[RequireComponent(typeof(Animator), typeof(CharacterController))]
public class Avatar : MonoBehaviour
{
    public string Id { get; private set; }
    private CharacterController m_controller;

    private Animator m_animator;
    private RuntimeAnimatorController m_defaultAnimatorController;

    private AnimationClip m_animationClip;
    private AnimatorOverrideController m_overrideController;

    private AnimationClipOverrides m_defaultOverrides;
    private AnimationClipOverrides m_currentOverrides;

    private AnimationData m_animData;

    [SerializeField]
    private SpriteRenderer renderers;
    public Vector3 Velocity { get { return m_controller.velocity; } }

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_overrideController = new AnimatorOverrideController(m_animator.runtimeAnimatorController);
        m_animator.runtimeAnimatorController = m_overrideController;
        m_currentOverrides = new AnimationClipOverrides(m_overrideController.overridesCount);
        m_overrideController.GetOverrides(m_currentOverrides);
    }
        public void Move(Vector3 velocity)
    {
        m_controller.Move(velocity);
    }

    public void SetAnimatorController(RuntimeAnimatorController controller)
    {
        m_animator.runtimeAnimatorController = controller;
    }

    public void OverrideAnimations(AnimationData data)
    {
        if(data == null)
        {
            foreach (AnimationData.AnimationClipDatum acd in m_animData.ClipDatas)
            {
                string key = string.Format("{0}_0_{1}_{2}", data.Part.ToString().ToLower(), acd.State.ToString().ToLower(), acd.Direction.ToString().ToLower());
                m_currentOverrides[key] = null;
            }
            m_overrideController.ApplyOverrides(m_currentOverrides);
        }
        else
        {
            m_animData = data;
            foreach (AnimationData.AnimationClipDatum acd in data.ClipDatas)
            {
                string key = string.Format("{0}_0_{1}_{2}", data.Part.ToString().ToLower(), acd.State.ToString().ToLower(), acd.Direction.ToString().ToLower());
                m_currentOverrides[key] = acd.Clip;
            }
            m_overrideController.ApplyOverrides(m_currentOverrides);
        }

    }
}
