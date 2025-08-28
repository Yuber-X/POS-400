using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace MiPOSCSharpMySQL.Controlador
{

    public class AdminCredentialDialog : Form
    {
        private TextBox txtUsuario;
        private TextBox txtPassword;
        private Button btnOK;
        private Button btnCancel;

        public string Usuario => txtUsuario.Text.Trim();
        public string Password => txtPassword.Text;

        public AdminCredentialDialog()
        {
            this.Text = "Validación Administrador";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 360;
            this.Height = 200;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var lblUsuario = new Label() { Left = 20, Top = 20, Width = 120, Text = "Usuario admin:" };
            txtUsuario = new TextBox() { Left = 150, Top = 18, Width = 170 };

            var lblPassword = new Label() { Left = 20, Top = 60, Width = 120, Text = "Contraseña:" };
            txtPassword = new TextBox() { Left = 150, Top = 58, Width = 170, UseSystemPasswordChar = true };

            btnOK = new Button() { Text = "OK", Left = 150, Width = 80, Top = 100, DialogResult = DialogResult.OK };
            btnCancel = new Button() { Text = "Cancelar", Left = 240, Width = 80, Top = 100, DialogResult = DialogResult.Cancel };

            btnOK.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(Usuario) || string.IsNullOrWhiteSpace(Password))
                {
                    MessageBox.Show("Complete usuario y contraseña.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.None; // no cerrar
                }
            };

            this.Controls.Add(lblUsuario);
            this.Controls.Add(txtUsuario);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }
    }

}
