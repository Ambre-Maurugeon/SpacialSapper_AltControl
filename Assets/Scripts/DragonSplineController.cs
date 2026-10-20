using UnityEngine;

public class DragonSplineController : MonoBehaviour
{
    // lap duration per spline 
    [SerializeField] private float _lapDuration;
    public float lapDuration => _lapDuration;
}
