using UnityEngine;

public class fence : MonoBehaviour
{
    public GameObject player;
    public BoxCollider2D fenceCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // disable the fence collider if the player's state is not "solid"
        if (player.GetComponent<Shoot_Atom>().currentState != "solid")
        {
            fenceCollider.enabled = false;
        }
        else
        {
            fenceCollider.enabled = true;
        }
    }
}
