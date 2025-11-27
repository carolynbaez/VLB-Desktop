using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using VlbBet.Core;
using VlbBet.Infrastructure;

namespace VlbBet.App
{
    public partial class BetForm : Form
    {
        private readonly BetService _betService = new BetService();

        private ApiSession _apiSession;
        private TicketApiClient _ticketClient;
        private MqttGameClient _mqttClient;
        private bool _mqttConnected = false;

        private readonly List<BettedItem> _betted = new List<BettedItem>();
        private List<Game> _program = new List<Game>();
        private bool _active = false;
        private int _evento = 0;

        private Timer _timer;
        private long _unixHourTarget = 0;

        public BetForm()
        {
            InitializeComponent();

            try
            {
                ApplyModernStyle();
                ConfigureBetsGrid();
                ConfigureTicketsGrid();

                var baseAddress = "https://vlb.virsbet.com/";

                _apiSession = new ApiSession(baseAddress);
                _ticketClient = new TicketApiClient(_apiSession);

                _mqttClient = new MqttGameClient("vlb.virsbet.com", 1883);
                _mqttClient.GamesLineReceived += OnGamesLineReceived;
                _mqttClient.HourReceived += OnHourReceived;
                _mqttClient.ConnectionChanged += OnMqttConnectionChanged;

                _timer = new Timer { Interval = 1000 };
                _timer.Tick += Timer_Tick;
                _timer.Start();

                pnlOverlay.Visible = false;

                UpdateWaitScreen();

                this.Load += async delegate { await BetForm_LoadAsync(); };
                this.Resize += delegate { ApplyRoundedButtons(); };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inicializando servicios:\n\n" + ex,
                    "Error en constructor BetForm",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ===== ESTILO VISUAL =====

        private void ApplyModernStyle()
        {
            BackColor = Color.FromArgb(10, 16, 30);

            panelNav.BackColor = Color.FromArgb(16, 32, 64);
            panelMain.BackColor = Color.FromArgb(18, 24, 44);

            StylePrimaryButton(btnNavCaja);
            StylePrimaryButton(btnNavPagar);
            StyleDangerButton(btnNavCerrar);

            StyleSecondaryButton(btnCancelBet);
            StyleWarningButton(btnGenerateTicket);   // ✅ botón amarillo
            StyleSecondaryButton(btnRefreshTickets);

            StyleSecondaryButton(btnPayCancel);
            StylePrimaryButton(btnPayVerify);

            StyleGrid(dgvBets);
            StyleGrid(dgvTickets);

            pnlPayModal.BackColor = Color.FromArgb(24, 34, 60);

            ApplyRoundedButtons();
        }

        private void ApplyRoundedButtons()
        {
            int navRadius = 18;
            int smallRadius = 14;

            MakeRoundedButton(btnNavCaja, navRadius);
            MakeRoundedButton(btnNavPagar, navRadius);
            MakeRoundedButton(btnNavCerrar, navRadius);

            MakeRoundedButton(btnCancelBet, smallRadius);
            MakeRoundedButton(btnGenerateTicket, smallRadius);
            MakeRoundedButton(btnRefreshTickets, smallRadius);

            MakeRoundedButton(btnPayCancel, smallRadius);
            MakeRoundedButton(btnPayVerify, smallRadius);
        }

        private void StylePrimaryButton(Button btn)
        {
            if (btn == null) return;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(33, 150, 243);
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
        }

        private void StyleSecondaryButton(Button btn)
        {
            if (btn == null) return;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(55, 71, 79);
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
        }

        private void StyleWarningButton(Button btn)
        {
            if (btn == null) return;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(255, 193, 7); // amarillo
            btn.ForeColor = Color.Black;
            btn.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
        }

        private void StyleDangerButton(Button btn)
        {
            if (btn == null) return;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(211, 47, 47);
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
        }

        private void StyleGrid(DataGridView grid)
        {
            if (grid == null) return;

            grid.BorderStyle = BorderStyle.None;
            grid.BackgroundColor = Color.FromArgb(20, 28, 48);
            grid.EnableHeadersVisualStyles = false;

            grid.ColumnHeadersVisible = true;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            grid.ColumnHeadersHeight = 30;

            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.RowTemplate.Height = 28;

            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 40, 70);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);

            grid.DefaultCellStyle.BackColor = Color.FromArgb(25, 35, 60);
            grid.DefaultCellStyle.ForeColor = Color.WhiteSmoke;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 121, 255);
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Regular);

