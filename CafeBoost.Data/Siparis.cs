﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CafeBoost.Data
{
    
    public class Siparis
    {
        public List<SiparisDetay> SiparisDetaylar { get; set; }
        public int MasaNo { get; set; }
        public DateTime? AcilisZamani { get; set; }
        public DateTime? KapanisZamani { get; set; }
        public SiparisDurum Durum { get; set; }
        //public string ToplamTutarTL { get { return ToplamTutar() + "TL"; } }

        public Siparis()
        {
            SiparisDetaylar = new List<SiparisDetay>();
            AcilisZamani = DateTime.Now;
        }
        public decimal ToplamTutar() // public decimal ToplamTutar() => SiparisDetaylar.Sum(x => x.Tutar()); returnsüz hali ama aynılar
        {
            return SiparisDetaylar.Sum(x => x.Tutar());
        }
    }
}