using System.Collections.Generic;
using UnityEngine;

public class WindTunnel : MonoBehaviour
{
    public float windForce = 20f;
    public float windDirection = Mathf.PI / 2f; // Radians
    public float durationToZero = 3f; // Seconds until force fully diminishes

    public GameObject player;

    // Track how long each object has been in the tunnel
    public float timeInTunnel = 0;

    void OnTriggerStay2D(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null) return;


        timeInTunnel += Time.deltaTime;

        float t = timeInTunnel;
        float diminishingMultiplier = Mathf.Clamp01(1f - (t / durationToZero));

        // Adjust based on player's state
        int stateMultiplier = 1;
        string state = player.GetComponent<Shoot_Atom>().currentState;
        switch (state)
        {
            case "solid": stateMultiplier = 1; break;
            case "liquid": stateMultiplier = 2; break;
            case "gas": stateMultiplier = 3; break;
        }

        Vector2 direction = new Vector2(
            Mathf.Cos(windDirection),
            Mathf.Sin(windDirection)
        ).normalized;

        Vector2 force = direction * windForce * diminishingMultiplier * stateMultiplier;
        rb.AddForce(force, ForceMode2D.Force);

        Debug.Log($"Wind force on {other.name}: {force}, time inside: {t:F2}s");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        timeInTunnel = 0;
    }
}
