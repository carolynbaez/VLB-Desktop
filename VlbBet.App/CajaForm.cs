using System;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using VlbBet.Core;
using VlbBet.Infrastructure;

namespace VlbBet.App
{
    public partial class CajaForm : Form
    {
        private readonly TicketApiClient _ticketClient;
        private readonly CashDrawerApiClient _cashDrawerApiClient;

        public CajaForm(TicketApiClient ticketClient = null)
        {
            InitializeComponent();

            var baseAddress = new Uri("https://vlb.virsbet.com/");

            _ticketClient = ticketClient ?? new TicketApiClient(baseAddress);

            // ✅ YA NO ES NULL: inicializamos el cliente de caja
            _cashDrawerApiClient = new CashDrawerApiClient(baseAddress);

            // Defaults del rango
            dtpHasta.Value = DateTime.Now;
            dtpDesde.Value = DateTime.Now.Date;

            ApplyStyle();
        }

        private void ApplyStyle()
        {
            panelCard.BackColor = Color.FromArgb(18, 24, 44);

            StyleDatePicker(dtpDesde);
            StyleDatePicker(dtpHasta);

            StylePrimaryButton(btnImprimir);
            StyleAccentButton(btnBuscar);
        }

        private void StylePrimaryButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(33, 150, 243);
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
        }

        private void StyleAccentButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(244, 67, 54);
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
        }

        private void StyleDatePicker(DateTimePicker dtp)
        {
            dtp.CalendarTitleBackColor = Color.RoyalBlue;
            dtp.CalendarForeColor = Color.Black;
        }

        private static string Money(decimal v)
        {
            // RD$ o US$ depende de tu Culture. Si quieres RD$, cambia a "es-DO"
            // return v.ToString("N2", CultureInfo.GetCultureInfo("es-DO"));
            return v.ToString("N2", CultureInfo.InvariantCulture);
        }

        private void SetLoading(bool loading)
        {
            btnBuscar.Enabled = !loading;
            btnImprimir.Enabled = !loading;
            Cursor = loading ? Cursors.WaitCursor : Cursors.Default;
        }

        private void ClearValues()
        {
            lblMontoApostadoVal.Text = "0.00";
            lblRecargasVal.Text = "0.00";
            lblMontoGanadoVal.Text = "0.00";
            lblRetirosVal.Text = "0.00";
            lblMontoPagadoVal.Text = "0.00";
            lblBalanceVal.Text = "0.00";
            lblTotalCajaVal.Text = "0.00";
        }

        private async void btnBuscar_Click(object sender, EventArgs e)
        {
            var init = dtpDesde.Value.Date;
            var end = dtpHasta.Value.Date;

            if (end < init)
            {
                MessageBox.Show("La fecha 'end' no puede ser menor que 'init'.", "Rango inválido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SetLoading(true);

                // ✅ Llamada real al API (POST por defecto)
                var balance = await _cashDrawerApiClient.GetBalanceAsync(init, end, AppSession.UserId, AppSession.Point); 

                // ✅ Pintar valores (si vienen null, usa 0)
                var totalSales = balance?.Total ?? 0m;
                var totalDeposits = balance?.Deposito ?? 0m;
                var totalWins = balance?.Winned ?? 0m;
                var totalWithdrawals = balance?.Balance ?? 0m;
                var totalPaid = balance?.Paid ?? 0m;

                // Puedes decidir si "Balance" y "TotalCaja" vienen servidos o los calculas aquí.
                var balanceVal = totalSales - totalWins;
                var totalCaja = balance?.Total ?? balanceVal;

                lblMontoApostadoVal.Text = Money(balance.Betted ?? 0);     // betted
                lblMontoGanadoVal.Text = Money(balance.Winned ?? 0);     // winned
                lblMontoPagadoVal.Text = Money(balance.Paid ?? 0);       // paid
                lblBalanceVal.Text = Money(balance.Balance ?? 0);    // balance
                lblRecargasVal.Text = Money(balance.Deposito ?? 0);   // deposito
                lblRetirosVal.Text = Money(balance.Retiro ?? 0);     // retiro
                lblTotalCajaVal.Text = Money(totalCaja);      // total

                var printObject = new PrintObject();
                printObject.logo = "MLB";
                printObject.Type = "Cuadre";
                printObject.data = balance;

                await PrintModule.ToPrintAsync(printObject);

            }
            catch (Exception ex)
            {
                ClearValues();
                MessageBox.Show(
                    "No se pudo obtener el cuadre de caja.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Impresión de reporte de caja (pendiente implementar).",
                "Print", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
