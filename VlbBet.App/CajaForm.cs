using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using VlbBet.Infrastructure;

namespace VlbBet.App
{
    public partial class CajaForm : Form
    {
        private readonly TicketApiClient _ticketClient;

        public CajaForm(TicketApiClient ticketClient = null)
        {
            InitializeComponent();

            // Reusar cliente HTTP si viene desde BetForm
            if (ticketClient != null)
            {
                _ticketClient = ticketClient;
            }
            else
            {
                var baseAddress = new Uri("https://vlb.virsbet.com/");
                _ticketClient = new TicketApiClient(baseAddress);
            }

            ApplyStyle();
        }

        private void ApplyStyle()
        {
            // Fondo general ya oscuro en Designer

            // Card en color oscuro (ya en Designer)
            panelCard.BackColor = Color.FromArgb(18, 24, 44);

            // DatePickers más integrados
            StyleDatePicker(dtpDesde);
            StyleDatePicker(dtpHasta);

            // Botones con el mismo estilo que BetForm
            StylePrimaryButton(btnImprimir); // azul
            StyleAccentButton(btnBuscar);    // rojo

            // Labels ya tienen colores puestos en Designer
        }

        // ==== Helpers de estilo (mismos tonos que BetForm) ====

        private void StylePrimaryButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(33, 150, 243); // azul
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
        }

        private void StyleAccentButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(244, 67, 54); // rojo
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
        }

        private void StyleDatePicker(DateTimePicker dtp)
        {
            dtp.CalendarTitleBackColor = Color.RoyalBlue;
            dtp.CalendarForeColor = Color.Black;
        }

        // ==== LÓGICA (placeholder; luego conectamos con tu API Node) ====

        private async void btnBuscar_Click(object sender, EventArgs e)
        {
            DateTime desde = dtpDesde.Value.Date;
            DateTime hasta = dtpHasta.Value.Date;



            // Aquí debes llamar a tu endpoint de cuadre de caja.
            // Por ahora solo simulo respuesta:
            await Task.Delay(100);

            lblMontoApostadoVal.Text = "0";
            lblRecargasVal.Text = "0";
            lblMontoGanadoVal.Text = "0";
            lblRetirosVal.Text = "0";
            lblMontoPagadoVal.Text = "0";
            lblBalanceVal.Text = "0";
            lblTotalCajaVal.Text = "0";
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            // Aquí integrarás tu ToPrint o impresión nativa
            MessageBox.Show("Impresión de reporte de caja (pendiente implementar).",
                "Print", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
