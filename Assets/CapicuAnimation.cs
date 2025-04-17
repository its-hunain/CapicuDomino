using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CapicuAnimation : Singleton<CapicuAnimation>
{
    [SerializeField] private Image targetImage;            // Your UI Image
    [SerializeField] private List<Sprite> spriteFrames;    // Assign 120 sprites in order
    [SerializeField] private float frameRate = 30f;        // Frames per second

    private Coroutine playRoutine;


    public void PlayAnimation()
    {
        targetImage.enabled = true;
        if (playRoutine != null) StopCoroutine(playRoutine);
        playRoutine = StartCoroutine(PlaySpriteSequence());
    }

    private IEnumerator PlaySpriteSequence()
    {
        float delay = 1f / frameRate;

        for (int i = 0; i < spriteFrames.Count; i++)
        {
            targetImage.sprite = spriteFrames[i];
            yield return new WaitForSeconds(delay);
        }

        playRoutine = null;
        targetImage.enabled = false;
    }
}
