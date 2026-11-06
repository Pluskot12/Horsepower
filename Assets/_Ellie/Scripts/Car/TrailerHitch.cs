using UnityEngine;

/// <summary>
/// Attach to the TRAILER. Hinges the trailer to the truck at a hitch point.
/// Truck needs a child "HitchPoint" (rear), trailer needs a child "TongueAnchor" (front).
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class TrailerHitch : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D truckBody;
    public Transform truckHitchPoint;
    public Transform trailerTongueAnchor;

    [Header("Limits")]
    public bool useLimits = true;
    [Range(0f, 180f)] public float maxAngle = 55f;

    [Header("Feel")]
    public float linearDrag = 0.05f;
    public float angularDrag = 2f;      // main source of hitch damping; raise if it still sways

    [Header("Optional")]
    public bool breakable = false;
    public float breakForce = 20000f;

    public HingeJoint2D Hinge { get; private set; }
    Rigidbody2D rb;

    void Start()   // Start, so the truck's own setup has finished
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = linearDrag;
        rb.angularDamping = angularDrag;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        if (truckBody.mass > 0f && rb.mass > truckBody.mass * 1.5f)
            Debug.LogWarning("Trailer is much heavier than the truck; expect instability. Try mass <= truck mass.");

        // 1) Make the starting pose valid: same rotation, tongue exactly on the hitch.
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.rotation = truckBody.rotation;
        transform.rotation = truckBody.transform.rotation;
        Vector3 delta = truckHitchPoint.position - trailerTongueAnchor.position;
        transform.position += delta;
        rb.position = transform.position;

        // 2) Ignore truck <-> trailer collisions.
        foreach (var a in truckBody.GetComponentsInChildren<Collider2D>())
            foreach (var b in GetComponentsInChildren<Collider2D>())
                Physics2D.IgnoreCollision(a, b, true);

        // 3) Joint.
        Hinge = gameObject.AddComponent<HingeJoint2D>();
        Hinge.connectedBody = truckBody;
        Hinge.autoConfigureConnectedAnchor = false;
        Hinge.anchor = transform.InverseTransformPoint(trailerTongueAnchor.position);
        Hinge.connectedAnchor = truckBody.transform.InverseTransformPoint(truckHitchPoint.position);
        Hinge.enableCollision = false;
        Hinge.useMotor = false;

        if (useLimits)
        {
            Hinge.useLimits = true;
            Hinge.limits = new JointAngleLimits2D { min = -maxAngle, max = maxAngle };
        }
        if (breakable) Hinge.breakForce = breakForce;
    }

    void OnJointBreak2D(Joint2D j) { Debug.Log("Trailer detached"); }

    void OnDrawGizmosSelected()
    {
        if (truckHitchPoint) { Gizmos.color = Color.green; Gizmos.DrawWireSphere(truckHitchPoint.position, 0.1f); }
        if (trailerTongueAnchor) { Gizmos.color = Color.red; Gizmos.DrawWireSphere(trailerTongueAnchor.position, 0.15f); }
    }
}