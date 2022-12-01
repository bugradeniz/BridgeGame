using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{

    public bool initialized;
    public long rewardGivingTimeTicks;
    long oneDay = 864000000000;
    public GameObject rewardMenu;
    public Text remainingTimeText;
    public void initializeDailyReward()
    {
        if (PlayerPrefs.HasKey("lastDailyReward"))
        {
            rewardGivingTimeTicks = long.Parse(PlayerPrefs.GetString("lastDailyReward")) + oneDay;  //bir sonraki odul tarihini tutulan tarihe bir gun ekleyerek hesapliyor.
            long currentTime = System.DateTime.Now.Ticks;   // mevcut tarih aliniyor
            if (currentTime >= rewardGivingTimeTicks)       // bu tarih odul alma tarihine esit veya buyukse odul veriliyor.
            {
                giveReward();
            }

        }
        else
        {
            giveReward();   // odul alma tarihi kaydi yoksa yani daha once odul almamissa odul veriliyor. odul verme fonksiyonunda bu kayit zaten olusturuluyor.
        }
        initialized = true;     // odul sistemi kurulum kilidi aciliyor.

    }
    public void giveReward()    
    {
        LevelController.Current.giveMoneyToPlayer(100);// oyuncuya 100 elmas veriyor 
        rewardMenu.SetActive(true);     //odul verildi ekrani aktif ediliyor
        PlayerPrefs.SetString("lastDailyReward", System.DateTime.Now.Ticks.ToString()); //mevcut tarihi player pref de sakliyor
        rewardGivingTimeTicks = long.Parse(PlayerPrefs.GetString("lastDailyReward")) + oneDay;  //bir sonraki odul tarihini tutulan tarihe bir gun ekleyerek hesapliyor.
    }
    // Update is called once per frame
    void Update()
    {
        if (initialized)        // kurulum yapilmissa
        {
            if (LevelController.Current.startMenu.activeInHierarchy)    // ve start menu aktif ise
            {
                long currentTime = System.DateTime.Now.Ticks;
                long remainingTime = rewardGivingTimeTicks - currentTime;   // odul alinacak tarihten mevcut tarih cikartiliyor.
                if (remainingTime <= 0)         // bu fark 0 veya kucukse 
                {
                    giveReward();   // odul veriliyor.
                }
                else
                {   //henuz zamani gelmediyse  kalan sure start menudaki kalan zaman textinde gosteriliyor.

                    System.TimeSpan timeSpan = System.TimeSpan.FromTicks(remainingTime);
                    remainingTimeText.text = string.Format("{0}:{1}:{2}", timeSpan.Hours.ToString("D2"), timeSpan.Minutes.ToString("D2"), timeSpan.Seconds.ToString("D2"));


                }
            }
        }
    }
    public void tapToTurnButton()   // odul alindi menusunu kapatan buton.
    {
        rewardMenu.SetActive(false);
    }
}
