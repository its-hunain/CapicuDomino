using UnityEngine;
using System.Collections;
using System;

public class Epoch {

	public static Epoch instance = new Epoch();

	public DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	public double serverTimeDiff = 0;

	public static double Now {
		get {
			double now = (DateTime.UtcNow - Epoch.instance.epochStart).TotalMilliseconds - Epoch.instance.serverTimeDiff;
			
			return now;
		}
	} //P.E.

	public static void UpdateDiff(double serverTime) {
		double localTime = (DateTime.UtcNow - Epoch.instance.epochStart).TotalMilliseconds;
		Epoch.instance.serverTimeDiff = localTime - serverTime;
	} //F.E.

	public static double SecondsElapsed(double t) {
		double difference = (Now - t) / 1000.0;

		if (difference < 0) {
			return 0;
		}

		return difference;
	} //F.E.

	public static double SecondsLeft(double t) {
		double difference = (t - Now) / 1000.0;

		if (difference < 0) {
			return 0;
		}

		return difference;
	} //F.E.

	public static float GetTimeDiffInSeconds(double currentActionTime)
	{
		var timeDiffInMilliseconds = Epoch.Now - currentActionTime;
		return (float)timeDiffInMilliseconds / 1000;
	}

} //CLS END