using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlatform.Entityes
{
    [Table("Questions")]
    class Question
    {
        [Key]
        public int question_id { get; set; }
        
        public int test_id { get; set; }

        private string question_text;
        public string Question_Text
        {
            get { return question_text; }
            set { question_text = value; }
        } 
        public Question() { }
        public Question(string question_text, int test_id)
        {
            this.question_text = question_text;
            this.test_id = test_id;
        }
    }
}
