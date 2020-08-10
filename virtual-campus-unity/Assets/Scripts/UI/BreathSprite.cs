using UnityEngine;
using System.Collections;

public class BreathSprite : MonoBehaviour
{

    public SpriteRenderer sr;
    public float speed;
    public float maxAlpha;
    public float minAlpha;

    bool increase = true;


    void Update()
    {
        if (increase)
        {
            Color c = sr.color;
            c.a += speed;
            if (c.a > maxAlpha)
            {
                c.a = maxAlpha;
                increase = false;
            }
            sr.color = c;
        }
        else
        {
            Color c = sr.color;
            c.a -= speed;
            if (c.a < minAlpha)
            {
                c.a = minAlpha;
                increase = true;
            }
            sr.color = c;
        }
    }
}
