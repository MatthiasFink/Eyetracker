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
    
    public partial class Slide
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Slide()
        {
            this.Slide_Answer = new HashSet<Slide_Answer>();
            this.Slide_Choice = new HashSet<Slide_Choice>();
            this.Tracking = new HashSet<Tracking>();
        }
    
        public int id { get; set; }
        public int test_definition_id { get; set; }
        public int num { get; set; }
        public string title { get; set; }
        public string filepath { get; set; }
        public byte[] image { get; set; }
        public string image_mime { get; set; }
        public Nullable<int> duration { get; set; }
        public bool is_multiple_choice { get; set; }
        public string question { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Slide_Answer> Slide_Answer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Slide_Choice> Slide_Choice { get; set; }
        public virtual Test_Definition Test_Definition { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tracking> Tracking { get; set; }
    }
}
