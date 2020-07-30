using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateInkTalkOnStart : CreateInkTalk
{
    [SerializeField] private StoryScript _storyScript;
    [SerializeField] private NPCInfo _speaker;

    void Start()
    {
        if (_storyScript) 
            storyScript = _storyScript;
        if (_speaker) 
            speaker = _speaker;

        Create();
    }
}
