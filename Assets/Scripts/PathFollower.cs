using PathCreation;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public GeneratePath generatePath;
    public PathCreator pathCreator;
    public EndOfPathInstruction endOfPathInstruction;
    public float speed = 5;
    float distanceTravelled;

    bool isActivePath = false;
    bool isFinalMouseRun = false;
    bool triggerSendRequest = false;
    void Start()
    {
        if (SocketClient.instance == null)
        {
            ActivePath(false);
        }
        else
        {
            SocketClient.instance.OnJoinRoom();
        }            
    }
    public void ActivePath(bool _isFinalMouseRun)
    {
        gameObject.SetActive(true);
        isFinalMouseRun = _isFinalMouseRun;
        generatePath.ActivePath(_isFinalMouseRun);
        SetPathChanged();
    }

    void SetPathChanged()
    {
        if (pathCreator != null)
        {
            pathCreator.pathUpdated += OnPathChanged;
            isActivePath = true;
            triggerSendRequest = false;
        }
    }

    void Update()
    {
        if (!isActivePath) return;
        if (pathCreator != null)
        {
            distanceTravelled += speed * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
        }
        if(distanceTravelled >= pathCreator.path.length)
        {
            if (!isFinalMouseRun && !triggerSendRequest)
            {
                distanceTravelled = 0f;
                SocketClient.instance.OnRequestNextRun();
                triggerSendRequest = true;
            }
            else
            {
                SocketClient.instance.OnEndGame();
            }
            isActivePath = false;
            gameObject.SetActive(false);
        }
    }
    void OnPathChanged() {
        distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
            
    }
}
