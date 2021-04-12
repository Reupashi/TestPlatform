﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlatform
{
    class User
    {
        public int id { get; set; }

        private string login, password, email;
        
        public string Login {
            get { return login; }
            set { login = value; } }
        
        public string Password { 
            get { return password; }
            set { password = value; } }
        
        public string Email {
            get { return email; }
            set { email = value; }
        }
        public User() { }

        public User(string login, string password, string email) { //Констуруктор 
            this.login = login;
            this.password = password;
            this.email = email;
        }

    }
}
