using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeSpawner : MonoBehaviour
{
    // BU SCRIPT IKI NOKTA ARASINDA BIR BOX KOLIDER OLUSTURUYOR.

    public GameObject startReferance, endReferance; //hesaplama icin gerekli iki referans noktasi.
    public BoxCollider hiddenPlatform;  // boyutu ve pozisyonu hesaplanacak olan box colider objesi.
    void Start()
    {

        Vector3 direction = endReferance.transform.position - startReferance.transform.position; // iki nokta arasindaki mesafe ve yon bilgilerini almak icin once bitis noktasindan baslangic noktasini cikartiyoruz.
        float distance = direction.magnitude;   // magnitude ile mesafeyi aliyoruz.
        direction = direction.normalized;       // normalized ile birim yon vektorunu aliyoruz.
        hiddenPlatform.transform.forward = direction;   // objenin yonunu elde ettigimiz birim yon vektoru olarak atiyoruz.
        hiddenPlatform.size = new Vector3(hiddenPlatform.size.x, hiddenPlatform.size.y, distance);  // x ve y boyutlari sabit iken z boyutunu elde ettigimiz mesafe olarak atiyoruz.

        // pozisyonu olarak :     baslangic noktasindan mesafenin yarisi kadar yon boyunca uzakliga getir. Ayrica ortalanmis colideri koselere denk gelmesi icin y eksninde y boyutunun yarisi * cos(A) kadar azalt, z ekseninde ise y boyutunun yarisi * sin(A) kadar arttir. (direction z = cos(A),direction y =sin(A))
        hiddenPlatform.transform.position = startReferance.transform.position + (direction * distance / 2) + (new Vector3(0, -direction.z, direction.y) * hiddenPlatform.size.y / 2);

    }


}
