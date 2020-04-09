# Controllers
Controllers

## DevKit Arduino
The development version will use [Arduino Nano BLE 33 Sense](https://store.arduino.cc/arduino-nano-33-ble-sense)
The board is equiped with USB, BlueTooth, IMU (accelerometer, gyroscope, magnetometer) in a very small form factor so it is a good fit.

### Accelerometer
The accelerometer on board is [LSM9DS1](https://content.arduino.cc/assets/Nano_BLE_Sense_lsm9ds1.pdf).
It is programmed using the [Arduino LSM9DS1 Library](https://www.arduino.cc/en/Reference/ArduinoLSM9DS1)

> TRICKY: The coordinate system of the sensor's magnetometer is misaligned with the accel and gyro.

The library does not provide a way to convert the IMU data to AHRS (High Accuracy Attitude Sensor) for this sensor, although Arduino has libraries that work with other IMUs. 
For the initial versions use [Madgwick's algorithm](https://x-io.co.uk/open-source-imu-and-ahrs-algorithms/).

There are other sources that use [IMU for AHRS](https://github.com/kriswiner/LSM9DS1/blob/master/LSM9DS1_MS5611_BasicAHRS_t3.ino)

There may be left <-> right hand coordinate system conversion when getting input to Unity. [See converting quaternions.](https://gamedev.stackexchange.com/questions/157946/converting-a-quaternion-in-a-right-to-left-handed-coordinate-system)

The magnetometer readings from the Arduino IMU are not calibrated, this should be done manually, [here is a nice read about magnetometer callibration](https://appelsiini.net/2018/calibrate-magnetometer/).
