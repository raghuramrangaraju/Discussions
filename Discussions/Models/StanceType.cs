//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Discussions.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class StanceType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StanceType()
        {
            this.Claims = new HashSet<Claim>();
        }
    
        public long StanceTypeId { get; set; }
        public string StanceTypeName { get; set; }
        public Nullable<long> QuestionId { get; set; }
        public string Description { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Claim> Claims { get; set; }
        public virtual Question Question { get; set; }
    }
}
