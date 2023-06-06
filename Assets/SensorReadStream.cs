using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class SensorReadStream
{
    private SerialPort sp;

    public SensorReadStream() {
        foreach (string mysps in SerialPort.GetPortNames()) {
            Debug.Log(mysps); 
            //if (mysps != "COM1") { the_com = mysps; break; }
        }
        // TODO
        sp = new SerialPort("/dev/tty.usbmodem11301", 115200);
        if (!sp.IsOpen) {
            Debug.Log("Opening " + "/dev/tty.usbmodem11301" + ", baud 115200");
            sp.Open();
            sp.ReadTimeout = 1000;
            sp.DtrEnable = true;
            sp.Handshake = Handshake.None;
            if (sp.IsOpen) { Debug.Log("Open"); }
        }
    }

    public string read() {
        // TODO figure out safer port open check mechanism
        // if (!sp.IsOpen) {
        //     sp.Open();
        //     print("opened sp");
        // }
        // if (sp.IsOpen) {
            string reading = "";
            try {
                reading = sp.ReadLine();
                //Debug.Log("Reading: " + reading);
                return reading;
            } 
            catch (System.TimeoutException e) {
                Debug.Log("timeout ------------------------------------------------------");
                throw new System.TimeoutException("SerialPort read timed out", e);
            } 
            catch (System.FormatException e) {
                Debug.Log("Input was not formatted correctly: " + reading);
                throw new System.FormatException("Input was not formatted correctly: " + reading, e);
            // }
        }
    }
}
