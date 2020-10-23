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
    public partial class GecmisSiparislerForm : Form
    {
        private readonly KafeVeri kafeVeri;

        public GecmisSiparislerForm(KafeVeri kafeVeri)
        {
            this.kafeVeri = kafeVeri;
            InitializeComponent();
            dgvSiparisler.DataSource = kafeVeri.GecmisSiparisler;
        }

        private void dgvSiparisler_SelectionChanged(object sender, EventArgs e)
        {
            // En az 1 seçili satır varsa
            if (dgvSiparisler.SelectedRows.Count > 0)
            {
                // seçili satırlarının ilkinin üzerindeki Siparis nesnesi 
                Siparis seciliSiparis = (Siparis)dgvSiparisler.SelectedRows[0].DataBoundItem;
                dgvSiparisDetaylar.DataSource = seciliSiparis.SiparisDetaylar;
            }
        }
    }
}
