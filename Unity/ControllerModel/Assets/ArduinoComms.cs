using UnityEngine;
using System.IO.Ports;

public class ArduinoComms : MonoBehaviour
{
    SerialPort port;

    [SerializeField]
    Transform upTransform;

    [SerializeField]
    Transform northTransform;

    [SerializeField]
    Transform eastTransform;

    [SerializeField]
    Transform accelMagneController;

    [SerializeField]
    Transform gytoController;

    [SerializeField]
    Transform fusedController;

    [SerializeField]
    Vector3 offset = Vector3.zero;

    [SerializeField]
    Quaternion gameAreaOrientation = Quaternion.identity;

    [SerializeField]
    float driftCompensationRatio = 0.02f;

    private float minMagneX = +1000;
    private float maxMagneX = -1000;

    private float minMagneY = +1000;
    private float maxMagneY = -1000;

    private float minMagneZ = +1000;
    private float maxMagneZ = -1000;

    private Vector3 magnetometerCalibrated;

    private Vector3 east;
    private Vector3 north;
    private Vector3 up;

    float lastGyroReadTime = float.NaN;

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

                    var x = float.Parse(tokens[2]);
                    var y = float.Parse(tokens[3]);
                    var z = float.Parse(tokens[4]);

                    this.magnetometerCalibrated = new Vector3(-y, z, x) + offset;
                    this.north = Vector3.ProjectOnPlane(this.magnetometerCalibrated, this.up);
                    this.northTransform.position = this.north.normalized * 0.35f;
                }

                if (line.StartsWith("ACCEL: "))
                {
                    var tokens = line.Split(' ');
                    var time = float.Parse(tokens[1].Substring(0, tokens[1].Length - 2)) / 1000f;
                    var x = float.Parse(tokens[2]);
                    var y = float.Parse(tokens[3]);
                    var z = float.Parse(tokens[4]);
                    this.up = new Vector3(-y, z, -x);
                    this.upTransform.position = this.up.normalized * 0.35f;
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
                        this.gytoController.rotation *= Quaternion.Euler(y * delta, -z * delta, x * delta);
                        this.fusedController.rotation *= Quaternion.Euler(y * delta, -z * delta, x * delta);
                    }
                    this.lastGyroReadTime = time;
                }
            }
        }
        catch (System.Exception)
        {
        }

        // TODO: Use the acceleromater and magnetometer to compensate gyro drift.
        this.east = Vector3.Cross(this.up, this.north);
        eastTransform.position = east.normalized * 0.35f;

        var accelMagneRotation = Quaternion.Inverse(Quaternion.LookRotation(this.east, this.up));
        this.accelMagneController.rotation = accelMagneRotation;

        this.fusedController.rotation = Quaternion.Lerp(this.fusedController.rotation, gameAreaOrientation * accelMagneRotation, this.driftCompensationRatio);
    }
}