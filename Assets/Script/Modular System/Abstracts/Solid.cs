using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solid : MonoBehaviour
{
    [SerializeField][Range(0.1f, 2f)] private float fadeDuration;
    private float duration;

    private SpriteRenderer myRenderer;
    private Shader myMaterial;
    private Color myColor;
    public Color MyColor { get { return myColor; } set { myColor = value; } }

    private float initialAlpha;

    void OnEnable()
    {
        initialAlpha = 1f;
        duration = 0;
    }

    void Start()
    {
        myRenderer = GetComponent<SpriteRenderer>();
        myMaterial = Shader.Find("GUI/Text Shader");
    }

    void ColorSprite()
    {
        myRenderer.material.shader = myMaterial;
        float alphaDecrement = initialAlpha * (duration / fadeDuration);
        myColor.a = Mathf.Clamp01(initialAlpha - alphaDecrement);

        myRenderer.color = myColor;
    }

    public void Finish()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        duration += Time.deltaTime;



        if (duration < fadeDuration)
        {
            ColorSprite();
        }
        else
        {
            Finish();
        }
    }

    private void Reset()
    {
        if (GetComponent<SpriteRenderer>() == null)
            gameObject.AddComponent<SpriteRenderer>();
    }
}