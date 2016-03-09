using UnityEngine;
using System.Collections;

public class BallBounce : MonoBehaviour {
	public bool hitTop = false;
	public bool hitBottom = false;
	public bool wall1 = false;
	public bool wall2 = false;
	public bool wallFront = false;
	public bool wallBack = false;
	public string name;
	public Vector3 m_vHeading;
	public Vector3 m_vVelocity;
	public Vector3 Target;
	public float wanderRadius;
	private int State;
	private float hunger, libido, timer;
	private GameObject mate;
	private bool anotherFish;
	private float startTime;
	private float timeCounter;
	private float libidoCounter;
	private float hungerCounter;
	private bool incrementCounters;
	private bool isMating;
	private bool stopMating;
	private bool loop;
	private GameObject prey;
	private GameObject predator;
	
	private bool isKoi;
	private bool isGoldfish;
	private bool isDolphin;
	private bool isAmberjack;
	
	private bool isPrey;
	private bool isMate;
	private int fishNumber;
	private bool m_bTag;

	// Use this for initialization
	void Awake () {
		m_vHeading = new Vector3 (0.0f, 0.0f, 0.0f);//new Vector3 (Mathf.Sin (Random.Range (-1.0f, 1.0f) * Mathf.PI), -Mathf.Cos(Random.Range (-1.0f, 1.0f) * Mathf.PI), 0.0f);
		m_vVelocity = new Vector3 (Mathf.Sin (Random.value * 2 * Mathf.PI), -Mathf.Cos(Random.value * 2 * Mathf.PI), Mathf.Cos (Random.value * 2 * Mathf.PI));
		wanderRadius = 1.2f;
		Target = new Vector3 (wanderRadius * Mathf.Cos (Random.value * Mathf.PI * 2), wanderRadius * Mathf.Sin (Random.value * Mathf.PI * 2), -wanderRadius * Mathf.Cos (Random.value * Mathf.PI * 2));
		State = 0;
		hunger = Random.Range (0.0f,100.0f);
		libido = Random.Range (0.0f, 100.0f);
		timer = Random.Range (0.0f, 60.0f);
		anotherFish = false;
		//StartCoroutine (counter1 ());
		incrementCounters = true;
		isMating = false;
		loop = true;
		isPrey = false;
		isMate = false;
		//fishNumber = -1;
		m_bTag = false;
		//isKoi = false;
		//isGoldfish = false;
		//isDolphin = false;
		
		//startTime = Time.time;
		
		//timeCounter = Time.time + timer;
		//libidoCounter = Time.time + libido;
		//hungerCounter = Time.time + hunger;
	}

	public void unTag() {
		m_bTag = false;
	}

	public void Tag() {
		m_bTag = true;
	}

	public void setTag(bool val) {
		m_bTag = val;
	}

	public bool getTag() {
		return m_bTag;
	}

	public void setDolphin() {
		this.isDolphin= true;
	}
	
	public bool getDolphin(){
		return this.isDolphin;
	}
	
	public void setAmberjack() {
		this.isAmberjack= true;
	}
	
	public bool getAmberjack(){
		return this.isAmberjack;
	}
	
	public void setGoldfish() {
		this.isGoldfish = true;
	}
	
	public bool getGoldfish(){
		return this.isGoldfish;
	}
	
	public void setKoi() {
		this.isKoi = true;
	}
	
	public bool getKoi(){
		return this.isKoi;
	}
	
	public GameObject getPrey() {
		return prey;
	}
	
	public void setPrey(GameObject ent) {
		this.prey = ent;
	}
	
	public GameObject getPredator(){
		return predator;
	}
	
	public void setPredator(GameObject ent) {
		this.predator = ent;
	}
	
	
	public bool getStopMating() {
		return isMating;
	}    
	
	public void setStopMating(bool stopMating) {
		this.stopMating = stopMating;
	}
	
	public bool getIsMating() {
		return isMating;
	}    
	
	public void setIsMating(bool isMating) {
		this.isMating = isMating;
	}
	
	public bool getAnotherFish() {
		return anotherFish;
	}
	
	public void setAnotherFish(bool another) {
		anotherFish = another;
	}
	
	public bool getAnotherPrey() {
		return isPrey;
	}
	
	public void setAnotherPrey(bool another) {
		this.isPrey = another;
	}
	
	public bool getAnotherMate() {
		return isMate;
	}
	
	public void setAnotherMate(bool another) {
		this.isMate = another;
	}
	
	public GameObject getMate() {
		return mate;
	}
	
	public void setMate(GameObject ent) {
		mate = ent;
	}
	
	public float getTimer()
	{
		return timer;
	}
	
	public void setTimer(float timer)
	{
		this.timer = timer;
	}
	
	public float getLibido()
	{
		return libido;
	}
	
	public void setLibido(float libido)
	{
		this.libido = libido;
	}
	
	public float getHunger()
	{
		return hunger;
	}
	
	public void setHunger(float hunger)
	{
		this.hunger = hunger;
	}
	
	public int getState()
	{
		return State;
	}
	
	public void setState(int state)
	{
		this.State = state;
	}
	
	public Vector3 getVelocity()
	{
		return m_vVelocity;
	}
	
	public void setVelocity(Vector3 velocity)
	{
		m_vVelocity = velocity;
	}
	
	public Vector3 getHeading()
	{
		return m_vHeading;
	}
	
	public void setHeading(Vector3 heading)
	{
		m_vHeading = heading;
	}
	
	public Vector3 getTarget()
	{
		return Target;
	}
	
	public void setTarget(Vector3 target)
	{
		this.Target = target;
	}
	
	public void setFishNumber(int number) {
		this.fishNumber = number;
	}
	
	public int getFishNumber() {
		return fishNumber;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (isMating && loop) {
			StartCoroutine(matingTime());
		}
		
		if (incrementCounters) {
			StartCoroutine(counter1 ());
		}
	}
	
	IEnumerator matingTime() {
		loop = false;
		yield return new WaitForSeconds (10);
		isMating = false;
		loop = true;
	}
	
	
	IEnumerator counter1() {
		incrementCounters = false;
		timer++;
		hunger++;
		libido++;
		if (hunger > 100.0f)
			hunger = 100.0f;
		
		if (libido > 100.0f)
			libido = 100.0f;
		
		//    Debug.Log (timer);
		yield return new WaitForSeconds (1);
		incrementCounters = true;
		
	}
	
}
