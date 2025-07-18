using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dominio;
using Negocio;

namespace WinForm_AppPoke
{
    public partial class Form1 : Form
    {
        private List<Pokemons> listaPokemons;  //Creo el atribulo: --- listaPokemons ---, para capturar todo lo que lea de la DB y despues lo muestro o asigno en la DataGredView...
        private List<Elemento> listaElemento;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnActualizar.Text = "🔄 Actualizar";
            cargar();
            cboCampo.Items.Add("Número");
            cboCampo.Items.Add("Nombre");
            cboCampo.Items.Add("Descripción");
        }

        private void cargar()
        {
            PokemonsNegocio negocio = new PokemonsNegocio();
            ElementoNegocio elementoNegocio = new ElementoNegocio();

            try
            {
                listaPokemons = negocio.listar();
                dgvPokemons.DataSource = listaPokemons;
                ocultarColumnas();
                cargarImagen(listaPokemons[0].UrlImagen);
                
                listaElemento = elementoNegocio.listar();
                dgvElemento.DataSource = listaElemento;
                dgvElemento.Columns["Id"].Visible = false;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void ocultarColumnas()
        {
            dgvPokemons.Columns["UrlImagen"].Visible = false;  //Oculto la columa --- UrlImagen ---
            dgvPokemons.Columns["Id"].Visible = false;
        }

        private void dgvPokemons_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPokemons.CurrentRow != null)
            {
                Pokemons seleccionado = (Pokemons)dgvPokemons.CurrentRow.DataBoundItem;
                cargarImagen(seleccionado.UrlImagen);
            }
        }
        private void cargarImagen(string imagen)   // Creo el metodo cargar imagen y proceso la excetion y cargo una imagen para cuando no pueda cargar la imagen predeterminada...
        {
            try
            {
                // Liberar imagen anterior
                if (pbxPokemons.Image != null)
                {
                    pbxPokemons.Image.Dispose();
                    pbxPokemons.Image = null;
                }

                // Verifica si es una URL (mayúsculas o minúsculas)
                if (imagen.ToUpper().StartsWith("HTTP"))
                {
                    pbxPokemons.Load(imagen); // Cargar imagen desde Internet
                }
                else if (File.Exists(imagen)) // Si es ruta local válida
                {
                    using (FileStream fs = new FileStream(imagen, FileMode.Open, FileAccess.Read))
                    {
                        pbxPokemons.Image = Image.FromStream(fs);
                    }
                }
                else
                {
                    // Imagen predeterminada si falla
                    pbxPokemons.Load("https://img.freepik.com/vector-premium/vector-icono-imagen-predeterminado-pagina-imagen-faltante-diseno-sitio-web-o-aplicacion-movil-no-hay-foto-disponible_87543-11093.jpg");
                }
            }
            catch
            {
                pbxPokemons.Load("https://img.freepik.com/vector-premium/vector-icono-imagen-predeterminado-pagina-imagen-faltante-diseno-sitio-web-o-aplicacion-movil-no-hay-foto-disponible_87543-11093.jpg");
            }
        }

        private void btnAgragar_Click(object sender, EventArgs e)
        {
            frmAgregarPokemon frmAgregarPokemon = new frmAgregarPokemon();
            frmAgregarPokemon.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            try
            {
                Pokemons seleccionado;
                if (dgvPokemons.CurrentRow != null)
                {
                    seleccionado = (Pokemons)dgvPokemons.CurrentRow.DataBoundItem;

                    frmAgregarPokemon ModificarPokemon = new frmAgregarPokemon(seleccionado);
                    ModificarPokemon.ShowDialog();
                    cargar();
                    
                }
                else
                {
                     MessageBox.Show("Para modificar debe tener seleccionado un Pokémon...");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnEliminarFisico_Click(object sender, EventArgs e)
        {
            eliminar();
        }

        private void btnEliminarLogico_Click(object sender, EventArgs e)
        {
            eliminar(true);
        }
        


        private void eliminar(bool valor = false)
        {
            PokemonsNegocio negocio = new PokemonsNegocio();
            Pokemons seleccionado;
            try
            {
                if (dgvPokemons.CurrentRow != null)
                {
                        DialogResult respuesta = MessageBox.Show("¿Esta seguro el registro?", "Eliminando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (respuesta == DialogResult.Yes)
                    {
                        seleccionado = (Pokemons)dgvPokemons.CurrentRow.DataBoundItem;
                        if (valor) 
                        {
                            negocio.eliminarLogicoPokemon(seleccionado.Id);
                        }
                        else
                        {
                            negocio.eliminarPokemon(seleccionado.Id);
                        }
                        cargar();
                    }
                }
                else
                {
                    MessageBox.Show("Para eliminar debe tener seleccionado un Pokémon...");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            cargar();
        }

        private void txtFiltroRapido_TextChanged(object sender, EventArgs e)
        {
            List<Pokemons> listaFiltroRapido;

            //listaFiltroRapido = listaPokemons.FindAll(x => x.Nombre.ToUpper().Contains(txtFiltroRapido.Text.ToUpper()) || x.Tipo.Descripcion.ToUpper().Contains(txtFiltroRapido.Text.ToUpper()));
            //dgvPokemons.DataSource = listaFiltroRapido;  // Con esto asigno la lista del filtro a la grilla...


            // if (txtFiltroRapido.Text. == "")   filtra vacio
            if (txtFiltroRapido.Text.Length >= 3) // Busca a partir del tercer caracter escrito...
            {
                listaFiltroRapido = listaPokemons.FindAll(x => x.Nombre.ToUpper().Contains(txtFiltroRapido.Text.ToUpper()) || x.Tipo.Descripcion.ToUpper().Contains(txtFiltroRapido.Text.ToUpper()));
            }
            else
            {
                listaFiltroRapido = listaPokemons;
            }
            dgvPokemons.DataSource = null;
            dgvPokemons.DataSource = listaFiltroRapido;
            ocultarColumnas();
        }

        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cboCampo.SelectedItem.ToString();
            if (opcion == "Número")
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Mayor a");
                cboCriterio.Items.Add("Menor a");
                cboCriterio.Items.Add("Igual a");
            }
            else
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Comienza con");
                cboCriterio.Items.Add("Termina con");
                cboCriterio.Items.Add("Contiene");
            }
        }

        private bool validarFiltro()
        {
            if (cboCampo.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione campo para filtrar.");
                return true;

            }


            if (cboCampo.SelectedItem.ToString() == "Número")
            {
                if (string.IsNullOrEmpty(cboCriterio.Text))
                {
                    MessageBox.Show("Debes seleccionar criterio para filtrar...");
                    return true;
                }
                else if (string.IsNullOrEmpty(txtFiltroAvanzado.Text))
                {
                        MessageBox.Show("Debes cargar el filtro para numericos...");
                        return true;
                }
                if (!(soloNumeros(txtFiltroAvanzado.Text)))
                {
                        MessageBox.Show("Debes cargar solo numeros para campo numerico...");
                        return true;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(cboCriterio.Text))
                {
                    MessageBox.Show("Debes seleccionar criterio para filtrar...");
                    return true;
                }
            }

            return false;
        }
        private bool soloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                {
                    return false;
                }
            }
            return true;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (validarFiltro())
                return;

            PokemonsNegocio negocio = new PokemonsNegocio();
            try
            {
                string campo = cboCampo.SelectedItem.ToString();
                string criterio = cboCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text;
                dgvPokemons.DataSource = negocio.filtrar(campo, criterio, filtro);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }
    }
}
