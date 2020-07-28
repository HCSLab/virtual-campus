using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour 
{
	public float speed = 500;
    public float jumpStrength = 1000;
    public Transform parent;
    public LayerMask layerMask;

    private Rigidbody rb;
    private Vector3 movement;
    private bool inputJump;
    public Vector3 forwardVector;
    private bool grounded;

    void Start ()
    {
        inputJump = false;
        movement = Vector3.zero;
        grounded = false;

        rb = GetComponent<Rigidbody>();
        forwardVector = transform.forward;
        parent.transform.rotation = Quaternion.LookRotation(forwardVector);
    }

    void FixedUpdate ()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        movement = parent.transform.rotation * new Vector3(moveHorizontal, 0.0f, moveVertical);
       
        rb.AddForce (movement * speed);       

        if (Physics.Raycast(transform.position, Vector3.down, 0.6f, layerMask))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }

        if (inputJump && grounded)
        {
            rb.AddForce(Vector3.up * jumpStrength);
            inputJump = false;
        }
        else
        {
            inputJump = false;
        }
    }

    void Update()
    {
        parent.transform.position = transform.position;      

       

       /* Vector3 vel = rb.velocity.normalized;

        float dot = Vector3.Dot(movement.normalized, parent.transform.forward);
        if (dot > 0f || dot > -0.7f)
        {
            parent.transform.rotation = Quaternion.LookRotation(vel);
        }*/

        //movement = parent.transform.rotation * new Vector3(moveHorizontal, 0.0f, moveVertical);

        if (Input.GetKeyUp(KeyCode.Space))
        {
            inputJump = true;
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            forwardVector = Quaternion.AngleAxis(-90.0f, Vector3.up) * forwardVector;
            parent.transform.rotation = Quaternion.LookRotation(forwardVector);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            forwardVector = Quaternion.AngleAxis(90.0f, Vector3.up) * forwardVector;
            parent.transform.rotation = Quaternion.LookRotation(forwardVector);
        }

        /* Vector3 vel = rb.velocity.normalized;

        float dot = Vector3.Dot(movement.normalized, parent.transform.forward);
        if (dot > 0f || dot > -0.7f)
        {
            parent.transform.rotation = Quaternion.LookRotation(vel);
        }*/
    }
}
