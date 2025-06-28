using UnityEngine;

public class Shoot_Atom : MonoBehaviour
{
    private Vector3 startDragPosition;
    private Vector3 endDragPosition;
    private Rigidbody2D rb;
    public float shootForce = 10f;
    public float maxForce = 20f;
    private LineRenderer lineRenderer;

    void Start()
    {
        // get the rigidbody
        rb = GetComponent<Rigidbody2D>();

        // init the line renderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    void Update()
    {
        // Every frame check if the mouse is pressed
        if (Input.GetMouseButtonDown(0))
        {
            // start position for dragging
            startDragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startDragPosition.z = 0;
            // enable the line
            lineRenderer.enabled = true;
        }

        // every frame check if the mouse is being held
        if (Input.GetMouseButton(0))
        {
            // get the mouse position in world coordinates
            Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentMousePosition.z = 0;

            // create the line to show the drag
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, currentMousePosition);

            // calculate force strength based on drag distance and adjust line thickness
            float forceStrength = Mathf.Clamp((startDragPosition - currentMousePosition).magnitude * shootForce, 0, maxForce);
            float thickness = Mathf.Lerp(0.1f, 0.5f, forceStrength / maxForce);
            lineRenderer.startWidth = thickness;
            lineRenderer.endWidth = thickness;
        }

        // every frame check if the mouse is released
        if (Input.GetMouseButtonUp(0))
        {
            // get the end position for dragging
            endDragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            endDragPosition.z = 0;

            // calculate the direction and apply the force
            Vector3 direction = startDragPosition - endDragPosition;
            float forceStrength = Mathf.Clamp((startDragPosition - endDragPosition).magnitude * shootForce, 0, maxForce);
            rb.AddForce(direction.normalized * forceStrength, ForceMode2D.Impulse);

            // remove the line renderer
            lineRenderer.enabled = false;
        }
    }
}