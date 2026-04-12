using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using iTextSharp.text.pdf;

namespace PSC10SP
{
    public partial class frmProducto : Form
    {
        public frmProducto()
        {
            InitializeComponent();
            // 🔹 Desactiva el autoescalado DPI
            this.AutoScaleMode = AutoScaleMode.None;
            this.AutoScaleDimensions = new SizeF(96F, 96F); // Base estándar (100%)

            // 🔹 Fija tamaño y posición
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Normal;
            this.ClientSize = new Size(1280, 720); // Ajusta según tu diseño real

            EstiloDGV();
            EstiloDGV_Ofer();
            EstiloDGV_Oferta();
            EstiloDGV_Stock();
            EstiloDGV_Art();

        }

        bool ExisteData;

        string oCC;
        string cOF;
        string fDE;
        string fFN;
        string pOF;
        string Des;

        string suc;

        string can;

        private void frmProducto_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.Text = "Articulos/Productos";

            txtArticulo.Text = Busco.BuscaUltimoNumero("4"); //Busca el siguiente codigo de Articulo
            picProducto.Image = Image.FromFile(ruta.imagenDefinida); //Coloca la imagen segun la ruta que esta en la clase

            txtAltura.Text = "40"; //Coloca la altura maxima de las barras
            txtCopia.Text = "2"; // Cantidad de copias a imprimir

            MostrarCodigoBarra(txtArticulo.Text, txtAltura.Text); //Muestra el codigo de Barra
        }

