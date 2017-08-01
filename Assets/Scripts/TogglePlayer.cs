using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglePlayer : MonoBehaviour {

    public GameObject APlayer; //Player controlled by default animations
    public GameObject NPlayer; //Player controlled by perception neuron
    public Transform APlayerPos;
    public Transform NPlayerPos;

    bool togg = true;      


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Toggling between APlayer and NPlayer by Pressing Left Thumbstick
        if (getReal3D.Input.GetButtonDown("TogglePlayer"))
        {
            togg = !togg;
        }
      
        if (togg)
        {
            APlayer.SetActive(true);
            NPlayer.SetActive(false);
            

            Debug.Log("changed to neuron player");


        }
        else
        {
            APlayer.SetActive(false);
            NPlayer.SetActive(true);
            NPlayerPos.position = APlayerPos.position;
            Debug.Log("changed to animated player");
        }
        

    }
}
