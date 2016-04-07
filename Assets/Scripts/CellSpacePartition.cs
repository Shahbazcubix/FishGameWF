using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;


public class CellSpacePartition : MonoBehaviour {
	
	public GameObject goldFish;
	public GameObject Dolfin;
	private GameObject dolfinUsing;
	public 	float wanderAngle = 5f;

	public float m_dWanderDistance = 2.0f;
	//public Vector3 m_vWanderTarget;
	public float m_dWanderRadius = 1.2f;
	public float m_dWanderJitter = 80.0f;

	public Dictionary<string, double> domDictionary = new Dictionary<string, double>();
	public double[] tempDouble = new double[2];

	public FuzzyLogicGui flg;
	public double m_dViewDistance;

	public bool fuzzifyInUse;

	private string fileName = "";

	private Animation anim;

	public GameObject hotLight;
	public GameObject coldLight;
	public GameObject hotLightPosition;

	public Text MateText;
	public int mateCount;
	public Text PredatorText;
	public int predatorCount;
	public Text PreyText;
	public int preyCount;
	public Text BabyText;
	public int babyCount;
	//public GameObject plant;
	public GameObject stone;
	public GameObject stone1by2;
	public GameObject stone2by3;
	public GameObject stone2by4;
	public GameObject stone5by5;
	public GameObject stone5by6;
	
	public GameObject seaweed;
	public GameObject seaweed5by5;
	public GameObject seaweed5by8;
	public GameObject coral1;
	public GameObject coral2;
	public GameObject coral4;
	public GameObject seaShell1;
	public GameObject seaShell6;
	public GameObject seaShell11;
	public GameObject sponge1;
	public GameObject sponge2by2;
	public GameObject sponge3by3;
	
	
	public GameObject fish;
	public GameObject fishDolphin;
	public Material fishMaterial;
	public Material mateMaterial;
	
	public int counter;
	
	public List<Cell> m_Cells = new List<Cell>();
	private List<GameObject> m_Neighbors;
	public List<GameObject> m_Vehicles = new List<GameObject> ();
	public List<GameObject> m_WanderList = new List<GameObject> ();
	public List<GameObject> m_PlantList = new List<GameObject> ();
	public List<GameObject> m_RedFish = new List<GameObject> ();

	private int MaxEntities = 300; //cannot handle 300 entities.  Maybe that's why not appear as in Netbeans steering behavior project...
	private double m_dSpaceWidth =    50;
	private double m_dSpaceHeight = 50;
	private double m_dSpaceDepth = 50;
	
	private CellSpacePartition m_pCellSpace;
	
	private int NumCellsX = 10; //7
	private int NumCellsY = 10;
	private int NumCellsZ = 10;
	private int NumAgents = 50;//why do all spheres go to x-z positive corner?
	private double cx = 1000;
	private double cy = 1000, cz = 1000;
	
	private int m_iNumCellsX;
	private int m_iNumCellsY;
	private int m_iNumCellsZ;
	private double m_dCellSizeX;
	private double m_dCellSizeY;
	private double m_dCellSizeZ;
	
	private BoundingCircle QueryCircle;
	private Vector3 TempTargetPos;
	private int TempQueryRadius;
	
	private Vector3 FeelerU, FeelerD, FeelerR, FeelerL, FeelerF;
	private Vector3 FeelerUNormal, FeelerDNormal, FeelerRNormal, FeelerLNormal, FeelerFNormal;
	private float DistToClosestIP, DistToThisIP;
	private Vector3 ClosestPoint;
	private Vector3 Overshoot;
	private bool WallCollision;
	private enum feeler { forward, up, down, left, right};
	private int caseSwitch;
	private float rayLength;
	
	
	public GameObject container;//do I need this?
	
	
	
	private float MaxSpeed;// = 150.0f;  What are good values for this and MaxSteeringForce?
	private float MaxSteeringForce;
	private Vector3 SteeringForceSum;
	private float VehicleMass;
	private float VehicleScale;
	private Vector3 m_vVelocity;
	private Vector3 Force;
	private float SeparationWeight;
	private float CohesionWeight;
	private float AlignmentWeight;
	private float ObstacleAvoidanceWeight;
	
	
	
	private Vector3 tempVector;
	private Vector3 tempHeadingOne;
	private Vector3 tempHeadingTwo;
	private Vector3 hitNormal;
	private Vector3 tempHeading;
	private Vector3 tempVelocity;
	
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
	
	private BallBounce tempScript; 
	private Vector3 m_vWanderTarget;
	//private float m_dWanderRadius;
	private float WanderDist;
	private float WanderWeight;
	
	public GameObject closestGO;
	public float closestFish;
	public Eat cp;
	public bool anotherFish;
	private int z;
	
	GameObject GoldFishPreyGO;
	GameObject KoiMateGO;
	public int AmberjackCount;

	private int numberGF;
	private int numberKoi;
	private int numberAJ;

