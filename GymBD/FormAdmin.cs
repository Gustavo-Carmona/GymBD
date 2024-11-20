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
    public partial class FormAdmin : Form
    {

        private static FormAdmin instancia = null;
        private string connectionString = "Server=localhost;Database=gymdb;User Id=root;Password=;";

        public static FormAdmin ventana_unica()
        {
            if (instancia == null)
            {
                instancia = new FormAdmin();

            }
            return instancia;
        }
        public FormAdmin()
        {
            InitializeComponent();
            this.FormClosed += (s, e) => { instancia = null; };
        }

        private bool ConfirmarOperacion(string operacion)
        {
            DialogResult result = MessageBox.Show($"¿Está seguro de {operacion} este administrador?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return result == DialogResult.Yes;
        }


        private bool ValidarCampos()
        {
            if (string.IsNullOrEmpty(txt_nombre.Text) || string.IsNullOrEmpty(txt_contrasena.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return false;
            }
            return true;
        }

        private void LimpiarCampos()
        {
            txt_nombre.Clear();      
            txt_contrasena.Clear();

            dgv_admin.DataSource = null;
        }



        private void btn_agregar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;  // Validar los campos antes de continuar

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                
                string query = "INSERT INTO administrador (Nombre, Contrasena) VALUES (@nombre, @contrasena)";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", txt_nombre.Text);
                    cmd.Parameters.AddWithValue("@contrasena", txt_contrasena.Text);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Administrador agregado con éxito.");
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

                    // Modificar administrador existente
                    string query = "UPDATE administrador SET Nombre = @nombre, Contrasena = @contrasena WHERE id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre", txt_nombre.Text);
                        cmd.Parameters.AddWithValue("@contrasena", txt_contrasena.Text);
                        cmd.Parameters.AddWithValue("@id", dgv_admin.SelectedRows[0].Cells["id"].Value);  // Obtener el ID de la fila seleccionada

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Administrador modificado con éxito.");
                    LimpiarCampos();  
                }
            }
        }

        private void btn_eliminar_Click(object sender, EventArgs e)
        {
            if (dgv_admin.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un administrador para eliminar.");
                return;
            }

            if (ConfirmarOperacion("eliminar"))
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Eliminar administrador seleccionado
                    string query = "DELETE FROM administrador WHERE id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", dgv_admin.SelectedRows[0].Cells["id"].Value);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Administrador eliminado con éxito.");
                    LimpiarCampos();  // Limpiar los campos después de eliminar
                }
            }
        }

        private void btn_consultar_Click(object sender, EventArgs e)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT id, Nombre, Contrasena FROM administrador";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    dgv_admin.DataSource = dataTable;  
                }
            }
        }

        private void FormAdmin_Load(object sender, EventArgs e)
        {
            if (Form1.UsuarioRol == "Cliente")
            {
                btn_agregar.Enabled = false;
                btn_modificar.Enabled = false;
                btn_eliminar.Enabled = false;
                btn_consultar.Enabled = false;
            }
            else if (Form1.UsuarioRol == "Administrador")
            {
                btn_agregar.Enabled = true;
                btn_modificar.Enabled = true;
                btn_eliminar.Enabled = true;
                btn_consultar.Enabled = true;
            }
        }

        private void dgv_admin_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                
                DataGridViewRow row = dgv_admin.Rows[e.RowIndex];
                
                txt_nombre.Text = row.Cells["Nombre"].Value.ToString();  
                txt_contrasena.Text = row.Cells["Contrasena"].Value.ToString();  
            }
        }

        private void btn_limpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }
    }
}
