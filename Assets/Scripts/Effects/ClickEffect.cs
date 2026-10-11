using UnityEngine;
using UnityEngine.Serialization;

//機關被按住/放開時切換材質
public class ClickEffect : MonoBehaviour
{
    [FormerlySerializedAs("houserenderer")] public Renderer targetRenderer;
    [FormerlySerializedAs("housematerials")] public Material[] materials;//2是按住，3是放開

    public void Press()
    {
        targetRenderer.material = materials[2];
    }

    public void Release()
    {
        targetRenderer.material = materials[3];
    }
}
