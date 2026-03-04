using UnityEngine;

public class BlockDrag : MonoBehaviour
{
    private Camera cam;

    private Rigidbody highlightedBlock;
    private Rigidbody selectedBlock;

    private Vector3 offset;
    private bool isDragging = false;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
#if UNITY_EDITOR
        HandleMouse();
#endif
        HandleTouch();
    }

    void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
            TrySelect(Input.mousePosition);

        if (Input.GetMouseButton(0) && isDragging && selectedBlock != null)
            Drag(Input.mousePosition);

        if (Input.GetMouseButtonUp(0))
            StopDragging();
    }

    void HandleTouch()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
            TrySelect(touch.position);

        if (touch.phase == TouchPhase.Moved && isDragging && selectedBlock != null)
            Drag(touch.position);

        if (touch.phase == TouchPhase.Ended)
            StopDragging();
    }

    void TrySelect(Vector3 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit)) return;
        if (!hit.collider.CompareTag("Block")) return;

        Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
        if (rb == null) return;

        // Aynı bloğa ikinci dokunuş → sürükleme başlat
        if (highlightedBlock == rb)
        {
            selectedBlock = rb;
            offset = selectedBlock.position - hit.point;
            isDragging = true;
            return;
        }

        // Yeni blok seçildi
        ClearHighlight();

        highlightedBlock = rb;

        SimpleOutline outline = highlightedBlock.GetComponent<SimpleOutline>();
        if (outline != null)
            outline.EnableOutline();
    }

    void Drag(Vector3 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit)) return;

        Vector3 targetPos = hit.point + offset;

        targetPos.y = selectedBlock.position.y;
        targetPos.z = selectedBlock.position.z;

        selectedBlock.MovePosition(targetPos);
    }

    void StopDragging()
    {
        isDragging = false;
        selectedBlock = null;
    }

    void ClearHighlight()
    {
        if (highlightedBlock != null)
        {
            SimpleOutline outline = highlightedBlock.GetComponent<SimpleOutline>();
            if (outline != null)
                outline.DisableOutline();
        }

        highlightedBlock = null;
    }
}