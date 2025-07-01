using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Avia
{
    public partial class EditRowWindow : Window
    {
        private object _editingObject;
        private string _tableName;
        private string _mode; // "add" или "edit"
        private Dictionary<string, Control> _controls = new();

        public EditRowWindow(string tableName, object editingObject, string mode = "edit")
        {
            InitializeComponent();
            _editingObject = editingObject;
            _tableName = tableName;
            _mode = mode;
            BuildFields();
        }

        public object ResultObject => _editingObject;

        private void BuildFields()
        {
            FieldsPanel.Children.Clear();
            _controls.Clear();

            if (_tableName == "Users")
            {
                AddTextField("Login", "Логин", GetProp("Login"));
                AddPasswordField("Password", "Пароль", _mode == "edit" ? "" : GetProp("Password"));
                AddComboBox("Role", "Роль", new[] { "admin", "client" }, GetProp("Role"));
            }
            else if (_tableName == "Flights")
            {
                AddTextField("Departure", "Отправление", GetProp("Departure"));
                AddTextField("Destination", "Назначение", GetProp("Destination"));
                AddDatePicker("Date", "Дата", GetProp("Date"));
                AddTextField("Airline", "Авиакомпания", GetProp("Airline"));
                AddTextField("Price", "Цена", GetProp("Price"));
                AddTextField("SeatsTotal", "Всего мест", GetProp("SeatsTotal"));
                AddTextField("SeatsAvailable", "Свободно мест", GetProp("SeatsAvailable"));
                AddTextField("BaggageInfo", "Багаж", GetProp("BaggageInfo"));
            }
            else if (_tableName == "Bookings")
            {
                // Для UserId и FlightId лучше использовать ComboBox с вариантами, но здесь для простоты - TextBox
                AddTextField("UserId", "ID пользователя", GetProp("UserId"));
                AddTextField("FlightId", "ID рейса", GetProp("FlightId"));
                AddDatePicker("BookingDate", "Дата бронирования", GetProp("BookingDate"));
                AddComboBox("Status", "Статус", new[] { "booked", "paid", "cancelled" }, GetProp("Status"));
                AddTextField("SeatsReserved", "Забронировано мест", GetProp("SeatsReserved"));
            }
        }

        private string? GetProp(string prop) => _editingObject.GetType().GetProperty(prop)?.GetValue(_editingObject)?.ToString();

        private void SetProp(string prop, object? value)
        {
            var property = _editingObject.GetType().GetProperty(prop);
            if (property != null)
            {
                if (property.PropertyType == typeof(int) && int.TryParse(value?.ToString(), out int intVal))
                    property.SetValue(_editingObject, intVal);
                else if (property.PropertyType == typeof(decimal) && decimal.TryParse(value?.ToString(), out decimal decVal))
                    property.SetValue(_editingObject, decVal);
                else if (property.PropertyType == typeof(DateTimeOffset) && value is DateTime dt)
                    property.SetValue(_editingObject, new DateTimeOffset(dt));
                else if (property.PropertyType == typeof(DateTimeOffset) && value is DateTimeOffset dto)
                    property.SetValue(_editingObject, dto);
                else
                    property.SetValue(_editingObject, value);
            }
        }

        private void AddTextField(string prop, string label, string? value)
        {
            var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 0) };
            panel.Children.Add(new Label { Content = label, Width = 120 });
            var tb = new TextBox { Text = value ?? "", Width = 200 };
            panel.Children.Add(tb);
            FieldsPanel.Children.Add(panel);
            _controls[prop] = tb;
        }

        private void AddPasswordField(string prop, string label, string? value)
        {
            var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 0) };
            panel.Children.Add(new Label { Content = label, Width = 120 });
            var pb = new PasswordBox { Password = value ?? "", Width = 200 };
            panel.Children.Add(pb);
            FieldsPanel.Children.Add(panel);
            _controls[prop] = pb;
        }

        private void AddComboBox(string prop, string label, IEnumerable<string> items, string? value)
        {
            var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 0) };
            panel.Children.Add(new Label { Content = label, Width = 120 });
            var cb = new ComboBox { Width = 200, ItemsSource = items.ToList(), SelectedValue = value ?? items.First() };
            panel.Children.Add(cb);
            FieldsPanel.Children.Add(panel);
            _controls[prop] = cb;
        }

        private void AddDatePicker(string prop, string label, string? value)
        {
            var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 0) };
            panel.Children.Add(new Label { Content = label, Width = 120 });
            var dp = new DatePicker { Width = 200 };
            if (DateTimeOffset.TryParse(value, out var dto))
                dp.SelectedDate = dto.DateTime;
            panel.Children.Add(dp);
            FieldsPanel.Children.Add(panel);
            _controls[prop] = dp;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var kv in _controls)
                {
                    object? val = null;
                    if (kv.Value is TextBox tb)
                        val = tb.Text;
                    else if (kv.Value is PasswordBox pb)
                        val = pb.Password;
                    else if (kv.Value is ComboBox cb)
                        val = cb.SelectedValue?.ToString();
                    else if (kv.Value is DatePicker dp)
                        val = dp.SelectedDate ?? DateTime.Now;

                    // Особая обработка пароля
                    if (_tableName == "Users" && kv.Key == "Password")
                    {
                        if (!string.IsNullOrEmpty(val?.ToString()))
                        {
                            val = PasswordHasher.HashPassword(val.ToString());
                        }
                        else if (_mode == "edit")
                        {
                            // Не менять пароль, если поле пустое при редактировании
                            continue;
                        }
                    }
                    SetProp(kv.Key, val);
                }
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
