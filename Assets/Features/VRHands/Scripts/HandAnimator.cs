using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

[RequireComponent(typeof(Animator))]
public class HandAnimator : MonoBehaviour
{
    /// <summary>
    /// Parameters for realtime animation
    /// </summary>
    [SerializeField] private XRInputValueReader<Vector2> m_StickInput = new XRInputValueReader<Vector2>("Thumbstick");

    [SerializeField] private XRInputValueReader<float> m_TriggerInput = new XRInputValueReader<float>("Trigger");
    [SerializeField] private XRInputValueReader<float> m_GripInput = new XRInputValueReader<float>("Grip");

    [SerializeField] private XRPokeInteractor xRPokeInteractor;

    private bool isUIAnimationPlaying = false;
    private Animator handAnimator = null;

    /// <summary>
    /// List of fingers animated when grabbing / using grab action
    /// </summary>
    private readonly List<Finger> grippingFingers = new List<Finger>()
    {
        new Finger(FingerType.Middle),
        new Finger(FingerType.Ring),
        new Finger(FingerType.Pinky)
    };

    /// <summary>
    /// List of fingers animated when pointing / using trigger action
    /// </summary>
    private readonly List<Finger> pointingFingers = new List<Finger>()
    {
        new Finger(FingerType.Index)
    };

    /// <summary>
    /// List of fingers animated when locomtion / using thumbstick
    /// </summary>
    private readonly List<Finger> thumbFinger = new List<Finger>()
    {
        new Finger(FingerType.Thumb)
    };

    /// <summary>
    /// List of fingers animated while UI Interaction
    /// </summary>
    private readonly List<Finger> uiFingers = new List<Finger>()
    {
        new Finger(FingerType.Thumb),
        new Finger(FingerType.Middle),
        new Finger(FingerType.Ring),
        new Finger(FingerType.Pinky)
    };

    /// <summary>
    /// Add your own hand animation here. For example a fist.
    /// </summary>
    //private readonly List<Finger> fistFingers = new List<Finger>()
    //{
    //    new Finger(FingerType.Thumb),
    //    new Finger(FingerType.Index),
    //    new Finger(FingerType.Middle),
    //    new Finger(FingerType.Ring),
    //    new Finger(FingerType.Pinky)
    //};

    private void OnEnable()
    {
        xRPokeInteractor.uiHoverEntered.AddListener(ActivateUIHandPose);
        xRPokeInteractor.uiHoverExited.AddListener(DeactivateUIHandPose);
    }

    private void Start()
    {
        this.handAnimator = GetComponent<Animator>();
    }

    private void ActivateUIHandPose(UIHoverEventArgs arg0)
    {
        isUIAnimationPlaying = true;
        SetFingerAnimationValues(uiFingers, 1);
        AnimateActionInput(uiFingers);
    }

    private void DeactivateUIHandPose(UIHoverEventArgs arg0)
    {
        isUIAnimationPlaying = false;
        SetFingerAnimationValues(uiFingers, 0);
        AnimateActionInput(uiFingers);
    }

    private void Update()
    {
        if (isUIAnimationPlaying)
            return;
        if (m_StickInput != null)
        {
            var stickVal = m_StickInput.ReadValue();
            SetFingerAnimationValues(thumbFinger, stickVal.y);
            AnimateActionInput(thumbFinger);
        }

        if (m_TriggerInput != null)
        {
            var triggerVal = m_TriggerInput.ReadValue();
            SetFingerAnimationValues(pointingFingers, triggerVal);
            AnimateActionInput(pointingFingers);
        }

        if (m_GripInput != null)
        {
            var gripVal = m_GripInput.ReadValue();
            SetFingerAnimationValues(grippingFingers, gripVal);
            AnimateActionInput(grippingFingers);
        }
    }

    public void SetFingerAnimationValues(List<Finger> fingersToAnimate, float targetValue)
    {
        foreach (Finger finger in fingersToAnimate)
        {
            finger.target = targetValue;
        }
    }

    public void AnimateActionInput(List<Finger> fingersToAnimate)
    {
        foreach (Finger finger in fingersToAnimate)
        {
            var fingerName = finger.type.ToString();
            var animationBlendValue = finger.target;
            handAnimator.SetFloat(fingerName, animationBlendValue);
        }
    }
}
