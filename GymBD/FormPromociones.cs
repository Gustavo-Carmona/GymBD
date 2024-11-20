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
    public partial class FormPromociones : Form
    {
        private static FormPromociones instancia = null;
        private string connectionString = "Server=localhost;Database=gymdb;User Id=root;Password=;";


        public static FormPromociones ventana_unica()
        {
            if (instancia == null)
            {
                instancia = new FormPromociones();

            }
            return instancia;
        }
        public FormPromociones()
        {
            InitializeComponent();
            this.FormClosed += (s, e) => { instancia = null; };

        }

       
        private bool ValidarCampos()
        {
            if (string.IsNullOrEmpty(txt_descripcion.Text) || string.IsNullOrEmpty(txt_descuento.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return false;
            }

            // Validar que el descuento sea un número válido
            decimal descuento;
            if (!decimal.TryParse(txt_descuento.Text, out descuento) || descuento < 0 || descuento > 100)
            {
                MessageBox.Show("El descuento debe ser un número entre 0 y 100.");
                return false;
            }

            if (dtp_inicio.Value >= dtp_fin.Value)
            {
                MessageBox.Show("La fecha de inicio debe ser anterior a la fecha de fin.");
                return false;
            }

            return true;
        }

        private bool ConfirmarOperacion(string operacion)
        {
            DialogResult result = MessageBox.Show($"¿Está seguro de {operacion} esta promoción?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return result == DialogResult.Yes;
        }

        private void LimpiarCampos()
        {
            txt_descripcion.Clear();     
            txt_descuento.Clear();        
            dtp_inicio.Value = DateTime.Now;  
            dtp_fin.Value = DateTime.Now;

            dgv_promociones.DataSource = null;
        }


        private void FormPromociones_Load(object sender, EventArgs e)
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

                // Insertar nueva promoción
                string query = "INSERT INTO promocion (descripcion, descuento, fechaInicio, fechaFin) " +
                               "VALUES (@descripcion, @descuento, @fechaInicio, @fechaFin)";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@descripcion", txt_descripcion.Text);
                    cmd.Parameters.AddWithValue("@descuento", Convert.ToDecimal(txt_descuento.Text));
                    cmd.Parameters.AddWithValue("@fechaInicio", dtp_inicio.Value);
                    cmd.Parameters.AddWithValue("@fechaFin", dtp_fin.Value);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Promoción agregada con éxito.");
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

                    // Modificar promoción existente
                    string query = "UPDATE promocion SET descripcion = @descripcion, descuento = @descuento, " +
                                   "fechaInicio = @fechaInicio, fechaFin = @fechaFin WHERE id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@descripcion", txt_descripcion.Text);
                        cmd.Parameters.AddWithValue("@descuento", Convert.ToDecimal(txt_descuento.Text));
                        cmd.Parameters.AddWithValue("@fechaInicio", dtp_inicio.Value);
                        cmd.Parameters.AddWithValue("@fechaFin", dtp_fin.Value);
                        cmd.Parameters.AddWithValue("@id", dgv_promociones.SelectedRows[0].Cells["id"].Value);  // Obtener ID de la fila seleccionada

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Promoción modificada con éxito.");
                    LimpiarCampos();  // Limpiar los campos después de modificar
                }
            }
        }

        private void btn_eliminar_Click(object sender, EventArgs e)
        {
            if (dgv_promociones.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione una promoción para eliminar.");
                return;
            }

            if (ConfirmarOperacion("eliminar"))
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Eliminar promoción seleccionada
                    string query = "DELETE FROM promocion WHERE id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", dgv_promociones.SelectedRows[0].Cells["id"].Value);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Promoción eliminada con éxito.");
                    LimpiarCampos();  // Limpiar los campos después de eliminar
                }
            }
        }

        private void btn_consultar_Click(object sender, EventArgs e)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT id, descripcion, descuento, fechaInicio, fechaFin FROM promocion";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    dgv_promociones.DataSource = dataTable;  // Mostrar los datos en el DataGridView
                }
            }
        }

        private void dgv_promociones_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.RowIndex >= 0)
            {
                
                DataGridViewRow row = dgv_promociones.Rows[e.RowIndex];
              
                txt_descripcion.Text = row.Cells["descripcion"].Value.ToString();
                txt_descuento.Text = row.Cells["descuento"].Value.ToString();
                dtp_inicio.Value = Convert.ToDateTime(row.Cells["fechaInicio"].Value);
                dtp_fin.Value = Convert.ToDateTime(row.Cells["fechaFin"].Value);
            }
        }

        private void btn_limpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }
    }
}
