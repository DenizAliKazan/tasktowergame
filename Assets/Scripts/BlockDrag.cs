using UnityEngine;

public class BlockDrag : MonoBehaviour
{
    private Rigidbody selectedBlock;
    private Vector3 offset;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // Mouse desteði
        #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Block"))
                {
                    selectedBlock = hit.collider.GetComponent<Rigidbody>();
                    offset = selectedBlock.position - hit.point;
                }
            }
        }

        if (Input.GetMouseButton(0) && selectedBlock != null)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 targetPos = hit.point + offset;

                targetPos.y = selectedBlock.position.y; // sadece X ekseni
                targetPos.z = selectedBlock.position.z;

                selectedBlock.MovePosition(targetPos);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            selectedBlock = null;
        }
        #endif

        // Touch desteði (mobil)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = cam.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (touch.phase == TouchPhase.Began)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("Block"))
                    {
                        selectedBlock = hit.collider.GetComponent<Rigidbody>();
                        offset = selectedBlock.position - hit.point;
                    }
                }
            }

            if (touch.phase == TouchPhase.Moved && selectedBlock != null)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 targetPos = hit.point + offset;

                    targetPos.y = selectedBlock.position.y; // sadece X ekseni
                    targetPos.z = selectedBlock.position.z;

                    selectedBlock.MovePosition(targetPos);
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                selectedBlock = null;
            }
        }
    }
}