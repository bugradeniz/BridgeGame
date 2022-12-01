using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelController : MonoBehaviour
{
    public static LevelController Current;
    public bool gameActive = false;

    [Header("UI ELEMANLARI")]
    public GameObject startMenu, gameMenu, gameOverMenu, finishMenu;
    public Button rewardedAdButton;
    public Text scoreText, finishScoreText, currentLevelText, nextLevelText, startingMenuMoneyText, gameOverMenuMoneyText, finishGameMenuMoneyText;
    public Slider levelProgressBar;
    public DailyReward dailyReward;
    
    
    [Header("LEVEL BILGILERI")]
    int currentLevel;
    public int score;
    public float maxDistance;
    public GameObject finishLine;


    [Header("SES KAYNAKLARI")]
    public AudioSource gameMusicAudioSource;
    public AudioClip victoryAudioClip, gameOverAudioClip;


    void Start()
    {
        Current = this;
        currentLevel = PlayerPrefs.GetInt("currentLevel");                          // Level bilgisi player prefte currentLevel olarak aliniyor ve tutuluyor

        PlayerControler.Current = GameObject.FindObjectOfType<PlayerControler>();   // karakter static objesi gecikme olmamasi icin burada kuruluyor.


        //burada oyun sahnesindeki her bir tex objesinin icerisindeki initTextObject fonksiyonu cagirilarak baslangicta dil guncellemesi yapiliyor.
        GameObject[] parentsInScene = this.gameObject.scene.GetRootGameObjects();
        foreach (GameObject parent in parentsInScene)
        {
            TextObject[] textObjectsInParent= parent.GetComponentsInChildren<TextObject>(true);                                 
            foreach (TextObject textObject in textObjectsInParent)
            {
                textObject.InitTextObject();
            }
        }


        GameObject.FindObjectOfType<MarketController>().InitializeMarketController();
        dailyReward.initializeDailyReward();

        // Slider Level sayilari guncelleniyor.
        currentLevelText.text = (currentLevel + 1).ToString();
        nextLevelText.text = (currentLevel + 2).ToString();

        updateMoneyText();      // para textlerini guncelleniyor
        
        gameMusicAudioSource = Camera.main.GetComponent<AudioSource>();     // kameranin audio source bilesenini aliniyor.


        //giveMoneyToPlayer(3000);

    }

    void Update()
    {
        if (gameActive)
        {
            float distance = finishLine.transform.position.z - PlayerControler.Current.transform.position.z;    // bitise kadar mesafe hesaplanip slider da gosteriliyor.
            levelProgressBar.value = 1 - (distance / maxDistance);
        }
    }
    public void showRewardedAd(){
       
    }
    public void StartLevel()    // bolumu baslatma butonu ile calisan fonksiyon.
    {
        maxDistance = finishLine.transform.position.z - PlayerControler.Current.transform.position.z; // baslangic noktasindan sona kadar maximum mesafe aliniyor

        PlayerControler.Current.ChangeSpeed(PlayerControler.Current.runningSpeed);  // karakterin hizi ayarlaniyor.
        startMenu.SetActive(false);     // baslangic ui kapatiliyor
        gameMenu.SetActive(true);       // oyun ici ui aciliyor.
        PlayerControler.Current.animator.SetBool("running", true);  // karakterin kosma animasyonu baslatiliyor.


        gameActive = true;              // oyun aktif ediliyor.Karakterin update metodu devreye giriyor.
    }
    public void restartLevel()          // oldukten sonra tekrar baslama butonu ile calisan fonksiyon
    {
        LevelLoader.Current.changeLevel(this.gameObject.scene.name); // aktif sahne tekrar yukleniyor.
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void loadNextLevel()         // oyun kazanildiktan sonra siradaki level butonu ile calisan fonksiyon
    {
        LevelLoader.Current.changeLevel("Level " + (currentLevel + 1)); // bir sonraki level yukleniyor.


    }
    public void GameOver() // olunce calisan fonksiyon
    {   
        
        updateMoneyText();
        gameMusicAudioSource.Stop();
        gameMusicAudioSource.PlayOneShot(gameOverAudioClip);
        gameMenu.SetActive(false);
        gameOverMenu.SetActive(true);
        gameActive = false;
    }
    public void finishGame()    // bolumu gecince calisan fonksiyon
    {   
        
        giveMoneyToPlayer(score);
        gameMusicAudioSource.Stop();
        gameMusicAudioSource.PlayOneShot(victoryAudioClip);

        PlayerPrefs.SetInt("currentLevel", currentLevel + 1);       // player prefte tutulan mevcut level 1 arttiriliyor.
        finishScoreText.text = scoreText.text.ToString();
        gameMenu.SetActive(false);
        finishMenu.SetActive(true);
        gameActive = false;

    }
    public void changeScore(int increment)  // skoru guncelleyen fonksiyon
    {
        score += increment;
        scoreText.text = score.ToString();
    }
    public void updateMoneyText()   // para textlerini guncelleyn fonksiyon
    {

        int money = PlayerPrefs.GetInt("money");
        startingMenuMoneyText.text = money.ToString();
        gameOverMenuMoneyText.text = money.ToString();
        finishGameMenuMoneyText.text = money.ToString();
    }
    public void giveMoneyToPlayer(int increment) // mevcut parayi guncelleyen fonksiyon
    {

        int money = PlayerPrefs.GetInt("money");
        money = Mathf.Max(0, money + increment);
        PlayerPrefs.SetInt("money", money);
        updateMoneyText();

    }
}
