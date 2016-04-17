using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace UnityDeviceBridge {
	public enum BatteryState {
		Unknown,
		Unplugged,
		Charging,
		Full,
	}

	public static class Battery {
		#if UNITY_IOS
		// iOS C interfaces
		[DllImport("__Internal")]
		private static extern bool udb_getBatteryMonitoringEnabled();
		[DllImport("__Internal")]
		private static extern void udb_setBatteryMonitoringEnabled(bool isBatteryMonitoringEnabled);
		[DllImport("__Internal")]
		private static extern float udb_getBatteryLevel();
		[DllImport("__Internal")]
		private static extern int udb_getBatteryState();
		#endif

		public static bool IsMonitoringEnabled {
			get {
				#if UNITY_IOS
				if(!Application.isEditor) {
					return udb_getBatteryMonitoringEnabled();
				} else {
					return false;
				}
				#elif UNITY_ANDROID
				return true;
				#else
				return false;
				#endif
			}
			set {
				#if UNITY_IOS
				if(!Application.isEditor) {
					udb_setBatteryMonitoringEnabled(value);
				}
				#endif
			}
		}

		public static void getLevelAndState(out float batteryLevel, out BatteryState batteryState) {
			batteryLevel = -1.0f;
			batteryState = BatteryState.Unknown;

			#if UNITY_IOS
			if(!Application.isEditor) {
				batteryLevel = udb_getBatteryLevel();
				batteryState = (BatteryState)udb_getBatteryState();
			}
			#elif UNITY_ANDROID
			if(!Application.isEditor) {
				using(var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				using(var currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"))
				using(var intentFilter = new AndroidJavaObject("android.content.IntentFilter", "android.intent.action.BATTERY_CHANGED"))
				using(var batteryIntent = currentActivity.Call<AndroidJavaObject>("registerReceiver", null, intentFilter)) {
					var level = batteryIntent.Call<int>("getIntExtra", "level", -1);
					var scale = batteryIntent.Call<int>("getIntExtra", "scale", -1);
					if(level >= 0 && scale > 0) {
						batteryLevel = level / (float)scale;
					} else {
						batteryLevel = -1.0f;
					}

					var state = batteryIntent.Call<int>("getIntExtra", "status", -1);
					if(state == 2) {
						// BATTERY_STATUS_CHARGING
						batteryState = BatteryState.Charging;
					} else if(state == 3) {
						// BATTERY_STATUS_DISCHARGING
						batteryState = BatteryState.Unplugged;
					} else if(state == 4) {
						// BATTERY_STATUS_NOT_CHARGING
						batteryState = BatteryState.Unplugged;
					} else if(state == 5) {
						// BATTERY_STATUS_FULL
						batteryState = BatteryState.Full;
					} else {
						batteryState = BatteryState.Unknown;
					}
				}
			}
			#endif
		}
	}
}