using CafeBoost.Data;
using CyberBoost.UI.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyberBoost.UI
{
    public partial class AnaForm : Form
    {
        int masaAdedi = 20;
        KafeVeri db = new KafeVeri();
        public AnaForm()
        {
            InitializeComponent();
            VeriOku();
            //OrnekUrunleriYukle();
            MasalariOlustur();
        }

        private void VeriOku()
        {
            try
            {
                string json = File.ReadAllText("veri.json");
                db = JsonConvert.DeserializeObject<KafeVeri>(json);
            }
            catch (Exception)
            {
            }
        }

        private void OrnekUrunleriYukle()
        {
            db.Urunler.Add(new Urun
            {
                UrunAd = "Kola",
                BirimFiyat = 6m //decimal, 6 yazsan da olur cast etmek gerekir.
            });
            db.Urunler.Add(new Urun
            {
                UrunAd = "Ayran",
                BirimFiyat = 4m
            });
        }

        private void MasalariOlustur()
        {
            #region Imaj Listesinin Hazırlanması
            ImageList ilist = new ImageList();
            ilist.Images.Add("bos", Resources.bos); //"" li yere nasıl kaydettiysen onu yaz
            ilist.Images.Add("dolu", Resources.dolu);
            ilist.ImageSize = new Size(64, 64);
            lvwMasalar.LargeImageList = ilist;

            #endregion
            #region Masaların Oluşturulması
            ListViewItem lvi;
            for (int i = 1; i <= db.MasaAdet; i++)
            {
                lvi = new ListViewItem("Masa " + i);
                lvi.ImageKey = db.AktifSiparisler.Any(x => x.MasaNo == i) ? "dolu" : "bos";
                lvi.Tag = i; // masaları etiketledik 
                lvwMasalar.Items.Add(lvi);
            }
            #endregion
        }

        private void tsmiUrunler_Click(object sender, EventArgs e)
        {
            new UrunlerForm(db).ShowDialog();
        }

        private void tsmiGecmisSiparisler_Click(object sender, EventArgs e)
        {
            new GecmisSiparislerForm(db).ShowDialog();
        }

        private void lvwMasalar_DoubleClick(object sender, EventArgs e)
        {
            int masaNo = (int)lvwMasalar.SelectedItems[0].Tag;
            Siparis siparis = AktifSiparisBul(masaNo);

            if (siparis == null)
            {
                siparis = new Siparis();
                siparis.MasaNo = masaNo;
                db.AktifSiparisler.Add(siparis);
                lvwMasalar.SelectedItems[0].ImageKey = "dolu";
            }

            SiparisForm frmSiparis = new SiparisForm(db, siparis, this); // basınca siparisform açılır
            DialogResult dr = frmSiparis.ShowDialog();
            
            // sipariş iptal edildiyse ya da ödeme alındıysa 
            if (dr == DialogResult.OK)
            {
                lvwMasalar.SelectedItems[0].ImageKey = "bos";
            }
        }
        private Siparis AktifSiparisBul(int masaNo)
        {
            foreach (var item in db.AktifSiparisler)
            {
                if (item.MasaNo == masaNo)
                {
                    return item;
                }
            }
            return null;

            #region Linq yöntemi
            //return db.AktifSiparisler.FirstOrDefault(x => x.Masa.No == masaNo); 
            #endregion
        }

        public void MasaTasi (int kaynak, int hedef)
        {
            foreach (ListViewItem lvi in lvwMasalar.Items)
            {
                if ((int)lvi.Tag == kaynak)
                {
                    lvi.ImageKey = "bos";
                }
                if ((int)lvi.Tag == hedef)
                {
                    lvi.ImageKey = "dolu";
                }
            }
        }

        private void AnaForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            VeriKaydet();
        }

        private void VeriKaydet()
        {
            string json = JsonConvert.SerializeObject(db, Formatting.Indented);
            File.WriteAllText("veri.json", json);
        }
    }
}
