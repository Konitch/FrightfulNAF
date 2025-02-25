using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackRabbit : MonoBehaviour
{
    public int HP = 100;
    public Animator animator;

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
    }
}
