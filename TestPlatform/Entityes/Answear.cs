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
        private int right = 0;
        public int Right
        {
            get { return right; }
            set { right = value; }
        }

        public int question_id { get; set; }

        [Key]
        public int answeare_id { get; set; }

        private string answeare;
        public string Answeare
        {
            get { return answeare; }
            set { answeare = value; }
        }
        public Answear() { }
        public Answear(string answeare, int question_id, int right)
        {
            this.right = right;
            this.answeare = answeare;
            this.question_id = question_id;
        }
    }
}
