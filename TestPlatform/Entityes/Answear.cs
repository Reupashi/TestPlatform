using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlatform.Entityes
{
    [Table("Answeares")]
    class Answear
    {
        public int question_id { get; set; }

        [Key]
        public int answear_id { get; set; }

        private string answear_text;
        public string Answear_Text
        {
            get { return answear_text; }
            set { answear_text = value; }
        }
        public Answear() { }
        public Answear(string answear_text)
        {
            this.answear_text = answear_text;
        }
    }
}
