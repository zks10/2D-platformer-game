using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;
    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update() {
        horizontalInput = Input.GetAxis("Horizontal");
        // Control the facing of the player
        if (horizontalInput > 0.01f) {
            transform.localScale = Vector3.one;
        } else if (horizontalInput < -0.01f) {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // Set animator parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());

        if (wallJumpCooldown > 0.2f) {
            body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

            if (onWall() && !isGrounded()) {
                body.gravityScale = 0.9f; 
                body.linearVelocity = Vector2.zero;
            }else {
                body.gravityScale = 2.2f;
            }
        } else {
            wallJumpCooldown += Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.UpArrow) && isGrounded()) {
            Jump();  
        }
    }

    private void Jump() {
        if (isGrounded()){
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
            anim.SetTrigger("jump");    
        } else if (onWall() && !isGrounded()) {
            if (horizontalInput == 0.9f) {
                body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            } else {
                body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);
            }
            wallJumpCooldown = 0;
            
        }
        
    }

    private bool isGrounded() {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }
    
    private bool onWall() {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack() {
        return horizontalInput == 0 && !onWall();
        // return horizontalInput == 0 && isGrounded() && !onWall();
    }



}
