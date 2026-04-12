using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using System.Data.SqlClient;
using System.Xml.Linq;
namespace PSC10SP
{
    public partial class frmVENSUC : Form
    {
        public frmVENSUC()
        {
            InitializeComponent();
            EstiloDGV();
        }
        public Boolean EData;
        public string var1;
        public string var2;

        private void frmVENSUC_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true; //activa las teclas de funciones
            this.Text = "Consulta";
        }

        private void frmVENSUC_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) //aqui es la condicional de si toco la tecla escape "esc" y cierra el formulario
                this.Close();
        }



        private void txtBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter) //Pregunta si presionaste la letra enter
            {
                e.Handled = true;
                if (txtBuscar.Text.Trim() != string.Empty) //Aqui pregunta si es diferente de vacio el txtbox
                {
                    txtBuscar.Focus(); //Mueve el cursor al siguiente txtbox
                }
            }
        }


        private void txtBuscar_Leave(object sender, EventArgs e)
        {
            BuscarDataSuc(txtBuscar.Text);
        }

        private void btnBuscarSuc_Click(object sender, EventArgs e)
        {
            BuscarDataSuc(txtBuscar.Text);
        }

        private void btnSelecciona_Click(object sender, EventArgs e)
        {
            if (dgv.RowCount > 0)
            {
                EData = true;
                var1 = dgv.CurrentRow.Cells["NOMBREDESUCURSAL"].Value.ToString();
                var2 = dgv.CurrentRow.Cells["DIRECCION"].Value.ToString();


                this.Close();
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            //Limpiar el Datagridview
            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            EData = false;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnSelecciona.PerformClick();
        }

public void BuscarDataSuc(string _strBuscar)
{
    dgv.Rows.Clear();
    dgv.Refresh();

    EData = false;

    string _query = "SELECT IDSUCURSAL, NOMBREDESUCURSAL, DIRECCION " +
                    "FROM SUCURSALES WHERE NOMBREDESUCURSAL LIKE '%" + _strBuscar + "%' ORDER BY NOMBREDESUCURSAL ASC";

    

    using (SqlConnection cxn = new SqlConnection(cnn.db))
    {
        cxn.Open();
        using (SqlCommand cmd = new SqlCommand(_query, cxn))
        {
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                dgv.Rows.Add();
                int nRow = dgv.Rows.Count - 1;

                dgv.Rows[nRow].Cells["NOMBREDESUCURSAL"].Value = rdr["NOMBREDESUCURSAL"].ToString();
                dgv.Rows[nRow].Cells["DIRECCION"].Value = rdr["DIRECCION"].ToString();
            }
        }
    }
}



        private void EstiloDGV()
        {
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.ColumnHeadersVisible = true;
            this.dgv.RowHeadersVisible = false;

            this.dgv.Columns.Clear();

            
            this.dgv.Columns.Add("NOMBREDESUCURSAL", "NOMBRE DE SUCURSAL");
            this.dgv.Columns.Add("DIRECCION", "DIRECCION DE LA SUCURSAL");

            DataGridViewColumn column = dgv.Columns["NOMBREDESUCURSAL"]; column.Width = 140;
            column = dgv.Columns["DIRECCION"]; column.Width = 400;

            this.dgv.BorderStyle = BorderStyle.FixedSingle;
            this.dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            this.dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgv.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            this.dgv.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            this.dgv.BackgroundColor = Color.LightGray;

            this.dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 6, 0, 6);
            this.dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.CornflowerBlue;
            this.dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }



    }
}
