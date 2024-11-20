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
    public partial class FormFichaMedica : Form
    {
        private static FormFichaMedica instancia = null;
        private string connectionString = "Server=localhost;Database=gymdb;User Id=root;Password=;";


        public static FormFichaMedica ventana_unica()
        {
            if (instancia == null)
            {
                instancia = new FormFichaMedica();

            }
            return instancia;
        }
        public FormFichaMedica()
        {
            InitializeComponent();
            this.FormClosed += (s, e) => { instancia = null; };
        }

      

        private bool ValidarCampos()
        {
            if (string.IsNullOrEmpty(txt_id.Text) || string.IsNullOrEmpty(txt_peso.Text) || string.IsNullOrEmpty(txt_talla.Text) || string.IsNullOrEmpty(txt_grasacorp.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return false;
            }
            return true;
        }

        private bool ConfirmarOperacion(string operacion)
        {
            DialogResult result = MessageBox.Show($"¿Está seguro de {operacion} esta ficha médica?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return result == DialogResult.Yes;
        }


        private void LimpiarCampos()
        {
            txt_id.Clear();
            txt_peso.Clear();
            txt_talla.Clear();
            txt_grasacorp.Clear();
            txt_buscar_id.Clear();
           
            dtp_fregistro.Value = DateTime.Now;

           
            fgv_fmedica.DataSource = null;  
        }
        private void FormFichaMedica_Load(object sender, EventArgs e)
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
            if (!ValidarCampos()) return; // Asume que tienes un método para validar campos

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "INSERT INTO fichamedica (peso, talla, porcentajeGrasaCorporal, fechaRegistro, id_cliente) " +
                                   "VALUES (@peso, @talla, @porcentajeGrasaCorporal, @fechaRegistro, @id_cliente)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@peso", txt_peso.Text);
                        cmd.Parameters.AddWithValue("@talla", txt_talla.Text);
                        cmd.Parameters.AddWithValue("@porcentajeGrasaCorporal", txt_grasacorp.Text);
                        cmd.Parameters.AddWithValue("@fechaRegistro", dtp_fregistro.Value);
                        cmd.Parameters.AddWithValue("@id_cliente", txt_id.Text);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Ficha médica agregada con éxito.");
                    LimpiarCampos(); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar la ficha médica: {ex.Message}");
            }
        }

        private void btn_modificar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;  

            if (ConfirmarOperacion("modificar"))
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Modificar ficha médica existente
                    string query = "UPDATE fichamedica SET peso = @peso, talla = @talla, porcentajeGrasaCorporal = @porcentajeGrasaCorporal, fechaRegistro = @fechaRegistro " +
                                   "WHERE id_cliente = @id_cliente";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@peso", txt_peso.Text);
                        cmd.Parameters.AddWithValue("@talla", txt_talla.Text);
                        cmd.Parameters.AddWithValue("@porcentajeGrasaCorporal", txt_grasacorp.Text);
                        cmd.Parameters.AddWithValue("@fechaRegistro", dtp_fregistro.Value); // Para DateTimePicker
                        cmd.Parameters.AddWithValue("@id_cliente", txt_id.Text);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Ficha médica modificada con éxito.");
                    LimpiarCampos();  
                }
            }
        }

        private void btn_eliminar_Click(object sender, EventArgs e)
        {
            if (fgv_fmedica.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione una ficha medica para eliminar.");
                return;
            }

            if (ConfirmarOperacion("eliminar"))
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Eliminar administrador seleccionado
                    string query = "DELETE FROM fichamedica WHERE id_cliente = @id_cliente";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_cliente", fgv_fmedica.SelectedRows[0].Cells["id_cliente"].Value);
                        cmd.Parameters.AddWithValue("@porcentajeGrasaCorporal", txt_grasacorp.Text);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Ficha medica eliminado con éxito.");
                    LimpiarCampos();  
                }
            }
        }

        private void btn_consultar_Click(object sender, EventArgs e)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Consulta para obtener las fichas médicas
                string query = "SELECT id_cliente, peso, talla, porcentajeGrasaCorporal, fechaRegistro FROM fichamedica";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    fgv_fmedica.DataSource = dataTable;  // Mostrar los datos en el DataGridView
                }
            }
        }

        private void fgv_fmedica_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) 
            {
                DataGridViewRow filaSeleccionada = fgv_fmedica.Rows[e.RowIndex];

                
                txt_peso.Text = filaSeleccionada.Cells["peso"].Value?.ToString();
                txt_talla.Text = filaSeleccionada.Cells["talla"].Value?.ToString();
                txt_grasacorp.Text = filaSeleccionada.Cells["porcentajeGrasaCorporal"].Value?.ToString();
                dtp_fregistro.Value = filaSeleccionada.Cells["fechaRegistro"].Value != null
                    ? Convert.ToDateTime(filaSeleccionada.Cells["fechaRegistro"].Value)
                    : DateTime.Now; 
            }
        }

        private void btn_limpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void btn_buscar_Click(object sender, EventArgs e)
        {
            string idCliente = txt_buscar_id.Text.Trim();

            if (string.IsNullOrEmpty(idCliente))
            {
                MessageBox.Show("Por favor, ingrese un ID para buscar.");
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    
                    string query = "SELECT id_cliente, peso, talla, porcentajeGrasaCorporal, fechaRegistro FROM fichamedica WHERE id_cliente = @id_cliente";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_cliente", idCliente);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                
                                txt_id.Text = reader["id_cliente"].ToString();
                                txt_peso.Text = reader["peso"].ToString();
                                txt_talla.Text = reader["talla"].ToString();
                                txt_grasacorp.Text = reader["porcentajeGrasaCorporal"].ToString();
                                dtp_fregistro.Value = Convert.ToDateTime(reader["fechaRegistro"]);
                            }
                            else
                            {
                                MessageBox.Show("No se encontró ninguna ficha médica con el ID proporcionado.");
                                LimpiarCampos(); 
                                return;
                            }
                        }
                    }

                    
                    using (var adapter = new MySqlDataAdapter(query, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@id_cliente", idCliente);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        fgv_fmedica.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar la ficha médica: {ex.Message}");
            }
        }
    }
}
