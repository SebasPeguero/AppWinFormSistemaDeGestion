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
    public partial class frmRecibos : Form
    {
        public frmRecibos()
        {
            InitializeComponent();
            EstiloDataGridView();
        }

        string oldValue;
        string newValor;
        string newValue;
        double nTotal;

        private void frmRecibos_Load(object sender, EventArgs e)
        {
            this.Text = "Recibos";
            this.KeyPreview = true;

            lblFecha.Text = DateTime.Now.ToString("yyyyMMdd");
            string _recibo = Busco.BuscaUltimoNumero("3");  // buscamos el ultimo recibo a realizar

            lblRecibo.Text = Convert.ToInt32(_recibo).ToString("D10"); // ahora tiene 10 posiciones
        }

        private void frmRecibos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        // --------------------------------------------------------
        // TEXTBOX
        // --------------------------------------------------------
        private void txtCliente_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtCliente.Text.Trim() != string.Empty)
                {
                    txtValor.Focus();
                }
            }
        }

        private void txtCliente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
            {
                btnVENCTE.PerformClick();
            }
        }

        private void txtCliente_Leave(object sender, EventArgs e)
        {
            if (txtCliente.Text.Trim() != string.Empty)
            {
                lblNombre.Text = Busco.BuscaNombreCliente(txtCliente.Text);

                BuscarMovimientosCliente(txtCliente.Text);
            }
        }

        private void txtValor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtValor.Text.Trim() != string.Empty)
                {
                    dgv.Focus();  // mueve el focus hacia adentro de la DatagridView
                }
            }
        }


        private void txtValor_Leave(object sender, EventArgs e)
        {
            string texto = txtValor.Text.Trim();

            if (string.IsNullOrEmpty(texto))
                return;

            string textoNormalizado = texto.Replace(',', '.');

            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese una cantidad válida.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtValor.Focus();
                return;
            }

            txtValor.Text = valor.ToString("N2");
        }

        // ---------------------------------------------------------------------
        // MANEJO DE LA GRILLA
        // ---------------------------------------------------------------------
        private void dgv_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 4)
            {
                return;
            }
            // toma el valor de la celda
            oldValue = (string)dgv[e.ColumnIndex, e.RowIndex].Value;
        }

        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 4)
            {
                return;
            }
            // actualiza el nuevo valor
            newValor = (string)dgv[e.ColumnIndex, e.RowIndex].Value;

            if (newValor != string.Empty)
            {
                newValue = (string)dgv[4, e.RowIndex].Value;
                ActualizaTotalRecibo();
            }
            else
            {
                dgv[e.ColumnIndex, e.RowIndex].Value = oldValue;  // si presionaste ESC coloca el valor anterior
            }
        }


        // --------------------------------------------------------
        // BOTONES
        // --------------------------------------------------------
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Convertimos a números para comparar matemáticamente
            decimal.TryParse(lblTotal.Text, out decimal totalGrid);
            decimal.TryParse(txtValor.Text, out decimal totalPago);

            // Comparamos los números, no el texto
            if (totalGrid == totalPago && totalPago > 0)
            {
                InsertaMvtoCliente();

                // ... resto de tu código de reportes ...

                BorrarTablasReporte();
                LlenarReciboHeaderReporte();
                LlenarReciboDetailsReporte();

                MessageBox.Show("Recibo guardado correctamente.");
                btnLimpiar.PerformClick();
            }
            else
            {
                MessageBox.Show("Hay diferencia entre el monto recibido (" + totalPago.ToString("N2") + ") y las facturas seleccionadas (" + totalGrid.ToString("N2") + ")",
                    "Error de Cuadre", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            txtCliente.Focus();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnVENCTE_Click(object sender, EventArgs e)
        {
            frmVENCTE frm = new frmVENCTE();
            frm.ShowDialog();

            if (frm.EData == true)
            {
                txtCliente.Text = frm.var1;
                lblNombre.Text = frm.var2;

                BuscarMovimientosCliente(txtCliente.Text);
            }
        }



        // --------------------------------------------------------
        // METODOS
        // --------------------------------------------------------
        private void LimpiarFormulario()
        {
            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            string _recibo = Busco.BuscaUltimoNumero("2");

            lblRecibo.Text = Convert.ToInt32(_recibo).ToString("D10"); // ahora tiene 8 posiciones

            txtCliente.Clear();
            lblNombre.Text = "";
            lblTotal.Text = "";

            lblFecha.Text = DateTime.Now.ToString("yyyyMMdd");
        }

        private void BuscarMovimientosCliente(string nmrCliente)
        {
            // Limpiamos la grilla
            dgv.Rows.Clear();
            dgv.Refresh();

            string sQuery = @"SELECT IDCLIENTE, FECHA, TIPDOC, DOCUMENTO, APLICADO, ORIGEN, MONTO, BCEPENDIENTE 
                        FROM MVTOCTE 
                       WHERE IDCLIENTE = @A1
                         AND ORIGEN    = @A2 
                         AND ACTIVO    = 1
                    ORDER BY FECHA, DOCUMENTO ASC";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                try
                {
                    cxn.Open();
                    SqlCommand cmd = new SqlCommand(sQuery, cxn);
                    cmd.Parameters.AddWithValue("@A1", nmrCliente);

                    // CORRECCIÓN: Tu base de datos tiene 'FC', no '1'.
                    cmd.Parameters.AddWithValue("@A2", "FC");

                    SqlDataReader rsd = cmd.ExecuteReader();

                    while (rsd.Read())
                    {
                        int xRows = dgv.Rows.Add(); // Agrega fila y devuelve el índice

                        dgv[0, xRows].Value = rsd["TIPDOC"].ToString();
                        dgv[1, xRows].Value = rsd["DOCUMENTO"].ToString();

                        // Formato de fecha corto
                        if (DateTime.TryParse(rsd["FECHA"].ToString(), out DateTime fecha))
                            dgv[2, xRows].Value = fecha.ToShortDateString();
                        else
                            dgv[2, xRows].Value = rsd["FECHA"].ToString();

                        // Formato numérico
                        decimal bce = Convert.ToDecimal(rsd["BCEPENDIENTE"]);
                        dgv[3, xRows].Value = bce.ToString("N2");

                        dgv[4, xRows].Value = "0.00"; // Inicializamos en cero
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al buscar movimientos: " + ex.Message);
                }
            }
        }

        private void ActualizaTotalRecibo()
        {
            lblTotal.Text = "";
            nTotal = 0;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                double abono = Convert.ToDouble(row.Cells[4].Value.ToString());
                nTotal = nTotal + abono;

                lblTotal.Text = nTotal.ToString();
            }
        }


        private void BorrarTablasReporte()
        {
            string _rxs = "S";

            if (_rxs == "S")
            {
                string stQuery = "DELETE FROM ReciboHeader";
                OleDbConnection cxn = new OleDbConnection(cnn.ac); cxn.Open();
                OleDbCommand cmd = new OleDbCommand(stQuery, cxn);

                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            if (_rxs == "S")
            {
                string stQuery = "DELETE FROM MVTOcte";
                OleDbConnection cxn = new OleDbConnection(cnn.ac); cxn.Open();
                OleDbCommand cmd = new OleDbCommand(stQuery, cxn);

                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }

        private void LlenarReciboHeaderReporte()
        {
            string stQuery = "INSERT INTO RECIBOHEADER ( RECIBO, CLIENTE, NOMBRE, FECHA, MONTO ) VALUES (@A1, @A2, @A3, @A4, @A5 )";
            OleDbConnection cxn = new OleDbConnection(cnn.ac); cxn.Open();
            OleDbCommand cmd = new OleDbCommand(stQuery, cxn);

            cmd.Parameters.AddWithValue("@A1", lblRecibo.Text);
            cmd.Parameters.AddWithValue("@A2", txtCliente.Text);
            cmd.Parameters.AddWithValue("@A3", lblNombre.Text);
            cmd.Parameters.AddWithValue("@A4", lblFecha.Text);
            cmd.Parameters.AddWithValue("@A5", txtValor.Text);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        private void LlenarReciboDetailsReporte()
        {
            // Query para Access (OleDb)
            string stQuery = "INSERT INTO MVTOCTE (IDCLIENTE, FECHA, DOCUMENTO, APLICADO, ORIGEN, MONTO, BCEPENDIENTE, TIPDOC ) " +
                             "VALUES ( @A1, @A2, @A3, @A4, @A5, @A6, @A7, @A8 )";

            using (OleDbConnection cxn = new OleDbConnection(cnn.ac))
            {
                cxn.Open();

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue; // Saltamos fila vacía

                    string nmDoc = row.Cells[0].Value != null ? row.Cells[0].Value.ToString() : "";

                    // USAMOS LA CELDA 4 (Valor a Pagar), NO LA 3
                    string valorPagoStr = row.Cells[4].Value != null ? row.Cells[4].Value.ToString() : "0";

                    // Convertimos a decimal para evitar errores si está vacío
                    decimal.TryParse(valorPagoStr, out decimal montoPago);

                    using (OleDbCommand cmd = new OleDbCommand(stQuery, cxn))
                    {
                        cmd.Parameters.AddWithValue("@A1", txtCliente.Text);
                        cmd.Parameters.AddWithValue("@A2", lblFecha.Text);
                        cmd.Parameters.AddWithValue("@A3", nmDoc);
                        cmd.Parameters.AddWithValue("@A4", lblRecibo.Text);
                        cmd.Parameters.AddWithValue("@A5", "C"); // En el reporte usas 'C'

                        // Pasamos el número (Double/Decimal) para que Access no de error de tipos
                        cmd.Parameters.AddWithValue("@A6", (double)montoPago);

                        cmd.Parameters.AddWithValue("@A7", "0");
                        cmd.Parameters.AddWithValue("@A8", "RC");

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void InsertaMvtoCliente()
        {
            string stQuery = "INSERT INTO MVTOCTE (IDCLIENTE, FECHA, DOCUMENTO, APLICADO, ORIGEN, MONTO, BCEPENDIENTE, TIPDOC ) " +
                             "VALUES ( @A1, @A2, @A3, @A4, @A5, @A6, @A7, @A8 )";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    // 1. Evitar leer la fila vacía de "nuevo registro" si existe
                    if (row.IsNewRow) continue;

                    // 2. Obtener valores y validarlos
                    string nmDoc = row.Cells[0].Value != null ? row.Cells[0].Value.ToString() : "";

                    // LOGICA: ¿De dónde sacamos el pago? 
                    // En tu evento DoubleClick copiaste el saldo a la celda 4.
                    // Si quieres guardar LO QUE SE VA A PAGAR, deberías leer la celda 4, no la 3.
                    // Asumiré que la Celda 4 es el "Monto a Pagar".
                    string valorCeldaPago = row.Cells[4].Value != null ? row.Cells[4].Value.ToString() : "0";

                    // 3. CONVERSIÓN SEGURA A NÚMERO
                    decimal montoPago = 0;
                    // Intentamos convertir. Si falla (por ejemplo texto vacío), se queda en 0.
                    decimal.TryParse(valorCeldaPago, out montoPago);

                    // Si el pago es 0, quizás no quieras guardarlo. Puedes poner un if(montoPago > 0) aquí.

                    using (SqlCommand cmd = new SqlCommand(stQuery, cxn))
                    {
                        cmd.Parameters.AddWithValue("@A1", txtCliente.Text);
                        cmd.Parameters.AddWithValue("@A2", lblFecha.Text); // Asegúrate que en SQL esto sea Date o DateTime
                        cmd.Parameters.AddWithValue("@A3", nmDoc);
                        cmd.Parameters.AddWithValue("@A4", lblRecibo.Text);
                        cmd.Parameters.AddWithValue("@A5", "2");

                        // AQUÍ ESTABA EL ERROR: Ahora enviamos un decimal, no un string
                        cmd.Parameters.AddWithValue("@A6", montoPago);

                        cmd.Parameters.AddWithValue("@A7", 0); // Pasamos 0 como número, no "0" texto
                        cmd.Parameters.AddWithValue("@A8", "RC");

                        cmd.ExecuteNonQuery();
                    }

                    // OJO: Si cambiaste a usar la celda 4 (pago), asegúrate de pasar el valor correcto aquí también
                    ActualizaBalanceDocumento(txtCliente.Text, nmDoc, montoPago.ToString());
                    CambiarEstatusDocumento(txtCliente.Text, nmDoc);
                }
            } // El using cierra la conexión automáticamente

            ActualizaBalanceCliente(txtCliente.Text, lblTotal.Text);
            ActualizaUltimoSecuenciaRecibo(lblRecibo.Text);
        }

        private void ActualizaUltimoSecuenciaRecibo(string numSecuencia)
        {
            string sQuery = "UPDATE SECUENCIAS " +
                            "   SET CONTADOR ='" + numSecuencia +
                            "' FROM SECUENCIAS " +
                            " WHERE ID ='2'";

            SqlConnection cxn = new SqlConnection(cnn.db); cxn.Open();
            SqlCommand cmd = new SqlCommand(sQuery, cxn);
            cmd.ExecuteNonQuery();
            cxn.Close();
        }

        private void CambiarEstatusDocumento(string nmrCliente, string nmrDocu)
        {
            // 1. Verificamos si el balance llegó a 0
            string sQuery = @"SELECT BCEPENDIENTE 
                        FROM MVTOCTE 
                       WHERE IDCLIENTE = @cte 
                         AND DOCUMENTO = @doc 
                         AND ORIGEN    = 'FC'"; // Cambiado a FC

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();
                SqlCommand cmd = new SqlCommand(sQuery, cxn);
                cmd.Parameters.AddWithValue("@cte", nmrCliente);
                cmd.Parameters.AddWithValue("@doc", nmrDocu);

                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    decimal bce = Convert.ToDecimal(result);

                    // 2. Si es 0, desactivamos la factura (ACTIVO = 0)
                    if (bce <= 0)
                    {
                        string uQuery = @"UPDATE MVTOCTE 
                                     SET ACTIVO = 0 
                                   WHERE IDCLIENTE = @cte 
                                     AND DOCUMENTO = @doc 
                                     AND ORIGEN    = 'FC'";

                        SqlCommand cmdUpdate = new SqlCommand(uQuery, cxn);
                        cmdUpdate.Parameters.AddWithValue("@cte", nmrCliente);
                        cmdUpdate.Parameters.AddWithValue("@doc", nmrDocu);

                        cmdUpdate.ExecuteNonQuery();
                    }
                }
            }
        }


        private void ActualizaBalanceCliente(string numCliente, string TotalReciboCliente)
        {
            // CORRECCIÓN:
            // 1. Usamos una sintaxis UPDATE limpia (sin el FROM redundante).
            // 2. Usamos @A1 y @A2 para pasar los datos de forma segura.
            string sQuery = @"UPDATE CLIENTES 
                         SET BALANCEPENDIENTE = BALANCEPENDIENTE - @A1 
                       WHERE IDCLIENTE = @A2";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                try
                {
                    cxn.Open();
                    SqlCommand cmd = new SqlCommand(sQuery, cxn);

                    // IMPORTANTE: Convertimos el string a Decimal aquí mismo.
                    // Si TotalReciboCliente viene vacío, esto podría fallar, así que ponemos una protección.
                    decimal monto = 0;
                    decimal.TryParse(TotalReciboCliente, out monto);

                    cmd.Parameters.AddWithValue("@A1", monto);       // El monto a restar
                    cmd.Parameters.AddWithValue("@A2", numCliente);  // El ID del cliente (ej: CL001)

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar el balance del cliente: " + ex.Message);
                }
            }
        }

        private void ActualizaBalanceDocumento(string nmrCliente, string nmrDocu, string nmrPago)
        {
            // CORRECCIÓN: Usamos parámetros (@pago) y cambiamos el origen a 'FC'
            string sQuery = @"UPDATE MVTOCTE 
                         SET BCEPENDIENTE = BCEPENDIENTE - @pago
                       WHERE IDCLIENTE = @cliente
                         AND DOCUMENTO = @docu
                         AND ORIGEN    = 'FC'"; // Ojo: Debe coincidir con tu base de datos ('FC')

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                try
                {
                    cxn.Open();
                    SqlCommand cmd = new SqlCommand(sQuery, cxn);

                    // Conversión segura del monto
                    decimal.TryParse(nmrPago, out decimal monto);

                    cmd.Parameters.AddWithValue("@pago", monto);
                    cmd.Parameters.AddWithValue("@cliente", nmrCliente);
                    cmd.Parameters.AddWithValue("@docu", nmrDocu);

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar saldo documento: " + ex.Message);
                }
            }
        }


        private void EstiloDataGridView()
        {
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.ColumnHeadersVisible = true;
            this.dgv.RowHeadersVisible = false;

            this.dgv.Columns.Add("Col00", "Tipo");
            this.dgv.Columns.Add("Col01", "Documento");
            this.dgv.Columns.Add("Col02", "Fecha");
            this.dgv.Columns.Add("Col03", "Bce Pendiente");
            this.dgv.Columns.Add("Col04", "Valor a Pagar");

            DataGridViewColumn column = dgv.Columns[00]; column.Width = 100;
            column = dgv.Columns[01]; column.Width = 150;
            column = dgv.Columns[02]; column.Width = 150;
            column = dgv.Columns[03]; column.Width = 150;
            column = dgv.Columns[04]; column.Width = 200;

            this.dgv.BorderStyle = BorderStyle.None;

            // -------------------------------------------------------------
            // AQUÍ ESTÁ EL ARREGLO DE COLORES (Letras negras)
            // -------------------------------------------------------------

            // 1. Color de letra NEGRO para que se vea
            this.dgv.DefaultCellStyle.ForeColor = Color.Black;

            // 2. Fondo BLANCO para las filas normales
            this.dgv.DefaultCellStyle.BackColor = Color.White;

            // 3. Filas Alternas (Zebra): Fondo gris claro y letra negra
            this.dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            this.dgv.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
            // -------------------------------------------------------------

            this.dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            // Configuración al seleccionar una fila
            this.dgv.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            this.dgv.DefaultCellStyle.SelectionForeColor = Color.Black; // O Color.WhiteSmoke si prefieres letra blanca al seleccionar

            this.dgv.BackgroundColor = Color.LightGray;

            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 6, 0, 6);
            this.dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.CornflowerBlue;
            this.dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }

        // Evento para que al dar DOBLE CLIC, se copie el saldo pendiente a la columna de pago
        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verificar que sea una fila válida
            if (e.RowIndex >= 0)
            {
                // Columna 3 = Balance Pendiente
                // Columna 4 = Valor a Pagar
                string saldoPendiente = dgv.Rows[e.RowIndex].Cells[3].Value.ToString();

                // Copiamos el saldo a la columna de pago
                dgv.Rows[e.RowIndex].Cells[4].Value = saldoPendiente;

                // Actualizamos el total de abajo
                ActualizaTotalRecibo();
            }
        }
    }
}
