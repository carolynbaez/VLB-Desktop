using System.Drawing;
using System.Windows.Forms;

namespace VlbBet.App
{
    partial class BetForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelNav;
        private Button btnNavCaja;
        private Button btnNavPagar;
        private Button btnNavCerrar;

        private Panel panelMain;
        private TextBox txtBetInput;
        private TextBox txtAmount;
        private Label lblAmountWin;
        private DataGridView dgvBets;
        private DataGridView dgvTickets;
        private Label lblEvento;
        private Label lblTimer;
        private Button btnCancelBet;
        private Button btnGenerateTicket; // ✅ NUEVO
        private Button btnRefreshTickets;
        private Label lblMontoAGanarLabel;
        private Label lblMontoLabel;

        private Panel pnlOverlay;
        private Panel pnlPayModal;
        private Label lblPayTitle;
        private Label lblPayPrompt;
        private TextBox txtPayTicketNumber;
        private Button btnPayCancel;
        private Button btnPayVerify;

        private Panel pnlWait;
        private Label lblWaitMessage;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panelNav = new Panel();
            btnNavCaja = new Button();
            btnNavPagar = new Button();
            btnNavCerrar = new Button();

            panelMain = new Panel();
            lblEvento = new Label();
            lblTimer = new Label();
            txtBetInput = new TextBox();
            txtAmount = new TextBox();
            lblMontoLabel = new Label();
            lblMontoAGanarLabel = new Label();
            lblAmountWin = new Label();
            btnCancelBet = new Button();
            btnGenerateTicket = new Button(); // ✅ NUEVO
            dgvBets = new DataGridView();
            dgvTickets = new DataGridView();
            btnRefreshTickets = new Button();

            pnlOverlay = new Panel();
            pnlPayModal = new Panel();
            lblPayTitle = new Label();
            lblPayPrompt = new Label();
            txtPayTicketNumber = new TextBox();
            btnPayCancel = new Button();
            btnPayVerify = new Button();

            pnlWait = new Panel();
            lblWaitMessage = new Label();

