using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;


public class CellSpacePartition : MonoBehaviour {
	
	public GameObject goldFish, Dolfin, dolfinUsing, SeaHorse;
	public 	float wanderAngle = 5f, m_dWanderDistance = 2.0f, m_dWanderRadius = 1.2f, m_dWanderJitter = 80.0f;

	public Dictionary<string, double> domDictionary = new Dictionary<string, double>();
	public double[] tempDouble = new double[2];

    private CellSpacePartition m_pCellSpace;
    private BoundingCircle QueryCircle;
    public Eat cp;
    private BallBounce tempScript;

    public double m_dViewDistance;

	public bool fuzzifyInUse;
    private bool WallCollision;
    public bool anotherFish;

    private string fileName = "";

	private Animation anim;

	public Text MateText, PredatorText, PreyText, BabyText;

	public int mateCount, predatorCount, preyCount, babyCount;

    //public GameObject plant;
	public GameObject stone, stone1by2, stone2by3, stone2by4, stone5by5, stone5by6;
	
	public GameObject seaweed, seaweed5by5, seaweed5by8, coral1, coral2, coral4, seaShell1, seaShell6, seaShell11,
        sponge1, sponge2by2, sponge3by3;
	
	
	public GameObject fish, fishDolphin;
	public Material fishMaterial, mateMaterial;
	
	public List<Cell> m_Cells = new List<Cell>();
	private List<GameObject> m_Neighbors;
    public List<GameObject> m_Vehicles = new List<GameObject>(), m_WanderList = new List<GameObject>(), m_PlantList = new List<GameObject>(),
     m_RedFish = new List<GameObject>();

	private double m_dSpaceWidth = 50, m_dSpaceHeight = 50, m_dSpaceDepth = 50;

    private int NumCellsX = 10, NumCellsY = 10, NumCellsZ = 10, NumAgents = 50, counter, TempQueryRadius;//why do all spheres go to x-z positive corner?
	private double cx = 1000, cy = 1000, cz = 1000, m_dCellSizeX, m_dCellSizeY, m_dCellSizeZ;

    private int m_iNumCellsX, m_iNumCellsY, m_iNumCellsZ, MaxEntities = 300, state;
	
	private Vector3 FeelerU, FeelerD, FeelerR, FeelerL, FeelerF;
	private Vector3 FeelerUNormal, FeelerDNormal, FeelerRNormal, FeelerLNormal, FeelerFNormal;
	private float DistToClosestIP, DistToThisIP;
	private Vector3 ClosestPoint, TempTargetPos, Overshoot;

	private enum feeler { forward, up, down, left, right};
	private int caseSwitch;
	private float rayLength;
	
	public GameObject container;//do I need this?
	
	
	private float MaxSpeed;// = 150.0f;  What are good values for this and MaxSteeringForce?
	private float MaxSteeringForce;
	private Vector3 SteeringForceSum;
	private float VehicleMass, VehicleScale;
	private Vector3 m_vVelocity, Force;
	private float SeparationWeight, CohesionWeight, AlignmentWeight, ObstacleAvoidanceWeight;
	
	private Vector3 tempVector, tempHeadingOne, tempHeadingTwo, hitNormal, tempHeading, tempVelocity;
	
	public Vector3 DolphinVel;
	
	public enum State {
		Flock,
		Eat,
		Flee,
		Mate,
		Wander,
		Spawn,
		Dead
	};
	
	public enum Deceleration {
		
		slow = 3, normal = 2, fast = 1
		
	};
	
	Deceleration dec;
	
	
	private Vector3 m_vWanderTarget;
	//private float m_dWanderRadius;
	private float WanderDist, WanderWeight;
	
	public GameObject closestGO, GoldFishPreyGO, KoiMateGO;
	public float closestFish;
	
	public int AmberjackCount;
	private int numberGF, numberKoi,  numberAJ, z;

	// Use this for initialization
	void Awake () {
        //setup the spatial subdivision class
        //    CellSpacePartition((double) cx, (double) cy,(double) cz,
        //               NumCellsX, NumCellsY, NumCellsZ, NumAgents);


        CreateCells();

        InitializeFish();

        InitializePlants();
        
		fuzzifyInUse = false;

	}
	
	
	void Start()
	{
		m_vWanderTarget = new Vector3 (m_dWanderRadius * Mathf.Cos (Random.value * Mathf.PI * 2), m_dWanderRadius * Mathf.Sin (Random.value * Mathf.PI * 2), m_dWanderRadius * Mathf.Cos (Random.value * Mathf.PI * 2));

		mateCount = 0;
		//MateText.text = "# fish Mating: " + mateCount.ToString ();
		//predatorCount = 0;
		//PredatorText.text = "# Predators: " + predatorCount.ToString ();
		//preyCount = 0;
		//PreyText.text = "# Prey: " + preyCount.ToString ();
		
		
		
		MaxSpeed = 150.0f;//150
		MaxSteeringForce = 4.0f; //should be 2.0f
		VehicleMass = 2.0f;
		VehicleScale = 3.0f;
		m_vVelocity = new Vector3 (0.0f, 0.0f, 0.0f);//Vector3.zero;
		m_Neighbors = new List<GameObject> ();
		SeparationWeight = 1.0f;//multiplying values sep/coh/align by 10 seems to work well!
		CohesionWeight = 2.0f;
        ObstacleAvoidanceWeight = .7f;// 0.7f;//what is a good weight for testing this? .o5
		AlignmentWeight = 1.0f;
		
		DistToThisIP = 0.0f;
		DistToClosestIP = 2000.0f;
		ClosestPoint = new Vector3 (0.0f, 0.0f, 0.0f);//Vector3.zero;
		Overshoot = new Vector3 (0.0f, 0.0f, 0.0f);//Vector3.zero;
		hitNormal = new Vector3 (0.0f, 0.0f, 0.0f);
		Force = new Vector3 (0.0f, 0.0f, 0.0f);
		
		m_vWanderTarget = new Vector3 (0.0f, 0.0f, 0.0f);//Vector3.zero;
		m_dWanderRadius = 1.2f;
		WanderDist = 2.0f;
		WanderWeight = 1.0f;
		
		tempHeading = new Vector3 (0.0f, 0.0f, 0.0f);
		tempVelocity = new Vector3 (0.0f, 0.0f, 0.0f);
		
		
		FeelerD = new Vector3 (0.0f, 0.0f, 0.0f);
		FeelerU = new Vector3 (0.0f, 0.0f, 0.0f);
		FeelerR = new Vector3 (0.0f, 0.0f, 0.0f);
		FeelerL = new Vector3 (0.0f, 0.0f, 0.0f);
		FeelerF = new Vector3 (0.0f, 0.0f, 0.0f);
		WallCollision = false;
		caseSwitch = -1;
		rayLength = 100.0f; //500, should use 10?  must use 40 or spheres leave box.  supposed to be 40!!!
		
		DolphinVel = new Vector3 (0.0f, 0.0f, 0.0f);
		closestFish = 1000.0f;
		
		anotherFish = false;
		counter = 0;
		z = 0;
		cp = gameObject.GetComponent<Eat> ();
		//double number = cp.GetDesirability (300.0d, 30.0d, 80.0d, 50.0d);
		////Debug.Log ("Desirability: " + number);
		//cp.m_FuzzyModule.WriteAllDOMs ();
		GoldFishPreyGO = null;
		KoiMateGO = null;
        
	}
	
