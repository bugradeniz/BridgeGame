using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketController : MonoBehaviour
{
    public static MarketController current;

    [Header("ESYALAR LISTESI")]
    public List<MarketItem> items;

    public List<Item> equippedItems;//indexler:
                                    //  0- kafa
                                    //  1- yuz
                                    //  ...

    public GameObject marketMenu;


    // MARKET ESYA BUTONLARINI AYARLAYAN GIYILI ESYALARI GIYEN FONKSIYON
    public void InitializeMarketController(){
        current=this;
        foreach (MarketItem item in items)  // listedeki her bir market esyanin kendi kurulum fonksiyonu cagiriliyor.
        {
            item.initializeItem();
        }
    }

    //MARKETI ACIP KAPATAN buton fonksiyonu
    public void activateMarketMenu(bool activate){
        marketMenu.SetActive(activate);
    }
}
