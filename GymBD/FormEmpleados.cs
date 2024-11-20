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
    public partial class FormEmpleados : Form
    {

        private static FormEmpleados instancia = null;
        private string connectionString = "Server=localhost;Database=gymdb;User Id=root;Password=;";


        public static FormEmpleados ventana_unica()
        {
            if (instancia == null)
            {
                instancia = new FormEmpleados();

            }
            return instancia;
        }

        public FormEmpleados()
        {
            InitializeComponent();
            this.FormClosed += (s, e) => { instancia = null; };
        }

        
        private bool ValidarCampos()
        {
            if (string.IsNullOrEmpty(txt_id.Text) || string.IsNullOrEmpty(txt_nombre.Text) ||
                string.IsNullOrEmpty(txt_apellido.Text) || string.IsNullOrEmpty(txt_direccion.Text) ||
                string.IsNullOrEmpty(txt_telefono.Text) || string.IsNullOrEmpty(txt_email.Text) ||
                string.IsNullOrEmpty(txt_puesto.Text) || string.IsNullOrEmpty(txt_salario.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return false;
            }

            if (!int.TryParse(txt_id.Text, out _))
            {
                MessageBox.Show("El ID debe ser un número.");
                return false;
            }

            return true;
        }

        private bool ConfirmarOperacion(string operacion)
        {
            DialogResult result = MessageBox.Show($"¿Está seguro de {operacion} este empleado?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return result == DialogResult.Yes;
        }

        private void LimpiarCampos()
        {
            txt_id.Clear();
            txt_nombre.Clear();
            txt_apellido.Clear();
            txt_direccion.Clear();
            txt_telefono.Clear();
            txt_email.Clear();
            txt_puesto.Clear();
            txt_salario.Clear();
            txt_buscar_id.Clear();
            dtp_fnacimiento.Value = DateTime.Now;
            dtp_fcontratacion.Value = DateTime.Now;
            dgv_empleado.DataSource = null;
        }

        private void FormEmpleados_Load(object sender, EventArgs e)
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

        

        private void btn_agregar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Validar que el ID no exista ya
                    string queryValidarID = "SELECT COUNT(*) FROM persona WHERE id = @id";
                    using (var cmdValidar = new MySqlCommand(queryValidarID, conn))
                    {
                        cmdValidar.Parameters.AddWithValue("@id", txt_id.Text);
                        int count = Convert.ToInt32(cmdValidar.ExecuteScalar());
                        if (count > 0)
                        {
                            MessageBox.Show("El ID proporcionado ya existe. Por favor, elija otro.");
                            return;
                        }
                    }

                    using (var transaction = conn.BeginTransaction())
                    {
                        // Insertar en tabla `persona`
                        string queryPersona = "INSERT INTO persona (id, Nombre, Apellido, FechaNacimiento, Direccion, Telefono, Email) " +
                                              "VALUES (@id, @Nombre, @Apellido, @FechaNacimiento, @Direccion, @Telefono, @Email)";
                        using (var cmd = new MySqlCommand(queryPersona, conn))
                        {
                            cmd.Transaction = transaction;
                            cmd.Parameters.AddWithValue("@id", txt_id.Text);
                            cmd.Parameters.AddWithValue("@Nombre", txt_nombre.Text);
                            cmd.Parameters.AddWithValue("@Apellido", txt_apellido.Text);
                            cmd.Parameters.AddWithValue("@FechaNacimiento", dtp_fnacimiento.Value);
                            cmd.Parameters.AddWithValue("@Direccion", txt_direccion.Text);
                            cmd.Parameters.AddWithValue("@Telefono", txt_telefono.Text);
                            cmd.Parameters.AddWithValue("@Email", txt_email.Text);
                            cmd.ExecuteNonQuery();
                        }

                        // Insertar en tabla `empleado`
                        string queryEmpleado = "INSERT INTO empleado (id, puesto, salario, fechaContratacion) " +
                                               "VALUES (@id, @puesto, @salario, @fechaContratacion)";
                        using (var cmdEmpleado = new MySqlCommand(queryEmpleado, conn))
                        {
                            cmdEmpleado.Transaction = transaction;
                            cmdEmpleado.Parameters.AddWithValue("@id", txt_id.Text);
                            cmdEmpleado.Parameters.AddWithValue("@puesto", txt_puesto.Text);
                            cmdEmpleado.Parameters.AddWithValue("@salario", txt_salario.Text);
                            cmdEmpleado.Parameters.AddWithValue("@fechaContratacion", dtp_fcontratacion.Value);
                            cmdEmpleado.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show("Empleado agregado exitosamente.");
                        LimpiarCampos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar el empleado: {ex.Message}");
            }
        }

        private void btn_modificar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            if (ConfirmarOperacion("modificar"))
            {
                try
                {
                    using (var conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();

                        string queryPersona = "UPDATE persona SET Nombre = @Nombre, Apellido = @Apellido, FechaNacimiento = @FechaNacimiento, " +
                                              "Direccion = @Direccion, Telefono = @Telefono, Email = @Email WHERE id = @id";
                        using (var cmd = new MySqlCommand(queryPersona, conn))
                        {
                            cmd.Parameters.AddWithValue("@Nombre", txt_nombre.Text);
                            cmd.Parameters.AddWithValue("@Apellido", txt_apellido.Text);
                            cmd.Parameters.AddWithValue("@FechaNacimiento", dtp_fnacimiento.Value);
                            cmd.Parameters.AddWithValue("@Direccion", txt_direccion.Text);
                            cmd.Parameters.AddWithValue("@Telefono", txt_telefono.Text);
                            cmd.Parameters.AddWithValue("@Email", txt_email.Text);
                            cmd.Parameters.AddWithValue("@id", txt_id.Text);
                            cmd.ExecuteNonQuery();
                        }

                        string queryEmpleado = "UPDATE empleado SET puesto = @puesto, salario = @salario, fechaContratacion = @fechaContratacion " +
                                               "WHERE id = @id";
                        using (var cmdEmpleado = new MySqlCommand(queryEmpleado, conn))
                        {
                            cmdEmpleado.Parameters.AddWithValue("@puesto", txt_puesto.Text);
                            cmdEmpleado.Parameters.AddWithValue("@salario", txt_salario.Text);
                            cmdEmpleado.Parameters.AddWithValue("@fechaContratacion", dtp_fcontratacion.Value);
                            cmdEmpleado.Parameters.AddWithValue("@id", txt_id.Text);
                            cmdEmpleado.ExecuteNonQuery();
                        }

                        MessageBox.Show("Empleado modificado con éxito.");
                        LimpiarCampos();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al modificar el empleado: {ex.Message}");
                }
            }
        }

        private void btn_eliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_id.Text))
            {
                MessageBox.Show("Por favor, seleccione un empleado de la lista para eliminar.");
                return;
            }

            if (ConfirmarOperacion("eliminar"))
            {
                try
                {
                    using (var conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();

                        string queryEmpleado = "DELETE FROM empleado WHERE id = @id";
                        using (var cmd = new MySqlCommand(queryEmpleado, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", txt_id.Text);
                            cmd.ExecuteNonQuery();
                        }

                        string queryPersona = "DELETE FROM persona WHERE id = @id";
                        using (var cmd = new MySqlCommand(queryPersona, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", txt_id.Text);
                            cmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Empleado eliminado con éxito.");
                        LimpiarCampos();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar el empleado: {ex.Message}");
                }
            }
        }

        private void btn_consultar_Click(object sender, EventArgs e)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT p.id, p.Nombre, p.Apellido, p.FechaNacimiento, p.Direccion, p.Telefono, p.Email, e.puesto, e.salario, e.fechaContratacion " +
                               "FROM persona p INNER JOIN empleado e ON p.id = e.id  WHERE p.id < 100";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    dgv_empleado.DataSource = dataTable;
                }
            }
        }

        private void dgv_empleado_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow filaSeleccionada = dgv_empleado.Rows[e.RowIndex];

                txt_id.Text = filaSeleccionada.Cells["id"].Value?.ToString();
                txt_nombre.Text = filaSeleccionada.Cells["Nombre"].Value?.ToString();
                txt_apellido.Text = filaSeleccionada.Cells["Apellido"].Value?.ToString();
                txt_telefono.Text = filaSeleccionada.Cells["Telefono"].Value?.ToString();
                txt_email.Text = filaSeleccionada.Cells["Email"].Value?.ToString();
                txt_puesto.Text = filaSeleccionada.Cells["Puesto"].Value?.ToString();
                txt_salario.Text = filaSeleccionada.Cells["Salario"].Value?.ToString();
                txt_direccion.Text = filaSeleccionada.Cells["Direccion"].Value?.ToString();
                dtp_fcontratacion.Value = filaSeleccionada.Cells["FechaContratacion"].Value != null
                    ? Convert.ToDateTime(filaSeleccionada.Cells["FechaContratacion"].Value)
                    : DateTime.Now;
                dtp_fnacimiento.Value = filaSeleccionada.Cells["FechaNacimiento"].Value != null
                    ? Convert.ToDateTime(filaSeleccionada.Cells["FechaNacimiento"].Value)
                    : DateTime.Now;
            }
        }

        private void btn_limpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void btn_buscar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_buscar_id.Text))
            {
                MessageBox.Show("Por favor, ingrese un ID para buscar.");
                return;
            }

            if (!int.TryParse(txt_buscar_id.Text, out _))
            {
                MessageBox.Show("El ID debe ser un número válido.");
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                   
                    string query = "SELECT p.id, p.Nombre, p.Apellido, p.FechaNacimiento, p.Direccion, p.Telefono, p.Email, " +
                                   "e.puesto, e.salario, e.fechaContratacion " +
                                   "FROM persona p INNER JOIN empleado e ON p.id = e.id " +
                                   "WHERE p.id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txt_buscar_id.Text);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                
                                txt_id.Text = reader["id"].ToString();
                                txt_nombre.Text = reader["Nombre"].ToString();
                                txt_apellido.Text = reader["Apellido"].ToString();
                                dtp_fnacimiento.Value = Convert.ToDateTime(reader["FechaNacimiento"]);
                                txt_direccion.Text = reader["Direccion"].ToString();
                                txt_telefono.Text = reader["Telefono"].ToString();
                                txt_email.Text = reader["Email"].ToString();
                                txt_puesto.Text = reader["puesto"].ToString();
                                txt_salario.Text = reader["salario"].ToString();
                                dtp_fcontratacion.Value = Convert.ToDateTime(reader["fechaContratacion"]);
                            }
                            else
                            {
                                MessageBox.Show("No se encontró ningún empleado con el ID proporcionado.");
                                return;
                            }
                        }
                    }

                    
                    string queryDGV = "SELECT p.id, p.Nombre, p.Apellido, p.FechaNacimiento, p.Direccion, p.Telefono, p.Email, " +
                                      "e.puesto, e.salario, e.fechaContratacion " +
                                      "FROM persona p INNER JOIN empleado e ON p.id = e.id " +
                                      "WHERE p.id = @id";
                    using (var cmdDGV = new MySqlCommand(queryDGV, conn))
                    {
                        cmdDGV.Parameters.AddWithValue("@id", txt_buscar_id.Text);
                        MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmdDGV);
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);
                        dgv_empleado.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar el empleado: {ex.Message}");
            }
        }
    }
}
