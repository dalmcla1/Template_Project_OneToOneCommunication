using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimHelper : MonoBehaviour {

    public List<Animator> modelAnimators;
    public Text animName;
    public int currentClipNumber;

    private AnimationClip[] _animationClips;

    void Start () {

        // Get a list of the animation clips
        _animationClips = modelAnimators[0].runtimeAnimatorController.animationClips;

        // Iterate over the clips and gather their information
        foreach (AnimationClip animClip in _animationClips)
        {
            Debug.Log(animClip.name + ": " + animClip.length);
        }

        currentClipNumber = 0;
        PlayAnimationClip(currentClipNumber);
    }

	void Update () {
		
	}

    public void OnNextAnimButtonPressed()
    {
        currentClipNumber++;
        if(currentClipNumber > _animationClips.Length - 1)
        {
            currentClipNumber = 0;
        }
        PlayAnimationClip(currentClipNumber);
    }

    public void OnPrevAnimButtonPressed()
    {
        currentClipNumber--;
        if (currentClipNumber < 0)
        {
            currentClipNumber = _animationClips.Length - 1;
        }
        PlayAnimationClip(currentClipNumber);
    }

    private void PlayAnimationClip(int __clipNumber)
    {
        foreach (Animator __modelAnimator in modelAnimators)
        {
            __modelAnimator.Play(_animationClips[__clipNumber].name);
            animName.text = _animationClips[__clipNumber].name;
        }
       
    }

}
