using UnityEngine;

//機關被按住/放開時切換材質
public class click_effect : MonoBehaviour
{
    public Renderer houserenderer;
    public Material[] housematerials;

    public void clickdown()
    {
        houserenderer.material = housematerials[2];
    }

    public void clickup()
    {
        houserenderer.material = housematerials[3];
    }
}