            panelNav.SuspendLayout();
            panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvBets).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvTickets).BeginInit();
            pnlOverlay.SuspendLayout();
            pnlPayModal.SuspendLayout();
            pnlWait.SuspendLayout();
            SuspendLayout();

            // panelNav
            panelNav.BackColor = Color.FromArgb(16, 32, 64);
            panelNav.Controls.Add(btnNavCaja);
            panelNav.Controls.Add(btnNavPagar);
            panelNav.Controls.Add(btnNavCerrar);
            panelNav.Dock = DockStyle.Top;
            panelNav.Location = new Point(0, 0);
            panelNav.Margin = new Padding(4, 5, 4, 5);
            panelNav.Name = "panelNav";
            panelNav.Padding = new Padding(17);
            panelNav.Size = new Size(1371, 117);
            panelNav.TabIndex = 0;

            // btnNavCaja
            btnNavCaja.Location = new Point(17, 17);
            btnNavCaja.Margin = new Padding(4, 5, 4, 5);
            btnNavCaja.Name = "btnNavCaja";
            btnNavCaja.Size = new Size(186, 83);
            btnNavCaja.TabIndex = 0;
            btnNavCaja.Text = "Cuadre Caja";
            btnNavCaja.UseVisualStyleBackColor = true;
            btnNavCaja.Click += btnNavCaja_Click;

            // btnNavPagar
            btnNavPagar.Location = new Point(234, 17);
            btnNavPagar.Margin = new Padding(4, 5, 4, 5);
            btnNavPagar.Name = "btnNavPagar";
            btnNavPagar.Size = new Size(186, 83);
            btnNavPagar.TabIndex = 2;
            btnNavPagar.Text = "Pagar";
            btnNavPagar.UseVisualStyleBackColor = true;
            btnNavPagar.Click += btnNavPagar_Click;

            // btnNavCerrar
            btnNavCerrar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnNavCerrar.Location = new Point(1169, 17);
            btnNavCerrar.Margin = new Padding(4, 5, 4, 5);
            btnNavCerrar.Name = "btnNavCerrar";
            btnNavCerrar.Size = new Size(186, 83);
            btnNavCerrar.TabIndex = 5;
            btnNavCerrar.Text = "Cerrar";
            btnNavCerrar.UseVisualStyleBackColor = true;
            btnNavCerrar.Click += btnNavCerrar_Click;

            // panelMain
            panelMain.BackColor = Color.FromArgb(16, 22, 40);
            panelMain.Controls.Add(lblEvento);
            panelMain.Controls.Add(lblTimer);
            panelMain.Controls.Add(txtBetInput);
            panelMain.Controls.Add(txtAmount);
            panelMain.Controls.Add(lblMontoLabel);
            panelMain.Controls.Add(lblMontoAGanarLabel);
            panelMain.Controls.Add(lblAmountWin);
            panelMain.Controls.Add(btnCancelBet);
            panelMain.Controls.Add(btnGenerateTicket); // ✅ NUEVO
            panelMain.Controls.Add(dgvBets);
            panelMain.Controls.Add(dgvTickets);
            panelMain.Controls.Add(btnRefreshTickets);
            panelMain.Dock = DockStyle.Fill;
            panelMain.Location = new Point(0, 117);
            panelMain.Margin = new Padding(4, 5, 4, 5);
            panelMain.Name = "panelMain";
            panelMain.Padding = new Padding(23, 27, 23, 27);
            panelMain.Size = new Size(1371, 783);
            panelMain.TabIndex = 1;

            // lblEvento
            lblEvento.AutoSize = true;
            lblEvento.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblEvento.ForeColor = Color.White;
            lblEvento.Location = new Point(23, 17);
            lblEvento.Margin = new Padding(4, 0, 4, 0);
            lblEvento.Name = "lblEvento";
            lblEvento.Size = new Size(95, 32);
            lblEvento.TabIndex = 6;
            lblEvento.Text = "VLB-00";

            // lblTimer
            lblTimer.AutoSize = true;
            lblTimer.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblTimer.ForeColor = Color.FromArgb(0, 230, 118);
            lblTimer.Location = new Point(143, 17);
            lblTimer.Margin = new Padding(4, 0, 4, 0);
            lblTimer.Name = "lblTimer";
            lblTimer.Size = new Size(77, 32);
            lblTimer.TabIndex = 7;
            lblTimer.Text = "00:00";

            // txtBetInput
            txtBetInput.Location = new Point(23, 83);
            txtBetInput.Margin = new Padding(4, 5, 4, 5);
            txtBetInput.Name = "txtBetInput";
            txtBetInput.Size = new Size(313, 31);
            txtBetInput.TabIndex = 1;
            txtBetInput.KeyDown += txtBetInput_KeyDown;

            // txtAmount
            txtAmount.Location = new Point(360, 83);
            txtAmount.Margin = new Padding(4, 5, 4, 5);
            txtAmount.Name = "txtAmount";
            txtAmount.Size = new Size(170, 31);
            txtAmount.TabIndex = 2;
            txtAmount.TextChanged += txtAmount_TextChanged;
            txtAmount.KeyDown += txtAmount_KeyDown;

            // lblMontoLabel
            lblMontoLabel.AutoSize = true;
            lblMontoLabel.ForeColor = Color.WhiteSmoke;
            lblMontoLabel.Location = new Point(360, 53);
            lblMontoLabel.Margin = new Padding(4, 0, 4, 0);
            lblMontoLabel.Name = "lblMontoLabel";
            lblMontoLabel.Size = new Size(147, 25);
            lblMontoLabel.TabIndex = 11;
            lblMontoLabel.Text = "Monto apostado";

            // lblMontoAGanarLabel
            lblMontoAGanarLabel.AutoSize = true;
            lblMontoAGanarLabel.ForeColor = Color.WhiteSmoke;
            lblMontoAGanarLabel.Location = new Point(557, 53);
            lblMontoAGanarLabel.Margin = new Padding(4, 0, 4, 0);
            lblMontoAGanarLabel.Name = "lblMontoAGanarLabel";
            lblMontoAGanarLabel.Size = new Size(134, 25);
            lblMontoAGanarLabel.TabIndex = 10;
            lblMontoAGanarLabel.Text = "Monto a ganar:";

            // lblAmountWin
            lblAmountWin.AutoSize = true;
            lblAmountWin.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold, GraphicsUnit.Point);
            lblAmountWin.ForeColor = Color.FromArgb(0, 230, 118);
            lblAmountWin.Location = new Point(557, 83);
            lblAmountWin.Margin = new Padding(4, 0, 4, 0);
            lblAmountWin.Name = "lblAmountWin";
            lblAmountWin.Size = new Size(25, 30);
            lblAmountWin.TabIndex = 3;
            lblAmountWin.Text = "0";

            // btnCancelBet
            btnCancelBet.Location = new Point(743, 78);
            btnCancelBet.Margin = new Padding(4, 5, 4, 5);
            btnCancelBet.Name = "btnCancelBet";
            btnCancelBet.Size = new Size(157, 47);
            btnCancelBet.TabIndex = 8;
            btnCancelBet.Text = "Cancelar";
            btnCancelBet.UseVisualStyleBackColor = true;
            btnCancelBet.Click += btnCancelBet_Click;

            // btnGenerateTicket ✅ NUEVO (amarillo)
            btnGenerateTicket.Location = new Point(912, 78);
            btnGenerateTicket.Margin = new Padding(4, 5, 4, 5);
            btnGenerateTicket.Name = "btnGenerateTicket";
            btnGenerateTicket.Size = new Size(190, 47);
            btnGenerateTicket.TabIndex = 12;
            btnGenerateTicket.Text = "Generar Ticket";
            btnGenerateTicket.UseVisualStyleBackColor = true;
            btnGenerateTicket.Click += btnGenerateTicket_Click;

            // dgvBets
            dgvBets.AllowUserToAddRows = false;
            dgvBets.AllowUserToDeleteRows = false;
            dgvBets.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvBets.BackgroundColor = Color.FromArgb(20, 28, 48);
            dgvBets.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvBets.Location = new Point(20, 150);
            dgvBets.Margin = new Padding(4, 5, 4, 5);
            dgvBets.Name = "dgvBets";
            dgvBets.RowHeadersWidth = 62;
            dgvBets.RowTemplate.Height = 25;
            dgvBets.Size = new Size(1324, 250);
            dgvBets.TabIndex = 4;

            // dgvTickets
            dgvTickets.AllowUserToAddRows = false;
            dgvTickets.AllowUserToDeleteRows = false;
            dgvTickets.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvTickets.BackgroundColor = Color.FromArgb(20, 28, 48);
            dgvTickets.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTickets.Location = new Point(20, 433);
            dgvTickets.Margin = new Padding(4, 5, 4, 5);
            dgvTickets.Name = "dgvTickets";
            dgvTickets.RowHeadersWidth = 62;
            dgvTickets.RowTemplate.Height = 25;
            dgvTickets.Size = new Size(1324, 250);
            dgvTickets.TabIndex = 5;

            // btnRefreshTickets
            btnRefreshTickets.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnRefreshTickets.Location = new Point(23, 700);
            btnRefreshTickets.Margin = new Padding(4, 5, 4, 5);
            btnRefreshTickets.Name = "btnRefreshTickets";
            btnRefreshTickets.Size = new Size(214, 50);
            btnRefreshTickets.TabIndex = 9;
            btnRefreshTickets.Text = "Refrescar tickets";
            btnRefreshTickets.UseVisualStyleBackColor = true;
            btnRefreshTickets.Click += btnRefreshTickets_Click;

            // pnlOverlay
            pnlOverlay.BackColor = Color.FromArgb(160, 0, 0, 0);
            pnlOverlay.Controls.Add(pnlPayModal);
            pnlOverlay.Dock = DockStyle.Fill;
            pnlOverlay.Location = new Point(0, 0);
            pnlOverlay.Margin = new Padding(4, 5, 4, 5);
            pnlOverlay.Name = "pnlOverlay";
            pnlOverlay.Size = new Size(1371, 900);
            pnlOverlay.TabIndex = 12;
            pnlOverlay.Visible = false;

            // pnlPayModal
            pnlPayModal.BackColor = Color.FromArgb(24, 34, 60);
            pnlPayModal.Controls.Add(lblPayTitle);
            pnlPayModal.Controls.Add(lblPayPrompt);
            pnlPayModal.Controls.Add(txtPayTicketNumber);
            pnlPayModal.Controls.Add(btnPayCancel);
            pnlPayModal.Controls.Add(btnPayVerify);
            pnlPayModal.Location = new Point(400, 250);
            pnlPayModal.Margin = new Padding(4, 5, 4, 5);
            pnlPayModal.Name = "pnlPayModal";
            pnlPayModal.Padding = new Padding(23, 27, 23, 27);
            pnlPayModal.Size = new Size(571, 333);
            pnlPayModal.TabIndex = 0;

            // lblPayTitle
            lblPayTitle.Dock = DockStyle.Top;
            lblPayTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
            lblPayTitle.ForeColor = Color.White;
            lblPayTitle.Location = new Point(23, 27);
            lblPayTitle.Margin = new Padding(4, 0, 4, 0);
            lblPayTitle.Name = "lblPayTitle";
            lblPayTitle.Padding = new Padding(0, 0, 0, 13);
            lblPayTitle.Size = new Size(525, 50);
            lblPayTitle.TabIndex = 0;
            lblPayTitle.Text = "Verificación de ticket";
            lblPayTitle.TextAlign = ContentAlignment.MiddleLeft;

            // lblPayPrompt
            lblPayPrompt.AutoSize = true;
            lblPayPrompt.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            lblPayPrompt.ForeColor = Color.WhiteSmoke;
            lblPayPrompt.Location = new Point(29, 100);
            lblPayPrompt.Margin = new Padding(4, 0, 4, 0);
            lblPayPrompt.Name = "lblPayPrompt";
            lblPayPrompt.Size = new Size(153, 28);
            lblPayPrompt.TabIndex = 1;
            lblPayPrompt.Text = "Digite el ticket";

            // txtPayTicketNumber
            txtPayTicketNumber.Location = new Point(29, 142);
            txtPayTicketNumber.Margin = new Padding(4, 5, 4, 5);
            txtPayTicketNumber.Name = "txtPayTicketNumber";
            txtPayTicketNumber.Size = new Size(513, 31);
            txtPayTicketNumber.TabIndex = 2;
            txtPayTicketNumber.KeyDown += txtPayTicketNumber_KeyDown;

            // btnPayCancel
            btnPayCancel.Location = new Point(29, 225);
            btnPayCancel.Margin = new Padding(4, 5, 4, 5);
            btnPayCancel.Name = "btnPayCancel";
            btnPayCancel.Size = new Size(229, 53);
            btnPayCancel.TabIndex = 3;
            btnPayCancel.Text = "Cancelar";
            btnPayCancel.UseVisualStyleBackColor = true;
            btnPayCancel.Click += btnPayCancel_Click;

            // btnPayVerify
            btnPayVerify.Location = new Point(314, 225);
            btnPayVerify.Margin = new Padding(4, 5, 4, 5);
            btnPayVerify.Name = "btnPayVerify";
            btnPayVerify.Size = new Size(229, 53);
            btnPayVerify.TabIndex = 4;
            btnPayVerify.Text = "Verificar";
            btnPayVerify.UseVisualStyleBackColor = true;
            btnPayVerify.Click += btnPayVerify_Click;

            // pnlWait
            pnlWait.BackColor = Color.FromArgb(80, 10, 16, 30);
            pnlWait.Dock = DockStyle.Fill;
            pnlWait.Location = new Point(23, 27);
            pnlWait.Name = "pnlWait";
            pnlWait.Size = new Size(1325, 729);
            pnlWait.TabIndex = 999;
            pnlWait.Visible = false;

            // lblWaitMessage
            lblWaitMessage.AutoSize = false;
            lblWaitMessage.Dock = DockStyle.Fill;
            lblWaitMessage.BackColor = Color.Transparent;
            lblWaitMessage.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblWaitMessage.ForeColor = Color.White;
            lblWaitMessage.Location = new Point(0, 0);
            lblWaitMessage.Name = "lblWaitMessage";
            lblWaitMessage.Size = new Size(1325, 729);
            lblWaitMessage.TabIndex = 0;
            lblWaitMessage.Text = "LEYENDO LÍNEA...\r\n\r\n(ESPERANDO DATOS DE MQTT)";
            lblWaitMessage.TextAlign = ContentAlignment.MiddleCenter;
            pnlWait.Controls.Add(lblWaitMessage);

            // BetForm
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(10, 16, 30);
            ClientSize = new Size(1371, 900);
            Controls.Add(panelMain);
            Controls.Add(panelNav);
            Controls.Add(pnlOverlay);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "BetForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "VLB Bet - POS";

            panelNav.ResumeLayout(false);
            panelMain.ResumeLayout(false);
            panelMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvBets).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvTickets).EndInit();
            pnlOverlay.ResumeLayout(false);
            pnlPayModal.ResumeLayout(false);
            pnlPayModal.PerformLayout();
            pnlWait.ResumeLayout(false);
            ResumeLayout(false);

            // MUY IMPORTANTE: agregar pnlWait DENTRO de panelMain
            panelMain.Controls.Add(pnlWait);
            pnlWait.BringToFront();
        }
    }
}
