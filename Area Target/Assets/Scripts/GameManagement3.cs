using PathCreation;
using System.Collections;
using UnityEngine;

public class GameManagement3 : MonoBehaviour
{
    [Header("Destination_A")]
    [SerializeField]
    private TrailRenderer trailRenderer_A;
    [SerializeField]
    public PathCreator pathCreator_A;

    private Material pathMaterial_A;

    private bool startedDestinationA;
    private bool arrivedDestinationA;

    private float distanceTravelled_A;

    [Header("Destination_B")]
    [SerializeField]
    private TrailRenderer trailRenderer_B;
    [SerializeField]
    public PathCreator pathCreator_B;

    private Material pathMaterial_B;

    private bool arrivedDestinationB;

    private float distanceTravelled_B;

    [Header("Destination_C")]
    [SerializeField]
    private TrailRenderer trailRenderer_C;
    [SerializeField]
    public PathCreator pathCreator_C;

    private Material pathMaterial_C;

    private bool arrivedDestinationC;

    private float distanceTravelled_C;

    [Header("Air Conditioning")]
    public GameObject CentralAirCondition;
    public GameObject AirCondition_B;
    public GameObject AirCondition_C;

    private Material centralAirCondition_mat;
    private Material airCondition_B_mat;
    private Material airCondition_C_mat;

    [Header("Speed")]
    public float TrailMovingSpeed;

    private void Start()
    {
        pathMaterial_A = trailRenderer_A.GetComponent<Renderer>().material;
        pathMaterial_B = trailRenderer_B.GetComponent<Renderer>().material;
        pathMaterial_C = trailRenderer_C.GetComponent<Renderer>().material;

        centralAirCondition_mat = CentralAirCondition.GetComponent<Renderer>().material;
        airCondition_B_mat = AirCondition_B.GetComponent<Renderer>().material;
        airCondition_C_mat = AirCondition_C.GetComponent<Renderer>().material;

        trailRenderer_A.gameObject.SetActive(false);
        trailRenderer_B.gameObject.SetActive(false);
        trailRenderer_C.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                if (raycastHit.collider.gameObject.tag == "DestinationA")
                {
                    if(startedDestinationA != true)
                    {
                        StartCoroutine(StartDestinationA());
                        trailRenderer_A.gameObject.SetActive(true);
                        startedDestinationA = true;
                    }
                }
            }
        }

        if(arrivedDestinationA != false)
        {
            pathMaterial_A.mainTextureOffset -= new Vector2(Time.deltaTime, 0);
        }

        if(arrivedDestinationB != false)
        {
            pathMaterial_B.mainTextureOffset += new Vector2(Time.deltaTime / 10, 0);
        }

        if(arrivedDestinationC != false)
        {
            pathMaterial_C.mainTextureOffset += new Vector2(Time.deltaTime / 10, 0);
        }
    }

    private IEnumerator StartDestinationA()
    {
        while (distanceTravelled_A < pathCreator_A.path.length)
        {
            distanceTravelled_A += TrailMovingSpeed * Time.deltaTime;
            trailRenderer_A.transform.position = pathCreator_A.path.GetPointAtDistance(distanceTravelled_A, EndOfPathInstruction.Stop);
            yield return null;
        }

        centralAirCondition_mat.SetColor("_BaseColor", Color.blue);

        arrivedDestinationA = true;

        trailRenderer_B.gameObject.SetActive(true);
        StartCoroutine(StartDestinationB());

        trailRenderer_C.gameObject.SetActive(true);
        StartCoroutine(StartDestinationC());
    }

    private IEnumerator StartDestinationB()
    {
        while (distanceTravelled_B < pathCreator_B.path.length)
        {
            distanceTravelled_B += TrailMovingSpeed * Time.deltaTime;
            trailRenderer_B.transform.position = pathCreator_B.path.GetPointAtDistance(distanceTravelled_B, EndOfPathInstruction.Stop);
            yield return null;
        }

        airCondition_B_mat.SetColor("_BaseColor", Color.red);

        arrivedDestinationB = true;
    }

    private IEnumerator StartDestinationC()
    {
        while (distanceTravelled_C < pathCreator_C.path.length)
        {
            distanceTravelled_C += TrailMovingSpeed * Time.deltaTime;
            trailRenderer_C.transform.position = pathCreator_C.path.GetPointAtDistance(distanceTravelled_C, EndOfPathInstruction.Stop);
            yield return null;
        }

        airCondition_C_mat.SetColor("_BaseColor", Color.red);

        arrivedDestinationC = true;
    }
}
