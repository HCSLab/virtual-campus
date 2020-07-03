//-----------------------------------------------------------------------
// Copyright 2017 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------
using UnityEngine;
#if UNITY_5_5_OR_NEWER
using UnityEngine.PostProcessing;
#endif

/*
 * Dynamic Light Adaptation
 * 
 * Dynamic Light Adaptation component automatically adjusts scene exposure
 * dependeing on how bright/dark the environment you are looking at. 
 * 
 * How to enable Tobii dynamic light adaptation in your game:
 * 1- Add DynamicLightAdaptation component to any object in your scene
 * 2- Drag the prfile "EyeAdaptationProfile.asset" to the profile public member of this component (Post Processing Profile). 
 * 3- Click on the profile file and from Unity inspector window go to "Eye Adaptation" section.
 * 4- Set the eye adaptation configuration values that fit with your scene. 
 */

[DisallowMultipleComponent]
public class DynamicLightAdaptation : MonoBehaviour
{
#if UNITY_5_5_OR_NEWER
    public PostProcessingProfile postProcessingProfile;

    private PostProcessingBehaviour _postProcessingBehavior;
    // Use this for initialization
    void Start () {
        Initialize();   
    }

    void Initialize()
    {
        _postProcessingBehavior = gameObject.AddComponent<PostProcessingBehaviour>();

        if (postProcessingProfile != null)
        {
            _postProcessingBehavior.profile = postProcessingProfile;

            var settings = _postProcessingBehavior.profile.eyeAdaptation.settings;

            _postProcessingBehavior.profile.eyeAdaptation.settings = settings;
            _postProcessingBehavior.profile.eyeAdaptation.enabled = true;
          
        }
        else
        {
            Debug.LogError("You must drag a profile to the post processing profile public member of DynamicLightAdaptation component");
        }
    }

    void OnEnable()
    {
        if (_postProcessingBehavior != null)
        {
            _postProcessingBehavior.enabled = true;
        }
    }

    void OnDisable()
    {
        if (_postProcessingBehavior != null)
        {
            _postProcessingBehavior.enabled = false;
        }
    }
#else
    void Start()
    {
        Debug.LogError("Dynamic light adaptation does not work with Unity versions before 5.5");
    }
#endif
}
