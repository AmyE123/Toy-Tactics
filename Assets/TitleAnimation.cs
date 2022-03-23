using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleAnimation : MonoBehaviour
{
    public Animator _anim;

    // Start is called before the first frame update
    void Start()
    {
        _anim.SetBool("isStatic", true);

        int randChoice = Random.Range(0, 4);
        _anim.SetFloat("endChoice", ((float) randChoice) / 3f );

    }
}
