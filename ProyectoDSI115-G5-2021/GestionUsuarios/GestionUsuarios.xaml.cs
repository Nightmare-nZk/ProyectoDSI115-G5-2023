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

namespace ProyectoDSI115_G5_2021.GestionUsuarios
{
    /// <summary>
    /// Lógica de interacción para GestionUsuarios.xaml
    /// </summary>
    public partial class GestionUsuarios : Window
    {
        private DataTable dt = new DataTable();
        private ControlBD control = new ControlBD();
        private bool editando, contrasenaCorrecta, emailCorrecto, open = false, combo1 = false, combo2 = false;
        public GestionUsuarios()
        {
            InitializeComponent();
            CargarTabla();
        }

        private void CargarTabla()
        {
            //TODO: Hacer función de carga de usuarios.
            dt = control.ConsultarUsuarios();
            dataUsuarios.ItemsSource = dt.DefaultView;
        }

        private void LlenarComboTipos()
        {
            //Se recibe un listado de los tipos de usuario.
            List<TipoUsuario> tipos = control.ConsultarTipoUsuario();
            //Se vincula al combobox, define los títulos, valores y la posición inicial
            comboRoles.ItemsSource = tipos;
            comboRoles.DisplayMemberPath = "nombreTipoUsuario";
            comboRoles.SelectedValuePath = "codTipoUsuario";
        }

        private void LlenarComboEmpleados()
        {
            //Igual que LlenarComboTipos, pero aplicado a empleados
            List<EmpleadoItem> empleados = control.ConsultarEmpleadosLista();
            //Vinculando al combobox, definiendo valores y títulos.
            comboEmpleado.ItemsSource = empleados;
            comboEmpleado.DisplayMemberPath = "nombreEmpleado";
            comboEmpleado.SelectedValuePath = "codEmpleado";
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            //Variables para habilitar guardado.
            if (!open)
            {
                contrasenaCorrecta = false;
                emailCorrecto = false;
                // Inicializando formulario para agregar usuario
                HabilitarEdicion(true);
                LlenarComboTipos();
                LlenarComboEmpleados();
                // Booleano para determinar el resultado de guardar
                editando = false;
                var uriSource = new Uri("/Images/regreso.png", UriKind.Relative);
                imgAgregar.Source = new BitmapImage(uriSource);
                open = true;
            }
            else
            {
                HabilitarEdicion(false);
                cuadroEmail.Background = Brushes.White;
                cuadroContrasena.Background = Brushes.White;
                cuadroContrasenaConfirmar.Background = Brushes.White;
                var uriSource = new Uri("/Images/agregar.png", UriKind.Relative);
                imgAgregar.Source = new BitmapImage(uriSource);
                open = false;
            }
        }

