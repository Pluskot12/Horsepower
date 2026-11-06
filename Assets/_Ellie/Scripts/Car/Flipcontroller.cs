using System.Collections.Generic;
using UnityEngine;

public class FlipController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Rigidbody2D carBody;
    [SerializeField] private Transform carHitch;

    [Header("Settings")]
    [SerializeField] private float flipOffset = 0f;
    [SerializeField] private float hitchMaxAngle = 55f;
    [SerializeField] private float maxTilt = 60f;
    [SerializeField] private float embedTolerance = 0.03f;
    [SerializeField] private float liftStep = 0.1f;
    [SerializeField] private float maxLift = 4f;

    private const float AngleStep = 1f;

    private bool facingRight = true;
    private List<Transform> roots = new List<Transform>();
    private List<Joint2D> joints = new List<Joint2D>();

    private Transform trailerRoot;
    private Transform trailerTongue;


    private void Start()
    {
        joints = new List<Joint2D>();

        AddRoot(carBody.transform);
    }

    public void SetAttachment(Transform attactment, Transform anchor)
    {
        trailerRoot = attactment;
        trailerTongue = anchor;

        AddRoot(attactment);
    }

    public void AddRoot(Transform root)
    {
        roots.Add(root);

        foreach (var j in root.GetComponentsInChildren<Joint2D>(true))
        {
            if (j.enabled) joints.Add(j);
        }
    }

    public void SetFacing(bool right)
    {
        if (right != facingRight)
        {
            Flip();
        }
    }

    public void Flip()
    {
        float pivotX = carBody.position.x - (flipOffset * carBody.transform.localScale.x);

        foreach (var j in joints)
        {
            j.enabled = false;
        }

        foreach (var r in roots)
        {
            Vector3 p = r.position;
            p.x = pivotX - (p.x - pivotX);
            r.position = p;

            Vector3 e = r.eulerAngles;
            r.rotation = Quaternion.Euler(0f, 0f, -e.z);

            Vector3 s = r.localScale;
            s.x = -s.x;
            r.localScale = s;
        }

        FreeWheelsFromTerrain();
        Physics2D.SyncTransforms();

        foreach (var j in joints)
        {
            j.enabled = true;
        }

        facingRight = !facingRight;
    }

    private void FreeWheelsFromTerrain()
    {
        Transform car = carBody.transform;
        var carWheels = GetWheels(car);
        var trailerWheels = trailerRoot != null ? GetWheels(trailerRoot) : null;

        for (float lifted = 0f; ; lifted += liftStep)
        {
            bool carFree = FreeCar(car, carWheels);
            bool trailerFree = FreeTrailer(car, trailerWheels);

            if (carFree && trailerFree)
            {
                return;
            }

            if (lifted >= maxLift)
            {
                Debug.LogWarning("Wheels still in terrain after raising by " + maxLift);
                return;
            }

            Raise(car, liftStep);
        }
    }

    private bool FreeCar(Transform car, List<CircleCollider2D> wheels)
    {
        if (wheels.Count == 0)
        {
            return true;
        }

        Vector2 pivot = Vector2.zero;

        foreach (var w in wheels)
        {
            pivot += WheelCenter(w);
        }

        pivot /= wheels.Count;

        float z = Mathf.DeltaAngle(0f, car.eulerAngles.z);
        return RotateToFree(car, pivot, wheels, -maxTilt - z, maxTilt - z);
    }


    private bool FreeTrailer(Transform car, List<CircleCollider2D> wheels)
    {
        if (wheels == null || wheels.Count == 0 || carHitch == null || trailerTongue == null)
        {
            return true;
        }

        trailerRoot.position += carHitch.position - trailerTongue.position;

        float rel = Mathf.DeltaAngle(car.eulerAngles.z, trailerRoot.eulerAngles.z);
        return RotateToFree(trailerRoot, carHitch.position, wheels, -hitchMaxAngle - rel, hitchMaxAngle - rel);
    }


    private bool RotateToFree(Transform root, Vector2 pivot, List<CircleCollider2D> wheels, float minAngle, float maxAngle)
    {
        Vector3 startPos = root.position;
        Quaternion startRot = root.rotation;

        float start = Mathf.Clamp(0f, minAngle, maxAngle);
        float range = maxAngle - minAngle;

        for (float offset = 0f; offset <= range; offset += AngleStep)
        {
            for (int dir = 1; dir >= -1; dir -= 2)
            {
                float angle = start + offset * dir;
                if (angle < minAngle || angle > maxAngle)
                {
                    continue;
                }

                root.SetPositionAndRotation(startPos, startRot);
                root.RotateAround(pivot, Vector3.forward, angle);
                if (!AnyStuck(wheels))
                {
                    return true;
                }

            }
        }

        root.SetPositionAndRotation(startPos, startRot);
        root.RotateAround(pivot, Vector3.forward, start);

        return false;
    }

    private void Raise(Transform car, float amount)
    {
        car.position += Vector3.up * amount;
        foreach (var r in roots)
        {
            if (r != car && r != trailerRoot) r.position += Vector3.up * amount;
        }
    }

    private bool AnyStuck(List<CircleCollider2D> wheels)
    {
        foreach (var w in wheels)
        {
            float r = Mathf.Max(0.01f, WheelRadius(w) - embedTolerance);

            if (Physics2D.OverlapCircle(WheelCenter(w), r, groundMask) != null)
            {
                return true;
            }
        }

        return false;
    }

    private static List<CircleCollider2D> GetWheels(Transform root)
    {
        var list = new List<CircleCollider2D>();

        foreach (var c in root.GetComponentsInChildren<CircleCollider2D>())
        {
            if (c.enabled && !c.isTrigger)
            {
                list.Add(c);
            }
        }

        return list;
    }

    private static Vector2 WheelCenter(CircleCollider2D c)
    {
        return c.transform.TransformPoint(c.offset);
    }

    private static float WheelRadius(CircleCollider2D c)
    {
        Vector3 s = c.transform.lossyScale;
        return c.radius * Mathf.Max(Mathf.Abs(s.x), Mathf.Abs(s.y));
    }
}