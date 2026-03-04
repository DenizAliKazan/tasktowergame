using UnityEngine;

public class SimpleOutline : MonoBehaviour
{
    public Material outlineMaterial;

    private GameObject outlineObject;

    void Awake()
    {
        CreateOutline();
        outlineObject.SetActive(false);
    }

    void CreateOutline()
    {
        outlineObject = new GameObject("Outline");
        outlineObject.transform.SetParent(transform);
        outlineObject.transform.localPosition = Vector3.zero;
        outlineObject.transform.localRotation = Quaternion.identity;
        outlineObject.transform.localScale = Vector3.one;

        MeshFilter mf = outlineObject.AddComponent<MeshFilter>();
        mf.mesh = GetComponent<MeshFilter>().mesh;

        MeshRenderer mr = outlineObject.AddComponent<MeshRenderer>();
        mr.material = outlineMaterial;
    }

    public void EnableOutline()
    {
        if (outlineObject != null)
            outlineObject.SetActive(true);
    }

    public void DisableOutline()
    {
        if (outlineObject != null)
            outlineObject.SetActive(false);
    }
}