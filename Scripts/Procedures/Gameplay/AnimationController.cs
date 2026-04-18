using UnityEngine;

public enum CharacterAnimationState
{
    Idle,
    Run,
    Attack,
    Jump,
    Hit,
    Death
}

public class AnimationController : MonoBehaviour
{
    public Animator animator;
    public CharacterAnimationState state;

    public const string ANIMSTATE = "AnimState";
    public const string ATTACK = "Attack";
    public const string JUMP = "Jump";
    public const string HURT = "Hurt";
    public const string DEATH = "Death";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger(ANIMSTATE, 1);
    }

    public void SwitchAnimationState(CharacterAnimationState state,bool force = false)
    {
        if(this.state == state && !force) return;

        this.state = state;

        switch (this.state)
        {
            case CharacterAnimationState.Idle:
                animator.SetInteger(ANIMSTATE, 1);
                break;
            case CharacterAnimationState.Run:
                animator.SetInteger(ANIMSTATE, 2);
                break;
            case CharacterAnimationState.Attack:
                animator.SetTrigger(ATTACK);
                break;
            case CharacterAnimationState.Jump:
                animator.SetTrigger(JUMP);
                break;
            case CharacterAnimationState.Hit:
                animator.SetTrigger(HURT);
                break;
            case CharacterAnimationState.Death:
                animator.SetTrigger(DEATH);
                break;
            default:
                break;
        }
    }
}