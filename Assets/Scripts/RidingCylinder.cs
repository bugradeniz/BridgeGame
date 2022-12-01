using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidingCylinder : MonoBehaviour
{
    private bool _filled;
    private float _value;

    /// <summary>
    /// silindirin kendi boyutunu degistirdigi fonksiyon.
    /// Konum ve boyut ayarlari :
    /// y pozisyonu: (silindir sayisi-1) * -0.5 + boyut degeri* -0.25
    /// x ve z boyutu : (boyut degeri)* 0.5
    /// </summary>
    /// <param name="value">arttirilmak veya azaltilmak istenen miktar</param>
    public void incrementCylinderVolume(float value)
    {
        _value += value;
        if (_value > 1)             // eger deger 1 den buyuk olmus ise maximum boyuta, yani 1 degerine sabitleniyor
        {
            float leftValue = _value - 1;
            int cylinderCount = PlayerControler.Current.cylinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylinderCount - 1) - 0.25f, transform.localPosition.z);
            transform.localScale = new Vector3(0.5f, transform.localScale.y, 0.5f);

            
            PlayerControler.Current.createCylinder(leftValue);              // kalan miktar ile de yeni bir silindir olusturuluyor.
            
        }
        else if (_value < 0)
        {
            PlayerControler.Current.destroyCylinder(this);  // boyut degeri birden kucuk hale geldi ise silindir yok ediliyor.

            
        }
        else        //ara degerlerde vermis oldugumuz hesaplamalar ile silindirin boyutu ve konumu  guncelleniyor.
        {
            int cylinderCount = PlayerControler.Current.cylinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylinderCount - 1) - 0.25f * _value, transform.localPosition.z);
            transform.localScale = new Vector3(0.5f * _value, transform.localScale.y, 0.5f * _value);

            
        }
    }
}
