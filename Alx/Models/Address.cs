//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Alx.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Address
    {
        public Address()
        {
            this.Announcements = new HashSet<Announcement>();
        }
    
        public int Id { get; set; }
        public string AddressName { get; set; }
        public Nullable<int> CityId { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }
    
        public virtual ICollection<Announcement> Announcements { get; set; }
    }
}