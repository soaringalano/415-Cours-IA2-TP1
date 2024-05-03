using FLS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FLS.Rules;
using System;

public class FLSTorqueHelper
{

    public static FLSTorqueHelper Instance = new FLSTorqueHelper();

    private static FuzzyEngineFactory fuzzyEngineFactory = new FuzzyEngineFactory();

    private static IFuzzyEngine brakeFuzzyEngine;

    private static IFuzzyEngine forwardMotorFuzzyEngine;

    public static void initializeFLS()
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
        var lowBrake = brakeTorque.MembershipFunctions.AddTriangle("lowBrake", 0, 8, 15);
        var midBrake = brakeTorque.MembershipFunctions.AddTriangle("midBrake", 10, 16, 21);
        var highBrake = brakeTorque.MembershipFunctions.AddTriangle("highBrake", 18, 24, 30);

        /// ¨¦tat du traffic
        var trafficCondition = new LinguisticVariable("trafficCondition");
        var lowTraffic = trafficCondition.MembershipFunctions.AddTrapezoid("lowTraffic", 0, 0, 20, 40);
        var mediumTraffic = trafficCondition.MembershipFunctions.AddTriangle("mediumTraffic", 30, 50, 70);
        var highTraffic = trafficCondition.MembershipFunctions.AddTrapezoid("highTraffic", 60, 80, 100, 100);

        /// Base de regles: d¨¦finition des regles
        var accRule1 = Rule.If(motorTorque.Is(lowAcceleration)).Then(brakeTorque.Is(lowBrake));
        var accRule2 = Rule.If(motorTorque.Is(mediumAcceleration)).Then(brakeTorque.Is(midBrake));
        var accRule3 = Rule.If(motorTorque.Is(highAcceleration)).Then(brakeTorque.Is(highBrake));

        var roadRule1 = Rule.If(roadCondition.Is(badCondition)).Then(brakeTorque.Is(highBrake));
        var roadRule2 = Rule.If(roadCondition.Is(midCondition)).Then(brakeTorque.Is(midBrake));
        var roadRule3 = Rule.If(roadCondition.Is(goodCondition)).Then(brakeTorque.Is(lowBrake));

        var trafficRule1 = Rule.If(trafficCondition.Is(lowTraffic)).Then(brakeTorque.Is(lowBrake));
        var trafficRule2 = Rule.If(trafficCondition.Is(mediumTraffic)).Then(brakeTorque.Is(midBrake));
        var trafficRule3 = Rule.If(trafficCondition.Is(highTraffic)).Then(brakeTorque.Is(highBrake));


        /// Instanciation du moteur de fuzzification
        /// Application des regles & inf¨¦rences
        brakeFuzzyEngine = fuzzyEngineFactory.Default();
        brakeFuzzyEngine.Rules.Add(accRule1, accRule2, accRule3, roadRule1, roadRule2, roadRule3, trafficRule1, trafficRule2, trafficRule3);

        var forwardRule1 = Rule.If(roadCondition.Is(badCondition)).Then(motorTorque.Is(lowAcceleration));
        var forwardRule2 = Rule.If(roadCondition.Is(midCondition)).Then(motorTorque.Is(mediumAcceleration));
        var forwardRule3 = Rule.If(roadCondition.Is(goodCondition)).Then(motorTorque.Is(highAcceleration));
        var forwardRule4 = Rule.If(trafficCondition.Is(lowTraffic)).Then(motorTorque.Is(highAcceleration));
        var forwardRule5 = Rule.If(trafficCondition.Is(mediumTraffic)).Then(motorTorque.Is(mediumAcceleration));
        var forwardRule6 = Rule.If(trafficCondition.Is(highTraffic)).Then(motorTorque.Is(lowAcceleration));

        forwardMotorFuzzyEngine = fuzzyEngineFactory.Default();
        forwardMotorFuzzyEngine.Rules.Add(forwardRule1, forwardRule2, forwardRule3, forwardRule4, forwardRule5, forwardRule6);

    }

    public static float CalculateBrakeTorque(float motorTorque, float roadCondition, float trafficCondition)
    {
        return (float)brakeFuzzyEngine.Defuzzify(new {motorTorque = (double)motorTorque, roadCondition = (double)roadCondition, trafficCondition = (double)trafficCondition});
    }

    public static float CalculateForwardMotorTorque(float motorTorque, float roadCondition, float trafficCondition)
    {
        return (float)forwardMotorFuzzyEngine.Defuzzify(new { motorTorque = (double)motorTorque, roadCondition = (double)roadCondition, trafficCondition = (double)trafficCondition });
    }

    public static float CalculateBackwardMotorTorque(float maxMotorTorque, float roadCondition, float trafficCondition)
    {

        return CalculateForwardMotorTorque(maxMotorTorque, roadCondition, trafficCondition) / 2;
    }
}
