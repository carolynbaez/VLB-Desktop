using System.Drawing;
using System.Windows.Forms;

namespace VlbBet.App
{
    partial class CajaForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelRoot;
        private Panel panelCard;
        private Panel panelHeader;
        private Label lblTitle;

        private DateTimePicker dtpDesde;
        private DateTimePicker dtpHasta;
        private Button btnBuscar;
        private Button btnImprimir;

        private Label lblMontoApostado;
        private Label lblMontoApostadoVal;
        private Label lblRecargas;
        private Label lblRecargasVal;

        private Label lblMontoGanado;
        private Label lblMontoGanadoVal;
        private Label lblRetiros;
        private Label lblRetirosVal;

        private Label lblMontoPagado;
        private Label lblMontoPagadoVal;
        private Label lblBalance;
        private Label lblBalanceVal;

        private Label lblTotalCaja;
        private Label lblTotalCajaVal;

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
            components = new System.ComponentModel.Container();
            panelRoot = new Panel();
            panelCard = new Panel();
            panelHeader = new Panel();
            lblTitle = new Label();
            dtpDesde = new DateTimePicker();
            dtpHasta = new DateTimePicker();
            btnBuscar = new Button();
            btnImprimir = new Button();
            lblMontoApostado = new Label();
            lblMontoApostadoVal = new Label();
            lblRecargas = new Label();
            lblRecargasVal = new Label();
            lblMontoGanado = new Label();
            lblMontoGanadoVal = new Label();
            lblRetiros = new Label();
            lblRetirosVal = new Label();
            lblMontoPagado = new Label();
            lblMontoPagadoVal = new Label();
            lblBalance = new Label();
            lblBalanceVal = new Label();
            lblTotalCaja = new Label();
            lblTotalCajaVal = new Label();
            panelRoot.SuspendLayout();
            panelCard.SuspendLayout();
            panelHeader.SuspendLayout();
            SuspendLayout();
            // ===== FORM GENERAL =====
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(900, 520);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CajaForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Cuadre de Caja - MLB Virtual";
            BackColor = Color.FromArgb(10, 16, 30);
            // ===== ROOT =====
            panelRoot.Dock = DockStyle.Fill;
            panelRoot.Padding = new Padding(24);
            panelRoot.BackColor = Color.Transparent;
            panelRoot.Controls.Add(panelCard);
            // ===== CARD CENTRAL (similar a panelMain de BetForm) =====
            panelCard.Dock = DockStyle.Fill;
            panelCard.BackColor = Color.FromArgb(18, 24, 44);
            panelCard.Padding = new Padding(16);
            panelCard.BorderStyle = BorderStyle.None;
            // Añadimos controles al card (orden importante)
            panelCard.Controls.Add(btnImprimir);
            panelCard.Controls.Add(lblTotalCaja);
            panelCard.Controls.Add(lblTotalCajaVal);
            panelCard.Controls.Add(lblMontoPagado);
            panelCard.Controls.Add(lblMontoPagadoVal);
            panelCard.Controls.Add(lblBalance);
            panelCard.Controls.Add(lblBalanceVal);
            panelCard.Controls.Add(lblMontoGanado);
            panelCard.Controls.Add(lblMontoGanadoVal);
            panelCard.Controls.Add(lblRetiros);
            panelCard.Controls.Add(lblRetirosVal);
            panelCard.Controls.Add(lblMontoApostado);
            panelCard.Controls.Add(lblMontoApostadoVal);
            panelCard.Controls.Add(lblRecargas);
            panelCard.Controls.Add(lblRecargasVal);
            panelCard.Controls.Add(dtpDesde);
            panelCard.Controls.Add(dtpHasta);
            panelCard.Controls.Add(btnBuscar);
            panelCard.Controls.Add(panelHeader);
            // ===== HEADER NEGRO + TEXTO AMARILLO =====
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 60;
            panelHeader.BackColor = Color.Black;
            panelHeader.Padding = new Padding(4);
            panelHeader.Controls.Add(lblTitle);
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.Yellow;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Text = "Reporte de Venta MLB VIRTUAL";
            // ===== FECHAS + BOTÓN BUSCAR =====
            dtpDesde.Format = DateTimePickerFormat.Short;
            dtpDesde.Location = new Point(60, 90);
            dtpDesde.Name = "dtpDesde";
            dtpDesde.Size = new Size(180, 23);
            dtpDesde.TabIndex = 0;
            dtpHasta.Format = DateTimePickerFormat.Short;
            dtpHasta.Location = new Point(350, 90);
            dtpHasta.Name = "dtpHasta";
            dtpHasta.Size = new Size(180, 23);
            dtpHasta.TabIndex = 1;
            btnBuscar.Location = new Point(640, 84);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(160, 35);
            btnBuscar.TabIndex = 2;
            btnBuscar.Text = "Buscar";
            btnBuscar.UseVisualStyleBackColor = true;
            btnBuscar.Click += btnBuscar_Click;
            // ===== BLOQUE MONTOS =====
            // Monto Apostado
            lblMontoApostado.AutoSize = true;
            lblMontoApostado.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            lblMontoApostado.ForeColor = Color.WhiteSmoke;
            lblMontoApostado.Location = new Point(60, 150);
            lblMontoApostado.Text = "Monto Apostado: RD$";
            lblMontoApostadoVal.AutoSize = true;
            lblMontoApostadoVal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblMontoApostadoVal.ForeColor = Color.White;
            lblMontoApostadoVal.Location = new Point(255, 150);
            lblMontoApostadoVal.Text = "0";
            // Recargas
            lblRecargas.AutoSize = true;
            lblRecargas.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            lblRecargas.ForeColor = Color.WhiteSmoke;
            lblRecargas.Location = new Point(60, 175);
            lblRecargas.Text = "Recargas: RD$";
            lblRecargasVal.AutoSize = true;
            lblRecargasVal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblRecargasVal.ForeColor = Color.White;
            lblRecargasVal.Location = new Point(255, 175);
            lblRecargasVal.Text = "0";
            // Monto Ganado
            lblMontoGanado.AutoSize = true;
            lblMontoGanado.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            lblMontoGanado.ForeColor = Color.WhiteSmoke;
            lblMontoGanado.Location = new Point(350, 150);
            lblMontoGanado.Text = "Monto Ganado: RD$";
            lblMontoGanadoVal.AutoSize = true;
            lblMontoGanadoVal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblMontoGanadoVal.ForeColor = Color.White;
            lblMontoGanadoVal.Location = new Point(545, 150);
            lblMontoGanadoVal.Text = "0";
            // Retiros
            lblRetiros.AutoSize = true;
            lblRetiros.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            lblRetiros.ForeColor = Color.WhiteSmoke;
            lblRetiros.Location = new Point(350, 175);
            lblRetiros.Text = "Retiros: RD$";
            lblRetirosVal.AutoSize = true;
            lblRetirosVal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblRetirosVal.ForeColor = Color.White;
            lblRetirosVal.Location = new Point(545, 175);
            lblRetirosVal.Text = "0";
            // Monto Pagado
            lblMontoPagado.AutoSize = true;
            lblMontoPagado.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            lblMontoPagado.ForeColor = Color.WhiteSmoke;
            lblMontoPagado.Location = new Point(60, 215);
            lblMontoPagado.Text = "Monto Pagado: RD$";
            lblMontoPagadoVal.AutoSize = true;
            lblMontoPagadoVal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblMontoPagadoVal.ForeColor = Color.White;
            lblMontoPagadoVal.Location = new Point(255, 215);
            lblMontoPagadoVal.Text = "0";
            // Balance
            lblBalance.AutoSize = true;
            lblBalance.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            lblBalance.ForeColor = Color.WhiteSmoke;
            lblBalance.Location = new Point(350, 215);
            lblBalance.Text = "Balance: RD$";
            lblBalanceVal.AutoSize = true;
            lblBalanceVal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblBalanceVal.ForeColor = Color.White;
            lblBalanceVal.Location = new Point(545, 215);
            lblBalanceVal.Text = "0";
            // Total en caja (barra amarilla)
            lblTotalCaja.BackColor = Color.Gold;
            lblTotalCaja.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            lblTotalCaja.ForeColor = Color.Black;
            lblTotalCaja.Location = new Point(520, 260);
            lblTotalCaja.Padding = new Padding(8, 4, 8, 4);
            lblTotalCaja.Size = new Size(180, 25);
            lblTotalCaja.TextAlign = ContentAlignment.MiddleLeft;
            lblTotalCaja.Text = "Total en Caja: RD$";
            lblTotalCajaVal.AutoSize = true;
            lblTotalCajaVal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTotalCajaVal.ForeColor = Color.Black;
            lblTotalCajaVal.Location = new Point(670, 264);
            lblTotalCajaVal.Text = "0";
            // ===== BOTÓN PRINT =====
            btnImprimir.Location = new Point(60, 260);
            btnImprimir.Name = "btnImprimir";
            btnImprimir.Size = new Size(220, 35);
            btnImprimir.TabIndex = 3;
            btnImprimir.Text = "Print";
            btnImprimir.UseVisualStyleBackColor = true;
            btnImprimir.Click += btnImprimir_Click;
            // ===== ADD ROOT =====
            Controls.Add(panelRoot);
            panelRoot.ResumeLayout(false);
            panelCard.ResumeLayout(false);
            panelCard.PerformLayout();
            panelHeader.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
