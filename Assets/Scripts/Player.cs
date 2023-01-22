using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //animations
    private Animator animation;

    private enum MovementState
    {
        running, jumping, falling
    }

    //physics
    public float gravity = -300;
    public Vector2 velocity;
    public float maxVelocity = 100;
    public float acceleration = 10;
    public float maxAcceleration = 10;
    public float jumpVelocity = 30;
    public float groundHeight = .5f;
    public bool isGrounded = false;

    public bool isHoldingJump = false;
    public float maxHoldTime = 0.25f;
    public float holdJumpTimer = 0.0f;

    public float jumpGroundThreshold = 3;
    private float groundDistance;

    // Start is called before the first frame update
    void Start()
    {
        animation = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = transform.position;
        groundDistance = Mathf.Abs(pos.y - groundHeight);

        if (isGrounded || groundDistance <= jumpGroundThreshold)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isGrounded = false;
                velocity.y = jumpVelocity;
                isHoldingJump = true;
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isHoldingJump = false;
        }
    }

    private void FixedUpdate()
    {
        MovementState state = 0;
        Vector2 pos = transform.position;

        if (!isGrounded)
        {
            if (isHoldingJump)
            {
                holdJumpTimer += Time.fixedDeltaTime;
                state = MovementState.jumping;

                if (holdJumpTimer >= maxHoldTime)
                {
                    isHoldingJump = false;
                    state = MovementState.falling;
                }
            }



            pos.y += velocity.y * Time.fixedDeltaTime;
            if (!isHoldingJump)
            {    
                velocity.y += gravity * Time.fixedDeltaTime;
                state = MovementState.falling;            
            }

            if (pos.y <= groundHeight)
            {
                pos.y = groundHeight;
                isGrounded = true;
                velocity.y = 0;
                holdJumpTimer = 0f;
            }
        }
        else
        {
            state = MovementState.running;

            float velocityRatio = velocity.x / maxVelocity;
            if (acceleration > 1)
            {
                acceleration = maxAcceleration * (1 - velocityRatio);
            }
            else
            {
                acceleration = 1;
            }
            
            velocity.x += acceleration * Time.fixedDeltaTime;
            if (velocity.x >= maxVelocity)
            {
                velocity.x = maxVelocity;
            }
        }

        transform.position = pos;

        animation.SetInteger("state", (int)state);
    }
}
