using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Weapons : MonoBehaviour {

	//Animator komponenta
	Animator anim;
	public Camera gunCamera;
	//Opcije oružja
	[Header("Gun Camera Options")]
	public float fovSpeed = 15.0f;
	public float defaultFov = 40.0f;
	public float aimFov = 25.0f;
	//ime oružja
	[Header("UI Weapon Name")]
	public string weaponName;
	private string storedWeaponName;
	[Header("Weapon Sway")]
	public bool weaponSway;
	public float swayAmount = 0.02f;
	public float maxSwayAmount = 0.06f;
	public float swaySmoothValue = 4.0f;
	private Vector3 initialSwayPosition;
	private float lastFired;
	//Postavke oružja
	[Header("Weapon Settings")]
	public float fireRate;
	public bool autoReload;
	public float autoReloadDelay;
	private bool isReloading;
	private bool isRunning;
	private bool isAiming;
	private bool isWalking;
	private bool isInspecting;
	private int currentAmmo;
	public int ammo;
	private bool outOfAmmo;
	//Postavke metka
	[Header("Bullet Settings")]
	public float bulletForce = 400.0f;
	public float showBulletInMagDelay = 0.6f;
	public SkinnedMeshRenderer bulletInMagRenderer;
	//postavke granate
	[Header("Grenade Settings")]
	public float grenadeSpawnDelay = 0.35f;
	[Header("Muzzleflash Settings")]
	public bool randomMuzzleflash = false;
	private int minRandomValue = 1;
	[Range(2, 25)]
	public int maxRandomValue = 5;
	private int randomMuzzleflashValue;
	public bool enableMuzzleflash = true;
	public ParticleSystem muzzleParticles;
	public bool enableSparks = true;
	public ParticleSystem sparkParticles;
	public int minSparkEmission = 1;
	public int maxSparkEmission = 7;
	[Header("Muzzleflash Light Settings")]
	public Light muzzleflashLight;
	public float lightDuration = 0.02f;

	//Komponenta audio source
	[Header("Audio Source")]
	public AudioSource mainAudioSource;
	public AudioSource shootAudioSource;

	//UI tekst
	[Header("UI Components")]
	public Text timescaleText;
	public Text currentWeaponText;
	public Text currentAmmoText;
	public Text totalAmmoText;

	[System.Serializable]

	//Prefabi
	public class prefabs
	{  
		[Header("Prefabs")]
		public Transform bulletPrefab;
		public Transform casingPrefab;
		public Transform grenadePrefab;
	}
	public prefabs Prefabs;
	
	[System.Serializable]
	//mjesta stvaranja
	public class spawnpoints
	{  
		[Header("Spawnpoints")]
		public Transform casingSpawnPoint;
		public Transform bulletSpawnPoint;
		public Transform grenadeSpawnPoint;
	}
	public spawnpoints Spawnpoints;

	[System.Serializable]
	//zvukovi
	public class soundClips
	{
		public AudioClip shootSound;
		public AudioClip takeOutSound;
		public AudioClip holsterSound;
		public AudioClip reloadSoundOutOfAmmo;
		public AudioClip reloadSoundAmmoLeft;
		public AudioClip aimSound;
	}
	public soundClips SoundClips;

	private bool soundHasPlayed = false;

	private void Awake () {
		
		//dohvaćanje animator komponente
		anim = GetComponent<Animator>();
		//Postavljanje metaka
		currentAmmo = ammo;

		muzzleflashLight.enabled = false;
	}

	private void Start () {
		
		//Ime pružja
		storedWeaponName = weaponName;
		currentWeaponText.text = weaponName;
		totalAmmoText.text = ammo.ToString();

		//Skloni oružje
		initialSwayPosition = transform.localPosition;

		//Set the shoot sound to audio source
		shootAudioSource.clip = SoundClips.shootSound;
	}

	private void LateUpdate () {
		
		//if provjerava ako je oružje sklonjeno
		if (weaponSway == true) 
		{
			float movementX = -Input.GetAxis ("Mouse X") * swayAmount;
			float movementY = -Input.GetAxis ("Mouse Y") * swayAmount;
			//Clamp između minimalne i maksimalne vrijednosti
			movementX = Mathf.Clamp 
				(movementX, -maxSwayAmount, maxSwayAmount);
			movementY = Mathf.Clamp 
				(movementY, -maxSwayAmount, maxSwayAmount);
			//Lerp funkcija dviju točaka
			//Lerp je matematička funkcija koja vrača vrijednost između dviju točaka na linearnoj skali
			Vector3 finalSwayPosition = new Vector3 
				(movementX, movementY, 0);
			transform.localPosition = Vector3.Lerp 
				(transform.localPosition, finalSwayPosition + 
					initialSwayPosition, Time.deltaTime * swaySmoothValue);
		}
	}
	
	private void Update () {

		//Pritiskom na desni klik, približi se oružje kameri radi lakšeg ciljanja
		if(Input.GetButton("Fire2") && !isReloading && !isRunning && !isInspecting) 
		{
			
			isAiming = true;
			//Ciljanje
			anim.SetBool ("Aim", true);

			//pritisak na desni klik
			gunCamera.fieldOfView = Mathf.Lerp(gunCamera.fieldOfView,
				aimFov,fovSpeed * Time.deltaTime);

			if (!soundHasPlayed) 
			{
				mainAudioSource.clip = SoundClips.aimSound;
				mainAudioSource.Play ();
	
				soundHasPlayed = true;
			}
		} 
		else 
		{
			//Kad pustimo desni klik
			gunCamera.fieldOfView = Mathf.Lerp(gunCamera.fieldOfView,
				defaultFov,fovSpeed * Time.deltaTime);

			isAiming = false;
			//Zaustavljanje ciljanja
			anim.SetBool ("Aim", false);
				
			soundHasPlayed = false;
		}

		//Ako je nasumični muzzleflash istinit,  generira nasumične integer vrijednosti
		if (randomMuzzleflash == true) 
		{
			randomMuzzleflashValue = Random.Range (minRandomValue, maxRandomValue);
		}

		currentAmmoText.text = currentAmmo.ToString ();

		//provjera animacije

		AnimationCheck ();

		
		//Pokretanje animacije napada nožem pritiskom na tipku f
		if (Input.GetKeyDown (KeyCode.F) && !isInspecting) 
		{
			anim.Play ("Knife Attack 2", 0, 0f);
		}
			
		//Pokretanje animacije bacanja granate pritiskom na tipgu g
		if (Input.GetKeyDown (KeyCode.G) && !isInspecting) 
		{
			StartCoroutine (GrenadeSpawnDelay ());
		
			anim.Play("GrenadeThrow", 0, 0.0f);
		}

		//Provjera ako je oružje prazno
		if (currentAmmo == 0) 
		{
			currentWeaponText.text = "OUT OF AMMO";
			outOfAmmo = true;
			if (autoReload == true && !isReloading) 
			{
				StartCoroutine (AutoReload ());
			}
		} 
		//ako ima municije
		else 
		{
			currentWeaponText.text = storedWeaponName.ToString ();
			outOfAmmo = false;
		}
			
		//Automatsko pucanje
		if (Input.GetMouseButton (0) && !outOfAmmo && !isReloading && !isInspecting && !isRunning) 
		{
			if (Time.time - lastFired > 1 / fireRate) 
			{
				lastFired = Time.time;

				//kod pucnja oduzimamo metak		
				currentAmmo -= 1;
				//pokretanje zvuka
				shootAudioSource.clip = SoundClips.shootSound;
				shootAudioSource.Play ();

				if (!isAiming) //ako nije približeno
				{
					anim.Play ("Fire", 0, 0f);
					if (!randomMuzzleflash && 
						enableMuzzleflash == true) 
					{
						muzzleParticles.Emit (1);
						StartCoroutine(MuzzleFlashLight());
					} 
					else if (randomMuzzleflash == true)
					{
						//emitirati ako je varijabla muzzleflasha na 1
						if (randomMuzzleflashValue == 1) 
						{
							if (enableSparks == true) 
							{
								//Emitiranje nasumičnu količinu čestica
								sparkParticles.Emit (Random.Range (minSparkEmission, maxSparkEmission));
							}
							if (enableMuzzleflash == true) 
							{
								muzzleParticles.Emit (1);
								//svijetlost prilikom pucnja
								StartCoroutine (MuzzleFlashLight ());
							}
						}
					}
				} 
				else //ako je približeno
				{
					//animacija
					anim.Play ("Aim Fire", 0, 0f);

					//ako je random muzzle neistinit
					if (!randomMuzzleflash) {
						muzzleParticles.Emit (1);
					//ako je istinit
					} 
					else if (randomMuzzleflash == true) 
					{
						//emitirati ako je varijabla muzzleflasha na 1
						if (randomMuzzleflashValue == 1) 
						{
							if (enableSparks == true) 
							{
								//Emitiranje nasumičnu količinu čestica
								sparkParticles.Emit (Random.Range (minSparkEmission, maxSparkEmission));
							}
							if (enableMuzzleflash == true) 
							{
								muzzleParticles.Emit (1);
								//svijetlost prilikom pucnja
								StartCoroutine(MuzzleFlashLight ());
							}
						}
					}
				}

				//stvaranje metka na spawn point
				var bullet = (Transform)Instantiate (
					Prefabs.bulletPrefab,
					Spawnpoints.bulletSpawnPoint.transform.position,
					Spawnpoints.bulletSpawnPoint.transform.rotation);

				//dodaj brzinu
				bullet.GetComponent<Rigidbody>().velocity = 
					bullet.transform.forward * bulletForce;
				
			
			}
		}

		//Pregled oružja pritiskom na tiplu t
		if (Input.GetKeyDown (KeyCode.T)) 
		{
			anim.SetTrigger ("Inspect");
		}

		//Pritiskom na tipku r, pokreče se punjenje puske
		if (Input.GetKeyDown (KeyCode.R) && !isReloading && !isInspecting) 
		{
			Reload ();
		}

		//WSAD kretanje
		if (Input.GetKey (KeyCode.W) && !isRunning || 
			Input.GetKey (KeyCode.A) && !isRunning || 
			Input.GetKey (KeyCode.S) && !isRunning || 
			Input.GetKey (KeyCode.D) && !isRunning) 
		{
			anim.SetBool ("Walk", true);
		} else {
			anim.SetBool ("Walk", false);
		}

		//trčanje pritiskom na tipku Shift
		if ((Input.GetKey (KeyCode.W) && Input.GetKey (KeyCode.LeftShift))) 
		{
			isRunning = true;
		} else {
			isRunning = false;
		}
		
		//nemogučnost ciljanja kod trčanja
		if (isRunning == true) 
		{
			anim.SetBool ("Run", true);
		} 
		else 
		{
			anim.SetBool ("Run", false);
		}
	}

	private IEnumerator GrenadeSpawnDelay () {
		
		yield return new WaitForSeconds (grenadeSpawnDelay);
		//Stvaranje granate na spawn pointu
		Instantiate(Prefabs.grenadePrefab, 
			Spawnpoints.grenadeSpawnPoint.transform.position, 
			Spawnpoints.grenadeSpawnPoint.transform.rotation);
	}
	//funkcija za auto reload
	private IEnumerator AutoReload () {
		
		yield return new WaitForSeconds (autoReloadDelay);

		if (outOfAmmo == true) 
		{
			anim.Play ("Reload Out Of Ammo", 0, 0f);
			mainAudioSource.clip = SoundClips.reloadSoundOutOfAmmo;
			mainAudioSource.Play ();
		} 

		currentAmmo = ammo;
		outOfAmmo = false;
	}

	//Reload
	private void Reload () {
		
		//provjera ako oružje nema više metaka
		if (outOfAmmo == true) 
		{
			anim.Play ("Reload Out Of Ammo", 0, 0f);
			mainAudioSource.clip = SoundClips.reloadSoundOutOfAmmo;
			mainAudioSource.Play ();

		} 
		else 
		{
			anim.Play ("Reload Ammo Left", 0, 0f);
			mainAudioSource.clip = SoundClips.reloadSoundAmmoLeft;
			mainAudioSource.Play ();
		}
		currentAmmo = ammo;
		outOfAmmo = false;
	}

	

	//Stvaranje svijetlosti prilikom pucnja
	private IEnumerator MuzzleFlashLight () {
		
		muzzleflashLight.enabled = true;
		yield return new WaitForSeconds (lightDuration);
		muzzleflashLight.enabled = false;
	}

	//Provjera animacije
	private void AnimationCheck () {
		

		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Reload Out Of Ammo") || 
			anim.GetCurrentAnimatorStateInfo (0).IsName ("Reload Ammo Left")) 
		{
			isReloading = true;
		} 
		else 
		{
			isReloading = false;
		}

		//Provjera pregleda oružja
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Inspect")) 
		{
			isInspecting = true;
		} 
		else 
		{
			isInspecting = false;
		}
	}
}