        private void frmProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
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
            if ((int)e.KeyChar == (int)Keys.Enter) //Pregunta si presionaste la letra enter
            {
                e.Handled = true;
                if (txtArticulo.Text.Trim() != string.Empty)
                {
                    txtNombre.Focus();
                }
            }
        }

        private void txtArticulo_Leave(object sender, EventArgs e)
        {
            if (txtArticulo.Text.Trim() != string.Empty)
            {
                BuscarArticulo(txtArticulo.Text);
            }
        }


        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter) //Pregunta si presionaste la letra enter
            {
                e.Handled = true;
                if (txtNombre.Text.Trim() != string.Empty)
                {
                    txtCosto.Focus();
                }
            }
        }


        private void txtSuplidor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter) //Pregunta si presionaste la letra enter
            {
                e.Handled = true;
                if (txtNombre.Text.Trim() != string.Empty)
                {
                    txtCosto.Focus();
                }
            }
        }

        private void txtSuplidor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
            {
                btnSuplidor.PerformClick();
            }
        }

        private void txtSuplidor_Leave(object sender, EventArgs e)
        {
            if (txtSuplidor.Text.Trim() != string.Empty)
            {
                lblSuplidor.Text = Busco.BuscarSuplidor(txtSuplidor.Text, out bool found);

                if (!found)
                {
                    MessageBox.Show("Suplidor ingresado no existe.", "Suplidor no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSuplidor.Clear();
                    lblSuplidor.Text = "";
                    txtSuplidor.Focus();
                }
            }
        }

        private void btnSuplidor_Click(object sender, EventArgs e)
        {
            frmVENSUP frm = new frmVENSUP();
            frm.ShowDialog();

            if (frm.EData == true)
            {
                txtSuplidor.Text = frm.var1;
                lblSuplidor.Text = frm.var2;
            }
        }



        private void txtCosto_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtCosto.Text.Trim() != string.Empty)
                {
                    txtPorciento.Focus();
                }
                return;

            }
            //Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                //permitido
                return;
            }

            //Permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtCosto.Text.Contains('.')
                && !txtCosto.Text.Contains(','))
            {
                return;
            }
            e.Handled = true;
        }

        private void txtCosto_Leave(object sender, EventArgs e)
        {

            string texto = txtCosto.Text.Trim();
            if (string.IsNullOrEmpty(texto))
                return;


            // reemplaza ', por '.'
            string textoNormalizado = texto.Replace(',', '.');
            // Intentar convertir a decimal con punto como Separador 
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCosto.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura Local
            txtCosto.Text = valor.ToString("N2"); // 123, 123.00
        }

        private void txtPorciento_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtPorciento.Text.Trim() != string.Empty)
                {
                    txtBarcode.Focus();
                }
                return;

            }
            //Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                //permitido
                return;
            }

            //Permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtPorciento.Text.Contains('.')
                && !txtPorciento.Text.Contains(','))
            {
                return;
            }
            e.Handled = true;
        }

        private void txtBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter) //Pregunta si presionaste la letra enter
            {
                e.Handled = true;
                if (txtBarcode.Text.Trim() != string.Empty)
                {
                    txtReorden.Focus();
                }
            }
        }


        private void txtReorden_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtReorden.Text.Trim() != string.Empty)
                {
                    txtPrecioA.Focus();
                }
                return;

            }
            //Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                //permitido
                return;
            }

            //Permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtReorden.Text.Contains('.')
                && !txtReorden.Text.Contains(','))
            {
                return;
            }
            e.Handled = true;
        }


        private void txtPrecioA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtPrecioA.Text.Trim() != string.Empty)
                {
                    txtPrecioB.Focus();
                }
                return;

            }
            //Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                //permitido
                return;
            }

            //Permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtPrecioA.Text.Contains('.')
                && !txtPrecioA.Text.Contains(','))
            {
                return;
            }
            e.Handled = true;
        }

        private void txtPrecioB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtPrecioB.Text.Trim() != string.Empty)
                {
                    txtPrecioC.Focus();
                }
                return;

            }
            //Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                //permitido
                return;
            }

            //Permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtPrecioB.Text.Contains('.')
                && !txtPrecioB.Text.Contains(','))
            {
                return;
            }
            e.Handled = true;
        }

        private void txtPrecioC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtPrecioC.Text.Trim() != string.Empty)
                {
                    txtPrecioD.Focus();
                }
                return;

            }
            //Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                //permitido
                return;
            }

            //Permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtPrecioC.Text.Contains('.')
                && !txtPrecioC.Text.Contains(','))
            {
                return;
            }
            e.Handled = true;
        }

        private void txtPrecioD_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtPrecioD.Text.Trim() != string.Empty)
                {
                    txtPrecioE.Focus();
                }
                return;

            }
            //Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                //permitido
                return;
            }

            //Permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtPrecioD.Text.Contains('.')
                && !txtPrecioD.Text.Contains(','))
            {
                return;
            }
            e.Handled = true;
        }

        private void txtPrecioE_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtPrecioE.Text.Trim() != string.Empty)
                {
                    txtCategoria.Focus();
                }
                return;

            }
            //Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                //permitido
                return;
            }

            //Permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtPrecioE.Text.Contains('.')
                && !txtPrecioE.Text.Contains(','))
            {
                return;
            }
            e.Handled = true;
        }


        private void txtSucursal_KeyPress(object sender, KeyPressEventArgs e)
        {

            if ((int)e.KeyChar == (int)Keys.Enter) //Pregunta si presionaste la letra enter
            {
                e.Handled = true;
                if (txtSucursal.Text.Trim() != string.Empty)
                {
                    txtCantidad.Focus();
                }
            }
        }

        private void txtSucursal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
            {
                btnSucursal.PerformClick();
            }
        }

        private void txtSucursal_Leave(object sender, EventArgs e)
        {
            if (txtSucursal.Text.Trim() != string.Empty)
            {
                // Busco.BuscarDataSuc(txtSucursal.Text);
            }
        }



        private void txtPorciento_Leave(object sender, EventArgs e)
        {
            string texto = txtPorciento.Text.Trim();
            if (string.IsNullOrEmpty(texto))
                return;


            // reemplaza ', por '.'
            string textoNormalizado = texto.Replace(',', '.');
            // Intentar convertir a decimal con punto como Separador 
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPorciento.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura Local
            txtPorciento.Text = valor.ToString("N2"); // 123, 123.00
        }



        private void txtReorden_Leave(object sender, EventArgs e)
        {
            string texto = txtReorden.Text.Trim();
            if (string.IsNullOrEmpty(texto))
                return;


            // reemplaza ', por '.'
            string textoNormalizado = texto.Replace(',', '.');
            // Intentar convertir a decimal con punto como Separador 
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtReorden.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura Local
            txtReorden.Text = valor.ToString("N2"); // 123, 123.00
        }

        private void txtPrecioA_Leave(object sender, EventArgs e)
        {
            string texto = txtPrecioA.Text.Trim();
            if (string.IsNullOrEmpty(texto))
                return;


            // reemplaza ', por '.'
            string textoNormalizado = texto.Replace(',', '.');
            // Intentar convertir a decimal con punto como Separador 
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecioA.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura Local
            txtPrecioA.Text = valor.ToString("N2"); // 123, 123.00
        }

        private void txtPrecioB_Leave(object sender, EventArgs e)
        {
            string texto = txtPrecioB.Text.Trim();
            if (string.IsNullOrEmpty(texto))
                return;


            // reemplaza ', por '.'
            string textoNormalizado = texto.Replace(',', '.');
            // Intentar convertir a decimal con punto como Separador 
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecioB.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura Local
            txtPrecioB.Text = valor.ToString("N2"); // 123, 123.00
        }

        private void txtPrecioC_Leave(object sender, EventArgs e)
        {
            string texto = txtPrecioC.Text.Trim();
            if (string.IsNullOrEmpty(texto))
                return;


            // reemplaza ', por '.'
            string textoNormalizado = texto.Replace(',', '.');
            // Intentar convertir a decimal con punto como Separador 
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecioC.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura Local
            txtPrecioC.Text = valor.ToString("N2"); // 123, 123.00
        }



        private void txtPrecioD_Leave(object sender, EventArgs e)
        {
            string texto = txtPrecioD.Text.Trim();
            if (string.IsNullOrEmpty(texto))
                return;


            // reemplaza ', por '.'
            string textoNormalizado = texto.Replace(',', '.');
            // Intentar convertir a decimal con punto como Separador 
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecioD.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura Local
            txtPrecioD.Text = valor.ToString("N2"); // 123, 123.00
        }

        private void txtPrecioE_Leave(object sender, EventArgs e)
        {
            string texto = txtPrecioE.Text.Trim();
            if (string.IsNullOrEmpty(texto))
                return;


            // reemplaza ', por '.'
            string textoNormalizado = texto.Replace(',', '.');
            // Intentar convertir a decimal con punto como Separador 
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecioE.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura Local
            txtPrecioE.Text = valor.ToString("N2"); // 123, 123.00
        }

        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtCantidad.Text.Trim() != string.Empty)
                {
                    btnInsertar.Focus();
                }
                return;

            }
            //Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                //permitido
                return;
            }

            //Permite un solo punto o coma decimal, pero no ambos
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
            if (txtSucursal.Text.Trim() != string.Empty)
            {
                btnInsertar.PerformClick();
            }
        }


        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgv.RowCount > 0)
                btnEditarLinea.PerformClick();
        }

        private void txtAltura_KeyPress(object sender, KeyPressEventArgs e)
        {

            if ((int)e.KeyChar == (int)Keys.Enter) //Pregunta si presionaste la letra enter
            {
                e.Handled = true;
                if (txtAltura.Text.Trim() != string.Empty)
                {
                    txtCopia.Focus();
                }
            }
        }

        private void txtAltura_Leave(object sender, EventArgs e)
        {
            if (txtAltura.Text.Trim() != string.Empty)
            {
                btnActualiza.PerformClick();
            }
        }

        private void txtCopia_KeyPress(object sender, KeyPressEventArgs e)
        {

            if ((int)e.KeyChar == (int)Keys.Enter) //Pregunta si presionaste la letra enter
            {
                e.Handled = true;
                if (txtCopia.Text.Trim() != string.Empty)
                {
                    btnImprimir.Focus();
                }
            }
        }


        // --------------------------------------------------
        // BOTONES
        // --------------------------------------------------


        private void btnInsertar_Click(object sender, EventArgs e)
        {
            if (txtSucursal.Text.Trim() != "")
            {
                if (txtCantidad.Text.Trim() != "")
                {
                    AgregarLinea();
                    ContarCantidades();

                    LimpiarDetalle();
                    txtSucursal.Focus();
                }
            }
        }

        private void btnLimpiarDetalle_Click(object sender, EventArgs e)
        {
            LimpiarDetalle();
            txtSucursal.Focus();
        }



        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario(); //Limpia el Formulario
            LimpiarDetalle();
            txtArticulo.Text = Busco.BuscaUltimoNumero("0"); // busca el siguiente registro a utilizar
            txtArticulo.Focus();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarMaestros())
                return;

            if (!ValidarFormulario())
                return;

            if (!ExisteData)
            {
                InsertarArticulo(); //Inserta el nuevo registro
                GuardarInventario(txtArticulo.Text, txtReorden.Text);
                GuardarLasOfertas(txtArticulo.Text);
                ActualizaSecuenciaArticulo("4", txtArticulo.Text); //Actualiza la NUEVA secuencia
            }
            else
            {
                ActualizarArticulo();
                GuardarInventario(txtArticulo.Text, txtReorden.Text);
                GuardarLasOfertas(txtArticulo.Text);
            }

            btnLimpiar.PerformClick();
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            if (ExisteData == true)
            {
                DialogResult dialogResult = MessageBox.Show("¿Deseas Borrar el Registro?", "ITLA",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialogResult == DialogResult.Yes)
                {
                    BorrarRegistro(txtArticulo.Text);
                    btnLimpiar.PerformClick();
                }
            }

        }


        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void btnArticulo_Click(object sender, EventArgs e)
        {
            frmVENART frm = new frmVENART();
            frm.ShowDialog();

            if (frm.EData == true)
            {
                txtArticulo.Text = frm.var1;
                BuscarArticulo(txtArticulo.Text);
            }
        }

        private void btnSucursal_Click(object sender, EventArgs e)
        {
            frmVENSUC frm = new frmVENSUC();
            frm.ShowDialog();

            if (frm.EData == true)
            {
                txtSucursal.Text = frm.var1;
                lblSucursal.Text = frm.var2;
                //lblSucursal.Text = Busco.BuscarDataSuc(txtSucursal.Text);
            }
        }


        private void btnBorrarLinea_Click(object sender, EventArgs e)
        {
            BorrarUnaLineaDgv();
            ContarCantidades();

            LimpiarDetalle();
            txtSucursal.Focus();

        }

        private void btnEditarLinea_Click(object sender, EventArgs e)
        {
            LimpiarDetalle();

            txtSucursal.Text = dgv.CurrentRow.Cells[0].Value.ToString();
            lblSucursal.Text = dgv.CurrentRow.Cells[1].Value.ToString();
            txtCantidad.Text = dgv.CurrentRow.Cells[2].Value.ToString();

            BorrarUnaLineaDgv();
            ContarCantidades();
            txtSucursal.Focus();
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();

        }

        private void btnActualiza_Click(object sender, EventArgs e)
        {
            if (txtAltura.Text.Trim() != string.Empty)
            {
                long altura = long.Parse(txtAltura.Text);
                string barra = txtArticulo.Text.ToString();

                picBarCode.SizeMode = PictureBoxSizeMode.CenterImage;
                picBarCode.BackColor = Color.White;
                picBarCode.Image = Code128(barra, PrintTextInCode: true, Height: altura);
            }
        }


        private void picArticulo_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string _imagen = openFileDialog1.FileName;
                picProducto.Image = Image.FromFile(_imagen);
            }
        }

        // --------------------------------------------------
        // BARCODE
        // --------------------------------------------------
        private void MostrarCodigoBarra(string _codigo, string _altura)
        {
            long nHeight = long.Parse(_altura);
            string sTexto = Convert.ToString(_codigo);
            picBarCode.SizeMode = PictureBoxSizeMode.CenterImage;
            picBarCode.BackColor = Color.White;
            picBarCode.Image = Code128(sTexto, PrintTextInCode: true, Height:
           nHeight);
        }
        public enum Code128SubTypes
        {
            CODE128 = iTextSharp.text.pdf.Barcode.CODE128,
            CODE128_RAW = iTextSharp.text.pdf.Barcode.CODE128_RAW,
            CODE128_UCC = iTextSharp.text.pdf.Barcode.CODE128_UCC,
        }
        public static Bitmap Code128(string _code, Code128SubTypes codeType =
       Code128SubTypes.CODE128, bool PrintTextInCode = false,

        float Height = 0, bool GenerateChecksum = true,
       bool ChecksumText = true)
        {
            if (_code.Trim() == "")
            {
                return null;
            }
            else
            {
                Barcode128 barcode = new Barcode128();
                barcode.CodeType = (int)codeType;
                barcode.StartStopText = true;
                barcode.GenerateChecksum = GenerateChecksum;
                barcode.ChecksumText = ChecksumText;
                if (Height != 0) barcode.BarHeight = Height;
                barcode.Code = _code;
                try
                {
                    System.Drawing.Bitmap bm = new
                   System.Drawing.Bitmap(barcode.CreateDrawingImage
                    (System.Drawing.Color.Black,
                   System.Drawing.Color.White));
                    if (PrintTextInCode == false)
                    {
                        return bm;
                    }
                    else
                    {
                        Bitmap bmT;
                        bmT = new Bitmap(bm.Width, bm.Height + 14);
                        Graphics g = Graphics.FromImage(bmT);
                        g.FillRectangle(new SolidBrush(Color.White), 0, 0,
                       bm.Width, bm.Height + 14);
                        Font drawFont = new Font("Arial", 8);
                        SolidBrush drawBrush = new SolidBrush(Color.Black);
                        SizeF stringSize = new SizeF();
                        stringSize = g.MeasureString(_code, drawFont);
                        float xCenter = (bm.Width - stringSize.Width) / 2;
                        float x = xCenter;
                        float y = bm.Height;
                        StringFormat drawFormat = new StringFormat();
                        drawFormat.FormatFlags = StringFormatFlags.NoWrap;
                        g.DrawImage(bm, 0, 0);
                        g.DrawString(_code, drawFont, drawBrush, x, y,
                       drawFormat);
                        return bmT;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error Codigo De Barra Code128. Desc:"
                   + ex.Message);
                }
            }
        }




        // --------------------------------------------------
        // METODOS
        // --------------------------------------------------
        private void EstiloDGV()
        {
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.ColumnHeadersVisible = true;
            this.dgv.RowHeadersVisible = false;

            // LIMPIAMOS COLUMNAS ANTERIORES PARA EVITAR DUPLICADOS
            this.dgv.Columns.Clear();

            // DEFINICIÓN DE COLUMNAS (El orden es CRITICO)
            this.dgv.Columns.Add("ColID", "ID");                     // Índice 0: ID (Número)
            this.dgv.Columns.Add("ColNombre", "Sucursal / Almacen"); // Índice 1: Nombre (Texto)
            this.dgv.Columns.Add("ColCant", "Cantidad");             // Índice 2: Cantidad (Número)

            // CONFIGURACIÓN VISUAL
            // Columna 0 (ID) - La hacemos pequeña o invisible
            dgv.Columns[0].Width = 50;
            // dgv.Columns[0].Visible = false; // Descomenta si quieres ocultar el ID

            // Columna 1 (Nombre)
            dgv.Columns[1].Width = 430;

            // Columna 2 (Cantidad)
            dgv.Columns[2].Width = 125;
            dgv.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Estilos generales
            this.dgv.BorderStyle = BorderStyle.FixedSingle;
            this.dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            this.dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgv.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            this.dgv.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            this.dgv.BackgroundColor = Color.White;

            this.dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 6, 0, 6);
            this.dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.CornflowerBlue;
            this.dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }

        private void EstiloDGV_Stock()
        {
            this.dgvStock.EnableHeadersVisualStyles = false;
            this.dgvStock.AllowUserToAddRows = false;
            this.dgvStock.AllowUserToDeleteRows = false;
            this.dgvStock.ColumnHeadersVisible = true;
            this.dgvStock.RowHeadersVisible = true;


            this.dgvStock.Columns.Add("A", "Sucursal / Almacen");
            this.dgvStock.Columns.Add("B", "Nombre Sucursal / Almacen");
            this.dgvStock.Columns.Add("C", "Cantidad");


            DataGridViewColumn
            column = dgvStock.Columns[0]; column.Width = 160;
            column = dgvStock.Columns[1]; column.Width = 420;
            column = dgvStock.Columns[2]; column.Width = 125;
            dgvStock.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            this.dgvStock.BorderStyle = BorderStyle.FixedSingle;
            this.dgvStock.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            this.dgvStock.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvStock.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            this.dgvStock.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            this.dgvStock.BackgroundColor = Color.White;

            this.dgvStock.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.dgvStock.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 6, 0, 6);
            this.dgvStock.ColumnHeadersDefaultCellStyle.BackColor = Color.CornflowerBlue;
            this.dgvStock.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

        }

        private void EstiloDGV_Ofer()
        {
            this.dgvOfer.EnableHeadersVisualStyles = false;
            this.dgvOfer.AllowUserToAddRows = false;
            this.dgvOfer.AllowUserToDeleteRows = false;
            this.dgvOfer.ColumnHeadersVisible = true;
            this.dgvOfer.RowHeadersVisible = false;

            this.dgvOfer.Columns.Add("A", "IDPRODUCTO");
            this.dgvOfer.Columns.Add("B", "Cant Oferta");
            this.dgvOfer.Columns.Add("C", "Cant Entregar");
            this.dgvOfer.Columns.Add("D", "Fecha Desde");
            this.dgvOfer.Columns.Add("E", "Fecha Fin");
            this.dgvOfer.Columns.Add("F", "Precio Oferta");
            this.dgvOfer.Columns.Add("G", "Descuento Oferta");


            DataGridViewColumn
            column = dgvOfer.Columns[0]; column.Width = 125;
            column = dgvOfer.Columns[1]; column.Width = 125;
            column = dgvOfer.Columns[2]; column.Width = 125;

            column = dgvOfer.Columns[3]; column.Width = 125;
            column = dgvOfer.Columns[4]; column.Width = 125;
            dgvOfer.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column = dgvOfer.Columns[5]; column.Width = 125;
            dgvOfer.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


            this.dgvOfer.BorderStyle = BorderStyle.FixedSingle;
            this.dgvOfer.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            this.dgvOfer.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvOfer.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            this.dgvOfer.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            this.dgvOfer.BackgroundColor = Color.White;

            this.dgvOfer.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.dgvOfer.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 6, 0, 6);
            this.dgvOfer.ColumnHeadersDefaultCellStyle.BackColor = Color.CornflowerBlue;
            this.dgvOfer.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }

        private void EstiloDGV_Oferta()
        {
            this.dgvOferta.EnableHeadersVisualStyles = false;
            this.dgvOferta.AllowUserToAddRows = false;
            this.dgvOferta.AllowUserToDeleteRows = false;
            this.dgvOferta.ColumnHeadersVisible = true;
            this.dgvOferta.RowHeadersVisible = false;

            this.dgvOferta.Columns.Add("A", "IDPRODUCTO");
            this.dgvOferta.Columns.Add("B", "Cant Oferta");
            this.dgvOferta.Columns.Add("C", "Cant Entregar");
            this.dgvOferta.Columns.Add("D", "Fecha Desde");
            this.dgvOferta.Columns.Add("E", "Fecha Fin");
            this.dgvOferta.Columns.Add("F", "Precio Oferta");
            this.dgvOferta.Columns.Add("G", "Descuento Oferta");


            DataGridViewColumn
            column = dgvOferta.Columns[0]; column.Width = 125;
            column = dgvOferta.Columns[1]; column.Width = 125;
            column = dgvOferta.Columns[2]; column.Width = 125;

            column = dgvOferta.Columns[3]; column.Width = 125;
            column = dgvOferta.Columns[4]; column.Width = 125;
            dgvOferta.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column = dgvOferta.Columns[5]; column.Width = 125;
            dgvOferta.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            this.dgvOferta.BorderStyle = BorderStyle.FixedSingle;
            this.dgvOferta.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            this.dgvOferta.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvOferta.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            this.dgvOferta.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            this.dgvOferta.BackgroundColor = Color.White;

            this.dgvOferta.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.dgvOferta.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 6, 0, 6);
            this.dgvOferta.ColumnHeadersDefaultCellStyle.BackColor = Color.CornflowerBlue;
            this.dgvOferta.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }


        private void EstiloDGV_Art()
        {
            this.dgvArt.EnableHeadersVisualStyles = false;
            this.dgvArt.AllowUserToAddRows = false;
            this.dgvArt.AllowUserToDeleteRows = false;
            this.dgvArt.ColumnHeadersVisible = true;
            this.dgvArt.RowHeadersVisible = false;

            this.dgvArt.Columns.Add("A", "PRODUCTO");
            this.dgvArt.Columns.Add("B", "NOMBRE / DESCRIPCION");
            this.dgvArt.Columns.Add("C", "COSTO");
            this.dgvArt.Columns.Add("D", "PRECIO A");
            this.dgvArt.Columns.Add("E", "PRECIO B");
            this.dgvArt.Columns.Add("F", "PRECIO C");
            this.dgvArt.Columns.Add("G", "PRECIO D");
            this.dgvArt.Columns.Add("H", "PRECIO E");
            this.dgvArt.Columns.Add("I", "EXISTENCIA");


            DataGridViewColumn
            column = dgvArt.Columns[0]; column.Width = 125;
            column = dgvArt.Columns[1]; column.Width = 125;
            column = dgvArt.Columns[2]; column.Width = 125;

            column = dgvArt.Columns[3]; column.Width = 125;
            column = dgvArt.Columns[4]; column.Width = 125;
            dgvArt.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column = dgvArt.Columns[5]; column.Width = 125;
            dgvArt.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


            this.dgvArt.BorderStyle = BorderStyle.FixedSingle;
            this.dgvArt.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            this.dgvArt.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvArt.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            this.dgvArt.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            this.dgvArt.BackgroundColor = Color.White;

            this.dgvArt.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.dgvArt.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 6, 0, 6);
            this.dgvArt.ColumnHeadersDefaultCellStyle.BackColor = Color.CornflowerBlue;
            this.dgvArt.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }



        private void LimpiarFormulario()
        {
            txtArticulo.Clear();
            txtNombre.Clear();


            txtCosto.Clear();
            txtPorciento.Clear();
            txtBarcode.Clear();
            txtReorden.Clear();


            txtPrecioA.Clear();
            txtPrecioB.Clear();
            txtPrecioC.Clear();
            txtPrecioD.Clear();
            txtPrecioE.Clear();

            txtSucursal.Clear();
            lblSucursal.Text = "";
            txtCantidad.Clear();


            txtAltura.Clear();
            txtCopia.Clear();


            picProducto.Image = null;
            picBarCode.Image = null;

            txtAltura.Text = "40";
            txtCopia.Text = "2";

            LimpiarDetalleOfertas();

            //Limpiar el Datagridview
            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            this.dgvOfer.Rows.Clear();
            this.dgvOfer.Refresh();

        }
        private void BuscarArticulo(string _articulo)
        {
            // Limpiamos la imagen al principio por seguridad
            picProducto.Image = null;

            ExisteData = false;
            LimpiarFormulario();

            // 1. QUERY CORREGIDO:
            // - Cambié IDSUPLIDOR por SUPLIDOR
            // - Quité SUPLIDORNOMBRE (lo buscaremos después con tu clase Busco)
            string query = @"SELECT IDPRODUCTO, DESCRIPCION, COSTO, PRECIOA, PRECIOB, PRECIOC, PRECIOD, PRECIOE,
                            IMPUESTO, STOCK, BARCODE, FOTO, REORDEN, SUPLIDOR
                       FROM PRODUCTOS
                      WHERE IDPRODUCTO = @A1 AND ESTATUS = 1";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();
                SqlCommand cmd = new SqlCommand(query, cxn);
                cmd.Parameters.AddWithValue("@A1", _articulo);

                SqlDataReader rcd = cmd.ExecuteReader();

                if (rcd.Read())
                {
                    ExisteData = true;

                    txtArticulo.Text = rcd["IDPRODUCTO"].ToString();
                    txtNombre.Text = rcd["DESCRIPCION"].ToString();

                    // 2. CORRECCIÓN SUPLIDOR
                    // Primero obtenemos el ID que está en la tabla productos
                    string idSuplidor = rcd["SUPLIDOR"].ToString();
                    txtSuplidor.Text = idSuplidor;

                    // Ahora usamos tu clase 'Busco' para traer el nombre real desde la tabla Proveedores
                    bool existeProv;
                    // Nota: Si Busco.BuscarSuplidor devuelve string, úsalo así:
                    string nombreProv = Busco.BuscarSuplidor(idSuplidor, out existeProv);
                    if (existeProv)
                    {
                        lblSuplidor.Text = nombreProv;
                    }
                    else
                    {
                        lblSuplidor.Text = "No encontrado";
                    }

                    txtCosto.Text = rcd["COSTO"].ToString();
                    txtPorciento.Text = rcd["IMPUESTO"].ToString();
                    txtBarcode.Text = rcd["BARCODE"].ToString();
                    txtReorden.Text = rcd["REORDEN"].ToString();

                    txtPrecioA.Text = rcd["PRECIOA"].ToString();
                    txtPrecioB.Text = rcd["PRECIOB"].ToString();
                    txtPrecioC.Text = rcd["PRECIOC"].ToString();

                    // 3. MANEJO DE NULLS (Precios D y E eran NULL en la base de datos)
                    if (rcd["PRECIOD"] != DBNull.Value) txtPrecioD.Text = rcd["PRECIOD"].ToString();
                    else txtPrecioD.Text = "0.00";

                    if (rcd["PRECIOE"] != DBNull.Value) txtPrecioE.Text = rcd["PRECIOE"].ToString();
                    else txtPrecioE.Text = "0.00";

                    lblExistencia.Text = rcd["STOCK"].ToString();

                    // 4. LA FOTO DEBE CARGARSE DENTRO DEL IF (Solo si existe el producto)
                    if (rcd["FOTO"] != DBNull.Value)
                    {
                        try
                        {
                            picProducto.Image = ConvertImage.ByteArrayToImage((byte[])rcd["FOTO"]);
                        }
                        catch
                        {
                            // Si falla la conversión, carga la default
                            if (System.IO.File.Exists(ruta.imagenDefinida))
                                picProducto.Image = Image.FromFile(ruta.imagenDefinida);
                        }
                    }
                    else
                    {
                        if (System.IO.File.Exists(ruta.imagenDefinida))
                            picProducto.Image = Image.FromFile(ruta.imagenDefinida);
                    }

                    // Limpiar el Datagridview (Opcional, depende de tu lógica)
                    // this.dgv.Rows.Clear(); 
                }
                else
                {
                    // Si no encuentra el producto
                    MessageBox.Show("Producto no encontrado.");
                    return; // Salimos para no ejecutar lo de abajo con datos vacios
                }
            }
            // El using cierra la conexión aquí automáticamente

            // Estas funciones deben ir fuera del using pero después de cargar los datos
            if (ExisteData)
            {
                MostrarCodigoBarra(txtArticulo.Text, "50"); // Asumo que 50 es la altura o txtAltura.Text
                MostrarInventario(txtArticulo.Text);
                MostrarOfertas(txtArticulo.Text);
            }
        }

        private void MostrarInventario(string _articulo)
        {
            // Limpiamos SOLO el grid de stock (abajo izquierda)
            this.dgvStock.Rows.Clear();
            this.dgvStock.Refresh();

            // Query ajustado a tus fotos exactas:
            // Relacionamos A.IDALMACEN con B.IDSUCURSAL
            string query = @"SELECT B.IDSUCURSAL, B.NOMBREDESUCURSAL, A.STOCK
                     FROM ALMACENSTOCK A
                     INNER JOIN SUCURSALES B ON A.IDALMACEN = B.IDSUCURSAL 
                     WHERE A.IDPRODUCTO = @A1";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                try
                {
                    cxn.Open();
                    SqlCommand cmd = new SqlCommand(query, cxn);
                    cmd.Parameters.AddWithValue("@A1", _articulo);

                    SqlDataReader rcd = cmd.ExecuteReader();

                    while (rcd.Read())
                    {
                        // Obtenemos los datos de manera segura
                        string id = rcd["IDSUCURSAL"].ToString();
                        string nombre = rcd["NOMBREDESUCURSAL"].ToString();
                        string stock = rcd["STOCK"].ToString();

                        // ¡CORRECCIÓN! Usamos dgvStock, NO dgv
                        this.dgvStock.Rows.Add(id, nombre, stock);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error en Inventario: " + ex.Message);
                }
            }
        }

        private void MostrarOfertas(string _articulo)
        {

            this.dgvOfer.Rows.Clear();
            this.dgvOfer.Refresh();

            string query = @"SELECT * FROM ARTICULOS_OFERTAS WHERE IDPRODUCTO = @A1";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();

                SqlCommand cmd = new SqlCommand(query, cxn);
                cmd.Parameters.AddWithValue("@A1", _articulo);
                SqlDataReader rcd = cmd.ExecuteReader();

                while (rcd.Read())
                {
                    dgvOfer.Rows.Add();
                    int nRow = dgvOfer.Rows.Count - 1;

                    dgvOfer[0, nRow].Value = rcd["IDPRODUCTO"].ToString();
                    dgvOfer[1, nRow].Value = rcd["OfertaCantPorCompra"].ToString();
                    dgvOfer[2, nRow].Value = rcd["CantidadOfertada"].ToString();
                    dgvOfer[3, nRow].Value = rcd["FechaDesde"].ToString();
                    dgvOfer[4, nRow].Value = rcd["FechaFin"].ToString();
                    dgvOfer[5, nRow].Value = rcd["PrecioOferta"].ToString();
                    dgvOfer[6, nRow].Value = rcd["Descuento"].ToString();
                }
            }
        }

        private void AgregarLinea()
        {
            dgv.Rows.Add();
            int nRow = dgv.Rows.Count - 1;

            dgv[0, nRow].Value = txtSucursal.Text;
            dgv[1, nRow].Value = lblSucursal.Text;
            dgv[2, nRow].Value = txtCantidad.Text;
        }

        private void ContarCantidades()
        {
            double _nCant = 0;
            double _xcant = 0;

            lblExistencia.Text = "00";

            foreach (DataGridViewRow cRow in dgv.Rows)
            {
                _xcant = Convert.ToDouble(dgv.CurrentRow.Cells[2].Value);
                _nCant = _nCant + _xcant;
            }

            lblExistencia.Text = _nCant.ToString("N2");
        }

        private void LimpiarDetalle()
        {
            lblExistencia.Text = "";
            txtSucursal.Clear();
            txtCantidad.Clear();
        }


        private bool ValidarMaestros()
        {
            // 1. Validar Suplidor
            if (!string.IsNullOrWhiteSpace(txtSuplidor.Text))
            {
                lblSuplidor.Text = Busco.BuscarSuplidor(txtSuplidor.Text, out bool found);

                if (!found)
                {
                    MostrarMensaje_Maestro("suplidor / proveedor");
                    return false;
                }
            }

            // 2. Validar Categoría
            if (!string.IsNullOrWhiteSpace(txtCategoria.Text))
            {
                lblCategoria.Text = Busco.BuscarCategoria(txtCategoria.Text, out bool found);

                if (!found)
                {
                    MostrarMensaje_Maestro("categoria");
                    return false;
                }
            }

            // 3. Validar SubCategoría (Depende de que haya categoría)
            if (!string.IsNullOrWhiteSpace(txtCategoria.Text))
            {
                if (!string.IsNullOrWhiteSpace(txtSubcategoria.Text))
                {
                    lblSubcategoria.Text = Busco.BuscarSubcategoria(txtCategoria.Text, txtSubcategoria.Text, out bool found);

                    if (!found)
                    {
                        MostrarMensaje_Maestro("Subcategoria");
                        return false;
                    }
                }
            }

            return true;
        }


        private void MostrarMensaje_Maestro(string campo)
        {
            MessageBox.Show($"{campo} registro no existe", "Miapp", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }



        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(txtArticulo.Text))
            {
                MostrarMensaje("Articulo");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MostrarMensaje("Nombre");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtCosto.Text))
            {
                MostrarMensaje("Costo");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtPorciento.Text))
            {
                MostrarMensaje("% Impuesto");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtBarcode.Text))
            {
                MostrarMensaje("BarCode");
                return false;
            }


            if (string.IsNullOrWhiteSpace(txtReorden.Text))
            {
                MostrarMensaje("Reorden");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPrecioA.Text))
            {
                MostrarMensaje("Precio A");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPrecioB.Text))
            {
                MostrarMensaje("Precio B");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPrecioC.Text))
            {
                MostrarMensaje("Precio C");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPrecioD.Text))
            {
                MostrarMensaje("Precio D");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPrecioE.Text))
            {
                MostrarMensaje("Precio E");
                return false;
            }

            return true;
        }


        private void MostrarMensaje(string campo)
        {
            MessageBox.Show($"Falta el {campo}", "Miapp", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ActualizaSecuenciaArticulo(string Id, string articulo)
        {
            string query = @"UPDATE SECUENCIAS SET CONTADOR = @A2 WHERE ID = @A1";

            //UPDATE SECUENCIAS SET CONTADOR = @A2 FROM SECUENCIA WHERE ID = @A1
            //UPDATE T1 SET CONTADOR = @A2 FROM SECUENCIA T1 WHERE ID = @A1

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();

                SqlCommand cmd = new SqlCommand(query, cxn);
                cmd.Parameters.AddWithValue("@A1", Id);
                cmd.Parameters.AddWithValue("@A2", articulo);

                cmd.ExecuteNonQuery();
                cmd.Dispose();

            }
        }

        private void InsertarArticulo()
        {
            if (picProducto.Image == null)
                picProducto.Image = Image.FromFile(ruta.imagenDefinida);

            byte[] byteArray = ConvertImage.ImageToByteArray(picProducto.Image);


            string query = @"INSERT INTO PRODUCTOS ( BARCODE, IDPRODUCTO, DESCRIPCION, COSTO, PRECIOA, PRECIOB, PRECIOC, PRECIOD, PRECIOE, IMPUESTO, STOCK, REORDEN, ESTATUS, FOTO)
                             VALUES (@A1, @A2, @A3, @A4, @A5, @A6, @A7, @A8, @A9, @A10, @A11, @A12, @A13, @A14)  
                              ";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();
                SqlCommand cmd = new SqlCommand(query, cxn);
                cmd.Parameters.AddWithValue("@A1", txtBarcode.Text);
                cmd.Parameters.AddWithValue("@A2", txtArticulo.Text);
                cmd.Parameters.AddWithValue("@A3", txtNombre.Text);
                cmd.Parameters.AddWithValue("@A4", txtCosto.Text);
                cmd.Parameters.AddWithValue("@A5", txtPrecioA.Text);
                cmd.Parameters.AddWithValue("@A6", txtPrecioB.Text);
                cmd.Parameters.AddWithValue("@A7", txtPrecioC.Text);
                cmd.Parameters.AddWithValue("@A8", txtPrecioD.Text);
                cmd.Parameters.AddWithValue("@A9", txtPrecioE.Text);
                cmd.Parameters.AddWithValue("@A10", txtPorciento.Text);
                cmd.Parameters.AddWithValue("@A11", lblExistencia.Text);
                cmd.Parameters.AddWithValue("@A12", txtReorden.Text);
                cmd.Parameters.AddWithValue("@A13", "1");
                cmd.Parameters.AddWithValue("@A14", byteArray);

                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }


        private void ActualizarArticulo()
        {
            if (picProducto.Image == null)
                picProducto.Image = Image.FromFile(ruta.imagenDefinida);

            byte[] byteArray = ConvertImage.ImageToByteArray(picProducto.Image);

            // ---------------------------------------------------------
            // PASO 1: CONVERTIR LOS DATOS DE TEXTO A NÚMEROS
            // ---------------------------------------------------------
            // Usamos decimal.TryParse para evitar que el programa explote si la caja está vacía

            decimal costo = 0; decimal.TryParse(txtCosto.Text, out costo);
            decimal precioA = 0; decimal.TryParse(txtPrecioA.Text, out precioA);
            decimal precioB = 0; decimal.TryParse(txtPrecioB.Text, out precioB);
            decimal precioC = 0; decimal.TryParse(txtPrecioC.Text, out precioC);
            decimal precioD = 0; decimal.TryParse(txtPrecioD.Text, out precioD);
            decimal precioE = 0; decimal.TryParse(txtPrecioE.Text, out precioE);
            decimal impuesto = 0; decimal.TryParse(txtPorciento.Text, out impuesto);
            decimal stock = 0; decimal.TryParse(lblExistencia.Text, out stock);
            decimal reorden = 0; decimal.TryParse(txtReorden.Text, out reorden);

            // Nota: Si el IDPRODUCTO en tu base de datos es numérico (INT), 
            // también debes convertir txtArticulo.Text. Si es Varchar, déjalo como string.

            string query = @"UPDATE PRODUCTOS
                     SET BARCODE = @A1,
                         DESCRIPCION = @A3,
                         COSTO = @A4,
                         PRECIOA = @A5,
                         PRECIOB = @A6,
                         PRECIOC = @A7,
                         PRECIOD = @A8,
                         PRECIOE = @A9,
                         IMPUESTO = @A10,  
                         STOCK = @A11,
                         REORDEN = @A12,
                         FOTO = @A14
                   WHERE IDPRODUCTO = @A2";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();
                SqlCommand cmd = new SqlCommand(query, cxn);

                cmd.Parameters.AddWithValue("@A1", txtBarcode.Text);
                cmd.Parameters.AddWithValue("@A2", txtArticulo.Text); // ID del producto
                cmd.Parameters.AddWithValue("@A3", txtNombre.Text);   // Descripción/Nombre

                // PASO 2: ENVIAR LAS VARIABLES NUMÉRICAS, NO LOS TEXTBOX
                cmd.Parameters.AddWithValue("@A4", costo);
                cmd.Parameters.AddWithValue("@A5", precioA);
                cmd.Parameters.AddWithValue("@A6", precioB);
                cmd.Parameters.AddWithValue("@A7", precioC);
                cmd.Parameters.AddWithValue("@A8", precioD);
                cmd.Parameters.AddWithValue("@A9", precioE);
                cmd.Parameters.AddWithValue("@A10", impuesto);
                cmd.Parameters.AddWithValue("@A11", stock);
                cmd.Parameters.AddWithValue("@A12", reorden);

                cmd.Parameters.AddWithValue("@A14", byteArray);

                cmd.ExecuteNonQuery();
            }
        }

        private void BorrarRegistro(string articulo)
        {
            string query = @"UPDATE PRODUCTO SET ESTATUS = 0 WHERE IDPRODUCTO = @A2";

            using (SqlConnection cxn = new SqlConnection())
            {
                cxn.Open();

                SqlCommand cmd = new SqlCommand(query, cxn);
                cmd.Parameters.AddWithValue("@A2", articulo);

                cmd.ExecuteNonQuery();
                cmd.Dispose();

            }
        }

        private void BorrarUnaLineaDgv()
        {
            int cuantaSon = Convert.ToInt32(dgv.RowCount);

            if (cuantaSon == 1)
                dgv.Rows.RemoveAt(dgv.RowCount - 1);
            else
                dgv.Rows.Remove(dgv.CurrentRow);
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                e.Graphics.DrawImage(picBarCode.Image, 0, 0);
            }
            catch
            {

            }

        }


        private void BorrarExistencia_Ofer(string articulo)
        {
            string queri;

            queri = @"DELETE FROM ARTICULOS_OFERTAS WHERE IDPRODUCTO = @A1";
            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();
                SqlCommand cmd = new SqlCommand(queri, cxn);
                cmd.Parameters.AddWithValue("@A1", articulo);

                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }


        private void Borrar_Stock(string articulo)
        {
            string queri;

            queri = @"DELETE FROM ALMACENSTOCK WHERE IDPRODUCTO = @A1";
            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();
                SqlCommand cmd = new SqlCommand(queri, cxn);
                cmd.Parameters.AddWithValue("@A1", articulo);

                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }


        private void GuardarInventario(string articulo, string reorden)
        {
            // 1. Borramos stock anterior para evitar duplicados
            Borrar_Stock(articulo);

            string queri = @"INSERT INTO ALMACENSTOCK (IDPRODUCTO, IDALMACEN, STOCK, REORDEN) 
                     VALUES (@A1, @A2, @A3, @A4)";

            // 2. Abrimos la conexión UNA SOLA VEZ fuera del bucle (Más rápido)
            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                try
                {
                    cxn.Open();

                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        // Saltamos la fila vacía de "nuevo registro" si existe
                        if (row.IsNewRow) continue;

                        // --- OBTENCIÓN DE DATOS SEGURA ---

                        // Leemos el ID de la SUCURSAL (Columna 0)
                        // Si es nulo, le asignamos "0" para evitar errores
                        string valorIdSucursal = row.Cells[0].Value != null ? row.Cells[0].Value.ToString() : "0";

                        // Leemos la CANTIDAD (Columna 2)
                        string valorCantidad = row.Cells[2].Value != null ? row.Cells[2].Value.ToString() : "0";

                        // --- CONVERSIÓN DE TEXTO A NÚMERO ---

                        int idSucursal = 0;
                        int.TryParse(valorIdSucursal, out idSucursal); // Convierte a entero

                        decimal cantidad = 0;
                        decimal.TryParse(valorCantidad, out cantidad); // Convierte a decimal

                        // VALIDACIÓN: Si el ID es 0, algo salió mal o la fila está vacía; no guardamos.
                        if (idSucursal == 0) continue;

                        // --- GUARDADO EN BASE DE DATOS ---
                        using (SqlCommand cmd = new SqlCommand(queri, cxn))
                        {
                            cmd.Parameters.AddWithValue("@A1", articulo);
                            cmd.Parameters.AddWithValue("@A2", idSucursal); // Ahora enviamos el NÚMERO correcto
                            cmd.Parameters.AddWithValue("@A3", cantidad);
                            cmd.Parameters.AddWithValue("@A4", reorden);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al guardar inventario: " + ex.Message);
                }
            }
        }


        private void GuardarLasOfertas(string articulo)
        {
            // 1. Limpiamos ofertas anteriores
            BorrarExistencia_Ofer(articulo);

            string queri = @"INSERT INTO ARTICULOS_OFERTAS (IDPRODUCTO, OfertaCantPorCompra, CantidadOfertada,
                     FechaDesde, FechaFin, PrecioOferta, Descuento) 
                     VALUES (@A1, @A2, @A3, @A4, @A5, @A6, @A7)";

            // 2. Abrimos conexión UNA VEZ fuera del bucle
            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();

                foreach (DataGridViewRow row in dgvOfer.Rows)
                {
                    if (row.IsNewRow) continue; // Saltamos fila vacía

                    // --- 1. OBTENER VALORES COMO TEXTO (Prevenir nulos) ---
                    string sCantCompra = row.Cells[0].Value != null ? row.Cells[0].Value.ToString() : "0";
                    string sCantOfer = row.Cells[1].Value != null ? row.Cells[1].Value.ToString() : "0";
                    string sFechaDesde = row.Cells[2].Value != null ? row.Cells[2].Value.ToString() : "";
                    string sFechaFin = row.Cells[3].Value != null ? row.Cells[3].Value.ToString() : "";
                    string sPrecio = row.Cells[4].Value != null ? row.Cells[4].Value.ToString() : "0";
                    string sDesc = row.Cells[5].Value != null ? row.Cells[5].Value.ToString() : "0";

                    // --- 2. CONVERTIR A NÚMEROS Y FECHAS REALES ---

                    // Números
                    decimal cantCompra = 0; decimal.TryParse(sCantCompra, out cantCompra);
                    decimal cantOfer = 0; decimal.TryParse(sCantOfer, out cantOfer);
                    decimal precio = 0; decimal.TryParse(sPrecio, out precio);
                    decimal desc = 0; decimal.TryParse(sDesc, out desc);

                    // FECHAS (Aquí estaba el error)
                    // Intentamos convertir el texto a Fecha. Si falla o está vacío, usamos DateTime.Now o DBNull
                    object fechaD = DBNull.Value;
                    object fechaF = DBNull.Value;

                    if (DateTime.TryParse(sFechaDesde, out DateTime dDesde))
                    {
                        fechaD = dDesde;
                    }
                    else
                    {
                        // Si la fecha no es válida, asignamos la fecha de HOY o DBNull
                        // SQL explotará si mandas "0", pero aceptará DateTime.Now
                        fechaD = DateTime.Now;
                    }

                    if (DateTime.TryParse(sFechaFin, out DateTime dFin))
                    {
                        fechaF = dFin;
                    }
                    else
                    {
                        fechaF = DateTime.Now;
                    }

                    // --- 3. ENVIAR A SQL ---
                    using (SqlCommand cmd = new SqlCommand(queri, cxn))
                    {
                        cmd.Parameters.AddWithValue("@A1", articulo);
                        cmd.Parameters.AddWithValue("@A2", cantCompra); // Enviamos número
                        cmd.Parameters.AddWithValue("@A3", cantOfer);   // Enviamos número

                        // AQUÍ LA SOLUCIÓN: Enviamos un objeto DateTime real, no un string
                        cmd.Parameters.AddWithValue("@A4", fechaD);
                        cmd.Parameters.AddWithValue("@A5", fechaF);

                        cmd.Parameters.AddWithValue("@A6", precio);     // Enviamos número
                        cmd.Parameters.AddWithValue("@A7", desc);       // Enviamos número

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        // --------------------------------------------------
        // PESTAÑA OFERTA
        // --------------------------------------------------



        private void txtOfertaCantPorCompra_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtOfertaCantPorCompra.Text.Trim() != string.Empty)
                {
                    txtCantidadOfertada.Focus();
                }
                return;

            }
            //Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                //permitido
                return;
            }

            //Permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtOfertaCantPorCompra.Text.Contains('.')
                && !txtOfertaCantPorCompra.Text.Contains(','))
            {
                return;
            }
            e.Handled = true;
        }

        private void txtOfertaCantPorCompra_Leave(object sender, EventArgs e)
        {

            string texto = txtOfertaCantPorCompra.Text.Trim();
            if (string.IsNullOrEmpty(texto))
                return;


            // reemplaza ', por '.'
            string textoNormalizado = texto.Replace(',', '.');
            // Intentar convertir a decimal con punto como Separador 
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtOfertaCantPorCompra.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura Local
            txtOfertaCantPorCompra.Text = valor.ToString("N2"); // 123, 123.00
        }

        private void txtCantidadOfertada_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtCantidadOfertada.Text.Trim() != string.Empty)
                {
                    txtFechaInicia.Focus();
                }
                return;

            }
            //Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                //permitido
                return;
            }

            //Permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtCantidadOfertada.Text.Contains('.')
                && !txtCantidadOfertada.Text.Contains(','))
            {
                return;
            }
            e.Handled = true;
        }

        private void txtCantidadOfertada_Leave(object sender, EventArgs e)
        {

            string texto = txtCantidadOfertada.Text.Trim();
            if (string.IsNullOrEmpty(texto))
                return;


            // reemplaza ', por '.'
            string textoNormalizado = texto.Replace(',', '.');
            // Intentar convertir a decimal con punto como Separador 
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCantidadOfertada.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura Local
            txtCantidadOfertada.Text = valor.ToString("N2"); // 123, 123.00
        }

        private void txtFechaInicia_KeyPress(object sender, KeyPressEventArgs e)
        {
            {
                // Si presiona Enter
                if ((int)e.KeyChar == (int)Keys.Enter)
                {
                    e.Handled = true;
                    if (txtFechaInicia.Text.Trim() != string.Empty)
                    {
                        txtFechaFin.Focus();
                    }
                    return;
                }

                // Permite dígitos y teclas de control (borrar, retroceso, etc.)
                if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
                {
                    return;
                }

                // Permite punto o coma decimal (solo uno)
                if ((e.KeyChar == '.' || e.KeyChar == ',')
                    && !txtFechaInicia.Text.Contains('.')
                    && !txtFechaInicia.Text.Contains(','))
                {
                    return;
                }

                // Si no cumple nada de lo anterior, se bloquea la tecla
                e.Handled = true;
            }
        }

        private void txtFechaInicia_Leave(object sender, EventArgs e)
        {
            string texto = txtFechaInicia.Text.Trim();

            if (string.IsNullOrEmpty(texto))
                return;

            //validar que sean exactamente 8 digitos (sin contar puntos o comas)
            string SoloDigitos = new string(texto.Where(char.IsDigit).ToArray());

            if (SoloDigitos.Length != 8)
            {
                MessageBox.Show("Debe tener exactamente 8 digitos", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFechaInicia.Focus();
                txtFechaInicia.SelectAll();
                return;
            }

            //extraer partes

            int dia = int.Parse(SoloDigitos.Substring(0, 2));
            int mes = int.Parse(SoloDigitos.Substring(0, 2));

            //validar dia y mes

            if (dia < 1 || dia > 31)
            {
                MessageBox.Show("El dia debe estar entre 1 y 31 para ser valido", "QLab", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFechaInicia.Focus();
                txtFechaInicia.SelectAll();
                return;

            }

            //si esta todo bien continua
        }

        private void txtFechaFin_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Si presiona Enter
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtFechaFin.Text.Trim() != string.Empty)
                {
                    txtPrecioOferta.Focus();
                }
                return;
            }

            // Permite dígitos y teclas de control (borrar, retroceso, etc.)
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                return;
            }

            // Permite punto o coma decimal (solo uno)
            if ((e.KeyChar == '.' || e.KeyChar == ',')
                && !txtFechaFin.Text.Contains('.')
                && !txtFechaFin.Text.Contains(','))
            {
                return;
            }

            // Si no cumple nada de lo anterior, se bloquea la tecla
            e.Handled = true;
        }

        private void txtFechaFin_Leave(object sender, EventArgs e)
        {
            string texto = txtFechaFin.Text.Trim();

            if (string.IsNullOrEmpty(texto))
                return;

            // Validar que sean exactamente 8 dígitos (sin contar puntos o comas)
            string soloDígitos = new string(texto.Where(char.IsDigit).ToArray());

            if (soloDígitos.Length != 8)
            {
                MessageBox.Show("Debe tener exactamente 8 dígitos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFechaFin.Focus();
                txtFechaFin.SelectAll();
                return;
            }

            // Extraer partes
            int día = int.Parse(soloDígitos.Substring(0, 2));    // posiciones 1-2
            int mes = int.Parse(soloDígitos.Substring(2, 2));    // posiciones 3-4

            // Validar día y mes
            if (día < 1 || día > 31)
            {
                MessageBox.Show("El día debe estar entre 01 y 31.", "QLab", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFechaFin.Focus();
                txtFechaFin.SelectAll();
                return;
            }

            if (mes < 1 || mes > 12)
            {
                MessageBox.Show("El mes debe estar entre 01 y 12.", "QLab", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFechaFin.Focus();
                txtFechaFin.SelectAll();
                return;
            }
        }

        private void txtPrecioOferta_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtPrecioOferta.Text.Trim() != string.Empty)
                {
                    txtDescuentoOferta.Focus();
                }
                return;

            }
            //Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                //permitido
                return;
            }

            //Permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtPrecioOferta.Text.Contains('.')
                && !txtPrecioOferta.Text.Contains(','))
            {
                return;
            }
            e.Handled = true;
        }

        private void txtPrecioOferta_Leave(object sender, EventArgs e)
        {
            string texto = txtPrecioOferta.Text.Trim();
            if (string.IsNullOrEmpty(texto))
                return;


            // reemplaza ', por '.'
            string textoNormalizado = texto.Replace(',', '.');
            // Intentar convertir a decimal con punto como Separador 
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecioOferta.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura Local
            txtPrecioOferta.Text = valor.ToString("N2"); // 123, 123.00
        }

        private void txtDescuentoOferta_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtDescuentoOferta.Text.Trim() != string.Empty)
                {
                    btnInsertarOferta.Focus();
                }
                return;

            }
            //Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                //permitido
                return;
            }

            //Permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtDescuentoOferta.Text.Contains('.')
                && !txtDescuentoOferta.Text.Contains(','))
            {
                return;
            }
            e.Handled = true;
        }

        private void txtDescuentoOferta_Leave(object sender, EventArgs e)
        {
            string texto = txtDescuentoOferta.Text.Trim();
            if (string.IsNullOrEmpty(texto))
                return;


            // reemplaza ', por '.'
            string textoNormalizado = texto.Replace(',', '.');
            // Intentar convertir a decimal con punto como Separador 
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescuentoOferta.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura Local
            txtDescuentoOferta.Text = valor.ToString("N2"); // 123, 123.00
        }

        private void dgvOfer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnInsertarOferta.PerformClick();
        }

        private void btnInsertarOferta_Click(object sender, EventArgs e)
        {
            if (txtFechaInicia.Text.Trim() != "")
            {
                if (txtFechaFin.Text.Trim() != "")
                {
                    AgregarLineaOferta();

                    LimpiarDetalleOfertas();
                    txtOfertaCantPorCompra.Focus();
                }
            }
        }

        private void btnLimpiarOferta_Click(object sender, EventArgs e)
        {
            LimpiarDetalleOfertas();
            txtOfertaCantPorCompra.Focus();
        }

        private void btnBorrarOferta_Click(object sender, EventArgs e)
        {
            BorrarUnaLineaDgvOfer();

            LimpiarDetalle();
            txtOfertaCantPorCompra.Focus();
        }

        private void btnEditarOferta_Click(object sender, EventArgs e)
        {
            LimpiarDetalleOfertas();

            txtOfertaCantPorCompra.Text = dgvOfer.CurrentRow.Cells[0].Value.ToString();
            txtCantidadOfertada.Text = dgvOfer.CurrentRow.Cells[1].Value.ToString();
            txtFechaInicia.Text = dgvOfer.CurrentRow.Cells[2].Value.ToString();
            txtFechaFin.Text = dgvOfer.CurrentRow.Cells[3].Value.ToString();
            txtPrecioOferta.Text = dgvOfer.CurrentRow.Cells[4].Value.ToString();
            txtDescuentoOferta.Text = dgvOfer.CurrentRow.Cells[5].Value.ToString();

            BorrarUnaLineaDgvOfer();
            txtOfertaCantPorCompra.Focus();
        }

        private void AgregarLineaOferta()
        {
            dgvOfer.Rows.Add();
            int nRow = dgv.Rows.Count - 1;

            dgvOfer[0, nRow].Value = txtOfertaCantPorCompra.Text;
            dgvOfer[1, nRow].Value = txtCantidadOfertada.Text;
            dgvOfer[2, nRow].Value = txtFechaInicia.Text;
            dgvOfer[3, nRow].Value = txtFechaFin.Text;
            dgvOfer[4, nRow].Value = txtPrecioOferta.Text;
            dgvOfer[5, nRow].Value = txtDescuentoOferta.Text;
        }

        private void LimpiarDetalleOfertas()
        {
            txtOfertaCantPorCompra.Clear();
            txtCantidadOfertada.Clear();
            txtFechaInicia.Clear();
            txtFechaFin.Clear();
            txtPrecioOferta.Clear();
            txtDescuentoOferta.Clear();
        }

        private void BorrarUnaLineaDgvOfer()
        {
            int cuantaSon = Convert.ToInt32(dgvOfer.RowCount);

            if (cuantaSon == 1)
                dgvOfer.Rows.RemoveAt(dgvOfer.RowCount - 1);
            else
                dgvOfer.Rows.Remove(dgvOfer.CurrentRow);
        }


        // --------------------------------------------------
        // PESTAÑA CONSULTA
        // --------------------------------------------------



        private void txtBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter) //Pregunta si presionaste la letra enter
            {
                e.Handled = true;
                if (txtBuscar.Text.Trim() != string.Empty) //Aqui pregunta si es diferente de vacio el txtbox
                {
                    btnBuscar.Focus(); //Mueve el cursor al siguiente txtbox
                }
            }
        }

        private void txtBuscar_Leave(object sender, EventArgs e)
        {
            btnBuscar.PerformClick();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscarData(txtBuscar.Text);
        }

        private void btnLimpiarConsulta_Click(object sender, EventArgs e)
        {
            //Limpiar el Datagridview
            this.dgvArt.Rows.Clear();
            this.dgvArt.Refresh();

            this.dgvOferta.Rows.Clear();
            this.dgvOferta.Refresh();

            this.dgvStock.Rows.Clear();
            this.dgvStock.Refresh();

        }

        private void btnEditarArticulo_Click(object sender, EventArgs e)
        {

            if (dgvArt.RowCount > 0)
            {
                
                txtArticulo.Text     = dgvArt.CurrentRow.Cells[0].Value.ToString();

                BuscarArticulo(txtArticulo.Text);

                tabControl1.SelectedTab = tabPage2;
            }
        }

        private void dgvArt_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (dgvArt.RowCount > 0)
            {

                string articulo = dgvArt.CurrentRow.Cells[0].Value.ToString();

                ConsultarInventario(articulo);
                ConsultarOferta(articulo);


            }
        }

        private void dgvArt_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnEditarArticulo.PerformClick();
        }

        private void ConsultarInventario(string _articulo)
        {
            this.dgvStock.Rows.Clear();
            this.dgvStock.Refresh();

            // CORRECCIÓN: Unimos ALMACENSTOCK con PRODUCTOS para buscar por ITEM (Texto)
            // y unimos con SUCURSALES para obtener el nombre.
            string query = @"SELECT 
                        A.IDALMACEN, 
                        S.NOMBREDESUCURSAL, 
                        A.STOCK 
                     FROM ALMACENSTOCK A
                     INNER JOIN PRODUCTOS P ON P.IDPRODUCTO = A.IDPRODUCTO
                     INNER JOIN SUCURSALES S ON S.IDSUCURSAL = A.IDALMACEN
                     WHERE P.ITEM = @itemBuscado";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                try
                {
                    cxn.Open();
                    SqlCommand cmd = new SqlCommand(query, cxn);
                    cmd.Parameters.AddWithValue("@itemBuscado", _articulo); // Aquí pasas 'S01' o 'LAP-001'

                    SqlDataReader rcd = cmd.ExecuteReader();

                    while (rcd.Read())
                    {
                        string id = rcd["IDALMACEN"].ToString();
                        string nombre = rcd["NOMBREDESUCURSAL"].ToString();
                        string stock = rcd["STOCK"].ToString();

                        this.dgvStock.Rows.Add(id, nombre, stock);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error cargando inventario: " + ex.Message);
                }
            }
        }

        private void ConsultarOferta(string _articulo)
        {
            this.dgvOferta.Rows.Clear();
            this.dgvOferta.Refresh();

            string query = @"SELECT * FROM ARTICULOS_OFERTAS WHERE IDPRODUCTO = @A1";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();

                SqlCommand cmd = new SqlCommand(query, cxn);
                cmd.Parameters.AddWithValue("@A1", _articulo);
                SqlDataReader rcd = cmd.ExecuteReader();

                while (rcd.Read())
                {
                    dgvOferta.Rows.Add();
                    int nRow = dgvOferta.Rows.Count - 1;

                    dgvOferta[0, nRow].Value = rcd["IDPRODUCTO"].ToString();
                    dgvOferta[1, nRow].Value = rcd["OfertaCantPorCompra"].ToString();
                    dgvOferta[2, nRow].Value = rcd["CantidadOfertada"].ToString();
                    dgvOferta[3, nRow].Value = rcd["FechaDesde"].ToString();
                    dgvOferta[4, nRow].Value = rcd["FechaFin"].ToString();
                    dgvOferta[5, nRow].Value = rcd["PrecioOferta"].ToString();
                    dgvOferta[6, nRow].Value = rcd["Descuento"].ToString();
                }
            }
        }


        private void BuscarData(string buscar)
        {
            //Limpiar el Datagridview
            this.dgvArt.Rows.Clear();
            this.dgvArt.Refresh();
          

            string _query = "SELECT IDPRODUCTO, DESCRIPCION, COSTO, PRECIOA, PRECIOB, PRECIOC, PRECIOD, PRECIOE, STOCK " +
                "FROM PRODUCTOS A WHERE ESTATUS = 1 AND DESCRIPCION LIKE '%" + buscar + "%' ORDER BY DESCRIPCION ASC";

            SqlConnection cxn = new SqlConnection(cnn.db); cxn.Open();
            SqlCommand cmd = new SqlCommand(_query, cxn);
            cmd.Parameters.AddWithValue("@A2", buscar);

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                dgvArt.Rows.Add(); //se le añade una linea a la datagridview
                int nRow = dgvArt.Rows.Count - 1; //obtiene la linea anterior

                dgvArt[0, nRow].Value = rdr["IDPRODUCTO"].ToString();
                dgvArt[1, nRow].Value = rdr["DESCRIPCION"].ToString();
                dgvArt[2, nRow].Value = rdr["COSTO"].ToString();
                dgvArt[3, nRow].Value = rdr["PRECIOA"].ToString();
                dgvArt[4, nRow].Value = rdr["PRECIOB"].ToString();
                dgvArt[5, nRow].Value = rdr["PRECIOC"].ToString();
                dgvArt[6, nRow].Value = rdr["PRECIOD"].ToString();
                dgvArt[7, nRow].Value = rdr["PRECIOE"].ToString();
                dgvArt[8, nRow].Value = rdr["STOCK"].ToString();

                
            }
            cmd.Dispose();
            cxn.Close();
        }


        private void txtCategoria_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if(txtCategoria.Text.Trim() != string.Empty)
                {
                    txtSubcategoria.Focus();
                }
            }
        }

        private void txtCategoria_Leave(object sender, EventArgs e)
        {
            {
                if (!string.IsNullOrWhiteSpace(txtCategoria.Text))
                {
                    lblCategoria.Text = Busco.BuscarCategoria(txtCategoria.Text, out bool found);

                    if (!found)
                    {
                        MessageBox.Show("La Categoria ingresada no existe.", "Categoria no encontrada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtSuplidor.Clear();
                        lblSuplidor.Text = "";
                        txtSuplidor.Focus();
                    }
                }
            }
        }

        private void txtCategoria_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.F4)
                btnCategoria.PerformClick();
        }

        private void btnCategorial_Click(object sender, EventArgs e)
        {
            frmVENCAT frm = new frmVENCAT();
            frm.ShowDialog();

            if (frm.EData == true)
            {
                txtCategoria.Text = frm.var1;
                lblCategoria.Text = frm.var2;
            }
        }

        private void btnSubcategoria_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
                btnSubCategoria.PerformClick();
        }

        private void btnSubcategoria_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtSubcategoria.Text.Trim() != string.Empty)
                {
                    txtSucursal.Focus();
                }
            }
        }

        private void btnSubcategoria_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSubcategoria.Text))
            {
                lblCategoria.Text = Busco.BuscarSubcategoria(txtCategoria.Text, txtSubcategoria.Text, out bool found);

                if (!found)
                {
                    MessageBox.Show("La subcategoria ingresada no existe.", "Subcategoria no encontrada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSuplidor.Clear();
                    lblSuplidor.Text = "";
                    txtSuplidor.Focus();
                }
            }
        }

        private void btnSubcategoria_Click(object sender, EventArgs e)
        {
            frmVENSUBCAT frm = new frmVENSUBCAT();
            frm.ShowDialog();

            if (frm.EData == true)
            {
                txtSubcategoria.Text = frm.var1;
                lblSubcategoria.Text = frm.var2;
            }
        }

    }
}