        private void BotonGuardar_Click(object sender, RoutedEventArgs e)
        {
            //TO DO: Lógica de guardado de usuarios
            Usuario aGuardar = new Usuario();
            aGuardar.tipoUsuario = new TipoUsuario();
            aGuardar.codigo = cuadroUsuario.Text;
            aGuardar.correoElectronico = cuadroEmail.Text;
            aGuardar.empleado = comboEmpleado.SelectedValue.ToString();
            aGuardar.tipoUsuario.codTipoUsuario = comboRoles.SelectedValue.ToString();
            if (editando)
            {
                //Lógica de edición de registro
            }
            else
            {
                //Lógica de creación de registro
                String mensaje = control.AgregarUsuario(aGuardar, cuadroContrasena.Password.ToString());
                if (mensaje.Equals("Ha ocurrido un error"))
                {
                    MessageBox.Show(mensaje, "Error al registrar", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show(mensaje, "Registro de usuario", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            //Después de guardar, reiniciar los campos de edición
            //Limpia ComboBox para llenar en la siguiente edición
            comboEmpleado.ItemsSource = null;
            comboEmpleado.Items.Clear();
            comboRoles.ItemsSource = null;
            comboRoles.Items.Clear();
            HabilitarEdicion(false);
            cuadroEmail.Background = Brushes.White;
            cuadroContrasena.Background = Brushes.White;
            cuadroContrasenaConfirmar.Background = Brushes.White;
            botonGuardar.SetCurrentValue(IsEnabledProperty, false);
            var uriSource = new Uri("/Images/agregar.png", UriKind.Relative);
            imgAgregar.Source = new BitmapImage(uriSource);
        }

        private void HabilitarEdicion(bool valor)
        {
            //Cuadro de usuario
            cuadroUsuario.SetCurrentValue(IsEnabledProperty, valor);
            cuadroUsuario.Text = "";
            //Cuadro de contraseña y confirmación
            cuadroContrasena.SetCurrentValue(IsEnabledProperty, valor);
            cuadroContrasena.Clear();
            cuadroContrasenaConfirmar.SetCurrentValue(IsEnabledProperty, valor);
            cuadroContrasenaConfirmar.Clear();
            //Cuadro de email
            cuadroEmail.SetCurrentValue(IsEnabledProperty, valor);
            cuadroEmail.Text = "";
            //Lista de empleados
            comboEmpleado.SetCurrentValue(IsEnabledProperty, valor);
            comboRoles.SetCurrentValue(IsEnabledProperty, valor);
            btnBorrar.SetCurrentValue(IsEnabledProperty, !valor);
            btnEditar.SetCurrentValue(IsEnabledProperty, !valor);
        }

        private void CorreoValido(string mail)
        {
            try
            {
                System.Net.Mail.MailAddress mailAddress = new System.Net.Mail.MailAddress(mail);
                cuadroEmail.Background = Brushes.White;
                emailCorrecto = true;
            }
            catch
            {
                cuadroEmail.Background = Brushes.LightPink;
                emailCorrecto = false;
            }
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CuadroEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            CorreoValido(cuadroEmail.Text);
            PermitirGuardado();
        }

        private void PermitirGuardado()
        {
            //
            if (contrasenaCorrecta && emailCorrecto && !comboEmpleado.SelectedItem.Equals(null) && !comboRoles.SelectedItem.Equals(null) && !cuadroUsuario.Text.Equals(""))
            {
                botonGuardar.SetCurrentValue(IsEnabledProperty, true);
            }
            else
            {
                botonGuardar.SetCurrentValue(IsEnabledProperty, false);
            }
        }

        private void CuadroUsuario_TextChanged(object sender, TextChangedEventArgs e)
        {
            PermitirGuardado();
        }

        private void ComboRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            combo2 = !comboEmpleado.IsDropDownOpen;
            PermitirGuardado();
        }

        private void ComboRoles_DropDownClosed(object sender, EventArgs e)
        {
            if (combo2) PermitirGuardado();
            combo2 = true;
        }

        private void ComboEmpleado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            combo1 = !comboEmpleado.IsDropDownOpen;
            PermitirGuardado();
        }

        private void ComboEmpleado_DropDownClosed(object sender, EventArgs e)
        {
            if (combo1) PermitirGuardado();
            combo1 = true;
        }

        private void CuadroContrasenaConfirmar_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string nuevaContra = cuadroContrasena.Password.ToString();
            //Comprobar que la contraseña sea de al menos 6 caracteres y coincidan.
            PruebaContrasena(nuevaContra);
        }

        private void PruebaContrasena(string contra)
        {
            contrasenaCorrecta = false;
            if (contra.Length > 5)
            {
                if (contra.Equals(cuadroContrasenaConfirmar.Password.ToString()))
                {
                    cuadroContrasena.Background = Brushes.White;
                    cuadroContrasenaConfirmar.Background = Brushes.White;
                    contrasenaCorrecta = true;
                }
                else
                {
                    cuadroContrasena.Background = Brushes.LightGoldenrodYellow;
                    cuadroContrasenaConfirmar.Background = Brushes.LightPink;
                }
            }
            else
            {
                cuadroContrasena.Background = Brushes.LightPink;
                cuadroContrasenaConfirmar.Background = Brushes.LightPink;
            }
            PermitirGuardado();
        }

        private void CuadroContrasena_PasswordChanged(object sender, RoutedEventArgs e)
        {
            //Comprobando que la contraseña sea de 6 caracteres
            string nuevaContra = cuadroContrasena.Password.ToString();
            PruebaContrasena(nuevaContra);
        }
    }
}
