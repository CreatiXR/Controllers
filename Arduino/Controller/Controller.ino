#include <Arduino_LSM9DS1.h>

void setup() {
  Serial.begin(9600);
  while (!Serial);

  if (!IMU.begin()) {
    Serial.println("ERROR: Failed to initialize IMU!");
    while (1);
  }
}

void loop() {
  float x, y, z;
  if (IMU.gyroscopeAvailable()) {
    // Degrees per second...
    IMU.readGyroscope(x, y, z);

    Serial.print("GYRO: ");
    Serial.print(micros());
    Serial.print(" ");
    Serial.print(x);
    Serial.print(" ");
    Serial.print(y);
    Serial.print(" ");
    Serial.print(z);
    Serial.println();
  }

  if (IMU.magneticFieldAvailable()) {
    IMU.readMagneticField(x, y, z);

    Serial.print("MAGNE: ");
    Serial.print(micros());
    Serial.print(" ");
    Serial.print(x);
    Serial.print(" ");
    Serial.print(y);
    Serial.print(" ");
    Serial.print(z);
    Serial.println();
  }

  if (IMU.accelerationAvailable()) {
    IMU.readAcceleration(x, y, z);

    Serial.print("ACCEL: ");
    Serial.print(micros());
    Serial.print(" ");
    Serial.print(x);
    Serial.print(" ");
    Serial.print(y);
    Serial.print(" ");
    Serial.print(z);
    Serial.println();
  }
}