	void Update()
	{
		
		for (int i = 0; i < m_Vehicles.Count; i++) {
			//bool anothFish = false;
			closestFish = 1000.0f;
			
			tempScript = m_Vehicles [i].GetComponent<BallBounce> ();
            state = tempScript.getState();
			//setAnimationSpeed(i);
			
			switch ((State)state) {
			case State.Flock:
				Flock (i);
                    break;
				
			case State.Spawn:
				Spawn (i);
				break;
				
			case State.Dead:
				Die (i);
				break;
			} //end switch
		} // end for loop
	}// end update()
	
	
	public void Die(int i) {
		
		
		//for (int i = 0; i < m_Vehicles.Count; i++) {
		//BallBounce tempScript = m_Vehicles [i].GetComponent<BallBounce> ();
		tempVelocity = tempScript.getVelocity ();
		tempHeading = tempScript.getHeading();
		////Debug.Log (tempHeadingOne);  //values range from 0.0 to 0.#  Setting heading is working because values are changing.  
		m_vVelocity = Vector3.zero;
		SteeringForceSum = Vector3.zero;
		Force = Vector3.zero;
		RaycastHit hit;
		//reset caseSwitch every Update.
		caseSwitch = -1;
		DistToClosestIP = 2000.0f;
		int layerMask = 1 << 8;
		//layerMask = ~layerMask;
		
		//rayLength = 100.0f;
		
		FeelerForward(m_Vehicles[i].transform.position, tempVelocity, rayLength, layerMask, i);
		
		//Vector3 forward = m_Vehicles[i].transform.TransformDirection(tempHeadingOne);
		
		
		//3rd attempt feelers.
		
		//not working at all.
		Quaternion rotation = Quaternion.AngleAxis (45.0f, Vector3.right);
		Vector3 t = new Vector3 (0.0f, 0.0f, 0.0f);
		t = Quaternion.Inverse (rotation) * tempVelocity;
		
		FeelerRight (m_Vehicles [i].transform.position, t, rayLength / 2.0f, layerMask, i);
		
		
		
		rotation = Quaternion.AngleAxis (45.0f, Vector3.left);
		//Vector3 t = new Vector3(0.0f,0.0f,0.0f);
		t = Quaternion.Inverse (rotation) * tempVelocity;
		
		FeelerLeft (m_Vehicles [i].transform.position, t, rayLength / 2.0f, layerMask, i);
		
		
		
		
		rotation = Quaternion.AngleAxis (45.0f, Vector3.down);
		//Vector3 t = new Vector3(0.0f,0.0f,0.0f);
		t = Quaternion.Inverse (rotation) * tempVelocity;
		
		FeelerDown(m_Vehicles [i].transform.position, t, rayLength / 2.0f, layerMask, i);
		
		
		
		
		rotation = Quaternion.AngleAxis (45.0f, Vector3.up);
		//Vector3 t = new Vector3(0.0f,0.0f,0.0f);
		t = Quaternion.Inverse (rotation) * tempVelocity;
		
		FeelerUp(m_Vehicles [i].transform.position, t, rayLength / 2.0f, layerMask, i);
		
		
		addNormalForce (caseSwitch, i);
		
		//spheres only make significant movements when raycast hits wall.  
		CalculateNeighbors (m_Vehicles [i].transform.position, 10.0f); //this radius is not used to calculate neighbors for testing?
		
		//    Force = Vector3.zero;
		
		//Force = Wander () * WanderWeight;
		
		FlockingForce (i);
		
		
		translatePosition (i);
		
		if (m_Vehicles [i].GetComponent<BallBounce> ().getAmberjack ())
			numberAJ--;
		
		if (m_Vehicles [i].GetComponent<BallBounce> ().getKoi ())
			numberKoi--;

		if (m_Vehicles [i].GetComponent<BallBounce> ().getGoldfish ())
			numberGF--;


		if (numberKoi < 10) {
			GameObject clone;
			Vector3 temp = new Vector3(Random.Range (10.0f, 900.0f), Random.Range (10.0f, 900.0f),Random.Range (10.0f, 900.0f));//for some reason all spheres move to one corner all time. 
			clone = Instantiate(fish, temp, Quaternion.identity) as GameObject;    
			clone.GetComponent<BallBounce>().setKoi ();
			//clone.GetComponent<BallBounce>().setFishNumber(i);
			////Debug.Log (clone.GetComponent<BallBounce>().getKoi());//this returns true!!!
			
			numberKoi++;
			
			m_Vehicles.Add (clone);
			m_WanderList.Add (clone);
			this.AddEntity(clone);
		}

		if (numberGF < 10) {
			GameObject clone;
			Vector3 temp = new Vector3(Random.Range (10.0f, 900.0f), Random.Range (10.0f, 900.0f),Random.Range (10.0f, 900.0f));//for some reason all spheres move to one corner all time. 
			clone = Instantiate(goldFish, temp, Quaternion.identity) as GameObject;    
			clone.GetComponent<BallBounce>().setGoldfish ();
			////Debug.Log (clone.GetComponent<BallBounce>().getKoi());//this returns true!!!
			
			numberGF++;
			
			m_Vehicles.Add (clone);
			m_WanderList.Add (clone);
			this.AddEntity(clone);
		}


		Vector3 OldPos = m_Vehicles [i].transform.position;
		////Debug.Log (SteeringForceSum); //these forces seem correct.
		
		
		this.RemoveEntity (m_Vehicles [i], OldPos);
		//}
		GameObject ent = m_Vehicles [i];
		
		m_Vehicles.RemoveAt (i);
		m_WanderList.RemoveAt (i);
		
		m_Vehicles.TrimExcess ();
		m_WanderList.TrimExcess ();
		
		Object.Destroy (ent, 0.0f);
		
		//Debug.Log ("Prey Destroyed!!!!!!!!!!!!!!!!!!!!!!!!!!");
		//Debug.Log ("Number Fish: " + m_Vehicles.Count);
		//preyCount--;
		//PreyText.text = "# Prey: " + preyCount.ToString ();
		
	}
	
	
	
	
	public void Spawn(int i) {
		
		
		//for (int i = 0; i < m_Vehicles.Count; i++) {
		//BallBounce tempScript = m_Vehicles [i].GetComponent<BallBounce> ();
		tempVelocity = tempScript.getVelocity ();
		tempHeading = tempScript.getHeading();
		////Debug.Log (tempHeadingOne);  //values range from 0.0 to 0.#  Setting heading is working because values are changing.  
		m_vVelocity = Vector3.zero;
		SteeringForceSum = Vector3.zero;
		Force = Vector3.zero;
		RaycastHit hit;
		//reset caseSwitch every Update.
		caseSwitch = -1;
		DistToClosestIP = 2000.0f;
		int layerMask = 1 << 8;
		//layerMask = ~layerMask;


		//rayLength = 100.0f;
		
		FeelerForward(m_Vehicles[i].transform.position, tempVelocity, rayLength, layerMask, i);
		
		
		//3rd attempt feelers.
		
		//not working at all.
		Quaternion rotation = Quaternion.AngleAxis (45.0f, Vector3.right);
		Vector3 t = new Vector3 (0.0f, 0.0f, 0.0f);
		t = Quaternion.Inverse (rotation) * tempVelocity;
		
		FeelerRight (m_Vehicles [i].transform.position, t, rayLength / 2.0f, layerMask, i);
		
		
		rotation = Quaternion.AngleAxis (45.0f, Vector3.left);
		//Vector3 t = new Vector3(0.0f,0.0f,0.0f);
		t = Quaternion.Inverse (rotation) * tempVelocity;
		
		FeelerLeft (m_Vehicles [i].transform.position, t, rayLength / 2.0f, layerMask, i);
		
		
		
		rotation = Quaternion.AngleAxis (45.0f, Vector3.down);
		//Vector3 t = new Vector3(0.0f,0.0f,0.0f);
		t = Quaternion.Inverse (rotation) * tempVelocity;
		
		FeelerDown(m_Vehicles [i].transform.position, t, rayLength / 2.0f, layerMask, i);
		
		
		
		
		rotation = Quaternion.AngleAxis (45.0f, Vector3.up);
		//Vector3 t = new Vector3(0.0f,0.0f,0.0f);
		t = Quaternion.Inverse (rotation) * tempVelocity;
		
		FeelerUp(m_Vehicles [i].transform.position, t, rayLength / 2.0f, layerMask, i);
		
		
		addNormalForce (caseSwitch, i);
		
		//spheres only make significant movements when raycast hits wall.  
		CalculateNeighbors (m_Vehicles [i].transform.position, 10.0f); //this radius is not used to calculate neighbors for testing?
		
		//    Force = Vector3.zero;
		
		//Force = Wander () * WanderWeight;
		
		FlockingForce (i);
		
		Vector3 OldPos = m_Vehicles [i].transform.position;
		
		
		
		translatePosition (i);
		
		
		this.UpdateEntity (m_Vehicles [i], OldPos);
		//}
		
		float random = Random.Range (0.0f, 10.0f);
		if (random > 4.0f) {
			GameObject clone = null;
			
			if (m_Vehicles[i].GetComponent<BallBounce>().getKoi () && numberKoi < 50) {
				Vector3 temp = new Vector3 (Random.Range (20.0f, 900.0f),Random.Range (20.0f, 900.0f),Random.Range (20.0f, 900.0f));//for some reason all spheres move to one corner all time. 
				clone = Instantiate (fish, temp, Quaternion.identity) as GameObject;                                            //this is error.  Find solution solve problem!!!  make tank larger!
				clone.GetComponent<BallBounce>().setKoi ();
				GameObject child = clone.transform.Find("FishKoiMesh").gameObject;
				
				if (child != null) {
					
					Renderer rend = child.GetComponent<Renderer>();
					//Material material = new Material(fishMaterial);
					
					rend.material.color = Color.blue;
				}

				numberKoi++;

				clone.GetComponent<BallBounce>().setVelocity(new Vector3(Random.Range (0.5f, 1.0f),Random.Range (0.5f, 1.0f),Random.Range (0.5f, 1.0f) ));
				//clone.gameObject.GetComponent<BallBounce> ().setVelocity (Vector3.zero);
				//clone.gameObject.GetComponent<BallBounce> ().setHeading (Vector3.zero);
				m_Vehicles.Add (clone);
				m_WanderList.Add (clone);
				this.AddEntity (clone);
				
			}
			
			if (m_Vehicles[i].GetComponent<BallBounce>().getGoldfish () && numberGF < 50) {
				Vector3 temp = new Vector3 (Random.Range (20.0f, 900.0f),Random.Range (20.0f, 900.0f),Random.Range (20.0f, 900.0f));//for some reason all spheres move to one corner all time. 
				clone = Instantiate (goldFish, temp, Quaternion.identity) as GameObject;                                            //this is error.  Find solution solve problem!!!  make tank larger!
				clone.GetComponent<BallBounce>().setGoldfish ();
				GameObject child = clone.transform.Find("GoldFishMesh").gameObject;
				
				if (child != null) {
					
					Renderer rend = child.GetComponent<Renderer>();
					//Material material = new Material(fishMaterial);
					
					rend.material.color = Color.blue;
				}

				numberGF++;

				
				clone.GetComponent<BallBounce>().setVelocity(new Vector3(Random.Range (0.5f, 1.0f),Random.Range (0.5f, 1.0f),Random.Range (0.5f, 1.0f) ));
				//clone.gameObject.GetComponent<BallBounce> ().setVelocity (Vector3.zero);
				//clone.gameObject.GetComponent<BallBounce> ().setHeading (Vector3.zero);
				m_Vehicles.Add (clone);
				m_WanderList.Add (clone);
				this.AddEntity (clone);
				
				
			}
			
			
			if (m_Vehicles[i].GetComponent<BallBounce>().getAmberjack () && numberAJ < 40) {
				for(int q = 0; q < 100; q++)
					//Debug.Log ("Dolphin Count: " + AmberjackCount);
				
				numberAJ++;
				Vector3 temp = new Vector3 (Random.Range (30.0f, 900.0f),Random.Range (30.0f, 900.0f),Random.Range (30.0f, 900.0f));//for some reason all spheres move to one corner all time. 
				clone = Instantiate (fishDolphin, temp, Quaternion.identity) as GameObject;                                            //this is error.  Find solution solve problem!!!  make tank larger!
				clone.GetComponent<BallBounce>().setAmberjack ();
				GameObject child = clone.transform.Find("AmberjackMesh").gameObject;
				
				if (child != null) {
					
					Renderer rend = child.GetComponent<Renderer>();
					//Material material = new Material(fishMaterial);
					
					rend.material.color = Color.blue;
				}
				
				clone.GetComponent<BallBounce>().setVelocity(new Vector3(Random.Range (0.5f, 1.0f),Random.Range (0.5f, 1.0f),Random.Range (0.5f, 1.0f) ));
				//clone.gameObject.GetComponent<BallBounce> ().setVelocity (Vector3.zero);
				//clone.gameObject.GetComponent<BallBounce> ().setHeading (Vector3.zero);
				m_Vehicles.Add (clone);
				m_WanderList.Add (clone);
				this.AddEntity (clone);
				
				
			}
			
			
		}
	//	if (m_Vehicles [i].gameObject.GetComponent<BallBounce> ().getFishNumber () == 1) {
	//		File.AppendAllText (fileName, "state:" + tempScript.getState ().ToString () + "\n");
	//	}

		//Debug.Log ("Baby added!!!!!!!!!!!!!!!!!!!!!!!!!!");
		tempScript.setState (0);
		tempScript.setTimer (0.0f);
		float tempHunger = tempScript.getHunger ();
		tempScript.setHunger (tempHunger + 10.0f);
	}
	
	
	
	
	public void Flock(int i)
	{
		
		tempVelocity = tempScript.getVelocity ();
		tempHeading = tempScript.getHeading();
		////Debug.Log (tempHeadingOne);  //values range from 0.0 to 0.#  Setting heading is working because values are changing.  
		m_vVelocity = Vector3.zero;
		SteeringForceSum = Vector3.zero;
		Force = Vector3.zero;
		RaycastHit hit;
		//reset caseSwitch every Update.
		caseSwitch = -1;
		DistToClosestIP = 2000.0f;
		int layerMask = 1 << 8;
		//layerMask = ~layerMask;

		//rayLength = 100.0f;
		
		FeelerForward(m_Vehicles[i].transform.position, tempVelocity, rayLength, layerMask, i);
		
		//3rd attempt feelers.
		
		//not working at all.
		Quaternion rotation = Quaternion.AngleAxis (45.0f, Vector3.right);
		Vector3 t = new Vector3 (0.0f, 0.0f, 0.0f);
		t = Quaternion.Inverse (rotation) * tempVelocity;
		
		FeelerRight (m_Vehicles [i].transform.position, t, rayLength / 2.0f, layerMask, i);
		
		
		
		
		rotation = Quaternion.AngleAxis (45.0f, Vector3.left);
		//Vector3 t = new Vector3(0.0f,0.0f,0.0f);
		t = Quaternion.Inverse (rotation) * tempVelocity;
		
		FeelerLeft (m_Vehicles [i].transform.position, t, rayLength / 2.0f, layerMask, i);
		
		
		
		rotation = Quaternion.AngleAxis (45.0f, Vector3.down);
		//Vector3 t = new Vector3(0.0f,0.0f,0.0f);
		t = Quaternion.Inverse (rotation) * tempVelocity;
		
		FeelerDown(m_Vehicles [i].transform.position, t, rayLength / 2.0f, layerMask, i);
		
		
		
		
		rotation = Quaternion.AngleAxis (45.0f, Vector3.up);
		//Vector3 t = new Vector3(0.0f,0.0f,0.0f);
		t = Quaternion.Inverse (rotation) * tempVelocity;
		
		FeelerUp(m_Vehicles [i].transform.position, t, rayLength / 2.0f, layerMask, i);
		
		
		addNormalForce (caseSwitch, i);
		
		//spheres only make significant movements when raycast hits wall.  
		CalculateNeighbors (m_Vehicles [i].transform.position, 50.0f); //this radius is not used to calculate neighbors for testing?
		//TagVehiclesWithinViewRange (m_Vehicles [i], m_Vehicles, 200.0d);
		//    Force = Vector3.zero;
		
		//Force = Wander () * WanderWeight;

	//	if (!m_Vehicles[i].GetComponent<BallBounce>().getDolphin())
	//		AvoidPredator (i, m_Vehicles[i]);

		FlockingForce (i);

		
		Vector3 OldPos = m_Vehicles [i].transform.position;
		
		translatePosition (i);
		
		
		this.UpdateEntity (m_Vehicles [i], OldPos);
		//}
		
	}
	
	
	
