using FLS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FLS.Rules;
using System;

public class FLSTorqueHelper : MonoBehaviour
{

    public static FLSTorqueHelper Instance;

    //private static LinguisticVariable motorTorque, roadCondition, brakeTorque;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            initializeFLS();
        }
        else
        {
            Destroy(this);
        }
    }

    private IFuzzyEngine fuzzyEngine;

    public void initializeFLS()
    {

        var motorTorque = new LinguisticVariable("motorTorque");
        var lowAcceleration = motorTorque.MembershipFunctions.AddTrapezoid("lowAcceleration", 0, 0, 20, 40);
        var mediumAcceleration = motorTorque.MembershipFunctions.AddTriangle("mediumAcceleration", 30, 50, 70);
        var highAcceleration = motorTorque.MembershipFunctions.AddTrapezoid("highAcceleration", 50, 80, 100, 100);


        /// ¨¦tat de la rue 
        var roadCondition = new LinguisticVariable("roadCondition");
        var badCondition = roadCondition.MembershipFunctions.AddTrapezoid("dryRoad", 0, 0, 20, 40);
        var midCondition = roadCondition.MembershipFunctions.AddTriangle("humidRoad", 30, 50, 70);
        var goodCondition = roadCondition.MembershipFunctions.AddTrapezoid("slipperyRoad", 60, 80, 100, 100);


        /// Cons¨¦quents (Outputs) : variables linguistiques & Fuzzification
        var brakeTorque = new LinguisticVariable("brakeTorque");
        var lowBrake = brakeTorque.MembershipFunctions.AddTriangle("lowBrake", 0, 8, 10);
        var midBrake = brakeTorque.MembershipFunctions.AddTriangle("midBrake", 8, 11, 15);
        var highBrake = brakeTorque.MembershipFunctions.AddTriangle("highBrake", 12, 16, 20);

        /// Base de regles: d¨¦finition des regles
        var rule1 = Rule.If(motorTorque.Is(lowAcceleration).Or(motorTorque.Is(mediumAcceleration))).Then(brakeTorque.Is(lowBrake));
        var rule2 = Rule.If(motorTorque.Is(highAcceleration)).Then(brakeTorque.Is(highBrake));
        var rule3 = Rule.If(roadCondition.Is(badCondition)).Then(brakeTorque.Is(highBrake));
        var rule4 = Rule.If(roadCondition.Is(midCondition)).Then(brakeTorque.Is(midBrake));
        var rule5 = Rule.If(roadCondition.Is(goodCondition)).Then(brakeTorque.Is(lowBrake));

        /// Instanciation du moteur de fuzzification
        /// Application des regles & inf¨¦rences
        fuzzyEngine = new FuzzyEngineFactory().Default();
        fuzzyEngine.Rules.Add(rule1, rule2, rule3, rule4, rule5);

    }

    public float CalculateBrakeTorque(float motorTorque, float roadCondition)
    {
        return (float)fuzzyEngine.Defuzzify(new {motorTorque = (double)motorTorque, roadCondition = (double)roadCondition});
    }

    public float CalculateForwardMotorTorque(float maxMotorTorque, float minMotorTorque, float roadCondition, float trafficCondition, float accelerationDuration)
    {

        return 0;
    }

    public float CalculateBackwardMotorTorque(float maxMotorTorque, float minMotorTorque, float roadCondition, float trafficCondition, float accelerationDuration)
    {

        return 0;
    }
}
