using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public Camera mainCamera;
    public Material lineMaterial;
    private LineRenderer lineRenderer;
    private Vector2 target;
    private Vector3 targetPosition;
    public float speed = 15.0f;

    private bool hasDirectionTooReach = false;

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
        if (hasDirectionTooReach)
        {
            RemoveGravityImpactOnPlayer();
            PursueMovementToTarget();
        } else
        {
            ResetGravityImpactOnPlayer();
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            hasDirectionTooReach = false;
            StartReachTarget();
        }
    }

    private void StartReachTarget()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        //If something was hit, the RaycastHit2D.collider will not be null.
        if (hit.collider != null && hit.collider.tag == "Grappable")
        {
            hasDirectionTooReach = true;
            targetPosition = Input.mousePosition;
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // add force impulse
            addImpulseForceToReachTarget();
            

            // launch attraction to target
            PursueMovementToTarget();
        } else
        {
            lineRenderer.enabled = false;
        }
    }

    private void addImpulseForceToReachTarget()
    {
        // calculate needed force to reach position
        float xDistance = transform.position.x - targetPosition.x;
        float yDistance = Mathf.Abs(transform.position.y - targetPosition.y);

        Debug.Log(0.001f * xDistance);
        Debug.Log(0.05f * yDistance);

        Vector2 force = new Vector2(0, 0.05f * yDistance);
        GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
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

    private void DrawLine()
    {
        Vector2 targetPosition;

        if(hasDirectionTooReach == true)
        {
            targetPosition = target;
        } else
        {
            targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
        Vector2 currentPosition = transform.position;

        lineRenderer.SetPosition(0, targetPosition);
        lineRenderer.SetPosition(1, currentPosition);

        lineRenderer.positionCount = 2;
        lineRenderer.enabled = true;
    }

    private void RemoveGravityImpactOnPlayer()
    {
        /*if (rigidbody.bodyType == RigidbodyType2D.Dynamic)
        {
            rigidbody.bodyType = RigidbodyType2D.Static;
        }*/
    }

    private void ResetGravityImpactOnPlayer() 
    {
        //rigidbody.bodyType = RigidbodyType2D.Dynamic;
    }
}
