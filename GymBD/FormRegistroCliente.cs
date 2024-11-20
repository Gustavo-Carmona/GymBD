using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;
using MySqlConnector;
using static GymBD.FormRegistroCliente;

namespace GymBD
{
    public partial class FormRegistroCliente : Form
    {
        private static FormRegistroCliente instancia = null;
        private string connectionString = "server=localhost;database=gymdb;User Id=root;pwd=;";



        public static FormRegistroCliente ventana_unica()
        {
            if (instancia == null)
            {
                instancia = new FormRegistroCliente();

            }
            return instancia;
        }

        public FormRegistroCliente()
        {
            InitializeComponent();
            this.FormClosed += (s, e) => { instancia = null; };
        }

        private void FormRegistroCliente_Load(object sender, EventArgs e)
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

        private bool ValidarCampos()
        {
            if (string.IsNullOrEmpty(txt_id.Text) || string.IsNullOrEmpty(txt_nombre.Text) || string.IsNullOrEmpty(txt_apellidos.Text) ||
                string.IsNullOrEmpty(txt_direccion.Text) || string.IsNullOrEmpty(txt_telefono.Text) || string.IsNullOrEmpty(txt_email.Text) ||
                string.IsNullOrEmpty(txt_plan.Text) || dtp_fnacimiento.Value == null)
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return false;
            }
            return true;
        }

