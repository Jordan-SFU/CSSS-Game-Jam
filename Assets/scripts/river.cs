using UnityEngine;

public class river : MonoBehaviour
{
    public GameObject player;
    public BoxCollider2D riverCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // if the player collides with the river, kill them
        if (other.gameObject == player && player.GetComponent<Shoot_Atom>().currentState == "liquid")
        {
            player.GetComponent<Shoot_Atom>().KillPlayer();
        }
    }
}
