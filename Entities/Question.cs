//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace boardreview.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Question
    {
        public int QuestionID { get; set; }
        public string ShortURL { get; set; }
        public string QuestionText { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public string Password { get; set; }
        public Nullable<int> CreatedByUserID { get; set; }
        public bool Active { get; set; }
        public Nullable<System.DateTime> ModifiedDateTime { get; set; }
    }
}