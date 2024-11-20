using MySqlConnector;
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
using static GymBD.ConexionDB;

namespace GymBD
{
    public partial class Form1 : Form
    {
        public static DataTable DataSource { get; private set; }
        private ConexionBD conexionBD;
        public static string UsuarioRol { get; private set; }




        public Form1()
        {
            InitializeComponent();
            conexionBD = new ConexionBD();
        }

        private void CargarDatos()
        {
            ServicioDatos servicio = new ServicioDatos();
            DataTable dt = servicio.ObtenerDatos("administrador");
            Form1.DataSource = dt;
        }

        private void LimpiarCampos()
        {
            txt_nombre.Clear();
            txt_contrasena.Clear();
         
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private string usuarioRol = "";

       

        private void btn_ingresar_Click(object sender, EventArgs e)
        {
            string nombreUsuario = txt_nombre.Text;
            string contrasena = txt_contrasena.Text;

            // Validar si los campos no están vacíos
            if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(contrasena))
            {
                MessageBox.Show("Datos incorrectos. Asegúrese de llenar todos los campos correctamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LimpiarCampos();

            // Validar las credenciales y acceder al sistema
            if (ValidarCredenciales(nombreUsuario, contrasena))
            {
                // Aquí debes determinar el rol del usuario y asignarlo a la variable UsuarioRol
                if (EsAdministrador(nombreUsuario)) // Se asume que tienes esta función para verificar si es administrador
                {
                    UsuarioRol = "Administrador";
                }
                else
                {
                    UsuarioRol = "Cliente";
                }

                MessageBox.Show("Inicio de sesión exitoso", "Login", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FormMenuGym menu = new FormMenuGym();
                menu.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarCredenciales(string nombreUsuario, string contrasena)
        {
            using (MySqlConnection conexion = conexionBD.ObtenerConexion())
            {
                // Primero, verifica en la tabla administrador
                string queryAdministrador = "SELECT COUNT(*) FROM administrador WHERE Nombre = @Nombre AND Contrasena = @Contrasena";

                using (MySqlCommand cmd = new MySqlCommand(queryAdministrador, conexion))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombreUsuario);
                    cmd.Parameters.AddWithValue("@Contrasena", contrasena);

                    var result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        int count = Convert.ToInt32(result);
                        if (count > 0)
                        {
                            // Si se encuentra en la tabla administrador, el usuario es un administrador
                            return true;
                        }
                    }
                }

                // Luego, verifica en la tabla persona
                string queryPersona = "SELECT COUNT(*) FROM persona WHERE Nombre = @Nombre AND id = @Contrasena";

                using (MySqlCommand cmd = new MySqlCommand(queryPersona, conexion))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombreUsuario);
                    cmd.Parameters.AddWithValue("@Contrasena", contrasena);

                    var result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        int count = Convert.ToInt32(result);
                        if (count > 0)
                        {
                            // Si se encuentra en la tabla persona, el usuario es un cliente
                            return true;
                        }
                    }
                }

                return false; // Si no se encuentra en ninguna de las tablas, la credencial es incorrecta
            }
        }

        private bool EsAdministrador(string nombreUsuario)
        {
            using (MySqlConnection conexion = conexionBD.ObtenerConexion())
            {
              
                string queryAdministrador = "SELECT COUNT(*) FROM administrador WHERE Nombre = @Nombre";

                using (MySqlCommand cmd = new MySqlCommand(queryAdministrador, conexion))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombreUsuario);
                    var result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result) > 0; 
                }
            }
        }

    }
}
