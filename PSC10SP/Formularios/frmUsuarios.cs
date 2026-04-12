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
    public partial class frmUsuarios : Form
    {
        public frmUsuarios()
        {
            InitializeComponent();
        }

        Boolean ExisteData;


        private void frmUsuarios_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true; //activa las teclas de funciones
            this.Text = "Maestro de Usuario";

            ExisteData = false;
            lblIdentifica.Text = Busco.BuscaUltimoNumero("0"); // busca el siguiente registro a utilizar

            picUsuario.Image = Image.FromFile(ruta.imagenDefinida);
        }

        private void frmUsuarios_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape) //aqui es la condicional de si toco la tecla escape "esc" y cierra el formulario
            {
                this.Close();
            }
        }


        //--------------------------------------------------------------
        // TEXT BOX'S

        private void txtUsuario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4) //aqui es la condicional de si toco la tecla escape "esc" y cierra el formulario
            {
                btnUsuarios.PerformClick();
            }
        }


        private void txtUsuario_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter) //Pregunta si presionaste la letra enter
            {
                e.Handled = true;
                if(txtUsuario.Text.Trim() != string.Empty) //Aqui pregunta si es diferente de vacio el txtbox
                {
                    txtPassword.Focus(); //Mueve el cursor al siguiente txtbox
                }
            }    
        }

        private void txtUsuario_Leave(object sender, EventArgs e)
        {
            BuscarUsuario(txtUsuario.Text);
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter) //Pregunta si presionaste la letra enter
            {
                e.Handled = true;
                if (txtPassword.Text.Trim() != string.Empty) //Aqui pregunta si es diferente de vacio el txtbox
                {
                    txtNombre.Focus(); //Mueve el cursor al siguiente txtbox
                }
            }
        }

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter) //Pregunta si presionaste la letra enter
            {
                e.Handled = true;
                if (txtNombre.Text.Trim() != string.Empty) //Aqui pregunta si es diferente de vacio el txtbox
                {
                    txtEmail.Focus(); //Mueve el cursor al siguiente txtbox
                }
            }
        }

        private void txtEmail_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter) //Pregunta si presionaste la letra enter
            {
                e.Handled = true;
                if (txtEmail.Text.Trim() != string.Empty) //Aqui pregunta si es diferente de vacio el txtbox
                {
                    txtPosicion.Focus(); //Mueve el cursor al siguiente txtbox
                }
            }
        }


        private void txtPosicion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter) //Pregunta si presionaste la letra enter
            {
                e.Handled = true;
                if (txtPosicion.Text.Trim() != string.Empty) //Aqui pregunta si es diferente de vacio el txtbox
                {
                    btnGuardar.Focus(); //Mueve el cursor al siguiente txtbox
                }
            }
        }

        private void txtPosicion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4) //aqui es la condicional de si toco la tecla escape "esc" y cierra el formulario
            {
                lblPosicion.Text = Busco.BuscarPuestoDeTrabajo(txtPosicion.Text);
            }
        }



        private void txtPosicion_Leave(object sender, EventArgs e)
        {
            if (txtPosicion.Text.Trim() != string.Empty) //Aqui pregunta si es diferente de vacio el txtbox
            {
                btnGuardar.Focus(); //Mueve el cursor al siguiente txtbox
            }
        }
        //--------------------------------------------------------------
        //BOTONES

        private void button1_Click(object sender, EventArgs e) //boton guardar
        {
            if (!ValidarFormulario())
                return;

            if (!ExisteData)
            {
                InsertarData(); //Inserta el nuevo registro
                ActualizaSecuenciaDeUsuario("0", lblIdentifica.Text); //Actualiza la NUEVA secuencia
                LimpiarFormulario(); //Limpia el Formulario
                lblIdentifica.Text = Busco.BuscaUltimoNumero("0"); // busca el siguiente registro a utilizar
            }
            else
            {
                ActualizarData();
                LimpiarFormulario(); //Limpia el Formulario
                lblIdentifica.Text = Busco.BuscaUltimoNumero("0"); // busca el siguiente registro a utilizar
            }
        }


        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace( txtUsuario.Text))
            {
                MostrarMensaje("Usuario");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MostrarMensaje("Nombre");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MostrarMensaje("Email");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MostrarMensaje("Password");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtPosicion.Text))
            {
                MostrarMensaje("Puesto de Trabajo");
                return false;
            }

            return true;
        }


        private void MostrarMensaje(string campo)
        {
            MessageBox.Show($"Falta el {campo}", "Miapp", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }



        private void button2_Click(object sender, EventArgs e) //Boton limpiar
        {
            LimpiarFormulario();
            txtUsuario.Focus();
        }

        private void button3_Click(object sender, EventArgs e) //Boton borrar
        {
            if(ExisteData == true)
            {
                BorrarData();
                LimpiarFormulario();
                txtUsuario.Focus();
            }
        }

        private void button4_Click(object sender, EventArgs e) //boton salir
        {
            this.Close();
        }

        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            frmVENUSR frm = new frmVENUSR();
            frm.ShowDialog();

            if(frm.EData == true)
            {
                txtUsuario.Text = frm.var1;
                BuscarUsuario(txtUsuario.Text);
            }
        }

        private void btnPosicion_Click(object sender, EventArgs e)
        {
            frmVENPOS frm = new frmVENPOS();
            frm.ShowDialog();

            if (frm.EData == true)
            {
                txtPosicion.Text = frm.var1;
                lblPosicion.Text = frm.var2;
                
            }
        }


        private void btnSucursal_Click(object sender, EventArgs e)
        {
            // Crear instancia del formulario de sucursales
            frmVENSUC frm = new frmVENSUC();
            frm.ShowDialog();

            if (frm.EData == true)
            {
                txtSucursal.Text = frm.var1;  // o asigna a la caja de texto que quieras
                                             // Aquí puedes llamar un método para buscar sucursal si tienes uno, ejemplo:
                                             // BuscarSucursal(txtUsuario.Text);
            }
        }


        private void picUsuario_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string _imagen = openFileDialog1.FileName;
                picUsuario.Image = Image.FromFile(_imagen);
            }
        }

        //---------------------------------------------------------------

        //---------------------------------------------------------------
        //METODOS

        private void LimpiarFormulario()
        {
            txtUsuario.Clear();
            txtPassword.Clear();
            txtEmail.Clear();
            txtNombre.Clear();
            txtPosicion.Clear();
            txtSucursal.Clear();
            lblIdentifica.Text = "";
            lblPosicion.Text = "";
            picUsuario.Image = null;

            picUsuario.Image = Image.FromFile(ruta.imagenDefinida);
            ExisteData =false;

    }


        private void BuscarUsuario(string _nameUsr)
        {
            ExisteData = false;

            string _Query =
                    "SELECT IDENTIFICACION, NOMBRE, NOMBRECORTO, CLAVE, POSICION, " +
                    "CORREO, SUCURSAL, B.NOMBREDEPOSICION, FOTO FROM USUARIO " + // Coma agregada entre SUCURSAL y B.NOMBREDEPOSICION
                    "AS A INNER JOIN POSICIONES AS B ON A.POSICION = B.IDPOSICION " +
                    "WHERE NOMBRECORTO = '" + _nameUsr + "' AND ACTIVO = 1";  // Aquí las comillas para insertar el valor de _nameUsr

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                SqlCommand cmd = new SqlCommand(_Query, cnx); // Envia el script al servidor
                SqlDataReader rdr = cmd.ExecuteReader(); // Ejecuta el query en el servidor

                if (rdr.Read()) // Verifica si el contenedor trajo información
                {
                    ExisteData = true;
                    lblIdentifica.Text = rdr["IDENTIFICACION"].ToString();
                    txtEmail.Text = rdr["CORREO"].ToString();
                    txtUsuario.Text = rdr["NOMBRECORTO"].ToString();
                    txtPassword.Text = rdr["CLAVE"].ToString();
                    txtPosicion.Text = rdr["POSICION"].ToString();
                    txtNombre.Text = rdr["NOMBRE"].ToString();
                    txtSucursal.Text = rdr["SUCURSAL"].ToString();

                    try
                    {
                        picUsuario.Image = ConvertImage.ByteArrayToImage((byte[])rdr["FOTO"]);
                    }
                    catch { }
                }
            }
        }

        //---------------------------------------------------------------







        private void label1_Click(object sender, EventArgs e)
        {

        }


        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void txtPosicion_TextChanged(object sender, EventArgs e)
        {

        }


        private void InsertarData()
        {
            byte[] bytesArrayImagen = ConvertImage.ImageToByteArray(picUsuario.Image);

            string _Query = "INSERT INTO USUARIO (NOMBRE, NOMBRECORTO, CLAVE, POSICION, CORREO, FOTO, SUCURSAL)" +
                "VALUES (@A1, @A2, @A3, @A4, @A5, @A6, @A7)";

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(_Query, cnx);

                cmd.Parameters.AddWithValue("@A1", txtNombre.Text);
                cmd.Parameters.AddWithValue("@A2", txtUsuario.Text);
                cmd.Parameters.AddWithValue("@A3", txtPassword.Text);
                cmd.Parameters.AddWithValue("@A4", txtPosicion.Text);
                cmd.Parameters.AddWithValue("@A5", txtEmail.Text);
                cmd.Parameters.AddWithValue("@A6", bytesArrayImagen);
                cmd.Parameters.AddWithValue("@A7", txtSucursal.Text);

                cmd.ExecuteNonQuery();

           

            }
        }

        private void ActualizarData()
        {
            byte[] bytesArrayImagen = ConvertImage.ImageToByteArray(picUsuario.Image);

            string _Query =
                "UPDATE USUARIO " +
                "   SET NOMBRE =@A1," +
                "      CLAVE = @A3, " +
                "      POSICION = @A4, " +
                "      CORREO = @A5, " +
                "      FOTO = @A6, " +
                "      SUCURSAL = @A7 " +
                " WHERE NOMBRECORTO = @A2;";

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(_Query, cnx);

                cmd.Parameters.AddWithValue("@A1", txtNombre.Text);
                cmd.Parameters.AddWithValue("@A2", txtUsuario.Text);
                cmd.Parameters.AddWithValue("@A3", txtPassword.Text);
                cmd.Parameters.AddWithValue("@A4", txtPosicion.Text);
                cmd.Parameters.AddWithValue("@A5", txtEmail.Text);
                cmd.Parameters.AddWithValue("@A7", txtSucursal.Text);
                cmd.Parameters.AddWithValue("@A6", bytesArrayImagen);

                cmd.ExecuteNonQuery();

      

            }
        }

        private void BorrarData()
        {
            byte[] bytesArrayImagen = ConvertImage.ImageToByteArray(picUsuario.Image);

            string _Query =
               "DELETE FROM USUARIO WHERE NOMBRECORTO = @A2";

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(_Query, cnx);
                cmd.Parameters.AddWithValue("@A2", txtUsuario.Text);

                cmd.ExecuteNonQuery();

            }
        }


        private void ActualizaSecuenciaDeUsuario(string numID, string numSec)
        {
            string _query = "UPDATE SECUENCIAS SET CONTADOR = @A2 WHERE ID = @A1;";

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(_query, cnx);
                cmd.Parameters.AddWithValue("@A2", numSec);
                cmd.Parameters.AddWithValue("@A1", numID);
                cmd.ExecuteNonQuery();

            }
        }

        private void lblIdentifica_Click(object sender, EventArgs e)
        {

        }
    }
}
