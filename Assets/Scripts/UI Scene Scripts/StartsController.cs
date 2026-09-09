using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartsController : MonoBehaviour
{
    public List<Image> stars = new List<Image>();
    private void OnEnable()
    {
        UpdateStars();
    }
    void UpdateStars()
    {
        if (gameObject.activeInHierarchy)
        {
            for (int i = 0; i < 10; i++)
            {
                int randomStar = Random.Range(0, stars.Count);
                StartCoroutine(_UpdateAlpha(randomStar));
            }
            Invoke("UpdateStars", 2);
        }
    }
    private IEnumerator _UpdateAlpha(int randomStar)
    {
        bool enabled = stars[randomStar].color.a >= 1 ? true : false;
        Color color = stars[randomStar].color;
        if (enabled)
        {
            while (stars[randomStar].color.a >= 0)
            {
                yield return null;
                color.a -= 0.01f;
                stars[randomStar].color = color;
            }
        }
        else
        {
            while (stars[randomStar].color.a <= 1)
            {
                yield return null;
                color.a += 0.01f;
                stars[randomStar].color = color;
            }
        }
    }
}