	public int PositionToIndex(Vector3 pos) 
	{
		int idx = (int)(m_iNumCellsX * pos.x / m_dSpaceWidth) +
			((int)((m_iNumCellsY) * pos.y / m_dSpaceHeight) * m_iNumCellsX) +
				((int)((m_iNumCellsZ) * pos.z / m_dSpaceDepth) * m_iNumCellsX * m_iNumCellsY);
		
		if (idx > (int)m_Cells.Count - 1) {
			idx = (int)m_Cells.Count - 1;
		}
		
		return idx;
	}
	
	
	
	public void AddEntity(GameObject ent) 
	{
		if (ent.Equals (null))
			return;
		
		int idx = PositionToIndex (ent.transform.position);
		
		m_Cells [idx].Members.Add (ent);
	}
	
	public void RemoveEntity(GameObject ent, Vector3 OldPos) {
		int OldIdx = PositionToIndex (OldPos);
		
		m_Cells [OldIdx].Members.Remove (ent);
	}
	
	public void UpdateEntity(GameObject ent, Vector3 OldPos) 
	{
		int OldIdx = PositionToIndex (OldPos);
		int NewIdx = PositionToIndex (ent.transform.position);
		
		if (NewIdx == OldIdx)
			return;
		
		m_Cells [OldIdx].Members.Remove (ent);
		m_Cells [NewIdx].Members.Add (ent);
	}
	
	
	public void CalculateNeighbors(Vector3 TargetPos, float QueryRadius)
	{
		m_Neighbors.Clear();
		
		QueryCircle = new BoundingCircle (TargetPos, QueryRadius);
		
		for (int i = 0; i < m_Cells.Count; i++)
		{
			//isOverlappedWith(QueryCircle) uses calling object's radius, not QueryCircle's radius. 
			if (m_Cells[i].BCircle.isOverlappedWith (QueryCircle) &&
			    m_Cells[i].Members.Count != 0) 
			{
				for (int j = 0; j < m_Cells[i].Members.Count; j++)
				{
					if (m_Cells[i].Members[j] != null) {
						if (Vector3.Distance(m_Cells[i].Members[j].transform.position, TargetPos) <
						    2 * QueryRadius)
						{
							m_Neighbors.Add (m_Cells[i].Members[j]);
							
						}
					}
				}
			}
		}
	}
	
	
	
	
	private Vector3 Seek(Vector3 TargetPos, GameObject ent)//ent is m_Vehicles[i]
	{
		
		Vector3 DesiredVelocity = new Vector3 (0.0f, 0.0f, 0.0f);
		DesiredVelocity = (TargetPos - ent.transform.position) * MaxSpeed;
		return (DesiredVelocity - m_vVelocity);
	}
	
