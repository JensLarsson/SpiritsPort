using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectDisplay : MonoBehaviour
{

    /// <summary>
    /// Instantiates icons for the active effects
    /// </summary>
    /// <param name="effects"></param>
    public void ShowEffectIcons(List<OverTimeEffect> effects)
    {
        foreach (OverTimeEffect effect in effects)
        {
            Image image = new GameObject().AddComponent<Image>();
            image.transform.parent = this.transform;
            image.sprite = effect.Icon;
        }
    }

    /// <summary>
    /// Destroys the icon objects currently residing as child objects
    /// </summary>
    public void HideEffectIcons()
    {
        foreach (Image image in GetComponentsInChildren<Image>())
        {
            Destroy(image.gameObject);
        }
    }
}