        private bool ConfirmarOperacion(string operacion)
        {
            DialogResult result = MessageBox.Show($"¿Está seguro de {operacion} este cliente?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return result == DialogResult.Yes;
        }

        private void LimpiarCampos()
        {
            txt_id.Clear();
            txt_nombre.Clear();
            txt_apellidos.Clear();
            txt_direccion.Clear();
            txt_telefono.Clear();
            txt_email.Clear();
            txt_plan.Clear();
            dtp_fnacimiento.Value = DateTime.Now;
            dtp_fregistro.Value = DateTime.Now;
            txt_buscar.Clear();

            dgv_cliente.DataSource = null;  
        }

        // boton modificar
        private void button2_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            if (ConfirmarOperacion("modificar"))
            {
                try
                {
                    using (var conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();

                        // Modificar datos en la tabla persona
                        string queryPersona = "UPDATE persona SET Nombre = @Nombre, Apellido = @Apellido, FechaNacimiento = @FechaNacimiento, " +
                                              "Direccion = @Direccion, Telefono = @Telefono, Email = @Email WHERE id = @id";

                        using (var cmd = new MySqlCommand(queryPersona, conn))
                        {
                            cmd.Parameters.AddWithValue("@Nombre", txt_nombre.Text);
                            cmd.Parameters.AddWithValue("@Apellido", txt_apellidos.Text);
                            cmd.Parameters.AddWithValue("@FechaNacimiento", dtp_fnacimiento.Value);
                            cmd.Parameters.AddWithValue("@Direccion", txt_direccion.Text);
                            cmd.Parameters.AddWithValue("@Telefono", txt_telefono.Text);
                            cmd.Parameters.AddWithValue("@Email", txt_email.Text);
                            cmd.Parameters.AddWithValue("@id", txt_id.Text);

                            cmd.ExecuteNonQuery();
                        }

                        
                        string queryCliente = "UPDATE cliente SET fechaRegistro = @fechaRegistro, id_plan = @id_plan";

                        using (var cmd = new MySqlCommand(queryCliente, conn))
                        {
                            cmd.Parameters.AddWithValue("@fechaRegistro", dtp_fregistro.Value);
                            cmd.Parameters.AddWithValue("@id_plan", txt_plan.Text);
                           

                            cmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Cliente modificado con éxito.");
                        LimpiarCampos();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al modificar el cliente: {ex.Message}");
                }
            }
        }

        private void btn_agregar_Click(object sender, EventArgs e)
        {
            string connectionString = "server = localhost; database = gymdb; uid = root; pwd =; ";
            MySqlConnection conn = new MySqlConnection(connectionString);

            try
            {
                // Validar que el ID no esté vacío
                if (string.IsNullOrEmpty(txt_id.Text))
                {
                    MessageBox.Show("El ID no puede estar vacío.");
                    return;
                }

                // Verificar si el ID ya existe en la base de datos
                conn.Open();
                string queryVerificarId = "SELECT COUNT(*) FROM persona WHERE id = @id";
                using (var cmdVerificar = new MySqlCommand(queryVerificarId, conn))
                {
                    cmdVerificar.Parameters.AddWithValue("@id", txt_id.Text);
                    int count = Convert.ToInt32(cmdVerificar.ExecuteScalar());
                    if (count > 0)
                    {
                        MessageBox.Show("El ID ingresado ya existe.");
                        return;
                    }
                }

                // Abrir conexión nuevamente para la transacción
                conn.Close();
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    string queryPersona = "INSERT INTO persona (id, Nombre, Apellido, FechaNacimiento, Direccion, Telefono, Email) " +
                                          "VALUES (@id, @Nombre, @Apellido, @FechaNacimiento, @Direccion, @Telefono, @Email)";
                    using (var cmd = new MySqlCommand(queryPersona, conn))
                    {
                        cmd.Transaction = transaction;

                        cmd.Parameters.AddWithValue("@id", txt_id.Text);  // Usar el ID proporcionado
                        cmd.Parameters.AddWithValue("@Nombre", txt_nombre.Text);
                        cmd.Parameters.AddWithValue("@Apellido", txt_apellidos.Text);
                        cmd.Parameters.AddWithValue("@FechaNacimiento", dtp_fnacimiento.Value);
                        cmd.Parameters.AddWithValue("@Direccion", txt_direccion.Text);
                        cmd.Parameters.AddWithValue("@Telefono", txt_telefono.Text);
                        cmd.Parameters.AddWithValue("@Email", txt_email.Text);

                        cmd.ExecuteNonQuery();
                    }

                    string queryCliente = "INSERT INTO cliente (id, fechaRegistro, id_plan) " +
                                          "VALUES (@id, @fechaRegistro, @id_plan)";
                    using (var cmdCliente = new MySqlCommand(queryCliente, conn))
                    {
                        cmdCliente.Transaction = transaction;
                        cmdCliente.Parameters.AddWithValue("@id", txt_id.Text);  
                        cmdCliente.Parameters.AddWithValue("@fechaRegistro", dtp_fregistro.Value);
                        cmdCliente.Parameters.AddWithValue("@id_plan", txt_plan.Text);

                        cmdCliente.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }

                MessageBox.Show("Datos agregados exitosamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar los datos: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void btn_eliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_id.Text))
            {
                MessageBox.Show("Por favor, seleccione un cliente de la lista para eliminar.");
                return;
            }

            if (ConfirmarOperacion("eliminar"))
            {
                try
                {
                    using (var conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();

                        // Eliminar los datos de la tabla cliente
                        string queryEliminarCliente = "DELETE FROM cliente WHERE id = @id";
                        using (var cmd = new MySqlCommand(queryEliminarCliente, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", txt_id.Text);
                            cmd.ExecuteNonQuery();
                        }

                        // Eliminar los datos de la tabla persona
                        string queryEliminarPersona = "DELETE FROM persona WHERE id = @id";
                        using (var cmd = new MySqlCommand(queryEliminarPersona, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", txt_id.Text);
                            cmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Cliente eliminado con éxito.");
                        LimpiarCampos(); 
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar el cliente: {ex.Message}");
                }
            }
        }

        private void btn_consultar_Click(object sender, EventArgs e)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT p.id, p.Nombre, p.Apellido, p.FechaNacimiento, p.Direccion, p.Telefono, p.Email, " +
               "c.fechaRegistro, c.id_plan " +
               "FROM persona p " +
               "INNER JOIN cliente c ON p.id = c.id " +
               "WHERE p.id >= 100";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    dgv_cliente.DataSource = dataTable;
                }
            }
        }


        private void dgv_cliente_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow filaSeleccionada = dgv_cliente.Rows[e.RowIndex];

                txt_id.Text = filaSeleccionada.Cells["id"].Value?.ToString();
                txt_nombre.Text = filaSeleccionada.Cells["Nombre"].Value?.ToString();
                txt_apellidos.Text = filaSeleccionada.Cells["Apellido"].Value?.ToString();
                dtp_fnacimiento.Value = filaSeleccionada.Cells["FechaNacimiento"].Value != null
                    ? Convert.ToDateTime(filaSeleccionada.Cells["FechaNacimiento"].Value)
                    : DateTime.Now;
                txt_direccion.Text = filaSeleccionada.Cells["Direccion"].Value?.ToString();
                txt_telefono.Text = filaSeleccionada.Cells["Telefono"].Value?.ToString();
                txt_email.Text = filaSeleccionada.Cells["Email"].Value?.ToString();
                dtp_fregistro.Value = filaSeleccionada.Cells["fechaRegistro"].Value != null
                    ? Convert.ToDateTime(filaSeleccionada.Cells["fechaRegistro"].Value)
                    : DateTime.Now;
                txt_plan.Text = filaSeleccionada.Cells["id_plan"].Value?.ToString();
            }
        }

        private void txt_limpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void txt_buscar_id_Click(object sender, EventArgs e)
        {
            string buscarId = txt_buscar.Text;

           
            if (string.IsNullOrEmpty(buscarId))
            {
                MessageBox.Show("Por favor, ingrese un ID para buscar.");
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                   
                    string query = "SELECT p.id, p.Nombre, p.Apellido, p.FechaNacimiento, p.Direccion, p.Telefono, p.Email, " +
                                   "c.fechaRegistro, c.id_plan " +
                                   "FROM persona p " +
                                   "INNER JOIN cliente c ON p.id = c.id " +
                                   "WHERE p.id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", buscarId);

                        MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        if (dataTable.Rows.Count > 0)
                        {
                            
                            dgv_cliente.DataSource = dataTable;

                           
                            DataRow fila = dataTable.Rows[0];
                            txt_id.Text = fila["id"].ToString();
                            txt_nombre.Text = fila["Nombre"].ToString();
                            txt_apellidos.Text = fila["Apellido"].ToString();
                            dtp_fnacimiento.Value = fila["FechaNacimiento"] != DBNull.Value
                                ? Convert.ToDateTime(fila["FechaNacimiento"])
                                : DateTime.Now;
                            txt_direccion.Text = fila["Direccion"].ToString();
                            txt_telefono.Text = fila["Telefono"].ToString();
                            txt_email.Text = fila["Email"].ToString();
                            dtp_fregistro.Value = fila["fechaRegistro"] != DBNull.Value
                                ? Convert.ToDateTime(fila["fechaRegistro"])
                                : DateTime.Now;
                            txt_plan.Text = fila["id_plan"].ToString();
                        }
                        else
                        {
                            MessageBox.Show("No se encontró ningún cliente con ese ID.");
                            LimpiarCampos();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar el cliente: {ex.Message}");
            }
        }
    }
}
