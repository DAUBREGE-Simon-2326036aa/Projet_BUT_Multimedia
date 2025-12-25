using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace BUT
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Movement m_Movement;
        [SerializeField] Animator m_Animator;

        [Header("Animator Settings")]
        [SerializeField] string m_AnimatorSpeedParam = "Speed";

        // Propriété CurrentSpeed avec Setter modifié
        float m_CurrentSpeed;
        public float CurrentSpeed
        {
            set
            {
                m_CurrentSpeed = value;
                OnSpeedChange?.Invoke(m_CurrentSpeed);

                if (m_Animator != null)
                {
                    m_Animator.SetFloat(m_AnimatorSpeedParam, m_CurrentSpeed, 0.08f, Time.fixedDeltaTime);
                }
            }
            get => m_CurrentSpeed;
        }

        // État du Sprint
        bool m_IsSprinting;
        public bool IsSprinting { set => m_IsSprinting = value; get => m_IsSprinting; }

        // État de Mouvement
        private bool m_IsMoving;
        public bool IsMoving
        {
            set
            {
                if (m_IsMoving == value) return;
                m_IsMoving = value;
                OnMovingChange?.Invoke(m_IsMoving);
            }
            get => m_IsMoving;
        }

        // Directions et Vecteurs
        private Vector3 m_Direction;
        public Vector3 Direction { set => m_Direction = value; get => m_Direction; }

        public Vector3 FullDirection => (GroundRotationOffset * Direction * CurrentSpeed + Vector3.up * GravityVelocity);

        private Quaternion m_GroundRotationOffset;
        public Quaternion GroundRotationOffset { set => m_GroundRotationOffset = value; get => m_GroundRotationOffset; }

        // Gravité
        public const float GRAVITY = -9.81f; // Correction standard terrestre (était -9.31f)

        private float m_GravityVelocity;
        public float GravityVelocity { set => m_GravityVelocity = value; get => m_GravityVelocity; }

        // Saut
        private int m_JumpNumber;
        public int JumpNumber { set => m_JumpNumber = value; get => m_JumpNumber; }

        [Header("Ground Detection")]
        [SerializeField] float m_RayLength; // Correction ortho: RayLenght -> RayLength
        [SerializeField] LayerMask m_RayMask;

        RaycastHit m_Hit;

        private bool m_IsGrounded;
        public bool IsGrounded
        {
            set
            {
                if (m_IsGrounded == value) return;
                m_IsGrounded = value;
                OnGroundedChange?.Invoke(m_IsGrounded);
                if (m_Animator != null) m_Animator.SetBool("IsGrounded", m_IsGrounded);
            }
            get => m_IsGrounded;
        }

        // Composants internes
        private CharacterController m_CharacterController;
        private Transform m_CameraTransform; // Optimisation: Cache de la caméra
        private Vector2 m_MovementInput;
        private Vector3 m_MovementDirection;

        [Header("Events")]
        public UnityEvent<float> OnSpeedChange;
        public UnityEvent<bool> OnMovingChange;
        public UnityEvent<bool> OnGroundedChange;

        private void Awake()
        {
            m_CharacterController = GetComponent<CharacterController>();

            // Optimisation : On récupère la caméra une seule fois au début
            if (Camera.main != null)
            {
                m_CameraTransform = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning("Aucune MainCamera trouvée ! Le mouvement directionnel risque de ne pas fonctionner.");
            }

            // Sécurité : si l'Animator n'est pas assigné, on cherche un Animator sur l'enfant ou le même GameObject
            if (m_Animator == null)
            {
                m_Animator = GetComponentInChildren<Animator>();
            }
        }

        // --- Méthodes Publiques pour Events ---
        public void MovingChanged(bool _moving) => OnMovingChange?.Invoke(_moving);
        public void SpeedChanged(float _speed) => OnSpeedChange?.Invoke(_speed);
        public void GroundedChanged(bool _grounded) => OnGroundedChange?.Invoke(_grounded);

        private void OnDisable()
        {
            IsMoving = false;
            StopAllCoroutines(); // Bonne pratique pour éviter les coroutines fantômes
        }

        private void OnEnable()
        {
            StartCoroutine(Moving());
        }

        // Boucle principale de mouvement
        IEnumerator Moving()
        {
            while (enabled)
            {
                if (m_MovementInput.magnitude > 0.1f)
                {
                    if (!IsMoving) IsMoving = true;

                    // clamp input magnitude
                    m_MovementInput = Vector3.ClampMagnitude(m_MovementInput, 1);
                }
                else if (IsMoving)
                {
                    IsMoving = false;
                }

                ManageDirection();
                ManageGravity();

                if (IsMoving) ApplyRotation();

                ApplyMovement();

                yield return new WaitForFixedUpdate();
            }
        }

        // --- Input System Callbacks ---

        public void SetInputMove(InputAction.CallbackContext _context)
        {
            m_MovementInput = _context.ReadValue<Vector2>();
        }

        public void SetInputJump(InputAction.CallbackContext _context)
        {
            if (!_context.started || (!m_CharacterController.isGrounded && JumpNumber >= m_Movement.MaxJumpNumber)) return;

            if (JumpNumber == 0) StartCoroutine(WaitForLanding());
            JumpNumber++;

            if (m_Movement.MinimazeJumpPower)
                GravityVelocity += m_Movement.JumpPower / JumpNumber;
            else
                GravityVelocity += m_Movement.JumpPower;
        }

        public void SetInputSprint(InputAction.CallbackContext _context)
        {
            IsSprinting = _context.started || _context.performed;
        }

        IEnumerator WaitForLanding()
        {
            yield return new WaitUntil(() => !m_CharacterController.isGrounded);
            yield return new WaitUntil(() => m_CharacterController.isGrounded);
            JumpNumber = 0;
        }

        // --- Logique Core ---

        private void ManageDirection()
        {
            // Set direction based on input
            m_MovementDirection = new Vector3(m_MovementInput.x, 0, m_MovementInput.y);

            // Modify direction according to camera view (Utilisation du cache m_CameraTransform)
            if (m_CameraTransform != null)
            {
                m_MovementDirection = m_CameraTransform.TransformDirection(m_MovementDirection);
            }

            m_MovementDirection.y = transform.forward.y; // Garder l'orientation Y actuelle si nécessaire, ou 0 pour plat

            // Raycast Sol pour l'orientation
            Debug.DrawRay(transform.position, -transform.up * m_RayLength, Color.red);

            if (Physics.Raycast(transform.position, -transform.up, out m_Hit, m_RayLength, m_RayMask))
            {
                IsGrounded = true;
                float angleOffset = Vector3.SignedAngle(transform.up, m_Hit.normal, transform.right);
                GroundRotationOffset = Quaternion.AngleAxis(angleOffset, transform.right);
                Debug.DrawRay(transform.position, GroundRotationOffset * m_MovementDirection, Color.green);
            }
            else
            {
                IsGrounded = m_CharacterController.isGrounded;
                GroundRotationOffset = Quaternion.identity;
            }

            m_MovementDirection.Normalize();
            Direction = m_MovementDirection;

            Debug.DrawRay(transform.position, Direction, Color.red);

            // Calculate speed
            CurrentSpeed = ((IsSprinting) ? m_Movement.SprintFactor : 1) * m_Movement.MaxSpeed * m_Movement.SpeedFactor.Evaluate(m_MovementInput.magnitude);
        }

        public void ApplyRotation()
        {
            if (!IsMoving || Direction.magnitude < 0.1f) return;

            // Calculate target rotation
            // On projette la direction sur le plan horizontal pour éviter que le perso ne penche en avant/arrière
            Vector3 lookDirection = Vector3.ProjectOnPlane(Direction, transform.up);

            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection, transform.up);

                // Lerp toward the target rotation
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation,
                    m_Movement.MaxAngularSpeed * Mathf.Deg2Rad * m_Movement.AngularSpeedFactor.Evaluate(Direction.magnitude) * Time.fixedDeltaTime);
            }
        }

        public void ApplyMovement()
        {
            Debug.DrawRay(transform.position, FullDirection, Color.yellow);
            // Move toward the direction with the current speed
            m_CharacterController.Move(FullDirection * Time.fixedDeltaTime);
        }

        private void ManageGravity()
        {
            if (m_CharacterController.isGrounded && GravityVelocity <= 0)
            {
                JumpNumber = 0;
                GravityVelocity = -2f; // Force constante pour plaquer au sol
            }
            else
            {
                // Apply Gravity
                GravityVelocity += GRAVITY * m_Movement.GravityMultiplier * Time.fixedDeltaTime;
            }
        }
    }
}