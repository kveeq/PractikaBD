using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using Microsoft.Data.SqlClient;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Drawing;
using Microsoft.VisualBasic;
using System.IO;

// все машины добавит в один лист, при добавлении, каждый вн ключ присваивать к комбобокусу  присваитвать имя выбранного элемента комбобокса к выводимому
namespace PractikaBD
{
    /// <summary>
    /// Interaction logic for MainWindow. xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int x = 0;
        private List<Car> autos;
        private Client client;
        private List<Client> clients;
        private List<Client> updateClients;
        private List<Contract> contract;
        private bool state = true;

        public MainWindow()
        {
            InitializeComponent();
            tabControl.Visibility = Visibility.Hidden;
            GetInfoForCmbBoxAdd("Fuel", Fuel_add);
            GetInfoForCmbBoxAdd("Kpp", KPP_add);
            GetInfoForCmbBoxAdd("Marka", Marka_add);
            //GetInfo("Model", Model_add);
            GetInfoForCmbBoxAdd("Country", Country_add);
            GetInfoForCmbBoxAdd("DriveUnit", Privod_add);
            autos = GetList();
            Show_search(x);
            clients = GetListClient();
            data.ItemsSource = clients;
            NewModel_add.Visibility = Visibility.Hidden;
            BtnAddInBd.Visibility = Visibility.Hidden;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedIndex == 1)
            {
                FillProfil();
            }

