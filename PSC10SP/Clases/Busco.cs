using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
namespace PSC10SP
{
    public class Busco
    {
        public static string BuscaUltimoNumero(string _nmID)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();//abre la base de datos

                string _Query = "SELECT CONTADOR + 1 AS ULTIMO FROM SECUENCIAS WHERE ID = '" + _nmID + "'";
               
                SqlCommand cmd = new SqlCommand(_Query, cnx); //envia el script al servidor
                SqlDataReader rdr = cmd.ExecuteReader(); //ejecuta el query en el servidor

                if (rdr.Read()) //Pregunta si el contenedor trajo informacion
                {
                  return rdr["ULTIMO"].ToString();
                }
              }

                return null;
        }

        public static string BuscarPuestoDeTrabajo(string _puesto)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();//abre la base de datos

                string _Query = "SELECT NOMBREDEPOSICION FROM POSICIONES WHERE IDPOSICION = '" + _puesto + "'";

                SqlCommand cmd = new SqlCommand(_Query, cnx); //envia el script al servidor
                SqlDataReader rdr = cmd.ExecuteReader(); //ejecuta el query en el servidor

                if (rdr.Read()) //Pregunta si el contenedor trajo informacion
                {
                    return rdr["NOMBREDEPOSICION"].ToString();
                }
            }

            return null;
        }


        public static string BuscarSuplidor(string suplidor, out bool found)
        {
            found = false;

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                // Seleccionamos NombreEmpresa
                string _Query = "SELECT NombreEmpresa FROM PROVEEDORES WHERE IDPROVEEDOR = @id";

                SqlCommand cmd = new SqlCommand(_Query, cnx);
                cmd.Parameters.AddWithValue("@id", suplidor); // Usamos parametro por seguridad
                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    found = true;
                    // CORRECCION: Aquí debemos leer "NombreEmpresa", tal como dice el SELECT arriba
                    return rdr["NombreEmpresa"].ToString();
                }
            }

            return null;
        }



        public static string BuscarCategoria(string categoria, out bool found)
        {
            found = false;
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                string _Query = "SELECT CategoriaNombre FROM Categorias WHERE Categoria ='" + categoria + "'";

                SqlCommand cmd = new SqlCommand(_Query, cnx);
                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    found = true;
                    return rdr["CategoriaNombre"].ToString();
                }
            }

            return null;
        }
        public static string BuscarSubcategoria(string categoria, string sub, out bool found)
        {
            found = false;

            // 1. AGREGA ESTA LÍNEA TEMPORAL PARA VER QUÉ ESTÁS ENVIANDO
            System.Windows.Forms.MessageBox.Show("Buscando en BD -> Categoria ID: " + categoria + " / Subcategoria ID: " + sub);

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                // Usamos .Trim() para borrar espacios en blanco "fantasmas" que a veces dan problemas
                string catLimpia = categoria.Trim();
                string subLimpia = sub.Trim();

                string _Query = "SELECT NOMBRE " +
                                "FROM CATEGORIAS_SUB " +
                                "WHERE IDCategoria = @cat " +
                                "  AND SUBCATEGORIA = @sub";

                SqlCommand cmd = new SqlCommand(_Query, cnx);

                // Usamos parámetros (es más seguro y evita errores de comillas)
                cmd.Parameters.AddWithValue("@cat", catLimpia);
                cmd.Parameters.AddWithValue("@sub", subLimpia);

                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    found = true;
                    return rdr["NOMBRE"].ToString();
                }
            }

            return null;
        }

        public static string BuscarCliente(string cliente, out bool found, out string nivel, out string tipo, out string paga)
        {
            found = false;
            nivel = null;
            tipo = null;
            paga = null;

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                string _Query = "SELECT NOMBRECLIENTE, NIVELPRECIO, TIPOCLIENTE, IMPUESTOPAGA FROM CLIENTES WHERE IDCLIENTE = @A1";

                SqlCommand cmd = new SqlCommand(_Query, cnx);
                cmd.Parameters.AddWithValue("@A1", cliente);
                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    found = true;
                    nivel = rdr["NIVELPRECIO"].ToString();
                    tipo = rdr["TIPOCLIENTE"].ToString();
                    paga = rdr["IMPUESTOPAGA"].ToString();

                    return rdr["NOMBRECLIENTE"].ToString();
                }
            }

            return null;
        }

        public static string BuscaNombreCliente(string _idCliente)
        {
            // Usamos 'using' para asegurar que la conexión se cierre correctamente
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                // Consulta simple solo para obtener el nombre
                string _Query = "SELECT NOMBRECLIENTE FROM CLIENTES WHERE IDCLIENTE = @id";

                SqlCommand cmd = new SqlCommand(_Query, cnx);

                // Usar parámetros es más seguro que concatenar textos (+)
                cmd.Parameters.AddWithValue("@id", _idCliente);

                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    // Si encuentra el cliente, devuelve el nombre
                    return rdr["NOMBRECLIENTE"].ToString();
                }
            }

            // Si no encuentra nada, devuelve null (o podrías poner return "" para vacio)
            return null;
        }

    }
}
