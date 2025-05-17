using UnityEngine;

public class MagnetPlayer : MonoBehaviour
{
    public enum Polarity { Positive, Negative }
    public Polarity playerPolarity = Polarity.Positive;

    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float magnetForce = 15f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Movement
        float move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector3(move * moveSpeed, rb.velocity.y, 0);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Switch polarity
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            playerPolarity = (playerPolarity == Polarity.Positive) ? Polarity.Negative : Polarity.Positive;
            Debug.Log("Switched polarity to " + playerPolarity);
        }
    }

    void FixedUpdate()
    {
        // Check all magnetic objects and apply force
        GameObject[] magnets = GameObject.FindGameObjectsWithTag("Magnet");

        foreach (GameObject magnet in magnets)
        {
            Vector3 direction = magnet.transform.position - transform.position;
            float distance = direction.magnitude;
            if (distance < 10f) // Only affect nearby
            {
                Polarity magnetPolarity = magnet.GetComponent<MagnetSurface>().surfacePolarity;

                // Determine attraction or repulsion
                float force = (magnetPolarity == playerPolarity) ? -magnetForce : magnetForce;

                Vector3 forceDir = direction.normalized * force / distance;
                rb.AddForce(forceDir, ForceMode.Force);
            }
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
