using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class Eagle_FlightController : MonoBehaviour
{

    //*******************************
    //**  Define Thrust Variables  **
    //*******************************
    public float verticalThrust = 1f;
    public float horizontalThrust = 1f;
    public float forwardThrust = 1f;

    //*******************************
    //**  Define Torque Variables  **
    //*******************************
    public float pitchTorque = 1f;
    public float yawTorque = 1f;
    public float rollTorque = 1f;

    //*************************************************
    //**  Define RCS HoverMode Translation Variables **
    //*************************************************

    public float translationLRValue = 1f;
    public float translationFBValue = 1f;
    public float translationUDValue = 1f;

    //************************
    //**  Define HUD Items  **
    //************************
    public Text heightCounter;
    public Text vThrustCounter;
    public Text flightModeIndicator;
    public Text gravityShieldIndicator;


    public Rigidbody eagleRB;
    public Transform rayEmitterObject;

    bool gravityShields = false;
    bool flightModeHover = true;
    //bool flightModeStandard = false; //not needed

    //**********************************************
    //**  Vertical Engine Particle Systems Setup  **
    //**********************************************

    public ParticleSystem VertEngine_FL;
    public ParticleSystem VertEngine_FR;
    public ParticleSystem VertEngine_RL;
    public ParticleSystem VertEngine_RR;

    //************************************************
    //**  Horizontal Engine Particle Systems Setup  **
    //************************************************

    public ParticleSystem MainEngine_Top;
    public ParticleSystem MainEngine_Bottom;
    public ParticleSystem MainEngine_Left;
    public ParticleSystem MainEngine_Right;


    //************************
    //**  RCS Module Setup  **
    //************************

    public ParticleSystem RCS_FL_F;
    public ParticleSystem RCS_FL_R;
    public ParticleSystem RCS_FL_T;
    public ParticleSystem RCS_FL_B;

    public ParticleSystem RCS_FR_F;
    public ParticleSystem RCS_FR_R;
    public ParticleSystem RCS_FR_T;
    public ParticleSystem RCS_FR_B;

    public ParticleSystem RCS_RL_F;
    public ParticleSystem RCS_RL_R;
    public ParticleSystem RCS_RL_T;
    public ParticleSystem RCS_RL_B;

    public ParticleSystem RCS_RR_F;
    public ParticleSystem RCS_RR_R;
    public ParticleSystem RCS_RR_T;
    public ParticleSystem RCS_RR_B;



    // Use this for initialization
    void Start()
    {
        eagleRB = GetComponent<Rigidbody>();
        StopAllRCS();
        gravityShields = false;
        gravityShieldIndicator.text = "Inactive";
    }

    // Update is called once per frame
    void Update()
    {
        //Find object below me
        Raycaster();

        //***********************************
        //***********************************
        //**  FLIGHT MODE TOGGLE CONTROLS  **
        //***********************************
        //***********************************

        if (Input.GetKeyUp(KeyCode.JoystickButton6))
        {
            if (flightModeHover == true)
            {
                flightModeHover = false;
                flightModeIndicator.text = "Flight Mode";
            }
            else
            {
                flightModeHover = true;
                flightModeIndicator.text = "Hover Mode";
            }
        }

        if (Input.GetKeyUp(KeyCode.JoystickButton3))
        {
            if (gravityShields == false)
            {
                gravityShields = true;
                eagleRB.useGravity = false;
                gravityShieldIndicator.text = "Active";
            }
            else
            {
                gravityShields = false;
                eagleRB.useGravity = true;
                gravityShieldIndicator.text = "Inactive";
            }
        }

        //check what mode is active
        if (flightModeHover == true)
        {
            HoverModeControls();
        }
        else
        {
            FlightModeControl();
        }


    }

    void StopAllRCS()
    {

        RCS_FL_F.Stop();
        RCS_FL_R.Stop();
        RCS_FL_T.Stop();
        RCS_FL_B.Stop();

        RCS_FR_F.Stop();
        RCS_FR_R.Stop();
        RCS_FR_T.Stop();
        RCS_FR_B.Stop();

        RCS_RL_F.Stop();
        RCS_RL_R.Stop();
        RCS_RL_T.Stop();
        RCS_RL_B.Stop();

        RCS_RR_F.Stop();
        RCS_RR_R.Stop();
        RCS_RR_T.Stop();
        RCS_RR_B.Stop();

    }

    private void KillMainEngines()
    {
        MainEngine_Top.Stop();
        MainEngine_Bottom.Stop();
        MainEngine_Left.Stop();
        MainEngine_Right.Stop();
    }

    private void KillVertEngines()
    {
        VertEngine_FL.Stop();
        VertEngine_FR.Stop();
        VertEngine_RL.Stop();
        VertEngine_RR.Stop();
    }

    private void Raycaster()
    {
        Transform rayEmitter = rayEmitterObject.transform;
        rayEmitter.transform.position = this.transform.position;
        RaycastHit hit;
        Ray downRay = new Ray(rayEmitter.transform.position, -Vector3.up);
        if (Physics.Raycast(downRay, out hit))
        {
            float height = hit.distance;
            height = Mathf.Round(height * 100f) / 100f;
            heightCounter.text = (height + " Meters");

            //if (height > 3.5f) {
            //    VertEngine_FL.Stop ();
            //    VertEngine_FR.Stop ();
            //    VertEngine_RL.Stop ();
            //    VertEngine_RR.Stop ();
            //    flightModeHover = false;
            //}
        }
    }

    private void FlightModeControl()
    {
        //**************************
        //**************************
        //** FLIGHT MODE CONTROLS **
        //**************************
        //**************************


        float verticalValue = 0;
        vThrustCounter.text = (verticalValue * 100) + "%";
        KillVertEngines();

        float throttleValue = Input.GetAxisRaw("ForwardThrust");
        throttleValue = ((throttleValue + 1.0f) / 2.0f);
        if (throttleValue > 0)
        {
            throttleValue = Mathf.Round(throttleValue * 100f) / 100f;
            eagleRB.AddRelativeForce(-transform.forward * forwardThrust * throttleValue);
            MainEngine_Top.Play();
            MainEngine_Bottom.Play();
            MainEngine_Left.Play();
            MainEngine_Right.Play();
        }
        else
        {
            KillMainEngines();
        }

        float rollValue = Input.GetAxisRaw("Roll");
        if (rollValue != 0)
        {
            eagleRB.AddTorque(transform.forward * rollTorque * rollValue);
            if (rollValue > 0.05f)
            {
                //positive Roll
                RCS_FL_B.Play();
                RCS_RL_B.Play();
                RCS_FR_T.Play();
                RCS_RR_T.Play();
            }
            else if (rollValue < -0.05f)
            {
                //negative Roll
                RCS_FL_T.Play();
                RCS_RL_T.Play();
                RCS_FR_B.Play();
                RCS_RR_B.Play();
            }
            else
            {
                //No Roll
                StopAllRCS();
            }
        }

        float pitchValue = Input.GetAxisRaw("Pitch");
        if (pitchValue != 0)
        {
            eagleRB.AddTorque(transform.right * pitchTorque * (pitchValue * 2));
            if (pitchValue > 0.05f)
            {
                //positive pitch
                RCS_FL_B.Play();
                RCS_FR_B.Play();
                RCS_RL_T.Play();
                RCS_RR_T.Play();
            }
            else if (pitchValue < -0.05f)
            {
                //negative pitch
                RCS_FL_T.Play();
                RCS_FR_T.Play();
                RCS_RL_B.Play();
                RCS_RR_B.Play();
            }
            else
            {
                //No pitch
                StopAllRCS();
            }
        }

        float yawValue = Input.GetAxisRaw("Yaw");
        if (yawValue != 0)
        {
            eagleRB.AddTorque(transform.up * yawTorque * (yawValue * 2));
            if (yawValue > 0.5f)
            {
                //positive yaw
                RCS_FL_F.Play();
                RCS_FL_R.Play();
                RCS_FL_T.Play();
                RCS_FL_B.Play();
                RCS_RR_F.Play();
                RCS_RR_R.Play();
                RCS_RR_T.Play();
                RCS_RR_B.Play();
            }
            else if (yawValue < -0.5f)
            {
                //negative yaw
                RCS_FR_F.Play();
                RCS_FR_R.Play();
                RCS_FR_T.Play();
                RCS_FR_B.Play();
                RCS_RL_F.Play();
                RCS_RL_R.Play();
                RCS_RL_T.Play();
                RCS_RL_B.Play();
            }
            else
            {
                //No yaw
                StopAllRCS();
            }
        }

    }

    private void HoverModeControls()
    {
        //*************************
        //*************************
        //** HOVER MODE CONTROLS **
        //*************************
        //*************************


        KillMainEngines();

        flightModeIndicator.text = "Hover Mode";
        float verticalValue = Input.GetAxis("VerticalThrust");
        verticalValue = ((verticalValue + 1.0f) / 2.0f);
        if (verticalValue > 0)
        {
            verticalValue = Mathf.Round(verticalValue * 100f) / 100f;
            eagleRB.AddRelativeForce(transform.up * verticalThrust * verticalValue);
            VertEngine_FL.Play();
            VertEngine_FR.Play();
            VertEngine_RL.Play();
            VertEngine_RR.Play();
        }
        else
        {
            KillVertEngines();
        }
        vThrustCounter.text = (verticalValue * 100) + "%";

        float translateLRValue = Input.GetAxis("TranslateH");
        if (translateLRValue < 0)
        {
            eagleRB.AddRelativeForce(transform.right * translationLRValue);
            RCS_FR_F.Play();
            RCS_FR_R.Play();
            RCS_FR_T.Play();
            RCS_FR_B.Play();
            RCS_RR_F.Play();
            RCS_RR_R.Play();
            RCS_RR_T.Play();
            RCS_RR_B.Play();
        }
        else
        {
            RCS_FR_F.Stop();
            RCS_FR_R.Stop();
            RCS_FR_T.Stop();
            RCS_FR_B.Stop();
            RCS_RR_F.Stop();
            RCS_RR_R.Stop();
            RCS_RR_T.Stop();
            RCS_RR_B.Stop();

        }
        if (translateLRValue > 0)
        {
            eagleRB.AddRelativeForce(-transform.right * translationLRValue);
            RCS_FL_F.Play();
            RCS_FL_R.Play();
            RCS_FL_T.Play();
            RCS_FL_B.Play();
            RCS_RL_F.Play();
            RCS_RL_R.Play();
            RCS_RL_T.Play();
            RCS_RL_B.Play();
        }
        else
        {
            RCS_FL_F.Stop();
            RCS_FL_R.Stop();
            RCS_FL_T.Stop();
            RCS_FL_B.Stop();
            RCS_RL_F.Stop();
            RCS_RL_R.Stop();
            RCS_RL_T.Stop();
            RCS_RL_B.Stop();
        }

        float translateFBValue = Input.GetAxis("TranslateF");
        if (translateFBValue < 0)
        {
            eagleRB.AddRelativeForce(transform.forward * translationFBValue);
            RCS_FL_F.Play();
            RCS_FR_F.Play();
            RCS_RL_F.Play();
            RCS_RR_F.Play();
        }
        else
        {
            RCS_FL_F.Stop();
            RCS_FR_F.Stop();
            RCS_RL_F.Stop();
            RCS_RR_F.Stop();
        }
        if (translateFBValue > 0)
        {
            eagleRB.AddRelativeForce(-transform.forward * translationFBValue);
            RCS_FL_R.Play();
            RCS_FR_R.Play();
            RCS_RL_R.Play();
            RCS_RR_R.Play();
        }
        else
        {
            RCS_FL_R.Stop();
            RCS_FR_R.Stop();
            RCS_RL_R.Stop();
            RCS_RR_R.Stop();
        }


        if (Input.GetKey(KeyCode.JoystickButton15))
        {
            eagleRB.AddRelativeForce(transform.up * translationUDValue);
            RCS_FL_B.Play();
            RCS_FR_B.Play();
            RCS_RL_B.Play();
            RCS_RR_B.Play();
        }
        else
        {
            RCS_FL_B.Stop();
            RCS_FR_B.Stop();
            RCS_RL_B.Stop();
            RCS_RR_B.Stop();
        }

        if (Input.GetKey(KeyCode.JoystickButton17))
        {
            eagleRB.AddRelativeForce(-transform.up * translationUDValue);
            RCS_FL_T.Play();
            RCS_FR_T.Play();
            RCS_RL_T.Play();
            RCS_RR_T.Play();
        }
        else
        {
            RCS_FL_T.Stop();
            RCS_FR_T.Stop();
            RCS_RL_T.Stop();
            RCS_RR_T.Stop();
        }
    }

}


