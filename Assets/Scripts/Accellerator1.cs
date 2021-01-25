using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Accellerator1 : MonoBehaviour
{
    private Rigidbody rb;
    private float steerAngle;
    private float horizontalInput;
    private float verticalInput;
    private int liveCount;
    private bool IsPaused;
    private int State;
    private bool IsSheilded;

    public WheelCollider wFL, wBL, wFR, wBR;
    public Transform tFL, tBL, tFR, tBR;
    public float maxSteerAngle;
    public float speed;
    public OSC osc;
    public TextMeshProUGUI Lives;
    public GameObject InsideCamera;
    public GameObject OutsideCamera;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        steerAngle = 0;
        osc.SetAllMessageHandler( OnReceiveXYZ);
        liveCount = 5;
        IsPaused = false;
        State = 0;
        IsSheilded = false;
        InsideCamera.SetActive(true);
        OutsideCamera.SetActive(false);
    }

    void update()
    {

    }

    void FixedUpdate()
    {
        Steer();
        Accelerate();
        WheelsPositions();
        if (State == 4)
        {
            System.Threading.Thread.Sleep(3000);
        }
    }

    void Steer()
    {
        steerAngle = maxSteerAngle * horizontalInput;
        wFL.steerAngle = steerAngle;
        wFR.steerAngle = steerAngle;    
    }

    void Accelerate()
    {
        wFL.motorTorque = verticalInput * speed;
        wFR.motorTorque = verticalInput * speed;
    }

    void WheelsPositions()
    {
        WheelPosition(wBL, tBL);
        WheelPosition(wBR, tBR);
        WheelPosition(wFL, tFL);
        WheelPosition(wFR, tFR);
    }

    void WheelPosition(WheelCollider wc, Transform t)
    {
        Vector3 pos = t.position;
        Quaternion rot = t.rotation;
        wc.GetWorldPose(out pos, out rot);
        t.position = pos;
        t.rotation = rot;
    }


    void OnReceiveXYZ(OscMessage message)
    {
        if(!IsPaused){
            horizontalInput = -message.GetFloat(0);
            verticalInput = message.GetFloat(2);
        }
        Debug.Log(message.GetFloat(0)); //tilt side ways
        Debug.Log(message.GetFloat(2)); //tilt forward/backward
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Eye"))
        {
            other.gameObject.SetActive(false);
            OutsideCamera.SetActive(true);
            InsideCamera.SetActive(false);
        }

        if (other.gameObject.CompareTag("Sheild"))
        {
            other.gameObject.SetActive(false);
            IsSheilded = true;
        }

        if (other.gameObject.CompareTag("Dragon"))
        {
            other.gameObject.SetActive(false);
            if (IsSheilded == false)
            {
                liveCount = liveCount - 1;
            }
            else
            {
                IsSheilded = false;
            }
            UpdateLives();
        }

        if (other.gameObject.CompareTag("Heart"))
        {
            other.gameObject.SetActive(false);
            liveCount = liveCount + 1;
            UpdateLives();
        }        
    }

    void UpdateLives()
    {
        if (liveCount <= 0)
        {
            //end
        }
        else
        {
            if (liveCount > 5)
            {
                liveCount = 5;
            }
            Lives.text = "";
            for (int i = 0; i < liveCount; i++)
            {
                Lives.text = Lives.text + "\u2665";
            }
        }
    }
}
