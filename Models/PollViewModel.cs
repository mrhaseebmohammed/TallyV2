using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using boardreview.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace boardreview.Models
{
    public class PollViewModel
    {
        public Question Question { get; set; }
        public Answer[] Answers { get; set; }
    }
}