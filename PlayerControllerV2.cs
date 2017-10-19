using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;

public class PlayerController : MonoBehaviour
{

    private Rigidbody rb; //Rigid body component of player
    public float speed; //Used to control speed of player movement
    private int count; //Counter for objects picked uo
    public Text countText; //Text to display current count
    public Text winText; //Text at end game to display player win
    private Vector3 intialPosition; //Used to acquire initial position
    public static int ArduinoReadVal; //Used to get sensor status

    //Create a SerialPort object later used to communicate with Arduino.
    public static SerialPort arduinoPort;

    void Start()
    { //Executed in very first frame of the game

        rb = GetComponent<Rigidbody>(); //Set reference to rigid body
        intialPosition = transform.position; //Acquire initial player pos
        count = 0;
        SetCountText();
        winText.text = "";
        //Open serial connection; Delay read for 30 units
        OpenArduinoConnection();
    }

    void Update()
    { //Following code executed before rendering the frame



        //Try to check if button was pressed; otherwise catch exception
        //do nothing
        try
        {
            ArduinoReadVal = arduinoPort.ReadByte();

            if (ArduinoReadVal == 49)
            {
                transform.position = intialPosition;
            }
        }

        catch (System.Exception)
        {
        }

    }

    void FixedUpdate()
    { //Executed before any physics computations completed

        //Check how much to move in vertical and horizontal direction
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        //Get vector to define movement
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(movement * speed); //Add force for movement with speed
    }

    //If there is a collision determine if the object we've collided with
    //has tag "Pick Up"; Increment count and update screen text
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            count += 1;
            SetCountText();
        }
    }

    //Print to the game screen the current count; If player collected
    //all objects then print win message.
    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 7) winText.text = "You Win!";
    }

    /* //Check if input from Arduino is 1, if so, it means button pressed
    //so reset player position to starting point
    void resetBallPosition(char resetPos) {
        if (resetPos == '1') {
            transform.position = intialPosition;
        } 
    } */

    public void OpenArduinoConnection()
    {
        //Opens connection to Arduino
        arduinoPort = new SerialPort("COM1", 9600); //Port name and baud rate for Arduino

        Debug.Log("Arduino Connection Starting..");

        if (arduinoPort != null)
        { //Check if objected successfully created

            if (arduinoPort.IsOpen)
            { //Check if port is already open
                arduinoPort.Close();
                Debug.Log("Closing port because it was already opened...");
            }

            else
            {
                arduinoPort.Open(); //Open the connection
                arduinoPort.ReadTimeout = 25; //Set number of ms to wait before time-out error
                Debug.Log("Arduino connected!");
            }
        }

        else
        {

            Debug.Log("Error: Port == Null");
        }
    }

    void OnApplicationQuit()
    {
        if (arduinoPort != null)
        {
            arduinoPort.Close();
            Debug.Log("Arduino port closed!");
        }
    }

}
