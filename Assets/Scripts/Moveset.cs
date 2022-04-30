using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moveset : MonoBehaviour
{
    //Public variables
    public float dodgeDist;
    public AnimationCurve dodgeMove;

    //Private variables

    /*atkData contains a 2 dimensions table containing the data of the fighters move. 
     atkData[i] will be the table containing the data for the move n°i
     atkData[i][j] will be the respective data of the attack; Pattern below*/
    private int[][] atkData =
        {   //0 Startup, 1 Active, 2 Recovery, 3 Damage, 4 HitStun, 5 BlockStun, 6 Move, 7 Knockback, 8 xPos, 9 yPos, 10 xScale, 11 yScale
            new int[] {6, 3, 14 , 40, 20, 11, 0, 1 /*Can I put these elsewhere?*/, 1, 1, 1, 1 }, //WeakAttack 1
            new int[] {11, 3, 20 , 70, 20, 11, 0, 1 /*Can I put these elsewhere?*/, 1, 1, 1, 1 }, //WeakAttack 2
            new int[] {13, 6, 21 , 100, 20, 11, 10, 1 /*Can I put these elsewhere?*/, 1, 1, 1, 1 }, //WeakAttack 3
        };

    private int atkNumber;
    private int frameCount; //This is a flexible variable used for the various scripts found in the moveset
    private int comboFrames; //The number of frames we have until the game considers we dropped the combo if one happening

    private float chargeVal = 0;

    private bool dodging;
    private bool attacking;
    private bool charging;
    private bool isHit;

    //Object components and scripts
    private PlayerController controller;
    private Rigidbody2D body;
    private SpriteRenderer render;

    // Start is called before the first frame update
    void Start()
    {
        atkNumber = 0;
        controller = this.GetComponentInParent<PlayerController>();
        body = this.GetComponentInParent<Rigidbody2D>();
        render = this.GetComponentInParent<SpriteRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isHit)
        {
            Hit(0, 0, frameCount, frameCount, new Vector2(-1, 0));
        }
        if (dodging)
        {
            Dodge();
        }

        if (attacking)
        {
            WeakAttack();
        }

        if (comboFrames > 0)
        {
            comboFrames -= 1;
        }
        else atkNumber = 0;
    }

    public void Dodge()
    {
        /*For now, dodging just make you hop backwards
        Later, it should allow the player to:
        -Hop in any direction
        -Become invulnerable for a short period of time after the start of the hop*/
        if (frameCount == 0)
        {
            controller.stun = true;
            dodging = true;
        }


        if (0 < frameCount && frameCount < 50)
        {
            body.velocity = new Vector2(-1,0) * dodgeDist * dodgeMove.Evaluate((float)frameCount / 50);
            render.color = Color.blue;
        }
        
        if (50 <= frameCount)
        {
            render.color = Color.magenta;
        }

        if (frameCount == 60)
        {
            controller.stun = false;
            dodging = false;
            frameCount = 0;
            render.color = Color.white;
        } 
        else frameCount += 1;
    }
    public void WeakAttack()
    {
        /*The basic attack. Moves you a little, makes very little damage but in exchange has very favorable frame data.
         *I plan on adding a combo system thanks to the frameData table later*/
        if (frameCount == 0)
        {
            controller.stun = true;
            attacking = true;
        }

        //Startup
        if (0 < frameCount && frameCount <= atkData[atkNumber][0])
        {
            render.color = Color.yellow;
        }
        //Active
        if (atkData[atkNumber][0] < frameCount && frameCount <= atkData[atkNumber][0] + atkData[atkNumber][1])
        {
            render.color = Color.red;
            body.velocity = new Vector2(atkData[atkNumber][6], 0);
        }
        //Recovery
        if (atkData[atkNumber][0] + atkData[atkNumber][1] < frameCount && frameCount <= atkData[atkNumber][0] + atkData[atkNumber][1] + atkData[atkNumber][2])
        {
            render.color = Color.gray;
            body.velocity = new Vector2();
        }
        //End
        if (frameCount > atkData[atkNumber][0] + atkData[atkNumber][1] + atkData[atkNumber][2])
        {
            controller.stun = false;
            attacking = false;
            frameCount = 0;
            comboFrames = 60;
            render.color = Color.white;
            if (atkNumber == atkData.Length-1)
                atkNumber = 0;
            else atkNumber += 1;
        }
        else frameCount += 1;
    }
    public void StrongAttack()
    {
        controller.stun = true;
        //Charge or Startup
        if (frameCount == 0 && charging)
        {
            
        } else if (frameCount == 0)
        {

        }
        //Active
        if (0 < frameCount && frameCount <= 4)
        {
            render.color = Color.red;
            body.velocity = new Vector2(atkData[atkNumber][6], 0);
        }
        //Recovery
        if (4 < frameCount && frameCount <=34)
        {
            render.color = Color.gray;
            body.velocity = new Vector2();
        }
        //End

    }

    public void Hit(int damage, int knockback, int hitStun, int blockStun, Vector2 knockbackVec)
    //Fonction called when the entity is hit by an attack
    {
        if (hitStun != 0)
        {
            controller.stun = true;
            isHit = true;
            frameCount = hitStun - 1;
            render.color = Color.magenta;
            Debug.Log("Ouch! " + damage);
            body.velocity = knockbackVec * knockback;

        } else
        {
            isHit = false;
            controller.stun = false;
        }
    }
    public void Erase()
    {

    }
}
