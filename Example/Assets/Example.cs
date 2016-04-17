using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Example : MonoBehaviour {
	[SerializeField] private Text logLabel;
	private Coroutine _checkBattery = null;

	// Use this for initialization
	void Start () {
		UnityDeviceBridge.Battery.IsMonitoringEnabled = true;
		_checkBattery = StartCoroutine(CheckBattery());
	}

	// Update is called once per frame
	void Update () {
	
	}

	void OnDestroy() {
		if(_checkBattery != null) {
			StopCoroutine(_checkBattery);
			_checkBattery = null;
		}
		UnityDeviceBridge.Battery.IsMonitoringEnabled = false;
	}

	IEnumerator CheckBattery() {
		while(true) {
			var batteryLevel = 0.0f;
			var batteryState = UnityDeviceBridge.BatteryState.Unknown;
			UnityDeviceBridge.Battery.getLevelAndState(out batteryLevel, out batteryState);
			logLabel.text = string.Format("CheckBattery - batteryLevel={0}, batteryState={1}", batteryLevel, batteryState) + "\n" + logLabel.text;
			yield return new WaitForSeconds(1.0f);
		}
	}
}
