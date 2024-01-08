using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    bool timerActive = false;
    float currentTime;
    public int startMinutes;
    public Text currentTimeText;
    // Start is called before the first frame update
    void Start()
    {
        currentTime = startMinutes*60;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerActive == true)
        {
            currentTime = currentTime - Time.deltaTime;

        }
        currentTimeText.text = currentTime.ToString();
    }
    // game over arrête le timer ?
    

  
   
}