using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;
    public float maxSpeed;
    public float attSpeed;
    public Animator animator;
    public Transform attackPoint;
    private float vMove;
    private float hMove;
    private float lastAtt = 0;
    private int AttackNum = 0;
    private int lastAttackNum = 0;
    private bool isGuarding = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        vMove = Input.GetAxisRaw("Vertical");
        hMove = Input.GetAxisRaw("Horizontal");
        if (Input.GetButton("Fire1") && (Time.time - lastAtt >= attSpeed))
        {
            MediumAttack();
            lastAtt = Time.time;
        }
        if (Input.GetButton("Fire2"))
        {
            isGuarding = true;
        } else
        {
            isGuarding = false;
        }
    }

    void FixedUpdate()
    {
        if (isGuarding)
        {
            animator.SetBool("Guarding", true);
        } else
        {
            animator.SetBool("Guarding", false);
        }
        animator.SetFloat("Velocity", rb.velocity.x);
        rb.AddForce(new Vector2((hMove * maxSpeed - rb.velocity.x) * speed, (vMove * maxSpeed - rb.velocity.y) * speed));
    }

    void LightAttack()
    {
        rb.AddForce(new Vector2(50,0));
        Collider2D[] hits = Physics2D.OverlapBoxAll(attackPoint.position, new Vector2(0.5f,0.5f), 0);
        foreach(Collider2D target in hits)
        {
            if (target.gameObject != gameObject)
            {
                target.GetComponent<Damage>().takeDamage(3);
            }
        }
        while (lastAttackNum == AttackNum)
        {
            AttackNum = Random.Range(1, 5);
            Debug.Log(AttackNum);
        }
        lastAttackNum = AttackNum;
        animator.SetInteger("AttackNum", AttackNum);
        animator.SetTrigger("Attack");
    }

    void MediumAttack()
    {
        rb.AddForce(new Vector2(100, 0));
        Collider2D[] hits = Physics2D.OverlapBoxAll(attackPoint.position, new Vector2(0.5f, 0.5f), 0);
        foreach(Collider2D target in hits)
        {
            if (target.gameObject != gameObject)
            {
                target.GetComponent <Damage>().takeDamage(6);
                target.GetComponent<Rigidbody2D>().AddForce(new Vector2(50, 0));
                //Penser à ajouter le stun quand possible
            }
        }
        while (lastAttackNum == AttackNum)
        {
            AttackNum = Random.Range(1, 5);
            Debug.Log(AttackNum);
        }
        lastAttackNum = AttackNum;
        animator.SetInteger("AttackNum", AttackNum);
        animator.SetTrigger("Attack");
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(attackPoint.position,new Vector3(0.5f,0.5f,0));
    }
}
