using UnityEngine;

namespace PathCreation.Examples
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower : MonoBehaviour
    {
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed = 5;
        [HideInInspector]public float distanceTravelled;
        Rigidbody rb;
        public bool RotateOnSpline;

        int frameCounter = 0;


        void Start() {
            if (pathCreator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                pathCreator.pathUpdated += OnPathChanged;
            }
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            frameCounter++;

            if (pathCreator != null)
            {
                distanceTravelled += speed * Time.deltaTime;
                transform.position = (pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction));

                if (RotateOnSpline && frameCounter++ % 3 == 0) // Rotate only every few frames
                {
                    transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
                }
            }
            rb.isKinematic = pathCreator != null;
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged() {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }

        public void SnapPlayerToClosestPoint() {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
            transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled);
        }
    }
}