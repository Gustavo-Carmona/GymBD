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
    public partial class FormMenuGym : Form
    {
        public FormMenuGym()
        {
            InitializeComponent();
        }

        private void clienteToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void formularioDeRegistroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRegistroCliente frm =  FormRegistroCliente.ventana_unica();
            frm.Show();
            frm.MdiParent =this;
            frm.BringToFront();
        }

        private void registroDeEmpleadoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormEmpleados frm = FormEmpleados.ventana_unica();
            frm.Show();
            frm.MdiParent = this;
            frm.BringToFront();
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAdmin frm = FormAdmin.ventana_unica();
            frm.Show();
            frm.MdiParent = this;
            frm.BringToFront();
        }

        private void maquinasToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormMaquina frm = FormMaquina.ventana_unica();
            frm.Show();
            frm.MdiParent = this;
            frm.BringToFront();
        }

        private void mantenimientoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMantenimiento frm =  FormMantenimiento.ventana_unica();
            frm.Show();
            frm.MdiParent = this;
            frm.BringToFront();
        }

        private void areasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAreas frm = new FormAreas();
            frm.Show();
            frm.MdiParent = this;
            frm.BringToFront();
        }

        private void dietasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormDieta frm = FormDieta.ventana_unica();
            frm.Show();
            frm.MdiParent = this;
            frm.BringToFront();
        }

        private void alimentosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAlimento frm = FormAlimento.ventana_unica();
            frm.Show();
            frm.MdiParent = this;
            frm.BringToFront();
        }

        private void fichaMedicaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormFichaMedica frm = FormFichaMedica.ventana_unica();
            frm.Show();
            frm.MdiParent = this;
            frm.BringToFront();
        }

        private void FormMenuGym_Load(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void ingresoDePlanesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPlan frm = FormPlan.ventana_unica();
            frm.Show();
            frm.MdiParent = this;
            frm.BringToFront();
        }

        private void ingresoDePromocionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPromociones frm = FormPromociones.ventana_unica();
            frm.Show();
            frm.MdiParent = this;
            frm.BringToFront();
        }

        private void btn_regresar_Click(object sender, EventArgs e)
        {
            this.Close();

            
            Form1 frm = new Form1();
            frm.Show();
        }

        private void cLIENTEToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormInfoCliente frm = FormInfoCliente.ventana_unica();
            frm.Show();
            frm.MdiParent = this;
            frm.BringToFront();
        }

        private void eMPLEADOToolStripMenuItem_Click(object sender, EventArgs e)
        {
           FormInfoEmpleado frm = FormInfoEmpleado.ventana_unica();
            frm.Show();
            frm.MdiParent = this;
            frm.BringToFront();
        }
    }
}
