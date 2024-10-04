using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowEffect : MonoBehaviour
{
    public static ShadowEffect instance;
    [SerializeField] private GameObject Shadow;
    private List<GameObject> pool = new List<GameObject>();
    private float duration;
    [Range(1f,100f)][SerializeField] private float speed = 1f;
    [SerializeField] private Color _color;

    private void Awake()
    {
        instance = this;
    }

    public GameObject GetShadows()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                pool[i].SetActive(true);
                pool[i].transform.position = transform.position;
                pool[i].transform.rotation = transform.rotation;
                pool[i].transform.localScale = transform.localScale;
                pool[i].GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
                pool[i].GetComponent<Solid>().MyColor = _color;
                pool[i].GetComponent<SpriteRenderer>().flipX = GetComponent<SpriteRenderer>().flipX;
                return pool[i];
            }
        }

        GameObject obj = Instantiate(Shadow, transform.position, transform.rotation) as GameObject;
        obj.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        obj.GetComponent<Solid>().MyColor = _color;
        pool.Add(obj);
        return obj;
    }

    public void ShadowSkill()
    {
        duration += speed * Time.deltaTime;

        if (duration > 1)
        {
            GetShadows();
            duration = 0;
        }
    }
}
