using iTextSharp.text.html.simpleparser;
using PSC10SP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PSC10
{
    public partial class frmClientes : Form
    {
        public frmClientes()
        {
            InitializeComponent();
        }

        bool ExisteData;

        private void frmClientes_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.Text = "Clientes";

            txtCliente.Text = Busco.BuscaUltimoNumero("1");
            LlenarCombox();
            picCliente.Image = Image.FromFile(ruta.imagenDefinida);

        }

        private void frmClientes_KeyDown(object sender, KeyEventArgs e)
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
                    cboTipo.Focus();
                }
            }
        }

        private void txtCliente_Leave(object sender, EventArgs e)
        {
            if (txtCliente.Text.Trim() != string.Empty)
            {
                BuscarCliente(txtCliente.Text);
            }
        }

        private void txtDocumento_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtCliente.Text.Trim() != string.Empty)
                {
                    txtNombre.Focus();
                }
            }
        }

        private void txtDocumento_Leave(object sender, EventArgs e)
        {

        }

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtNombre.Text.Trim() != string.Empty)
                {
                    txtDireccion.Focus();
                }
            }
        }

        private void txtDireccion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtDireccion.Text.Trim() != string.Empty)
                {
                    txtSector.Focus();
                }
            }
        }

        private void txtSector_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtSector.Text.Trim() != string.Empty)
                {
                    txtCiudad.Focus();
                }
            }
        }

        private void txtCiudad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtCiudad.Text.Trim() != string.Empty)
                {
                    txtTelefono.Focus();
                }
            }
        }

        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Solo números y teclas de control (Backspace, Supr, etc.)
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;  // Bloquea caracteres no numéricos
            }
        }

        private void txtTelefono_Leave(object sender, EventArgs e)
        {
            string movil = txtMovil.Text.Trim();

            // Validación básica: mínimo 8 y máximo 10 dígitos
            if (movil.Length < 8 || movil.Length > 10)
            {
                MessageBox.Show("Número de teléfono inválido. Debe tener entre 8 y 10 dígitos.",
                                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTelefono.Focus();
                txtTelefono.SelectAll();
            }
        }

        private void txtMovil_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Solo números y teclas de control (Backspace, Supr, etc.)
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;  // Bloquea caracteres no numéricos
            }
        }

        private void txtMovil_Leave(object sender, EventArgs e)
        {
            string movil = txtMovil.Text.Trim();

            // Solo dígitos y debe tener entre 8 y 10 dígitos
            string patron = @"^\d{8,10}$";

            if (!Regex.IsMatch(movil, patron))
            {
                MessageBox.Show("Número de teléfono inválido. Solo números (8 a 10 dígitos).",
                                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMovil.Focus();
                txtMovil.SelectAll();
            }
        }

        private void txtCorreo_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Letras, números y caracteres permitidos
            if (char.IsLetterOrDigit(e.KeyChar) ||
                e.KeyChar == '@' ||
                e.KeyChar == '.' ||
                e.KeyChar == '_' ||
                e.KeyChar == '-' ||
                char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true; // Bloquea caracteres no válidos
            }
        }

        private void txtCorreo_Leave(object sender, EventArgs e)
        {
            string correo = txtCorreo.Text.Trim();

            // Regex para validar correo
            string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (!Regex.IsMatch(correo, patron))
            {
                MessageBox.Show("Correo inválido, por favor ingrese uno correcto.",
                                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCorreo.Focus();
                txtCorreo.SelectAll();
            }
        }

        private void txtContacto_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtContacto.Text.Trim() != string.Empty)
                {
                    txtEnvio.Focus();
                }
            }
        }

        private void txtEnvio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtEnvio.Text.Trim() != string.Empty)
                {
                    btnGuardar.Focus();
                }
            }
        }


        private void btnCliente_Click(object sender, EventArgs e)
        {
            frmVENCTE frm = new frmVENCTE();
            frm.ShowDialog();

            if (frm.EData == true)
            {
                txtCliente.Text = frm.var1;
                BuscarCliente(txtCliente.Text);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarFormulario())
                return;

            if (!ExisteData)
            {
                InsertarClientes(txtCliente.Text);  // INSERTA EL NUEVO REGISTRO
                ActualizaSecuenciaClientes("1", txtCliente.Text); // ACTUALIZA LA NUEVA SECUENCIA
            }
            else
            {
                ActualizarClientes(txtCliente.Text);
            }

            btnLimpiar.PerformClick();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            txtCliente.Text = Busco.BuscaUltimoNumero("1");
            txtCliente.Focus();
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            if (ExisteData == true)
            {
                DialogResult dialogResult = MessageBox.Show("Deseas Borrar el Registro", "ITLA",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialogResult == DialogResult.Yes)
                {
                    BorrarRegistro(txtCliente.Text);
                    btnLimpiar.PerformClick();
                }
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void picCliente_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string _imagen = openFileDialog1.FileName;
                picCliente.Image = Image.FromFile(_imagen);
            }
        }


        private void picCliente_Click_1(object sender, EventArgs e)
        {
            // Configuramos el filtro para asegurar que solo seleccionen fotos
            openFileDialog1.Filter = "Archivos de Imagen|*.jpg;*.jpeg;*.png;*.bmp";
            openFileDialog1.Title = "Seleccionar foto del cliente";

            // Abrimos la ventana de busqueda
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string _imagen = openFileDialog1.FileName;

                // Cargamos la imagen seleccionada en el cuadro del cliente
                picCliente.Image = Image.FromFile(_imagen);

                // Aseguramos que la imagen se ajuste al tamaño del cuadro
                picCliente.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        // --------------------------------------------------------------
        // METODOS
        // --------------------------------------------------------------

        private void BuscarCliente(string cliente)
        {
            ExisteData = false;

            string miQuery =
                 @"SELECT IDCLIENTE, IDENTIFICACION, NOMBRECLIENTE, DIRECCION, SECTOR, CIUDAD, TELEFONO, MOVIL, CORREO, 
                       IMPUESTOPAGA, FOTO, NIVELPRECIO, CONTACTO, ENVIOSDIRECCION, TIPODOCUMENTO, TIPOCLIENTE,
                       BALANCEPENDIENTE
                    FROM CLIENTES WHERE IDCLIENTE = @A1 AND  ESTATUS = 'A'";

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                SqlCommand cmd = new SqlCommand(miQuery, cnx);
                cmd.Parameters.AddWithValue("@A1", cliente);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    ExisteData = true;

                    txtCliente.Text = dr["IDCLIENTE"].ToString();
                    txtDocumento.Text = dr["IDENTIFICACION"].ToString();
                    txtNombre.Text = dr["NOMBRECLIENTE"].ToString();
                    txtDireccion.Text = dr["DIRECCION"].ToString();
                    txtSector.Text = dr["SECTOR"].ToString();
                    txtCiudad.Text = dr["CIUDAD"].ToString();
                    txtTelefono.Text = dr["TELEFONO"].ToString();
                    txtMovil.Text = dr["MOVIL"].ToString();
                    txtCorreo.Text = dr["CORREO"].ToString();

                    txtEnvio.Text = dr["ENVIOSDIRECCION"].ToString();
                    txtContacto.Text = dr["CONTACTO"].ToString();
                    lblBalance.Text = dr["BALANCEPENDIENTE"].ToString();

                    if (dr["NIVELPRECIO"].ToString() == "1") cboNivel.Text = "PRECIO A";
                    if (dr["NIVELPRECIO"].ToString() == "2") cboNivel.Text = "PRECIO B";
                    if (dr["NIVELPRECIO"].ToString() == "3") cboNivel.Text = "PRECIO C";
                    if (dr["NIVELPRECIO"].ToString() == "4") cboNivel.Text = "PRECIO D";
                    if (dr["NIVELPRECIO"].ToString() == "5") cboNivel.Text = "PRECIO E";

                    // Obtenemos el valor, quitamos espacios y convertimos a mayuscula para comparar mejor
                    string pagaImpuesto = dr["IMPUESTOPAGA"].ToString().Trim().ToUpper();

                    // Verificamos: 'S' (de los inserts que hicimos), '1' (por si acaso), o 'TRUE'
                    if (pagaImpuesto == "S" || pagaImpuesto == "1" || pagaImpuesto == "TRUE")
                    {
                        cboPaga.Text = "Si"; // Esto debe coincidir con lo que pusiste en los Items
                    }
                    else
                    {
                        cboPaga.Text = "No";
                    }

                    if (dr["TIPODOCUMENTO"].ToString() == "1") cboDocumento.Text = "Cedula";
                    if (dr["TIPODOCUMENTO"].ToString() == "2") cboDocumento.Text = "Pasaporte";
                    if (dr["TIPODOCUMENTO"].ToString() == "3") cboDocumento.Text = "RNC";

                    if (dr["TIPOCLIENTE"].ToString() == "1") cboTipo.Text = "Persona";
                    if (dr["TIPOCLIENTE"].ToString() == "2") cboTipo.Text = "Empresa";
                    if (dr["TIPOCLIENTE"].ToString() == "3") cboTipo.Text = "Gobierno";
                    if (dr["TIPOCLIENTE"].ToString() == "4") cboTipo.Text = "Ongs";

                    try
                    {
                        picCliente.Image = ConvertImage.ByteArrayToImage((byte[])dr["FOTO"]);
                    }
                    catch
                    {
                        if (picCliente.Image == null)
                            picCliente.Image = Image.FromFile(ruta.imagenDefinida);
                    }
                }
            }
        }

        private void BorrarRegistro(string cliente)
        {
            string query = @"UPDATE CLIENTES SET ESTATUS = 0 WHERE IDCLIENTE = @A1";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();

                SqlCommand cmd = new SqlCommand(query, cxn);
                cmd.Parameters.AddWithValue("@A2", cliente);

                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }

        private void LimpiarFormulario()
        {
            txtCliente.Clear();
            txtDocumento.Clear();
            txtNombre.Clear();
            txtDireccion.Clear();
            txtSector.Clear();
            txtCiudad.Clear();
            txtTelefono.Clear();
            txtMovil.Clear();
            txtCorreo.Clear();
            txtContacto.Clear();
            txtEnvio.Clear();
            lblBalance.Text = "";
            picCliente.Image = null;

            ExisteData = false;

            if (picCliente.Image == null)
                picCliente.Image = Image.FromFile(ruta.imagenDefinida);
        }

        private void InsertarClientes(string cliente)
        {
            if (picCliente.Image == null)
                picCliente.Image = Image.FromFile(ruta.imagenDefinida);

            byte[] byteArray = ConvertImage.ImageToByteArray(picCliente.Image);

            string query = @"INSERT INTO CLIENTES ( IDCLIENTE, NOMBRECLIENTE, IDENTIFICACION, TIPOCLIENTE, DIRECCION, SECTOR,
                             CIUDAD, TELEFONO, MOVIL, CORREO, ESTATUS, FOTO, ENVIOSDIRECCION, BALANCEPENDIENTE, NIVELPRECIO, 
                             TIPODOCUMENTO, IMPUESTOPAGA )
                             VALUES (@A1, @A2, @A3, @A4, @A5, @A6, @A7, @A8, @A9, @A10, @A11, @A12, @A13, @A14, @A15, @A16, @A17)";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();
                SqlCommand cmd = new SqlCommand(query, cxn);

                cmd.Parameters.AddWithValue("@A1", txtCliente.Text);
                cmd.Parameters.AddWithValue("@A2", txtNombre.Text);
                cmd.Parameters.AddWithValue("@A3", txtDocumento.Text);
                cmd.Parameters.AddWithValue("@A4", ((ComboItem)cboTipo.SelectedItem).Valor);
                cmd.Parameters.AddWithValue("@A5", txtDireccion.Text);
                cmd.Parameters.AddWithValue("@A6", txtSector.Text);
                cmd.Parameters.AddWithValue("@A7", txtCiudad.Text);
                cmd.Parameters.AddWithValue("@A8", txtTelefono.Text);
                cmd.Parameters.AddWithValue("@A9", txtMovil.Text);
                cmd.Parameters.AddWithValue("@A10", txtCorreo.Text);
                cmd.Parameters.AddWithValue("@A11", "1");
                cmd.Parameters.AddWithValue("@A12", byteArray);
                cmd.Parameters.AddWithValue("@A13", txtEnvio.Text);
                cmd.Parameters.AddWithValue("@A14", lblBalance.Text);
                cmd.Parameters.AddWithValue("@A15", ((ComboItem)cboNivel.SelectedItem).Valor);
                cmd.Parameters.AddWithValue("@A16", ((ComboItem)cboDocumento.SelectedItem).Valor);
                cmd.Parameters.AddWithValue("@A17", ((ComboItem)cboPaga.SelectedItem).Valor);

                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }

        private void ActualizarClientes(string cliente)
        {
            // 1. Validación de Imagen
            if (picCliente.Image == null)
            {
                if (System.IO.File.Exists(ruta.imagenDefinida))
                    picCliente.Image = Image.FromFile(ruta.imagenDefinida);
                else
                    return; // O manejar el error si no hay imagen por defecto
            }

            byte[] byteArray = ConvertImage.ImageToByteArray(picCliente.Image);

            // 2. Preparar los valores de los ComboBox manualmente
            // Esto evita el error de "Referencia a objeto"

            // Nivel de Precio
            string _nivel = "1";
            if (cboNivel.Text == "PRECIO B") _nivel = "2";
            else if (cboNivel.Text == "PRECIO C") _nivel = "3";
            else if (cboNivel.Text == "PRECIO D") _nivel = "4";
            else if (cboNivel.Text == "PRECIO E") _nivel = "5";

            // Tipo de Documento
            string _tipoDoc = "1"; // Cedula por defecto
            if (cboDocumento.Text == "Pasaporte") _tipoDoc = "2";
            else if (cboDocumento.Text == "RNC") _tipoDoc = "3";

            // Paga Impuesto (Si/No a S/N o 1/2)
            string _paga = "N";
            if (cboPaga.Text == "Si") _paga = "S";

            // Tipo Cliente
            string _tipoCliente = "F"; // Persona Fisica
            if (cboTipo.Text == "Empresa") _tipoCliente = "J"; // Juridica
            else if (cboTipo.Text == "Gobierno") _tipoCliente = "3";
            else if (cboTipo.Text == "Ongs") _tipoCliente = "4";


            // 3. El Query Corregido (Corregí PAGAIMPUESTO por IMPUESTOPAGA)
            string query = @"UPDATE CLIENTES 
                     SET NOMBRECLIENTE = @A2 , 
                         IDENTIFICACION = @A3 , 
                         TIPOCLIENTE = @A4 , 
                         DIRECCION = @A5 , 
                         SECTOR = @A6 ,
                         CIUDAD = @A7 , 
                         TELEFONO = @A8 , 
                         MOVIL = @A9 , 
                         CORREO = @A10 , 
                         ESTATUS = @A11 , 
                         FOTO = @A12 , 
                         ENVIOSDIRECCION = @A13 , 
                         BALANCEPENDIENTE = @A14 , 
                         NIVELPRECIO = @A15 , 
                         TIPODOCUMENTO = @A16 , 
                         IMPUESTOPAGA = @A17
                     WHERE IDCLIENTE = @A1";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                try
                {
                    cxn.Open();
                    SqlCommand cmd = new SqlCommand(query, cxn);

                    cmd.Parameters.AddWithValue("@A1", txtCliente.Text);
                    cmd.Parameters.AddWithValue("@A2", txtNombre.Text);
                    cmd.Parameters.AddWithValue("@A3", txtDocumento.Text);

                    // Usamos las variables que calculamos arriba
                    cmd.Parameters.AddWithValue("@A4", _tipoCliente);

                    cmd.Parameters.AddWithValue("@A5", txtDireccion.Text);
                    cmd.Parameters.AddWithValue("@A6", txtSector.Text);
                    cmd.Parameters.AddWithValue("@A7", txtCiudad.Text);
                    cmd.Parameters.AddWithValue("@A8", txtTelefono.Text);
                    cmd.Parameters.AddWithValue("@A9", txtMovil.Text);
                    cmd.Parameters.AddWithValue("@A10", txtCorreo.Text);

                    // OJO: En los Inserts usamos 'A' para Activo, no '1'. Lo cambié a 'A'.
                    cmd.Parameters.AddWithValue("@A11", "A");

                    cmd.Parameters.AddWithValue("@A12", byteArray);
                    cmd.Parameters.AddWithValue("@A13", txtEnvio.Text);

                    // Convertimos el balance a numero para evitar error si está vacio
                    double balance = 0;
                    double.TryParse(lblBalance.Text, out balance);
                    cmd.Parameters.AddWithValue("@A14", balance);

                    // Usamos las variables calculadas
                    cmd.Parameters.AddWithValue("@A15", _nivel);
                    cmd.Parameters.AddWithValue("@A16", _tipoDoc);
                    cmd.Parameters.AddWithValue("@A17", _paga);

                    int resultado = cmd.ExecuteNonQuery();

                    if (resultado > 0)
                    {
                        MessageBox.Show("Cliente actualizado correctamente.");
                    }
                    else
                    {
                        MessageBox.Show("No se pudo actualizar (Verifica el ID).");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar: " + ex.Message);
                }
            }
        }

        private void ActualizaSecuenciaClientes(string id, string cliente)
        {
            string query = @"UPDATE SECUENCIAS SET CONTADOR = @A2 WHERE ID = @A1";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();

                SqlCommand cmd = new SqlCommand(query, cxn);
                cmd.Parameters.AddWithValue("@A1", id);
                cmd.Parameters.AddWithValue("@A2", cliente);

                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }

        private void LlenarCombox()
        {
            cboTipo.Items.Add(new ComboItem { Texto = "Persona", Valor = "1" });
            cboTipo.Items.Add(new ComboItem { Texto = "Empresa", Valor = "2" });
            cboTipo.Items.Add(new ComboItem { Texto = "Gobierno", Valor = "3" });
            cboTipo.Items.Add(new ComboItem { Texto = "Ongs", Valor = "4" });

            cboNivel.Items.Add(new ComboItem { Texto = "PRECIO A", Valor = "1" });
            cboNivel.Items.Add(new ComboItem { Texto = "PRECIO B", Valor = "2" });
            cboNivel.Items.Add(new ComboItem { Texto = "PRECIO C", Valor = "3" });
            cboNivel.Items.Add(new ComboItem { Texto = "PRECIO D", Valor = "4" });
            cboNivel.Items.Add(new ComboItem { Texto = "PRECIO E", Valor = "5" });

            cboDocumento.Items.Add(new ComboItem { Texto = "Cedula", Valor = "1" });
            cboDocumento.Items.Add(new ComboItem { Texto = "Pasaporte", Valor = "2" });
            cboDocumento.Items.Add(new ComboItem { Texto = "RNC", Valor = "3" });

            cboPaga.Items.Add(new ComboItem { Texto = "Si", Valor = "1" });
            cboPaga.Items.Add(new ComboItem { Texto = "No", Valor = "2" });

            // --- Seleccionar el primer elemento ---
            cboTipo.SelectedIndex = 0;
            cboNivel.SelectedIndex = 0;
            cboDocumento.SelectedIndex = 0;
            cboPaga.SelectedIndex = 0;
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(txtCliente.Text))
            {
                MostrarMensaje("Cliente");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtDocumento.Text))
            {
                MostrarMensaje("Documento");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MostrarMensaje("Nombre");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtDireccion.Text))
            {
                MostrarMensaje("Direccion");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtSector.Text))
            {
                MostrarMensaje("Sector");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtCiudad.Text))
            {
                MostrarMensaje("Ciudad");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                MostrarMensaje("Telefono");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtMovil.Text))
            {
                MostrarMensaje("Movil");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtCorreo.Text))
            {
                MostrarMensaje("Correo");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtContacto.Text))
            {
                MostrarMensaje("Contacto");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtEnvio.Text))
            {
                MostrarMensaje("Direccion de Envio");
                return false;
            }
            return true;
        }

        private void MostrarMensaje(string campo)
        {
            MessageBox.Show($"{campo}", "Miapp", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


    }
}
