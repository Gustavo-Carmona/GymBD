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
    public partial class FormDieta : Form
    {
        private static FormDieta instancia = null;
        private string connectionString = "Server=localhost;Database=gymdb;User Id=root;Password=;";


        public static FormDieta ventana_unica()
        {
            if (instancia == null)
            {
                instancia = new FormDieta();

            }
            return instancia;
        }
        public FormDieta()
        {
            InitializeComponent();
            this.FormClosed += (s, e) => { instancia = null; };
        }

       
        
        private bool ValidarCampos()
        {
            if (string.IsNullOrEmpty(txt_nombre.Text) ||
                string.IsNullOrEmpty(txt_descripcion.Text) ||
                !int.TryParse(txt_idcliente.Text, out _))
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return false;
            }
            return true;
        }


        private bool ConfirmarOperacion(string operacion)
        {
            DialogResult result = MessageBox.Show($"¿Está seguro de {operacion} esta dieta?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return result == DialogResult.Yes;
        }


        private void LimpiarCampos()
        {
            txt_nombre.Clear();
            txt_descripcion.Clear();
            txt_idcliente.Clear();
            dtp_fasignacion.Value = DateTime.Now;

            dgv_dieta.DataSource = null;
        }


        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void btn_agregar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos())
                return;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO dieta (id_cliente, nombre, fechaAsignacion, descripcion) VALUES (@id_cliente, @nombre, @fecha, @descripcion)";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_cliente", int.Parse(txt_idcliente.Text));
                        command.Parameters.AddWithValue("@nombre", txt_nombre.Text);
                        command.Parameters.AddWithValue("@fecha", dtp_fasignacion.Value);
                        command.Parameters.AddWithValue("@descripcion", txt_descripcion.Text);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Dieta agregada con éxito.");
                LimpiarCampos();
                btn_consultar_Click(sender, e); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar la dieta: {ex.Message}");
            }
        }

        private void btn_modificar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos())
                return;

            if (dgv_dieta.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione una dieta para modificar.");
                return;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    int idDieta = Convert.ToInt32(dgv_dieta.SelectedRows[0].Cells["id"].Value);

                    string query = "UPDATE dieta SET id_cliente = @id_cliente, nombre = @nombre, fechaAsignacion = @fecha, descripcion = @descripcion WHERE id = @id";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_cliente", int.Parse(txt_idcliente.Text));
                        command.Parameters.AddWithValue("@nombre", txt_nombre.Text);
                        command.Parameters.AddWithValue("@fecha", dtp_fasignacion.Value);
                        command.Parameters.AddWithValue("@descripcion", txt_descripcion.Text);
                        command.Parameters.AddWithValue("@id", idDieta);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Dieta modificada con éxito.");
                LimpiarCampos();
                btn_consultar_Click(sender, e); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al modificar la dieta: {ex.Message}");
            }
        }

        private void btn_eliminar_Click(object sender, EventArgs e)
        {
            if (dgv_dieta.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione una dieta para eliminar.");
                return;
            }

            if (!ConfirmarOperacion("eliminar"))
                return;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    int idDieta = Convert.ToInt32(dgv_dieta.SelectedRows[0].Cells["id"].Value);

                    string query = "DELETE FROM dieta WHERE id = @id";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", idDieta);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Dieta eliminada con éxito.");
                LimpiarCampos();
                btn_consultar_Click(sender, e); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar la dieta: {ex.Message}");
            }
        }

        private void btn_consultar_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                SELECT d.id, d.nombre, d.fechaAsignacion, d.descripcion, d.id_cliente
                FROM dieta d
                JOIN cliente c ON d.id_cliente = c.id;
            ";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        dgv_dieta.DataSource = table;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar dietas: {ex.Message}");
            }
        }

        private void FormDieta_Load(object sender, EventArgs e)
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

        private void dgv_dieta_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgv_dieta.Rows[e.RowIndex];

                
                txt_nombre.Text = row.Cells["nombre"].Value.ToString();  
                txt_descripcion.Text = row.Cells["descripcion"].Value.ToString();  
                txt_idcliente.Text = row.Cells["id_cliente"].Value.ToString();  
                dtp_fasignacion.Value = Convert.ToDateTime(row.Cells["fechaAsignacion"].Value);  
            }
        }

        private void btn_limpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }
    }
}
