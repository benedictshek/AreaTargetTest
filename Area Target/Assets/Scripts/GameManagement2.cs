using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GameManagement2 : MonoBehaviour
{
    public static GameManagement2 Instance { get; private set; }

    [SerializeField]
    private List<GameObject> nextCheckList;

    [SerializeField]
    private Material[] nextCheckList_Mat;

    [SerializeField]
    private List<Transform> checkListPoint;

    private float nextCheckList_offset;
    private float lineRenderer_offset;

    public bool CheckListOpen = false;

    public int checkListNum = 0;

    public LineRenderer lineRenderer_Path;
    private Material lineRenderer_Mat;

    private NavMeshPath path;
    public Transform CurrentARCameraPosition;
    public Button ResetPathBtn;

    public Text DebugText;

    private Material checkListOption_Mat;

    [Header("Texture Change")]
    public Texture NoChooseOptionTexture;
    public Texture ChooseOptionTexture;
    public Texture ChooseRemarkTexture;

    private GameObject remarkWindow;

    private bool isDirty;
    private bool isBadCondition;
    private bool isIncompleteAmenities;
    private bool isWrongPlacement;

    [Header("Info")]
    public Texture OpenInfoTexture;
    public Texture CloseInfoTexture;
    private GameObject Info;
    private Material index;

    [Header("Full Screen Detail")]
    public GameObject FullScreenDetail_Panel;
    public RawImage Info_image;
    private AspectRatioFitter aspectRatio;
    public TMP_Text Description;

    public Transform Arrow;

    private MeshCollider meshCollider;

    [Header("Guide")]
    public Transform GuidObj;
    public float MoveSpeed;
    private int nextPoint = 1;
    public LayerMask layerMask;
    private float guideRange;

    public Transform guidUIObj;
    public Transform guidUI;

    [Header("Minimap")]
    public GameObject CheckPointMinimap;
    private TMP_Text CheckPointNum;

    //public Camera test;

    private void Start()
    {
        Instance = this;

        nextCheckList_Mat = new Material[nextCheckList.Count];

        for(int i = 0; i < nextCheckList.Count; i++)
        {
            nextCheckList_Mat[i] = nextCheckList[i].GetComponent<MeshRenderer>().material;
        }

        lineRenderer_Mat = lineRenderer_Path.material;
        lineRenderer_Path.gameObject.SetActive(false);

        path = new NavMeshPath();

        aspectRatio = Info_image.GetComponent<AspectRatioFitter>();

        guideRange = GuidObj.GetComponent<SphereCollider>().radius;

        guidUI.gameObject.SetActive(false);

        Arrow.gameObject.SetActive(false);

        CheckPointMinimap.transform.position = checkListPoint[checkListNum].transform.position;
        CheckPointNum = CheckPointMinimap.GetComponentInChildren<TextMeshPro>();

        ResetPathBtn.interactable = false;
    }

    public void OpenCheckList()
    {
        CheckListOpen = true;
    }

    public void GoToNextCheckList()
    {
        bool hasPath = NavMesh.CalculatePath(CurrentARCameraPosition.position, checkListPoint[checkListNum].position, NavMesh.AllAreas, path);
        //DebugText.text = hasPath.ToString();

        lineRenderer_Path.positionCount = path.corners.Length;
        lineRenderer_Path.SetPositions(path.corners);
        lineRenderer_Path.gameObject.SetActive(true);

        /*meshCollider = lineRenderer_Path.gameObject.AddComponent<MeshCollider>();
        Mesh mesh = new Mesh();
        lineRenderer_Path.BakeMesh(mesh, true);
        meshCollider.sharedMesh = mesh;*/

        CheckPointMinimap.SetActive(true);
        CheckPointMinimap.transform.position = checkListPoint[checkListNum].transform.position;
        CheckPointNum.text = "0" + checkListNum;
        CheckPointNum.gameObject.transform.rotation = Quaternion.Euler(90, 0, -90);
    }

    public void CloseFullScreenDetailPanel()
    {
        FullScreenDetail_Panel.SetActive(false);
    }

    void DrawBounds(Bounds b, float delay = 0)
    {
        // bottom
        var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
        var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
        var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
        var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

        Debug.DrawLine(p1, p2, Color.blue, delay);
        Debug.DrawLine(p2, p3, Color.red, delay);
        Debug.DrawLine(p3, p4, Color.yellow, delay);
        Debug.DrawLine(p4, p1, Color.magenta, delay);

        // top
        var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
        var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
        var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
        var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

        Debug.DrawLine(p5, p6, Color.blue, delay);
        Debug.DrawLine(p6, p7, Color.red, delay);
        Debug.DrawLine(p7, p8, Color.yellow, delay);
        Debug.DrawLine(p8, p5, Color.magenta, delay);

        // sides
        Debug.DrawLine(p1, p5, Color.white, delay);
        Debug.DrawLine(p2, p6, Color.gray, delay);
        Debug.DrawLine(p3, p7, Color.green, delay);
        Debug.DrawLine(p4, p8, Color.cyan, delay);
    }

    private void Update()
    {
        if (Input.GetButton("Fire1") && CheckListOpen == true && !FullScreenDetail_Panel.activeSelf)
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                if (raycastHit.collider.gameObject.tag == "Next")
                {
                    nextCheckList_offset -= Time.deltaTime;
                    nextCheckList_Mat[checkListNum].mainTextureOffset = new Vector2(nextCheckList_offset, 0);

                    if(nextCheckList_offset <= -1)
                    {
                        CheckListOpen = false;
                        GetComponent<Animator>().SetBool("CheckListOpen_" + checkListNum, false);
                        checkListNum++;

                        if(Info != null)
                        {
                            Info.SetActive(false);
                        }

                        checkListOption_Mat = null;
                        isDirty = false;
                        isBadCondition = false;
                        isIncompleteAmenities = false;
                        isWrongPlacement = false;

                        GoToNextCheckList();

                        guidUI.gameObject.SetActive(true);
                        guidUIObj.parent.gameObject.transform.position = CurrentARCameraPosition.position;

                        ResetPathBtn.interactable = true;
                    }
                }
            }
        }
        else
        {
            if(CheckListOpen == true)
            {
                nextCheckList_offset = 0;
                nextCheckList_Mat[checkListNum].mainTextureOffset = new Vector2(nextCheckList_offset, 0);
            }
        }

        if (Input.GetButtonDown("Fire1") && !FullScreenDetail_Panel.activeSelf)
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                switch (raycastHit.collider.gameObject.tag)
                {
                    case "Condition":
                        if (checkListOption_Mat != null)
                        {
                            checkListOption_Mat.mainTexture = NoChooseOptionTexture;
                            remarkWindow.SetActive(false);
                        }

                        checkListOption_Mat = raycastHit.collider.gameObject.GetComponent<MeshRenderer>().material;
                        checkListOption_Mat.mainTexture = ChooseOptionTexture;
                        break;
                    case "Remark":
                        if (checkListOption_Mat != null)
                        {
                            checkListOption_Mat.mainTexture = NoChooseOptionTexture;
                        }

                        checkListOption_Mat = raycastHit.collider.gameObject.GetComponent<MeshRenderer>().material;
                        checkListOption_Mat.mainTexture = ChooseRemarkTexture;

                        remarkWindow = raycastHit.collider.gameObject.transform.Find("RemarkWindow").gameObject;
                        remarkWindow.SetActive(true);
                        break;
                    case "Remark_D":
                        Material remark_Dirty = raycastHit.collider.gameObject.GetComponent<MeshRenderer>().material;
                        if (isDirty)
                        {
                            remark_Dirty.mainTexture = NoChooseOptionTexture;
                            isDirty = false;
                        }
                        else
                        {
                            remark_Dirty.mainTexture = ChooseOptionTexture;
                            isDirty = true;
                        }
                        break;
                    case "Remark_BC":
                        Material remark_BadCondition = raycastHit.collider.gameObject.GetComponent<MeshRenderer>().material;
                        if (isBadCondition)
                        {
                            remark_BadCondition.mainTexture = NoChooseOptionTexture;
                            isBadCondition = false;
                        }
                        else
                        {
                            remark_BadCondition.mainTexture = ChooseOptionTexture;
                            isBadCondition = true;
                        }
                        break;
                    case "Remark_IA":
                        Material remark_IncompleteAmenities = raycastHit.collider.gameObject.GetComponent<MeshRenderer>().material;
                        if (isIncompleteAmenities)
                        {
                            remark_IncompleteAmenities.mainTexture = NoChooseOptionTexture;
                            isIncompleteAmenities = false;
                        }
                        else
                        {
                            remark_IncompleteAmenities.mainTexture = ChooseOptionTexture;
                            isIncompleteAmenities = true;
                        }
                        break;
                    case "Remark_WP":
                        Material remark_WrongPlacement = raycastHit.collider.gameObject.GetComponent<MeshRenderer>().material;
                        if (isWrongPlacement)
                        {
                            remark_WrongPlacement.mainTexture = NoChooseOptionTexture;
                            isWrongPlacement = false;
                        }
                        else
                        {
                            remark_WrongPlacement.mainTexture = ChooseOptionTexture;
                            isWrongPlacement = true;
                        }
                        break;
                    case "OpenDetail":
                        Info = raycastHit.collider.gameObject.transform.Find("Info").gameObject;
                        Info.SetActive(true);

                        index = raycastHit.collider.gameObject.transform.Find("Index").GetComponent<MeshRenderer>().material;
                        index.mainTexture = OpenInfoTexture;
                        break;
                    case "CloseDetail":
                        Info.SetActive(false);

                        index.mainTexture = CloseInfoTexture;
                        break;
                    case "Zoom":
                        FullScreenDetail_Panel.SetActive(true);

                        Texture texture = raycastHit.collider.gameObject.GetComponent<MeshRenderer>().material.mainTexture;
                        Info_image.texture = texture;
                        aspectRatio.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
                        aspectRatio.aspectRatio = (float)texture.width / texture.height;

                        Description.text = raycastHit.collider.gameObject.GetComponentInChildren<TextMeshPro>().text;
                        break;
                }
            }
        }

        if (lineRenderer_Path.gameObject.activeSelf)
        {
            lineRenderer_offset -= Time.deltaTime;
            lineRenderer_Mat.mainTextureOffset = new Vector2(lineRenderer_offset, 0);

            Vector3 guidUIPos = Camera.main.WorldToScreenPoint(guidUIObj.position);
            guidUI.position = guidUIPos;

            float borderSize = 100f;
            Vector3 targetPos = Camera.main.WorldToScreenPoint(GuidObj.position);
            bool isOffScreen = targetPos.x <= borderSize || targetPos.x >= Screen.width - borderSize || targetPos.y <= borderSize || targetPos.y >= Screen.height - borderSize;

            /*if (isOffScreen)
            {
                Vector3 cappedPos = targetPos;
                if (cappedPos.x <= borderSize) cappedPos.x = borderSize;
                if (cappedPos.x >= Screen.width - borderSize) cappedPos.x = Screen.width - borderSize;
                if (cappedPos.y <= borderSize) cappedPos.y = borderSize;
                if (cappedPos.y >= Screen.height - borderSize) cappedPos.y = Screen.height - borderSize;

                Vector3 arrowWorldPos = test.ScreenToWorldPoint(cappedPos);
                Arrow.position = arrowWorldPos;
                Arrow.localPosition = new Vector3(Arrow.localPosition.x, Arrow.localPosition.y, 0f);
            }*/

            if (Physics.CheckSphere(GuidObj.position, guideRange, layerMask))
            {
                if (nextPoint < lineRenderer_Path.positionCount)
                {
                    Vector3 nextPointPos = new Vector3(lineRenderer_Path.GetPosition(nextPoint).x, GuidObj.position.y, lineRenderer_Path.GetPosition(nextPoint).z);
                    if (Vector3.Distance(GuidObj.position, nextPointPos) < 0.1f)
                    {
                        nextPoint++;
                    }
                    else
                    {
                        GuidObj.position = Vector3.MoveTowards(GuidObj.position, nextPointPos, MoveSpeed * Time.deltaTime);
                    }
                }
            }

            if(isOffScreen != false)
            {
                Arrow.gameObject.SetActive(true);
                Vector3 dir = Camera.main.transform.InverseTransformPoint(GuidObj.position);
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Arrow.localEulerAngles = new Vector3(0, 0, angle);
            }
            else
            {
                Arrow.gameObject.SetActive(false);
            }

            float remainingDistance = Mathf.Ceil(Vector3.Distance(CurrentARCameraPosition.position, checkListPoint[checkListNum].transform.position));
            DebugText.text = "Ft: " + remainingDistance.ToString();
        }

        /*if (isOffScreen)
        {
            Vector3 capped = target;
            if (capped.x <= 0) capped.x = 0f;
            if (capped.x >= Screen.width) capped.x = Screen.width;
            if (capped.y <= 0) capped.y = 0f;
            if (capped.y >= Screen.height) capped.y = Screen.height;

            Vector3 pointer = Camera.main.ScreenToWorldPoint(capped);
            Arrow.position = pointer;
            Arrow.localPosition = new Vector3(Arrow.localPosition.x, Arrow.localPosition.y, 0f);
        }*/

        /*if (IsVisibleFrom(lineRenderer_Path, Camera.main))
        {
            DebugText.text = "ys";
        }
        else
        {
            DebugText.text = "no";
        }*/
    }

    private void LateUpdate()
    {
        CurrentARCameraPosition.position = new Vector3(Camera.main.transform.position.x, -1, Camera.main.transform.position.z);
    }

    public void ArrivedCheckPoint()
    {
        lineRenderer_Path.gameObject.SetActive(false);
        CheckPointMinimap.SetActive(false);
        guidUI.gameObject.SetActive(false);
        Arrow.gameObject.SetActive(false);
        DebugText.text = "";
        ResetPathBtn.interactable = false;
    }

    public void ResetPath()
    {
        NavMesh.CalculatePath(CurrentARCameraPosition.position, checkListPoint[checkListNum].position, NavMesh.AllAreas, path);

        lineRenderer_Path.positionCount = path.corners.Length;
        lineRenderer_Path.SetPositions(path.corners);

        guidUIObj.parent.gameObject.transform.position = CurrentARCameraPosition.position;
    }

    /*public static bool IsVisibleFrom(LineRenderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }*/
}
