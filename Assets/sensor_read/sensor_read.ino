#include <ArduinoJson.h>
#include <ArduinoJson.hpp>

// SPDX-FileCopyrightText: 2020 Carter Nelson for Adafruit Industries
//
// SPDX-License-Identifier: MIT
//
#include <Adafruit_APDS9960.h>
#include <Adafruit_BMP280.h>
#include <Adafruit_LIS3MDL.h>
#include <Adafruit_LSM6DS33.h>
#include <Adafruit_SHT31.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_BNO055.h>
#include <PDM.h>
#include <utility/imumaths.h>

/* Set the delay between fresh samples */
#define BNO055_SAMPLERATE_DELAY_MS (100)

// TODO: use temp, proximity, humidity, imu; comment out rest
Adafruit_APDS9960 apds9960; // proximity, light, color, gesture
Adafruit_BMP280 bmp280;     // temperature, barometric pressure
// Adafruit_LIS3MDL lis3mdl;   // magnetometer
// Adafruit_LSM6DS33 lsm6ds33; // accelerometer, gyroscope
Adafruit_SHT31 sht30;       // humidity
// by default address is 0x29 or 0x28)   id, address
Adafruit_BNO055 bno = Adafruit_BNO055(12345, 0x28, &Wire);

uint8_t proximity;
float temperature, altitude;
float humidity;

void setup(void) {
  Serial.begin(115200);
  // while (!Serial) delay(10);
  // Serial.println("Feather Sense Sensor Demo");
  Serial.flush();

  // initialize the sensors
  apds9960.begin();
  apds9960.enableProximity(true);
  bmp280.begin();
  sht30.begin();

  /* Initialise the sensor */
  if(!bno.begin())
  {
    /* There was a problem detecting the BNO055 ... check your connections */
    Serial.print("Oops, no BNO055 detected ... Check your wiring or I2C ADDR!");
    while(1);
  }
  bno.setExtCrystalUse(true);
  
}

void loop(void) {
  proximity = apds9960.readProximity();
  temperature = bmp280.readTemperature();
  altitude = bmp280.readAltitude(1013.25);
  humidity = sht30.readHumidity();

  sensors_event_t orient;
  bno.getEvent(&orient);

  //TODO: output in JSON format
  

  const int capacity = JSON_OBJECT_SIZE(6) + 3*JSON_OBJECT_SIZE(1) + 2*JSON_OBJECT_SIZE(1); 
  StaticJsonDocument<capacity> outputjson;

  JsonArray gridPos = outputjson.createNestedArray("gridPos");
  gridPos.add(0);
  gridPos.add(0);

  outputjson["prox"] = proximity;
  outputjson["temp"] = temperature;
  outputjson["alt"] = altitude;
  outputjson["hum"] = humidity;

  JsonArray orientarray = outputjson.createNestedArray("orient");
  orientarray.add(orient.orientation.x);
  orientarray.add(orient.orientation.y);
  orientarray.add(orient.orientation.z);

  // Serial.println("\nFeather Sense Sensor Demo");
  // Serial.println("---------------------------------------------");

  // Serial.print("Proximity: ");
  // Serial.println(proximity);

  // Serial.print("Temperature: ");
  // Serial.print(temperature);
  // Serial.println(" C");

  // Serial.print("Altitude: ");
  // Serial.print(altitude);
  // Serial.println(" m");
  
  // Serial.print("Humidity: ");
  // Serial.print(humidity);
  // Serial.println(" %");

  // Serial.print("X: ");
  // Serial.print(orient.orientation.x, 4);
  // Serial.print("\tY: ");
  // Serial.print(orient.orientation.y, 4);
  // Serial.print("\tZ: ");
  // Serial.println(orient.orientation.z, 4);

  Serial.flush();
  serializeJson(outputjson, Serial);
  Serial.println();
  // serializeJsonPretty(outputjson, Serial);
  delay(100);
}

/*****************************************************************/
/* int32_t getPDMwave(int32_t samples) {
  short minwave = 30000;
  short maxwave = -30000;

  while (samples > 0) {
    if (!samplesRead) {
      yield();
      continue;
    }
    for (int i = 0; i < samplesRead; i++) {
      minwave = min(sampleBuffer[i], minwave);
      maxwave = max(sampleBuffer[i], maxwave);
      samples--;
    }
    // clear the read count
    samplesRead = 0;
  }
  return maxwave - minwave;
}



void onPDMdata() {
  // query the number of bytes available
  int bytesAvailable = PDM.available();

  // read into the sample buffer
  PDM.read(sampleBuffer, bytesAvailable);

  // 16-bit, 2 bytes per sample
  samplesRead = bytesAvailable / 2;
}
*/
