using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using TestPlatform.Entityes;

namespace TestPlatform
{
    [Table("Tests")]
    class Test
    {
        [Key]
        public int test_id { get; set; }

        private string name;

        public string Name 
        {
            get { return name; }
            set { name = value; }
        }

        public Test() { }

        public Test(string name)
        { //Констуруктор 
            this.name = name;
        }


    }
}
