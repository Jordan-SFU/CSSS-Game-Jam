using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class Shoot_Atom : MonoBehaviour
{
    public string atomName = "";
    public string currentState = "solid";
    private float mass = 1.0f;
    private List<GameObject> atoms = new List<GameObject>();
    public int strokeCount = 0;

    private Vector3 startDragPosition;
    private Vector3 endDragPosition;
    private Rigidbody2D rb;
    private ParticleSystem newParticleSystem;
    public float shootForce = 10f;
    public float maxForce = 20f;
    private LineRenderer lineRenderer;

    private TextMeshPro tmp;
    public float maxFontSize = 40f;
    public float minFontSize = 5f;


    bool canDrag = true;
    private bool isDragging = false;

    private Dictionary<string, int> elementCounts = new Dictionary<string, int>();

    // Reference to the levelManager
    private levelManager levelManager;

    void Start()
    {
        // Get the rigidbody
        rb = GetComponent<Rigidbody2D>();
        newParticleSystem = GetComponent<ParticleSystem>();
        tmp = GetComponentInChildren<TextMeshPro>();

        // Initialize the line renderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;

        // Set the material and color gradient for black-to-transparent fade
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.black, 0.0f), new GradientColorKey(Color.black, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;

        // Set rounded edges
        lineRenderer.numCapVertices = 10;

        tmp.text = atomName;
        AdjustFontSizeToFit(tmp);
        
        // Find the levelManager in the scene
        levelManager = GameObject.Find("LevelManager").GetComponent<levelManager>();
        if (levelManager == null)
        {
            Debug.LogError("levelManager not found in the scene!");
        }
    }


    public void AdjustFontSizeToFit(TextMeshPro textComponent)
    {
        RectTransform rect = textComponent.GetComponent<RectTransform>();
        float maxWidth = rect.rect.width;

        string text = textComponent.text;

        float fontSize = maxFontSize;
        textComponent.fontSize = fontSize;

        while (fontSize >= minFontSize)
        {
            Vector2 size = textComponent.GetPreferredValues(text, maxWidth, 1000f);
            if (size.x <= maxWidth)
            {
                break;
            }
            fontSize -= 1f;
            textComponent.fontSize = fontSize;
        }
    }
    void Update()
    {
        rb.mass = mass;
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            canDrag = false;
        }
        else
        {
            canDrag = true;
        }
        tmp.text = atomName;
        
        tmp.enableAutoSizing = true;
        tmp.fontSizeMin = 1;
        tmp.fontSizeMax = 20;
        tmp.textWrappingMode = TextWrappingModes.NoWrap;

        // Dragging and shooting logic...
        if (Input.GetMouseButtonDown(0) && canDrag)
        {
            startDragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startDragPosition.z = 0;
            lineRenderer.enabled = true;
            isDragging = true;
        }

        if (Input.GetMouseButton(0) && canDrag && isDragging)
        {
            Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentMousePosition.z = 0;

            Vector3 dragVector = currentMousePosition - startDragPosition;
            float dragDistance = dragVector.magnitude;
            float maxDistance = 5.0f;
            if (dragDistance > maxDistance)
            {
                dragVector = dragVector.normalized * maxDistance;
                currentMousePosition = startDragPosition + dragVector;
            }

            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, currentMousePosition);

            float forceStrength = Mathf.Clamp(dragDistance * shootForce, 0, maxForce);
            float thickness = Mathf.Lerp(0.1f, 0.5f, forceStrength / maxForce);
            lineRenderer.startWidth = thickness;
            lineRenderer.endWidth = thickness;
        }

        if (Input.GetMouseButtonUp(0) && canDrag && isDragging)
        {
            endDragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            endDragPosition.z = 0;

            Vector3 direction = startDragPosition - endDragPosition;
            float forceStrength = Mathf.Clamp((startDragPosition - endDragPosition).magnitude * shootForce, 0, maxForce);
            rb.AddForce(direction.normalized * forceStrength, ForceMode2D.Impulse);

            GameObject[] windTunnels = GameObject.FindGameObjectsWithTag("Fan");
            foreach (GameObject windTunnel in windTunnels)
            {
                windTunnel.GetComponent<WindTunnel>().timeInTunnel = 0;

            }
            // Remove the line renderer
            lineRenderer.enabled = false;

            // Increment the stroke count
            strokeCount++;
            isDragging = false;
        }
    }

    void UpdateStateFromLevelManager()
    {
        if (levelManager == null) return;

        foreach (var compoundState in levelManager.GetCompoundStateDictionary())
        {
            if (compoundState.Key == atomName)
            {
                if (currentState != compoundState.Value)
                {
                    currentState = compoundState.Value;
                    Debug.Log($"State updated: {atomName} is now {currentState}");
                }
                break;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Atom"))
        {
            onAtomHit(collision.gameObject);
            GameObject atom = collision.gameObject;
            List<string> elements = parseIntoElements(collision.gameObject.GetComponent<atomInfo>().elementString);

            for (int i = 0; i < elements.Count; i++)
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

            // Update the current state based on the levelManager
            UpdateStateFromLevelManager();

            Destroy(atom);
        }
        else if (collision.gameObject.CompareTag("Finish"))
        {
            if (collision.gameObject.GetComponent<goalRequirement>().goalCompound == atomName)
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

        var sorted = elementCounts.OrderBy(pair => pair.Key);
        foreach (KeyValuePair<string, int> kvp in sorted)
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

    void onAtomHit(GameObject atom)
    {
        // Change the particle color to match the atom's SpriteRenderer color
        var mainModule = newParticleSystem.main;
        SpriteRenderer atomSpriteRenderer = atom.GetComponent<SpriteRenderer>();
        if (atomSpriteRenderer != null)
        {
            mainModule.startColor = atomSpriteRenderer.color;
        }
        else
        {
            Debug.LogWarning("SpriteRenderer not found on the collided atom: " + atom.name);
        }

        // Play the particle system
        newParticleSystem.Play();

        // Update the mass
        mass += atom.GetComponent<atomInfo>().elementMass;

        // Log the hit
        Debug.Log("Hit atom: " + atom.GetComponent<atomInfo>().elementString + " with mass: " + atom.GetComponent<atomInfo>().elementMass + ". New mass: " + mass);


    }

    public void KillPlayer()
    {
        Debug.Log("Player has been killed!");
        // reload the current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

    }
}