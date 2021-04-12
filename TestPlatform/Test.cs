using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace TestPlatform
{
    class Test
    {
        public int id { get; set; }

        private string question;

        public string Question
        {
            get { return question; }
            set { question = value; }
        }
        public Test() { }

        public Test(string question)
        { //Констуруктор 
            this.question = question;
        }


    }
}
