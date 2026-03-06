using UnityEngine;

public class BlockDrag : MonoBehaviour
{
    private Rigidbody selectedBlock;
    private Rigidbody highlightedBlock;
    private Vector3 offset;
    private Camera cam;

    [Header("Glow Ayarları")]
    public Color glowColor = Color.white;
    [Range(0f, 10f)] public float outlineWidth = 5f;

    [Header("Fizik Dengesi")]
    [Tooltip("Sürüklenen bloğun diğerlerini fırlatmaması için kütle çarpanı")]
    public float dragMassMultiplier = 1f;

    void Start()
    {
        cam = Camera.main;
        StabilizeTower();
    }

    // Oyun başında kulenin patlamasını ve sallanmasını önler
    void StabilizeTower()
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");

        foreach (GameObject block in blocks)
        {
            Rigidbody rb = block.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.Sleep();
            }
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        HandleMouseInput();
#endif

        if (Input.touchCount > 0)
        {
            HandleTouchInput();
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) ProcessClick(Input.mousePosition);
        if (Input.GetMouseButton(0) && selectedBlock != null) DragBlock(Input.mousePosition);
        if (Input.GetMouseButtonUp(0)) ReleaseBlock();
    }

    private void HandleTouchInput()
    {
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began) ProcessClick(touch.position);
        else if (touch.phase == TouchPhase.Moved && selectedBlock != null) DragBlock(touch.position);
        else if (touch.phase == TouchPhase.Ended) ReleaseBlock();
    }

    private void ProcessClick(Vector2 screenPosition)
    {
        Ray ray = cam.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Block"))
            {
                Rigidbody hitRb = hit.collider.GetComponent<Rigidbody>();

                if (highlightedBlock == hitRb)
                {
                    selectedBlock = hitRb;

                    // Fizik etkisini kapat (kuleyi itmesin)
                    selectedBlock.isKinematic = true;

                    offset = selectedBlock.position - hit.point;
                }
                else
                {
                    SetHighlight(hitRb);
                }
            }
        }
    }

    private void DragBlock(Vector2 screenPosition)
    {
        Ray ray = cam.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPos = hit.point + offset;

            // Yukarı kalkamasın
            targetPos.y = selectedBlock.position.y;

            selectedBlock.transform.position = targetPos;
        }
    }

    private void ReleaseBlock()
    {
        if (selectedBlock != null)
        {
            // Fizik geri açılsın
            selectedBlock.isKinematic = false;

            selectedBlock = null;
        }
    }

    private void SetHighlight(Rigidbody target)
    {
        if (highlightedBlock != null) ToggleOutline(highlightedBlock, false);

        highlightedBlock = target;

        if (highlightedBlock != null) ToggleOutline(highlightedBlock, true);
    }

    private void ToggleOutline(Rigidbody rb, bool state)
    {
        Renderer rend = rb.GetComponent<Renderer>();

        if (rend != null)
        {
            if (state)
            {
                rend.material.SetColor("_EmissionColor", glowColor * 0.5f);
                rend.material.EnableKeyword("_EMISSION");
            }
            else
            {
                rend.material.DisableKeyword("_EMISSION");
            }
        }
    }
}