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
    public partial class FormAlimento : Form
    {

        private static FormAlimento instancia = null;
        private string connectionString = "Server=localhost;Database=gymdb;User Id=root;Password=;";


        public static FormAlimento ventana_unica()
        {
            if (instancia == null)
            {
                instancia = new FormAlimento();

            }
            return instancia;
        }
        public FormAlimento()
        {
            InitializeComponent();
            this.FormClosed += (s, e) => { instancia = null; };
        }

       

        private bool ValidarCampos()
        {
            if (string.IsNullOrEmpty(txt_nombre.Text) || string.IsNullOrEmpty(txt_descripcion.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return false;
            }
            return true;
        }

        private bool ConfirmarOperacion(string operacion)
        {
            DialogResult result = MessageBox.Show($"¿Está seguro de {operacion} este alimento?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return result == DialogResult.Yes;
        }

        private void LimpiarCampos()
        {
            txt_nombre.Clear();        
            txt_descripcion.Clear();   
            dgv_alimento.DataSource = null;  

          
        }



        private void FormAlimento_Load(object sender, EventArgs e)
        {
            if (Form1.UsuarioRol == "Cliente")
            {
                btn_agregar.Enabled = false;
                btn_modificar.Enabled = false;
                btn_eliminar.Enabled = false;
                btn_consultar.Enabled = true;
            }
            else if (Form1.UsuarioRol == "Administrador")
            {
                btn_agregar.Enabled = true;
                btn_modificar.Enabled = true;
                btn_eliminar.Enabled = true;
                btn_consultar.Enabled = true;
            }
        }

        private void btn_agregar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;  // Validar los campos antes de continuar

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Insertar nuevo alimento
                string query = "INSERT INTO alimento (nombre, descripcion) " +
                               "VALUES (@nombre, @descripcion)";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", txt_nombre.Text);
                    cmd.Parameters.AddWithValue("@descripcion", txt_descripcion.Text);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Alimento agregado con éxito.");
                LimpiarCampos();  // Limpiar los campos después de agregar
            }
        }

        private void btn_modificar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;  // Validar los campos antes de continuar

            if (ConfirmarOperacion("modificar"))
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Modificar alimento existente
                    string query = "UPDATE alimento SET nombre = @nombre, descripcion = @descripcion " +
                                   "WHERE id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre", txt_nombre.Text);
                        cmd.Parameters.AddWithValue("@descripcion", txt_descripcion.Text);
                        cmd.Parameters.AddWithValue("@id", dgv_alimento.SelectedRows[0].Cells["id"].Value);  // Obtener el ID de la fila seleccionada

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Alimento modificado con éxito.");
                    LimpiarCampos();  // Limpiar los campos después de modificar
                }
            }
        }

        private void btn_eliminar_Click(object sender, EventArgs e)
        {
            if (dgv_alimento.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un alimento para eliminar.");
                return;
            }

            if (ConfirmarOperacion("eliminar"))
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Eliminar alimento seleccionado
                    string query = "DELETE FROM alimento WHERE id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", dgv_alimento.SelectedRows[0].Cells["id"].Value);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Alimento eliminado con éxito.");
                    LimpiarCampos();  // Limpiar los campos después de eliminar
                }
            }
        }

        private void btn_consultar_Click(object sender, EventArgs e)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT id, nombre, descripcion FROM alimento";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    dgv_alimento.DataSource = dataTable;  
                }
            }
        }

        private void dgv_alimento_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
               
                DataGridViewRow row = dgv_alimento.Rows[e.RowIndex];

               
                txt_nombre.Text = row.Cells["nombre"].Value.ToString();
                txt_descripcion.Text = row.Cells["descripcion"].Value.ToString();
            }
        }

        private void btn_limpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }
    }
}
