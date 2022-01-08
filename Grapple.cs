using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    public Camera mainCamera;
    public Material lineMaterial;
    private LineRenderer lineRenderer;
    
    public float speed = 15.0f;
    public float rangeLimit = 10.0f;

    // Vars used for position of line etc
    private Vector2 HandPosition;
    private Vector2 target;
    private GameObject targetObject;

    // Var associated to state
    private string mode = "pull"; // reach or pull
    private bool hasDirectionTooReach = false;
    private bool hasObjectToPull = false;
    private bool isReachingPosition = false;


    public void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.2f;
        lineRenderer.startColor = Color.black;
        lineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == "reach" && hasDirectionTooReach)
        {
            PursueMovementToTarget();
        } else if (mode == "pull" && hasObjectToPull)
        {
            PursuePullingObject();
        } else if (isReachingPosition)
        {
            PursueReachingPosition();
        }

        if (Input.GetMouseButtonDown(0))
        {
            ReachInputPosition();
        }

        if (Input.GetKeyDown(KeyCode.E) == true)
        {
            if (hasDirectionTooReach == false && hasObjectToPull == false)
            {
                mode = mode == "reach" ? "pull" : "reach";
            }
        }
    }

    private void ReachInputPosition()
    {
        isReachingPosition = true;
        target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        HandPosition = transform.position;

        PursueReachingPosition();
    }

    private void PursueReachingPosition()
    {
        if (target != Vector2.zero)
        {
            float distanceFromTarget = Vector2.Distance(target, HandPosition);

            if (distanceFromTarget > 0)
            { 
                HandPosition = Vector2.MoveTowards(HandPosition, target, speed * Time.deltaTime);
                DrawLine(HandPosition, transform.position);
            } else
            {
                if (mode == "reach")
                {
                    StartReachTarget();
                }
                else if (mode == "pull")
                {
                    StartPullObject();
                }

                isReachingPosition = false;
                lineRenderer.enabled = false;
            }
        }
        else
        {
            isReachingPosition = false;
            lineRenderer.enabled = false;
        }
    }

    private void StartReachTarget()
    {
        hasDirectionTooReach = false;
        RaycastHit2D hit = Physics2D.Raycast(HandPosition, Vector2.zero);

        // If something was hit, the RaycastHit2D.collider will not be null.
        if (hit.collider != null && hit.collider.tag == "Reachable")
        {
            hasDirectionTooReach = true;
            target = HandPosition;

            // launch movement to target
            PursueMovementToTarget();
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    private void StartPullObject()
    {
        hasObjectToPull = false;
        RaycastHit2D hit = Physics2D.Raycast(HandPosition, Vector2.zero);

        // If something was hit, the RaycastHit2D.collider will not be null.
        if (hit.collider != null && hit.collider.tag == "Pullable")
        {
            hasObjectToPull = true;
            target = Camera.main.ScreenToWorldPoint(hit.collider.gameObject.transform.position);
            targetObject = hit.collider.gameObject;
        }
    }

    private void PursueMovementToTarget()
    {
        if (target != Vector2.zero)
        {
            float distanceBewteenTarget = Vector2.Distance(target, transform.position);

            if (distanceBewteenTarget > 1)
            {
                transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
                DrawLine(target, transform.position);
            }
            else
            {
                lineRenderer.enabled = false;
                hasDirectionTooReach = false;
            }
        }
        else
        {
            lineRenderer.enabled = false;
            hasDirectionTooReach = false;
        }
    }

    private void PursuePullingObject()
    {
        if (target != Vector2.zero)
        {
            float distanceBewteenTarget = Vector2.Distance(targetObject.transform.position, transform.position);

            if (distanceBewteenTarget > 1)
            {
                targetObject.transform.position = Vector2.MoveTowards(targetObject.transform.position, transform.position, speed * Time.deltaTime);
                DrawLine(targetObject.transform.position, transform.position);
            }
            else
            {
                lineRenderer.enabled = false;
                hasObjectToPull = false;
            }
        }
        else
        {
            lineRenderer.enabled = false;
            hasObjectToPull = false;
        }
    }

    private void DrawLine(Vector2 target, Vector2 currentPosition)
    {
        lineRenderer.SetPosition(0, target);
        lineRenderer.SetPosition(1, currentPosition);

        lineRenderer.positionCount = 2;
        lineRenderer.enabled = true;
    }
}
