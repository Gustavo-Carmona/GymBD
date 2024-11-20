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
    public partial class FormMantenimiento : Form
    {
        private static FormMantenimiento instancia = null;
        private string connectionString = "Server=localhost;Database=gymdb;User Id=root;Password=;";   


        public static FormMantenimiento ventana_unica()
        {
            if (instancia == null)
            {
                instancia = new FormMantenimiento();

            }
            return instancia;
        }

        public FormMantenimiento()
        {
            InitializeComponent();
            this.FormClosed += (s, e) => { instancia = null; };

        }

       
        private bool ValidarCampos()
        {
            if (string.IsNullOrEmpty(txt_descripcion.Text) || cmb_maquina.SelectedValue == null)
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return false;
            }
            return true;
        }

        private bool ConfirmarOperacion(string operacion)
        {
            DialogResult result = MessageBox.Show($"¿Está seguro de {operacion} este mantenimiento?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return result == DialogResult.Yes;
        }

        private void LimpiarCampos()
        {
           
            txt_descripcion.Clear();

            
            cmb_maquina.SelectedIndex = -1;  

            
            dtp_fmantenimiento.Value = DateTime.Now;  

            
            dgv_mantenimiento.DataSource = null;  
        }

        private void FormMantenimiento_Load(object sender, EventArgs e)
        {
            CargarMaquinas();
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

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void dtp_fmantenimiento_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btn_agregar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;  // Validar los campos antes de continuar

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Insertar nuevo mantenimiento
                string query = "INSERT INTO mantenimiento (id_maquina, descripcion, fecha) " +
                               "VALUES (@id_maquina, @descripcion, @fecha)";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id_maquina", cmb_maquina.SelectedValue);
                    cmd.Parameters.AddWithValue("@descripcion", txt_descripcion.Text);
                    cmd.Parameters.AddWithValue("@fecha", dtp_fmantenimiento.Value);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Mantenimiento agregado con éxito.");
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

                    // Modificar mantenimiento existente
                    string query = "UPDATE mantenimiento SET id_maquina = @id_maquina, descripcion = @descripcion, fecha = @fecha " +
                                   "WHERE id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_maquina", cmb_maquina.SelectedValue);
                        cmd.Parameters.AddWithValue("@descripcion", txt_descripcion.Text);
                        cmd.Parameters.AddWithValue("@fecha", dtp_fmantenimiento.Value);
                        cmd.Parameters.AddWithValue("@id", dgv_mantenimiento.SelectedRows[0].Cells["id"].Value);  // Obtener ID de la fila seleccionada

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Mantenimiento modificado con éxito.");
                    LimpiarCampos();  // Limpiar los campos después de modificar
                }
            }
        }

        private void btn_eliminar_Click(object sender, EventArgs e)
        {
            if (dgv_mantenimiento.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un mantenimiento para eliminar.");
                return;
            }

            if (ConfirmarOperacion("eliminar"))
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Eliminar mantenimiento seleccionado
                    string query = "DELETE FROM mantenimiento WHERE id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", dgv_mantenimiento.SelectedRows[0].Cells["id"].Value);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Mantenimiento eliminado con éxito.");
                    LimpiarCampos();  // Limpiar los campos después de eliminar
                }
            }
        }

        private void btn_consultar_Click(object sender, EventArgs e)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT m.id, ma.nombre AS maquina, m.descripcion, m.fecha " +
                               "FROM mantenimiento m " +
                               "JOIN maquina ma ON m.id_maquina = ma.id";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    dgv_mantenimiento.DataSource = dataTable;  // Mostrar los datos en el DataGridView
                }
            }
        }

        private void CargarMaquinas()
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Consulta para obtener las máquinas
                string query = "SELECT id, nombre FROM maquina";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    // Vincular los datos al ComboBox
                    cmb_maquina.DataSource = dataTable;
                    cmb_maquina.DisplayMember = "nombre"; // Campo a mostrar
                    cmb_maquina.ValueMember = "id";      // Valor asociado
                    cmb_maquina.SelectedIndex = -1;      // Ningún elemento seleccionado por defecto
                }
            }
        }

        private void dgv_mantenimiento_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) 
            {
               
                DataGridViewRow filaSeleccionada = dgv_mantenimiento.Rows[e.RowIndex];

                // Cargar los datos en los controles
               
                txt_descripcion.Text = filaSeleccionada.Cells["descripcion"].Value.ToString(); 
                dtp_fmantenimiento.Value = Convert.ToDateTime(filaSeleccionada.Cells["fecha"].Value); 
            }
        }

        private void btn_limpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }
    }
}