	private Vector3 Flee(Vector3 TargetPos, int i) 
	{
		Vector3 DesiredVelocity = ((m_Vehicles [i].transform.position - TargetPos).normalized * 150.0f);
		return (DesiredVelocity - m_Vehicles[i].gameObject.GetComponent<BallBounce>().getVelocity());
	}
	
	//possible solution!!!!!!***************
	private Vector3 SeparationPlus(GameObject ent)
	{
		Vector3 SteeringForce = new Vector3 (0.0f, 0.0f, 0.0f);
		
		for (int i = 0; i < m_Neighbors.Count; i++) {
			GameObject pV = m_Neighbors [i];
			bool areEqual = System.Object.ReferenceEquals(ent, pV);
			////Debug.Log (areEqual);
			if (!areEqual) {//testing equality of objects? Use above!
				Vector3 ToAgent = (ent.transform.position - pV.transform.position ); //changed from ent.transform.position - pV.transform.position and it works great!!!
				//but is this the answer???  sphere's are not keeping distance from eachother anymore...
				////Debug.Log (ToAgent);
				SteeringForce += ToAgent.normalized / ToAgent.magnitude;
			}
		}
		return SteeringForce;
	}
	
	
	private Vector3 AlignmentPlus(GameObject ent)
	{
		Vector3 AverageHeading = new Vector3 (0.0f, 0.0f, 0.0f);
		
		float NeighborCount = 0.0f;
		
		for (int i = 0; i < m_Neighbors.Count; i++) {
			GameObject pV = m_Neighbors [i];
			bool areEqual = System.Object.ReferenceEquals (ent, pV);
			BallBounce tempScript = m_Neighbors [i].GetComponent<BallBounce> ();
			//tempHeadingOne = tempScript.getHeading ();
			if (!areEqual) {
				AverageHeading += tempScript.getHeading();//tempHeadingOne;  THIS should be heading, not m_vVelocity...
				++NeighborCount;
			}
		}
		if (NeighborCount > 0.0f) {
			AverageHeading /= NeighborCount;
			AverageHeading -= tempHeadingOne; //THIS SHOULD ALSO BE HEADING, NOT m_vVelocity.
		}
		return AverageHeading;
	}
	
	
	private Vector3 CohesionPlus(GameObject ent)
	{
		Vector3 CenterOfMass = new Vector3 (0.0f, 0.0f, 0.0f);
		Vector3 SteeringForce = new Vector3 (0.0f, 0.0f, 0.0f);
		
		int NeighborCount = 0;
		
		for (int i = 0; i < m_Neighbors.Count; i++) {
			GameObject pV = m_Neighbors [i];
			bool areEqual = System.Object.ReferenceEquals(ent, pV);
			if (!areEqual) {
				CenterOfMass += pV.transform.position;
				NeighborCount++;
			}
		}
		
		if (NeighborCount > 0) {
			CenterOfMass /= (float)NeighborCount;
			SteeringForce = Seek (CenterOfMass, ent);
		}
		
		return SteeringForce.normalized;
	}
	
