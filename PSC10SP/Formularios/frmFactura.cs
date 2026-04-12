using PSC10;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSC10SP
{
    public partial class frmFactura : Form
    {
        public frmFactura(string account, string invoice)
        {
            InitializeComponent();

            EstiloDGV();

            _cuenta = account;
            _factura = invoice;
        }

        string _cuenta;
        string _factura;

        double _Impuesto;
        double _Total;
        double _SubTotal;
        double _LnImpuesto;
        double _nmCant;
        double _nmPrec;

        bool ExisteData;

        private void frmFactura_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.Text = "Factura";

            lblFactura.Text = Busco.BuscaUltimoNumero("2");
            lblFechaFactura.Text = DateTime.Now.ToString("ddMMyyyy");
            lblSucursal.Text = cnn.miSucursal;  // me indica el numero de sucursal / almacen 

            ExisteData = false;

            if (_cuenta != "")
            {
                txtCliente.Text = _cuenta;
                lblFactura.Text = _factura;
                BuscarFactura(lblFactura.Text, txtCliente.Text);
            }

        }

        private void frmFactura_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void txtCliente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
            {
                btnCliente.PerformClick();
            }
        }


        private void txtCliente_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtCliente.Text.Trim() != string.Empty)
                {
                    txtArticulo.Focus();
                }
            }
        }


        private void txtCliente_Leave(object sender, EventArgs e)
        {
            if (txtCliente.Text.Trim() != string.Empty)
            {
                lblCliente.Text = Busco.BuscarCliente(txtCliente.Text, out bool found, out string nivel, out string tipo, out string paga);

                if (found == true)
                {
                    lblNivel.Text = nivel;
                    lblTipo.Text = tipo;
                    lblPaga.Text = paga;
                }
            }
        }

        private void txtArticulo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
            {
                btnArticulo.PerformClick();
            }
        }

        private void txtArticulo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtArticulo.Text.Trim() != string.Empty)
                {
                    txtCantidad.Focus();
                }
                return;

                // Permite digitos, teclas de control y un punto decimal
                if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
                {
                    // pertimido
                    return;
                }

                // permite un solo punto o coma decimal, pero no ambos
                if ((e.KeyChar == '.'
                    || e.KeyChar == ',')
                    && !txtArticulo.Text.Contains('.')
                    && !txtArticulo.Text.Contains(','))
                {
                    return;
                }

                e.Handled = true;



            }

            // Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                // pertimido
                return;
            }

            // permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtArticulo.Text.Contains('.')
                && !txtArticulo.Text.Contains(','))
            {
                return;
            }

            e.Handled = true;
        }

        private void txtArticulo_Leave(object sender, EventArgs e)
        {
            string texto = txtArticulo.Text.Trim();

            if (string.IsNullOrEmpty(texto))
                return;

            string textoNormalizado = texto.Replace(',', '.');

            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un articulo válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtArticulo.Focus();
                return;
            }

            if (txtCliente.Text.Trim() != string.Empty)
            {
                lblCliente.Text =
                    Busco.BuscarCliente(txtCliente.Text, out bool found, out string nivel, out string tipo, out string paga);

                if (found == true)
                {
                    BuscarArticulo(txtArticulo.Text);
                }
                else
                {
                    MessageBox.Show("Coloca un cliente", "Miapp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Coloca un cliente", "Miapp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }


        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtCantidad.Text.Trim() != string.Empty)
                {
                    btnInsertarLn.Focus();
                }
                return;
            }

            // Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                // pertimido
                return;
            }

            // permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtCantidad.Text.Contains('.')
                && !txtCantidad.Text.Contains(','))
            {
                return;
            }

            e.Handled = true;

        }

        private void txtCantidad_Leave(object sender, EventArgs e)
        {
            string texto = txtCantidad.Text.Trim();

            if (string.IsNullOrEmpty(texto))
                return;

            string textoNormalizado = texto.Replace(',', '.');

            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese una cantidad válida.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCantidad.Focus();
                return;
            }

            txtCantidad.Text = valor.ToString("N2");

            CalcularLineaDetalle();
            TotalizarFactura();

            btnInsertarLn.PerformClick();

        }



        // ----------------------------------------
        // BOTONES
        // ----------------------------------------



        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (lblTotal.Text != string.Empty)
            {
                if (ExisteData == false)
                {
                    InsertarData();
                }
                else
                {
                    // ActualizarFactura();
                }

                // -----------------------------------------
                // IMPRIME FACTURA
                // -----------------------------------------
                //BorrarTablasReporte();

                //frmRPT000 frm = new frmRPT000(@"C:\ITLA\Reportes\rptFactura.rpt", "Factura");
                //frm.Show();

                // -----------------------------------------
                LimpiarFormulario();
                txtCliente.Focus();
            }
        }



        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            LimpiarDetalle();
            lblFactura.Text = Busco.BuscaUltimoNumero("2");
        }


        private void btnBorrar_Click(object sender, EventArgs e)
        {
            if (ExisteData == true)
            {
                DialogResult dialogResult = MessageBox.Show("Deseas Borrar el Registro", "ITLA",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialogResult == DialogResult.Yes)
                {
                    BorrarData(lblFactura.Text);
                    btnLimpiar.PerformClick();
                }
            }
        }


        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void btnCONFACT_Click(object sender, EventArgs e)
        {
             frmVENFAT frm = new frmVENFAT();
            frm.ShowDialog();

            if (frm.EData == true)
            {
                lblFactura.Text = frm.var1;
                txtCliente.Text = frm.var2;

                BuscarFactura(lblFactura.Text, txtCliente.Text);
            }
        }

        private void btnVENCTE_Click(object sender, EventArgs e)
        {
            frmVENCTE frm = new frmVENCTE();
            frm.ShowDialog();

            if (frm.EData == true)
            {
                txtCliente.Text = frm.var1;

                lblCliente.Text = Busco.BuscarCliente(txtCliente.Text, out bool found, out string nivel, out string tipo, out string paga);

                if (found == true)
                {
                    lblNivel.Text = nivel;
                    lblTipo.Text = tipo;
                    lblPaga.Text = paga;
                }
            }

        }

        private void btnArticulo_Click(object sender, EventArgs e)
        {
            frmVENART frm = new frmVENART();
            frm.ShowDialog();

            if (frm.EData == true)
            {
                txtArticulo.Text = frm.var1;

                if (txtCliente.Text.Trim() != string.Empty)
                {
                    // Buscamos los datos del cliente y los guardamos en variables 'out'
                    lblCliente.Text = Busco.BuscarCliente(txtCliente.Text, out bool found, out string nivel, out string tipo, out string paga);

                    if (found == true)
                    {
                        // --- CORRECCIÓN IMPORTANTE ---
                        // Debes pasar los datos encontrados a los Labels ANTES de buscar el artículo.
                        // Si no haces esto, BuscarArticulo no sabrá qué lista de precios usar.
                        lblNivel.Text = nivel;
                        lblTipo.Text = tipo;
                        lblPaga.Text = paga;
                        // -----------------------------

                        // Ahora sí, llamamos al método (que ya arreglamos con la subconsulta de la Sucursal)
                        BuscarArticulo(txtArticulo.Text);
                    }
                    else
                    {
                        MessageBox.Show("El cliente no aparece en la base de datos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Por favor, coloca un cliente antes de buscar artículos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }



        private void btnInsertarLn_Click(object sender, EventArgs e)
        {
            if (txtArticulo.Text.Trim() != string.Empty)
            {
                if (txtCantidad.Text.Trim() != string.Empty)
                {
                    InsertaLinea();
                    TotalizarFactura();
                    LimpiarDetalle();

                    txtArticulo.Focus();
                }
            }
        }



        private void btnEditarLinea_Click(object sender, EventArgs e)
        {
            if (dgv.RowCount > 0)
            {
                LimpiarDetalle();

                txtArticulo.Text = dgv.CurrentRow.Cells[0].Value.ToString();
                lblArticulo.Text = dgv.CurrentRow.Cells[1].Value.ToString();
                txtCantidad.Text = dgv.CurrentRow.Cells[2].Value.ToString();
                lblPrecio.Text = dgv.CurrentRow.Cells[3].Value.ToString();
                lblImpuestoLn.Text = dgv.CurrentRow.Cells[4].Value.ToString();
                lblTotalLn.Text = dgv.CurrentRow.Cells[5].Value.ToString();

                BorrarLinea();
                TotalizarFactura();

                txtArticulo.Focus();
            }

        }

        private void btnBorrarLn_Click(object sender, EventArgs e)
        {
            if (dgv.RowCount > 0)
            {
                BorrarLinea();
                TotalizarFactura();
                txtArticulo.Focus();
            }

        }

        private void btnLimpiarDT_Click(object sender, EventArgs e)
        {
            LimpiarDetalle();
            lblSubtotal.Text = "";
            lblImpuesto.Text = "";
            lblTotal.Text = "";

            this.dgv.Rows.Clear();
            this.dgv.Refresh();
        }






        // ----------------------------------------
        // METODOS
        // ----------------------------------------


        private void BorrarLinea()
        {
            int CuantasLineasTengo = Convert.ToInt32(dgv.RowCount);

            if (CuantasLineasTengo == 1)
            {
                dgv.Rows.RemoveAt(dgv.RowCount - 1); // se utiliza cuando te queda una sola linea
            }
            else
            {
                dgv.Rows.Remove(dgv.CurrentRow);  // utilizada cuando tienes varias linea
            }
        }



        private void EstiloDGV()
        {
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;

            // 1. FORZAMOS QUE SE VEAN
            this.dgv.ColumnHeadersVisible = true;
            this.dgv.RowHeadersVisible = false;

            // 2. AJUSTE DE ALTURA
            this.dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dgv.ColumnHeadersHeight = 30;

            // ---------------------------------------------------------
            // AQUI ESTABA EL ERROR: FALTABAN DEFINIR LAS COLUMNAS
            // ---------------------------------------------------------
            this.dgv.Columns.Clear();

            this.dgv.Columns.Add("Col0", "ARTICULO");      // Indice 0
            this.dgv.Columns.Add("Col1", "DESCRIPCION");   // Indice 1
            this.dgv.Columns.Add("Col2", "CANTIDAD");      // Indice 2
            this.dgv.Columns.Add("Col3", "PRECIO");        // Indice 3
            this.dgv.Columns.Add("Col4", "IMPUESTO");      // Indice 4
            this.dgv.Columns.Add("Col5", "TOTAL");         // Indice 5
            this.dgv.Columns.Add("Col6", "ALMACEN");       // Indice 6 (Lo usas al guardar)

            // Configuración de Anchos
            dgv.Columns[0].Width = 100; // Articulo
            dgv.Columns[1].Width = 250; // Descripcion
            dgv.Columns[2].Width = 80;  // Cantidad
            dgv.Columns[3].Width = 100; // Precio
            dgv.Columns[4].Width = 80;  // Impuesto
            dgv.Columns[5].Width = 100; // Total
            dgv.Columns[6].Width = 100; // Almacen (o ponle .Visible = false si no quieres que se vea)

            // Alineaciones numéricas para que se vea profesional
            dgv.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Estilos de Color
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

        private void LimpiarDetalle()
        {
            txtArticulo.Clear();
            lblArticulo.Text = "";
            txtCantidad.Clear();
            lblPrecio.Text = "";
            lblImpuestoLn.Text = "";
            lblTotalLn.Text = "";
        }



        private void BuscarArticulo(string nmArticulo)
        {
            // Limpiamos datos previos
            lblArticulo.Text = "";
            lblPrecio.Text = "0.00";

            // QUERY CORREGIDO Y VERIFICADO CON TUS IMÁGENES:
            // 1. Usamos NOMBREDESUCURSAL (que es como se llama en tu tabla realmente).
            // 2. La subconsulta convierte el nombre "Sucursal Romana" al ID "1" automáticamente.
            string miQuery = @"
        SELECT T1.IDPRODUCTO, T1.DESCRIPCION, T1.COSTO, T1.IMPUESTO, 
               T1.PRECIOA, T1.PRECIOB, T1.PRECIOC, T1.PRECIOD, T1.PRECIOE,
               T1.STOCK, T2.STOCK AS EXISTENCIA, T2.IDALMACEN 
          FROM PRODUCTOS T1 
    INNER JOIN ALMACENSTOCK T2 ON T1.IDPRODUCTO = T2.IDPRODUCTO
         WHERE T1.IDPRODUCTO = @A1 
           AND T1.ESTATUS    = 1 
           AND T2.IDALMACEN  = (SELECT TOP 1 IDSUCURSAL 
                                  FROM SUCURSALES 
                                 WHERE NOMBREDESUCURSAL = @A2)";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                try
                {
                    cxn.Open();
                    SqlCommand cmd = new SqlCommand(miQuery, cxn);

                    // Parámetros
                    cmd.Parameters.AddWithValue("@A1", nmArticulo);

                    // Aquí pasamos el TEXTO "Sucursal Romana". 
                    // SQL usará la subconsulta de arriba para encontrar su ID numérico.
                    cmd.Parameters.AddWithValue("@A2", lblSucursal.Text);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        txtArticulo.Text = dr["IDPRODUCTO"].ToString();
                        lblArticulo.Text = dr["DESCRIPCION"].ToString();

                        // Lógica de precios
                        string precio = "0.00";
                        if (lblNivel.Text == "1") precio = dr["PRECIOA"].ToString();
                        else if (lblNivel.Text == "2") precio = dr["PRECIOB"].ToString();
                        else if (lblNivel.Text == "3") precio = dr["PRECIOC"].ToString();
                        else if (lblNivel.Text == "4") precio = dr["PRECIOD"].ToString();
                        else if (lblNivel.Text == "5") precio = dr["PRECIOE"].ToString();

                        // Validación de nulos
                        if (string.IsNullOrEmpty(precio)) precio = "0.00";

                        // Formato moneda
                        if (decimal.TryParse(precio, out decimal p))
                            lblPrecio.Text = p.ToString("N2");
                        else
                            lblPrecio.Text = "0.00";

                        _LnImpuesto = Convert.ToDouble(dr["IMPUESTO"].ToString());
                    }
                    else
                    {
                        // Si no encuentra nada, avisa y limpia
                        MessageBox.Show($"El artículo no existe o no hay stock registrado en '{lblSucursal.Text}'");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al buscar artículo: " + ex.Message);
                }
            }
        }





        private void BuscarFactura(string nmrfactura, string nmcliente)
        {
            ExisteData = false;

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                try
                {
                    cnx.Open();

                    // CORRECCIÓN:
                    // 1. En HFACTURA (A) usamos 'CLIENTE'
                    // 2. En CLIENTES (B) usamos 'IDCLIENTE'
                    string tsQuery = @"
                 SELECT A.FACTURA, 
                        A.CLIENTE,        -- Antes decía A.IDCLIENTE
                        B.NOMBRECLIENTE, 
                        B.NIVELPRECIO, 
                        B.TIPOCLIENTE, 
                        B.IMPUESTOPAGA,
                        A.FECHA, 
                        A.SUBTOTAL, 
                        A.IMPUESTO, 
                        A.MONTOFACTURADO 
                   FROM HFACTURA A 
             INNER JOIN CLIENTES B ON A.CLIENTE = B.IDCLIENTE -- Join corregido
                  WHERE A.FACTURA = @A1 
                    AND A.CLIENTE = @A2   -- Where corregido
                    AND A.ACTIVO = '1'";

                    SqlCommand cmd = new SqlCommand(tsQuery, cnx);
                    cmd.Parameters.AddWithValue("@A1", nmrfactura);
                    cmd.Parameters.AddWithValue("@A2", nmcliente);

                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        ExisteData = true;

                        lblFactura.Text = rdr["FACTURA"].ToString();

                        // Leemos la columna correcta: CLIENTE
                        txtCliente.Text = rdr["CLIENTE"].ToString();
                        lblCliente.Text = rdr["NOMBRECLIENTE"].ToString();

                        if (DateTime.TryParse(rdr["FECHA"].ToString(), out DateTime fecha))
                            lblFechaFactura.Text = fecha.ToShortDateString();
                        else
                            lblFechaFactura.Text = rdr["FECHA"].ToString();

                        lblSubtotal.Text = string.Format("{0:N2}", rdr["SUBTOTAL"]);
                        lblImpuesto.Text = string.Format("{0:N2}", rdr["IMPUESTO"]);
                        lblTotal.Text = string.Format("{0:N2}", rdr["MONTOFACTURADO"]);

                        lblNivel.Text = rdr["NIVELPRECIO"].ToString();
                        lblTipo.Text = rdr["TIPOCLIENTE"].ToString();
                        lblPaga.Text = rdr["IMPUESTOPAGA"].ToString();

                        BuscarDetalle(nmrfactura);
                        TotalizarFactura();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al buscar factura: " + ex.Message);
                }
            }
        }


        private void BuscarDetalle(string nmrFactura)
        {
            // Limpiar el datagridview
            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            // QUERY CORREGIDO SEGÚN TU IMAGEN:
            // 1. Usamos IDREG en vez de SEC
            // 2. Usamos PRECIOVENTA en vez de PRECIODEVENTA
            string tsQuery = @"
        SELECT A.FACTURA, 
               A.IDREG, 
               A.ARTICULO, 
               B.DESCRIPCION, 
               A.CANTIDAD, 
               A.PRECIOVENTA, 
               A.IMPUESTO, 
               A.MONTOPORLINEA 
          FROM DFACTURA A 
    INNER JOIN PRODUCTOS B ON A.ARTICULO = B.IDPRODUCTO
         WHERE A.FACTURA = @A1";

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                try
                {
                    cnx.Open();
                    SqlCommand cmd = new SqlCommand(tsQuery, cnx);
                    cmd.Parameters.AddWithValue("@A1", nmrFactura);

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        // Agregamos una nueva fila
                        int xRows = dgv.Rows.Add();

                        // Llenamos las celdas (Asegúrate que el orden coincida con tus columnas visuales)
                        dgv[0, xRows].Value = rdr["ARTICULO"].ToString();
                        dgv[1, xRows].Value = rdr["DESCRIPCION"].ToString();
                        dgv[2, xRows].Value = rdr["CANTIDAD"].ToString();

                        // Aquí estaba el error principal, ahora usamos el nombre correcto
                        dgv[3, xRows].Value = rdr["PRECIOVENTA"].ToString();

                        dgv[4, xRows].Value = rdr["IMPUESTO"].ToString();
                        dgv[5, xRows].Value = rdr["MONTOPORLINEA"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error cargando el detalle de la factura: " + ex.Message);
                }
            }
        }

        private void TotalizarFactura()
        {
            _Impuesto = 0;
            _SubTotal = 0;
            _Total = 0;

            lblSubtotal.Text = "";
            lblImpuesto.Text = "";
            lblTotal.Text = "";

            foreach (DataGridViewRow row in dgv.Rows) // recorre la grilla desde la linea 0 hasta la ultima
            {
                double nImpuesto = Convert.ToDouble(row.Cells[4].Value.ToString() ?? "");
                double nSubtotal = Convert.ToDouble(row.Cells[5].Value.ToString() ?? "");
                double nTotal = nSubtotal + nImpuesto;

                _Impuesto = _Impuesto + nImpuesto;
                _SubTotal = _SubTotal + nSubtotal;
                _Total = _Total + nTotal;

                lblSubtotal.Text = _SubTotal.ToString();
                lblImpuesto.Text = _Impuesto.ToString();
                lblTotal.Text = _Total.ToString();
            }
        }

        private void CalcularLineaDetalle()
        {
            _nmCant = 0;
            _nmPrec = 0;

            _nmCant = Convert.ToDouble(txtCantidad.Text);
            _nmPrec = Convert.ToDouble(lblPrecio.Text);

            if (_nmCant > 0)
            {
                if (_nmPrec > 0)
                {
                    double total = _nmPrec * _nmCant;
                    lblImpuestoLn.Text = (total * _LnImpuesto).ToString();
                    lblTotalLn.Text = total.ToString();
                }
            }
        }

        private void InsertaLinea()
        {
            dgv.Rows.Add(); // agregamos una linea a la grilla
            int xRows = dgv.Rows.Count - 1; // tenemos la linea correcta a escribir

            dgv[00, xRows].Value = txtArticulo.Text;
            dgv[01, xRows].Value = lblArticulo.Text;
            dgv[02, xRows].Value = txtCantidad.Text;
            dgv[03, xRows].Value = lblPrecio.Text;
            dgv[04, xRows].Value = lblImpuestoLn.Text;
            dgv[05, xRows].Value = lblTotalLn.Text;
            dgv[06, xRows].Value = lblSucursal.Text;
        }

        private void BorrarData(string factura)
        {
            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();
                string borra = "BEGIN TRAN; DELETE FROM HFACTURA WHERE FACTURA = @A1; COMMIT TRAN;";
                SqlCommand cmd = new SqlCommand(borra, cxn);
                cmd.Parameters.AddWithValue("@A1", factura);
                cmd.ExecuteNonQuery();

                string borro = "BEGIN TRAN; DELETE FROM DFACTURA WHERE FACTURA = @A1; COMMIT TRAN;";
                SqlCommand cmo = new SqlCommand(borro, cxn);
                cmo.Parameters.AddWithValue("@A1", factura);
                cmo.ExecuteNonQuery();
            }
        }

        private void BorrarTablasReporte()
        {
            string stQuery = "DELETE FROM HFactura;";
            OleDbConnection cxn = new OleDbConnection(cnn.ac); cxn.Open();

            OleDbCommand cmd = new OleDbCommand(stQuery, cxn);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            string tsQuery = "DELETE FROM DFactura;";
            OleDbCommand cdm = new OleDbCommand(tsQuery, cxn);
            cdm.ExecuteNonQuery();
            cdm.Dispose();

            cxn.Close();
        }

        private void InsertarHeaderFacturaReporte()
        {
            if (dgv.RowCount > 0)
            {
                if (lblTotal.Text != string.Empty)
                {
                    string stQuery =
                        "INSERT INTO HFACTURA (FACTURA, CLIENTE, FECHA, SUBTOTAL, " +
                        " IMPUESTO, MONTOFACTURADO, ACTIVO, NOMBRE ) " +
                        "VALUES (@A0, @A1, @A2, @A3, @A4, @A5, @A6, @A7);";
                    OleDbConnection cnt = new OleDbConnection(cnn.db); cnt.Open();
                    OleDbCommand cmd = new OleDbCommand(stQuery, cnt);

                    cmd.Parameters.AddWithValue("@A0", lblFactura.Text);
                    cmd.Parameters.AddWithValue("@A1", txtCliente.Text);
                    cmd.Parameters.AddWithValue("@A2", lblFechaFactura.Text);
                    cmd.Parameters.AddWithValue("@A3", lblSubtotal.Text);
                    cmd.Parameters.AddWithValue("@A4", lblImpuesto.Text);
                    cmd.Parameters.AddWithValue("@A5", lblTotal.Text);
                    cmd.Parameters.AddWithValue("@A6", "1");
                    cmd.Parameters.AddWithValue("@A7", lblCliente.Text);

                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cnt.Close();
                }
            }
        }

        private void InsertarDetailsFacturaReporte()
        {
            string stQueri = "INSERT INTO DFACTURA " +
                             "          ( FACTURA, " +
                             "            ARTICULO, " +
                             "            DESCRIPCION, " +
                             "            CANTIDAD, " +
                             "            PRECIODEVENTA, " +
                             "            IMPUESTO, " +
                             "            MONTOPORLINEA, " +
                             "            CLIENTE,  " +
                             "            FECHA,    " +
                             "            ACTIVO ) " +
                             "     VALUES (@A0, @A1, @A2, @A3, @A4, @A5, @A6, @A7, @A9)";

            OleDbConnection cnx = new OleDbConnection(cnn.db); cnx.Open();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                string nmArt = row.Cells[0].Value.ToString() ?? ""; // CODIGO DEL ARTICULO
                string nmDes = row.Cells[1].Value.ToString() ?? ""; // CODIGO DEL ARTICULO
                string nmCan = row.Cells[2].Value.ToString() ?? ""; // CANTIDAD
                string nmPre = row.Cells[3].Value.ToString() ?? ""; // PRECIO
                string nmImp = row.Cells[4].Value.ToString() ?? ""; // IMPUESTO POR LINEA
                string nmTot = row.Cells[5].Value.ToString() ?? ""; // TOTAL POR LINEA
                string nmAlm = row.Cells[6].Value.ToString() ?? ""; // ID DEL ALMACEN

                OleDbCommand cmm = new OleDbCommand(stQueri, cnx);
                cmm.Parameters.AddWithValue("@A0", lblFactura.Text);
                cmm.Parameters.AddWithValue("@A1", nmArt);
                cmm.Parameters.AddWithValue("@A2", nmCan);
                cmm.Parameters.AddWithValue("@A3", nmPre);
                cmm.Parameters.AddWithValue("@A4", nmImp);
                cmm.Parameters.AddWithValue("@A5", nmTot);
                cmm.Parameters.AddWithValue("@A6", txtCliente.Text);
                cmm.Parameters.AddWithValue("@A7", lblFechaFactura.Text);
                cmm.Parameters.AddWithValue("@A9", "1");
                cmm.Parameters.AddWithValue("@A9", nmDes);

                cmm.ExecuteNonQuery();
                cmm.Dispose();

                RebajaInventario(nmArt, nmCan, nmAlm);
            }
        }

        private void LimpiarFormulario()
        {
            ExisteData = false;
            txtCliente.Clear();
            lblCliente.Text = "";

            lblSubtotal.Text = "";
            lblImpuesto.Text = "";
            lblTotal.Text = "";

            LimpiarDetalle();

            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            string _factura = Busco.BuscaUltimoNumero("2");

            lblFactura.Text = Convert.ToInt32(_factura).ToString("D8"); // ahora tiene 8 posiciones

            lblFechaFactura.Text = DateTime.Now.ToString("yyyyMMdd");
        }

        private void InsertarData()
        {
            if (dgv.RowCount > 0)
            {
                if (lblTotal.Text != string.Empty)
                {
                    string query = @"INSERT INTO HFACTURA (FACTURA, CLIENTE, FECHA, SUBTOTAL, IMPUESTO, MONTOFACTURADO)
                                     VALUES (@A1, @A2, @A3, @A4, @A5, @A6)";

                    using (SqlConnection cxn = new SqlConnection(cnn.db))
                    {
                        cxn.Open();
                        SqlCommand cmd = new SqlCommand(query, cxn);
                        cmd.Parameters.AddWithValue("@A1", lblFactura.Text);
                        cmd.Parameters.AddWithValue("@A2", txtCliente.Text);
                        cmd.Parameters.AddWithValue("@A3", lblFechaFactura.Text);
                        cmd.Parameters.AddWithValue("@A4", lblSubtotal.Text);
                        cmd.Parameters.AddWithValue("@A5", lblImpuesto.Text);
                        cmd.Parameters.AddWithValue("@A6", lblTotal.Text);
                        cmd.ExecuteNonQuery();

                        InsertarDetalleFactura();

                        ActualizaBalanceCliente(txtCliente.Text, lblTotal.Text);
                        ActualizaSecuenciaFactura(lblFactura.Text);
                        InsertarMvto();
                    }
                }
            }
        }
        private void InsertarDetalleFactura()
        {
            string query = @"
                            INSERT INTO DFACTURA (
                                        FACTURA, ARTICULO, CANTIDAD, PRECIOVENTA, IMPUESTO, MONTOPORLINEA, ALMACEN, CLIENTE, FECHA, ACTIVO )
                            VALUES (@A0, @A1, @A2, @A3, @A4, @A5, @A6, @A7, @A8 ) ";
            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();

                foreach (DataGridViewRow dr in dgv.Rows)
                {
                    SqlCommand cmd = new SqlCommand(query, cxn);

                    string A0 = lblFactura.Text; // factura

                    string A1 = dr.Cells[0].Value.ToString() ?? ""; // articulo
                    string A2 = dr.Cells[2].Value.ToString() ?? ""; // cantidad
                    string A3 = dr.Cells[3].Value.ToString() ?? ""; // precio
                    string A4 = dr.Cells[4].Value.ToString() ?? ""; // impuesto
                    string A5 = dr.Cells[5].Value.ToString() ?? ""; // monto facturado
                    string A6 = dr.Cells[6].Value.ToString() ?? ""; // almacen

                    cmd.Parameters.AddWithValue("@A0", A1); // artic
                    cmd.Parameters.AddWithValue("@A1", A2); // cantidad
                    cmd.Parameters.AddWithValue("@A3", A3); // precio
                    cmd.Parameters.AddWithValue("@A4", A4); // impuesto
                    cmd.Parameters.AddWithValue("@A5", A5); // monto
                    cmd.Parameters.AddWithValue("@A6", A6); // almacen
                    cmd.Parameters.AddWithValue("@A6", txtCliente.Text);
                    cmd.Parameters.AddWithValue("@A7", lblFechaFactura.Text);
                    cmd.Parameters.AddWithValue("@A8", "1");

                    cmd.ExecuteNonQuery();

                    RebajaInventario(A1, A3, A6);
                }
            }
        }

        private void InsertarMvto()
        {
            string stQueri =
                "INSERT INTO MVTOCTE (IDCLIENTE, FECHA, DOCUMENTO, APLICADO, ORIGEN, MONTO, " +
                " BCEPENDIENTE, ACTIVO, TIPDOC ) " +
                "VALUES (@A0, @A1, @A2, @A3, @A4, @A5, @A6, @A7, @A8);";

            SqlConnection cnt = new SqlConnection(cnn.db); cnt.Open();
            SqlCommand cmd = new SqlCommand(stQueri, cnt);

            cmd.Parameters.AddWithValue("@A0", txtCliente.Text);
            cmd.Parameters.AddWithValue("@A1", lblFechaFactura.Text);
            cmd.Parameters.AddWithValue("@A2", lblFactura.Text);
            cmd.Parameters.AddWithValue("@A3", "FT000000");
            cmd.Parameters.AddWithValue("@A4", "1");
            cmd.Parameters.AddWithValue("@A5", lblTotal.Text);
            cmd.Parameters.AddWithValue("@A6", lblTotal.Text);
            cmd.Parameters.AddWithValue("@A7", "1");
            cmd.Parameters.AddWithValue("@A8", "FT");

            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cnt.Close();
        }

        private void RebajaInventario(string articulo, string cant, string alm)
        {
            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();
                string query = @"UPDATE PRODUCTOS SET STOCK = STOCK - @A1 WHERE IDPRODUCTO = @A2";

                SqlCommand cmd = new SqlCommand(query, cxn);
                cmd.Parameters.AddWithValue("@A1", cant);
                cmd.Parameters.AddWithValue("@A2", articulo);
                cmd.ExecuteNonQuery();

                string queri = @"UPDATE ALMACENSTOCK SET STOCK = STOCK - @A1 WHERE IDPRODUCTO = @A2 AND IDALMACEN = @A2";

                SqlCommand cmo = new SqlCommand(query, cxn);
                cmo.Parameters.AddWithValue("@A1", cant);
                cmo.Parameters.AddWithValue("@A2", articulo);
                cmo.Parameters.AddWithValue("@A3", alm);
                cmo.ExecuteNonQuery();
            }
        }

        private void ActualizaSecuenciaFactura(string factura)
        {
            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();
                string query = @"UPDATE SECUENCIAS SET CONTADOR = @A1 WHERE ID = @A2";

                SqlCommand cmd = new SqlCommand(query, cxn);
                cmd.Parameters.AddWithValue("@A1", factura);
                cmd.Parameters.AddWithValue("@A2", "2");
                cmd.ExecuteNonQuery();
            }
        }

        private void ActualizaBalanceCliente(string cliente, string total)
        {
            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();
                string query = @"UPDATE CLIENTES SET BALANCEPENDIENTE = BALANCEPENDIENTE + @A1 WHERE IDCLIENTE = @A2";

                SqlCommand cmd = new SqlCommand(query, cxn);
                cmd.Parameters.AddWithValue("@A1", total);
                cmd.Parameters.AddWithValue("@A2", cliente);
                cmd.ExecuteNonQuery();
            }
        }


    }
}
