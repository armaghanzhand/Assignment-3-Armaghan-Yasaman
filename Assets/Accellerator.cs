using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Accellerator : MonoBehaviour
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
    public TextMeshProUGUI Hints;
    public TextMeshProUGUI Lives;
    public GameObject State1;
    public GameObject State2;
    //public GameObject State2_5;
    public GameObject State3;

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
        if (other.gameObject.CompareTag("Dragon1") && State == 0)
        {
            other.gameObject.SetActive(false);
            liveCount--;
            Lives.text = "";
            for (int i = 0; i < liveCount; i++)
            {
                Lives.text = Lives.text + "\u2665";
            }
            IsPaused = true;
            Hints.text = "Perfect!\nWhen you hit a dragon, You will lose a life. If you lose all your lives, you will lose the game.\n Now hit the gem with the shape of heart to gain a life!";
            System.Threading.Thread.Sleep(1000);
            State1.SetActive(false);
            IsPaused = false;
            State = 1;
        }
        if (other.gameObject.CompareTag("Heart") && State == 1)
        {
            other.gameObject.SetActive(false);
            liveCount++;
            Lives.text = "";
            for (int i = 0; i < liveCount; i++)
            {
                Lives.text = Lives.text + "\u2665";
            }
            IsPaused = true;
            Hints.text = "Excellent!\nYou can avoid losing a life by wearing a sheild. Find the shield and wear it, then hit the next dragon.";
            System.Threading.Thread.Sleep(1000);
            IsPaused = false;
            State2.SetActive(false);
            State = 2;
        }
        if (other.gameObject.CompareTag("Sheild") && State == 2)
        {
            other.gameObject.SetActive(false);
            IsSheilded = true;
            //State2_5.SetActive(false);
        }
        if (other.gameObject.CompareTag("Dragon2") && State == 2)
        {
            other.gameObject.SetActive(false);
            IsPaused = true;
            if (IsSheilded)
            {
                Hints.text = "You are doing great!\nYou didn't lose any life because you had sheild. Move ahead to home for rest.\nOn the way there is a gem for you. You can guess what it does yourself.";
            }
            else
            {
                Hints.text = "You lost a life because you didn't have a sheild. But don't worry, you can still finish the game. Move ahead to home for rest.\nOn the way there is a gem for you. Can you guess what it does?";
            }
            System.Threading.Thread.Sleep(1000);
            IsPaused = false;
            State3.SetActive(false);
            State = 3;
        }
        if (other.gameObject.CompareTag("Home") && State == 3)
        {
            IsPaused = true;
            Hints.text = "I'm so proud of you!\nI have nothing else to teach you.\nYou are now ready to play the real game.\nGood Luck!";
            State = 4;
        }
    }
}
