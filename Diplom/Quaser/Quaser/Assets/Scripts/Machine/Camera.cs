using UnityEngine;

public class Camera : MonoBehaviour
{
    private readonly float speed = 0.02f;
    private readonly float speedRotation = 2f;
    
    private float mouseWheelDirection;
    private float MouseX;
    private float MouseY;

	void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * speed);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * speed);
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.up * speed);
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.down * speed);
        }

        if(Input.GetMouseButton(2))
        {
            MouseX = Input.GetAxis("Mouse X") * speedRotation;
            MouseY = -Input.GetAxis("Mouse Y") * speedRotation;

            transform.rotation *= Quaternion.Euler(MouseY, MouseX, 0);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
                transform.rotation.eulerAngles.y,
                0);
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            mouseWheelDirection = Input.GetAxis("Mouse ScrollWheel");
            transform.Translate(Vector3.forward * speed * mouseWheelDirection * 50);
        }
    }
}
