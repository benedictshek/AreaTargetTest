using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class GameMangement : MonoBehaviour
{
    public TMP_Dropdown dropDown;

    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private Transform guidStartPoint;

    public List<Transform> destinationPoints;

    public LayerMask layerMask;

    private float guideRange;

    [SerializeField]
    public Transform guidUIObj;
    [SerializeField]
    public Transform guidUI;

    private TMP_Text guidText;

    private bool isStartPath = false;

    private NavMeshPath path;

    [SerializeField]
    private LineRenderer lineRenderer_Path;

    [SerializeField]
    private Transform pathStartPoint;

    private Material lineRenderer_Mat;

    private float offset;

    private void Start()
    {
        guideRange = agent.GetComponent<SphereCollider>().radius;

        guidText = guidUI.GetComponentInChildren<TMP_Text>();

        lineRenderer_Mat = lineRenderer_Path.material;

        path = new NavMeshPath();

        lineRenderer_Path.gameObject.SetActive(false);
    }

    public void ScannedArea()
    {
        agent.transform.position = guidStartPoint.position;
    }

    private void Update()
    {
        Vector3 guidUIPos = Camera.main.WorldToScreenPoint(guidUIObj.position);
        guidUI.position = guidUIPos;

        if(lineRenderer_Path.enabled == true)
        {
            offset -= Time.deltaTime;
            lineRenderer_Mat.mainTextureOffset = new Vector2(offset, 0);
        }
    }

    private void FixedUpdate()
    {
        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            if (isStartPath == true)
            {
                guidText.text = "Arrived!";
                lineRenderer_Path.enabled = false;
            }
            else
            {
                guidText.text = "Select Destination";
            }
        }
        else
        {
            if (Physics.CheckSphere(agent.transform.position, guideRange / 5, layerMask))
            {
                guidText.text = "Follow me";
                agent.isStopped = false;
            }
            else
            {
                //guidText.text = "Waiting..";
                agent.isStopped = true;
            }
        }
    }


    public void GoToDestination()
    {
        if(dropDown.value == 0)
        {
            agent.ResetPath();
            isStartPath = false;
        }
        else if(dropDown.value == 1)
        {
            DestinationGuide(0);
        }
        else if (dropDown.value == 2)
        {
            DestinationGuide(1);
        }
        else
        {
            DestinationGuide(2);
        }
    }

    public void DestinationGuide(int destinationPoint)
    {
        lineRenderer_Path.gameObject.SetActive(true);

        agent.transform.position = guidStartPoint.position;

        agent.SetDestination(destinationPoints[destinationPoint].position);
        isStartPath = true;

        pathStartPoint.position = new Vector3(Camera.main.transform.position.x, -1, Camera.main.transform.position.z);
        NavMesh.CalculatePath(pathStartPoint.position, destinationPoints[destinationPoint].position, NavMesh.AllAreas, path);

        lineRenderer_Path.positionCount = path.corners.Length;
        lineRenderer_Path.SetPositions(path.corners);
        lineRenderer_Path.enabled = true;
    }
}
