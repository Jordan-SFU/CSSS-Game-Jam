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
        tmp.text = atomName;
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

            // increment the stroke count
            strokeCount++;
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