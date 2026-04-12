using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSC10SP
{
    public partial class frmStarts : Form
    {
        public frmStarts()
        {
            InitializeComponent();
        }

        bool _instanciaprevia;
        string password;


        private void frmStarts_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.Text = "Login";

            //evita que la app este abierta varias veces
            if (Process.GetProcessesByName("frmStarts").Length > 1)
            {
                _instanciaprevia = true;
                Application.Exit();
            }
        }

        private void frmStarts_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            lblHORA.Text = DateTime.Now.ToString("hh:mm:ss ");
            lblFECHA.Text = DateTime.Now.ToLongDateString();
        }

        private void txtUsuario_Enter(object sender, EventArgs e)
        {
            if (txtUsuario.Text == "Usuario")
            {
                txtUsuario.Text = "";
                txtUsuario.ForeColor = Color.Black;
            }
        }

        private void txtUsuario_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtUsuario.Text.Trim() != string.Empty)
                {
                    txtPassword.Focus();  
                }
            }
        }

        private void txtUsuario_Leave(object sender, EventArgs e)
        {
            if (txtUsuario.Text.Trim() != string.Empty)
            {
                BuscarUsuario(txtUsuario.Text);
            }
        }


        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtPassword.Text.Trim() != string.Empty)
                {
                    btnAceptar.Focus();
                }
            }
        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {
            if (txtPassword.Text.Trim() != string.Empty)
            {
                btnAceptar.PerformClick();
            }
        }

        private void txtPassword_Enter(object sender, EventArgs e)
        {
            if (txtUsuario.Text == "Password")
            {
                txtPassword.Text = "";
                txtPassword.ForeColor = Color.Black;
            }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (txtUsuario.Text.Trim() != string.Empty)
            {
                if (txtPassword.Text.Trim() != string.Empty)
                {
                    if(txtPassword.Text.Trim() == password)
                    {
                        frmMenu frm = new frmMenu();
                        frm.ShowDialog();
                        this.Hide();
                    }
                }
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void BuscarUsuario(string usuario)
        {
            string query = "SELECT CLAVE, SUCURSAL FROM USUARIO WHERE ACTIVO = 1 AND NOMBRECORTO = @A1";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();
                SqlCommand cmd = new SqlCommand(query, cxn);
                cmd.Parameters.AddWithValue("@A1", usuario);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    cnn.miSucursal = reader["SUCURSAL"].ToString(); // INDICA LA SUCURSAL DEL USUARIO
                    password = reader["CLAVE"].ToString();
                }
            }
        }
    }
}
