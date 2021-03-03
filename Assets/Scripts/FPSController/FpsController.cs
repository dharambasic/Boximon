using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


    public class FpsController : MonoBehaviour
    {
#pragma warning disable 649

    //Postavke oružja
		[Header("Arms")]
        [SerializeField]
        private Transform arms;
        [SerializeField]
        private Transform arms1;
        [SerializeField]
        private Vector3 armPosition;
    //postavke zvukova
		[Header("Audio Clips")]
        [SerializeField]
        private AudioClip walkingSound;
        [SerializeField]
        private AudioClip runningSound;
    //postavke kretanja
		[Header("Movement Settings")]
        [SerializeField]
        private float walkingSpeed = 5f;
        [SerializeField]
        private float runningSpeed = 9f;
        [SerializeField]
        private float movementSmoothness = 0.125f;
        [SerializeField]
        private float jumpForce = 35f;
    //postavke pogleda mišem
		[Header("Look Settings")]
        [SerializeField]
        private float mouseSensitivity = 7f;
        [SerializeField]
        private float rotationSmoothness = 0.05f;
        [SerializeField]
        private float minVerticalAngle = -90f;     
        [SerializeField]
        private float maxVerticalAngle = 90f;
        [SerializeField]
        private FpsInput input;
        private Rigidbody _rigidbody;
        private CapsuleCollider _collider;
        private AudioSource _audioSource;
        private SmoothRotation _rotationX;
        private SmoothRotation _rotationY;
        private SmoothVelocity _velocityX;
        private SmoothVelocity _velocityZ;
        private bool _isGrounded;
    //varijabla za zdravlje igrača
        public int playerHealth = 100;
        public GameObject Assault;
        public GameObject Pistol;
        public GameObject Canvas;
        public Enemy dragon;
    bool paused = false;
    //postavljanje raycasthit varijable
        private readonly RaycastHit[] _groundCastResults = new RaycastHit[8];
        private readonly RaycastHit[] _wallCastResults = new RaycastHit[8];

        /// inicijalizacija kontrolera na startu
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            _collider = GetComponent<CapsuleCollider>();
            _audioSource = GetComponent<AudioSource>();
			arms = AssignCharactersCamera();
        arms1 = AssignCharactersCamera1();
        _audioSource.clip = walkingSound;
            _audioSource.loop = true;
            _rotationX = new SmoothRotation(RotationXRaw);
            _rotationY = new SmoothRotation(RotationYRaw);
            _velocityX = new SmoothVelocity();
            _velocityZ = new SmoothVelocity();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            ValidateRotationRestriction();
        Canvas.gameObject.SetActive(false);
       
        //zaustavljanje glazbe iz start scene u game scenu
            GameObject.FindGameObjectWithTag("Music").GetComponent<Music>().StopMusic();
        
    }
		//postavljanje kamere na karakter
        private Transform AssignCharactersCamera()
        {
            var t = transform;
			arms.SetPositionAndRotation(t.position, t.rotation);
			return arms;
        }
    private Transform AssignCharactersCamera1()
    {
        var t1 = transform;
        arms1.SetPositionAndRotation(t1.position, t1.rotation);
        return arms1;
    }

