using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pokemons
{
    public partial class Form1 : Form
    {
        private List<Pokemons> listaPokemons;  //Creo el atribulo: --- listaPokemons ---, para capturar todo lo que lea de la DB y despues lo muestro o asigno en la DataGredView...
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PokemonsNegocio negocio = new PokemonsNegocio();
            listaPokemons = negocio.listar();
            dgvPokemons.DataSource = listaPokemons;
            dgvPokemons.Columns["UrlImagen"].Visible = false;  //Oculto la columa --- UrlImagen ---
            cargarImagen(listaPokemons[0].UrlImagen);
        }

        private void dgvPokemons_SelectionChanged(object sender, EventArgs e)
        {
            Pokemons seleccionado = (Pokemons)dgvPokemons.CurrentRow.DataBoundItem;
            cargarImagen(seleccionado.UrlImagen);
        }
        private void cargarImagen(string imagen)   // Creo el metodo cargar imagen y proceso la excetion y cargo una imagen para cuando no pueda cargar la imagen predeterminada...
        {
            try
            {
                pbxPokemons.Load(imagen);
            }
            catch (Exception ex)
            {

                pbxPokemons.Load("https://img.freepik.com/vector-premium/vector-icono-imagen-predeterminado-pagina-imagen-faltante-diseno-sitio-web-o-aplicacion-movil-no-hay-foto-disponible_87543-11093.jpg");
            }
        }
    }
}
