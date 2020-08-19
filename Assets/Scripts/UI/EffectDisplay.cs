using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectDisplay : MonoBehaviour
{
    [SerializeField] Image imagePrefab;

    /// <summary>
    /// Instantiates icons for the active effects
    /// </summary>
    /// <param name="effects"></param>
    public void ShowEffectIcons(List<OverTimeEffect> effects)
    {
        foreach (OverTimeEffect effect in effects)
        {
            Image image = Instantiate(imagePrefab, this.transform);
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
