using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlatform
{
    class User
    {
        public int id { get; set; }

        private string login, password, email, name, surname, group;
        int isAdmin;
        
        public string Login {
            get { return login; }
            set { login = value; } }
        
        public string Password { 
            get { return password; }
            set { password = value; } 
        }
        
        public string Email {
            get { return email; }
            set { email = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Surname
        {
            get { return surname; }
            set { surname = value; }
        }
        public string Group
        {
            get { return group; }
            set { group = value; }
        }
        public int IsAdmin
        {
            get { return isAdmin; }
            set { isAdmin = value; }
        }
        public User() { }

        public User(string login, string password, string email, string name,
                    string surname, string group, int isAdmin) { //Констуруктор 

            this.login = login;
            this.password = password;
            this.email = email;
            this.isAdmin = isAdmin;
            this.name = name;
            this.surname = surname;
            this.group = group;
        }

    }
}