	private void AccumulateForce(Vector3 ForceToAdd)
	{
		float MagnitudeSoFar = SteeringForceSum.magnitude;
		float MagnitudeRemaining = MaxSteeringForce - MagnitudeSoFar;
		
		if (MagnitudeRemaining <= 0.0) {
			return;
		}
		
		float MagnitudeToAdd = ForceToAdd.magnitude;
		
		if (MagnitudeToAdd < MagnitudeRemaining) {
			SteeringForceSum += ForceToAdd;
		} else {
			SteeringForceSum += ForceToAdd.normalized * MagnitudeRemaining;
		}
		return;
	}
	
	
	private Vector3 Flee(Vector3 TargetPos, GameObject ent)//ent is m_Vehicles[i].
	{
		Vector3 DesiredVelocity = (ent.transform.position - TargetPos).normalized;
		DesiredVelocity = DesiredVelocity * 150.0f;//.magnitude * 150.0f;  YOU CAN MULT A NORMALIZED VECTOR BY A FLOAT!!!
		
		return (DesiredVelocity - tempVelocity);
	}
	
	private Vector3 Evade(GameObject pursuer, GameObject ent)
	{
		Vector3 ToPursuer = pursuer.transform.position - ent.transform.position;
		
		float ThreatRange = 300.0f;
		if (ToPursuer.sqrMagnitude > ThreatRange * ThreatRange) {
			return new Vector3 (0.0f, 0.0f, 0.0f);
		}
		DolphinVel = pursuer.gameObject.GetComponent<BallBounce> ().getVelocity ();
		float LookAheadTime = ToPursuer.magnitude / (150.0f + DolphinVel.magnitude);
		
		return Flee ((pursuer.transform.position + (DolphinVel * LookAheadTime)), ent);
	}
	
	private Vector3 Arrive(Vector3 TargetPos, GameObject ent) {
		Vector3 ToTarget = TargetPos - ent.transform.position;
		
		float dist = ToTarget.magnitude;
		
		if (dist > 50.0f) {
			const float DecelerationTweaker = 0.3f;
			
			float speed = dist / ( 2.0f * DecelerationTweaker);//(float)Deceleration.fast
			
			speed = Mathf.Min (speed, 150.0f);
			
			Vector3 DesiredVelocity = ToTarget * (speed/dist);
			
			return DesiredVelocity - tempScript.getVelocity();
		}
		return new Vector3 (0.0f,0.0f,0.0f);
	}
	
	private Vector3 Pursuit(GameObject evader, int i) {
		Vector3 ToEvader = evader.transform.position - m_Vehicles [i].transform.position;
		
		float RelativeHeading = Vector3.Dot (m_Vehicles [i].gameObject.GetComponent<BallBounce> ().getHeading (), evader.gameObject.GetComponent<BallBounce> ().getHeading ());
		
		if(Vector3.Dot (ToEvader, m_Vehicles [i].gameObject.GetComponent<BallBounce> ().getHeading ()) > 0.0f
		   && (RelativeHeading < -0.95))
			return Seek (evader.transform.position, m_Vehicles[i]);
		
		Vector3 temp = evader.gameObject.GetComponent<BallBounce> ().getVelocity ();
		float LookAheadTime = ToEvader.magnitude / (150.0f + temp.magnitude);
		
		return Seek (evader.transform.position + (temp * LookAheadTime), m_Vehicles[i]);
	}
	
	
	public void FeelerForward(Vector3 pos, Vector3 Vel, float rayLength, int LayerMask, int i) {
		RaycastHit hit;
       // Vector3 Velo;
        //Velo = tempVelocity.normalized;

		if (Physics.Raycast (m_Vehicles [i].transform.position, tempVelocity, out hit, rayLength, LayerMask)) {
			
			////Debug.Log(hit.distance);
			//WallCollision = true;
			
			FeelerF = tempVelocity * rayLength; //tempHeadingOne.normalized  should be according to  original code...
			DistToThisIP = hit.distance;
			FeelerFNormal = hit.normal;
			//ClosestPoint = hit.point;
			
			if (DistToThisIP < DistToClosestIP) {
				DistToClosestIP = DistToThisIP;
				ClosestPoint = hit.point;
				caseSwitch = (int)feeler.forward;
			}
			
			//This draws line before forces are added to object.  
			Debug.DrawRay (m_Vehicles [i].transform.position, tempVelocity * rayLength, Color.green); 
			
		}
	}
	
	public void FeelerRight(Vector3 pos, Vector3 Vel, float rayLength, int LayerMask, int i) {
		RaycastHit hit;

       // Vel = Vel.normalized;

		if (Physics.Raycast (m_Vehicles [i].transform.position, Vel, out hit, rayLength / 2.0f, LayerMask)) {
			
			////Debug.Log(hit.distance);
			
			//This draws line before forces are added to object.  
			Debug.DrawRay (m_Vehicles [i].transform.position, Vel * rayLength / 2.0f , Color.blue); 
			
			//WallCollision = true;
			
			FeelerR = Vel * rayLength / 2.0f;
			DistToThisIP = hit.distance;
			FeelerRNormal = hit.normal;
			//ClosestPoint = hit.point;
			
			if (DistToThisIP < DistToClosestIP) {
				DistToClosestIP = DistToThisIP;
				ClosestPoint = hit.point;
				caseSwitch = (int)feeler.right;
			}
			
			
		}
	}
	
	public void FeelerLeft(Vector3 pos, Vector3 Vel, float rayLength, int LayerMask, int i) {
		RaycastHit hit;

       // Vel = Vel.normalized;

		if (Physics.Raycast (m_Vehicles [i].transform.position, Vel, out hit, rayLength / 2.0f, LayerMask)) {
			
			////Debug.Log(hit.distance);
			
			//This draws line before forces are added to object.  
			Debug.DrawRay (m_Vehicles [i].transform.position, Vel * rayLength / 2.0f, Color.gray); 
			
			//WallCollision = true;
			
			FeelerL = Vel * rayLength / 2.0f;
			DistToThisIP = hit.distance;
			FeelerLNormal = hit.normal;
			//ClosestPoint = hit.point;
			
			if (DistToThisIP < DistToClosestIP) {
				DistToClosestIP = DistToThisIP;
				ClosestPoint = hit.point;
				caseSwitch = (int)feeler.left;
			}
			
			
		}
	}
	
	public void FeelerDown(Vector3 pos, Vector3 Vel, float rayLength, int LayerMask, int i) {
		RaycastHit hit;
        //Vel = Vel.normalized;

		if (Physics.Raycast (m_Vehicles [i].transform.position, Vel, out hit, rayLength / 2.0f, LayerMask)) {
			
			////Debug.Log(hit.distance);
			
			//This draws line before forces are added to object.  
			Debug.DrawRay (m_Vehicles [i].transform.position, Vel * rayLength / 2.0f, Color.yellow); 
			
			WallCollision = true;
			FeelerD = Vel * rayLength / 2.0f;
			DistToThisIP = hit.distance;
			FeelerDNormal = hit.normal;
			//ClosestPoint = hit.point;
			
			if (DistToThisIP < DistToClosestIP) {
				DistToClosestIP = DistToThisIP;
				ClosestPoint = hit.point;
				caseSwitch = (int)feeler.down;
			}
			
			
		}
		
	}
	
