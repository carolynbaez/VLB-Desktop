using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using VlbBet.Core;
using VlbBet.Infrastructure; // si luego quieres usar Http/MQTT aquí

namespace VlbBet.App
{
    public partial class LoginForm : Form
    {
        // Aquí podrías inyectar clientes de API si quieres
        // private readonly AuthApiClient _authClient;
        //string url = "http://192.168.1.10:8000";

        //private readonly HttpClient _http;
        //private readonly JsonSerializerOptions _jsonOptions;

        private readonly AuthApiClient _authClient;


        public LoginForm()
        {
            InitializeComponent();

            ApplyStyle();

        
            // Para que Enter haga click en Login
            this.AcceptButton = btnLogin;

            _authClient = new AuthApiClient();
        }

        // Para liberar el HttpClient interno cuando cierres el form
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            _authClient?.Dispose();
        }

        private void ApplyStyle()
        {
            // Fondo del form ya está oscuro en el Designer

            // Card central con “sombra” ligera usando borde blanco
            panelCard.BorderStyle = BorderStyle.FixedSingle;

            // Header negro + amarillo ya configurado

            // Botones redondeados
            StylePrimaryButton(btnLogin);   // azul
            StyleSecondaryButton(btnRegister); // gris

            // Colores de texto
            lblPhone.ForeColor = Color.WhiteSmoke;
            lblPassword.ForeColor = Color.WhiteSmoke;
        }

        // ==== Estilo de botones (igual filosofía que BetForm) ====

        private void StylePrimaryButton(Button btn)
        {
            if (btn == null) return;

            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(33, 150, 243); // azul
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;

            btn.Resize += (s, e) => MakeRoundedButton(btn, 18);
            MakeRoundedButton(btn, 18);
        }

        private void StyleSecondaryButton(Button btn)
        {
            if (btn == null) return;

            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(96, 125, 139); // gris azulado
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;

            btn.Resize += (s, e) => MakeRoundedButton(btn, 14);
            MakeRoundedButton(btn, 14);
        }

        private void MakeRoundedButton(Button btn, int radius)
        {
            if (btn == null) return;
            if (btn.Width <= 0 || btn.Height <= 0) return;

            Rectangle rect = new Rectangle(0, 0, btn.Width, btn.Height);
            int diameter = radius * 2;

            using (GraphicsPath path = new GraphicsPath())
            {
                path.StartFigure();
                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();

                btn.Region = new Region(path);
            }
        }

        // ==== Bordes redondeados amarillos para los textboxes ====

        private void BorderPanel_Paint(object sender, PaintEventArgs e)
        {
            Panel pnl = sender as Panel;
            if (pnl == null) return;

            Rectangle rect = pnl.ClientRectangle;
            rect.Inflate(-1, -1); // margen interno

            int radius = 10;
            int diameter = radius * 2;

            using (GraphicsPath path = new GraphicsPath())
            {
                path.StartFigure();
                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen pen = new Pen(Color.Gold, 2))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }
        }

        // ==== Lógica básica de login (para conectar luego al API) ====


        private async void btnLogin_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;

            string phone = txtPhone.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(password))
            {
                ShowError("Debe digitar celular y password.");
                return;
            }

            try
            {
                var req = new AuthRequest()
                {
                    level = "4",
                    username = phone,
                    password = password

                };

                var res = await _authClient.LoginAsync(req);

                if (res == null)
                {
                    ShowError("No se pudo iniciar sesión (respuesta vacía).");
                    return;
                }

                if (string.IsNullOrWhiteSpace(res.user))
                {
                    ShowError("Credenciales inválidas.");
                    return;
                }

                SaveSession(res);

                this.Hide();

                using (var frm = new BetForm())
                {
                    frm.ShowDialog(this);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError("Error al iniciar sesión: " + ex.Message);
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Aquí puedes abrir un form de registro o ir a tu web
            MessageBox.Show("Aquí iría el registro de nuevo jugador.", "Registro",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowError(string msg)
        {
            lblError.Text = msg;
            lblError.Visible = true;
        }

        private void SaveSession(AuthResponse res)
        {
            //Guardando en memoria
            AppSession.Level = res.level;
            AppSession.Name = res.name;
            AppSession.Point = res.point;
            AppSession.User = res.user;
            AppSession.SessionId = res.sessionId;
            AppSession.UserId = res._id;

            //Guardando en disco
            Properties.Settings.Default.Level = AppSession.Level;
            Properties.Settings.Default.User = AppSession.User;
            Properties.Settings.Default.Point = AppSession.Point;
            Properties.Settings.Default.Name = AppSession.Name;
            Properties.Settings.Default.SessionId = AppSession.SessionId;
            Properties.Settings.Default.Save();
        }
    }
}
