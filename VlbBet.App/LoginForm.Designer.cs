using System.Drawing;
using System.Windows.Forms;

namespace VlbBet.App
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelRoot;
        private Panel panelCard;
        private Panel panelHeader;
        private Label lblTitle;

        private Label lblRole;
        private ComboBox cmbRole;

        private Label lblPhone;
        private Panel pnlPhone;
        private TextBox txtPhone;

        private Label lblPassword;
        private Panel pnlPassword;
        private TextBox txtPassword;

        private Button btnLogin;
        private Button btnRegister;

        private Label lblError;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panelRoot = new Panel();
            panelCard = new Panel();
            lblError = new Label();
            btnLogin = new Button();
            pnlPassword = new Panel();
            txtPassword = new TextBox();
            lblPassword = new Label();
            pnlPhone = new Panel();
            txtPhone = new TextBox();
            lblPhone = new Label();
            panelHeader = new Panel();
            lblTitle = new Label();
            panelRoot.SuspendLayout();
            panelCard.SuspendLayout();
            pnlPassword.SuspendLayout();
            pnlPhone.SuspendLayout();
            panelHeader.SuspendLayout();
            SuspendLayout();
            // 
            // panelRoot
            // 
            panelRoot.BackColor = Color.Transparent;
            panelRoot.Controls.Add(panelCard);
            panelRoot.Dock = DockStyle.Fill;
            panelRoot.Location = new Point(0, 0);
            panelRoot.Margin = new Padding(4, 5, 4, 5);
            panelRoot.Name = "panelRoot";
            panelRoot.Size = new Size(1286, 867);
            panelRoot.TabIndex = 0;
            // 
            // panelCard
            // 
            panelCard.Anchor = AnchorStyles.None;
            panelCard.BackColor = Color.FromArgb(18, 24, 44);
            panelCard.Controls.Add(lblError);
            panelCard.Controls.Add(btnLogin);
            panelCard.Controls.Add(pnlPassword);
            panelCard.Controls.Add(lblPassword);
            panelCard.Controls.Add(pnlPhone);
            panelCard.Controls.Add(lblPhone);
            panelCard.Controls.Add(panelHeader);
            panelCard.Location = new Point(377, 105);
            panelCard.Margin = new Padding(4, 5, 4, 5);
            panelCard.Name = "panelCard";
            panelCard.Size = new Size(614, 600);
            panelCard.TabIndex = 0;
            // 
            // lblError
            // 
            lblError.AutoSize = true;
            lblError.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold, GraphicsUnit.Point);
            lblError.ForeColor = Color.LightCoral;
            lblError.Location = new Point(66, 350);
            lblError.Margin = new Padding(4, 0, 4, 0);
            lblError.Name = "lblError";
            lblError.Size = new Size(0, 23);
            lblError.TabIndex = 0;
            lblError.Visible = false;
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(66, 414);
            btnLogin.Margin = new Padding(4, 5, 4, 5);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(486, 60);
            btnLogin.TabIndex = 1;
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // pnlPassword
            // 
            pnlPassword.BackColor = Color.FromArgb(240, 240, 240);
            pnlPassword.Controls.Add(txtPassword);
            pnlPassword.Location = new Point(66, 311);
            pnlPassword.Margin = new Padding(4, 5, 4, 5);
            pnlPassword.Name = "pnlPassword";
            pnlPassword.Padding = new Padding(11, 8, 11, 8);
            pnlPassword.Size = new Size(486, 50);
            pnlPassword.TabIndex = 3;
            pnlPassword.Paint += BorderPanel_Paint;
            // 
            // txtPassword
            // 
            txtPassword.BorderStyle = BorderStyle.None;
            txtPassword.Dock = DockStyle.Fill;
            txtPassword.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            txtPassword.Location = new Point(11, 8);
            txtPassword.Margin = new Padding(4, 5, 4, 5);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(464, 24);
            txtPassword.TabIndex = 0;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblPassword.ForeColor = Color.WhiteSmoke;
            lblPassword.Location = new Point(66, 277);
            lblPassword.Margin = new Padding(4, 0, 4, 0);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(87, 25);
            lblPassword.TabIndex = 4;
            lblPassword.Text = "Password";
            // 
            // pnlPhone
            // 
            pnlPhone.BackColor = Color.FromArgb(240, 240, 240);
            pnlPhone.Controls.Add(txtPhone);
            pnlPhone.Location = new Point(66, 203);
            pnlPhone.Margin = new Padding(4, 5, 4, 5);
            pnlPhone.Name = "pnlPhone";
            pnlPhone.Padding = new Padding(11, 8, 11, 8);
            pnlPhone.Size = new Size(486, 50);
            pnlPhone.TabIndex = 5;
            pnlPhone.Paint += BorderPanel_Paint;
            // 
            // txtPhone
            // 
            txtPhone.BorderStyle = BorderStyle.None;
            txtPhone.Dock = DockStyle.Fill;
            txtPhone.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            txtPhone.Location = new Point(11, 8);
            txtPhone.Margin = new Padding(4, 5, 4, 5);
            txtPhone.Name = "txtPhone";
            txtPhone.Size = new Size(464, 24);
            txtPhone.TabIndex = 0;
            // 
            // lblPhone
            // 
            lblPhone.AutoSize = true;
            lblPhone.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblPhone.ForeColor = Color.WhiteSmoke;
            lblPhone.Location = new Point(66, 169);
            lblPhone.Margin = new Padding(4, 0, 4, 0);
            lblPhone.Name = "lblPhone";
            lblPhone.Size = new Size(65, 25);
            lblPhone.TabIndex = 6;
            lblPhone.Text = "Celular";
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.Black;
            panelHeader.Controls.Add(lblTitle);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Margin = new Padding(4, 5, 4, 5);
            panelHeader.Name = "panelHeader";
            panelHeader.Padding = new Padding(6, 7, 6, 7);
            panelHeader.Size = new Size(614, 92);
            panelHeader.TabIndex = 9;
            // 
            // lblTitle
            // 
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.ForeColor = Color.Yellow;
            lblTitle.Location = new Point(6, 7);
            lblTitle.Margin = new Padding(4, 0, 4, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(602, 78);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Login";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(10, 16, 30);
            ClientSize = new Size(1286, 867);
            Controls.Add(panelRoot);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login - VLB";
            panelRoot.ResumeLayout(false);
            panelCard.ResumeLayout(false);
            panelCard.PerformLayout();
            pnlPassword.ResumeLayout(false);
            pnlPassword.PerformLayout();
            pnlPhone.ResumeLayout(false);
            pnlPhone.PerformLayout();
            panelHeader.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
