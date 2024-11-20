using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySqlConnector;

namespace GymBD
{
    public partial class FormInfoEmpleado : Form
    {
        private static FormInfoEmpleado instancia = null;
        private string connectionString = "server=localhost;database=gymdb;uid=root;pwd=;";  

        public static FormInfoEmpleado ventana_unica()
        {
            if (instancia == null)
            {
                instancia = new FormInfoEmpleado();
                
            }
            return instancia;
        }
        public FormInfoEmpleado()
        {
            InitializeComponent();
            this.FormClosed += (s, e) => { instancia = null; };
        }

        private void btn_consultar_Click(object sender, EventArgs e)
        {
            string query = "SELECT e.id, p.Nombre, p.Apellido, p.FechaNacimiento, p.Direccion, p.Telefono, p.Email, " +
                   "e.puesto, e.salario, e.fechaContratacion, " +
                   "f.peso, f.talla, f.porcentajeGrasaCorporal, f.fechaRegistro " +
                   "FROM empleado e " +
                   "JOIN persona p ON e.id = p.id " +
                   "LEFT JOIN fichamedica f ON e.id = f.id_cliente " +
                   "WHERE e.id < 100";  

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgv_empleado.DataSource = dt; 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void btn_limpiar_Click(object sender, EventArgs e)
        {
            txt_buscar_id.Clear();
            dgv_empleado.DataSource = null;
        }

        private void btn_buscar_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txt_buscar_id.Text, out int idEmpleado) && idEmpleado < 100)
            {
                string query = "SELECT e.id, p.Nombre, p.Apellido, p.FechaNacimiento, p.Direccion, p.Telefono, p.Email, " +
                               "e.puesto, e.salario, e.fechaContratacion, " +
                               "f.peso, f.talla, f.porcentajeGrasaCorporal, f.fechaRegistro " +
                               "FROM empleado e " +
                               "JOIN persona p ON e.id = p.id " +
                               "LEFT JOIN fichamedica f ON e.id = f.id_cliente " +  
                               "WHERE e.id = @idEmpleado";  

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@idEmpleado", idEmpleado);

                        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgv_empleado.DataSource = dt; 
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message); 
                    }
                }
            }
            else
            {
                
                MessageBox.Show("Por favor ingrese un ID válido menor a 100.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormInfoEmpleado_Load(object sender, EventArgs e)
        {

        }
    }
}
