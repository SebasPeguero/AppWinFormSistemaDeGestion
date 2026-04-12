using PSC10;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSC10SP
{
    public partial class frmMenu : Form
    {
        public frmMenu()
        {
            InitializeComponent();
        }

        private void frmMenu_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.Text = "MENU GENERAL";
        }

        private void frmMenu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblHORA.Text = DateTime.Now.ToString("hh:mm:ss ");
            lblFECHA.Text = DateTime.Now.ToLongDateString();
        }


        private void usuarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmUsuarios frm = new frmUsuarios();
            frm.ShowDialog();
        }
        private void almacenesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void articuloToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmProducto frm = new frmProducto();
            frm.ShowDialog();
        }

        private void facturaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmFactura frm = new frmFactura("", "");
            frm.ShowDialog();
        }

        private void clientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmClientes frm = new frmClientes();
            frm.ShowDialog();
        }

        private void reciboToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmRecibos frm = new frmRecibos();
            frm.ShowDialog();
        }

        private void movimientosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMVTOCTE frm = new frmMVTOCTE();
            frm.ShowDialog();
        }

        private void facturasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmVENFAT frm = new frmVENFAT();
            frm.ShowDialog();
        }

        private void recibosToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ordenesDeComprasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmVENART frm = new frmVENART();
            frm.ShowDialog();
        }
    }
}
