using UnityEngine;
using System.IO.Ports;

public class ArduinoComms : MonoBehaviour
{
    SerialPort port;

    [SerializeField]
    Transform accelerometer;

    [SerializeField]
    Transform magentometer;

    [SerializeField]
    Transform controller;

    private float minMagneX = +1000;
    private float maxMagneX = -1000;

    private float minMagneY = +1000;
    private float maxMagneY = -1000;

    private float minMagneZ = +1000;
    private float maxMagneZ = -1000;

    Vector3 magnetometerVector;
    Vector3 accelerometerVector;

    void Start()
    {
        System.ComponentModel.IContainer components =
            new System.ComponentModel.Container();
        port = new System.IO.Ports.SerialPort(components);
        port.PortName = "COM4";
        port.BaudRate = 9600;
        port.DtrEnable = true;
        port.ReadTimeout = 0;
        port.WriteTimeout = 0;
        port.Open();
    }

    float lastGyroReadTime = float.NaN;

    void Update()
    {
        try
        {
            while (true)
            {
                var line = port.ReadLine();

                if (line.StartsWith("MAGNE: "))
                {
                    var tokens = line.Split(' ');
                    var time = float.Parse(tokens[1].Substring(0, tokens[1].Length - 2)) / 1000f;
                    var sensorX = float.Parse(tokens[2]);
                    var sensorY = float.Parse(tokens[3]);
                    var senzorZ = float.Parse(tokens[4]);

                    // Magnetometer calibration...
                    minMagneX = Mathf.Min(minMagneX, sensorX);
                    maxMagneX = Mathf.Max(maxMagneX, sensorX);

                    minMagneY = Mathf.Min(minMagneY, sensorY);
                    maxMagneY = Mathf.Max(maxMagneY, sensorY);

                    minMagneZ = Mathf.Min(minMagneZ, senzorZ);
                    maxMagneZ = Mathf.Max(maxMagneZ, senzorZ);

                    var offsetX = (maxMagneX + minMagneX) / 2;
                    var offsetY = (maxMagneY + minMagneY) / 2;
                    var offsetZ = (maxMagneZ + minMagneZ) / 2;

                    var averageDeltaX = (maxMagneX - minMagneX) / 2f;
                    var averageDeltaY = (maxMagneY - minMagneY) / 2f;
                    var averageDeltaZ = (maxMagneZ - minMagneZ) / 2f;

                    var averageOffset = (averageDeltaX + averageDeltaY + averageDeltaZ) / 3f;
                    var scaleX = averageOffset / averageDeltaX;
                    var scaleY = averageOffset / averageDeltaY;
                    var scaleZ = averageOffset / averageDeltaZ;

                    var calibratedX = (sensorX - offsetX) * scaleX;
                    var calibratedY = (sensorY - offsetY) * scaleY;
                    var calibratedZ = (senzorZ - offsetZ) * scaleZ;

                    var unityX = calibratedX;
                    var unityY = calibratedZ;
                    var unityZ = calibratedY;

                    this.magnetometerVector = new Vector3(unityX, unityY, unityZ);
                    if (!float.IsNaN(magnetometerVector.x) && !float.IsNaN(magnetometerVector.y) && !float.IsNaN(magnetometerVector.z))
                    this.magentometer.position = this.magnetometerVector * 0.003f;
                }

                if (line.StartsWith("ACCEL: "))
                {
                    var tokens = line.Split(' ');
                    var time = float.Parse(tokens[1].Substring(0, tokens[1].Length - 2)) / 1000f;
                    var x = float.Parse(tokens[2]);
                    var y = float.Parse(tokens[3]);
                    var z = float.Parse(tokens[4]);

                    var unityX = y;
                    var unityY = z;
                    var unityZ = x;

                    this.accelerometerVector = new Vector3(unityX, unityY, unityZ);
                    this.accelerometer.position = accelerometerVector * 0.2f;
                }

                if (line.StartsWith("GYRO: "))
                {
                    var tokens = line.Split(' ');
                    // microseconds controller app time
                    var time = float.Parse(tokens[1]);
                    // degrees per second
                    var x = float.Parse(tokens[2]);
                    var y = float.Parse(tokens[3]);
                    var z = float.Parse(tokens[4]);
                    // seconds since last gyro reading
                    var delta = (time - lastGyroReadTime) / 1000000f;
                    if (!float.IsNaN(this.lastGyroReadTime))
                    {
                        this.controller.rotation *= Quaternion.Euler(y * delta, -z * delta, x * delta);
                    }
                    this.lastGyroReadTime = time;
                }

                // TODO: Use the acceleromater and magnetometer to compensate gyro drift.
            }
        }
        catch (System.Exception)
        {
        }
    }
}