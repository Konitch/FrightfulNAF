using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackRabbit : MonoBehaviour
{
    public int HP = 100;
    public Animator animator;
    // int once=1;

    public IEnumerator PlayBiteAnimation(Animator animator)
    {
        animator.SetBool("IsBiting", true);
        GameManager.Instance.TriggerJumpscare();
        yield return new WaitForSeconds(2.5f); // Espera a animação rodar
        animator.SetBool("IsBiting", false);
    }

    // void PlayDeathAnimation(Animator animator)
    // {
    //     // Reseta todas as bools do Animator
    //     foreach (AnimatorControllerParameter param in animator.parameters)
    //     {
    //         if (param.type == AnimatorControllerParameterType.Bool)
    //         {
    //             animator.SetBool(param.name, false);
    //         }
    //     }

    //     // Ativa o trigger de morte
    //     animator.SetTrigger("Die");
    // }
    
    public void TakeDamage(int damageAmount)
    {
        if(HP>0)
        {
            HP -= damageAmount;

            if(HP<=0)
            {
                // Play death animation
                animator.SetTrigger("die");

            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // print(GameManager.Instance.timeElapsed);
        // if (GameManager.Instance != null && GameManager.Instance.timeElapsed >= 50 && GameManager.Instance.currentHour == 12 && once == 1)
        // {
        //     PlayDeathAnimation(animator);
        //     once++;
        // }
    }
}
