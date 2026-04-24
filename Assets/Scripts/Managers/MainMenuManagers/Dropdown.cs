using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropdown : MonoBehaviour
{
    public RectTransform content;
    public bool isOpen;
    void Start()
    {
        isOpen = false;
        content.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isOpen)
        {
            content.gameObject.SetActive(true);
            Vector3 scale = content.localScale;
            scale.y = Mathf.Lerp(scale.y, isOpen ? 1 : 0, Time.deltaTime * 12);
            content.localScale = scale;
        }
        else
        {
            content.gameObject.SetActive(false);
            Vector3 scale = content.localScale;
            scale.y = Mathf.Lerp(scale.y, isOpen ? 1 : 0, Time.deltaTime * 12);
            content.localScale = scale;
        }
    }

    public void SetBool()
    {
        isOpen = !isOpen;
        /*if (content.gameObject.activeSelf == true)
        {
            content.gameObject.SetActive(false);
        }
        else
        {
            content.gameObject.SetActive(true);
        }*/
    }
}
