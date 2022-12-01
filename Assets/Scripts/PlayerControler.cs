using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    public static PlayerControler Current; //Static instance    

    [Header("Karakter Verileri")]
    public float runningSpeed;
    public float xSpeed;
    private float _currentRunningSpeed;
    public float limitX;

    [Header("Prefablar")]

    public GameObject bridgePiecePrefab;
    public GameObject RidingCylinderPrefab;
    public List<RidingCylinder> cylinders;


    private bool _spawningBridge;
    private BridgeSpawner _bridgeSpawner;
    private float _creatingBridgeTimer;
    private bool _finished;
    public Animator animator;
    private float _lastTouchedX;
    private float _scoreTimer = 0;
    volatile float _dropSoundTimer;


    public AudioSource cylinderAudioSource, triggerAudioSource, itemAudioSource;
    public AudioClip gatherAudioClip, dropAudioClip, coinAudioClip, buyAudioClip, equipItemAudioClip, unequipItemAudipClip;

    public List<GameObject> wearSpots;

    void Start()
    {
        Current = this;


    }

    // Update is called once per frame
    void Update()

    {

        if (LevelController.Current == null || !LevelController.Current.gameActive)         // LevelController sinifindaki oyun aktiflik bilgisine gore update fonksiyonu durduruluyor
        {
            return;
        }


        // karakterin saga ve sola gitmesini saglayan kodlar
        float newX = 0;
        float touchXDelta = 0;
        if (Input.touchCount > 0)   // telefonda dokunma oluyorsa burada hesaplama yapiliyor
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                _lastTouchedX = Input.GetTouch(0).position.x;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                touchXDelta = 5 * (Input.GetTouch(0).position.x - _lastTouchedX) / Screen.width;
                _lastTouchedX = Input.GetTouch(0).position.x;
            }


        }
        else if (Input.GetMouseButton(0))   // bilgisayarda tiklama yapiliyorsa burada hesaplaniyor
        {
            touchXDelta = Input.GetAxis("Mouse X");

        }


        newX = transform.position.x + xSpeed * touchXDelta * Time.deltaTime;    // bilgisayar veya telefondan cekilen X degisimine gore yeni x pozisyonu olusturuluyor.
        newX = Mathf.Clamp(newX, -limitX, limitX);  // platform sinirlarina gore sinirlar ciziliyor.

        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z + _currentRunningSpeed * Time.deltaTime);    // yeni z pozisyonu ve ilerlemek icin hesaplanan z pozisyonu ile yeni pozisyon vektoru olusturuluyor
        transform.position = newPosition;   // bu pozisyon vektoru karakterin pozisyonu olarak ataniyor.


        if (_spawningBridge)        // eger kopru olusturma kilidi acildiysa buraya giriyor
        {
            playDropSound();    // silindir azalirken cikartilan ses burada da kullaniliyor.
            _creatingBridgeTimer -= Time.deltaTime;             // her karede calismamasi icin bir zamanlayici
            if (_creatingBridgeTimer < 0)
            {
                _creatingBridgeTimer = 0.01f;                   // saniyenin her % x inde 1 kopru olusturuyor
                incrementCylinderVolume(-0.01f);                // once silindir azaltimi yapiliyor
                GameObject createdBridgePiece = Instantiate(bridgePiecePrefab, this.transform);         // olusturulmus prefabdan koopru objesi olusturuluyor
                createdBridgePiece.transform.SetParent(null);                                           // karakterimizden bagimsiz globalde tutuluyor.

                Vector3 direction = _bridgeSpawner.endReferance.transform.position - _bridgeSpawner.startReferance.transform.position;  // temas ile cektigimiz spawner objesinden baslangic ve bitis noktalari ile bir cizgi cekiyoruz.
                float distance = direction.magnitude;       // bu cizginin uzunlugu
                direction = direction.normalized;           // bu cizginin yon vektoru
                createdBridgePiece.transform.forward = direction;   // olusturulan kopru parcasinin z eksenini (onunu) hasapladigimiz yon vektorune cekiyoruz.
                float characterDistance = transform.position.z - _bridgeSpawner.startReferance.transform.position.z;    //karakterin cizgi boyunca hangi konumda odugunu baslangic noktasinin z eksenini cikartarak buluyoruz 
                characterDistance = Mathf.Clamp(characterDistance, 0, distance);        // bu deger 0 ile koprunun uzunlugu arasinda olmali (zaten oyle)
                Vector3 newPiecePosition = _bridgeSpawner.startReferance.transform.position + direction * characterDistance;    //yeni pozisyonu bu cizgi uzerinde karakterin ilerledigi mesafe kadar baslangic noktasina ekleyerek buluyoruz
                newPiecePosition.x = transform.position.x;      // x eksenindeki pozisyonunu karakterinkine esitliyoruz ki saga ve sola hareket edince altinda olussun.
                createdBridgePiece.transform.position = newPiecePosition;   // hesapladigimiz pozisyonu kopru parcasina atiyoruz.

                if (_finished)  // eger biti cizgisine gelindiyse 0.3 saniyede bir skor 1  arttiriliyor.
                {
                    _scoreTimer -= Time.deltaTime;
                    if (_scoreTimer < 0)
                    {
                        _scoreTimer = 0.3f;
                        LevelController.Current.changeScore(1);
                    }
                }
            }
        }
    }
    public void ChangeSpeed(float value)
    {
        _currentRunningSpeed = value;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AddCylinder")    //Toplanabilir silindir parcalari temas bolgesi
        {
            cylinderAudioSource.PlayOneShot(gatherAudioClip, 0.1f);
            incrementCylinderVolume(0.1f);  // silindir buyutme fonksiyonu cagiriliyor!
            Destroy(other.gameObject);      // toplanan parca yok ediliyor.
        }
        else if (other.tag == "SpawnBridge")  // eger kopru girisine gelindiyse baslangic ve bitis noktalarini elde etmek icin o kopru objesini aliyoruz. ve kopru olusturmayi baslatiyoruz.
        {
            startSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
        else if (other.tag == "StopSpawnBridge") // eger kopru sonuna gelindiyse kopru olusturma durduruluyor.
        {

            stopSpawningBridge();
            if (_finished) // eger oyun sonundaki spawner sonuna gelindiyse oyun bitis fonksiyonu devreye giriyor.( yuksek ihtilmalle boyle bir sey olmayacak... )
            {
                LevelController.Current.finishGame();
            }
        }
        else if (other.tag == "Finish")     // oyun sonuna gelindiginde bitis kilidi aciliyor ve son kopru olusturulmaya basliyor.
        {

            _finished = true;
            startSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
        else if (other.tag == "Coin")   // elmas toplandiginda skor arttiriliyor.
        {
            other.tag = "Untagged";     //2 defa girilmemesi icin

            triggerAudioSource.PlayOneShot(coinAudioClip, 0.1f);         
            LevelController.Current.changeScore(10);
            Destroy(other.gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (LevelController.Current.gameActive) // burada tuzak uzerinde olmusken ses cikartmaya ve silindir azaltmaya devam etmemesi icin oyun aktifligi sorgulaniyor.
        {
            if (other.tag == "Trap") // tuzaklara girdi sure boyunca calisan fonksiyon
            {
                playDropSound();
                incrementCylinderVolume(-Time.fixedDeltaTime*1f);  // silindir boyutu saniyede 1f kadar azaliyor. 
            }
        }
    }



    /// <summary>
    /// 
    /// Bu fonksiyon karakterin altina silindir eklemek icin kullanilir.
    /// Azaltmak icin (-) deger girilmesi yeterli.
    /// Silindir yokken eksi deger alirsa olum gerceklesir.(Oyun sonu haric)
    /// 
    /// </summary>
    /// <param name="value">
    /// arttirilmak veya azaltilmak istenen miktar
    /// </param>

    private void incrementCylinderVolume(float value)
    {

        if (cylinders.Count == 0)
        {
            if (value > 0)
            {
                createCylinder(value); // ilk arttirmada yeni bir silindir olusturuluyor.
            }
            else
            {
                if (_finished)
                {
                    LevelController.Current.finishGame();  // oyun sonuna gelinmis ve kopru olusturuluyorsa, silindirler bitince oyun bitis fonksiyonu calisir ve bolum gecilir.
                }
                else
                {
                    die();      // silindir yoksa ve azaltilmaya calisiliyorsa ve oyun sonunda degilse olum emri veriliyor.
                }
            }
        }
        else
        {
            cylinders[cylinders.Count - 1].incrementCylinderVolume(value);  //mevcut silindir varsa listedeki son silindirin silindir arttirma fonksiyonu calisiyor.
        }
    }
    /// <summary>
    /// Bu fonksiyon yeni bir silindir olusturulacagi zaman cagiriliyor.
    /// </summary>
    /// <param name="value">Olusturalacak yeni silindirin boyut degeri</param>
    public void createCylinder(float value)
    {
        RidingCylinder createdCylinder = Instantiate(RidingCylinderPrefab, transform).GetComponent<RidingCylinder>();
        cylinders.Add(createdCylinder);     //silindir listede tutuluyor
        createdCylinder.incrementCylinderVolume(value);         // yeni silindirin boyutunu kendi icindeki boyut arttirma fonksiyonu ayarliyor.
    }
    public void die()       // olunce calisan fonksiyon
    {
        LevelController.Current.GameOver(); // oyun sonu menusu ayarlamalari vs. icin fonksiyon cagiriliyor.
        gameObject.layer = 7;               // koprulerde yariyolda kalirsa dusmesi icin gorunmez colliderlar ile etkilesime girmeyen bir layer atamasi yapiliyor.
        Camera.main.transform.SetParent(null);  // kamera da karakter ile birlikte dusmemesi icin parentini null yapiyoruz
        animator.SetBool("dead", true);     // olum animasyonu calistiriliyor.


    }

    /// <summary>
    /// Bir silindir yok edilmek istendiginde cagrilan fonksiyon
    /// </summary>
    /// <param name="cylinder">Yok edilmek istenen obje parametre olarak aliniyor</param>
    public void destroyCylinder(RidingCylinder cylinder)
    {
        cylinders.Remove(cylinder);     //silindir listesinden cikartiliyor.
        Destroy(cylinder.gameObject);   // sonra yok ediliyor.
    }

    public void startSpawningBridge(BridgeSpawner spawner) // kopru olusturmaya baslandiginda girilen fonksiyon.
    {
        _bridgeSpawner = spawner;   // alinan kopru objesini baslangic ve bitis degerlerine erismek icin tutuyoruz.
        _spawningBridge = true;     // kopru olusturma kilidi aciliyor.
    }
    public void stopSpawningBridge() // kopru olusturma bittiginde girilen fonksiyon.
    {

        _spawningBridge = false;    // kopru olusturma kilidi kapaniyor.
    }

    // tuzakta kaldigimiz surece belirli araliklarla ses cikaran fonksiyon
    public void playDropSound()     
    {
        _dropSoundTimer -= Time.deltaTime;
        if (_dropSoundTimer < 0)
        {
            _dropSoundTimer = 0.15f;
            cylinderAudioSource.PlayOneShot(dropAudioClip, 0.1f);  

        }
    }
}
