using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EVE_Init : MonoBehaviour
{

    private LaunchManager launchManager;
    private LoggingManager log;

    // Start is called before the first frame update
    void Start()
    {

        //EVE
        launchManager = GameObject.FindGameObjectWithTag("LaunchManager").GetComponent<LaunchManager>();
        log = launchManager.LoggingManager;
        int maxDuration = int.Parse(log.GetParameterValue("maxDuration"));
        bool timePressure = log.GetParameterValue("timePressure").ToLower() == "yes";

        launchManager.FirstPersonController.SetActive(false);// Den alten Controller deaktivieren

        launchManager.FirstPersonController = GameObject.FindGameObjectWithTag("Player");
        //launchManager.FirstPersonController.SetActive(false); //Das Framework aktiviert den Controller selbst
        DontDestroyOnLoad(launchManager.FirstPersonController);//Der Controller wird auch bei einem Szenenwechsel erhalten


    }


}