            if (tabControl.SelectedIndex == 5)
            {
                GetContracts();
                contracts.ItemsSource = contract;
            }
        }

        private void FillProfil()
        {
            ProfilStatus.Text = $"Ваш статус: {client.Role}";//😊😊
            ProfilName.Text = client.Name;
            ProfilSurname.Text = client.Surname;
            ProfilEmail.Text = client.Email;
            ProfilTelephoneNumber.Text = client.TelePhoneNumber;
        }

        private List<Contract> GetContracts()
        {
            contract = new List<Contract>();
            string connectionString = @"Data Source=DESKTOP-OS1FU9G\SQLEXPRESS;Initial Catalog=AutoProcat;Integrated Security=True";
            string sqlExpression = $"SELECT * FROM Contract f Where IdClient = {client.IdClient}";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                Contract contrac = new Contract();
                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        //MessageBox.Show(reader.GetValue(0).ToString());
                        contrac = new Contract();
                        contrac.IdCar = int.Parse(reader.GetValue(0).ToString());
                        contrac.IdClient = int.Parse(reader.GetValue(1).ToString());
                        contrac.Time = int.Parse(reader.GetValue(2).ToString());
                        contrac.TotalPrice = int.Parse(reader.GetValue(3).ToString());
                        contract.Add(contrac);
                    }
                }
                reader.Close();
            }

            return contract;
        }

        private void GetInfoForCmbBoxAdd(string table, ComboBox Fuel_add)
        {
            string connectionString = @"Data Source=DESKTOP-OS1FU9G\SQLEXPRESS;Initial Catalog=AutoProcat;Integrated Security=True";
            string sqlExpression = $"SELECT f.Name FROM {table} f";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    // выводим названия столбцов
                    string columnName1 = reader.GetName(0);
                    bool exist = false;
                    while (reader.Read()) // построчно считываем данные
                    {
                        object name = reader.GetValue(0);
                        foreach (var item in Fuel_add.Items)
                        {
                            if (item.ToString() == name.ToString())
                                exist = true;
                        }
                        if (exist == false)
                            Fuel_add.Items.Add(name);
                        exist = false;
                    }
                }

                reader.Close();
            }
        }

        private void GetModelInfoCmbBoxAdd()
        {
            string connectionString = @"Data Source=DESKTOP-OS1FU9G\SQLEXPRESS;Initial Catalog=AutoProcat;Integrated Security=True";
            string sqlExpression = $"Select * FROM Marka m JOIN Model mo ON m.IdModel = mo.IdModel";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    // выводим названия столбцов
                    string columnName1 = reader.GetName(0);

                    while (reader.Read()) // построчно считываем данные
                    {
                        if (Marka_add.SelectedItem != null)
                        {
                            if (Marka_add.SelectedItem.ToString() == reader.GetValue(1).ToString())
                            {
                                Model_add.Items.Add(reader.GetValue(4).ToString());
                            }
                        }
                        //object name = reader.GetValue(0);
                        //Fuel_add.Items.Add(name);
                    }
                }

                reader.Close();
            }
        }

        public byte[] getJPGFromImageControl(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.ToArray();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ADD_image.Source == null)
                    MessageBox.Show("Добавьте картинку машины");
                else
                    AddCarInBd();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось добавить в базу {ex.Message}");
            }
        }

        private void AddCarInBd()
        {
            int a = GetModelId();
            string connectionString = @"Data Source=DESKTOP-OS1FU9G\SQLEXPRESS;Initial Catalog=AutoProcat;Integrated Security=True";
            var image = getJPGFromImageControl(ADD_image.Source as BitmapImage);
            string sqlExpression = $"INSERT INTO Car (\"IdFuel\", \"IdKpp\", \"IdDriveUnit\", \"IdMarka\", \"IdCountry\", \"Color\", \"NumberOfSeats\", \"Consumption\", \"Horsepower\", \"Price\", \"EngineVolume\", \"Image\", \"IdModel\") VALUES ({Fuel_add.SelectedIndex + 1}, {KPP_add.SelectedIndex + 1}, {Privod_add.SelectedIndex + 1}, {Marka_add.SelectedIndex + 1}, {Country_add.SelectedIndex + 1}, '{Color_add.Text}', {Col_mest_add.Text}, {Rashod_add.Text}, {Power_add.Text}, {Price_add.Text}, {V_add.Text}, @image, {a})";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@image", image);
                command.CommandText = sqlExpression;
                //SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.ExecuteNonQuery();
            }
            MessageBox.Show("Добавление прошло удачно");
        }

        private int GetModelId()
        {
            string connectionString = @"Data Source=DESKTOP-OS1FU9G\SQLEXPRESS;Initial Catalog=AutoProcat;Integrated Security=True";
            string sqlExpression = $"Select mo.IdModel FROM Marka m JOIN Model mo ON m.IdModel = mo.IdModel where mo.Name = '{Model_add.SelectedItem}'";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();


                if (reader.HasRows) // если есть данные //  ошибка
                {
                    // выводим названия столбцов

                    while (reader.Read()) // построчно считываем данные
                    {
                        return Convert.ToInt32(reader.GetValue(0));
                        //object name = reader.GetValue(0);
                        //Fuel_add.Items.Add(name);
                    }
                }

                reader.Close();
            }

            return 0;
        }

        private List<Car> GetList()
        {
            autos = new List<Car>();
            string connectionString = @"Data Source=DESKTOP-OS1FU9G\SQLEXPRESS;Initial Catalog=AutoProcat;Integrated Security=True";
            string sqlExpression = $"SELECT * FROM Car f";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                Car car = new Car();
                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        //MessageBox.Show(reader.GetValue(0).ToString());
                        car = new Car();
                        car.IdCar = int.Parse(reader.GetValue(0).ToString());
                        car.IdFuel = int.Parse(reader.GetValue(1).ToString());
                        car.IdKpp = int.Parse(reader.GetValue(2).ToString());
                        car.IdDriveUnit = int.Parse(reader.GetValue(3).ToString());
                        car.IdModel = int.Parse(reader.GetValue(13).ToString());
                        car.IdMarka = int.Parse(reader.GetValue(4).ToString());
                        car.IdCountry = int.Parse(reader.GetValue(5).ToString());
                        car.Color = reader.GetValue(6).ToString();
                        car.NumberOfSeats = int.Parse(reader.GetValue(7).ToString());
                        car.Consumption = int.Parse(reader.GetValue(8).ToString());
                        car.HorsePower = int.Parse(reader.GetValue(9).ToString());
                        car.Price = int.Parse(reader.GetValue(10).ToString());
                        car.EngineVolume = int.Parse(reader.GetValue(11).ToString());
                        autos.Add(car);
                    }
                }
                reader.Close();
            }

            return autos;
        }

        private List<Client> GetListClient()
        {
            clients = new List<Client>();
            string connectionString = @"Data Source=DESKTOP-OS1FU9G\SQLEXPRESS;Initial Catalog=AutoProcat;Integrated Security=True";
            string sqlExpression = $"SELECT * FROM Client f";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                Client client = new Client();
                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        //MessageBox.Show(reader.GetValue(0).ToString());
                        client = new Client();
                        client.IdClient = int.Parse(reader.GetValue(0).ToString());
                        client.Name = reader.GetValue(1).ToString();
                        client.Surname = reader.GetValue(2).ToString();
                        client.Email = reader.GetValue(3).ToString();
                        client.TelePhoneNumber = reader.GetValue(4).ToString();
                        int a = int.Parse(reader.GetValue(6).ToString());
                        if (a == 1)
                            client.Role = "Администратор";
                        else
                            client.Role = "Пользователь";
                        clients.Add(client);
                    }
                }
                reader.Close();
            }

            return clients;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            autos = GetList();
            TakeList(Marka_search.Text, Model_search.Text, Power_search.Text, Rashod_search.Text, Privod_search.Text, V_search.Text, KPP_search.Text, Fuel_search.Text, Color_search.Text, Col_mest_search.Text, Country_search.Text, Price_1_search.Text, Price_2_search.Text);
            if (x + 2 <= autos.Count - 1)
            {
                x += 2;

            }
            else
            {
                x = 0;
            }

            Show_search(x);
            DoHiddenAndVisible();
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            autos = GetList();
            TakeList(Marka_search.Text, Model_search.Text, Power_search.Text, Rashod_search.Text, Privod_search.Text, V_search.Text, KPP_search.Text, Fuel_search.Text, Color_search.Text, Col_mest_search.Text, Country_search.Text, Price_1_search.Text, Price_2_search.Text);
            if (x - 2 >= 0)
            {
                x -= 2;
            }
            else
            {
                x = autos.Count - 1;
            }

            Show_search(x);
            DoHiddenAndVisible();
        }

        private void DoHiddenAndVisible()
        {
            if (Block_1_label_1.Text == "")
            {
                Block_1.Visibility = Visibility.Hidden;
                Bron_button_1.Visibility = Visibility.Hidden;
                label_price_1.Visibility = Visibility.Hidden;
            }
            else
            {
                Block_1.Visibility = Visibility.Visible;
                Bron_button_1.Visibility = Visibility.Visible;
                label_price_1.Visibility = Visibility.Visible;
            }
            if (Block_2_label_1.Text == "")
            {
                Block_2.Visibility = Visibility.Hidden;
                Bron_button_2.Visibility = Visibility.Hidden;
                label_price_2.Visibility = Visibility.Hidden;
            }
            else
            {
                Block_2.Visibility = Visibility.Visible;
                Bron_button_2.Visibility = Visibility.Visible;
                label_price_2.Visibility = Visibility.Visible;
            }
        }

        private void Show_search(int count)
        {
            if (count <= autos.Count)
            {
                try
                {
                    GetValidData(count);
                    Block_1_label_2.Text = $"{autos[count].Marka} {autos[count].Model}";
                    Block_1_label_1.Text = $"{autos[count].HorsePower} л.с. /{autos[count].Consumption} л. /{autos[count].EngineVolume} л. /{autos[count].DriveUnit} привод /Кпп {autos[count].Kpp} /{autos[count].Fuel} /{autos[count].Color} /Кол-во мест {autos[count].NumberOfSeats} /{autos[count].Country}";
                    label_price_1.Content = $"{autos[count].Price} р./час";
                    Image_block_1.Source = GetImage(autos[count].IdCar);
                }
                catch
                {
                    Block_1_label_1.Clear();
                    Block_1_label_2.Clear();
                    label_price_1.Content = "";
                    Image_block_1.Source = null;
                }
            }
            else
            {
                Block_1_label_1.Clear();
                Block_1_label_2.Clear();
                label_price_1.Content = ""; ;
                Image_block_1.Source = null;
            }
            if (count + 1 <= autos.Count - 1)
            {
                Image_block_2.Source = GetImage(autos[count + 1].IdCar);
                try
                {
                    GetValidData(count + 1);
                    Block_2_label_2.Text = $"{autos[count + 1].Marka} {autos[count + 1].Model}";
                    Block_2_label_1.Text = $"{autos[count + 1].HorsePower} л.с. /{autos[count + 1].Consumption} л. /{autos[count + 1].EngineVolume} л. /{autos[count + 1].DriveUnit} привод /Кпп {autos[count + 1].Kpp} /{autos[count + 1].Fuel} /{autos[count + 1].Color} /Кол-во мест {autos[count + 1].NumberOfSeats} /{autos[count + 1].Country}";
                    label_price_2.Content = $"{autos[count + 1].Price} р./час";
                }
                catch
                {
                    Block_2_label_1.Clear();
                    Block_2_label_2.Clear();
                    label_price_2.Content = "";
                    Image_block_2.Source = null;
                }
            }
            else
            {
                Block_2_label_1.Clear();
                Block_2_label_2.Clear();
                label_price_2.Content = "";
                Image_block_2.Source = null;
            }

            Clear_add_Click(null, null);
        }

        private void GetValidData(int x)
        {
            autos[x].GetFuel(Fuel_add, autos[x].IdFuel);
            autos[x].GetKpp(KPP_add, autos[x].IdKpp);
            autos[x].GetMarka(Marka_add, autos[x].IdMarka);
            autos[x].GetDriveUnitl(Privod_add, autos[x].IdDriveUnit);
            autos[x].GetModel();
            autos[x].GetCountry(Country_add, autos[x].IdCountry);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            bool state = false;
            string connectionString = @"Data Source=DESKTOP-OS1FU9G\SQLEXPRESS;Initial Catalog=AutoProcat;Integrated Security=True";
            string sqlExpression = $"SELECT * FROM Client f";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        //MessageBox.Show(reader.GetValue(0).ToString());
                        if (txtEmail.Text == reader.GetValue(3).ToString())
                        {
                            if (txtPassword.Password == reader.GetValue(5).ToString())
                            {
                                Login.Visibility = Visibility.Hidden;
                                tabControl.Visibility = Visibility.Visible;
                                state = true;
                                client = new Client(int.Parse(reader.GetValue(0).ToString()), reader.GetValue(1).ToString(), reader.GetValue(2).ToString(), reader.GetValue(3).ToString(), reader.GetValue(4).ToString(), int.Parse(reader.GetValue(6).ToString()));

                                if (int.Parse(reader.GetValue(6).ToString()) == 2)
                                {
                                    AddCar.Visibility = Visibility.Hidden;
                                    ShowClients.Visibility = Visibility.Hidden;
                                }

                                else if (int.Parse(reader.GetValue(6).ToString()) == 1)
                                {
                                    AddCar.Visibility = Visibility.Visible;
                                    ShowClients.Visibility = Visibility.Visible;
                                }

                                FillProfil();
                            }
                        }
                    }
                    MessageBox.Show(state ? "Вы удачно авторизовались!" : "Не удалось войти! Логин или пароль не правильны!");
                }
                reader.Close();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Login.Visibility = Visibility.Hidden;
            Sign.Visibility = Visibility.Visible;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Login.Visibility = Visibility.Visible;
            Sign.Visibility = Visibility.Hidden;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                if (NameReg.Text != "" && SurnameReg.Text != "" && EmailReg.Text != "" && NumberPhoneReg.Text != "" && PasswordReg.Password != "" && CheckNick())
                    Registrate();
                else
                    throw new Exception("Заполните все поля!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось зарегистрироваться\\n {ex.Message}");
            }
        }

        private bool CheckNick()
        {
            GetListClient();
            foreach (var item in clients)
            {
                if (item.Email == EmailReg.Text)
                    throw new Exception("Такое email уже существует");
            }
            return true;
        }

        private void Registrate()
        {
            string connectionString = @"Data Source=DESKTOP-OS1FU9G\SQLEXPRESS;Initial Catalog=AutoProcat;Integrated Security=True";
            string sqlExpression = $"INSERT INTO Client (\"Name\", \"Surname\", \"Email\", \"TelephoneNumber\", \"Password\", \"IdRole\") VALUES ('{NameReg.Text}', '{SurnameReg.Text}', '{EmailReg.Text}', '{NumberPhoneReg.Text}', '{PasswordReg.Password}', {2})";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.ExecuteNonQuery();
            }

            MessageBox.Show("Вы удачно зарегистрировались");
        }

        private void Select_image_Click(object sender, RoutedEventArgs e)
        {
            var OFD = new OpenFileDialog();
            OFD.ShowDialog();
            string link = OFD.FileName;
            try
            {
                using (Bitmap bmp = new Bitmap(link))
                    ADD_image.Source = bmp.BitmapToImageSource();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Выберите корректное изображение \n Ошибка -  { ex.Message}");
            }
        }

        private BitmapImage GetImage(int id)
        {
            string connectionString = @"Data Source=DESKTOP-OS1FU9G\SQLEXPRESS;Initial Catalog=AutoProcat;Integrated Security=True";
            string sqlExpression = $"Select Image From Car Where IdCar = @IdCar";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@IdCar", id);
                command.CommandText = sqlExpression;
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    MemoryStream memory = new MemoryStream(reader.GetSqlBytes(0).Buffer);
                    Bitmap bitmap = new Bitmap(memory);
                    return bitmap.BitmapToImageSource();
                }
                command.ExecuteNonQuery();
            }

            return null;
        }

        private void Clear_add_Click(object sender, RoutedEventArgs e)
        {
            Fuel_add.SelectedItem = null;
            KPP_add.SelectedItem = null;
            Privod_add.SelectedItem = null;
            Model_add.SelectedItem = null;
            Marka_add.SelectedItem = null;
            ADD_image.Source = null;
            Country_add.SelectedItem = null;
            Color_add.Text = "";
            Col_mest_add.Text = "";
            Rashod_add.Text = "";
            Power_add.Text = "";
            Price_add.Text = "";
            V_add.Text = "";
            NewModel_add.Visibility = Visibility.Hidden;
            BtnAddInBd.Visibility = Visibility.Hidden;
        }

        private void Bron_button_1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BronCar(x);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось забронировать автомобиль\n {ex.Message}");
            }
        }

        private void Bron_button_2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BronCar(x + 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось забронировать автомобиль\n {ex.Message}");
            }
        }

        private void BronCar(int count)
        {
            int time = Convert.ToInt32(Interaction.InputBox("На сколько часов забронировать автомобиль?", "Бронь", "Введите кол-во часов"));
            string connectionString = @"Data Source=DESKTOP-OS1FU9G\SQLEXPRESS;Initial Catalog=AutoProcat;Integrated Security=True";
            string sqlExpression = $"INSERT INTO Contract (\"IdCar\", \"IdClient\", \"Time\", \"TotalPrice\") VALUES ({autos[count].IdCar}, {client.IdClient}, {time}, {time * autos[count].Price})";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.ExecuteNonQuery();
            }

            GetContracts();
            contracts.ItemsSource = contract;

            MessageBox.Show("Бронь успешно пройден");
        }

        private void TakeList(string _marka, string _model, string _power, string _rashod, string _privod, string _v, string _kpp, string _fuel, string _color, string _col_mest, string _country, string _price_1, string _price_2)
        {
            string[] mass = new string[11] { _marka, _model, _power, _rashod, _privod, _v, _kpp, _fuel, _color, _col_mest, _country };
            for (int j = 0; j < mass.Length; j++)
            {
                if (mass[j] != "")
                {
                    for (int i = 0; i < autos.Count; i++)
                    {
                        try
                        {
                            switch (j)
                            {
                                case 0:
                                    {
                                        if (autos[i].Marka != mass[j])
                                        {
                                            autos.RemoveAt(i);
                                            i--;
                                        }
                                    }
                                    break;
                                case 1:
                                    {
                                        if (autos[i].Model != mass[j])
                                        {
                                            autos.RemoveAt(i);
                                            i--;
                                        }
                                    }
                                    break;
                                case 2:
                                    {
                                        if (autos[i].HorsePower.ToString() != mass[j])
                                        {
                                            autos.RemoveAt(i);
                                            i--;
                                        }
                                    }
                                    break;
                                case 3:
                                    {
                                        if (autos[i].Consumption.ToString() != mass[j])
                                        {
                                            autos.RemoveAt(i);
                                            i--;
                                        }
                                    }
                                    break;
                                case 4:
                                    {
                                        if (autos[i].DriveUnit.ToString() != mass[j])
                                        {
                                            autos.RemoveAt(i);
                                            i--;
                                        }
                                    }
                                    break;
                                case 5:
                                    {
                                        if (autos[i].EngineVolume.ToString() != mass[j])
                                        {
                                            autos.RemoveAt(i);
                                            i--;
                                        }
                                    }
                                    break;
                                case 6:
                                    {
                                        if (autos[i].Kpp != mass[j])
                                        {
                                            autos.RemoveAt(i);
                                            i--;
                                        }
                                    }
                                    break;
                                case 7:
                                    {
                                        if (autos[i].Fuel != mass[j])
                                        {
                                            autos.RemoveAt(i);
                                            i--;
                                        }
                                    }
                                    break;
                                case 8:
                                    {
                                        if (autos[i].Color != mass[j])
                                        {
                                            autos.RemoveAt(i);
                                            i--;
                                        }
                                    }
                                    break;
                                case 9:
                                    {
                                        if (autos[i].NumberOfSeats.ToString() != mass[j])
                                        {
                                            autos.RemoveAt(i);
                                            i--;
                                        }
                                    }
                                    break;
                                case 10:
                                    {
                                        if (autos[i].Country != mass[j])
                                        {
                                            autos.RemoveAt(i);
                                            i--;
                                        }
                                    }
                                    break;
                            }
                        }
                        catch { }
                    }
                }
            }

            if (_price_1 == "")
            {
                _price_1 = "0";
            }
            if (_price_2 == "")
            {
                try
                {
                    for (int i = 0; i < autos.Count; i++)
                    {
                        if (Convert.ToInt32(autos[i].Price) < Convert.ToInt32(_price_1))
                        {
                            autos.RemoveAt(i);
                            i--;
                        }
                    }
                }
                catch { }
            }
            if (_price_2 != "")
            {
                try
                {
                    for (int i = 0; i < autos.Count; i++)
                    {
                        if (Convert.ToInt32(autos[i].Price) < Convert.ToInt32(_price_1) || Convert.ToInt32(autos[i].Price) > Convert.ToInt32(_price_2))
                        {
                            autos.RemoveAt(i);
                            i--;
                        }
                    }
                }
                catch { }
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            TakeList(Marka_search.Text, Model_search.Text, Power_search.Text, Rashod_search.Text, Privod_search.Text, V_search.Text, KPP_search.Text, Fuel_search.Text, Color_search.Text, Col_mest_search.Text, Country_search.Text, Price_1_search.Text, Price_2_search.Text);
            x = 0;
            Show_search(x);
            DoHiddenAndVisible();
        }

        private void ReductClientBtn_Click(object sender, RoutedEventArgs e)
        {
            updateClients = data.Items.Cast<Client>().ToList();
            string connectionString = @"Data Source=DESKTOP-OS1FU9G\SQLEXPRESS;Initial Catalog=AutoProcat;Integrated Security=True";
            string sqlExpression;
            foreach (var item in updateClients)
            {
                sqlExpression = $"Update Client Set Name = '{item.Name}', Surname = '{item.Surname}', Email = '{item.Email}', TelephoneNumber = '{item.TelePhoneNumber}' Where IdClient = {item.IdClient} ";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.ExecuteNonQuery();
                }
            }
            clients = GetListClient();
            data.ItemsSource = clients;
            MessageBox.Show("Данные успешно обновлены");
            ReductClientBtn.Content = "Редактировать";
        }

        private void DeleteClientBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(Interaction.InputBox("Введите ID клиента которую надо удалить?", "ID удаления", "Введите ID клиента"));
                if (client.IdClient != id)
                    deleteClient(id);
                else
                    throw new Exception("Нельзя удалить самого себя!");
            }
            catch
            {
                MessageBox.Show("Не удалось удалить клиента");
            }
        }

        private void deleteClient(int id)
        {
            string connectionString = @"Data Source=DESKTOP-OS1FU9G\SQLEXPRESS;Initial Catalog=AutoProcat;Integrated Security=True";
            string sqlExpression = $"DELETE FROM Contract WHERE IdClient = {id} DELETE FROM Client WHERE IdClient = {id}";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.ExecuteNonQuery();
            }
            clients = GetListClient();
            data.ItemsSource = clients;
            MessageBox.Show("Клиент успешно удален");
        }

        private void data_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            ReductClientBtn.Content = "Сохранить";
            updateClients = data.Items.Cast<Client>().ToList();
        }

        private void btnCloseProfil_Click(object sender, RoutedEventArgs e)
        {
            tabControl.Visibility = Visibility.Hidden;
            Login.Visibility = Visibility.Visible;
        }

        private void btnReductProfile_Click(object sender, RoutedEventArgs e)
        {
            state = !state;
            ProfilName.IsReadOnly = state;
            ProfilEmail.IsReadOnly = state;
            ProfilSurname.IsReadOnly = state;
            ProfilTelephoneNumber.IsReadOnly = state;
            btnReductProfile.Content = "Сохранить";
            if (state)
                UpdateProfile();
        }

        private void UpdateProfile()
        {
            string connectionString = @"Data Source=DESKTOP-OS1FU9G\SQLEXPRESS;Initial Catalog=AutoProcat;Integrated Security=True";
            string sqlExpression = $"Update Client Set Name = '{ProfilName.Text}', Surname = '{ProfilSurname.Text}', Email = '{ProfilEmail.Text}', TelephoneNumber = '{ProfilTelephoneNumber.Text}' Where IdClient = {client.IdClient} ";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.ExecuteNonQuery();
            }
            clients = GetListClient();
            data.ItemsSource = clients;
            client.Name = ProfilName.Text;
            client.Surname = ProfilSurname.Text;
            client.Email = ProfilEmail.Text;
            client.TelePhoneNumber = ProfilTelephoneNumber.Text;
            MessageBox.Show("Аккаунт успешно обновлен");
            btnReductProfile.Content = "Редактировать";
        }

        private void Marka_add_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Model_add.Items.Clear();
            NewModel_add.Visibility = Visibility.Visible;
            BtnAddInBd.Visibility = Visibility.Visible;
            GetModelInfoCmbBoxAdd();
        }

        private int GetLastModelId()
        {
            int a = 0;
            string connectionString = @"Data Source=DESKTOP-OS1FU9G\SQLEXPRESS;Initial Catalog=AutoProcat;Integrated Security=True";
            string sqlExpression = $"Select IdModel FROM Model";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();


                if (reader.HasRows) // если есть данные //  ошибка
                {
                    // выводим названия столбцов

                    while (reader.Read()) // построчно считываем данные
                    {
                        a = Convert.ToInt32(reader.GetValue(0));
                        //object name = reader.GetValue(0);
                        //Fuel_add.Items.Add(name);
                    }
                }

                reader.Close();
            }

            return a;
        }

        private void BtnAddInBD_Click(object sender, RoutedEventArgs e)
        {
            if (NewModel_add.Text == null || NewModel_add.Text == "")
            {
                MessageBox.Show("Не удалось добавить модель");
            }
            else
            {
                string connectionString = @"Data Source=DESKTOP-OS1FU9G\SQLEXPRESS;Initial Catalog=AutoProcat;Integrated Security=True";
                string sqlExpression = $"INSERT INTO Model (\"Name\") VALUES ('{NewModel_add.Text}')";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.ExecuteNonQuery();
                }

                sqlExpression = $"INSERT INTO Marka (\"Name\", \"IdModel\") VALUES ('{Marka_add.SelectedItem}', '{GetLastModelId()}')";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.ExecuteNonQuery();
                }

                Model_add.Items.Clear();
                NewModel_add.Visibility = Visibility.Hidden;
                NewModel_add.Text = "";
                BtnAddInBd.Visibility = Visibility.Hidden;
                GetModelInfoCmbBoxAdd();
            }
        }
    }
}