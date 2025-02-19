//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MYBUSINESS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.ExpenseDetails = new HashSet<ExpenseDetail>();
            this.LoanDetails = new HashSet<LoanDetail>();
            this.MOes = new HashSet<MO>();
            this.PODs = new HashSet<POD>();
            this.ProductDetails = new HashSet<ProductDetail>();
            this.RentDetails = new HashSet<RentDetail>();
            this.ServiceDetails = new HashSet<ServiceDetail>();
            this.SODs = new HashSet<SOD>();
            this.SubItems = new HashSet<SubItem>();
        }
    
        public decimal Id { get; set; }
        public string Name { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public Nullable<decimal> WholeSalePrice { get; set; }
        public Nullable<decimal> Stock { get; set; }
        public Nullable<int> StockDefective { get; set; }
        public Nullable<int> PerPack { get; set; }
        public decimal totalPiece { get; set; }
        public bool Saleable { get; set; }
        public string RackPosition { get; set; }
        public string Image { get; set; }
        public string Remarks { get; set; }
        public string BarCode { get; set; }
        public Nullable<int> ReOrder { get; set; }
        public Nullable<decimal> LocationId { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public bool IsService { get; set; }
        public string ShowIn { get; set; }
        public string Status { get; set; }
        public bool HasSubItems { get; set; }
        public string Category { get; set; }
        public string Cylinder { get; set; }
        public string Model { get; set; }
        public string Dimension { get; set; }
        public string OpenWeight { get; set; }
        public string ClosedWeight { get; set; }
        public string CoolantCapacity { get; set; }
        public string VoltageRating { get; set; }
        public string LubeOilCapacity { get; set; }
        public string Width { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExpenseDetail> ExpenseDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LoanDetail> LoanDetails { get; set; }
        public virtual Location Location { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MO> MOes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<POD> PODs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductDetail> ProductDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RentDetail> RentDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ServiceDetail> ServiceDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SOD> SODs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SubItem> SubItems { get; set; }
    }
}
