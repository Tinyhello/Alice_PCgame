using UnityEngine;
using UnityEngine.AI;

public class Navgation_line_test : MonoBehaviour
{
    public LineRenderer m_navPath;
    private NavMeshAgent aget;

    void Awake()
    {
        aget = GetComponent<NavMeshAgent>();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                aget.destination = hit.point;
            }
        }
        if (aget.hasPath)
        {//画路径线
            m_navPath.positionCount = aget.path.corners.Length;
            m_navPath.SetPositions(aget.path.corners);
        }
        else
        {
            m_navPath.positionCount = 0;
        }
    }
}