            grid.RowHeadersVisible = false;
            grid.GridColor = Color.FromArgb(45, 55, 80);
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

        private void ConfigureBetsGrid()
        {
            if (dgvBets == null) return;

            dgvBets.AutoGenerateColumns = false;
            dgvBets.Columns.Clear();

            dgvBets.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Number", HeaderText = "#", FillWeight = 10 });
            dgvBets.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Option", HeaderText = "Código", FillWeight = 15 });
            dgvBets.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Betted", HeaderText = "Apuesta", FillWeight = 55 });
            dgvBets.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Rate", HeaderText = "Línea", FillWeight = 20 });
        }

        private void ConfigureTicketsGrid()
        {
            if (dgvTickets == null) return;
            dgvTickets.AutoGenerateColumns = true;
        }

        // ===== CARGA / MQTT / API =====

        private async Task BetForm_LoadAsync()
        {
            try
            {
                if (_mqttClient != null)
                    await _mqttClient.ConnectAsync();

                await LoadTicketsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar formulario:\n\n" + ex,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async Task LoadTicketsAsync()
        {
            if (_ticketClient == null) return;

            try
            {
                var list = await _ticketClient.GetGamesAsync();
                dgvTickets.DataSource = null;
                dgvTickets.DataSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar tickets:\n\n" + ex.Message,
                    "Error HTTP",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void OnGamesLineReceived(object sender, GamesLineMessage e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnGamesLineReceived(sender, e)));
                return;
            }

            _active = e.Active;
            _evento = e.Num;
            _program = e.Games ?? new List<Game>();

            lblEvento.Text = $"VLB-{_evento}";

            UpdateWaitScreen();
        }

        private void OnHourReceived(object sender, HourMessage e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnHourReceived(sender, e)));
                return;
            }
            _unixHourTarget = e.Hour;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_unixHourTarget <= 0)
            {
                lblTimer.Text = "";
                return;
            }

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long diff = _unixHourTarget - now;
            if (diff < 0) diff = 0;

            var ts = TimeSpan.FromSeconds(diff);
            lblTimer.Text = $"{ts.Minutes:D2}:{ts.Seconds:D2}";
        }

        // ===== ENTRADAS DE APUESTA =====

        private void txtBetInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            string txt = (txtBetInput.Text ?? "").Trim();

            // Enter con vacío => etapa calcular / pasar a monto
            if (string.IsNullOrEmpty(txt))
            {
                if (_betted.Count < 3 || _betted.Count > 10) // ✅ backend 3..10
                {
                    MessageBox.Show("La jugada debe estar entre 3 y 10 apuestas.");
                    txtBetInput.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtAmount.Text))
                    txtAmount.Text = "50";

                if (decimal.TryParse(txtAmount.Text, out var monto))
                {
                    var amountWin = _betService.CalculateAmountWin(_betted, monto);
                    lblAmountWin.Text = ((int)amountWin).ToString();
                    txtAmount.Focus();
                    txtAmount.SelectAll();
                }
                else
                {
                    MessageBox.Show("Monto no válido.", "Error");
                    txtAmount.Focus();
                    txtAmount.SelectAll();
                }

                return;
            }

            // Agregar jugada
            if (!_mqttConnected || !_active || _program == null || _program.Count == 0)
            {
                MessageBox.Show("No hay línea activa para vender ahora.", "Info");
                return;
            }

            string error;
            bool ok = _betService.AddBetFromInput(txt, _program, _betted, out error);

            if (!ok)
            {
                MessageBox.Show(error, "Error de jugada");
                return;
            }

            txtBetInput.Clear();
            txtBetInput.Focus();

            dgvBets.DataSource = null;
            dgvBets.DataSource = _betted;
        }

        private void txtAmount_TextChanged(object sender, EventArgs e)
        {
            if (_betted.Count == 0)
            {
                lblAmountWin.Text = "0";
                return;
            }

            if (decimal.TryParse(txtAmount.Text, out var monto))
            {
                var amountWin = _betService.CalculateAmountWin(_betted, monto);
                lblAmountWin.Text = ((int)amountWin).ToString();
            }
            else
            {
                lblAmountWin.Text = "0";
            }
        }

        private async void txtAmount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await CreateTicketFromUiAsync();
        }

        private async void btnGenerateTicket_Click(object sender, EventArgs e)
        {
            await CreateTicketFromUiAsync();
        }

        private async Task CreateTicketFromUiAsync()
        {
            if (_ticketClient == null)
            {
                MessageBox.Show("Cliente HTTP no inicializado.");
                return;
            }

            // ✅ Reglas de venta válidas
            if (!_mqttConnected || !_active || _program == null || _program.Count == 0)
            {
                MessageBox.Show("No hay línea activa para vender ahora.", "Info");
                return;
            }

            if (_betted.Count < 3 || _betted.Count > 10)
            {
                MessageBox.Show("La jugada debe estar entre 3 y 10 apuestas.");
                txtBetInput.Focus();
                return;
            }

            if (!decimal.TryParse(txtAmount.Text, out var amount) || amount <= 0)
            {
                MessageBox.Show("Monto no válido.");
                txtAmount.Focus();
                txtAmount.SelectAll();
                return;
            }

            var amountWin = _betService.CalculateAmountWin(_betted, amount);
            lblAmountWin.Text = ((int)amountWin).ToString();

            if (amountWin > 100000)
            {
                MessageBox.Show("Monto a ganar superado.");
                return;
            }

            try
            {
                // ✅ Llama al endpoint /ticket/add (Node: Add)
                var resp = await _ticketClient.AddAsync(_betted, amount, amountWin);

                // resp puede venir "ok" o mensaje plano
                if (resp != null && resp.Msg == "ok" && resp.Ticket != null)
                {
                    MessageBox.Show("Ticket creado: " + resp.Ticket.Num);

                    _betted.Clear();
                    dgvBets.DataSource = null;
                    txtBetInput.Clear();
                    txtAmount.Clear();
                    lblAmountWin.Text = "0";
                    txtBetInput.Focus();

                    await LoadTicketsAsync();
                }
                else if (resp != null && !string.IsNullOrWhiteSpace(resp.Msg))
                {
                    MessageBox.Show(resp.Msg, "Servidor");
                }
                else
                {
                    MessageBox.Show("Respuesta inválida del servidor.", "Error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar ticket: " + ex.Message);
            }
        }

        private void btnCancelBet_Click(object sender, EventArgs e)
        {
            _betService.CancelCurrentBet(_betted);
            dgvBets.DataSource = null;
            txtBetInput.Clear();
            txtAmount.Clear();
            lblAmountWin.Text = "0";
            txtBetInput.Focus();
        }

        private async void btnRefreshTickets_Click(object sender, EventArgs e)
        {
            await LoadTicketsAsync();
        }

        // ===== NAV =====

        private void btnNavCaja_Click(object sender, EventArgs e)
        {
            using (var form = new CajaForm(_ticketClient))
                form.ShowDialog(this);
        }

        private void btnNavCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnNavPagar_Click(object sender, EventArgs e)
        {
            ShowPayModal();
        }

        // ===== MODAL PAGO =====

        private void ShowPayModal()
        {
            txtPayTicketNumber.Text = "";
            pnlOverlay.BringToFront();
            pnlPayModal.BringToFront();

            pnlPayModal.Left = (ClientSize.Width - pnlPayModal.Width) / 2;
            pnlPayModal.Top = (ClientSize.Height - pnlPayModal.Height) / 2;

            pnlOverlay.Visible = true;
            txtPayTicketNumber.Focus();
        }

        private void HidePayModal() => pnlOverlay.Visible = false;

        private async Task PayTicketAsync(string ticketNumber)
        {
            if (_ticketClient == null)
            {
                MessageBox.Show("Cliente HTTP no inicializado.");
                return;
            }

            string tk = (ticketNumber ?? "").Trim();
            if (string.IsNullOrEmpty(tk))
            {
                MessageBox.Show("Digite el número de ticket.");
                return;
            }

            try
            {
                // 1) primera llamada => winner (verify) o msg
                var r1 = await _ticketClient.PayTicketAsync(tk, "pay");

                if (r1 != null && r1.Msg == "verify" && r1.Winner != null)
                {
                    var w = r1.Winner;
                    var txt = $"GANADOR ✅\n\n" +
                              $"Monto a pagar: {w.AmountToPay:n2}\n" +
                              $"¿CONFIRMAR PAGO?";

                    var ok = MessageBox.Show(txt, "Verificación de pago", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (ok == DialogResult.Yes)
                    {
                        // 2) segunda llamada => paga
                        var r2 = await _ticketClient.PayTicketAsync(tk, "pay");
                        MessageBox.Show(r2?.Msg ?? "Pagado", "Pago");
                        HidePayModal();
                        await LoadTicketsAsync();
                    }
                    return;
                }

                MessageBox.Show(r1?.Msg ?? "Respuesta vacía", "Pago");
                HidePayModal();
                await LoadTicketsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al pagar ticket: " + ex.Message);
            }
        }

        private void btnPayCancel_Click(object sender, EventArgs e) => HidePayModal();

        private async void btnPayVerify_Click(object sender, EventArgs e)
        {
            await PayTicketAsync(txtPayTicketNumber.Text);
        }

        private async void txtPayTicketNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                await PayTicketAsync(txtPayTicketNumber.Text);
        }

        // ===== PANTALLA WAIT =====

        private void OnMqttConnectionChanged(object sender, bool connected)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnMqttConnectionChanged(sender, connected)));
                return;
            }

            _mqttConnected = connected;
            UpdateWaitScreen();
        }

        private void UpdateWaitScreen()
        {
            bool hasProgram = (_program != null && _program.Count > 0);

            bool showWait = (!_mqttConnected) || (!hasProgram) || (!_active);

            if (lblWaitMessage != null)
            {
                if (!_mqttConnected)
                    lblWaitMessage.Text = "CONECTANDO AL BROKER...\r\n\r\n(REVISAR RED / PUERTO 1883)";
                else if (!hasProgram)
                    lblWaitMessage.Text = "LEYENDO LÍNEA...\r\n\r\n(ESPERANDO DATOS DE MQTT)";
                else if (!_active)
                    lblWaitMessage.Text = "ESPERANDO PRÓXIMA LÍNEA...\r\n\r\n(NO SE PERMITEN VENTAS)";
                else
                    lblWaitMessage.Text = "";
            }

            if (pnlWait != null)
            {
                pnlWait.Visible = showWait;
                if (showWait) pnlWait.BringToFront();
            }

            bool enableSales = _mqttConnected && hasProgram && _active;

            if (txtBetInput != null) txtBetInput.Enabled = enableSales;
            if (txtAmount != null) txtAmount.Enabled = enableSales;
            if (btnCancelBet != null) btnCancelBet.Enabled = enableSales;
            if (btnGenerateTicket != null) btnGenerateTicket.Enabled = enableSales;
            if (dgvBets != null) dgvBets.Enabled = enableSales;

            if (btnRefreshTickets != null) btnRefreshTickets.Enabled = true;
            if (btnNavPagar != null) btnNavPagar.Enabled = true;
            if (btnNavCaja != null) btnNavCaja.Enabled = true;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            try { _mqttClient?.Dispose(); } catch { }
            try { _apiSession?.Dispose(); } catch { }
        }
    }
}