	// Use this for initialization
	void Awake () {
		//setup the spatial subdivision class
		//    CellSpacePartition((double) cx, (double) cy,(double) cz,
		//               NumCellsX, NumCellsY, NumCellsZ, NumAgents);
		
		AmberjackCount = 0;

		m_dViewDistance = 50.0d;

		numberGF = 0;
		numberKoi = 0;
		numberAJ = 0;

		m_dSpaceWidth = cx;//1000
		m_dSpaceHeight = cy;
		m_dSpaceDepth = cz;
		
		m_iNumCellsX = NumCellsX;//10
		m_iNumCellsY = NumCellsY;
		m_iNumCellsZ = NumCellsZ;
		
		m_dCellSizeX = cx / NumCellsX; //10
		m_dCellSizeY = cy / NumCellsY;
		m_dCellSizeZ = cz / NumCellsZ;
		
		m_Neighbors = new List<GameObject> (MaxEntities); //100
		
		//create the cells.  Is this correct?  Yes. 
		for (int z = 0; z < m_iNumCellsZ; ++z) {
			for (int y = 0; y < m_iNumCellsY; ++y) {
				for (int x = 0; x < m_iNumCellsX; ++x) {
					double left = x * m_dCellSizeX;
					double right = left + m_dCellSizeX;
					double bot = y * m_dCellSizeY;
					double top = bot + m_dCellSizeY;
					double front = z * m_dCellSizeZ;
					double back = front + m_dCellSizeZ;
					
					Vector3 tempFront = new Vector3((float)left, (float)bot,(float)front);
					Vector3 tempBack = new Vector3((float)right, (float)top, (float)back);
					Vector3 sum = (tempFront + tempBack);
					Vector3 midPoint = sum/2;
					
					m_Cells.Add (new Cell (midPoint, 50.0f));//value is 50, but screen in netbeans if 500x500.  so, use 10.0f?
				}
			}
			
			
		}
		
		
		for (int i = 0; i < 100; i++) { //numAgents 50
			
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

		for (int i = 0; i < 100; i++) { //50
			
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

		
		for (int i = 1; i < 75; i++) {//30
			
			GameObject clone;
			Vector3 temp = new Vector3(Random.Range (30.0f, 900.0f), Random.Range (30.0f, 900.0f),Random.Range (30.0f, 900.0f));//for some reason all spheres move to one corner all time. 
			clone = Instantiate(fishDolphin, temp, Quaternion.identity) as GameObject;    
			clone.GetComponent<BallBounce>().setAmberjack ();
			//clone.GetComponent<BallBounce>().setFishNumber(i);
			////Debug.Log (clone.GetComponent<BallBounce>().getKoi());//this returns true!!!

			numberAJ++;

			AmberjackCount++;
			m_Vehicles.Add (clone);
			m_WanderList.Add (clone);
			this.AddEntity(clone);
			
		}

		GameObject clone1;
		Vector3 temp1 = new Vector3(Random.Range (100.0f, 900.0f), Random.Range (100.0f, 900.0f),Random.Range (100.0f, 900.0f));//for some reason all spheres move to one corner all time. 
		clone1 = Instantiate(Dolfin, temp1, Quaternion.identity) as GameObject; 
		dolfinUsing = clone1;
		clone1.GetComponent<BallBounce>().setDolphin ();
		clone1.GetComponent<BallBounce> ().setState (4);
		//clone.GetComponent<BallBounce>().setFishNumber(i);
		////Debug.Log (clone.GetComponent<BallBounce>().getKoi());//this returns true!!!
		/// 
		m_Vehicles.Add (clone1);
		//m_WanderList.Add (clone1);
		this.AddEntity(clone1);


		
		GameObject plantClone; 
		
		for (int j = 0; j < 10; j++) {
			Vector3 plantLoc = new Vector3 (Random.Range (10.0f, 980.0f), 20.0f, Random.Range (10.0f, 980.0f));
			plantClone = Instantiate (seaweed, plantLoc, Quaternion.identity) as GameObject;
			
			m_PlantList.Add (plantClone);
		}
		
		for (int j = 0; j < 10; j++) {
			Vector3 plantLoc = new Vector3 (Random.Range (10.0f, 980.0f), 20.0f, Random.Range (10.0f, 980.0f));
			plantClone = Instantiate (seaweed5by5, plantLoc, Quaternion.identity) as GameObject;
			//m_PlantList.Add (plantClone);
		}
		
		
		for (int j = 0; j < 10; j++) {
			Vector3 plantLoc = new Vector3 (Random.Range (10.0f, 980.0f), 20.0f, Random.Range (10.0f, 980.0f));
			plantClone = Instantiate (seaweed5by8, plantLoc, Quaternion.identity) as GameObject;
			//m_PlantList.Add (plantClone);
		}
		
		
		//    Instantiate (plant, plantLoc, Quaternion.identity);
		for (int i = 0; i < 10; i++) {
			Vector3 stoneLoc = new Vector3 (Random.Range (10.0f, 900.0f), 10.0f, Random.Range (0.0f,1000.0f));
			Instantiate (stone, stoneLoc, Quaternion.identity);
		}
		
		for (int i = 0; i < 10; i++) {
			Vector3 coralLoc = new Vector3 (Random.Range (10.0f, 900.0f), 10.0f, Random.Range (0.0f,1000.0f));
			Instantiate (coral1, coralLoc, Quaternion.identity);
		}
		
		for (int i = 0; i < 10; i++) {
			Vector3 coralLoc = new Vector3 (Random.Range (10.0f, 900.0f), 10.0f, Random.Range (0.0f,1000.0f));
			Instantiate (coral2, coralLoc, Quaternion.identity);
		}
		
		for (int i = 0; i < 10; i++) {
			Vector3 coralLoc = new Vector3 (Random.Range (10.0f, 900.0f), 10.0f, Random.Range (0.0f,1000.0f));
			Instantiate (coral4, coralLoc, Quaternion.identity);
		}
		
		for (int i = 0; i < 10; i++) {
			Vector3 coralLoc = new Vector3 (Random.Range (10.0f, 900.0f), 10.0f, Random.Range (0.0f,1000.0f));
			Instantiate (seaShell1, coralLoc, Quaternion.identity);
		}
		
		for (int i = 0; i < 10; i++) {
			Vector3 coralLoc = new Vector3 (Random.Range (10.0f, 900.0f), 10.0f, Random.Range (0.0f,1000.0f));
			Instantiate (seaShell6, coralLoc, Quaternion.identity);
		}
		
		for (int i = 0; i < 10; i++) {
			Vector3 coralLoc = new Vector3 (Random.Range (10.0f, 900.0f), 10.0f, Random.Range (0.0f,1000.0f));
			Instantiate (seaShell11, coralLoc, Quaternion.identity);
		}
		
		for (int i = 0; i < 10; i++) {
			Vector3 coralLoc = new Vector3 (Random.Range (10.0f, 900.0f), 10.0f, Random.Range (0.0f,1000.0f));
			Instantiate (sponge1, coralLoc, Quaternion.identity);
		}
		
		for (int i = 0; i < 10; i++) {
			Vector3 coralLoc = new Vector3 (Random.Range (10.0f, 900.0f), 10.0f, Random.Range (0.0f,1000.0f));
			Instantiate (sponge2by2, coralLoc, Quaternion.identity);
		}
		
		for (int i = 0; i < 10; i++) {
			Vector3 coralLoc = new Vector3 (Random.Range (10.0f, 900.0f), 10.0f, Random.Range (0.0f,1000.0f));
			Instantiate (sponge3by3, coralLoc, Quaternion.identity);
		}
		
		for (int i = 0; i < 10; i++) {
			Vector3 coralLoc = new Vector3 (Random.Range (10.0f, 900.0f), 10.0f, Random.Range (0.0f,1000.0f));
			Instantiate (stone1by2, coralLoc, Quaternion.identity);
		}
		
		for (int i = 0; i < 10; i++) {
			Vector3 coralLoc = new Vector3 (Random.Range (10.0f, 900.0f), 10.0f, Random.Range (0.0f,1000.0f));
			Instantiate (stone2by3, coralLoc, Quaternion.identity);
		}
		
		for (int i = 0; i < 10; i++) {
			Vector3 coralLoc = new Vector3 (Random.Range (20.0f, 900.0f), 10.0f, Random.Range (20.0f,900.0f));
			Instantiate (stone2by4, coralLoc, Quaternion.identity);
		}
		
		for (int i = 0; i < 10; i++) {
			Vector3 coralLoc = new Vector3 (Random.Range (10.0f, 900.0f), 10.0f, Random.Range (0.0f,1000.0f));
			Instantiate (stone5by5, coralLoc, Quaternion.identity);
		}
		
		for (int i = 0; i < 10; i++) {
			Vector3 coralLoc = new Vector3 (Random.Range (10.0f, 900.0f), 10.0f, Random.Range (0.0f,1000.0f));
			Instantiate (stone5by6, coralLoc, Quaternion.identity);
		}

		hotLightPosition = Instantiate (hotLight, new Vector3 (500.0f, 900.0f, 500.0f), Quaternion.identity) as GameObject;
		//Instantiate (coldLight, new Vector3 (100.0f, 900.0f, 100.0f), Quaternion.identity);

		flg = GameObject.Find ("Main Camera").gameObject.GetComponent<FuzzyLogicGui> ();
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
		ObstacleAvoidanceWeight = 0.7f;//what is a good weight for testing this? .o5
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

		/*
		fileName = System.Environment.GetFolderPath (System.Environment.SpecialFolder.DesktopDirectory);
		fileName += "/Myfile.txt";


		BallBounce temp = m_Vehicles [0].gameObject.GetComponent<BallBounce> ();
		temp.setTimer (0.0f);
		m_Vehicles [0].transform.position = new Vector3 (500.0f, 800.0f, 500.0f);
		temp.setHunger (0.0f);
		temp.setLibido (0.0f);

		BallBounce temp1 = m_Vehicles [1].gameObject.GetComponent<BallBounce> ();
		temp1.setTimer (30.0f);
		m_Vehicles [1].transform.position = new Vector3 (500.0f, 500.0f, 500.0f);
		temp1.setHunger (10.0f);
		temp1.setLibido (10.0f);

	
		BallBounce temp2 = m_Vehicles [2].gameObject.GetComponent<BallBounce> ();
		temp2.setTimer (0.0f);
		m_Vehicles [2].transform.position = new Vector3 (560.0f, 100.0f, 500.0f);
		temp2.setHunger (0.0f);
		temp2.setLibido (0.0f);
			*/
	}
	
	void Update()
	{
		
		for (int i = 0; i < m_Vehicles.Count; i++) {
			//bool anothFish = false;
			closestFish = 1000.0f;
			
			tempScript = m_Vehicles [i].GetComponent<BallBounce> ();

			setAnimationSpeed(i);

			double tempHunger = (double)tempScript.getHunger ();
			double tempLibido = (double)tempScript.getLibido ();

			if (tempScript.getDolphin ()) {
				tempScript.setTimer (0);
				tempScript.setState (4);
				//DolphinVel = tempScript.getVelocity();
			}

			float seconds = tempScript.getTimer ();
			if (seconds > 50.0f && tempScript.getState () != 3 && tempScript.getState () != 5 && tempScript.getState () != 1 && tempScript.getState () != 2 &&
			    tempScript.getState () != 6) {
				////Debug.Log ("changed state to Wander");
				tempScript.setState (4);
				tempScript.setTimer (0.0f);
			}
			
			int state = tempScript.getState ();
			GameObject closestGfMateGO = null;
			float closestGoldFishFloat = 1000.0f;
			
			
			
			if (state == 4 && tempScript.getGoldfish()) {
				////Debug.Log ("Entered Loop!!!"); Working!!!
				
				//this is to find mate...
				for (int j = 0; j < m_WanderList.Count; j++) {
					if (m_WanderList[j] != null) {
						BallBounce tempState = m_WanderList [j].GetComponent<BallBounce> ();
						if (tempState.getGoldfish()) { //check for all goldfish, in any state..
							bool areEqual = System.Object.ReferenceEquals (m_Vehicles [i], m_WanderList [j]);
							if (!areEqual) {
								float temp = Vector3.Distance (m_Vehicles [i].transform.position, m_WanderList [j].transform.position);
								if (temp < closestGoldFishFloat) {
									closestGoldFishFloat = temp;
									closestGfMateGO = m_WanderList [j];
									//anothFish = true;
									tempScript.setAnotherFish (true);//when do you reset this to false????  This is reset to false...
									//closestGO.gameObject.GetComponent<BallBounce>().setAnotherFish(true);
								}
							}
						}
					}
				}
				
				float closestPlantDouble = 1000.0f;
				GameObject closestPlantGO = null;
				Vector3 tempPosition = Vector3.zero;
				
				for (int k = 0; k < m_PlantList.Count; k++) {
					
					tempPosition = m_PlantList[k].transform.position;
					tempPosition.y += 500.0f;
					
					float temp = Vector3.Distance (m_Vehicles[i].transform.position, tempPosition);
					
					if (temp < closestPlantDouble) {
						closestPlantDouble = temp;
						closestPlantGO = m_PlantList[k];
					}
				}
				
				if (tempScript.getAnotherFish ()) {
					//
				//	if (!fuzzifyInUse) {
					//	fuzzifyInUse = true;
					//	domDictionary.Clear ();
					//	flg.setBoolDictionary(true);
						float tempCold = Vector3.Distance (m_Vehicles[i].transform.position, hotLightPosition.transform.position);
						tempDouble = cp.GetDesirability ((double)closestPlantDouble, (double)closestGoldFishFloat, tempHunger, tempLibido, tempCold, i);
						//if (tempScript.getFishNumber() == 10) {
							//flg.enterNumber(i);
						//}
					//	fuzzifyInUse = false;
					//	flg.setBoolDictionary(false);

						if (tempCold < 300.0f) {
							m_Vehicles[i].GetComponent<BallBounce>().setState (4);
							continue;
						}
					//}



					if (tempDouble [0] > tempDouble [1]) {
						//BallBounce tempPrey = closestGO.GetComponent<BallBounce> ();
						//Debug.Log ("EEEAAAATTTTT!!!!!!");
						tempScript.setState (1);//eat
						//tempPrey.setState (2);//flee
						tempScript.setPrey (closestPlantGO);
						//tempPrey.setPredator (m_Vehicles [i]);
						tempScript.setAnotherFish (false);
						//tempPrey.setAnotherFish (false);
						//tempScript.setHunger (0.0f);//set hunger to 0 after eat!!!
						
						//predatorCount++;
						//PredatorText.text = "# Predators: " + predatorCount.ToString();
						//preyCount++;
						//PreyText.text = "# Prey: " + preyCount.ToString();
						
						////Debug.Log (m_Vehicles[i].GetComponent<BallBounce>().getKoi ());
						/*
                        if (m_Vehicles[i].GetComponent<BallBounce>().getKoi ()) {
                            GameObject child = m_Vehicles[i].transform.Find("FishKoiMesh").gameObject;
                            
                            
                            if (child != null) {
                                
                                Renderer rend = child.GetComponent<Renderer>();
                                //Material material = new Material(fishMaterial);
                                
                                rend.material.color = Color.red;
                            }
                        }
                        */
						
						////Debug.Log (m_Vehicles[i].GetComponent<BallBounce>().getGoldfish());
						if (m_Vehicles[i].GetComponent<BallBounce>().getGoldfish())
						{
							GameObject child = m_Vehicles[i].transform.Find("GoldFishMesh").gameObject;
							
							
							if (child != null) {
								
								Renderer rend = child.GetComponent<Renderer>();
								//Material material = new Material(fishMaterial);
								
								rend.material.color = Color.red;
							}
							
							
						}
						/*
                        if (closestGO.GetComponent<BallBounce>().getKoi ()) {
                            GameObject child = closestGO.transform.Find ("FishKoiMesh").gameObject;
                            
                            
                            if (child != null) {
                                
                                Renderer rend = child.GetComponent<Renderer>();
                                //Material material = new Material(fishMaterial);
                                
                                rend.material.color = Color.yellow;
                            }
                        }
                        
                        if (closestGO.GetComponent<BallBounce>().getGoldfish ()) {
                            GameObject child = closestGO.transform.Find ("GoldFishMesh").gameObject;
                            
                            
                            if (child != null) {
                                
                                Renderer rend = child.GetComponent<Renderer>();
                                //Material material = new Material(fishMaterial);
                                
                                rend.material.color = Color.yellow;
                            }
                        }
                        */
						
						
					} else {
						
						if (closestGfMateGO == null) {
							
							continue;
						}
						
						closestGfMateGO.gameObject.GetComponent<BallBounce> ().setAnotherFish (true);
						counter++;
						//mateCount += 2;
						//MateText.text = "# fish mating: " + mateCount.ToString();
						////Debug.Log ("MATE" + counter);
						
						BallBounce tempClosest = closestGfMateGO.GetComponent<BallBounce> ();  
						
						//if closest fish is hunting, leave fish alone and no mating.  
						if (tempClosest.getState () == 1) {
							tempClosest.setAnotherFish (false);
							tempScript.setState (0);
							tempScript.setTimer (0.0f);
							continue;
						}
						
						//if closest fish is mating, interfere and mate  
						if (tempClosest.getState () == 3) {
							GameObject tempM = tempClosest.getMate ();
							tempM.gameObject.GetComponent<BallBounce>().setState (0);
							tempM.gameObject.GetComponent<BallBounce>().setMate(null);
							tempM.gameObject.GetComponent<BallBounce>().setIsMating(false);
							tempClosest.setMate (null);
							tempClosest.setIsMating(false);

							//change closest goldfish's mate back to yellow.  
							GameObject child = tempM.transform.Find ("GoldFishMesh").gameObject;
							
							if (child != null) {
								
								Renderer rend = child.GetComponent<Renderer> ();
								//Material material = new Material(fishMaterial);
								
								rend.material.color = Color.yellow;

							}
						}
						
						//if closest fish is fleeing, leave fish alone and not mating.  
						if (tempClosest.getState () == 2) {
							tempClosest.setAnotherFish (false);
							tempScript.setState (0);
							tempScript.setTimer (0.0f);
							continue;
						}
						
						//is closest fish is spawning, then leave fish alone and no mating.  
						if (tempClosest.getState () == 5) {
							tempClosest.setAnotherFish (false);
							tempScript.setState (0);
							tempScript.setTimer (0.0f);
							continue;
						}
						
						//tempScript.setLibido (0.0f);                
						//otherwise, Mate
						tempScript.setState (3);
						tempScript.setTimer (0.0f);
						tempScript.setIsMating (true);
						
						
						tempClosest.setState (3);
						tempScript.setMate (closestGfMateGO);
						tempClosest.setMate (m_Vehicles [i]);    
						tempClosest.setIsMating (true);
						tempClosest.setTimer (0.0f);
						tempScript.setAnotherFish (false);
						tempClosest.setAnotherFish (false);
						/*
                        if (m_Vehicles[i].GetComponent<BallBounce>().getKoi ()) {
                            GameObject child = m_Vehicles[i].transform.Find("FishKoiMesh").gameObject;
                            
                            if (child != null) {
                                
                                Renderer rend = child.GetComponent<Renderer>();
                                //Material material = new Material(fishMaterial);
                                
                                rend.material.color = Color.green;
                            }
                        }
                        */
						if (m_Vehicles[i].GetComponent<BallBounce>().getGoldfish ()) {
							GameObject child = m_Vehicles[i].transform.Find("GoldFishMesh").gameObject;
							
							if (child != null) {
								
								Renderer rend = child.GetComponent<Renderer>();
								//Material material = new Material(fishMaterial);
								
								rend.material.color = Color.green;
							}
						}
						/*
                        if (closestGfMateGO.GetComponent<BallBounce>().getKoi ()) {
                            GameObject child = closestGfMateGO.transform.Find ("FishKoiMesh").gameObject;
                            
                            if (child != null) {
                                
                                Renderer rend = child.GetComponent<Renderer>();
                                //Material material = new Material(fishMaterial);
                                
                                rend.material.color = Color.green;
                            }
                        }
                        */
						if (closestGfMateGO.GetComponent<BallBounce>().getGoldfish ()) {
							GameObject child = closestGfMateGO.transform.Find ("GoldFishMesh").gameObject;
							
							if (child != null) {
								
								Renderer rend = child.GetComponent<Renderer>();
								//Material material = new Material(fishMaterial);
								
								rend.material.color = Color.green;
							}
						}
						//m_Vehicles[i].gameObject.GetComponent<Renderer>().sharedMaterial.color = Color.green;
						//    closestGO.gameObject.GetComponent<Renderer>().material.color = Color.green;
					}
				}
			}
			
			//GameObject GoldFishPreyGO = null;
			//GameObject KoiMateGO = null;
			float closestKoiFloat = 1000.0f;
			float closestGfFloat = 1000.0f;
			
			
			
			if (state == 4 && tempScript.getKoi ()) {
				////Debug.Log ("Entered Loop!!!"); Working!!!
				for (int j = 0; j < m_WanderList.Count; j++) {
					//    if (m_WanderList[j] != null) {
					BallBounce tempState = m_WanderList [j].GetComponent<BallBounce> ();
					
					if (tempState.getGoldfish ()) {
						//bool areEqual = System.Object.ReferenceEquals (m_Vehicles [i], m_WanderList [j]);
						//if (!areEqual) {
						float temp = Vector3.Distance (m_Vehicles [i].transform.position, m_WanderList [j].transform.position);
						if (temp < closestGfFloat) {
							closestGfFloat = temp;
							GoldFishPreyGO = m_WanderList [j];
							//anothFish = true;
							tempScript.setAnotherPrey (true);//when do you reset this to false????  This is reset to false...
							//GoldFishPreyGO.gameObject.GetComponent<BallBounce>().setAnotherFish(true);
						}
						//}
						
						
					} //here
					
					
					if (tempState.getKoi ()) {
						bool areEqual = System.Object.ReferenceEquals (m_Vehicles [i], m_WanderList [j]);
						if (!areEqual) {
							float temp = Vector3.Distance (m_Vehicles [i].transform.position, m_WanderList [j].transform.position);
							if (temp < closestKoiFloat) {
								closestKoiFloat = temp;
								KoiMateGO = m_WanderList [j];
								//anothFish = true;
								tempScript.setAnotherMate (true);//when do you reset this to false????  This is reset to false...
								//GoldFishPreyGO.gameObject.GetComponent<BallBounce>().setAnotherFish(true);
							}
						}
						
						
					} //here
					
					
					//}
				}
				
				
				//GoldFishPreyGO.gameObject.GetComponent<BallBounce> ().setAnotherFish (true);//you don't need this because other fish's state would be eat or mate, not wander. 
				
				//if (!fuzzifyInUse) {
				//	fuzzifyInUse = true;
				//	domDictionary.Clear ();
				//	flg.setBoolDictionary(true);
					float tempCold = Vector3.Distance (m_Vehicles[i].transform.position, hotLightPosition.transform.position);
					tempDouble = cp.GetDesirability ((double)closestGfFloat, (double)closestKoiFloat, tempHunger, tempLibido, tempCold, i);
					//if (tempScript.getFishNumber() == 10) {
						//flg.enterNumber(i);
					//}
				//	fuzzifyInUse = false;
				//	flg.setBoolDictionary(false);

					if (tempCold < 300.0f) {
						m_Vehicles[i].GetComponent<BallBounce>().setState (4);
						continue;
					}
				//}

				
				if (tempDouble [0] > tempDouble [1]) {
					if (tempScript.getAnotherPrey()) {
						BallBounce tempPrey = GoldFishPreyGO.GetComponent<BallBounce> ();
						
						//if prey Goldfish is hunting, change gf state to fleeing.
						if (tempPrey.getState () == 1) {
							tempPrey.setPrey (null);
							//continue;
						}
						
						//if prey is already fleeing, take over and hunt
						if (tempPrey.getState () == 2) {
							GameObject tempPr = tempPrey.getPredator();
							tempPr.gameObject.GetComponent<BallBounce>().setState (0);
							tempPr.gameObject.GetComponent<BallBounce>().setPrey(null);

							GameObject child = tempPr.transform.Find ("FishKoiMesh").gameObject;
							
							if (child != null) {
								
								Renderer rend = child.GetComponent<Renderer> ();
								//Material material = new Material(fishMaterial);
								
								rend.material.color = Color.white;
							}


							tempPrey.setPredator (null);
						}
						
						//if gf state is mating, change state to fleeing.
						if (tempPrey.getState () == 3) {
							GameObject tempM = tempPrey.getMate ();
							tempM.gameObject.GetComponent<BallBounce>().setState (0);
							tempM.gameObject.GetComponent<BallBounce>().setMate (null);
							tempM.gameObject.GetComponent<BallBounce>().setIsMating(false);


							GameObject child = tempM.transform.Find ("GoldFishMesh").gameObject;
							
							if (child != null) {
								
								Renderer rend = child.GetComponent<Renderer> ();
								//Material material = new Material(fishMaterial);
								
								rend.material.color = Color.yellow;
							}


							tempPrey.setMate (null);
							tempPrey.setIsMating(false);
						}
						
						//if gf state is fleeing, change fish's predator to new predator.  Remember to check when prey is gone, to change back to flock for
						//any predator fish.  
						//if (tempPrey.getState () == 2) {
						
						//}
						//if gf is spawning, leave alone and remain in wander state.
						if(tempPrey.getState () == 5) {
							tempScript.setAnotherPrey (false);
							tempScript.setState (0);
							tempScript.setTimer (0.0f);
							continue;
						}
						
						//Debug.Log ("EEEAAAATTTTT!!!!!!");
						tempScript.setState (1);//eat

						//add predator to flee from***********
						m_RedFish.Add (m_Vehicles[i]);

						tempPrey.setState (2);//flee
						tempScript.setPrey (GoldFishPreyGO);
						tempPrey.setPredator (m_Vehicles [i]);
						tempScript.setAnotherPrey (false);
						tempPrey.setAnotherPrey (false);
						//tempScript.setHunger (0.0f);
						
					//	predatorCount++;
					//	PredatorText.text = "# Predators: " + predatorCount.ToString();
					//	preyCount++;
					//	PreyText.text = "# Prey: " + preyCount.ToString();
						
						////Debug.Log (m_Vehicles[i].GetComponent<BallBounce>().getKoi ());
						
						if (m_Vehicles[i].GetComponent<BallBounce>().getKoi ()) {
							GameObject child = m_Vehicles[i].transform.Find("FishKoiMesh").gameObject;
							
							
							if (child != null) {
								
								Renderer rend = child.GetComponent<Renderer>();
								//Material material = new Material(fishMaterial);
								
								rend.material.color = Color.red;
							}
						}
						/*
                        ////Debug.Log (m_Vehicles[i].GetComponent<BallBounce>().getGoldfish());
                        if (m_Vehicles[i].GetComponent<BallBounce>().getGoldfish())
                        {
                            GameObject child = m_Vehicles[i].transform.Find("GoldFishMesh").gameObject;
                            
                            
                            if (child != null) {
                                
                                Renderer rend = child.GetComponent<Renderer>();
                                //Material material = new Material(fishMaterial);
                                
                                rend.material.color = Color.red;
                            }


                        }

                        if (GoldFishPreyGO.GetComponent<BallBounce>().getKoi ()) {
                            GameObject child = GoldFishPreyGO.transform.Find ("FishKoiMesh").gameObject;
                        
                        
                        if (child != null) {
                            
                            Renderer rend = child.GetComponent<Renderer>();
                            //Material material = new Material(fishMaterial);
                            
                            rend.material.color = Color.yellow;
                        }
                        }
*/
						
						if (GoldFishPreyGO.GetComponent<BallBounce>().getGoldfish ()) {
							GameObject child = GoldFishPreyGO.transform.Find ("GoldFishMesh").gameObject;
							
							
							if (child != null) {
								
								Renderer rend = child.GetComponent<Renderer>();
								//Material material = new Material(fishMaterial);
								
								rend.material.color = Color.yellow;
							}
						}

					}
					/*here*/} else {
					if (tempScript.getAnotherMate()) {
						BallBounce tempClosest = KoiMateGO.GetComponent<BallBounce> ();
						
						//if closest fish is hunting, leave alone, remain in flocking state.
						if (tempClosest.getState () == 1) {
							tempScript.setAnotherMate (false);
							tempScript.setState (0);
							tempScript.setTimer (0.0f);
							continue;
						}
						
						//is closest fish is fleeing, leave alone, remain in flocking state. 
						if (tempClosest.getState () == 2) {
							tempScript.setAnotherMate (false);
							tempScript.setState (0);
							tempScript.setTimer (0.0f);
							continue;
						}
						
						//if closest fish is mating, start new mating with present fish.
						if (tempClosest.getState () == 3) {
							GameObject tempM = tempClosest.getMate ();
							tempM.gameObject.GetComponent<BallBounce>().setMate (null);
							tempM.gameObject.GetComponent<BallBounce>().setState (0);
							tempM.gameObject.GetComponent<BallBounce>().setIsMating(false);

							GameObject child = tempM.transform.Find ("FishKoiMesh").gameObject;
							
							if (child != null) {
								
								Renderer rend = child.GetComponent<Renderer> ();
								//Material material = new Material(fishMaterial);
								
								rend.material.color = Color.white;
							}


							tempScript.setTimer (0.0f);
							tempClosest.setMate (null);
							tempClosest.setIsMating(false);
						}
						
						//if closest fish is spawning, leave alone and remain in wander state.
						if (tempClosest.getState () == 5) {
							tempScript.setAnotherMate (false);
							tempScript.setTimer (0.0f);
							tempScript.setState (0);
							continue;
						}
						
						counter++;
					//	mateCount += 2;
					//	MateText.text = "# fish mating: " + mateCount.ToString();
						//Debug.Log ("MATE" + counter);
						//tempScript.setLibido (0.0f);                
						tempScript.setState (3);
						tempScript.setTimer (0.0f);
						tempScript.setIsMating (true);
						
						
						tempClosest.setState (3);
						tempScript.setMate (KoiMateGO);
						tempClosest.setMate (m_Vehicles [i]);    
						tempClosest.setIsMating (true);
						tempClosest.setTimer (0.0f);
						tempScript.setAnotherMate (false);
						tempClosest.setAnotherMate (false);
						
						if (m_Vehicles[i].GetComponent<BallBounce>().getKoi ()) {
							GameObject child = m_Vehicles[i].transform.Find("FishKoiMesh").gameObject;
							
							if (child != null) {
								
								Renderer rend = child.GetComponent<Renderer>();
								//Material material = new Material(fishMaterial);
								
								rend.material.color = Color.green;
							}
						}
						/*
                        if (m_Vehicles[i].GetComponent<BallBounce>().getGoldfish ()) {
                            GameObject child = m_Vehicles[i].transform.Find("GoldFishMesh").gameObject;
                            
                            if (child != null) {
                                
                                Renderer rend = child.GetComponent<Renderer>();
                                //Material material = new Material(fishMaterial);
                                
                                rend.material.color = Color.green;
                            }
                        }
*/
						if (KoiMateGO.GetComponent<BallBounce>().getKoi ()) {
							GameObject child = KoiMateGO.transform.Find ("FishKoiMesh").gameObject;
							
							if (child != null) {
								
								Renderer rend = child.GetComponent<Renderer>();
								//Material material = new Material(fishMaterial);
								
								rend.material.color = Color.green;
							}
						}
						/*
                        if (KoiMateGO.GetComponent<BallBounce>().getGoldfish ()) {
                            GameObject child = KoiMateGO.transform.Find ("GoldFishMesh").gameObject;
                            
                            if (child != null) {
                                
                                Renderer rend = child.GetComponent<Renderer>();
                                //Material material = new Material(fishMaterial);
                                
                                rend.material.color = Color.green;
                            }
                        }*/
					}
					//m_Vehicles[i].gameObject.GetComponent<Renderer>().sharedMaterial.color = Color.green;
					//    GoldFishPreyGO.gameObject.GetComponent<Renderer>().material.color = Color.green;
				}//here
				
			}
			
			GameObject closestEitherFish = null;
			GameObject DolphinMateGO = null;
			float closestFishFloat = 1000.0f;
			float closestDolphinFloat = 1000.0f;
			
			tempScript.setAnotherPrey (false);
			tempScript.setAnotherMate (false);
			
			if (state == 4 && tempScript.getAmberjack ()) {
				////Debug.Log ("Entered Loop!!!"); Working!!!
				for (int j = 0; j < m_WanderList.Count; j++) {
					//    if (m_WanderList[j] != null) {
					BallBounce tempState = m_WanderList [j].GetComponent<BallBounce> ();
					
					if (tempState.getKoi ()) {
						//bool areEqual = System.Object.ReferenceEquals (m_Vehicles [i], m_WanderList [j]);
						//if (!areEqual) {
						float temp = Vector3.Distance (m_Vehicles [i].transform.position, m_WanderList [j].transform.position);
						if (temp < closestFishFloat) {
							closestFishFloat = temp;
							closestEitherFish = m_WanderList [j];
							//anothFish = true;
							tempScript.setAnotherPrey (true);//when do you reset this to false????  This is reset to false...
							//GoldFishPreyGO.gameObject.GetComponent<BallBounce>().setAnotherFish(true);
						}
						//}
						
						
					} //here
					
					
					if (tempState.getAmberjack ()) {
						bool areEqual = System.Object.ReferenceEquals (m_Vehicles [i], m_WanderList [j]);
						if (!areEqual) {
							float temp = Vector3.Distance (m_Vehicles [i].transform.position, m_WanderList [j].transform.position);
							if (temp < closestDolphinFloat) {
								closestDolphinFloat = temp;
								DolphinMateGO = m_WanderList [j];
								//anothFish = true;
								tempScript.setAnotherMate (true);//when do you reset this to false????  This is reset to false...
								//GoldFishPreyGO.gameObject.GetComponent<BallBounce>().setAnotherFish(true);
							}
						}
						
						
					} //here
					
					
					
					//}
				}
				
				
				//GoldFishPreyGO.gameObject.GetComponent<BallBounce> ().setAnotherFish (true);//you don't need this because other fish's state would be eat or mate, not wander. 
				
				//if (!fuzzifyInUse) {
				//	fuzzifyInUse = true;
				//	domDictionary.Clear ();
				//	flg.setBoolDictionary(true);
					float tempCold = Vector3.Distance (m_Vehicles[i].transform.position, hotLightPosition.transform.position);
					tempDouble = cp.GetDesirability ((double)closestFishFloat, (double)closestDolphinFloat, tempHunger, tempLibido, tempCold, i);
					//if (tempScript.getFishNumber() == 10) {
						//flg.enterNumber(i);
				//	}
				//	fuzzifyInUse = false;
				//	flg.setBoolDictionary(false);

					if (tempCold < 300.0f) {
						m_Vehicles[i].GetComponent<BallBounce>().setState (4);
						continue;
					}

				//}

				
				if (tempDouble [0] > tempDouble [1]) {
					if (tempScript.getAnotherPrey()) {
						BallBounce tempPrey = closestEitherFish.GetComponent<BallBounce> ();
						
						//if closest fish is hunting, set its prey to flock, and hunt the hunter
						if(tempPrey.getState () == 1) {
							GameObject tempP = tempPrey.getPrey ();
							tempP.gameObject.GetComponent<BallBounce>().setPredator(null);
							tempP.gameObject.GetComponent<BallBounce>().setState (0);
							tempPrey.setPrey (null);
						}
						
						//if prey is already fleeing, take over as predator. 
						if (tempPrey.getState () == 2) {
							GameObject tempPr = tempPrey.getPredator ();
							tempPr.gameObject.GetComponent<BallBounce>().setPrey (null);
							tempPr.gameObject.GetComponent<BallBounce>().setState (0);

							GameObject child = tempPr.transform.Find ("AmberjackMesh").gameObject;
							
							if (child != null) {
								
								Renderer rend = child.GetComponent<Renderer> ();
								//Material material = new Material(fishMaterial);
								
								rend.material.color = Color.gray;
							}

							tempPrey.setPredator (null);
						}
						
						//if prey is mating, end mating and become prey.
						if (tempPrey.getState () == 3) {
							GameObject tempM = tempPrey.getMate ();
							tempM.gameObject.GetComponent<BallBounce>().setState (0);
							tempM.gameObject.GetComponent<BallBounce>().setMate(null);
							tempM.gameObject.GetComponent<BallBounce>().setIsMating(false);

							GameObject child = tempM.transform.Find ("FishKoiMesh").gameObject;
							
							if (child != null) {
								
								Renderer rend = child.GetComponent<Renderer> ();
								//Material material = new Material(fishMaterial);
								
								rend.material.color = Color.white;
							}


							tempPrey.setIsMating(false);
							tempPrey.setMate (null);
						}

						//if prey is spawning, leave alone.  
						if (tempPrey.getState () == 5) {
							tempScript.setAnotherPrey(false);
							tempScript.setState (0);
							tempScript.setTimer (0.0f);
							continue;
						}
						
						//Debug.Log ("EEEAAAATTTTT!!!!!!");
						tempScript.setState (1);//eat

						//Add predator to flee from
						m_RedFish.Add (m_Vehicles[i]);

						tempPrey.setState (2);//flee
						tempScript.setPrey (closestEitherFish);
						tempPrey.setPredator (m_Vehicles [i]);
						tempScript.setAnotherPrey (false);
						tempPrey.setAnotherPrey (false);
						//tempScript.setHunger (0.0f);
						
						//predatorCount++;
						//PredatorText.text = "# Predators: " + predatorCount.ToString();
						//preyCount++;
						//PreyText.text = "# Prey: " + preyCount.ToString();
						
						////Debug.Log (m_Vehicles[i].GetComponent<BallBounce>().getKoi ());
						
						if (m_Vehicles[i].GetComponent<BallBounce>().getAmberjack ()) {
							GameObject child = m_Vehicles[i].transform.Find("AmberjackMesh").gameObject;
							
							
							if (child != null) {
								
								Renderer rend = child.GetComponent<Renderer>();
								//Material material = new Material(fishMaterial);
								
								rend.material.color = Color.red;
							}
						}
						/*
                        ////Debug.Log (m_Vehicles[i].GetComponent<BallBounce>().getGoldfish());
                        if (m_Vehicles[i].GetComponent<BallBounce>().getGoldfish())
                        {
                            GameObject child = m_Vehicles[i].transform.Find("GoldFishMesh").gameObject;
                            
                            
                            if (child != null) {
                                
                                Renderer rend = child.GetComponent<Renderer>();
                                //Material material = new Material(fishMaterial);
                                
                                rend.material.color = Color.red;
                            }


                        }
*/
						if (closestEitherFish.GetComponent<BallBounce>().getKoi ()) {
							GameObject child = closestEitherFish.transform.Find ("FishKoiMesh").gameObject;
							
							
							if (child != null) {
								
								Renderer rend = child.GetComponent<Renderer>();
								//Material material = new Material(fishMaterial);
								
								rend.material.color = Color.yellow;
							}
						}
						
						/*
                        if (closestEitherFish.GetComponent<BallBounce>().getAmberjack ()) {
                            GameObject child = closestEitherFish.transform.Find ("AmberjackMesh").gameObject;
                            
                            
                            if (child != null) {
                                
                                Renderer rend = child.GetComponent<Renderer>();
                                //Material material = new Material(fishMaterial);
                                
                                rend.material.color = Color.yellow;
                            }
                        }*/
						
						
					} //end getAnotherPrey()
					/*here*/} else {
					if (tempScript.getAnotherMate()) {
						BallBounce tempClosest = DolphinMateGO.GetComponent<BallBounce> (); 

						//if closest fish is hunting, leave alone
						if(tempClosest.getState () == 1) {
							tempScript.setAnotherMate(false);
							tempScript.setTimer (0.0f);
							tempScript.setState (0);
							continue;
						}

						//if closest fish is mating, take over.  
						if (tempClosest.getState () == 3) {
							GameObject tempM = tempClosest.getMate ();
							tempM.gameObject.GetComponent<BallBounce>().setState (0);
							tempM.gameObject.GetComponent<BallBounce>().setMate(null);
							tempM.gameObject.GetComponent<BallBounce>().setIsMating(false);

							GameObject child = tempM.transform.Find ("AmberjackMesh").gameObject;
							
							if (child != null) {
								
								Renderer rend = child.GetComponent<Renderer> ();
								//Material material = new Material(fishMaterial);
								
								rend.material.color = Color.gray;
							}

							tempClosest.setIsMating(false);
							tempClosest.setMate (null);
						}
						
						counter++;
					//	mateCount += 2;
					//	MateText.text = "# fish mating: " + mateCount.ToString();
						//Debug.Log ("MATE" + counter);
						//tempScript.setLibido (0.0f);                
						tempScript.setState (3);
						tempScript.setTimer (0.0f);
						tempScript.setIsMating (true);
						
						tempClosest.setState (3);
						tempScript.setMate (DolphinMateGO);
						tempClosest.setMate (m_Vehicles [i]);    
						tempClosest.setIsMating (true);
						tempClosest.setTimer (0.0f);
						tempScript.setAnotherMate (false);
						tempClosest.setAnotherMate (false);
						
						if (m_Vehicles[i].GetComponent<BallBounce>().getAmberjack ()) {
							GameObject child = m_Vehicles[i].transform.Find("AmberjackMesh").gameObject;
							
							if (child != null) {
								
								Renderer rend = child.GetComponent<Renderer>();
								//Material material = new Material(fishMaterial);
								
								rend.material.color = Color.green;
							}
						}
						/*
                        if (m_Vehicles[i].GetComponent<BallBounce>().getGoldfish ()) {
                            GameObject child = m_Vehicles[i].transform.Find("GoldFishMesh").gameObject;
                            
                            if (child != null) {
                                
                                Renderer rend = child.GetComponent<Renderer>();
                                //Material material = new Material(fishMaterial);
                                
                                rend.material.color = Color.green;
                            }
                        }
*/
						if (DolphinMateGO.GetComponent<BallBounce>().getAmberjack ()) {
							GameObject child = DolphinMateGO.transform.Find ("AmberjackMesh").gameObject;
							
							if (child != null) {
								
								Renderer rend = child.GetComponent<Renderer>();
								//Material material = new Material(fishMaterial);
								
								rend.material.color = Color.green;
							}
						}
						/*
                        if (closestFish.GetComponent<BallBounce>().getGoldfish ()) {
                            GameObject child = closestFish.transform.Find ("GoldFishMesh").gameObject;
                            
                            if (child != null) {
                                
                                Renderer rend = child.GetComponent<Renderer>();
                                //Material material = new Material(fishMaterial);
                                
                                rend.material.color = Color.green;
                            }
                        }
                    }*/
						//m_Vehicles[i].gameObject.GetComponent<Renderer>().sharedMaterial.color = Color.green;
						//    GoldFishPreyGO.gameObject.GetComponent<Renderer>().material.color = Color.green;
					}//end isAnotherMate()  here
					
				} //end else
			} //end state 4 and isDolphin == true
			
			switch ((State)state) {
			case State.Flock:
				Flock (i);
				break;
				
			case State.Wander:
				Wander (i);
				break;
				
			case State.Mate:
				Mate (i);
				break;
				
			case State.Spawn:
				Spawn (i);
				break;
				
			case State.Eat:
				Predator (i);
				break;
				
			case State.Flee:
				Prey (i);
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
	
	
	public void Prey(int i) {
		
		GameObject tempPredator = tempScript.getPredator ();
		
		float dist = Vector3.Distance (m_Vehicles [i].transform.position, tempPredator.transform.position);


		if (dist < 100.0f) {
			tempScript.setState (6);
			tempScript.setTimer (0.0f);
			BallBounce temp = tempPredator.GetComponent<BallBounce> ();
			temp.setState (0);
			temp.setTimer (0.0f);
			temp.setHunger (0.0f);
			temp.setVelocity (Vector3.zero);

			
			if (tempPredator.GetComponent<BallBounce>().getKoi ()) {
				GameObject child = tempPredator.transform.Find ("FishKoiMesh").gameObject;
				
				if (child != null) {
					
					Renderer rend = child.GetComponent<Renderer> ();
					//Material material = new Material(fishMaterial);
					
					rend.material.color = Color.white;
				}
			}
			
			if (tempPredator.GetComponent<BallBounce>().getGoldfish ()) {
				GameObject child = tempPredator.transform.Find ("GoldFishMesh").gameObject;
				
				if (child != null) {
					
					Renderer rend = child.GetComponent<Renderer> ();
					//Material material = new Material(fishMaterial);
					
					rend.material.color = Color.yellow;
				}
			}
			
			if (tempPredator.GetComponent<BallBounce>().getAmberjack ()) {
				GameObject child = tempPredator.transform.Find ("AmberjackMesh").gameObject;
				
				if (child != null) {
					
					Renderer rend = child.GetComponent<Renderer> ();
					//Material material = new Material(fishMaterial);
					
					rend.material.color = Color.gray;
				}
			}
			
			
			//predatorCount--;
			//PredatorText.text = "# Predators: " + predatorCount.ToString ();
			
		}
		//for (int i = 0; i < m_Vehicles.Count; i++) {
		//BallBounce tempScript = m_Vehicles [i].GetComponent<BallBounce> ();
		tempVelocity = tempScript.getVelocity ();
		tempHeading = tempScript.getHeading ();
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
		
		
		CalculateNeighbors (m_Vehicles [i].transform.position, 10.0f); //this radius is not used to calculate neighbors for testing?
		
		//    AccumulateForce (Force);
		Force = Vector3.zero;
		
		//Force = Flee (tempPredator.transform.position, m_Vehicles [i]);
		Force = Evade (tempPredator, m_Vehicles [i]);

		AccumulateForce (Force);

		if (!m_Vehicles[i].GetComponent<BallBounce>().getDolphin())
			AvoidPredator (i, m_Vehicles[i]);

		Vector3 OldPos = m_Vehicles [i].transform.position;
		
		translatePosition (i);
		
		
		this.UpdateEntity (m_Vehicles [i], OldPos);
		//}
		
		
	}
	
	public void Predator(int i) {
		
		
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


		GameObject tempPrey = m_Vehicles [i].GetComponent<BallBounce> ().getPrey ();
		
		if (m_Vehicles [i].GetComponent<BallBounce> ().getGoldfish ()) {
			Vector3 tempLocation = Vector3.zero;
			float distance = 0.0f;
			
			
			
			if (tempPrey != null) {
				tempLocation = tempPrey.transform.position;
				float number = Random.Range (100.0f, 500.0f);
				tempLocation.y += number;
				distance = Vector3.Distance (m_Vehicles [i].transform.position, tempLocation);
			}
			
			if (distance < 100.0f) {
				BallBounce tempBB = m_Vehicles [i].GetComponent<BallBounce> ();
				tempBB.setState (0);
				tempBB.setTimer (0.0f);
				tempBB.setHunger (0.0f);
				tempVelocity = Vector3.zero;
				//tempBB.setVelocity (Vector3.zero);
				
				GameObject child = m_Vehicles [i].transform.Find ("GoldFishMesh").gameObject;
				
				if (child != null) {
					
					Renderer rend = child.GetComponent<Renderer> ();
					//Material material = new Material(fishMaterial);
					
					rend.material.color = Color.yellow;
				}
				
				
			}
		}
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
		
		CalculateNeighbors (m_Vehicles [i].transform.position, 10.0f); //this radius is not used to calculate neighbors for testing?
		
		//    AccumulateForce (Force);
		Force = Vector3.zero;
		
		if (m_Vehicles [i].GetComponent<BallBounce> ().getKoi () || m_Vehicles[i].GetComponent<BallBounce>().getAmberjack ()) {
			//tempPrey = tempScript.getPrey ();
			if (tempPrey != null) {
				Force = Pursuit (tempPrey, i);
			}
		}
		
		if (m_Vehicles [i].GetComponent<BallBounce> ().getGoldfish ()) {
			
			Vector3 tempLoc = Vector3.zero;
			if (tempPrey != null) {
				tempLoc = tempPrey.transform.position;
				tempLoc.y += 400.0f; //try to make this random number.
			}
			Force = Arrive (tempLoc, m_Vehicles[i]);
		}
		
		AccumulateForce (Force);

		if (!m_Vehicles[i].GetComponent<BallBounce>().getDolphin())
			AvoidPredator (i, m_Vehicles[i]);

		Vector3 OldPos = m_Vehicles [i].transform.position;
		
		translatePosition (i);
		
		
		this.UpdateEntity (m_Vehicles [i], OldPos);
		//}
		
		
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
			
			
			/*
            clone.GetComponent<BallBounce>().setVelocity(Vector3.zero);
            //clone.gameObject.GetComponent<BallBounce> ().setVelocity (Vector3.zero);
            //clone.gameObject.GetComponent<BallBounce> ().setHeading (Vector3.zero);
            m_Vehicles.Add (clone);
            m_WanderList.Add (clone);
            this.AddEntity (clone);
*/
			
			
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
	
	
	
	public void Mate(int i) {
		//for (int i = 0; i < m_Vehicles.Count; i++) {
		//BallBounce tempScript = m_Vehicles [i].GetComponent<BallBounce> ();
		GameObject tempMate = tempScript.getMate ();
		bool tMate = tempScript.getIsMating ();


		if (!tMate) {
			tempScript.setState(5);
			tempScript.setLibido (0.0f);
			//mateCount--;
		//	MateText.text = "# fish mating: " + mateCount.ToString();
			//m_Vehicles[i].gameObject.GetComponent<Renderer>().material.color = Color.clear;
			
			if (m_Vehicles[i].GetComponent<BallBounce>().getKoi ()) {
				GameObject child = m_Vehicles[i].transform.Find ("FishKoiMesh").gameObject;
				
				if (child != null) {
					
					Renderer rend = child.GetComponent<Renderer>();
					//Material material = new Material(fishMaterial);
					
					rend.material.color = Color.white;
				}
			}
			
			if (m_Vehicles[i].GetComponent<BallBounce>().getGoldfish ()) {
				GameObject child = m_Vehicles[i].transform.Find ("GoldFishMesh").gameObject;
				
				if (child != null) {
					
					Renderer rend = child.GetComponent<Renderer>();
					//Material material = new Material(fishMaterial);
					
					rend.material.color = Color.yellow;
				}
			}
			
			
			if (m_Vehicles[i].GetComponent<BallBounce>().getAmberjack ()) {
				GameObject child = m_Vehicles[i].transform.Find ("AmberjackMesh").gameObject;
				
				if (child != null) {
					
					Renderer rend = child.GetComponent<Renderer>();
					//Material material = new Material(fishMaterial);
					
					rend.material.color = Color.gray;
				}
			}
			
			
			//tempMate.gameObject.GetComponent<Renderer>().material = fishMaterial;
			//tempMate.gameObject.GetComponent<BallBounce>().setState(5);
		}
		
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
		
		
		CalculateNeighbors (m_Vehicles [i].transform.position, 10.0f); //this radius is not used to calculate neighbors for testing?
		
		//    AccumulateForce (Force);
		Force = Vector3.zero;
		//GameObject tempMate = tempScript.getMate ();

		if (!m_Vehicles[i].GetComponent<BallBounce>().getDolphin())
			AvoidPredator (i, m_Vehicles[i]);

		Force = Vector3.zero;

		//Force = Arrive (tempMate.transform.position, m_Vehicles[i]);
		if (tempMate == null) {
			tempScript.setState (0);
		} else {
			Force = Seek (tempMate.transform.position, m_Vehicles [i]);
		}
		
		//    //Debug.Log (Force);
		/*float temp = Vector3.Distance (tempMate.transform.position, m_Vehicles[i].transform.position);

        if (temp < 10.0f) {
            //Debug.Log ("Collision!!!!!!!");
            tempVelocity = Vector3.zero;
            SteeringForceSum = Vector3.zero;
        }*/
		
		AccumulateForce (Force);



		Vector3 OldPos = m_Vehicles [i].transform.position;
		
		translatePosition (i);
		
		
		this.UpdateEntity (m_Vehicles [i], OldPos);
		//}
	}
	
	
	
	
	public void Wander(int i){
		

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
		//CalculateNeighbors (m_Vehicles [i].transform.position, 10.0f); //this radius is not used to calculate neighbors for testing?
		
		//    Force = Vector3.zero;
		
		//Force = Wander () * WanderWeight;

		if (!m_Vehicles[i].GetComponent<BallBounce>().getDolphin())
			AvoidPredator (i, m_Vehicles[i]);


		//FlockingForce (i);
		Force = Vector3.zero;
		FlockingForce (i);

		//AccumulateForce (Force);


		Vector3 OldPos = m_Vehicles [i].transform.position;
		
		translatePosition (i);
		
		
		this.UpdateEntity (m_Vehicles [i], OldPos);
		//}
		
	}
	
	
	
	
	public void Flock(int i)
	{
		
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
		CalculateNeighbors (m_Vehicles [i].transform.position, 50.0f); //this radius is not used to calculate neighbors for testing?
		//TagVehiclesWithinViewRange (m_Vehicles [i], m_Vehicles, 200.0d);
		//    Force = Vector3.zero;
		
		//Force = Wander () * WanderWeight;

		if (!m_Vehicles[i].GetComponent<BallBounce>().getDolphin())
			AvoidPredator (i, m_Vehicles[i]);

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
	
	private Vector3 Wander(GameObject ent)
	{
		//float m_dWanderJitter = 1.0f * Time.deltaTime;
		//m_dWanderJitter = Mathf.Clamp (m_dWanderJitter, 0.0f, 100.0f);
		float m_dWanderJitter = 80.0f;
		float JitterThisTimeSlice = m_dWanderJitter * Time.deltaTime;
		
		//BallBounce tempScript = ent.GetComponent<BallBounce> ();
		Vector3 tempTarget = new Vector3 (0.0f, 0.0f, 0.0f);
		tempTarget = tempScript.getTarget();
		
		Vector3 tempWander = new Vector3 (Random.value * JitterThisTimeSlice, Random.value * JitterThisTimeSlice, Random.value * JitterThisTimeSlice);
		
		tempTarget += tempWander;
		
		
		
		tempTarget = tempTarget.normalized;
		
		tempTarget *= 2.4f;
		
		tempScript.setTarget (tempTarget);
		Vector3 MainTarget = new Vector3 (0.0f, 0.0f, 0.0f);
		
		MainTarget = ent.transform.position;
		MainTarget.x += 2.4f * Mathf.Cos (Random.value * Mathf.PI * 2.0f); //1.2
		MainTarget.y += 2.4f * Mathf.Cos (Random.value * Mathf.PI * 2.0f); //.5
		MainTarget.z += 2.4f * Mathf.Sin (Random.value * Mathf.PI * 2.0f);
		
		
		
		//tempTarget + tempVelocity * 2.0f;//this is problem!!!
		//MainTarget += ent.transform.position + ent.transform.forward;
		
		//Vector3 Target = new Vector3(0.0f,0.0f,0.0f);
		//Target = transform.TransformPoint(MainTarget);
		
		return Seek (MainTarget, ent);
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
			//Debug.Log (FeelerFNormal* Overshoot.magnitude * ObstacleAvoidanceWeight);//values are normal.
			AccumulateForce (Force);
			break;
		case 1: 
			Overshoot = m_Vehicles [i].transform.position + FeelerU - ClosestPoint;
			Force = FeelerUNormal * Overshoot.magnitude * ObstacleAvoidanceWeight;
			//Debug.Log (FeelerUNormal* Overshoot.magnitude * ObstacleAvoidanceWeight);
			AccumulateForce (Force);
			break;
			
		case 2: 
			Overshoot = m_Vehicles [i].transform.position + FeelerD - ClosestPoint;
			Force = FeelerDNormal * Overshoot.magnitude * ObstacleAvoidanceWeight;
			//Debug.Log ( FeelerDNormal* Overshoot.magnitude * ObstacleAvoidanceWeight);
			AccumulateForce (Force);
			break;
			
		case 3:
			Overshoot = m_Vehicles [i].transform.position + FeelerL - ClosestPoint;
			Force = FeelerLNormal * Overshoot.magnitude * ObstacleAvoidanceWeight;
			//Debug.Log (FeelerLNormal* Overshoot.magnitude * ObstacleAvoidanceWeight);
			AccumulateForce (Force);
			break;
			
		case 4:
			Overshoot = m_Vehicles [i].transform.position + FeelerR - ClosestPoint;
			Force = FeelerRNormal * Overshoot.magnitude * ObstacleAvoidanceWeight;
			//Debug.Log (FeelerRNormal* Overshoot.magnitude * ObstacleAvoidanceWeight);
			AccumulateForce (Force);
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
		
		tempVelocity += acceleration * Time.deltaTime;  //what is value of Time.deltaTime vs netbeans time function???
		////Debug.Log (tempHeadingOne); //this value of m_vVelocity is still 0,0,0 ...?

		if (tempScript.getDolphin ())
			tempVelocity = tempVelocity;

		if (tempVelocity.magnitude > MaxSpeed) {
			Vector3 tempV = m_vVelocity.normalized;
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
			m_Vehicles[i].transform.rotation = Quaternion.Slerp (m_Vehicles[i].transform.rotation, r, 0.5f);
		}

		/*
		if (m_Vehicles [i].transform.position.x < 0 || m_Vehicles [i].transform.position.x > 1000 ||
		    m_Vehicles [i].transform.position.y < 0 || m_Vehicles [i].transform.position.y > 1000 ||
		    m_Vehicles [i].transform.position.z < 0 || m_Vehicles [i].transform.position.z > 1000) {
			
			Vector3 randomLocation = new Vector3(Random.Range (30.0f, 900.0f), Random.Range (30.0f, 900.0f),Random.Range (30.0f, 900.0f));
			m_Vehicles[i].transform.position = randomLocation;
			
			tempScript.setVelocity (Vector3.zero);
			
		}*/
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

	public GameObject getHotSphere() {
		return hotLightPosition;
	}


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

	public Vector3 Separation(List<GameObject> neighbors, GameObject vehicle) {
		Vector3 SteeringForce = Vector3.zero;

		for (int a = 0; a < neighbors.Count; ++a) {
			bool areEqual = System.Object.ReferenceEquals (vehicle,neighbors[a]);
			if (!areEqual && neighbors[a].gameObject.GetComponent<BallBounce>().getTag ()) {
				Vector3 ToAgent = vehicle.transform.position - neighbors[a].transform.position;
				SteeringForce += ToAgent.normalized / ToAgent.magnitude;
			}
		}
		return SteeringForce;
	}

	public Vector3 Alignment(List<GameObject> neighbors, GameObject vehicle) {
		Vector3 AverageHeading = Vector3.zero;

		int NeighborCount = 0;

		for (int a = 0; a < neighbors.Count; ++a) {
			bool areEqual = System.Object.ReferenceEquals (vehicle, neighbors [a]);
			BallBounce temp = neighbors [a].gameObject.GetComponent<BallBounce> ();
			if (!areEqual && temp.getTag ()) {
				AverageHeading += temp.getHeading ();

				++NeighborCount;
			}
		}
		if (NeighborCount > 0) {
			AverageHeading /= (float)NeighborCount;
			AverageHeading -= vehicle.gameObject.GetComponent<BallBounce> ().getHeading ();
		}
		return AverageHeading;
	}

	public Vector3 Cohesion(List<GameObject> neighbors, GameObject vehicle) {
		Vector3 CenterOfMass = Vector3.zero;
		Vector3 SteeringForce = Vector3.zero;

		int NeighborCount = 0;

		for (int a = 0; a < neighbors.Count; ++a) {
			bool areEqual = System.Object.ReferenceEquals (vehicle, neighbors [a]);
			if (!areEqual && neighbors[a].gameObject.GetComponent<BallBounce>().getTag())
			{
				CenterOfMass += neighbors[a].transform.position;
				++NeighborCount;
			}
		}

		if (NeighborCount > 0) {
			CenterOfMass /= (float)NeighborCount;
			SteeringForce = Seek (CenterOfMass, vehicle);
		}
		return SteeringForce.normalized;
	}

	public void AvoidPredator(int i, GameObject ent) {
		Force = Vector3.zero;
		
		GameObject closestRedFish = null;
		float closestRFFloat = 300f;
		float temp = 0f;
		bool runAway = false;
		
		if (Vector3.Distance (m_Vehicles [i].transform.position, dolfinUsing.transform.position) < 300f)
			Force = Evade (dolfinUsing, m_Vehicles [i]) * 1.5f;
		
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

	public Vector3 wander2(int i) {
		float circleDistance = 100f;

		Vector3 circleCenter = Vector3.zero;
		circleCenter = m_Vehicles[i].GetComponent<BallBounce> ().getVelocity ();
		circleCenter = circleCenter.normalized;
		circleCenter = circleCenter * circleDistance;

		Vector3 displacement = Vector3.zero;
		displacement = new Vector3 (0f, -1f, 0f);
		displacement = displacement * circleDistance;

		float number = displacement.magnitude;
		displacement.x = Mathf.Cos (wanderAngle) * number;
		displacement.y = Mathf.Sin (wanderAngle) * number;
		displacement.z = Mathf.Cos (wanderAngle) * number;

		wanderAngle += Random.Range (0, 1) * 10f - 10f * 0.5f;

		Vector3 WanderForce = Vector3.zero;

		WanderForce = circleCenter + displacement;

		return Seek (WanderForce, m_Vehicles[i]);
	}

	public Vector3 wander3(int i) {

		float JitterThisTimeSlice = m_dWanderJitter * Time.fixedDeltaTime;

		m_vWanderTarget += new Vector3 (Random.value * JitterThisTimeSlice, Random.value * JitterThisTimeSlice, Random.value * JitterThisTimeSlice);

		m_vWanderTarget = m_vWanderTarget.normalized;

		m_vWanderTarget *= m_dWanderRadius;

		Vector3 target = m_vWanderTarget + new Vector3 (0f, 0f, m_dWanderDistance / Mathf.Cos (45));
		Vector3 heading = m_Vehicles [i].gameObject.GetComponent<BallBounce> ().getHeading ();

		Vector3 relative = transform.InverseTransformDirection (Vector3.right);

		Vector3 normal = relative - Vector3.Project (relative, heading);

		normal = normal.normalized * heading.magnitude;

		float[,] matrix = new float[3,3];
		float[,] identity = new float[3, 3];
		float[,] temp = new float[3, 3];

		for (int k = 0; k < 3; k++) {
			for (int j = 0; j < 3; j++) {
				matrix[k,j] = 0.0f;
			}
		}

		matrix [0,0] = heading.x; matrix [0,1] = heading.y; matrix [0,2] = heading.z;
		matrix [1,0] = normal.x; matrix [1,1] = normal.y; matrix [1,2] = normal.z;
		matrix [2,0] = 0f; matrix [2,1] = 0; matrix [2,2] = 1;

		identity [0,0] = 1f; identity [0,1] = 0f; identity [0,2] = 0f;
		identity [1,0] = 0f; identity [1,1] = 1f; identity [1,1] = 0f;
		identity [2,0] = 0f; identity [2,1] = 0f; identity [2,2] = 1f;

		temp[0,0] = (identity[0,0] * matrix[0,0]) + (identity[0,1] * matrix[1,0]) + (identity[0,2]* matrix[2,0]);
		temp[0,1] = (identity[0,0] * matrix[0,1]) + (identity[0,1] * matrix[1,1]) + (identity[0,2] * matrix[2,1]);
		temp[0,2] = (identity[0,0] * matrix[0,2]) + (identity[0,1] * matrix[1,2]) + (identity[0,2] * matrix[2,2]);
		
		//second
		temp[1,0] = (identity[1,0] * matrix[0,0]) + (identity[1,1] * matrix[1,0]) + (identity[1,2] * matrix[2,0]);
		temp[1,1] = (identity[1,0] * matrix[0,1]) + (identity[1,1] * matrix[1,1]) + (identity[1,2] * matrix[2,1]);
		temp[1,2] = (identity[1,0] * matrix[0,2]) + (identity[1,1] * matrix[1,2]) + (identity[1,2] * matrix[2,2]);
		
		//third
		temp[2,0] = (identity[2,0] * matrix[0,0]) + (identity[2,1] * matrix[1,0]) + (identity[2,2] * matrix[2,0]);
		temp[2,1] = (identity[2,0] * matrix[0,1]) + (identity[2,1] * matrix[1,1]) + (identity[2,2] * matrix[2,1]);
		temp[2,2] = (identity[2,0] * matrix[0,2]) + (identity[2,1] * matrix[1,2]) + (identity[2,2] * matrix[2,2]);

		identity = temp;

		matrix [0, 0] = 1f; matrix [0, 1] = 0f; matrix [0, 2] = 0f;
		matrix [1, 0] = 0f; matrix [1, 1] = 1f; matrix [1, 2] = 0f;
		matrix [2, 0] = m_Vehicles [i].transform.position.x; matrix [2, 1] = m_Vehicles [i].transform.position.y; matrix [2, 2] = m_Vehicles [i].transform.position.z;


		temp[0,0] = (identity[0,0] * matrix[0,0]) + (identity[0,1] * matrix[1,0]) + (identity[0,2]* matrix[2,0]);
		temp[0,1] = (identity[0,0] * matrix[0,1]) + (identity[0,1] * matrix[1,1]) + (identity[0,2] * matrix[2,1]);
		temp[0,2] = (identity[0,0] * matrix[0,2]) + (identity[0,1] * matrix[1,2]) + (identity[0,2] * matrix[2,2]);
		
		//second
		temp[1,0] = (identity[1,0] * matrix[0,0]) + (identity[1,1] * matrix[1,0]) + (identity[1,2] * matrix[2,0]);
		temp[1,1] = (identity[1,0] * matrix[0,1]) + (identity[1,1] * matrix[1,1]) + (identity[1,2] * matrix[2,1]);
		temp[1,2] = (identity[1,0] * matrix[0,2]) + (identity[1,1] * matrix[1,2]) + (identity[1,2] * matrix[2,2]);
		
		//third
		temp[2,0] = (identity[2,0] * matrix[0,0]) + (identity[2,1] * matrix[1,0]) + (identity[2,2] * matrix[2,0]);
		temp[2,1] = (identity[2,0] * matrix[0,1]) + (identity[2,1] * matrix[1,1]) + (identity[2,2] * matrix[2,1]);
		temp[2,2] = (identity[2,0] * matrix[0,2]) + (identity[2,1] * matrix[1,2]) + (identity[2,2] * matrix[2,2]);

		identity = temp;

		float tempX = (identity [0, 0] * target.x) + (identity [1, 0] * target.y) + (identity [2,0]);
		float tempY = (identity [0, 1] * target.x) + (identity [1,1] * target.y) + (identity [2,1]);
		float tempZ = /*(identity [0, 2] * target.x) + (identity [1, 2] * target.y)*/target.z + (identity [2, 2]);

		target.x = tempX;
		target.y = tempY;
		target.z = tempZ;

		return target - m_Vehicles[i].transform.position;

	}


}
