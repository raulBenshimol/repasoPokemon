using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dominio;
using Negocio;
using System.Configuration;

namespace WinForm_AppPoke
{
    public partial class frmAgregarPokemon : Form
    {
        private Pokemons pokemon = null;  
        private OpenFileDialog archivo = null;
        public frmAgregarPokemon()
        {
            InitializeComponent();
            Text = "Agregar Pokemon";
        }

        public frmAgregarPokemon(Pokemons pokemon)
        {
            this.pokemon = pokemon;
            InitializeComponent();
            Text = "Modificar Pokemon";
                
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            
            PokemonsNegocio negocio = new PokemonsNegocio();
            try
            {
                if (pokemon == null)
                    pokemon = new Pokemons();

                pokemon.Numero = int.Parse(txtNumero.Text);
                pokemon.Nombre = txtNombre.Text;
                pokemon.Descripcion = txtDescripcion.Text;
                pokemon.Tipo = (Elemento)cbxTipo.SelectedItem;
                pokemon.Debilidad = (Elemento)cbxDebilidad.SelectedItem;
                pokemon.UrlImagen = txtUrlImagen.Text;

                if (pokemon.Id != 0)
                {
                    guardarImagen();
                    negocio.modificarPokemon(pokemon);
                    MessageBox.Show("Modificado exitosamente");

                }
                else
                {
                    guardarImagen();
                    negocio.agregarPokemon(pokemon);
                    MessageBox.Show("Un pokemon fue agregado con exito...");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                Close();
            }
        }

        private void guardarImagen()
        {
            if (archivo != null && !(txtUrlImagen.Text.ToUpper().Contains("HTTP")))
            {
                string carpetaDestino = ConfigurationManager.AppSettings["images-PokeApp"];
                if (!Directory.Exists(carpetaDestino))
                    Directory.CreateDirectory(carpetaDestino);

                string rutaNueva = Path.Combine(carpetaDestino, archivo.SafeFileName);

                // 🔴 LIBERAR la imagen cargada en el PictureBox antes de borrar la anterior
                if (pbxPokemons.Image != null)
                {
                    pbxPokemons.Image.Dispose(); // Libera la imagen del sistema
                    pbxPokemons.Image = null;    // Limpia el control
                }

                // 🔴 Eliminar imagen anterior (si existía)
                if (!string.IsNullOrEmpty(txtUrlImagen.Text) && File.Exists(txtUrlImagen.Text))
                {
                    try
                    {
                        File.Delete(txtUrlImagen.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("No se pudo borrar la imagen anterior: " + ex.Message);
                    }
                }

                // ✅ Copiar nueva imagen
                File.Copy(archivo.FileName, rutaNueva, true);

                // ✅ Mostrar y guardar ruta
                pbxPokemons.Image = Image.FromFile(rutaNueva); // Vuelve a cargar la imagen
                txtUrlImagen.Text = rutaNueva;
                pokemon.UrlImagen = rutaNueva;
            }
        }

        private void cargarImagen(string imagen)   // Creo el metodo cargar imagen y proceso la excetion y cargo una imagen para cuando no pueda cargar la imagen predeterminada...
        {
            try
            {
                pbxPokemons.Load(imagen);
            }
            catch (Exception)
            {

                pbxPokemons.Load("https://img.freepik.com/vector-premium/vector-icono-imagen-predeterminado-pagina-imagen-faltante-diseno-sitio-web-o-aplicacion-movil-no-hay-foto-disponible_87543-11093.jpg");
            }
        }

        private void frmAgregarPokemon_Load(object sender, EventArgs e)
        {
            ElementoNegocio elementoNegocio = new ElementoNegocio();
            try
            {
                cbxTipo.DataSource = elementoNegocio.listar();
                cbxTipo.ValueMember = "Id";
                cbxTipo.DisplayMember = "Descripcion";
                cbxDebilidad.DataSource = elementoNegocio.listar();
                cbxDebilidad.ValueMember = "Id";
                cbxDebilidad.DisplayMember = "Descripcion";

                if (pokemon != null)
                {
                    txtNumero.Text = pokemon.Numero.ToString();
                    txtNombre.Text = pokemon.Nombre;
                    txtDescripcion.Text = pokemon.Descripcion;
                    txtUrlImagen.Text = pokemon.UrlImagen;
                    cbxTipo.SelectedValue = pokemon.Tipo.Id;
                    cbxDebilidad.SelectedValue = pokemon.Debilidad.Id;
                    cargarImagen(pokemon.UrlImagen);     
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void txtUrlImagen_Leave(object sender, EventArgs e)
        {
                cargarImagen(txtUrlImagen.Text);      
        }

        private void btnImgLocal_Click(object sender, EventArgs e)
        {
            archivo = new OpenFileDialog();
            archivo.Filter = "jpg|*.jpg;|png|*.png";
            if (archivo.ShowDialog() == DialogResult.OK)
            {


                //File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-PokeApp"] + archivo.SafeFileName);


                //  txtUrlImagen.Text = archivo.FileName; // Esto asigna en el txt la direccion de conde estoy copiadodo la imagen 
                //                                            mas no asigna la direccion de donde queda guarda la imagen...
     
                cargarImagen(archivo.FileName);
            }
        }
    }
}
