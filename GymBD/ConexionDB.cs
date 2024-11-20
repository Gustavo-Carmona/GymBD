using System.Collections.Generic;
using System;
using System.Data;
using MySqlConnector;
using static GymBD.ConexionDB;

namespace GymBD
{
    internal class ConexionDB
    {
        public class ConexionBD
        {
            public MySqlConnection ObtenerConexion()
            {
                string conexionString = "Server=localhost;Database=gymdb;User Id=root;Password=;";
                MySqlConnection conexion = new MySqlConnection(conexionString);

                if (conexion.State == ConnectionState.Closed)
                {
                    conexion.Open();
                }
                return conexion;
            }
        }
    }

    public class ServicioDatos
    {
        private ConexionBD conexionBD;

        public ServicioDatos()
        {
            conexionBD = new ConexionBD();
        }

        public DataTable ObtenerDatos(string nombreTabla)
        {
            var tablasPermitidas = new List<string> { "administrador", "cliente", "area", "persona", "empleado", "alimento", "dieta", "maquina", "mantenimiento", "plan", "promocion", "fichamedica" };

            if (!tablasPermitidas.Contains(nombreTabla))
            {
                throw new ArgumentException("Nombre de tabla no válido.");
            }

            DataTable dt = new DataTable();
            using (MySqlConnection conn = conexionBD.ObtenerConexion())  // Usamos 'using' para asegurar que la conexión se cierre automáticamente
            {
                string query = $"SELECT * FROM `{nombreTabla}`";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
            }  // La conexión se cierra automáticamente al salir del bloque 'using'

            return dt;
        }
    }

}