	public void FeelerUp(Vector3 pos, Vector3 Vel, float rayLength, int LayerMask, int i) {
		RaycastHit hit;
        //Vel = Vel.normalized;

		if (Physics.Raycast (m_Vehicles [i].transform.position, Vel, out hit, rayLength / 2.0f, LayerMask)) {
			
			////Debug.Log(hit.distance);
			
			//This draws line before forces are added to object.  
			Debug.DrawRay (m_Vehicles [i].transform.position, Vel * rayLength / 2.0f, Color.red); 
			
			FeelerU = Vel * rayLength / 2.0f;
			DistToThisIP = hit.distance;
			FeelerUNormal = hit.normal;
			//ClosestPoint = hit.point;
			
			if (DistToThisIP < DistToClosestIP) {
				DistToClosestIP = DistToThisIP;
				ClosestPoint = hit.point;
				caseSwitch = (int)feeler.up;
			}
			
			
		}
	}
	
	public void addNormalForce(int caseSwitch, int i) {


		switch (caseSwitch) {
		case 0: //is it correct to add m_Vehicles[i].transform.position?
			Overshoot = m_Vehicles [i].transform.position + FeelerF - ClosestPoint;
			Force = FeelerFNormal * Overshoot.magnitude * ObstacleAvoidanceWeight;
                if (Force.magnitude > 4)
                    Force = Force.normalized * 4f;
			AccumulateForce (Force);
			break;
		case 1: 
			Overshoot = m_Vehicles [i].transform.position + FeelerU - ClosestPoint;
			Force = FeelerUNormal * Overshoot.magnitude * ObstacleAvoidanceWeight;
                if (Force.magnitude > 4)
                    Force = Force.normalized * 4f;
                AccumulateForce (Force);
			break;
			
		case 2: 
			Overshoot = m_Vehicles [i].transform.position + FeelerD - ClosestPoint;
			Force = FeelerDNormal * Overshoot.magnitude * ObstacleAvoidanceWeight;
                if (Force.magnitude > 4)
                    Force = Force.normalized * 4f;
                AccumulateForce(Force);
			break;
			
		case 3:
			Overshoot = m_Vehicles [i].transform.position + FeelerL - ClosestPoint;
			Force = FeelerLNormal * Overshoot.magnitude * ObstacleAvoidanceWeight;
                if (Force.magnitude > 4)
                    Force = Force.normalized * 4f;
                AccumulateForce(Force);
			break;
			
		case 4:
			Overshoot = m_Vehicles [i].transform.position + FeelerR - ClosestPoint;
			Force = FeelerRNormal * Overshoot.magnitude * ObstacleAvoidanceWeight;
                if (Force.magnitude > 4)
                    Force = Force.normalized * 4f;
                AccumulateForce(Force);
			break;
			
		default: 
			////Debug.Log("No wall collision");
			break;
		}
	}
	
	public void translatePosition(int i) {
		//Vector3 OldPos = m_Vehicles [i].transform.position;
		////Debug.Log (SteeringForceSum); //these forces seem correct.
		Vector3 acceleration = SteeringForceSum / VehicleMass;
        // Debug.Log(acceleration * Time.deltaTime);
        tempVelocity += acceleration * Time.deltaTime;  //what is value of Time.deltaTime vs netbeans time function???
		////Debug.Log (tempHeadingOne); //this value of m_vVelocity is still 0,0,0 ...?

		//if (tempScript.getDolphin ())
		//	tempVelocity = tempVelocity;

		if (tempVelocity.magnitude > MaxSpeed) {
			Vector3 tempV = tempVelocity.normalized;
			tempVelocity = tempV * MaxSpeed;
			
		}
		
		//m_Vehicles [i].GetComponent<BallBounce> ().setVelocity (tempVelocity); //should you normalize m_vVelocity before setting it in fish?
		tempScript.setVelocity (tempVelocity);
		
		//m_Vehicles [i].transform.Translate (tempHeadingOne, Space.Self); //this seems to work better.  
		Vector3 tempPos = new Vector3(0.0f,0.0f,0.0f);
		
		tempPos = m_Vehicles[i].transform.position;
        tempPos += tempVelocity * Time.deltaTime * 50.0f;//Time.deltaTime is what's added from original code. 
		
		
		m_Vehicles[i].transform.position = tempPos;
		//    }
		
		if (tempVelocity.magnitude > 0.00000001) {
			tempHeading = tempVelocity.normalized;
			tempScript.setHeading (tempHeading);
		}
		
		
		
		if (tempVelocity != Vector3.zero) {
			Quaternion r = Quaternion.LookRotation(tempVelocity, Vector3.up);
            m_Vehicles[i].transform.rotation = Quaternion.Slerp (m_Vehicles[i].transform.rotation, r, .5f);
		}

		float x = m_Vehicles [i].transform.position.x;
		float y = m_Vehicles [i].transform.position.y;
		float z = m_Vehicles [i].transform.position.z;

		if (m_Vehicles [i].gameObject.GetComponent<BallBounce> ().getDolphin ()) {
			if (m_Vehicles [i].transform.position.x < 20f) {
				x = 20f;
			}  
			
			if (m_Vehicles [i].transform.position.x > 980f) {
				x = 980f;
			} 
			
			if (m_Vehicles [i].transform.position.y < 20f) {
				y = 20f;
			}
			
			if (m_Vehicles [i].transform.position.y > 980f) {
				y = 980f;
			}
			
			if (m_Vehicles [i].transform.position.z < 20f) {
				z = 20f;
			}
			
			if (m_Vehicles [i].transform.position.z > 980f) {
				z = 980f;
			}

		} else {

			if (m_Vehicles [i].transform.position.x < 0f) {
				x = 1f;
			}  
		
			if (m_Vehicles [i].transform.position.x > 1000f) {
				x = 999f;
			} 
		
			if (m_Vehicles [i].transform.position.y < 0f) {
				y = 1f;
			}
		
			if (m_Vehicles [i].transform.position.y > 1000f) {
				y = 999f;
			}
		
			if (m_Vehicles [i].transform.position.z < 0f) {
				z = 1f;
			}
		
			if (m_Vehicles [i].transform.position.z > 1000f) {
				z = 999f;
			}
		
		}
		Vector3 inBoundLocation = new Vector3(x,y,z);
		m_Vehicles[i].transform.position = inBoundLocation;
		
		//tempScript.setVelocity (Vector3.zero);

		
	}
	
	public void FlockingForce(int i) {

		AvoidPredator (i, m_Vehicles [i]);

		Force = Vector3.zero;

		Force = SeparationPlus (m_Vehicles [i]) * SeparationWeight;
		//Force = Separation (m_Neighbors, m_Vehicles [i]) * SeparationWeight;
		//Debug.Log (Force);
		
		AccumulateForce (Force);
		
		Force = Vector3.zero;
		//you should be able to take alignment out and it should work...you don't understand heading....
		Force = AlignmentPlus (m_Vehicles [i]) * AlignmentWeight;
		//Force = Alignment (m_Neighbors, m_Vehicles [i]) * AlignmentWeight; //what are the correct values for weights?  //try m_Vehicles
		////Debug.Log (Force);
		AccumulateForce (Force);
		
		Force = Vector3.zero;
		
		Force = CohesionPlus (m_Vehicles [i]) * CohesionWeight;
		//Force = Cohesion (m_Neighbors, m_Vehicles [i]) * CohesionWeight;
		////Debug.Log (Force);
		AccumulateForce (Force);
		
	}

	public List<GameObject> getVehicles() {
		return m_Vehicles;
	}
	
	public List<GameObject> getWanderList() {
		return m_WanderList;
	}
	
