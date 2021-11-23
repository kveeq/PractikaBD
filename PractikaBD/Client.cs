using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PractikaBD
{
    class Client
    {
        public int IdClient { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string TelePhoneNumber { get; set; }
        public string Role { get; set; }

        public Client(int idClient, string name, string surname, string email, string telephoneNumber, int idRole)
        {
            IdClient = idClient;
            Name = name;
            Surname = surname;
            Email = email;
            TelePhoneNumber = telephoneNumber;
            if (idRole == 1)
                Role = "Администратор";
            else if (idRole == 2)
                Role = "Пользователь";
        }

        public Client()
        {

        }
    }
}
