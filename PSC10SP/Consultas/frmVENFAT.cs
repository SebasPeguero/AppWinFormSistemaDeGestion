using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSC10SP
{
    public partial class frmVENFAT : Form
    {
        public frmVENFAT()
        {
            InitializeComponent();
            EstiloDataGridView();
        }

        public Boolean EData;
        public string var1;
        public string var2;

        private void frmVENFAT_Load(object sender, EventArgs e)
        {
            this.Text = "Consulta";
            this.KeyPreview = true;
            EData = false;
        }

        private void frmVENFAT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscarDatos(txtBuscar.Text);
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            // Verificamos que haya filas Y que haya una fila seleccionada actualmente
            if (dgv.Rows.Count > 0 && dgv.CurrentRow != null)
            {
                EData = true;

                // Usamos protección (? :) por si la celda viniera vacía de base de datos
                var1 = dgv.CurrentRow.Cells[0].Value != null ? dgv.CurrentRow.Cells[0].Value.ToString() : ""; // FACTURA
                var2 = dgv.CurrentRow.Cells[1].Value != null ? dgv.CurrentRow.Cells[1].Value.ToString() : ""; // CLIENTE

                this.Close();
            }
            else
            {
                MessageBox.Show("Por favor seleccione un registro válido.");
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            // Limpia el DataGridView
            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            EData = false;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtBuscar.Text.Trim() != string.Empty)
                {
                    btnBuscar.Focus();
                }
            }
        }

        private void BuscarDatos(string buscar)
        {
            // Limpia el DataGridView
            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            EData = false;

            string miQuery = @"   SELECT A.FACTURA, 
                                         A.CLIENTE, 
                                         B.NOMBRECLIENTE, 
                                         A.FECHA, 
                                         A.MONTOFACTURADO 
                                    FROM HFACTURA A 
                              INNER JOIN CLIENTES B ON A.CLIENTE = B.IDCLIENTE
                                    WHERE B.NOMBRECLIENTE LIKE @A1
                                      AND A.ACTIVO  = @A2 ";

            SqlConnection cxn = new SqlConnection(cnn.db); cxn.Open();
            SqlCommand cmd = new SqlCommand(miQuery, cxn);
            cmd.Parameters.AddWithValue("@A1", "%" + buscar + "%");
            cmd.Parameters.AddWithValue("@A2", "1");
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                dgv.Rows.Add();
                int xrows = dgv.Rows.Count - 1;

                dgv[0, xrows].Value = dr["FACTURA"].ToString();
                dgv[1, xrows].Value = dr["CLIENTE"].ToString();
                dgv[2, xrows].Value = dr["NOMBRECLIENTE"].ToString();
                dgv[3, xrows].Value = dr["FECHA"].ToString();
                dgv[4, xrows].Value = dr["MONTOFACTURADO"].ToString();
            }

            cmd.Dispose();
            cxn.Close();
        }

        private void EstiloDataGridView()
        {
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.ColumnHeadersVisible = true;
            this.dgv.RowHeadersVisible = false;

            this.dgv.Columns.Add("Col00", "FACTURA");
            this.dgv.Columns.Add("Col01", "CLIENTE");
            this.dgv.Columns.Add("Col02", "NOMBRE");
            this.dgv.Columns.Add("Col03", "FECHA");
            this.dgv.Columns.Add("Col04", "MONTO");

            DataGridViewColumn
            column = dgv.Columns[00]; column.Width = 140;
            column = dgv.Columns[01]; column.Width = 100;
            column = dgv.Columns[02]; column.Width = 300;
            column = dgv.Columns[03]; column.Width = 100;
            column = dgv.Columns[04]; column.Width = 100;

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

        private void btnSeleccionar_Click1(object sender, EventArgs e)
        {
            if (dgv.RowCount > 0)
            {
                EData = true;
                var1 = dgv.CurrentRow.Cells[0].Value.ToString();
                var2 = dgv.CurrentRow.Cells[1].Value.ToString();

                this.Close();
            }
        }

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // PROTECCIÓN: Si el usuario da clic en el encabezado (RowIndex = -1), no hacemos nada.
            if (e.RowIndex < 0) return;

            // Si la fila es válida, ejecutamos el botón seleccionar automáticamente
            btnSeleccionar.PerformClick();
        }
    }
}
