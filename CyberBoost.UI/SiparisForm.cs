using CafeBoost.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyberBoost.UI
{
    public partial class SiparisForm : Form
    {
        public event EventHandler<MasaTasimaEventArgs> MasaTasindi;

        readonly KafeVeri db; // ileride değiştirmeye çalışmayalım diye readonly (ekstra güvenlik önlemi denilebilir) demesek de aynı çalışır.
        readonly Siparis siparis;
        readonly BindingList<SiparisDetay> blSiparisDetaylar; // başta null
        public SiparisForm(KafeVeri KafeVeri, Siparis siparis)
        {
            db = KafeVeri;
            this.siparis = siparis;
            InitializeComponent();
            dgvSiparisDetaylar.AutoGenerateColumns = false;
            MasalariListele();
            UrunleriListele();
            MasaNoGuncelle();
            OdemeTutariGuncelle();

            blSiparisDetaylar = new BindingList<SiparisDetay>(siparis.SiparisDetaylar); // değişiklikleri bindinglist datasource a haber veriyor. normal list vermiyor.
            blSiparisDetaylar.ListChanged += BlSiparisDetaylar_ListChanged;
            dgvSiparisDetaylar.DataSource = blSiparisDetaylar; // ekranı kapatıp açınca kaybolmaması için yazdık.


        }

        private void MasalariListele()
        {
            cboMasalar.Items.Clear();
            for (int i = 0; i < db.MasaAdet; i++)
            {
                if (!db.AktifSiparisler.Any(x => x.MasaNo == i))
                {
                    cboMasalar.Items.Add(i);
                }
            }
        }

        private void BlSiparisDetaylar_ListChanged(object sender, ListChangedEventArgs e)
        {
            OdemeTutariGuncelle();
        }

        private void OdemeTutariGuncelle()
        {
            lblOdemeTutari.Text = siparis.ToplamTutarTL;
        }

        private void UrunleriListele()
        {
            cboUrun.DataSource = db.Urunler; //foreachle eklemekle aynı. urunlerdekini cboya aktarıyor
        }

        private void MasaNoGuncelle()
        {
            Text = $"Masa {siparis.MasaNo:00} - Sipariş Detayları ({siparis.AcilisZamani.Value.ToShortTimeString()})";
            lblMasaNo.Text = siparis.MasaNo.ToString("00");
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            Urun secilenUrun = (Urun)cboUrun.SelectedItem; //object olduğu için inte kast ettik üstüne gelince içinde obj olduğu yazıyor.
            int adet = (int)nudAdet.Value;

            #region koladan 3 tane ayrı ayrı girilirse onları toplu şekilde yazıyor.
            SiparisDetay detay = blSiparisDetaylar.FirstOrDefault(x => x.UrunAd == secilenUrun.UrunAd);

            if (detay != null)
            {
                detay.Adet += adet;
                OdemeTutariGuncelle();
            }
            #endregion
            else
            {
                detay = new SiparisDetay()
                {
                    UrunAd = secilenUrun.UrunAd,
                    BirimFiyat = secilenUrun.BirimFiyat,
                    Adet = adet
                };
                blSiparisDetaylar.Add(detay);
            }
        }

        private void dgvSiparisDetaylar_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DialogResult dr = MessageBox.Show("Seçili detayları silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (dr != DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close(); // show dialogla göstermediyse bu işe yarar yoksa çok da gerek yok
        }

        private void btnSiparisIptal_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Sipariş iptal ediliyor, emin misiniz?", "İptal Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (dr == DialogResult.Yes)
                SiparisKapat(SiparisDurum.Iptal);              
        }

        private void btnOdemeAl_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Masa kapatılıyor, emin misiniz?", "Ödeme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (dr == DialogResult.Yes)
                SiparisKapat(SiparisDurum.Odendi, siparis.ToplamTutar());
        }
        private void SiparisKapat(SiparisDurum siparisDurum, decimal odenenTutar = 0)
        {
            siparis.OdenenTutar = odenenTutar;
            siparis.KapanisZamani = DateTime.Now;
            siparis.Durum = siparisDurum;
            db.AktifSiparisler.Remove(siparis);
            db.GecmisSiparisler.Add(siparis);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnMasaTasi_Click(object sender, EventArgs e)
        {
            if (cboMasalar.SelectedIndex < 0) return;

            int kaynak = siparis.MasaNo;
            int hedef = (int)cboMasalar.SelectedItem;
            siparis.MasaNo = hedef;
            MasaNoGuncelle();
            MasalariListele();

            MasaTasimaEventArgs args = new MasaTasimaEventArgs()
            {
                EskiMasaNo = kaynak,
                YeniMasaNo = hedef
            };
            if (MasaTasindi != null)
            {
                MasaTasindiginda(args);
            }
        }

        protected virtual void MasaTasindiginda(MasaTasimaEventArgs args)
        {
            MasaTasindi?.Invoke(this, args);
        }
    }
}
