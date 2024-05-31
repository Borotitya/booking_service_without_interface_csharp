using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace TripPlanner
{
    public partial class MainForm : Form
    {
        private ComboBox cbCategory;
        private TextBox tbDestination;
        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;
        private Label lblTotalCost;
        private double totalCost = 0;
        private List<string> categories = new List<string> { "Отель", "Авиабилет", "Тур", "Ресторан", "Автомобиль" };
        private List<(string Category, string Destination, string FromDate, string ToDate, double Cost)> bookings = new List<(string, string, string, string, double)>();

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.cbCategory = new ComboBox();
            this.tbDestination = new TextBox();
            this.dtpFrom = new DateTimePicker();
            this.dtpTo = new DateTimePicker();
            this.lblTotalCost = new Label();

            Button btnBook = new Button { Text = "Забронировать", Location = new Point(10, 180), Size = new Size(100, 30) };
            btnBook.Click += new EventHandler(this.Book);

            Button btnShowTable = new Button { Text = "Показать таблицу", Location = new Point(120, 180), Size = new Size(100, 30) };
            btnShowTable.Click += new EventHandler(this.ShowTable);

            Button btnShowPrices = new Button { Text = "Показать цены", Location = new Point(230, 180), Size = new Size(100, 30) };
            btnShowPrices.Click += new EventHandler(this.ShowPrices);

            this.cbCategory.Location = new Point(10, 140);
            this.cbCategory.Size = new Size(200, 30);
            this.cbCategory.DropDownStyle = ComboBoxStyle.DropDownList;

            this.tbDestination.Location = new Point(10, 30);
            this.tbDestination.Size = new Size(280, 30);

            this.dtpFrom.Location = new Point(10, 90);
            this.dtpFrom.Size = new Size(200, 30);

            this.dtpTo.Location = new Point(220, 90);
            this.dtpTo.Size = new Size(200, 30);

            this.lblTotalCost.Location = new Point(10, 220);
            this.lblTotalCost.Size = new Size(200, 30);
            this.lblTotalCost.Text = "Общая стоимость: 0.00 руб.";

            Label lblCategory = new Label { Text = "Выберите категорию:", Location = new Point(10, 120), Size = new Size(200, 20) };
            Label lblDestination = new Label { Text = "Введите город для отдыха:", Location = new Point(10, 10), Size = new Size(200, 20) };
            Label lblFromDate = new Label { Text = "Выбор даты от:", Location = new Point(10, 70), Size = new Size(200, 20) };
            Label lblToDate = new Label { Text = "Выбор даты до:", Location = new Point(220, 70), Size = new Size(200, 20) };

            this.Controls.Add(lblCategory);
            this.Controls.Add(lblDestination);
            this.Controls.Add(lblFromDate);
            this.Controls.Add(lblToDate);
            this.Controls.Add(this.cbCategory);
            this.Controls.Add(this.tbDestination);
            this.Controls.Add(this.dtpFrom);
            this.Controls.Add(this.dtpTo);
            this.Controls.Add(this.lblTotalCost);
            this.Controls.Add(btnBook);
            this.Controls.Add(btnShowTable);
            this.Controls.Add(btnShowPrices);

            this.Text = "Планировщик поездок";
            this.Size = new Size(450, 300);

            InitializeBookingServices();
        }

        private void InitializeBookingServices()
        {
            foreach (var category in categories)
            {
                cbCategory.Items.Add(category);
            }
        }

        private void Book(object sender, EventArgs e)
        {
            string destination = tbDestination.Text;
            string fromDate = dtpFrom.Value.ToString("dd/MM/yyyy");
            string toDate = dtpTo.Value.ToString("dd/MM/yyyy");

            if (cbCategory.SelectedIndex == -1)
            {
                MessageBox.Show("Пожалуйста, выберите категорию.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string category = categories[cbCategory.SelectedIndex];
            int days = (dtpTo.Value - dtpFrom.Value).Days + 1;
            double cost = CalculateCost(category, days);

            totalCost += cost;
            bookings.Add((category, destination, fromDate, toDate, cost));

            lblTotalCost.Text = $"Общая стоимость: {totalCost:0.00} руб.";
            MessageBox.Show($"{category} забронирован для направления: {destination}. С: {fromDate}. По: {toDate}", $"Бронирование {category}", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private double CalculateCost(string category, int days)
        {
            switch (category)
            {
                case "Отель":
                    return 30000.0 * days;
                case "Авиабилет":
                    return 9000.0;
                case "Тур":
                    return 5000.0 * days;
                case "Ресторан":
                    return 1800.0 * days;
                case "Автомобиль":
                    return 2000.0 * days;
                default:
                    return 0.0;
            }
        }

        private void ShowTable(object sender, EventArgs e)
        {
            Form tableForm = new Form { Text = "Таблица бронирований", Size = new Size(500, 300) };

            TextBox tbBookings = new TextBox { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, Dock = DockStyle.Fill };
            tableForm.Controls.Add(tbBookings);

            foreach (var booking in bookings)
            {
                tbBookings.AppendText($"{booking.Category}: {booking.Destination} с {booking.FromDate} по {booking.ToDate} - {booking.Cost:0.00} руб.{Environment.NewLine}");
            }

            tableForm.Show();
        }

        private void ShowPrices(object sender, EventArgs e)
        {
            Form pricesForm = new Form { Text = "Список цен", Size = new Size(300, 200) };

            TextBox tbPrices = new TextBox { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, Dock = DockStyle.Fill };
            pricesForm.Controls.Add(tbPrices);

            foreach (var category in categories)
            {
                tbPrices.AppendText($"{category}: {CalculateCost(category, 1):0.00} руб. в день{Environment.NewLine}");
            }

            pricesForm.Show();
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
