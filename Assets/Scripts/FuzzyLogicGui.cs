using UnityEngine;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class FuzzyLogicGui : MonoBehaviour {
	
	private bool isHidden;
	private bool showFL;

	private GameObject hotLight;

	private double aNum;
	private int counter;
	
	private Rect windowRect;
	private List<GameObject> m_Vehicles = new List<GameObject> ();
	private List<GameObject> m_WanderList = new List<GameObject> ();
	private List<GameObject> m_PlantList = new List<GameObject> ();
	private Dictionary<string, double> domDictionary = new Dictionary<string, double> ();
	private Dictionary<string,string> consequentDictionary = new Dictionary<string,string> ();
	private double[] tempDouble;
	private string fileName;
	private string[] variables = new string[5];
	private double[] varNumbers = new double[4];
	private string distToPrey, distToMate, hunger, libido;
	
	private bool createDictionary;
	private bool goThru;
	
	private double globalDistance;
	private double mateDistance;
	
	public FuzzyModule fModule;
	private Eat cp;
	private string text = "";
	
	private bool isPaused;
	private string eatValue;
	private string libidoValue;
	
	private string textFieldString;
	
	public BallBounce tempScript;
	//public Dictionary<string, double> tempDictionary = new Dictionary<string, double>();
	private string[] tempNum = new string[25];
	private double[] aNumber = new double[25];
	
	private string[] names = new string[15];
	private string[] outcome = new string[2];
	private int key;
	private double dist;
	private string tempString1, tempString2;
	public CellSpacePartition cell;
	
	public bool runDump;
	
	// Use this for initialization
	void Start () {
		
		counter = 1;
		isHidden = false;
		windowRect = new Rect (20, 20, Screen.width - 100, Screen.height - 100);
		isPaused = false;
		textFieldString = "34";
		goThru = false;
		
		runDump = true;
		
		cp = GameObject.Find ("Container").gameObject.GetComponent<Eat> ();
		
		cell = GameObject.Find ("Container").gameObject.GetComponent<CellSpacePartition> ();
		
		domDictionary = GameObject.Find ("Container").gameObject.GetComponent<CellSpacePartition> ().getDictionary ();
		
		showFL = false;
		
		//fModule = cp.getFuzzyModule ();
		
		createDictionary = false;
		
		tempDouble = new double[2];
		tempDouble = cell.getTempDouble ();
		
		//m_Vehicles = GameObject.Find ("Container").gameObject.GetComponent<CellSpacePartition> ().getVehicles ();
		
		//m_WanderList = GameObject.Find ("Container").gameObject.GetComponent<CellSpacePartition> ().getWanderList ();
		
		//m_PlantList = GameObject.Find ("Container").gameObject.GetComponent<CellSpacePartition> ().getPlantList ();
		
		for (int j = 0; j < 4; j++) {
			variables[j] = "";
		}
		
		for (int d = 0; d < 25; d++) {
			tempNum[d] = "";
		}
		
		for (int d = 0; d < 25; d++) {
			aNumber[d] = 0.0d;
		}
		
		consequentDictionary.Add ("eTarget_Close, VeryHungry, HighLibido", "VeryDesirable");
		consequentDictionary.Add ("eTarget_Close, VeryHungry, MediumLibido", "VeryDesirable");
		consequentDictionary.Add ("eTarget_Close, VeryHungry, NoLibido", "VeryDesirable");
		
		consequentDictionary.Add ("eTarget_Close, Hungry, HighLibido", "Desirable");
		consequentDictionary.Add ("eTarget_Close, Hungry, MediumLibido", "Desirable");
		consequentDictionary.Add ("eTarget_Close, Hungry, NoLibido", "VeryDesirable");
		
		consequentDictionary.Add ("eTarget_Close, NotHungry, HighLibido", "Undesirable");
		consequentDictionary.Add ("eTarget_Close, NotHungry, MediumLibido", "Undesirable");
		consequentDictionary.Add ("eTarget_Close, NotHungry, NoLibido", "Undesirable");
		
		consequentDictionary.Add ("eTarget_Medium, VeryHungry, HighLibido", "Desirable");
		consequentDictionary.Add ("eTarget_Medium, VeryHungry, MediumLibido", "VeryDesirable");
		consequentDictionary.Add ("eTarget_Medium, VeryHungry, NoLibido", "VeryDesirable");
		
		consequentDictionary.Add ("eTarget_Medium, Hungry, HighLibido", "Desirable");
		consequentDictionary.Add ("eTarget_Medium, Hungry, MediumLibidoy", "Desirable");
		consequentDictionary.Add ("eTarget_Medium, Hungry, NoLibido", "VeryDesirable");
		
		consequentDictionary.Add ("eTarget_Medium, NotHungry, HighLibido", "Undesirable");
		consequentDictionary.Add ("eTarget_Medium, NotHungry, MediumLibido", "Undesirable");
		consequentDictionary.Add ("eTarget_Medium, NotHungry, NoLibido", "Undesirable");
		
		consequentDictionary.Add ("eTarget_Far, VeryHungry, HighLibido", "VeryDesirable");
		consequentDictionary.Add ("eTarget_Far, VeryHungry, MediumLibido", "VeryDesirable");
		consequentDictionary.Add ("eTarget_Far, VeryHungry, NoLibido", "VeryDesirable");
		
		consequentDictionary.Add ("eTarget_Far, Hungry, HighLibido", "Desirable");
		consequentDictionary.Add ("eTarget_Far, Hungry, MediumLibido", "Desirable");
		consequentDictionary.Add ("eTarget_Far, Hungry, NoLibido", "VeryDesirable");
		
		consequentDictionary.Add ("eTarget_Far, NotHungry, HighLibido","Undesirable");
		consequentDictionary.Add ("eTarget_Far, NotHungry, MediumLibido", "Undesirable");
		consequentDictionary.Add ("eTarget_Far, NotHungry, NoLibido", "Undesirable");
		
		//sex
		consequentDictionary.Add ("mTarget_Close, VeryHungry, HighLibido", "MediumSex");
		consequentDictionary.Add ("mTarget_Close, VeryHungry, MediumLibido", "MediumSex");
		consequentDictionary.Add ("mTarget_Close, VeryHungry, NoLibido", "NoSex");
		
		consequentDictionary.Add ("mTarget_Close, Hungry, HighLibido", "HighSex");
		consequentDictionary.Add ("mTarget_Close, Hungry, MediumLibido", "MediumSex");
		consequentDictionary.Add ("mTarget_Close, Hungry, NoLibido", "NoSex");
		
		consequentDictionary.Add ("mTarget_Close, NotHungry, HighLibido", "HighSex");
		consequentDictionary.Add ("mTarget_Close, NotHungry, MediumLibido", "HighSex");
		consequentDictionary.Add ("mTarget_Close, NotHungry, NoLibido", "NoSex");
		
		
		consequentDictionary.Add ("mTarget_Medium, VeryHungry, HighLibido", "HighSex"); //***this one affects program alot!!!
		consequentDictionary.Add ("mTarget_Medium, VeryHungry, MediumLibido", "NoSex");
		consequentDictionary.Add ("mTarget_Medium, VeryHungry, NoLibido", "NoSex");
		
		consequentDictionary.Add ("mTarget_Medium, Hungry, HighLibido", "HighSex");
		consequentDictionary.Add ("mTarget_Medium, Hungry, MediumLibido", "MediumSex");
		consequentDictionary.Add ("mTarget_Medium, Hungry, NoLibido", "NoSex");
		
		consequentDictionary.Add ("mTarget_Medium, NotHungry, HighLibido", "HighSex");
		consequentDictionary.Add ("mTarget_Medium, NotHungry, MediumLibido", "MediumSex");
		consequentDictionary.Add ("mTarget_Medium, NotHungry, NoLibido", "NoSex");
		consequentDictionary.Add ("mTarget_Far, VeryHungry, HighLibido", "MediumSex");
		consequentDictionary.Add ("mTarget_Far, VeryHungry, MediumLibido", "NoSex");
		consequentDictionary.Add ("mTarget_Far, VeryHungry, NoLibido", "NoSex");
		
		consequentDictionary.Add ("mTarget_Far, Hungry, HighLibido", "MediumSex");
		consequentDictionary.Add ("mTarget_Far, Hungry, MediumLibido", "MediumSex");
		consequentDictionary.Add ("mTarget_Far, Hungry, NoLibido", "NoSex");
		
		consequentDictionary.Add ("mTarget_Far, NotHungry, HighLibido", "HighSex");
		consequentDictionary.Add ("mTarget_Far, NotHungry, MediumLibido", "MediumSex");
		consequentDictionary.Add ("mTarget_Far, NotHungry, NoLibido", "NoSex");
		
		
		fileName = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
		fileName += "/Myfile.txt";
		
		File.WriteAllText (fileName, "Starting Data Dump!!!\n");

		m_Vehicles = GameObject.Find ("Container").gameObject.GetComponent<CellSpacePartition> ().getVehicles ();

		m_WanderList = GameObject.Find ("Container").gameObject.GetComponent<CellSpacePartition> ().getWanderList ();

	}
	
	
	
	// Update is called once per frame
	void Update () {
		/*
        if (runDump) {
            StartCoroutine (dumpValues ());
        }
*/
	}
	
	
	public bool getIsPaused() {
		return isPaused;
	}
	
	public void enterNumber(int i) {
		
		tempDouble = cell.getTempDouble ();

		
		tempScript = m_Vehicles [i].GetComponent<BallBounce> ();
		
		/*
        m_WanderList = GameObject.Find ("Container").gameObject.GetComponent<CellSpacePartition> ().getWanderList ();
        
        m_PlantList = GameObject.Find ("Container").gameObject.GetComponent<CellSpacePartition> ().getPlantList ();

        runDump = false;
*/
		/*
            //bool anothFish = false;
            float closestFish = 1000.0f;
        //m_Vehicles = GameObject.Find ("Container").gameObject.GetComponent<CellSpacePartition> ().getVehicles ();
            tempScript = m_Vehicles [i].GetComponent<BallBounce> ();
            
            double tempHunger = (double)tempScript.getHunger ();
            double tempLibido = (double)tempScript.getLibido ();
            

            int state = tempScript.getState ();
            float closestGoldFishFloat = 1000.0f;
            

            float closestFishFloat = 1000.0f;
            float closestDolphinFloat = 1000.0f;
            
            
            if (tempScript.getAmberjack ()) {
                ////Debug.Log ("Entered Loop!!!"); Working!!!
                for (int j = 0; j < m_WanderList.Count; j++) {
                    //    if (m_WanderList[j] != null) {
                BallBounce tempState = m_WanderList [i].GetComponent<BallBounce> ();
                    
                    if (tempState.getKoi ()) {
                        //bool areEqual = System.Object.ReferenceEquals (m_Vehicles [i], m_WanderList [j]);
                        //if (!areEqual) {
                    float temp = Vector3.Distance (m_Vehicles [i].transform.position, m_WanderList [j].transform.position);
                        if (temp < closestFishFloat) {
                            closestFishFloat = temp;
                            
                        }

                    } //here
                    
                    
                    if (tempState.getAmberjack ()) {
                    bool areEqual = System.Object.ReferenceEquals (m_Vehicles [i], m_WanderList [j]);
                        if (!areEqual) {
                        float temp = Vector3.Distance (m_Vehicles [i].transform.position, m_WanderList [j].transform.position);
                            if (temp < closestDolphinFloat) {
                                closestDolphinFloat = temp;
                                
                            }
                        }
                        
                    } //here
                    
                }
                
                
                //GoldFishPreyGO.gameObject.GetComponent<BallBounce> ().setAnotherFish (true);//you don"t need this because other fish"s state would be eat or mate, not wander. 
                
                ////Debug.Log ("Closest Koi Prey: " + closestFishFloat);
                ////Debug.Log ("Closest Dolphin Mate: " + closestDolphinFloat);
            if(!cell.getFuzzifyInUse()) {
                goThru = true;

                cell.setFuzzifyInUse(true);
                domDictionary.Clear();
                createDictionary = true;
                globalDistance = closestFishFloat;
                mateDistance = closestDolphinFloat;
                tempDouble = cp.GetDesirability ((double)closestFishFloat, (double)closestDolphinFloat, tempHunger, tempLibido);
                cell.setFuzzifyInUse(false);
                createDictionary = false;

            }
            else {
                goThru = false;
            }
            ////Debug.Log ("Eat" + tempDouble [0]);
                ////Debug.Log ("Mate" + tempDouble [1]);
                
            } //end state 4 and isDolphin == true
            //bool anothFish = false;

        //dump values to file...

        if (goThru) {
        BallBounce tempBallBounce = m_Vehicles [i].GetComponent<BallBounce> ();
*/
		
		//File.AppendAllText(fileName, "Time Intervale: " + counter.ToString() + "\n\n");
		
		File.AppendAllText(fileName, "\n\n\nFish's Number: "  + tempScript.getFishNumber().ToString() + "\n");
		
		//GUI.Label (new Rect(40, 510, 300, 20), "Fish's Hunger Level: ");
		File.AppendAllText(fileName, "Fish's Hunger Level: "  + tempScript.getHunger().ToString() + "\n");
		//GUI.Label (new Rect(40, 530, 20, 20), eatValue);
		
		//libidoValue = variables[3];
		
		//GUI.Label (new Rect(40, 550, 300, 20), "Fish's Libido Level: ");
		File.AppendAllText(fileName, "Fish's Libido Level: " + tempScript.getLibido().ToString() + "\n");
		//GUI.Label (new Rect(40, 570, 20,20), libidoValue);
		
		//File.AppendAllText(fileName, "Fish's Distance from Prey: " + globalDistance.ToString() + "\n");
		
		//File.AppendAllText(fileName, "Fish's Distance from Mate: " + mateDistance.ToString() + "\n\n");
		
		File.AppendAllText(fileName, "Fish's State: " + tempScript.getState ().ToString() + "\n\n");
		
		
		///////
		/// 
		
		//GUI.Label (new Rect(40, 170, 300, 20), "Antecedents");
		File.AppendAllText(fileName, "\n\nAntecedents\n");
		//GUI.Label (new Rect (40, 190, 300, 20), "FLV DistToTarget");
		File.AppendAllText(fileName, "FLV DistToTarget\n");
		names[0] = "eTarget_Close";
		
		domDictionary.TryGetValue("eTarget_Close", out aNumber[0]);
		////Debug.Log ("this is Fuzzy Set #1: " + aNumber[0]);
		tempNum[0] = aNumber[0].ToString();
		//Debug.Log ("eTargetClose" + aNumber[0]);
		//GUI.Label(new Rect(40, 210, 300, 20), "eTarget_Close: " + tempNum[0]);
		File.AppendAllText(fileName, "eTarget_Close: " + tempNum[0] + "\n");
		
		domDictionary.TryGetValue("eTarget_Medium", out aNumber[1]);
		
		tempNum[1] = aNumber[1].ToString();
		//Debug.Log ("eTargetMedium" + aNumber[1]);
		names[1] = "eTarget_Medium";
		
		//GUI.Label(new Rect(40, 230, 300, 20), "eTarget_Medium: " + tempNum[1]);
		File.AppendAllText(fileName, "eTarget_Medium: " + tempNum[1] + "\n");
		
		domDictionary.TryGetValue("eTarget_Far", out aNumber[2]);
		
		tempNum[2] = aNumber[2].ToString();
		////Debug.Log ("this is Fuzzy Set #3: " + aNumber[2]);
		names[2] = "eTarget_Far";
		//Debug.Log ("eTargetFar" + aNumber[2]);
		//GUI.Label(new Rect(40, 250, 120, 20), "eTarget_Far: " + tempNum[2]);
		File.AppendAllText(fileName, "eTarget_Far: " + tempNum[2] + "\n\n");
		////
		
		//GUI.Label(new Rect(40, 270, 300,20), "FLV DistToMate:");
		File.AppendAllText(fileName, "FLV DistToMate: " + "\n");
		
		domDictionary.TryGetValue("mTarget_Close", out aNumber[3]);
		
		tempNum[3] = aNumber[3].ToString();
		
		names[3] = "mTarget_Close";
		
		//GUI.Label(new Rect(40, 290, 300, 20), "mTarget_Close: " + tempNum[3]);
		File.AppendAllText(fileName, "mTarget_Close: " + tempNum[3] + "\n");
		
		domDictionary.TryGetValue("mTarget_Medium", out aNumber[4]);
		
		tempNum[4] = aNumber[4].ToString();
		
		names[4] = "mTarget_Medium";
		
		//GUI.Label(new Rect(40, 310, 300, 20), "mTarget_Medium: " + tempNum[4]);
		File.AppendAllText(fileName, "mTarget_Medium: " + tempNum[4] + "\n");
		
		domDictionary.TryGetValue("mTarget_Far", out aNumber[5]);
		
		tempNum[5] = aNumber[5].ToString();
		
		names[5] = "mTarget_Far";
		
		//GUI.Label(new Rect(40, 330, 300, 20), "mTarget_Far: " + tempNum[5]);
		File.AppendAllText(fileName, "mTarget_Far: " + tempNum[5] + "\n\n");
		
		///////
		//GUI.Label(new Rect(40, 350, 120,20), "FLV Hunger:");
		File.AppendAllText(fileName, "FLV Hunger: \n");
		domDictionary.TryGetValue("VeryHungry", out aNumber[6]);
		
		tempNum[6] = aNumber[6].ToString();
		
		names[6] = "VeryHungry";
		
		//GUI.Label(new Rect(40, 370, 300, 20), "Very Hungry: " + tempNum[6]);
		File.AppendAllText(fileName, "Very Hungry: " + tempNum[6] + "\n");
		
		domDictionary.TryGetValue("Hungry", out aNumber[7]);
		
		tempNum[7] = aNumber[7].ToString();
		
		names[7] = "Hungry";
		
		//GUI.Label(new Rect(40, 390, 300, 20), "Hungry: " + tempNum[7]);
		File.AppendAllText(fileName, "Hungry: " + tempNum[7] + "\n");
		
		domDictionary.TryGetValue("NotHungry", out aNumber[8]);
		
		tempNum[8] = aNumber[8].ToString();
		
		names[8] = "NotHungry";
		
		//GUI.Label(new Rect(40, 410, 300, 20), "Not Hungry: " + tempNum[8]);
		File.AppendAllText(fileName, "Not Hungry: " + tempNum[8] + "\n\n");
		
		//////
		//GUI.Label(new Rect(40, 430, 300,20), "FLV Libido:");
		File.AppendAllText(fileName, "FLV Libido: " + "\n");
		domDictionary.TryGetValue("HighLibido", out aNumber[9]);
		
		tempNum[9] = aNumber[9].ToString();
		
		names[9] = "HighLibido";
		
		//GUI.Label(new Rect(40, 450, 300, 20), "High Libido: " + tempNum[9]);
		File.AppendAllText(fileName, "High Libido: " + tempNum[9] + "\n");
		
		domDictionary.TryGetValue("MediumLibido", out aNumber[10]);
		
		tempNum[10] = aNumber[10].ToString();
		
		names[10]= "MediumLibido";
		
		//GUI.Label(new Rect(40, 470, 300, 20), "Medium Libido: " + tempNum[10]);
		File.AppendAllText(fileName, "Medium Libido: " + tempNum[10] + "\n");
		
		domDictionary.TryGetValue("NoLibido", out aNumber[11]);
		
		tempNum[11] = aNumber[11].ToString();
		
		names[11] = "NoLibido";
		
		//GUI.Label(new Rect(40, 490, 300, 20), "No Libido: " + tempNum[11]);
		File.AppendAllText(fileName, "No Libido: " + tempNum[11] + "\n\n");
		/*
            eatValue = variables[2];

            //GUI.Label (new Rect(40, 510, 300, 20), "Fish's Hunger Level: ");
            File.AppendAllText(fileName, "Fish's Hunger Level: "  + eatValue + "\n");
            //GUI.Label (new Rect(40, 530, 20, 20), eatValue);
            
            libidoValue = variables[3];

            //GUI.Label (new Rect(40, 550, 300, 20), "Fish's Libido Level: ");
            File.AppendAllText(fileName, "Fish's Libido Level: " + libidoValue + "\n");
            //GUI.Label (new Rect(40, 570, 20,20), libidoValue);

            File.AppendAllText(fileName, "Fish's Distance from Prey: " + variables[0] + "\n");

            File.AppendAllText(fileName, "Fish's Distance from Mate: " + variables[1] + "\n\n");
*/
		////
		//GUI.Label(new Rect(350, 230, 300,20), "Consequents");
		File.AppendAllText(fileName, "Consequents:\n ");
		//GUI.Label(new Rect(350, 250, 300,20), "FVL Desirability to Eat");
		File.AppendAllText(fileName, "FLV Desirability to Eat: \n");
		domDictionary.TryGetValue("VeryDesirable", out aNumber[12]);
		
		tempNum[12] = aNumber[12].ToString();
		
		//GUI.Label(new Rect(350, 270, 300, 20), "Very Desirable: " + tempNum[12]);
		File.AppendAllText(fileName, "Very Desirable: " + tempNum[12] + "\n");
		
		domDictionary.TryGetValue("Desirable", out aNumber[13]);
		
		tempNum[13] = aNumber[13].ToString();
		
		//GUI.Label(new Rect(350, 290, 300, 20), "Desirable: " + tempNum[13]);
		File.AppendAllText(fileName, "Desirable: " + tempNum[13] + "\n");
		
		domDictionary.TryGetValue("Undesirable", out aNumber[14]);
		
		tempNum[14] = aNumber[14].ToString();
		
		//GUI.Label(new Rect(350, 310, 300, 20), "Undesirable: " + tempNum[14]);
		File.AppendAllText(fileName, "Undesirable: " + tempNum[14] + "\n\n");
		
		//GUI.Label(new Rect(350, 330, 300,20), "FVL Desirability to Mate");
		File.AppendAllText(fileName, "FLV Desirability to Mate: \n");
		
		domDictionary.TryGetValue("HighSex", out aNumber[15]);
		
		tempNum[15] = aNumber[15].ToString();
		
		//GUI.Label(new Rect(350, 350, 300, 20), "HighSex: " + tempNum[15]);
		File.AppendAllText(fileName, "HighSex: " + tempNum[15] + "\n");
		
		domDictionary.TryGetValue("MediumSex", out aNumber[16]);
		
		tempNum[16] = aNumber[16].ToString();
		
		//GUI.Label(new Rect(350, 370, 300, 20), "MediumSex: " + tempNum[16]);
		File.AppendAllText(fileName, "MediumSex: " + tempNum[16] + "\n");
		
		domDictionary.TryGetValue("NoSex", out aNumber[17]);
		
		tempNum[17] = aNumber[17].ToString();
		
		//GUI.Label(new Rect(350, 390, 300, 20), "NoSex: " + tempNum[17]);
		File.AppendAllText(fileName, "NoSex: " + tempNum[17] + "\n\n");
		
		//GUI.Label(new Rect(650, 310, 300, 20), "Dufuzzified Crisp Values");
		File.AppendAllText(fileName, "Defuzzified Crisp Values \n");
		//GUI.Label(new Rect(650, 330, 300, 20), "Desirability to Eat: " + tempDouble[0].ToString());
		File.AppendAllText(fileName, "Desirability to Eat: " + tempDouble[0].ToString() + "\n");
		//GUI.Label(new Rect(650, 350, 300, 20), "Desirability to Mate: " + tempDouble[1].ToString());
		File.AppendAllText(fileName, "Desirability to Mate: " + tempDouble[1].ToString() + "\n\n");
		
		dist = 0.0d;
		key = -1;
		
		for (int y = 0; y < 3; y++) {
			if (aNumber[y] > dist) {
				dist = aNumber[y];
				key = y;
			}
		}
		
		string s1 = names[key];
		
		dist = 0.0d;
		key = -1;
		
		for (int j = 3; j < 6; j++) {
			if (aNumber[j] > dist) {
				dist = aNumber[j];
				key = j;
			}
		}
		
		string s2 = names[key];
		
		dist = 0.0d;
		key = -1;
		
		for (int q = 6; q < 9; q++) { //this is working!!!
			if (aNumber[q] > dist) {
				dist = aNumber[q];
				key = q;
			}
		}
		
		string s3 = names[key];
		
		dist = 0.0d;
		key = -1;
		
		for (int f = 9; f < 12; f++) {//this is working!!!
			if (aNumber[f] > dist) {
				dist = aNumber[f];
				key = f;
			}
		}
		
		string s4 = names[key];
		
		string firstRule = s1 + ", " + s3 + ", " + s4;
		
		string secondRule = s2 + ", " + s3 + ", " + s4;
		
		firstRule.Trim();
		secondRule.Trim();
		
		
		
		consequentDictionary.TryGetValue(firstRule, out outcome[0]);
		consequentDictionary.TryGetValue(secondRule, out outcome[1]);
		
		////Debug.Log (outcome[0] + " + " + outcome[1]);
		//GUI.Label (new Rect(950, 330, 300, 20), "Fuzzy Rules");
		File.AppendAllText(fileName, "Fuzzy Rules \n");
		//GUI.Label (new Rect(950, 350, 300, 20), "Desire to Eat:");
		File.AppendAllText(fileName, "Desire to Eat: \n");
		//GUI.Label (new Rect(950, 370, 300, 20), firstRule);
		File.AppendAllText(fileName, firstRule + "\n");
		//GUI.Label (new Rect(950, 390, 300, 20), "= " + outcome[0]);
		File.AppendAllText(fileName, "= " + outcome[0] + "\n");
		//GUI.Label (new Rect(950, 410, 300, 20), "Desire to Mate:");
		File.AppendAllText(fileName, "Desire to Mate: \n");
		//GUI.Label (new Rect(950, 430, 300, 20), secondRule);
		File.AppendAllText(fileName, secondRule + "\n");
		//GUI.Label (new Rect(950, 450, 300, 20), "= " + outcome[1]);
		File.AppendAllText(fileName, "= " + outcome[1] + "\n");
		
		
		//showFL = false;
		//createDictionary = false;
		//}
		/*
        yield return new WaitForSeconds (1);
        runDump = true;
        if (goThru) {
            counter++;
        }
*/
	} // end enterNumber
	
	public bool getBoolDictionary() {
		return createDictionary;
	}
	
	public void setBoolDictionary(bool val) {
		createDictionary = val;     
	}

	IEnumerator dumpValues() {
		runDump = false;


		//bool anothFish = false;
		float closestFish = 1000.0f;

		//m_Vehicles = GameObject.Find ("Container").gameObject.GetComponent<CellSpacePartition> ().getVehicles ();
		int i = 0;
		for (; i < m_Vehicles.Count; i++) {
			if (m_Vehicles[i].gameObject.GetComponent<BallBounce>().getFishNumber() == 1)
				break;
		}

		if (m_Vehicles [i].GetComponent<BallBounce> ().getFishNumber () == 1) {
			tempScript = m_Vehicles [i].GetComponent<BallBounce> ();
		
			float tempColdness = Vector3.Distance (m_Vehicles [i].transform.position, hotLight.transform.position);

			double tempHunger = (double)tempScript.getHunger ();
			double tempLibido = (double)tempScript.getLibido ();
		
		
			int state = tempScript.getState ();
			float closestGoldFishFloat = 1000.0f;
		
		
			float closestFishFloat = 1000.0f;
			float closestDolphinFloat = 1000.0f;
		
		
			if (tempScript.getAmberjack ()) {
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
						
						}
					
					} //here
				
				
					if (tempState.getAmberjack ()) {
						bool areEqual = System.Object.ReferenceEquals (m_Vehicles [i], m_WanderList [j]);
						if (!areEqual) {
							float temp = Vector3.Distance (m_Vehicles [i].transform.position, m_WanderList [j].transform.position);
							if (temp < closestDolphinFloat) {
								closestDolphinFloat = temp;
							
							}
						}
					
					} //here
				
				}
			}

			//File.AppendAllText(fileName, "\n\n\nFish's Number: "  + tempScript.getFishNumber().ToString() + "\n");
		
			//GUI.Label (new Rect(40, 510, 300, 20), "Fish's Hunger Level: ");
			File.AppendAllText (fileName, "hunger:" + tempScript.getHunger ().ToString () + "\n");
			//GUI.Label (new Rect(40, 530, 20, 20), eatValue);
		
			//libidoValue = variables[3];
		
			//GUI.Label (new Rect(40, 550, 300, 20), "Fish's Libido Level: ");
			File.AppendAllText (fileName, "libido:" + tempScript.getLibido ().ToString () + "\n");
			//GUI.Label (new Rect(40, 570, 20,20), libidoValue);
		
			File.AppendAllText (fileName, "distPrey:" + closestFishFloat.ToString () + "\n");
		
			File.AppendAllText (fileName, "distMate:" + closestDolphinFloat.ToString () + "\n");
		
			File.AppendAllText (fileName, "state:" + tempScript.getState ().ToString () + "\n");
			
			File.AppendAllText (fileName, "timer:" + tempScript.getTimer ().ToString () + "\n");

			File.AppendAllText (fileName, "coldness:" + tempColdness.ToString () + "\n");



			yield return new WaitForSeconds (1);
			runDump = true;

		}
	}
	
	
}
