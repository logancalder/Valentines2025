using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public Animator anim;
    public float moveSpeed;
    private Rigidbody2D rb;
    private float x;
    private float y;
    private Vector2 input;
    private bool moving;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float move = Input.GetAxis("Horizontal");
        if (move != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);
        }
        GetInput();
        Animate();
    }

    private void FixedUpdate()
    {
        rb.velocity = input * moveSpeed;
    }

    private void GetInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        input = new Vector2(x, y);
        input.Normalize();
    }
    private void Animate()
    {

        moving = input.magnitude > 0.1f;

        if (moving)
        {
            anim.SetFloat("X", input.x);
            anim.SetFloat("Y", input.y);
        }
        anim.SetBool("Moving", moving);
    }

}
