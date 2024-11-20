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
    public partial class FormMaquina : Form
    {

        private static FormMaquina instancia = null;
        private string connectionString = "Server=localhost;Database=gymdb;User Id=root;Password=;";


        public static FormMaquina ventana_unica()
        {
            if (instancia == null)
            {
                instancia = new FormMaquina();

            }
            return instancia;
        }
        public FormMaquina()
        {
            InitializeComponent();
            this.FormClosed += (s, e) => { instancia = null; };
        }

        
        private bool ValidarCampos()
        {
            if (string.IsNullOrEmpty(txt_nombre.Text) || cmb_area.SelectedValue == null)
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return false;
            }
            return true;
        }

        private void LimpiarCampos()
        {
            
            txt_nombre.Clear();

           
            cmb_area.SelectedIndex = -1;  

            
            dtp_fadquisicion.Value = DateTime.Now;  

            
            dgv_maquina.DataSource = null;  
        }

        private void btn_agregar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;  

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Insertar nueva máquina
                string query = "INSERT INTO maquina (nombre, id_area, fechaAdquisicion) " +
                               "VALUES (@nombre, @id_area, @fechaAdquisicion)";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", txt_nombre.Text);
                    cmd.Parameters.AddWithValue("@id_area", cmb_area.SelectedValue);
                    cmd.Parameters.AddWithValue("@fechaAdquisicion", dtp_fadquisicion.Value);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Máquina agregada con éxito.");
                LimpiarCampos();  // Limpiar los campos después de agregar
            }
        }

        private bool ConfirmarOperacion(string operacion)
        {
            DialogResult result = MessageBox.Show($"¿Está seguro de {operacion} esta máquina?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return result == DialogResult.Yes;
        }

        private void btn_modificar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;  // Validar los campos antes de continuar

            if (ConfirmarOperacion("modificar"))
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Modificar máquina existente
                    string query = "UPDATE maquina SET nombre = @nombre, id_area = @id_area, fechaAdquisicion = @fechaAdquisicion " +
                                   "WHERE id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre", txt_nombre.Text);
                        cmd.Parameters.AddWithValue("@id_area", cmb_area.SelectedValue);
                        cmd.Parameters.AddWithValue("@fechaAdquisicion", dtp_fadquisicion.Value);
                        cmd.Parameters.AddWithValue("@id", dgv_maquina.SelectedRows[0].Cells["id"].Value);  // Obtener ID de la fila seleccionada

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Máquina modificada con éxito.");
                    LimpiarCampos();  // Limpiar los campos después de modificar
                }
            }
        }

        private void btn_eliminar_Click(object sender, EventArgs e)
        {
            if (dgv_maquina.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione una máquina para eliminar.");
                return;
            }

            if (ConfirmarOperacion("eliminar"))
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Eliminar máquina seleccionada
                    string query = "DELETE FROM maquina WHERE id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", dgv_maquina.SelectedRows[0].Cells["id"].Value);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Máquina eliminada con éxito.");
                    LimpiarCampos();  // Limpiar los campos después de eliminar
                }
            }
        }

        private void btn_consultar_Click(object sender, EventArgs e)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT m.id, m.nombre, a.nombre AS area, m.fechaAdquisicion " +
                               "FROM maquina m " +
                               "JOIN area a ON m.id_area = a.id";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    dgv_maquina.DataSource = dataTable;  // Mostrar los datos en el DataGridView
                }
            }
        }

        private void LlenarComboBoxAreas()
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Consulta para obtener los IDs y nombres de las áreas
                    string query = "SELECT id, nombre FROM area";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            // Crea una lista para almacenar los datos
                            var listaAreas = new List<KeyValuePair<int, string>>();

                            while (reader.Read())
                            {
                                // Agrega cada área a la lista (id, nombre)
                                listaAreas.Add(new KeyValuePair<int, string>(
                                    reader.GetInt32("id"),
                                    reader.GetString("nombre")
                                ));
                            }

                            // Asigna la lista como fuente de datos del ComboBox
                            cmb_area.DataSource = listaAreas;
                            cmb_area.DisplayMember = "Value"; // Muestra el nombre del área
                            cmb_area.ValueMember = "Key";   // Utiliza el ID como valor
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar las áreas: " + ex.Message);
                }
            }
        }
        private void FormMaquina_Load(object sender, EventArgs e)
        {
            LlenarComboBoxAreas();
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

        private void dgv_maquina_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgv_maquina.Rows[e.RowIndex].Cells["id"].Value != null)
            {
                
                DataGridViewRow filaSeleccionada = dgv_maquina.Rows[e.RowIndex];

                try
                {
                    
                    txt_nombre.Text = filaSeleccionada.Cells["nombre"].Value.ToString();                 
                    dtp_fadquisicion.Value = Convert.ToDateTime(filaSeleccionada.Cells["fechaAdquisicion"].Value);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los datos de la máquina: " + ex.Message);
                }
            }
        }

        private void btn_limpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }
    }
}
