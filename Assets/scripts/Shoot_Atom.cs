using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Shoot_Atom : MonoBehaviour
{
    public string atomName = "";
    private List<GameObject> atoms = new List<GameObject>();
    private int strokeCount = 0;

    private Vector3 startDragPosition;
    private Vector3 endDragPosition;
    private Rigidbody2D rb;
    private ParticleSystem particleSystem;
    public float shootForce = 10f;
    public float maxForce = 20f;
    private LineRenderer lineRenderer;

    private TextMeshPro tmp;

    bool canDrag = true;
    private bool isDragging = false; // Flag to track if dragging has started

    private Dictionary<string, int> elementCounts = new Dictionary<string, int>();
    void Start()
    {
        // get the rigidbody
        rb = GetComponent<Rigidbody2D>();
        particleSystem = GetComponent<ParticleSystem>();
        tmp = GetComponentInChildren<TextMeshPro>();

        // init the line renderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;

        tmp.text = atomName;
    }

    void Update()
    {
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            canDrag = false;
        }
        else
        {
            canDrag = true;
        }
        tmp.text = atomName;

        // Check if the mouse is pressed and the atom is not moving
        if (Input.GetMouseButtonDown(0) && canDrag)
        {
            // Start position for dragging
            startDragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startDragPosition.z = 0;

            // Enable the line
            lineRenderer.enabled = true;

            // Set dragging flag to true
            isDragging = true;
        }

        // Check if the mouse is being held
        if (Input.GetMouseButton(0) && canDrag && isDragging)
        {
            // Get the mouse position in world coordinates
            Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentMousePosition.z = 0;

            // Create the line to show the drag
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, currentMousePosition);

            // Calculate force strength based on drag distance and adjust line thickness
            float forceStrength = Mathf.Clamp((startDragPosition - currentMousePosition).magnitude * shootForce, 0, maxForce);
            float thickness = Mathf.Lerp(0.1f, 0.5f, forceStrength / maxForce);
            lineRenderer.startWidth = thickness;
            lineRenderer.endWidth = thickness;
        }

        // Check if the mouse is released
        if (Input.GetMouseButtonUp(0) && canDrag && isDragging)
        {
            // Get the end position for dragging
            endDragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            endDragPosition.z = 0;

            // Calculate the direction and apply the force
            Vector3 direction = startDragPosition - endDragPosition;
            float forceStrength = Mathf.Clamp((startDragPosition - endDragPosition).magnitude * shootForce, 0, maxForce);
            rb.AddForce(direction.normalized * forceStrength, ForceMode2D.Impulse);

            // Remove the line renderer
            lineRenderer.enabled = false;

            // Increment the stroke count
            strokeCount++;

            // Reset dragging flag
            isDragging = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Atom"))
        {
            onAtomHit();
            GameObject atom = collision.gameObject;
            List<string> elements = parseIntoElements(collision.gameObject.GetComponent<atomInfo>().elementString);

            for( int i =0; i < elements.Count; i++)
            {
                if (!elementCounts.ContainsKey(elements[i]))
                {
                    elementCounts.Add(elements[i], 1);
                }
                else
                {
                    elementCounts[elements[i]] += 1;
                }
            }

            collectToFullAtomName();  
            atoms.Add(atom);

            // delete the atom from the scene
            Destroy(atom);
        }
        else if (collision.gameObject.CompareTag("Finish"))
        {
            if(collision.gameObject.GetComponent<goalRequirement>().goalCompound == atomName)
            {
                Debug.Log("You made the correct compound: " + atomName + " with " + strokeCount + " strokes!");
            }
            else
            {
                Debug.Log("You made the wrong compound: " + atomName + ". Expected: " + collision.gameObject.GetComponent<goalRequirement>().goalCompound);
            }
        }
    }

    void collectToFullAtomName()
        {
            atomName = "";
            foreach (KeyValuePair<string, int> kvp in elementCounts)
            {
                atomName += kvp.Key + (kvp.Value > 1 ? kvp.Value.ToString() : "");
            }
            

        }
    List<string> parseIntoElements(string atomName)
    {
        List<string> elements = new List<string>();
        string currentElement = "";

        for (int i = 0; i < atomName.Length; i++)
        {
            char c = atomName[i];
            if (char.IsUpper(c))
            {
                if (currentElement.Length > 0)
                {
                    elements.Add(currentElement);
                }
                currentElement = c.ToString();
            }
            else if (char.IsLower(c))
            {
                currentElement += c;
            }
        }

        if (currentElement.Length > 0)
        {
            elements.Add(currentElement);
        }

        return elements;
    }


    void onAtomHit()
    {
        particleSystem.Play();
    }
}