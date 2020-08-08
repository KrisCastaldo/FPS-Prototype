using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    [Header("UI References")]
    [Tooltip("Slider reference for sprint UI")]
    [SerializeField]
    Slider sprintSlider;
    [SerializeField]
    Image sprintFill;
    [Tooltip("Reload text reference")]
    [SerializeField]
    Text reload;

    [Tooltip("Gradient used for abilites")]
    public Gradient cDGradient;

    float value;

    public bool sprintCD;

    // Start is called before the first frame update
    void Start()
    {
        setSliders();
        reloading(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(sprintCD)
        updateSprintSlider();
    }

    public void setSliders()
    {
        sprintSlider.value = 7;
        sprintFill.color = cDGradient.Evaluate(7);
        value = 0;
    }

    void updateSprintSlider()
    {
        value +=Time.deltaTime;
        sprintSlider.value = value;
        sprintFill.color = cDGradient.Evaluate(value);

    }

    public void resetSprint()
    {
        sprintSlider.value = 0;
    }

    public void reloading(bool _reload)
    {
        reload.enabled = _reload;
        reload.text = "Relaoding";
    }
}
