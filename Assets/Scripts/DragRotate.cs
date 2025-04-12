using UnityEngine;

public class DragRotate : MonoBehaviour
{

    public float rotationSpeed = 0.2f;
    private Vector2 lastPosition;
    private bool isDragging;

    Quaternion InitialRotation;

    private void Start()
    {
        InitialRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastPosition;
            Rotate(delta);
            lastPosition = Input.mousePosition;
        }
    }

    void Rotate(Vector2 delta)
    {
        float rotX = -delta.y * rotationSpeed;
        float rotY = delta.x * rotationSpeed;
        transform.Rotate(-rotX, -rotY, 0, Space.World);
    }

    public void ResetRotation()
    {
        transform.rotation = Quaternion.identity;
    }
}
