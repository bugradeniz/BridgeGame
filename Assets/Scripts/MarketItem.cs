using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MarketItem : MonoBehaviour
{   
    [Header("ITEM BILGILERI")]
    public int itemId, wearId;
    public int price;

    [Header("BUTONLAR VE TEXTLER")]

    public Button buyButton, equipButton, unequipButton;
    public Text priceText;

    [Header("FIZIKI ESYA")]
    public GameObject itemPrefab; 

    //playerpref bilgileri:
    //- 0 : henuz satin alinmamis. (default)
    //- 1 : satin alinmis fakat giyilmemis
    //- 2 : satin alinmis ve giyilmis


    // bu market esyasinin satin alinip alinmadigini id bilgisiyle player preften ogreniliyor
    public bool hasItem()
    {
        bool hasItem = PlayerPrefs.GetInt("item" + itemId.ToString()) != 0;

        return hasItem;
    }

    // bu market esyasinin giyili olup olmadigi id bilgisiyle player preften ogreniliyor
    public bool isEquipped()
    {
        bool isEquipped = PlayerPrefs.GetInt("item" + itemId.ToString()) == 2;
        return isEquipped;
    }

    // bu kurulumda butonlar ayarlaniyor  ve varsa giyili esyanin giyilmesi saglaniyor.
    
    public void initializeItem()
    {
        priceText.text = price.ToString();
        if (hasItem())
        {

            buyButton.gameObject.SetActive(false);
            if (isEquipped())
            {
                equipItem();                                //satin alinmis ve giyilmis ise giyme fonksiyonu calisiyor. Bu giyme fonksiyonunda zaten cikartma butonu aktif ediliyor !
            }
            else
            {
                equipButton.gameObject.SetActive(true);     // satin alinmis fakat giyilmemisse giyme butonu aktif
            }
        }
        else
        {
            buyButton.gameObject.SetActive(true);       // satin alinmamissa (default) buy button aktif.
                                                 
        }
    }

    // satin alma fonksiyonu
    public void buyItem()
    {
        if (!hasItem())     // satin alinmis bir esyanin zaten buy butonu olmaz ama ne olur ne olmaz
        {
            int money = PlayerPrefs.GetInt("money");            // paramiz pp den cekiliyor.

            if (money >= price)         // paramiz yetiyorsa satin alma gerceklesiyor.
            {
                PlayerControler.Current.itemAudioSource.PlayOneShot(PlayerControler.Current.buyAudioClip, 0.1f); // satin alma sesi

                LevelController.Current.giveMoneyToPlayer(-price);  // ucret kadar para azaltiliyor.
                PlayerPrefs.SetInt("item" + itemId.ToString(), 1);  // pp guncelleniyor.
                buyButton.gameObject.SetActive(false);              // satin alindiktan sonra satin alinma butonu kapatilip
                equipButton.gameObject.SetActive(true);             // giyme butonu aktif ediliyor.

            }
        }
    }

    // giyme fonksiyonu 
    public void equipItem()
    {
        unequipItem();          // once hali hazirda giyilen esya cikartiliyor.
        MarketController.current.equippedItems[wearId] = Instantiate(itemPrefab, PlayerControler.Current.wearSpots[wearId].transform).GetComponent<Item>(); //tutulan fiziksel obje, denk gelen esya turundeki giyim noktasinda olusturuluyor. giyili objeler listesinde yine ayni denk gelen esya turundeki indexinde tutuluyor. indexler MarketController da aciklaniyor.
        MarketController.current.equippedItems[wearId].itemId = itemId;     // cikartirken  hangi esyayi cikarttigimizi anlamak icin id bilgisi fiziksel obje icinde tutuluyor.
        equipButton.gameObject.SetActive(false);    // giyme butonu kapatilip
        unequipButton.gameObject.SetActive(true);   // cikartma butonu aktif ediliyor
        PlayerPrefs.SetInt("item" + itemId.ToString(), 2);  // pp guncelleniyor.
        PlayerControler.Current.itemAudioSource.PlayOneShot(PlayerControler.Current.unequipItemAudipClip, 0.1f);    //SES

    }
    // cikarma fonksiyonu
    public void unequipItem()
    {
        Item equippedItem = MarketController.current.equippedItems[wearId]; // listeden denk gelen esya turundeki giyili esya aliniyor
        if (equippedItem != null)   // hali hazirda giyili bir esya varsa cikartiliyor. (ilk giyme fonksiyonunda da cagirildigi icin onemli)
        {
            MarketItem marketItem = MarketController.current.items[equippedItem.itemId];    //giyilirken eklenen id kullanilarak market controllerdaki tum market esyalar listesinden esyanin market esya objesine erisiliyor.
            PlayerPrefs.SetInt("item" + marketItem.itemId, 1);      //bu esya pp den cikartilmis olarak guncelleniyor.
            marketItem.unequipButton.gameObject.SetActive(false);   // cikartma butonu kapatilip
            marketItem.equipButton.gameObject.SetActive(true);      // giyme butonu aktif ediliyor.
            Destroy(equippedItem.gameObject);                       // fiziki obje yok ediliyor.
            PlayerControler.Current.itemAudioSource.PlayOneShot(PlayerControler.Current.unequipItemAudipClip, 0.1f);    //SES



        }
    }
   
}