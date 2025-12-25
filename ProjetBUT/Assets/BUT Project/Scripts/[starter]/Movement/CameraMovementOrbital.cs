using UnityEngine;
using UnityEngine.InputSystem;

namespace BUT
{
    public class CameraMovementOrbital : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Transform m_Target;
        [SerializeField] private Vector3 m_TargetOffset = new Vector3(0f, 1.5f, 0f); // Point de visée (tête)
        [SerializeField] private Vector3 m_DefaultOffset = new Vector3(0f, 2f, -5f);

        [Header("Rotation")]
        [SerializeField] private float m_SpeedRotation = 100f;
        [SerializeField] private Vector2 m_RotationXLimits = new Vector2(-45f, 45f);

        [Header("Collision")]
        [SerializeField] private LayerMask m_CollisionLayers;
        [SerializeField] private float m_CollisionBuffer = 0.2f;

        private Vector3 m_CurrentRotation;
        private float m_MaxDistance;
        private Vector3 m_CalculatedOffset;

        private PlayerInput m_PlayerInput;
        private InputAction m_LookAction;

        private void Start()
        {
            m_CurrentRotation = transform.eulerAngles;
            m_MaxDistance = m_DefaultOffset.magnitude;
            m_CalculatedOffset = m_DefaultOffset;

            if (m_Target == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) m_Target = player.transform;
            }

            m_PlayerInput = FindObjectOfType<PlayerInput>();
            if (m_PlayerInput != null)
            {
                m_LookAction = m_PlayerInput.actions["Look"];
                m_LookAction.performed += OnLook;
                m_LookAction.Enable();
            }
        }

        private void OnDestroy()
        {
            if (m_LookAction != null) m_LookAction.performed -= OnLook;
        }

        private void LateUpdate()
        {
            if (m_Target != null)
            {
                Vector3 pivotPos = m_Target.position + m_TargetOffset;
                Vector3 desiredPosition = pivotPos + m_CalculatedOffset;

                Vector3 direction = (desiredPosition - pivotPos).normalized;
                float distance = Vector3.Distance(pivotPos, desiredPosition);

                // Gestion de la collision
                if (Physics.Raycast(pivotPos, direction, out RaycastHit hit, distance, m_CollisionLayers))
                {
                    transform.position = hit.point - (direction * m_CollisionBuffer);
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10f);
                }

                transform.LookAt(pivotPos);
            }
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();

            m_CurrentRotation.y += input.x * m_SpeedRotation * Time.deltaTime;
            m_CurrentRotation.x -= input.y * m_SpeedRotation * Time.deltaTime;
            m_CurrentRotation.x = Mathf.Clamp(m_CurrentRotation.x, m_RotationXLimits.x, m_RotationXLimits.y);

            Quaternion rotation = Quaternion.Euler(m_CurrentRotation.x, m_CurrentRotation.y, 0);
            m_CalculatedOffset = rotation * new Vector3(0, 0, -m_MaxDistance);
        }
    }
}