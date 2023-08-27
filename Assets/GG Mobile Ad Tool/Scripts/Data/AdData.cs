using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ImageMetaData
{
    public int sizeX;
    public int sizeY;
    public ImageMetaData(int sizeX, int sizeY)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
    }
}

[System.Serializable]
public class AdData
{
    public string headLine;
    public int rating;
    public string desc;
    public int price;
    public Color themeColor;
    public byte[] adImage;
    public ImageMetaData metaData;
    public AdData(string headLine, int rating, string desc, int price, Color themeColor, Texture2D adImage)
    {
        metaData = new ImageMetaData(adImage.width, adImage.height);
        this.adImage = adImage.EncodeToPNG();
        this.headLine = headLine;
        this.rating = rating;
        this.desc = desc;
        this.price = price;
        this.themeColor = themeColor;
    }
}
