using UnityEngine;

public class RayGun : MonoBehaviour
{
    [SerializeField] private Transform rayContainer;
    [SerializeField] private GameObject rayPrefab;
    [SerializeField] private int raysPerLayer;
    [SerializeField] private int numOfLayers;
    [SerializeField] private float angleFromCenter;
    [SerializeField] private GameObject decal;

    private GameObject[,] paintRays;
    private Transform[] samplePaintRays;
    private GameObject[] scanRays;

    private bool scanning;
    private bool painting;

    public bool Scanning
    {
        set
        {
            if (scanning = value)
            {
                Painting = false;
            }
        }
    }

    public bool Painting
    {
        set
        {
            if (painting = value)
            {
                Scanning = false;
            }
            SetActiveRays(paintRays, painting);
        }
    }

    private void Awake()
    {
        paintRays = new GameObject[raysPerLayer, numOfLayers];
        samplePaintRays = new Transform[numOfLayers];

        for (int i = 0; i < numOfLayers; i++)
        {
            for (int j = 0; j < raysPerLayer; j++)
            {
                paintRays[i, j] = Instantiate(rayPrefab, rayContainer);
            }
            samplePaintRays[i] = paintRays[i, 0].transform;
        }

        Scanning = Painting = false;
    }

    private void Update()
    {
        Paint();
        Scan();
    }

    public void Scan()
    {
        if (!scanning)
            return;

        Scanning = false;
    }

    // Paint dots on the geometry
    public void Paint()
    {
        if (!painting)
            return;

        RandomlyOrientRays(paintRays);
        RandomlyPaintDecals(samplePaintRays);
        Painting = false;
    }

    private void PaintDecal(Vector3 position, Transform target)
    {
        Instantiate(decal, position, Quaternion.FromToRotation(Vector3.forward, target.forward)).transform
            .SetParent(target.transform);
    }

    private void RandomlyOrientRays(GameObject[,] rays)
    {
        for (int i = 0; i < numOfLayers; i++)
        {
            float angleFromCenter = this.angleFromCenter * ((i + 1) / (float)numOfLayers);
            float radianOffset = Random.Range(0, 2 * Mathf.PI);

            for (int j = 0; j < raysPerLayer; j++)
            {
                float radians = 2 * Mathf.PI * (j / (float)raysPerLayer) + radianOffset;
                rays[i, j].transform.localEulerAngles = angleFromCenter *
                    new Vector3(Mathf.Sin(radians), Mathf.Cos(radians), 0);
            }
        }
    }

    private void RandomlyPaintDecals(Transform[] rays)
    {
        foreach (Transform ray in rays)
        {
            if (!Physics.Raycast(ray.position, ray.forward, out RaycastHit hit))
                continue;

            PaintDecal(hit.point, hit.transform);
        }
    }

    private void SetActiveRays(GameObject[,] rays, bool active)
    {
        foreach (GameObject ray in rays)
        {
            ray.SetActive(active);
        }
    }
}