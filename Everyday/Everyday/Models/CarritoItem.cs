using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everyday.Models
{
    public class CarritoItem
    {
        private Producto _producto;
        private int _cantidad;

        public Producto Producto
        {
            get { return _producto; }
            set { _producto = value; }
        }

        public int cantidad
        {
            get { return _cantidad; }
            set { _cantidad = value; }
        }

        public CarritoItem() { }

        public CarritoItem(Producto producto, int cantidad)
        {
            this._producto = producto;
            this._cantidad = cantidad;
        }
    }
}