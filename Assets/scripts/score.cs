using UnityEngine;
using TMPro;

public class score : MonoBehaviour
{
    public TextMeshPro tmp;
    public GameObject player;
    private int stroke;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        stroke = player.GetComponent<Shoot_Atom>().strokeCount;
        tmp.text = "Strokes: " + stroke.ToString();
    }
}
