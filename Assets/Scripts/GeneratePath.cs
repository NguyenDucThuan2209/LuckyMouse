using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathCreator))]
public class GeneratePath : MonoBehaviour
{
    [SerializeField] List<Transform> m_pathHolderList;
    [SerializeField] Transform m_losePath;
        
    [SerializeField] bool m_isClosedLoop = true;
    [SerializeField] bool m_isLose;

    private PathCreator m_pathCreator;
    
    // Start is called before the first frame update
    void Start()
    {
        // Get Path's points from path holder
        var randomPathIndex = Random.Range(0, m_pathHolderList.Count - 1);
        var pathPoints = GetPath(m_pathHolderList[randomPathIndex]);

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
            Debug.Log("Path's point: " + point.name);            
        }

        return pathPoints;
    }
}
