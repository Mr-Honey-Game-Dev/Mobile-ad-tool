using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI headingTxt;   
    [SerializeField] TextMeshProUGUI priceTxt;   
    [SerializeField] TextMeshProUGUI descTxt;   
    [SerializeField] Image rateImage;   
    [SerializeField] Image bgImage;
    [SerializeField] Image ctaButton;
    [SerializeField] RawImage ProductLogo;
    AdData adData;
    public void Bind(AdData adData) 
    {
        this.adData = adData;
        Show();
    }
    void Show() 
    {
        headingTxt.text = adData.headLine;
        priceTxt.text = adData.price==0?"FREE":adData.price.ToString();
        descTxt.text = adData.desc;
        rateImage.fillAmount = adData.rating * 0.2f;
        bgImage.color = adData.themeColor;
        ctaButton.color = adData.themeColor;
        Texture2D tex= new Texture2D(24, 24);
        tex.LoadImage(adData.adImage);
        ProductLogo.texture = tex;
    }
}
