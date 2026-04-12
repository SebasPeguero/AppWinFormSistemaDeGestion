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
    public partial class frmVENSUP : Form
    {
        public frmVENSUP()
        {
            InitializeComponent();

            EstiloDGV();
        }

        public Boolean EData;
        public string var1;
        public string var2;

        private void frmVENSUP_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true; //activa las teclas de funciones
            this.Text = "Consulta";
        
    }

        private void frmVENSUP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {

                //aqui es la condicional de si toco la tecla escape "esc" y cierra el formulario
                this.Close();
            }
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
            BuscarData(txtBuscar.Text);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscarData(txtBuscar.Text);
        }

        private void btnSelecciona_Click(object sender, EventArgs e)
        {
            if (dgv.RowCount > 0)
            {
                EData = true;
                var1 = dgv.CurrentRow.Cells[0].Value.ToString();
                var2 = dgv.CurrentRow.Cells[1].Value.ToString();

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

        private void BuscarData(string _strBuscar)
        {
            // Limpiamos el grid
            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            // Variable de control (asumo que la usas fuera)
            // EData = false; 

            // 1. QUERY ACTUALIZADO PARA TABLA PROVEEDORES
            // Seleccionamos IDPROVEEDOR y NombreEmpresa según tu imagen
            string _query = "SELECT IDPROVEEDOR, NombreEmpresa " +
                            "FROM PROVEEDORES " +
                            "WHERE NombreEmpresa LIKE @buscar " +
                            "ORDER BY NombreEmpresa ASC";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                try
                {
                    cxn.Open();
                    SqlCommand cmd = new SqlCommand(_query, cxn);

                    // Parámetro de búsqueda
                    cmd.Parameters.AddWithValue("@buscar", "%" + _strBuscar + "%");

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        // 2. LECTURA DE COLUMNAS CORRECTAS
                        // Usamos los nombres exactos que vi en tu imagen
                        string id = rdr["IDPROVEEDOR"].ToString();
                        string nombre = rdr["NombreEmpresa"].ToString();

                        // 3. AGREGAR AL GRID
                        // Asumo que tu grid tiene 2 columnas visibles (ID y Nombre)
                        dgv.Rows.Add(id, nombre);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error cargando proveedores: " + ex.Message);
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

            this.dgv.Columns.Add("A", "SUPLIDOR");
            this.dgv.Columns.Add("B", "NOMBRE");

            DataGridViewColumn
            column = dgv.Columns[00]; column.Width = 140;
            column = dgv.Columns[01]; column.Width = 400;


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
