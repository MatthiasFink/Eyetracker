//------------------------------------------------------------------------------
// <auto-generated>
//     Der Code wurde von einer Vorlage generiert.
//
//     Manuelle Änderungen an dieser Datei führen möglicherweise zu unerwartetem Verhalten der Anwendung.
//     Manuelle Änderungen an dieser Datei werden überschrieben, wenn der Code neu generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Answer
    {
        public int id { get; set; }
        public int test_id { get; set; }
        public int question_id { get; set; }
        public string answer1 { get; set; }
        public Nullable<int> choice_id { get; set; }
        public Nullable<bool> is_correct { get; set; }
        public System.DateTime answered { get; set; }
    
        public virtual Choice Choice { get; set; }
        public virtual Question Question { get; set; }
        public virtual Test Test { get; set; }
    }
}
