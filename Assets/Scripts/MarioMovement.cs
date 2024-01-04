using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MarioMovement : MonoBehaviour {
	private Rigidbody m_rigidbody;
	private Camera m_camera;

	private Vector3 m_velocity;
	
	[SerializeField] private float m_moveSpeed = 20.0f,
								   m_maxJumpHeight = 4.5f,
								   m_maxJumpTime = 1.0f;
	[SerializeField] private float m_jumpForce => 2.0f * m_maxJumpHeight / (m_maxJumpTime / 2.0f);
	[SerializeField] private float m_gravity => -2.0f * m_maxJumpHeight / Mathf.Pow(m_maxJumpTime / 2.0f, 2);
	private float m_inputAxis;

	private bool m_grounded, m_hitLeft, m_hitRight, m_hitTop;
	private bool m_jumping;

	private void Awake() {
		m_rigidbody = GetComponent<Rigidbody>();
		m_camera = Camera.main;
	}

	private void Update() {
		HandleHorizontalMovement();

		Vector3 downOrigin = new Vector3(transform.position.x, transform.position.y - transform.localScale.y / 2.0f, transform.position.z);
		Vector3 upOrigin = new Vector3(transform.position.x, transform.position.y + transform.localScale.y / 2.0f, transform.position.z);
		// Vector3 leftOrigin = new Vector3(transform.position.x - transform.localScale.x / 4.0f, transform.position.y, transform.position.z);
		// Vector3 rightOrigin = new Vector3(transform.position.x + transform.localScale.x / 4.0f, transform.position.y, transform.position.z);
		Vector3 horizontalOrigin = new(transform.position.x, transform.position.y, transform.position.z);
        float verticalDistance = gameObject.transform.localScale.y / 2.0f;
        float horizontalDistance = gameObject.transform.localScale.x / 4.0f;
        float verticalRadius = gameObject.transform.localScale.y / 2.0f;
        float horizontalRadius = gameObject.transform.localScale.x / 4.0f;

		m_grounded = m_rigidbody.Raycast(Vector3.down, verticalDistance, verticalRadius);
        m_hitTop = m_rigidbody.Raycast(Vector3.up, verticalDistance, verticalRadius);
        m_hitLeft = m_rigidbody.Raycast(Vector3.left, horizontalDistance, horizontalRadius);
        m_hitRight = m_rigidbody.Raycast(Vector3.right, horizontalDistance, horizontalRadius);

		if(m_grounded || m_hitTop)
			HandleVerticalMovement();

			ApplyGravity();
	}

	private void FixedUpdate() {
		Vector3 position = m_rigidbody.position;
		position += m_velocity * Time.fixedDeltaTime;

		Vector2 leftEge = m_camera.ScreenToWorldPoint(Vector2.zero);
		Vector2 rightEdge = m_camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
		position.x = Mathf.Clamp(position.x, leftEge.x + transform.localScale.x / 2.0f, rightEdge.x);

		m_rigidbody.MovePosition(position);
	}

	private void HandleHorizontalMovement() {
		m_inputAxis = Input.GetAxisRaw("Horizontal");
		m_velocity.x = Mathf.MoveTowards(m_velocity.x, m_inputAxis * m_moveSpeed / 2.0f, m_moveSpeed * Time.deltaTime);

        if(m_hitLeft && m_inputAxis < 0.0f || m_hitRight && m_inputAxis > 0.0f) {
            m_velocity.x = 0.0f;
        }
	}

	private void HandleVerticalMovement() {
		m_velocity.y = Mathf.Max(m_velocity.y, 0.0f);
		m_jumping = m_velocity.y > 0.0f;

		if(Input.GetButtonDown("Jump")) {
			m_velocity.y = m_jumpForce;
			m_jumping = true;
		}
        if(m_jumping && m_hitTop)
            m_velocity.y = 0.0f;
	}

	private void ApplyGravity() {
		bool falling = m_velocity.y < 0.0f || !Input.GetButton("Jump");
		float multiplier = falling ? 2.0f : 1.0f;
	
        if(gameObject.transform.position.y > 1.0f) {
            m_velocity.y += m_gravity * multiplier * Time.deltaTime;
            m_velocity.y = Mathf.Max(m_velocity.y, m_gravity / 2.0f);
        }
		if(m_grounded && !m_jumping)
			m_velocity.y = 0.0f;
	}
}