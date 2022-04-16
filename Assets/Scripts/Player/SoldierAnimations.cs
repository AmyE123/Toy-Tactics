using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierAnimations : MonoBehaviour
{
    [SerializeField]
    private Animator _anim;

    const string IS_STATIC = "isStatic";
    const string IS_RUNNING = "isRunning";
    const string IS_JUMPING = "isJumping";
    const string END_CHOICE = "endChoice";

    public enum EndPose { Think, Tpose, PraiseTheSun, Dab };
    private float _runTime;
    private bool _isJumping;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetAsRunning()
    {
        _runTime = 0.1f;
    }

    public void StartJumping()
    {
        _isJumping = true;
    }

    public void StopJumping()
    {
        _isJumping = false;
    }

    public void SetNotStatic()
    {
        _anim.SetBool(IS_STATIC, false);
    }

    public void SetAsStatic()
    {
        _anim.SetBool(IS_STATIC, true);

        int randChoice = Random.Range(0, 4);
        _anim.SetFloat(END_CHOICE, ((float) randChoice) / 3f );
    }

    // Update is called once per frame
    void Update()
    {
        _runTime -= Time.deltaTime;

        _anim.SetBool(IS_RUNNING, _runTime > 0);
        _anim.SetBool(IS_JUMPING, _isJumping);
    }
}
