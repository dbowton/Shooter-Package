using UnityEngine;

public abstract class AI : Character
{
    public Animator animator;

    public override void update(float dt)
    {
        base.update(dt);
    }

    public override void Die()
    {
        base.Die();
        GameManager.Instance.ai.Remove(this);
        animator.SetTrigger("death");
        Destroy(this);
    }
}
