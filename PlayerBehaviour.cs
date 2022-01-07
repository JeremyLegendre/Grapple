using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    public Camera mainCamera;
    public Material lineMaterial;
    private LineRenderer lineRenderer;
    private Vector2 target;
    private Vector3 targetPosition;
    private GameObject targetObject;
    public float speed = 15.0f;
    public float rangeLimit = 10.0f;

    private bool hasDirectionTooReach = false;
    private bool hasObjectToPull = false;
    public string mode = "reach"; // reach or pull

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
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (mode == "reach")
            {
                StartReachTarget();
            } else if (mode == "pull")
            {
                StartPullObject();
            }
        }

        if (Input.GetKeyDown(KeyCode.E) == true)
        {
            if (hasDirectionTooReach == false && hasObjectToPull == false)
            {
                mode = mode == "reach" ? "pull" : "reach";
            }
        }
    }

    private void StartReachTarget()
    {
        hasDirectionTooReach = false;
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        // If something was hit, the RaycastHit2D.collider will not be null.
        if (hit.collider != null && hit.collider.tag == "Reachable")
        {
            hasDirectionTooReach = true;
            targetPosition = Input.mousePosition;
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);

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
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        //If something was hit, the RaycastHit2D.collider will not be null.
        if (hit.collider != null && hit.collider.tag == "Pullable")
        {
            hasObjectToPull = true;
            target = Camera.main.ScreenToWorldPoint(hit.collider.gameObject.transform.position);
            targetPosition = Input.mousePosition;
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
                DrawLine();
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
                Vector2 currentPosition = transform.position;

                lineRenderer.SetPosition(0, targetObject.transform.position);
                lineRenderer.SetPosition(1, currentPosition);

                lineRenderer.positionCount = 2;
                lineRenderer.enabled = true;
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

    private void DrawLine()
    {
        Vector2 targetPosition;

        if (hasDirectionTooReach == true || hasObjectToPull == true)
        {
            targetPosition = target;
        }
        else
        {
            targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
        Vector2 currentPosition = transform.position;

        lineRenderer.SetPosition(0, targetPosition);
        lineRenderer.SetPosition(1, currentPosition);

        lineRenderer.positionCount = 2;
        lineRenderer.enabled = true;
    }
}
