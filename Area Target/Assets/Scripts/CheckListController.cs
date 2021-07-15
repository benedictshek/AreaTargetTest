using UnityEngine;

public class CheckListController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "MainCamera")
        {
            GetComponentInParent<Animator>().SetBool("CheckListOpen_" + GameManagement2.Instance.checkListNum, true);
            GetComponent<Collider>().enabled = false;

            GameManagement2.Instance.ArrivedCheckPoint();
        }
    }
}
