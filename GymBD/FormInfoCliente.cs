using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace GymBD
{
    public partial class FormInfoCliente : Form
    {
        private static FormInfoCliente instancia = null;
        private string connectionString = "Server=localhost;Database=gymdb;User Id=root;Password=;";

        public static FormInfoCliente ventana_unica()
        {
            if (instancia == null)
            {
                instancia = new FormInfoCliente();

            }
            return instancia;
        }

        public FormInfoCliente()
        {
            InitializeComponent();
            this.FormClosed += (s, e) => { instancia = null; };
        }

        private void FormInfoCliente_Load(object sender, EventArgs e)
        {

        }

       
        private void btn_consultar_Click(object sender, EventArgs e)
        {
            string query = "SELECT p.id, p.Nombre, p.Apellido, p.FechaNacimiento, p.Direccion, p.Telefono, p.Email, f.peso, f.talla, f.porcentajeGrasaCorporal, f.fechaRegistro " +
                    "FROM persona p LEFT JOIN fichamedica f ON p.id = f.id_cliente WHERE p.id >= 100";  

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgv_cliente.DataSource = dt; 
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
            dgv_cliente.DataSource = null;
        }

        private void btn_buscar_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txt_buscar_id.Text, out int idCliente) && idCliente >= 100)
            {
                string query = "SELECT p.id, p.Nombre, p.Apellido, p.FechaNacimiento, p.Direccion, p.Telefono, p.Email, f.peso, f.talla, f.porcentajeGrasaCorporal, f.fechaRegistro " +
                               "FROM persona p LEFT JOIN fichamedica f ON p.id = f.id_cliente WHERE p.id = @idCliente";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@idCliente", idCliente);

                        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgv_cliente.DataSource = dt;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
            else
            {
                
                MessageBox.Show("Por favor ingrese un ID válido mayor o igual a 100.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void dgv_cliente_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }
    }  
}
