using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleUI : MonoBehaviour
{
    public bool isUIActive;
    public float lerpSpeed;
    public float inactiveSize;

    RectTransform _rect;
    CanvasGroup _grp;

    // Start is called before the first frame update
    void Start()
    {
        _rect = GetComponent<RectTransform>();
        _grp = GetComponent<CanvasGroup>(); 

        _grp.alpha = 0;
        _rect.localScale = Vector3.one * inactiveSize;
    }

    // Update is called once per frame
    void Update()
    {
        float size = isUIActive ? 1 : inactiveSize;
        float alpha = isUIActive ? 1 : 0;

        _rect.localScale = Vector3.Lerp(_rect.localScale, Vector3.one * size, Time.deltaTime * lerpSpeed);
        _grp.alpha = Mathf.Lerp(_grp.alpha, alpha, Time.deltaTime * lerpSpeed);

    }
}
