using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public variables
    public float speed;
    public float moveEnd;
    public AnimationCurve accelCurve;
    public Transform groundCheckPos;
    public bool stun;

    // Private variables
    private int frames;
    private float time;
    private float moveX;
    private float moveY;
    private float moveDuration;
    private float groundBoostTimer = 0;
    private Collider2D[] groundCheck;

    private bool jumpBtn = false;
    private bool flying;
    private bool grounded;
    
    // Object components and scripts
    private Rigidbody2D body;
    private Collider2D collider;
    private Moveset moveset;

    // Start is called before the first frame update
    void Start()
    {
        //Note to self: Gonna have to move that line somewhere else
        Application.targetFrameRate = 70;
        stun = false;
        body = this.GetComponentInParent<Rigidbody2D>();
        collider = this.GetComponentInParent<Collider2D>();
        moveset = this.GetComponentInParent<Moveset>();
        flying = true;
        time = Time.time;
        frames += 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (!stun)
        {
            //Get and manage the inputs
            moveX = Input.GetAxisRaw("Horizontal");
            moveY = Input.GetAxisRaw("Vertical");
            
            if (Input.GetButtonDown("Jump"))
                jumpBtn = true;

            if (Input.GetButtonDown("Fire1"))
                moveset.WeakAttack();
            
            if (Input.GetButtonDown("Fire2"))
                moveset.Hit(50, 10, 60, 60, new Vector2(-1,0));

            //In fighting games, most attacks aren't actually measured by their time in seconds but by "frame data"
            //This section of the code, mostly affiliated with the fighting mechanics, are put out of FixedUpdate for this reason
        }
        
    }

    // For physics things
    private void FixedUpdate()
    {

        //General Movement script

        //Classic test to see if you're grounded
        groundCheck = Physics2D.OverlapBoxAll(groundCheckPos.position, new Vector2(collider.transform.localScale.x, 0.2f), 0);
        grounded = false;
        for (int i = 0; i < groundCheck.Length; ++i)
        {
            if (groundCheck[i].gameObject.layer == 3)
            {
                grounded = true;
                groundBoostTimer = 1;
            }
        }

        if (groundBoostTimer > 0)
        {
            groundBoostTimer -= Time.fixedDeltaTime;
        }

        //On the ground or not modifiers
        if (grounded)
        {
            if (jumpBtn)
            {
                body.AddForce(new Vector2(0, 500));
                jumpBtn = false;
            }

        } else 
        {
            if (jumpBtn)
            {
                flying = !flying;
                jumpBtn = false;
            }
        }

        if (!stun)
        {
            //Moving around
            if (flying)
            {
                //Flying script
                //During flight, the player can go any direction they want and aren't affected by gravity
                body.gravityScale = 0;
                if ((moveX != 0 || moveY != 0) && !stun)
                {
                    moveDuration += Time.fixedDeltaTime;
                    body.velocity = new Vector2(moveX, moveY).normalized * (speed + sumBoost()) * accelCurve.Evaluate(moveDuration / moveEnd);
                } else {
                    moveDuration = 0;
                    body.velocity = new Vector2(body.velocity.x, body.velocity.y) * 0.8f;
                }
            } else 
            {
                //Grounded or falling script
                //When not flying, the player cannot go up by standard means. He also starts being affected by gravity again.
                //Note: I'm not sure if I should keep Unity's classic gravity or modify it a little myself
                body.gravityScale = 1;
                if (moveX != 0)
                {
                    moveDuration += Time.fixedDeltaTime;
                    body.velocity = new Vector2(moveX * (speed + sumBoost()) * accelCurve.Evaluate(moveDuration / moveEnd), body.velocity.y);
                } else {
                    moveDuration = 0;
                    body.velocity = new Vector2(body.velocity.x * 0.8f, body.velocity.y);
                }
            }
        }
    }

    private float sumBoost()
    {
        return (groundBoostTimer>0?10:0);
    }
}
