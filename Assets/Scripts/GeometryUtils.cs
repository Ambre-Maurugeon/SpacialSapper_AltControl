
using UnityEngine;
public class GeometryUtils
{
    public static Quaternion GetLookRotation(Vector3 from, Vector3 to)
    {
        Vector3 dir = (to - from).normalized;
        Quaternion lookRot = Quaternion.LookRotation(dir);

        return lookRot;
    }
}