	public List<GameObject> getPlantList() {
		return m_PlantList;
	}
	
	public Dictionary<string,double> getDictionary() {
		return domDictionary;
	}
	
	public void setFuzzifyInUse(bool use) {
		this.fuzzifyInUse = use;
	}
	
	public bool getFuzzifyInUse() {
		return fuzzifyInUse;
	}
	
	public double[] getTempDouble() {
		return tempDouble;
	}
    
    /*
	public void setAnimationSpeed(int i) {
		anim = m_Vehicles [i].GetComponent<Animation> ();
		
		float hotDistance = Vector3.Distance (hotLightPosition.transform.position, m_Vehicles [i].transform.position);
		
		if (hotDistance > 500.0f) {
			//foreach(AnimationState state in anim) {
			if(m_Vehicles[i].GetComponent<BallBounce>().getKoi ()) {
				anim["KoiSwim"].speed = 0.5f;
			}
			if(m_Vehicles[i].GetComponent<BallBounce>().getGoldfish ()) {
				anim["GoldfishSwim"].speed = 0.5f;
			}
			if(m_Vehicles[i].GetComponent<BallBounce>().getAmberjack ()) {
				anim["AmberjackSwim"].speed = 0.5f;
			}
			//}
		} else {
			//foreach(AnimationState state in anim) {
			if(m_Vehicles[i].GetComponent<BallBounce>().getKoi ()) {
				anim["KoiSwim"].speed = 1.0f;
			}
			if(m_Vehicles[i].GetComponent<BallBounce>().getGoldfish ()) {
				anim["GoldfishSwim"].speed = 1.0f;
			}
			if(m_Vehicles[i].GetComponent<BallBounce>().getAmberjack ()) {
				anim["AmberjackSwim"].speed = 1.0f;
			}
			//}
		}
	}
    */
	public void TagVehiclesWithinViewRange(GameObject m_pVehicle,List<GameObject> m_pVehicles, double m_dViewDistance) {
		BallBounce temp = null;

		for (int i = 0; i < m_pVehicles.Count; i++) {
			temp = m_pVehicles[i].gameObject.GetComponent<BallBounce>();
			temp.unTag();

			Vector3 to = temp.transform.position - m_pVehicle.transform.position;

			double range = m_dViewDistance + 100.0d;

			bool areEqual = System.Object.ReferenceEquals (m_pVehicles [i],m_pVehicle);
			if (!areEqual && to.magnitude*to.magnitude < m_dViewDistance * m_dViewDistance) {
				temp.Tag ();
			}
		}
	}


	public void AvoidPredator(int i, GameObject ent) {
		Force = Vector3.zero;
		
		GameObject closestRedFish = null;
		float closestRFFloat = 300f;
		float temp = 0f;
		bool runAway = false;
		
		//if (Vector3.Distance (m_Vehicles [i].transform.position, dolfinUsing.transform.position) < 300f)
		//	Force = Evade (dolfinUsing, m_Vehicles [i]) * 1.5f;
		
		AccumulateForce (Force);
		
		Force = Vector3.zero;
		
		for (int j = 0; j < m_RedFish.Count; j++) {
			if (m_RedFish[j].gameObject.GetComponent<BallBounce>().getState () != 1)
			{
				m_RedFish.RemoveAt (j);
				m_RedFish.TrimExcess();
				continue;
			}
			temp = Vector3.Distance(m_Vehicles[i].transform.position, m_RedFish[j].transform.position);
			if (temp < closestRFFloat) {
				closestRFFloat = temp;
				closestRedFish = m_RedFish[j];
				runAway = true;
			}
		}
		
		if (runAway) {
			Force = Evade (closestRedFish, m_Vehicles[i]); //.01
		}
		
		AccumulateForce (Force);

	}
    
    public void CreateCells()
    {

        AmberjackCount = 0;

        m_dViewDistance = 50.0d;

        numberGF = 0;
        numberKoi = 0;
        numberAJ = 0;

        m_dSpaceWidth = cx;//1000
        m_dSpaceHeight = cy;
        m_dSpaceDepth = cz;

        m_iNumCellsX = NumCellsX;//10  do you need these variables?
        m_iNumCellsY = NumCellsY;
        m_iNumCellsZ = NumCellsZ;

        m_dCellSizeX = cx / NumCellsX; //10
        m_dCellSizeY = cy / NumCellsY;
        m_dCellSizeZ = cz / NumCellsZ;

        m_Neighbors = new List<GameObject>(MaxEntities); //100

        //create the cells.  Is this correct?  Yes. 
        for (int z = 0; z < m_iNumCellsZ; ++z)
        {
            for (int y = 0; y < m_iNumCellsY; ++y)
            {
                for (int x = 0; x < m_iNumCellsX; ++x)
                {
                    double left = x * m_dCellSizeX;
                    double right = left + m_dCellSizeX;
                    double bot = y * m_dCellSizeY;
                    double top = bot + m_dCellSizeY;
                    double front = z * m_dCellSizeZ;
                    double back = front + m_dCellSizeZ;

                    Vector3 tempFront = new Vector3((float)left, (float)bot, (float)front);
                    Vector3 tempBack = new Vector3((float)right, (float)top, (float)back);
                    Vector3 sum = (tempFront + tempBack);
                    Vector3 midPoint = sum / 2;

                    m_Cells.Add(new Cell(midPoint, 50.0f));//value is 50, but screen in netbeans if 500x500.  so, use 10.0f?
                }
            }

        }

    }

