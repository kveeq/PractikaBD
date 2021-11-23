using Microsoft.Data.SqlClient;
using System.Windows.Controls;

namespace PractikaBD
{
    class Car
    {
        public int IdCar { get; set; }
        public int IdFuel { get; set; }
        public int IdKpp { get; set; }
        public int IdDriveUnit { get; set; }
        public int IdModel { get; set; }
        public int IdMarka { get; set; }
        public int IdCountry { get; set; }
        public string Fuel { get; set; }
        public string Kpp { get; set; }
        public string DriveUnit { get; set; }
        public string Model { get; set; }
        public string Marka { get; set; }
        public string Country { get; set; }
        public string Color { get; set; }
        public int NumberOfSeats { get; set; }
        public int Consumption { get; set; }
        public int HorsePower { get; set; }
        public double Price { get; set; }
        public int EngineVolume { get; set; }


        public void GetFuel(ComboBox FuelCmb, int IdFuel)
        {
            FuelCmb.SelectedIndex = IdFuel - 1;
            Fuel = FuelCmb.SelectedItem.ToString();
        }

        public void GetKpp(ComboBox FuelCmb, int IdFuel)
        {
            FuelCmb.SelectedIndex = IdFuel - 1;
            Kpp = FuelCmb.SelectedItem.ToString();
        }

        public void GetDriveUnitl(ComboBox FuelCmb, int IdFuel)
        {
            FuelCmb.SelectedIndex = IdFuel - 1;
            DriveUnit = FuelCmb.SelectedItem.ToString();
        }

        public void GetModel(ComboBox FuelCmb, int IdFuel)
        {
            FuelCmb.SelectedIndex = IdFuel - 1;
            Model = FuelCmb.SelectedItem.ToString();
        }

        public void GetModel()
        {
            string connectionString = @"Data Source=DESKTOP-OS1FU9G\SQLEXPRESS;Initial Catalog=AutoProcat;Integrated Security=True";
            string sqlExpression = $"Select* FROM Car c JOIN Model m ON c.IdModel = m.IdModel Where IdCar = {IdCar}";
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
                        //MessageBox.Show(reader.GetValue(0).ToString
                        Model = reader.GetValue(15).ToString();
                    }
                }
                reader.Close();
            }
        }

        public void GetMarka(ComboBox FuelCmb, int IdFuel)
        {
            FuelCmb.SelectedIndex = IdFuel - 1;
            Marka = FuelCmb.SelectedItem.ToString();
        }

        public void GetCountry(ComboBox FuelCmb, int IdFuel)
        {
            FuelCmb.SelectedIndex = IdFuel - 1;
            Country = FuelCmb.SelectedItem.ToString();
        }

    }
}