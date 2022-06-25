using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIPanel : MonoBehaviour
{
    [Header("Set in Inspector")] 
    public Dray dray;
    public Sprite healthEmpty;
    public Sprite healthHalf;
    public Sprite healthFull;

    private Text keyCountText;
    private List<Image> healthImages;

    private void Start()
    {
        Transform tempTrans = transform.Find("Key Count");
        keyCountText = tempTrans.GetComponent<Text>();

        Transform healthPanel = transform.Find("Health Panel");
        healthImages = new List<Image>();
        if (healthPanel != null)
        {
            for (int i = 0; i < 20; i++)
            {
                tempTrans = healthPanel.Find("H_" + i);
                if(tempTrans == null) break;
                healthImages.Add(tempTrans.GetComponent<Image>());
            }
        }
    }

    private void Update()
    {
        keyCountText.text = dray.numKeys.ToString();

        int health = dray.health;
        for (int i = 0; i < healthImages.Count; i++)
        {
            if (health > 1)
            {
                healthImages[i].sprite = healthFull;
            }
            else if (health == 1)
            {
                healthImages[i].sprite = healthHalf;
            }
            else
            {
                healthImages[i].sprite = healthEmpty;
            }

            health -= 2;
        }
    }
}