    public void InitializeFish()
    {
        BallBounce bbtemp;

        for (int i = 0; i < 100; i++)
        { //numAgents 50

            GameObject clone;
            Vector3 temp = new Vector3(Random.Range(200.0f, 800.0f), Random.Range(200.0f, 800.0f), Random.Range(200.0f, 800.0f));//for some reason all spheres move to one corner all time. 
            clone = Instantiate(fish, temp, Quaternion.identity) as GameObject;
            bbtemp = clone.GetComponent<BallBounce>();
            bbtemp.setKoi();
            bbtemp.setState(0);
            //clone.GetComponent<BallBounce>().setFishNumber(i);
            ////Debug.Log (clone.GetComponent<BallBounce>().getKoi());//this returns true!!!

            numberKoi++;

            m_Vehicles.Add(clone);
            m_WanderList.Add(clone);
            this.AddEntity(clone);

        }

        for (int i = 0; i < 100; i++)
        { //50

            GameObject clone;
            Vector3 temp = new Vector3(Random.Range(200.0f, 800.0f), Random.Range(200.0f, 800.0f), Random.Range(200.0f, 800.0f));//for some reason all spheres move to one corner all time. 
            clone = Instantiate(goldFish, temp, Quaternion.identity) as GameObject;
            bbtemp = clone.GetComponent<BallBounce>();
            bbtemp.setGoldfish();
            bbtemp.setState(0);
            ////Debug.Log (clone.GetComponent<BallBounce>().getKoi());//this returns true!!!

            numberGF++;

            m_Vehicles.Add(clone);
            m_WanderList.Add(clone);
            this.AddEntity(clone);

        }

        
        for (int i = 1; i < 75; i++)
        {//30 fishDolphin is aberjack

            GameObject clone;
            Vector3 temp = new Vector3(Random.Range(200.0f, 800.0f), Random.Range(200.0f, 800.0f), Random.Range(200.0f, 800.0f));//for some reason all spheres move to one corner all time. 
            clone = Instantiate(fishDolphin, temp, Quaternion.identity) as GameObject;
            bbtemp = clone.GetComponent<BallBounce>();
            bbtemp.setAmberjack();
            bbtemp.setState(0);
            //clone.GetComponent<BallBounce>().setFishNumber(i);
            ////Debug.Log (clone.GetComponent<BallBounce>().getKoi());//this returns true!!!

            numberAJ++;

            AmberjackCount++;
            m_Vehicles.Add(clone);
            m_WanderList.Add(clone);
            this.AddEntity(clone);

        }
        
        GameObject clone1;
        Vector3 temp1 = new Vector3(Random.Range(200.0f, 800.0f), Random.Range(200.0f, 800.0f), Random.Range(200.0f, 800.0f));//for some reason all spheres move to one corner all time. 
        clone1 = Instantiate(Dolfin, temp1, Quaternion.identity) as GameObject;
        dolfinUsing = clone1;
        clone1.GetComponent<BallBounce>().setDolphin();
        clone1.GetComponent<BallBounce>().setState(0);
        //clone.GetComponent<BallBounce>().setFishNumber(i);
        ////Debug.Log (clone.GetComponent<BallBounce>().getKoi());//this returns true!!!
        /// 
        m_Vehicles.Add(clone1);
        //m_WanderList.Add (clone1);
        this.AddEntity(clone1);


        GameObject seaHorse;

        //SeaHorse
        temp1 = new Vector3(Random.Range(200.0f, 800.0f), Random.Range(200.0f, 800.0f), Random.Range(200.0f, 800.0f));//for some reason all spheres move to one corner all time. 
        clone1 = Instantiate(SeaHorse, temp1, Quaternion.identity) as GameObject;
        seaHorse = clone1;
       // clone1.GetComponent<BallBounce>().setDolphin();
       // clone1.GetComponent<BallBounce>().setState(0);
        //clone.GetComponent<BallBounce>().setFishNumber(i);
        ////Debug.Log (clone.GetComponent<BallBounce>().getKoi());//this returns true!!!
        /// 
       // m_Vehicles.Add(clone1);
        //m_WanderList.Add (clone1);
       // this.AddEntity(clone1);
    }

    public void InitializePlants()
    {

        GameObject plantClone;

        for (int j = 0; j < 10; j++)
        {
            Vector3 plantLoc = new Vector3(Random.Range(10.0f, 980.0f), 20.0f, Random.Range(10.0f, 980.0f));
            plantClone = Instantiate(seaweed, plantLoc, Quaternion.identity) as GameObject;

            m_PlantList.Add(plantClone);
        }

        for (int j = 0; j < 10; j++)
        {
            Vector3 plantLoc = new Vector3(Random.Range(10.0f, 980.0f), 20.0f, Random.Range(10.0f, 980.0f));
            plantClone = Instantiate(seaweed5by5, plantLoc, Quaternion.identity) as GameObject;
            //m_PlantList.Add (plantClone);
        }


        for (int j = 0; j < 10; j++)
        {
            Vector3 plantLoc = new Vector3(Random.Range(10.0f, 980.0f), 20.0f, Random.Range(10.0f, 980.0f));
            plantClone = Instantiate(seaweed5by8, plantLoc, Quaternion.identity) as GameObject;
            //m_PlantList.Add (plantClone);
        }


        //    Instantiate (plant, plantLoc, Quaternion.identity);
        for (int i = 0; i < 10; i++)
        {
            Vector3 stoneLoc = new Vector3(Random.Range(10.0f, 900.0f), 10.0f, Random.Range(0.0f, 1000.0f));
            Instantiate(stone, stoneLoc, Quaternion.identity);
        }

        for (int i = 0; i < 10; i++)
        {
            Vector3 coralLoc = new Vector3(Random.Range(10.0f, 900.0f), 10.0f, Random.Range(0.0f, 1000.0f));
            Instantiate(coral1, coralLoc, Quaternion.identity);
        }

        for (int i = 0; i < 10; i++)
        {
            Vector3 coralLoc = new Vector3(Random.Range(10.0f, 900.0f), 10.0f, Random.Range(0.0f, 1000.0f));
            Instantiate(coral2, coralLoc, Quaternion.identity);
        }

        for (int i = 0; i < 10; i++)
        {
            Vector3 coralLoc = new Vector3(Random.Range(10.0f, 900.0f), 10.0f, Random.Range(0.0f, 1000.0f));
            Instantiate(coral4, coralLoc, Quaternion.identity);
        }

        for (int i = 0; i < 10; i++)
        {
            Vector3 coralLoc = new Vector3(Random.Range(10.0f, 900.0f), 10.0f, Random.Range(0.0f, 1000.0f));
            Instantiate(seaShell1, coralLoc, Quaternion.identity);
        }

        for (int i = 0; i < 10; i++)
        {
            Vector3 coralLoc = new Vector3(Random.Range(10.0f, 900.0f), 10.0f, Random.Range(0.0f, 1000.0f));
            Instantiate(seaShell6, coralLoc, Quaternion.identity);
        }

        for (int i = 0; i < 10; i++)
        {
            Vector3 coralLoc = new Vector3(Random.Range(10.0f, 900.0f), 10.0f, Random.Range(0.0f, 1000.0f));
            Instantiate(seaShell11, coralLoc, Quaternion.identity);
        }

        for (int i = 0; i < 10; i++)
        {
            Vector3 coralLoc = new Vector3(Random.Range(10.0f, 900.0f), 10.0f, Random.Range(0.0f, 1000.0f));
            Instantiate(sponge1, coralLoc, Quaternion.identity);
        }

        for (int i = 0; i < 10; i++)
        {
            Vector3 coralLoc = new Vector3(Random.Range(10.0f, 900.0f), 10.0f, Random.Range(0.0f, 1000.0f));
            Instantiate(sponge2by2, coralLoc, Quaternion.identity);
        }

        for (int i = 0; i < 10; i++)
        {
            Vector3 coralLoc = new Vector3(Random.Range(10.0f, 900.0f), 10.0f, Random.Range(0.0f, 1000.0f));
            Instantiate(sponge3by3, coralLoc, Quaternion.identity);
        }

        for (int i = 0; i < 10; i++)
        {
            Vector3 coralLoc = new Vector3(Random.Range(10.0f, 900.0f), 10.0f, Random.Range(0.0f, 1000.0f));
            Instantiate(stone1by2, coralLoc, Quaternion.identity);
        }

        for (int i = 0; i < 10; i++)
        {
            Vector3 coralLoc = new Vector3(Random.Range(10.0f, 900.0f), 10.0f, Random.Range(0.0f, 1000.0f));
            Instantiate(stone2by3, coralLoc, Quaternion.identity);
        }

        for (int i = 0; i < 10; i++)
        {
            Vector3 coralLoc = new Vector3(Random.Range(20.0f, 900.0f), 10.0f, Random.Range(20.0f, 900.0f));
            Instantiate(stone2by4, coralLoc, Quaternion.identity);
        }

        for (int i = 0; i < 10; i++)
        {
            Vector3 coralLoc = new Vector3(Random.Range(10.0f, 900.0f), 10.0f, Random.Range(0.0f, 1000.0f));
            Instantiate(stone5by5, coralLoc, Quaternion.identity);
        }

        for (int i = 0; i < 10; i++)
        {
            Vector3 coralLoc = new Vector3(Random.Range(10.0f, 900.0f), 10.0f, Random.Range(0.0f, 1000.0f));
            Instantiate(stone5by6, coralLoc, Quaternion.identity);
        }
    }
}
