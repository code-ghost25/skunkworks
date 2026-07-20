using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform playerBody;
    [SerializeField] private float sensitivity = 0.12f;
    [SerializeField] private float minPitch = -85f;
    [SerializeField] private float maxPitch = 85f;
    
    [Header("Recoil")]
    [SerializeField] private float recoilRecoverySpeed = 8f; // how fast the recoil settles back down

    private float pitch;
    private float recoilOffset;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    private void Update()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float yaw = mouseDelta.x * sensitivity;
        pitch -= mouseDelta.y * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        // Recoil offset decays back toward 0 every frame, independent of mouse input
        recoilOffset = Mathf.MoveTowards(recoilOffset, 0f, recoilRecoverySpeed * Time.deltaTime);
        
        //Yaw rotates left and right
        playerBody.Rotate(Vector3.up * yaw);
        //Pitch rotates camera up and down
        transform.localRotation = Quaternion.Euler(pitch + recoilOffset, 0f, 0f);
        //print(recoilOffset); debug cause im bad
    }
    
    // call this from the weapon script on each shot. psitive = kicks view upward
    public void AddRecoil(float amount)
    {
        recoilOffset -= amount;
    }
}
