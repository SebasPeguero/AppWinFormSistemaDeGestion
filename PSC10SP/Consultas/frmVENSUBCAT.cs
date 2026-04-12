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
    public partial class frmVENSUBCAT : Form
    {
        public frmVENSUBCAT()
        {
            InitializeComponent();
            EstiloDGV();
        }

        public Boolean EData;
        public string var1;
        public string var2;


        private void frmVENSUBCAT_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true; //activa las teclas de funciones
            this.Text = "Consulta";
        }

        private void frmVENSUBCAT_KeyDown(object sender, KeyEventArgs e)
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
            // 1. Limpiar el Datagridview
            this.dgv.Rows.Clear();
            this.dgv.Refresh();
            EData = false;

            // 2. CORRIGE AQUÍ EL NOMBRE DE LA TABLA
            // Asegúrate que 'CATEGORIAS_SUB' es el nombre real en tu SQL. 
            // Si se llama 'SUBCATEGORIAS', cámbialo abajo.
            string _query = "SELECT IDCATEGORIA, SUBCATEGORIA, NOMBRE " +
                            "FROM CATEGORIAS_SUB " +
                            "WHERE NOMBRE LIKE @buscar " +
                            "ORDER BY NOMBRE ASC";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                try
                {
                    cxn.Open();
                    SqlCommand cmd = new SqlCommand(_query, cxn);

                    // Uso de parámetro para evitar errores de comillas
                    cmd.Parameters.AddWithValue("@buscar", "%" + _strBuscar + "%");

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        // Agregamos una fila vacía
                        int nRow = dgv.Rows.Add();

                        // CORRECCIÓN DE ERRORES DE LÓGICA:

                        // Error 1: Tenías 'IDCAREGORIA' (typo). Debe coincidir con la base de datos.
                        dgv.Rows[nRow].Cells[0].Value = rdr["IDCATEGORIA"].ToString();

                        // Error 2: Asignamos la columna 1
                        dgv.Rows[nRow].Cells[1].Value = rdr["SUBCATEGORIA"].ToString();

                        // Error 3: Tenías dgv[0] otra vez. Esto sobrescribía el ID.
                        // Asumo que el NOMBRE va en la columna 2. Si solo tienes 2 columnas, ajusta esto.
                        if (dgv.Columns.Count > 2)
                        {
                            dgv.Rows[nRow].Cells[2].Value = rdr["NOMBRE"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al buscar: " + ex.Message);
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

            this.dgv.Columns.Add("A", "CATEGORIA");
            this.dgv.Columns.Add("B", "SUBCATEGORIA");
            this.dgv.Columns.Add("C", "DETALLE");

            DataGridViewColumn
            column = dgv.Columns[00]; column.Width = 180;
            column = dgv.Columns[01]; column.Width = 180;
            column = dgv.Columns[02]; column.Width = 400;


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
