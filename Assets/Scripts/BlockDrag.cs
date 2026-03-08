using UnityEngine;

public class BlockDrag : MonoBehaviour
{
    private Rigidbody glowingBlock;
    private Rigidbody draggingBlock;
    
    private Vector3 offset;
    private float zDistance; 
    private Camera cam;
    private Color originalColor;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        #if UNITY_EDITOR
        HandleMouseInput();
        #else
        HandleTouchInput();
        #endif
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) ProcessTouchDown(Input.mousePosition);
        if (Input.GetMouseButton(0) && draggingBlock != null) ProcessTouchMove(Input.mousePosition);
        if (Input.GetMouseButtonUp(0)) ProcessTouchUp();
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) ProcessTouchDown(touch.position);
            else if (touch.phase == TouchPhase.Moved && draggingBlock != null) ProcessTouchMove(touch.position);
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) ProcessTouchUp();
        }
    }

    void ProcessTouchDown(Vector2 screenPosition)
    {
        Ray ray = cam.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Block"))
            {
                Rigidbody hitRigidbody = hit.collider.GetComponent<Rigidbody>();

                if (glowingBlock == hitRigidbody)
                {
                    draggingBlock = hitRigidbody;
                    
                    // SİHİRLİ AYARLAR:
                    // Sürüklerken kuleyi sarsmasın diye ağırlığı geçici artırabilir veya sürtünmeyi düşürebiliriz
                    draggingBlock.useGravity = false; 
                    draggingBlock.isKinematic = false; // Asla kinematic yapmıyoruz
                    draggingBlock.interpolation = RigidbodyInterpolation.Interpolate; // Hareket pürüzsüzlüğü

                    zDistance = cam.WorldToScreenPoint(draggingBlock.position).z;
                    Vector3 clickWorldPosition = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, zDistance));
                    offset = draggingBlock.position - clickWorldPosition;
                }
                else
                {
                    RemoveGlow();
                    glowingBlock = hitRigidbody;
                    ApplyGlow(glowingBlock);
                }
            }
            else { RemoveGlow(); }
        }
        else { RemoveGlow(); }
    }

    void ProcessTouchMove(Vector2 screenPosition)
    {
        Vector3 currentScreenPoint = new Vector3(screenPosition.x, screenPosition.y, zDistance);
        Vector3 currentWorldPoint = cam.ScreenToWorldPoint(currentScreenPoint);

        Vector3 targetPos = currentWorldPoint + offset;

        // Kısıtlamalar: Sadece X ekseni (veya senin oyununa göre Z)
        targetPos.y = draggingBlock.position.y;
        targetPos.z = draggingBlock.position.z;

        // EN STABİL YÖNTEM: MovePosition
        // Bu komut objeyi hedefe fizik kuralları içinde "iter" ama kontrol dışı hızlanmaz.
        draggingBlock.MovePosition(targetPos);
        
        // Taşı çekerken kuleyi devirmemesi için hızı limitliyoruz
        if(draggingBlock.linearVelocity.magnitude > 5f) 
            draggingBlock.linearVelocity = draggingBlock.linearVelocity.normalized * 5f;
    }

    void ProcessTouchUp()
    {
        if (draggingBlock != null)
        {
            // HAVADA KALMA SORUNU ÇÖZÜMÜ:
            draggingBlock.useGravity = true; 
            draggingBlock.linearVelocity = Vector3.zero; // Bırakınca savrulmasın
            draggingBlock.angularVelocity = Vector3.zero; // Kendi etrafında dönmesin
            
            draggingBlock = null;
        }
    }

    // --- GÖRSEL ---
    void ApplyGlow(Rigidbody block)
    {
        if (block == null) return;
        Renderer renderer = block.GetComponent<Renderer>();
        if (renderer != null)
        {
            originalColor = renderer.material.color;
            renderer.material.EnableKeyword("_EMISSION");
            renderer.material.SetColor("_EmissionColor", Color.white * 0.4f); 
        }
    }

    void RemoveGlow()
    {
        if (glowingBlock == null) return;
        Renderer renderer = glowingBlock.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.DisableKeyword("_EMISSION");
            renderer.material.SetColor("_EmissionColor", Color.black); 
        }
        glowingBlock = null;
    }
}