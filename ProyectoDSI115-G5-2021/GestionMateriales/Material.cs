﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoDSI115_G5_2021.GestionMateriales
{
    class Material
    {
        public string codigo { get; set; }
        public string nombre { get; set; }
        public string cantidad { get; set; }
        public string unidad { get; set; }
        public string fecha { get; set; }

        public Material(string cod, string nom, string can, string uni, string fecha)
        {
            codigo = cod;
            nombre = nom;
            cantidad = can;
            unidad = uni;
            fecha = fecha;
        }
        public Material()
        {

        }
        public override string ToString()
        {
            return string.Format(" {0} - {1} - {2} - {3} - {4} - {5}", codigo, nombre, cantidad, unidad, fecha);
        }
    }
}