//optimizacija kuta miša
    private void ValidateRotationRestriction()
        {
            minVerticalAngle = ClampRotationRestriction(minVerticalAngle, -90, 90);
            maxVerticalAngle = ClampRotationRestriction(maxVerticalAngle, -90, 90);
            if (maxVerticalAngle >= minVerticalAngle) return;
            var min = minVerticalAngle;
            minVerticalAngle = maxVerticalAngle;
            maxVerticalAngle = min;
        }

        private static float ClampRotationRestriction(float rotationRestriction, float min, float max)
        {
            if (rotationRestriction >= min && rotationRestriction <= max) return rotationRestriction;
            return Mathf.Clamp(rotationRestriction, min, max);
        }
			
        /// provjerava dali je igrač na zemlji
        private void OnCollisionStay()
        {
            var bounds = _collider.bounds;
            var extents = bounds.extents;
            var radius = extents.x - 0.01f;
            Physics.SphereCastNonAlloc(bounds.center, radius, Vector3.down,
                _groundCastResults, extents.y - radius * 0.5f, ~0, QueryTriggerInteraction.Ignore);
            if (!_groundCastResults.Any(hit => hit.collider != null && hit.collider != _collider)) return;
            for (var i = 0; i < _groundCastResults.Length; i++)
            {
                _groundCastResults[i] = new RaycastHit();
            }

            _isGrounded = true;
        }
			
        /// rotira kameru prema kretnji igrača.
        private void FixedUpdate()
        {
            
            RotateCameraAndCharacter();
            MoveCharacter();
            _isGrounded = false;
        }
			
        /// Pomakne kameru prema igraču i brine se o zvukovima
        /// Ako se pritisne tipka Escape, aktivira se Canvas s Pause menijem
        private void Update()
        {
			arms.position = transform.position + transform.TransformVector(armPosition);
        arms1.position = transform.position + transform.TransformVector(armPosition);
        Jump();
            PlayFootstepSounds();
        //pritiskom na tipku 2, aktivira se gameObbject s Pištoljem
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            Pistol.gameObject.SetActive(true);
            Assault.gameObject.SetActive(false);
        }
        //pritiskom na tipku 1, aktivira se gameObject s automatskim oružjem
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Assault.gameObject.SetActive(true);
            Pistol.gameObject.SetActive(false);
        }

        //pritiskom na tipku Esc, aktivira se Pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(paused == true)
            {
                Time.timeScale = 1.0f;
                Canvas.gameObject.SetActive(false);
                
                paused = false;
                Cursor.lockState = CursorLockMode.Locked;
            
              
            } else
            {
                Time.timeScale = 0.0f;
                Canvas.gameObject.SetActive(true);
               
                paused = true;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
              
            }
        }

        //provjerava ako je zadnji "Boss" ubijen, čim je ubijen učitava se Win scena
        if(dragon.npcHP <= 0)
        {
            LevelMenager man = GameObject.Find("LevelMenager").GetComponent<LevelMenager>();
            man.LoadLevel("Win");
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

       
    }

   
        //Rotacija kamere prema kretanju karaktera
        private void RotateCameraAndCharacter()
        {
            var rotationX = _rotationX.Update(RotationXRaw, rotationSmoothness);
            var rotationY = _rotationY.Update(RotationYRaw, rotationSmoothness);
            var clampedY = RestrictVerticalRotation(rotationY);
            _rotationY.Current = clampedY;
			var worldUp = arms.InverseTransformDirection(Vector3.up);
			var rotation = arms.rotation *
                           Quaternion.AngleAxis(rotationX, worldUp) *
                           Quaternion.AngleAxis(clampedY, Vector3.left);
            transform.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
			arms.rotation = rotation;
        var worldUp1 = arms1.InverseTransformDirection(Vector3.up);
        var rotation1 = arms1.rotation *
                       Quaternion.AngleAxis(rotationX, worldUp) *
                       Quaternion.AngleAxis(clampedY, Vector3.left);
        transform.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
        arms1.rotation = rotation;
       }
			
        /// funkcija vrača rotaciju kamere oko y osi
        private float RotationXRaw
        {
            get { return input.RotateX * mouseSensitivity; }
        }
    
    /// funkcija vrača rotaciju kamere oko x osi
    private float RotationYRaw
        {
            get { return input.RotateY * mouseSensitivity; }
        }
			
        /// Ograničena vertikalna rotacija     
        private float RestrictVerticalRotation(float mouseY)
        {
			var currentAngle = NormalizeAngle(arms.eulerAngles.x);
        var currentAngle1 = NormalizeAngle(arms1.eulerAngles.x);
        var minY = minVerticalAngle + currentAngle;
            var maxY = maxVerticalAngle + currentAngle;
            return Mathf.Clamp(mouseY, minY + 0.01f, maxY - 0.01f);
        }
			
        /// Normalizacija kuteva
        private static float NormalizeAngle(float angleDegrees)
        {
            while (angleDegrees > 180f)
            {
                angleDegrees -= 360f;
            }

            while (angleDegrees <= -180f)
            {
                angleDegrees += 360f;
            }

            return angleDegrees;
        }
    //Funkcija kretanja igrača
        private void MoveCharacter()
        {
            var direction = new Vector3(input.Move, 0f, input.Strafe).normalized;
            var worldDirection = transform.TransformDirection(direction);
            var velocity = worldDirection * (input.Run ? runningSpeed : walkingSpeed);
          
         

            var smoothX = _velocityX.Update(velocity.x, movementSmoothness);
            var smoothZ = _velocityZ.Update(velocity.z, movementSmoothness);
            var rigidbodyVelocity = _rigidbody.velocity;
            var force = new Vector3(smoothX - rigidbodyVelocity.x, 0f, smoothZ - rigidbodyVelocity.z);
            _rigidbody.AddForce(force, ForceMode.VelocityChange);
        }

   
    //funkcija za skakanje karaktera
        private void Jump()
        {
            if (!_isGrounded || !input.Jump) return;
            _isGrounded = true;
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    // zvuk koraka
        private void PlayFootstepSounds()
        {
            if (_isGrounded && _rigidbody.velocity.sqrMagnitude > 0.1f)
            {
                _audioSource.clip = input.Run ? runningSound : walkingSound;
                if (!_audioSource.isPlaying)
                {
                    _audioSource.Play();
                }
            }
            else
            {
                if (_audioSource.isPlaying)
                {
                    _audioSource.Pause();
                }
            }
        }
			
        /// Pomoc kod glatke rotacije mišem
        private class SmoothRotation
        {
            private float _current;
            private float _currentVelocity;

            public SmoothRotation(float startAngle)
            {
                _current = startAngle;
            }
				
            /// Vraća glatku rotaciju
            public float Update(float target, float smoothTime)
            {
                return _current = Mathf.SmoothDampAngle(_current, target, ref _currentVelocity, smoothTime);
            }

            public float Current
            {
                set { _current = value; }
            }
        }
			
        /// pomoc kod glađeg kretanja
        private class SmoothVelocity
        {
            private float _current;
            private float _currentVelocity;

            /// Vraća glađe kretanje
            public float Update(float target, float smoothTime)
            {
                return _current = Mathf.SmoothDamp(_current, target, ref _currentVelocity, smoothTime);
            }

            public float Current
            {
                set { _current = value; }
            }
        }
			
        /// Input 
        [Serializable]
        private class FpsInput
        {
            [SerializeField]
            private string rotateX = "Mouse X";

            [SerializeField]
            private string rotateY = "Mouse Y";

             [SerializeField]
            private string move = "Horizontal";

             [SerializeField]
            private string strafe = "Vertical";

             [SerializeField]
            private string run = "Fire3";

             [SerializeField]
            private string jump = "Jump";

            /// vraća vrijednost virtualne osi x
            public float RotateX
            {
                get { return Input.GetAxisRaw(rotateX); }
            }
				         
            /// vraća vrijedost virtualne osi y  
            public float RotateY
            {
                get { return Input.GetAxisRaw(rotateY); }
            }
				        
            /// Vraća vrijednost kretanja karaktera naprijed i nazad
            public float Move
            {
                get { return Input.GetAxisRaw(move); }
            }
				       
            /// vraća vrijednost kretanja karatkrea lijevo i desno       
            public float Strafe
            {
                get { return Input.GetAxisRaw(strafe); }
            }
				    
            /// Vrača točno ili netočno s obzirom na to da li je tipka za trčati pritisnuta       
            public bool Run
            {
                get { return Input.GetButton(run); }
            }

        ///Vrača točno ili netočno s obzirom na to da li je tipka za skakanje pritisnuta               
        public bool Jump
            {
                get { return Input.GetButtonDown(jump); }
            }
        }

   


}
