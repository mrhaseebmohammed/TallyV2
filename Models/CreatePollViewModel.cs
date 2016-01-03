using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using boardreview.Entities;

namespace boardreview.Models
{
    public class CreatePollViewModel
    {
        [Required]
        [StringLength(8000, ErrorMessage = "Question cannot be longer than 8000 characters.")]
        [DisplayName("Question")]
        public string Question { get; set; }

        public AnswerText[] Answers { get; set; }

        [StringLength(20, ErrorMessage = "Name cannot be longer than 20 characters.")]
        public string ShortURL {get;set;}
    }

    public class AnswerText
    {
        [StringLength(200, ErrorMessage = "Answer cannot be longer than 200 characters.")]
        [DisplayName("Answer")]
        public string Value { get; set; }
    }
}