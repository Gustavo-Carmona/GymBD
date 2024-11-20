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
    public partial class FormPlan : Form
    {
        private static FormPlan instancia = null;
        private string connectionString = "Server=localhost;Database=gymdb;User Id=root;Password=;";


        public static FormPlan ventana_unica()
        {
            if (instancia == null)
            {
                instancia = new FormPlan();

            }
            return instancia;
        }
        public FormPlan()
        {
            InitializeComponent();
            this.FormClosed += (s, e) => { instancia = null; };
        }

       

        private bool ValidarCampos()
        {
            if (string.IsNullOrEmpty(txt_nombrep.Text) || string.IsNullOrEmpty(txt_costo.Text) || string.IsNullOrEmpty(txt_duracion.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return false;
            }

            // Validar que el costo sea un número válido
            decimal costo;
            if (!decimal.TryParse(txt_costo.Text, out costo) || costo < 0)
            {
                MessageBox.Show("El costo debe ser un número positivo.");
                return false;
            }

            // Validar que la duración sea un número válido
            int duracion;
            if (!int.TryParse(txt_duracion.Text, out duracion) || duracion <= 0)
            {
                MessageBox.Show("La duración debe ser un número entero mayor que cero.");
                return false;
            }

            return true;
        }

        private bool ConfirmarOperacion(string operacion)
        {
            DialogResult result = MessageBox.Show($"¿Está seguro de {operacion} este plan?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return result == DialogResult.Yes;
        }

        private void LimpiarCampos()
        {
            
            txt_nombrep.Clear();
            txt_costo.Clear();
            txt_duracion.Clear();

            
           dgv_plan.DataSource = null;


        }

        private void btn_agregar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return; 

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Insertar nuevo plan
                string query = "INSERT INTO plan (nombre, costo, duracionMeses) " +
                               "VALUES (@nombre, @costo, @duracionMeses)";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", txt_nombrep.Text);
                    cmd.Parameters.AddWithValue("@costo", Convert.ToDecimal(txt_costo.Text));
                    cmd.Parameters.AddWithValue("@duracionMeses", Convert.ToInt32(txt_duracion.Text));

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Plan agregado con éxito.");
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

                    // Modificar plan existente
                    string query = "UPDATE plan SET nombre = @nombre, costo = @costo, duracionMeses = @duracionMeses " +
                                   "WHERE id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre", txt_nombrep.Text);
                        cmd.Parameters.AddWithValue("@costo", Convert.ToDecimal(txt_costo.Text));
                        cmd.Parameters.AddWithValue("@duracionMeses", Convert.ToInt32(txt_duracion.Text));
                        cmd.Parameters.AddWithValue("@id", dgv_plan.SelectedRows[0].Cells["id"].Value);  // Obtener ID de la fila seleccionada

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Plan modificado con éxito.");
                    LimpiarCampos();  // Limpiar los campos después de modificar
                }
            }
        }

        private void btn_eliminar_Click(object sender, EventArgs e)
        {
            if (dgv_plan.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un plan para eliminar.");
                return;
            }

            if (ConfirmarOperacion("eliminar"))
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Eliminar plan seleccionado
                    string query = "DELETE FROM plan WHERE id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", dgv_plan.SelectedRows[0].Cells["id"].Value);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Plan eliminado con éxito.");
                    LimpiarCampos();  // Limpiar los campos después de eliminar
                }
            }
        }

        private void btn_consultar_Click(object sender, EventArgs e)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT id, nombre, costo, duracionMeses FROM plan";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    dgv_plan.DataSource = dataTable;  // Mostrar los datos en el DataGridView
                }
            }
        }

        private void FormPlan_Load(object sender, EventArgs e)
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

        private void dgv_plan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                
                DataGridViewRow row = dgv_plan.Rows[e.RowIndex];

               
                txt_nombrep.Text = row.Cells["nombre"].Value.ToString();
                txt_costo.Text = row.Cells["costo"].Value.ToString();
                txt_duracion.Text = row.Cells["duracionMeses"].Value.ToString();
            }
        }

        private void btn_limpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }
    }
}
