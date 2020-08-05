using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image imagePrefab;
    bool healthShowing = false;

    public void ShowHealth(int maxHealth, int currentHealth, bool overwriteMouseOver = false)
    {
        if (healthShowing && overwriteMouseOver)
        {
            HideHealth();
        }

        if (!healthShowing)
        {
            healthShowing = true;
            for (int i = 1; i <= maxHealth; i++)
            {
                Image image = Instantiate(imagePrefab, this.transform);
                if (i > currentHealth)
                {
                    image.color = Color.black;
                }
            }
        }
    }

    public void HideHealth()
    {
        healthShowing = false;
        foreach (Image image in GetComponentsInChildren<Image>())
        {
            Destroy(image.gameObject);
        }
    }

}
