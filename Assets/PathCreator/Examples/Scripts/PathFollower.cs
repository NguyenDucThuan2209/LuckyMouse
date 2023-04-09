using UnityEngine;

namespace PathCreation.Examples
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower : MonoBehaviour
    {
        public GeneratePath generatePath;
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        GameObject pathObject;
        public float speed = 5;
        float distanceTravelled;

        bool isActivePath = false;
        bool isFinalMouseRun = false;
        bool triggerSendRequest = false;
        void Start()
        {
            SocketClient.instance.OnJoinRoom();
        }
        public void ActivePath(bool _isFinalMouseRun)
        {
            isFinalMouseRun = _isFinalMouseRun;
            pathObject = GameObject.Find("PathCreator");
            if (pathObject != null)
            {
                generatePath = pathObject.GetComponent<GeneratePath>();
                if (generatePath != null)
                {
                    generatePath.ActivePath(_isFinalMouseRun);
                }
                pathCreator = pathObject.GetComponent<PathCreator>();
                SetPathChanged();

            }
        }

        void SetPathChanged()
        {
            if (pathCreator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
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
            //Debug.Log(" pathCreator.path.length ======  " + pathCreator.path.length);
            //Debug.Log("pathCreator.path.NumPoints ======  " + pathCreator.path.NumPoints);
            //Debug.Log("pathCreator.path.isClosedLoop ======  " + pathCreator.path.isClosedLoop);
            //Debug.Log("distanceTravelled ====== update " + distanceTravelled);

            if(distanceTravelled >= pathCreator.path.length)
            {
                Debug.Log(" Stop run path   ======  endOfPathInstruction " + isFinalMouseRun);
                
                if (!isFinalMouseRun && !triggerSendRequest)
                {
                    distanceTravelled = 0f;
                    Debug.Log(" Stop run path   ======  OnRequestNextRun ");
                    SocketClient.instance.OnRequestNextRun();
                    triggerSendRequest = true;
                }
                isActivePath = false;
            }



        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged() {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
            
        }
    }
}