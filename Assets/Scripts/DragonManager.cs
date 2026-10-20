using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class DragonManager : MonoBehaviour
{
    #region Fields
    [Header("Splines")]
    [Foldout("References"), SerializeField] private Transform _dragonSplines;

    [Header("Dragons")]
    [Foldout("References"), SerializeField] private Transform _dragonParent;
    [Foldout("References"), SerializeField] private GameObject _dragonPrefab;

    SplineContainer[] splines;
    public static DragonManager Instance { get; private set; }
    #endregion
    private void Awake()
    {
        // -- INSTANCE --
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

        // -- INIT myself --
        Init();
    }

    private void Init()
    {
        // assign all splines in children to splines field
        splines = _dragonSplines.GetComponentsInChildren<SplineContainer>();
    }

    [Button]
    public void SpawnDragonOnRdSpline()
    {
        // get random spline in dragon splines
        int r = Random.Range(0, splines.Count()-1);
        SplineContainer mySpline = splines[r];

        // spawn dragon at first knot pos of my spline
        Vector3 firstKnotPos = mySpline.Spline.ToArray()[0].Position;
        SpawnDragon(firstKnotPos, out GameObject dragon);

        // dragon starts follow my spline 
        if(dragon.TryGetComponent<DragonController>(out var controller))
            controller.FollowSpline(mySpline);
    }

    private void SpawnDragon(Vector3 position, out GameObject dragon)
    {
        dragon = Instantiate(_dragonPrefab, position, Quaternion.identity, _dragonParent); // default rot for now
    }
}
