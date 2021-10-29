﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProyectoDSI115_G5_2021.CotizacionRecibo
{
    public partial class CrearRecibo : Window
    {
        private GestionUsuarios.Usuario sesion;
        internal GestionUsuarios.Usuario Sesion { get => sesion; set => sesion = value; }
        ControlBD control;
        DataTable dt = new DataTable();
        DataTable dataTable = new DataTable();
        string codigoSolicitud { get; set; }
        List<DetalleRecibo> detalles = new List<DetalleRecibo>();
        
        private float totalTotal = 0;//Variable global para guardar el total de la compra del recibo

        public CrearRecibo()
        {
            InitializeComponent();
            CargarTabla();
            txtFecha.Text = GenerarFecha();
            txtCodigoRecibo.Text = GenerarCodigoRecibo();
        }

        public void CargarTabla()
        {
            control = new ControlBD();
            dt = control.consultarMateriales();
            dt = control.consultarProductosRecibo();
            dataMateriales.ItemsSource = dt.DefaultView;
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSeleccionar_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = dataMateriales.SelectedItem as DataRowView;
            if (row == null)
            {
                MessageBox.Show("Seleccione primero un material", "Seleccione un material", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            }
            else
            {
                //txtCodigo.Text = row.Row.ItemArray[0].ToString();
                txtNombre.Text = row.Row.ItemArray[1].ToString();
                txtPrecio.Text = row.Row.ItemArray[4].ToString();
                txtPresentacion.Text = row.Row.ItemArray[2].ToString();
            }
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            BuscarMaterial();
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                BuscarMaterial();
            }
        }

        private void BuscarMaterial()
        {
            dt.Clear();
            dt = control.BuscarMatYProRecibo(txtBuscar.Text);
            dataMateriales.ItemsSource = dt.DefaultView;
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtCodigoRecibo.Text = "";
            txtCliente.Text = "";
            txtNombre.Text = "";
            txtCantidad.Text = "";
            txtPresentacion.Text = "";
            txtPrecio.Text = "";
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                if (txtCantidad.Text == "")
                {
                    MessageBox.Show("Debe ingresar un valor a la cantidad", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    if (Convert.ToSingle(txtCantidad.Text) == 0.0)
                    {
                        MessageBox.Show("Debe ingresar un valor mayor a cero", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        if (Convert.ToSingle(txtCantidad.Text) < 0)
                        {
                            MessageBox.Show("Debe ingresar un valor mayor a cero", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else
                        {
                            DetalleRecibo detalle = new DetalleRecibo();
                            detalle.cantidad = Convert.ToSingle(txtCantidad.Text);
                            //detalle.codigo = GenerarCodigoS();
                            //detalle.codigoSolicitud = codigoSolicitud;
                            detalle.precio = Convert.ToSingle(txtPrecio.Text);
                            
                            GestionMateriales.Material mate = new GestionMateriales.Material();
                            mate.precio = txtPrecio.Text;
                            mate.nombre = txtNombre.Text;
                            mate.unidad = txtPresentacion.Text;

                            detalle.material = mate;
                            detalle.subtotal = detalle.cantidad * detalle.precio;

                            detalles.Add(detalle);
                            dataSoli.ItemsSource = null;
                            dataSoli.ItemsSource = detalles;

                            txtCantidad.Text = "";
                            txtPrecio.Text = "";
                            txtNombre.Text = "";
                            txtPresentacion.Text = "";

                            totalTotal = totalTotal + detalle.subtotal;
                            txtTotalRecibo.Text = "$ " + Convert.ToString(totalTotal);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Solo se permiten numeros en el campo cantidad", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                txtCantidad.Text = "";
            }
        }

        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            //if (cmbClientes.SelectedValue == null)
            if (txtCliente.Text == "")
            {
                MessageBox.Show("Debe seleccionar un cliente", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (txtCodigoRecibo.Text == "") {
                    MessageBox.Show("Debe agregar el codigo de la solicitud", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    SolicitudRecibo solicitud = new SolicitudRecibo();
                    solicitud.codigo = codigoSolicitud;
                    //solicitud.codigoReq = txtCodigoRecibo.Text;
                    //solicitud.codigoCliente = txtCliente.Text;
                    //solicitud.solicitante = sesion;
                    //solicitud.autorizador = new GestionUsuarios.Usuario();
                    //solicitud.autorizador.codigo = "";
                    //solicitud.autorizador.empleado = "";
                    //solicitud.fechaSolicitud = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
                    //solicitud.estado = "Pendiente";
                    solicitud.setListDetalles(detalles);
                    string respuesta = control.AgregarRecibo(solicitud);
                    MessageBox.Show(respuesta, "Resultado de la solicitud", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCodigoRecibo.Text = "";
                    txtCliente.Text = "";

                    detalles.Clear();
                    dataSoli.ItemsSource = null;
                }
            }
        }
        /*
        public string GenerarCodigoS()
        {
            DateTime fecha = DateTime.Now;
            string anio= fecha.Year.ToString();
            string mes = fecha.Month.ToString();
            string dia = fecha.Day.ToString();
            string hora = fecha.Hour.ToString();
            string min = fecha.Minute.ToString();
            string seg = fecha.Second.ToString();

            return dia + mes + anio + hora + min + seg;
        }
        */
        public string GenerarCodigoRecibo()
        {
            DateTime fecha = DateTime.Now;
            string anio = fecha.Year.ToString();
            string mes = fecha.Month.ToString();
            string dia = fecha.Day.ToString();
            string hora = fecha.Hour.ToString();
            string min = fecha.Minute.ToString();
            string seg = fecha.Second.ToString();

            return anio + mes + dia + hora + min + seg;
        }
        
        public string GenerarFecha()
        {
            DateTime fecha = DateTime.Now;
            string anio = fecha.Year.ToString();
            string mes = fecha.Month.ToString();
            string dia = fecha.Day.ToString();
            string hora = fecha.Hour.ToString();
            string min = fecha.Minute.ToString();
            string seg = fecha.Second.ToString();

            return dia +"/"+ mes + "/" + anio;
        }
        
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("¿Está seguro que desea cancelar? se borrará todos los datos de la solicitud", "Confirmacion",MessageBoxButton.YesNo,MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                txtCantidad.Text = "";
                txtPrecio.Text = "";
                txtNombre.Text = "";
                txtPresentacion.Text = "";
                detalles.Clear();
                dataSoli.ItemsSource = null;
                dataSoli.ItemsSource = detalles;
            }
            else
            {

            }
        }

        private void dgMateriales_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                DataGrid grid = sender as DataGrid;
                if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                {
                    DataRowView row = grid.SelectedItem as DataRowView;
                    // DataRowView row = dataMateriales.SelectedItem as DataRowView;
                   // DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
                    
                    txtNombre.Text = row.Row.ItemArray[1].ToString();
                    txtPrecio.Text = row.Row.ItemArray[4].ToString();
                    txtPresentacion.Text = row.Row.ItemArray[2].ToString();
                }
            }
        }
    }
}
