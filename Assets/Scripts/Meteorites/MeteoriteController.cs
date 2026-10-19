using System.Collections;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class MeteoriteController : MonoBehaviour
{
    private MeteoriteData _myData ; // { get; set; }
    private bool _hasCollided = false;

    private Vector3 _contactPoint;

    public void OnSpawn(MeteoriteData data)
    {
        InitData(data);

        CastRayTowardsPlanet(); // get futur contact point

        StartCoroutine("RushPlanet");
    }

    private void InitData(MeteoriteData data)
    {
        _myData = data;

        // visuals
    }

    private IEnumerator RushPlanet()
    {
        while (!_hasCollided)
        {
            transform.position += _myData.speed * Time.deltaTime * transform.forward; // meteorite is facing the planet and rushing towards it

            yield return new WaitForEndOfFrame();
        }

        // has collided => EXPLODEEES !!!!
        Explode();

    }

    // to detect planet collision point, and prevent collision with others elements on the trajectory (objects on planet)
    private void CastRayTowardsPlanet()
    {
        // distance 200 for now but get value in meteorite manager
        // layer mask by string for now (ig it's useless in editor)
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 200, LayerMask.GetMask("Planet")))
        {
            if (hit.point != null) _contactPoint = hit.point;
        }
    }

    private void Explode()
    {
        // spawn X fires near contact point
        for(int i = 0; i <_myData.flames; i++)
        {
            // spawn fire near contact point
            FireManager.Instance.SpawnFireNear(_contactPoint, _myData.radius);
        }

        // destroy meteorite 
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        _hasCollided = true;
        Debug.Log("collision");
    }

    void OnTriggerEnter(Collider collider)
    {
        _hasCollided = true;

        Debug.Log("trigger");
    }

}
