using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathCreator))]
public class GeneratePath : MonoBehaviour
{
    [SerializeField] List<Transform> m_pathHolderList;
    [SerializeField] List<Transform> m_losePathList;
        
    [SerializeField] bool m_isClosedLoop = true;
    [SerializeField] bool m_isLose;

    private PathCreator m_pathCreator;
    
    // Start is called before the first frame update
    void Start()
    {
        //ActivePath();
    }

    public void ActivePath(bool isFinalMouseRun)
    {
        int randomPathIndex;
        Transform[] pathPoints;
        if (!isFinalMouseRun)
        {
            // Get Path's points from path holder
            randomPathIndex = Random.Range(0, m_pathHolderList.Count - 1);
            pathPoints = GetPath(m_pathHolderList[randomPathIndex]);
        }
        else
        {
            // Get Path's points from path holder
            randomPathIndex = Random.Range(0, m_losePathList.Count - 1);
            pathPoints = GetPath(m_losePathList[randomPathIndex]);
        }


        // Create Bezier path
        BezierPath path = new BezierPath(pathPoints, m_isClosedLoop, PathSpace.xyz);
        m_pathCreator = GetComponent<PathCreator>();
        m_pathCreator.bezierPath = path;

    }

    private Transform[] GetPath(Transform pathHolder)
    {
        int pathLength = pathHolder.childCount;
        Transform[] pathPoints = new Transform[pathLength];

        for (int i = 0; i < pathLength; i++)
        {
            var point = pathHolder.GetChild(i);
            
            pathPoints[i] = point;
            //Debug.Log("Path's point: " + point.name);            
        }

        return pathPoints;
    }
}
