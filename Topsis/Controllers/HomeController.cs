using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Topsis.Models;

namespace Topsis.Controllers
{
    public class HomeController : Controller
    {
        int satir = 4; int sutun = 5;
        double[,] kararMatrisi = new Double[4, 5];
        double[,] standartKararMatrisi = new Double[4, 5];
        double[] agırlıkMatrisi = new Double[5];
        double[,] agirlikliStandartKararMatrisi = new Double[4, 5];
        double[] idealAyrim = new Double[5];
        double[] negatifIdealAyrim = new Double[5];
        double[] sonuc = new Double[4];
        double[] sonucSirali = new Double[4];
        String[] sonucIsim = new string[4];
        SortedList sortedList1 = new SortedList();


        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(topsisModel t)
        {
            //Karar Matrisinin (A) Oluşturulması 
            double[] agırlıkMatrisi = new Double[5];
            agırlıkMatrisi[0]= Double.Parse(t.w1);
            agırlıkMatrisi[1]= Double.Parse(t.w2);
            agırlıkMatrisi[2]= Double.Parse(t.w3);
            agırlıkMatrisi[3]= Double.Parse(t.w4);
            agırlıkMatrisi[4]= Double.Parse(t.w5);
            
            kararMatrisi[0, 0] = Double.Parse(t.a1);
            kararMatrisi[0, 1] = Double.Parse(t.a2);
            kararMatrisi[0, 2] = Double.Parse(t.a3);
            kararMatrisi[0, 3] = Double.Parse(t.a4);
            kararMatrisi[0, 4] = Double.Parse(t.a5);
            kararMatrisi[1, 0] = Double.Parse(t.a6);
            kararMatrisi[1, 1] = Double.Parse(t.a7);
            kararMatrisi[1, 2] = Double.Parse(t.a8);
            kararMatrisi[1, 3] = Double.Parse(t.a9);
            kararMatrisi[1, 4] = Double.Parse(t.a10);
            kararMatrisi[2, 0] = Double.Parse(t.a11);
            kararMatrisi[2, 1] = Double.Parse(t.a12);
            kararMatrisi[2, 2] = Double.Parse(t.a13);
            kararMatrisi[2, 3] = Double.Parse(t.a14);
            kararMatrisi[2, 4] = Double.Parse(t.a15);
            kararMatrisi[3, 0] = Double.Parse(t.a16);
            kararMatrisi[3, 1] = Double.Parse(t.a17);
            kararMatrisi[3, 2] = Double.Parse(t.a18);
            kararMatrisi[3, 3] = Double.Parse(t.a19);
            kararMatrisi[3, 4] = Double.Parse(t.a20);

            hesapla(agırlıkMatrisi);
            TempData["karaMatrisi"] = kararMatrisi;
            TempData["sKararMatrisi"] = standartKararMatrisi;
            TempData["aStandartKararMatrisi"] = agirlikliStandartKararMatrisi;
            TempData["iAyrım"] = idealAyrim;
            TempData["negatifIdealAyrim"] = negatifIdealAyrim;
            TempData["sonuc"] = sonuc;
            TempData["sonucSirali"] = sonucSirali;
            TempData["sonuclar"] = sonucIsim;

            return View();
        }


        public void hesapla(Double[] agırlıkMatrisi1)
        {

            //Standart Karar Matrisinin olusmasi
            double toplam = 0;
            for (int i = 0; i < 5; i++)
            {
                toplam = 0;
                for (int j = 0; j < 4; j++)
                {

                    toplam = toplam + Math.Pow(kararMatrisi[j, i], 2);
                }
                toplam = Math.Sqrt(toplam);
                for (int j = 0; j < 4; j++)
                {

                    standartKararMatrisi[j, i] = kararMatrisi[j, i] / toplam;
                }
            }

            //Ağırlıklı Standart Karar Matrisinin (V) Oluşturulması

            for (int i = 0; i < 5; i++)
            {
                agırlıkMatrisi[i] = agırlıkMatrisi1[i];
            }
            for (int i = 0; i < satir; i++)
            {
                for (int j = 0; j < sutun; j++)
                {

                    agirlikliStandartKararMatrisi[i, j] = standartKararMatrisi[i, j] * agırlıkMatrisi[j];
                }
            }


            //İdeal ( A*) ve Negatif İdeal (a- ) Çözümlerin Oluşturulması

            double max = 0;
            double min = 0;
            double[] enBuyukMatrisi = new Double[sutun];
            double[] enKucukMatrisi = new Double[sutun];
            for (int j = 0; j < sutun; j++)
            {
                for (int i = 0; i < satir - 1; i++)
                {
                    if (agirlikliStandartKararMatrisi[i, j] <= agirlikliStandartKararMatrisi[i + 1, j])
                    {
                        max = agirlikliStandartKararMatrisi[i + 1, j];
                        min = agirlikliStandartKararMatrisi[i, j];
                    }
                    else
                    {
                        max = agirlikliStandartKararMatrisi[i, j];
                        min = agirlikliStandartKararMatrisi[i + 1, j];
                    }

                }
                enBuyukMatrisi[j] = max;
                enKucukMatrisi[j] = min;
            }

            //Ayırım Ölçülerinin Hesaplanması

            double toplamPozitif = 0;
            double toplamNegatif = 0;
            for (int i = 0; i < satir; i++)
            {
                for (int j = 0; j < sutun; j++)
                {
                    toplamNegatif = toplamNegatif + Math.Pow((agirlikliStandartKararMatrisi[i, j] - enKucukMatrisi[j]), 2);
                    toplamPozitif = toplamPozitif + Math.Pow((agirlikliStandartKararMatrisi[i, j] - enBuyukMatrisi[j]), 2);

                }
                idealAyrim[i] = Math.Sqrt(toplamPozitif);
                negatifIdealAyrim[i] = Math.Sqrt(toplamNegatif);
            }

            //İdeal Çözüme Göreli Yakınlığın Hesaplanması

            for (int i = 0; i < satir; i++)
            {
                sonuc[i] = negatifIdealAyrim[i] / (idealAyrim[i] + negatifIdealAyrim[i]);
            }
            //sırala
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {

                }

            }

            
            sortedList1.Add(sonuc[0], "Bilge Kaan Mahallesi");
            sortedList1.Add(sonuc[1], "Fatih Sultan Mehmet Han Mahalles");
            sortedList1.Add(sonuc[2], "Cengizhan Mahallesi");
            sortedList1.Add(sonuc[3], "Gümüshaneliler Mahallesi");

            for (int i = 0; i < sortedList1.Count; i++)
            {
                sonucSirali[i] =(double) sortedList1.GetKey(i);
                sonucIsim[i] = (String)sortedList1.GetByIndex(i);
            }

        }
    }
}