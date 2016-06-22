using UnityEngine;
using System.Collections;

public class SeaHorseMovement : MonoBehaviour
{
    Quaternion r;
    Vector3 tempVelocity, velocity, tempHeading, SteeringForceSum, yRotationRight, yRotationLeft, xRotateDown, xRotateUp, startRotation;
    Quaternion sumUpVector, sumDownVector, sumRightVector, sumLeftVector;
    float VehicleMass, MaxSpeed = 150f;
    bool buttonDown;

    // Use this for initialization
    void Start()
    {
        tempVelocity = new Vector3(0f, 0f, 0f);
        yRotationRight = new Vector3(0f, 5f, 0f);
        yRotationLeft = new Vector3(0f, -5f, 0f);
        xRotateUp = new Vector3(5f, 0f, 0f);
        xRotateDown = new Vector3(-5f, 0f, 0f);

        velocity = Vector3.zero;
        VehicleMass = 1f;
        tempHeading = Vector3.zero;
        SteeringForceSum = Vector3.zero;
        buttonDown = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        SteeringForceSum = Vector3.zero;

        if (Input.GetKey("w"))
        {
            //transform.Rotate(Vector3.forward * 5f, Space.Self);
            transform.Translate(Vector3.left * 5f, Space.Self);
          
        }
        if (Input.GetKey("a"))
        {
           
            transform.Rotate(yRotationLeft, Space.Self);
                 
        }
        if (Input.GetKey("d"))
        {
            buttonDown = true;
            transform.Rotate(yRotationRight, Space.Self);
           
        }

        if (Input.GetKey("s"))
        {
            //transform.Rotate(Vector3.back * 5f, Space.Self);
            transform.Translate(Vector3.right * 5f, Space.Self);
 
           
        }

        if (Input.GetKey("v"))
        {
           // transform.Rotate(xRotateUp, Space.Self);
      
              transform.Translate(Vector3.down * 5f, Space.Self);
           
        }

        if (Input.GetKey("space"))
        {
           // transform.Rotate(xRotateDown, Space.Self);
           
              transform.Translate(Vector3.up * 5f, Space.Self);
            
        }
        
    }

    public void translatePosition()
    {
        tempVelocity = Vector3.zero;

        //Vector3 OldPos = m_Vehicles [i].transform.position;
        ////Debug.Log (SteeringForceSum); //these forces seem correct.
        Vector3 acceleration = SteeringForceSum / VehicleMass;
        // Debug.Log(acceleration * Time.deltaTime);
        tempVelocity += acceleration * Time.deltaTime;  //what is value of Time.deltaTime vs netbeans time function???
                                                        ////Debug.Log (tempHeadingOne); //this value of m_vVelocity is still 0,0,0 ...?

        //if (tempScript.getDolphin ())
        //	tempVelocity = tempVelocity;

        if (tempVelocity.magnitude > MaxSpeed)
        {
            Vector3 tempV = tempVelocity.normalized;
            tempVelocity = tempV * MaxSpeed;

        }

        //m_Vehicles [i].GetComponent<BallBounce> ().setVelocity (tempVelocity); //should you normalize m_vVelocity before setting it in fish?
        setVelocity(tempVelocity);

        //m_Vehicles [i].transform.Translate (tempHeadingOne, Space.Self); //this seems to work better.  
        Vector3 tempPos = new Vector3(0.0f, 0.0f, 0.0f);

        tempPos = transform.position;
        tempPos += tempVelocity * Time.deltaTime * 50.0f;//Time.deltaTime is what's added from original code. 


        transform.position = tempPos;
        //    }

        if (tempVelocity.magnitude > 0.00000001)
        {
            tempHeading = tempVelocity.normalized;
            setHeading(tempHeading);
        }



        if (tempVelocity != Vector3.zero)
        {
            Quaternion r = Quaternion.LookRotation(tempVelocity, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, r, .5f);
        }

        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;


        if (transform.position.x < 0f)
        {
            x = 1f;
        }

        if (transform.position.x > 1000f)
        {
            x = 999f;
        }

        if (transform.position.y < 0f)
        {
            y = 1f;
        }

        if (transform.position.y > 1000f)
        {
            y = 999f;
        }

        if (transform.position.z < 0f)
        {
            z = 1f;
        }

        if (transform.position.z > 1000f)
        {
            z = 999f;
        }


        Vector3 inBoundLocation = new Vector3(x, y, z);
        transform.position = inBoundLocation;

        //tempScript.setVelocity (Vector3.zero);


    }

    public void setVelocity(Vector3 vel)
    {
        velocity = vel;
    }

    public Vector3 getVelocity()
    {
        return velocity;
    }

    public void setHeading(Vector3 heading)
    {
        tempHeading = heading;
    }

    public Vector3 getHeading()
    {
        return tempHeading;
    }

    public Vector3 getPosition()
    {
        return transform.position;
    }

    public Quaternion getRotation()
    {
        return transform.rotation;
    }
}
