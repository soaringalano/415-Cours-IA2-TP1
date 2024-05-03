using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FLS;
using FLS.Rules;

public class CarController : MonoBehaviour
{
    public float maxSteerAngle;

    public float maxMotorTorque;

    public float maxBrakeTorque;

    // 0 means very slippery 100 means very dry
    public float roadCondition;

    // 0 means heavy traffic 100 means no traffic
    public float trafficCondition;

    public WheelCollider frontLeftWheel, frontRightWheel, rearLeftWheel, rearRightWheel;

    void Awake()
    {
        FLSTorqueHelper.initializeFLS();
    }

    void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.W))
        {
            MoveForward();
        }
        else if(Input.GetKey(KeyCode.S))
        {
            MoveBackward();
        }
        if(Input.GetKey(KeyCode.A))
        {
            TurnLeft();
        }
        else if(Input.GetKey(KeyCode.D))
        {
            TurnRight();
        }
        if(Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            ResetSteerAngle();
        }
        if(Input.GetKey(KeyCode.Space))
        {
            Brake();
        }
    }

    void MoveForward()
    {
        float wheelTorque = FLSTorqueHelper.CalculateForwardMotorTorque(maxMotorTorque, roadCondition, trafficCondition);
        frontLeftWheel.motorTorque = wheelTorque;
        frontRightWheel.motorTorque = wheelTorque;
        rearLeftWheel.motorTorque = wheelTorque;
        rearRightWheel.motorTorque = wheelTorque;

        frontLeftWheel.brakeTorque = 0;
        frontRightWheel.brakeTorque = 0;
        rearLeftWheel.brakeTorque = 0;
        rearRightWheel.brakeTorque = 0;
    }

    void MoveBackward()
    {
        float wheelTorque = FLSTorqueHelper.CalculateBackwardMotorTorque(maxMotorTorque, roadCondition, trafficCondition);
        frontLeftWheel.motorTorque = -wheelTorque;
        frontRightWheel.motorTorque = -wheelTorque;
        rearLeftWheel.motorTorque = -wheelTorque;
        rearRightWheel.motorTorque = -wheelTorque;

        frontLeftWheel.brakeTorque = 0;
        frontRightWheel.brakeTorque = 0;
        rearLeftWheel.brakeTorque = 0;
        rearRightWheel.brakeTorque = 0;
    }

    void TurnLeft()
    {
        frontLeftWheel.steerAngle = -10;
        frontRightWheel.steerAngle = -10;
    }

    void TurnRight()
    {
        frontLeftWheel.steerAngle = 10;
        frontRightWheel.steerAngle = 10;
    }

    void Brake()
    {
        float brakeTorque = FLSTorqueHelper.CalculateBrakeTorque(maxBrakeTorque, roadCondition, trafficCondition);
        Debug.Log("Brake torque : " + brakeTorque);
        frontLeftWheel.brakeTorque = brakeTorque;
        frontRightWheel.brakeTorque = brakeTorque;
        rearLeftWheel.brakeTorque = brakeTorque;
        rearRightWheel.brakeTorque = brakeTorque;

        frontLeftWheel.motorTorque = 0;
        frontRightWheel.motorTorque = 0;
        rearLeftWheel.motorTorque = 0;
        rearRightWheel.motorTorque = 0;
    }

    void ResetSteerAngle()
    {
        frontLeftWheel.steerAngle = 0;
        frontRightWheel.steerAngle = 0;
    }

}
