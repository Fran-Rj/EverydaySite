//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Everyday.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Pago
    {
        public int idPay { get; set; }
        public int idUser { get; set; }
        public int quantity { get; set; }
        public Nullable<System.DateTime> createdAt { get; set; }
    
        public virtual Usuario Usuario { get; set; }
    }
}
