using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
   public float speed;
   public float runSpeed;
   public float jumpHeight;
   public float gravity;
   public float mouseSensitivity;
   public float cameraDistance;
   public bool thirdPerson;
   public float rotationSpeed;
   public GameObject camera;
   public GameObject eye;
   private CharacterController controller;
   private PlayerInput playerInput;
   private InputAction moveAction;
   private InputAction jumpAction;
   private InputAction lookAction;
   private InputAction runAction;

   private Vector3 moveDir;
   private Vector3 velocity;
   private bool isRunning;
   private float rotationY;
   private float rotationX;
   private Vector3 cameraOffset;

   [SerializeField] private bool isGrounded;


   void Start()
   {
      Cursor.visible = false;
      Cursor.lockState = CursorLockMode.Locked;
      controller = GetComponent<CharacterController>();
      playerInput = GetComponent<PlayerInput>();

      moveAction = playerInput.actions["Move"];
      jumpAction = playerInput.actions["Jump"];
      lookAction = playerInput.actions["Look"];
      runAction = playerInput.actions["Run"];

      moveAction.Enable();
      jumpAction.Enable();
      lookAction.Enable();
      runAction.Enable();

      // init values
      moveDir = Vector3.zero;
      velocity = Vector3.zero;
      speed = 10f;
      runSpeed = 20f;
      jumpHeight = 2f;
      gravity = -9.81f;
      mouseSensitivity = 0.1f;
      rotationSpeed = 20f;
      isRunning = false;
      thirdPerson = true;
      rotationX = 0;
      rotationY = 0;
      cameraDistance = -7f;
      cameraOffset = new Vector3(0, 5, cameraDistance);

   }

   void Update()
   {
      InputSystem();
      Movement();
   }

   private void LateUpdate()
   {
      CameraMovement();
   }

   void InputSystem()
   {
      // Apply Rotation from mouse
      rotationX += lookAction.ReadValue<Vector2>().x * mouseSensitivity;
      rotationY += lookAction.ReadValue<Vector2>().y * mouseSensitivity;


      // Move based on transform rotation & use input vector as direction (-1, 0, 1)
      Vector2 moveInput = moveAction.ReadValue<Vector2>();
      Vector3 forward = camera.transform.forward;
      Vector3 right = camera.transform.right;
      forward.y = 0;
      right.y = 0;
      moveDir = forward * moveInput.y + right * moveInput.x;
      isRunning = runAction.IsPressed();
      isGrounded = controller.isGrounded;
   }

   void Movement()
   {
      // Ground Check
     if (controller.isGrounded && velocity.y < 0)
     {
        velocity.y = 0;
     }

     // Move
     float curSpeed = isRunning ? runSpeed : speed;
     if (!thirdPerson)
     {
       controller.transform.rotation = Quaternion.Euler(0, rotationX, 0);
       controller.Move(moveDir * curSpeed * Time.deltaTime);
     }
     else
     {

       controller.Move(moveDir * curSpeed * Time.deltaTime);
       if (moveDir.magnitude > 0.1f)
       {
          controller.transform.forward = Vector3.Slerp(controller.transform.forward, moveDir, Time.deltaTime * rotationSpeed);
       }
     }

     // Jump
     if (isGrounded && jumpAction.IsPressed())
     {
        velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
     }
     velocity.y += gravity * Time.deltaTime;
     controller.Move(velocity * Time.deltaTime);
   }

   void CameraMovement()
   {
      if(!thirdPerson)
      {
         camera.transform.position = eye.transform.position;
         camera.transform.rotation = Quaternion.Euler(-rotationY, rotationX, 0);
      }
      else
      {
         camera.transform.position = transform.position + cameraOffset;
         camera.transform.LookAt(transform.position);
         camera.transform.RotateAround(transform.position,Vector3.up,rotationX);
      }
   }
}
