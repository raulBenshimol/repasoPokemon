using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dominio;


namespace Negocio
{
    public class PokemonsNegocio
    {
        public List<Pokemons> listar()
        {
            List<Pokemons> lista = new List<Pokemons>();
            SqlConnection conexion = new SqlConnection();
            SqlCommand comando = new SqlCommand();
            SqlDataReader lector;

            try
            {
                conexion.ConnectionString = "server=.\\SQLEXPRESS; database=POKEDEX_DB; integrated security=true";
                comando.CommandType = System.Data.CommandType.Text;
                comando.CommandText = "Select Numero, Nombre, P.Descripcion , UrlImagen, E.Descripcion Tipo, D.Descripcion Debilidad, P.Id, P.IdTipo, P.IdDebilidad From POKEMONS P, ELEMENTOS E, ELEMENTOS D where E.Id = P.IdTipo and D.Id = P.IdDebilidad and P.Activo = 1";
                comando.Connection = conexion;

                conexion.Open();
                lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    Pokemons aux = new Pokemons();
                    aux.Id = (int)lector["Id"];
                    aux.Numero = lector.GetInt32(0);
                    aux.Nombre = (string)lector["Nombre"];
                    aux.Descripcion = (string)lector["Descripcion"];

                    //if (!(lector.IsDBNull(lector.GetOrdinal("UrlDescripcion"))))
                    //    aux.UrlImagen = (string)lector["UrlImagen"];


                    if (!(lector["UrlImagen"] is DBNull))
                        aux.UrlImagen = (string)lector["UrlImagen"];

                    aux.Tipo = new Elemento();
                    aux.Tipo.Id = (int)lector["IdTipo"];
                    aux.Tipo.Descripcion = (string)lector["Tipo"];
                    aux.Debilidad = new Elemento();
                    aux.Debilidad.Id = (int)lector["IdDebilidad"];
                    aux.Debilidad.Descripcion = (string)lector["Debilidad"];

                    lista.Add(aux);
                }

                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                conexion.Close();
            }
        }
        public void agregarPokemon(Pokemons nuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("insert into POKEMONS (Numero, Nombre, Descripcion, IdTipo, IdDebilidad, UrlImagen, Activo) values " +
                    "(" + nuevo.Numero + ", '" + nuevo.Nombre + "', '" + nuevo.Descripcion + "', @idTipo, @idDebilidad, @urlImagen, 1)");
                datos.setearParametro("@idTipo", nuevo.Tipo.Id);
                datos.setearParametro("@idDebilidad", nuevo.Debilidad.Id);
                datos.setearParametro("@urlImagen", nuevo.UrlImagen);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void modificarPokemon(Pokemons modificar)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("update POKEMONS set Numero = @numero, Nombre = @nombre, Descripcion = @des, UrlImagen = @img, IdTipo = @idTipo, IdDebilidad = @idDebi where Id = @id");
                datos.setearParametro("@numero", modificar.Numero);
                datos.setearParametro("@nombre", modificar.Nombre);
                datos.setearParametro("@des", modificar.Descripcion);
                datos.setearParametro("@img", modificar.UrlImagen);
                datos.setearParametro("@idTipo", modificar.Tipo.Id);
                datos.setearParametro("@idDebi", modificar.Debilidad.Id);
                datos.setearParametro("@id", modificar.Id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally 
            { 
                datos.cerrarConexion(); 
            }
        }

        public void eliminarPokemon(int Id)
        {
            AccesoDatos eliminar = new AccesoDatos();
            try
            {
                eliminar.setearConsulta("delete from POKEMONS where Id = @id");
                eliminar.setearParametro("@id", Id);
                eliminar.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                eliminar.cerrarConexion();
            }
        }

        public void eliminarLogicoPokemon(int Id)
        {
            AccesoDatos eliminarLogico = new AccesoDatos();
            try
            {
                eliminarLogico.setearConsulta("update POKEMONS set Activo = 0 where Id = @id");
                eliminarLogico.setearParametro("id", Id);
                eliminarLogico.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                eliminarLogico.cerrarConexion();
            }
        }

        public List<Pokemons> filtrar(string campo, string criterio, string filtro)
        {
            List<Pokemons> lista = new List<Pokemons>();
            AccesoDatos filtrar = new AccesoDatos();
            try
            {
                string consulta = "Select Numero, Nombre, P.Descripcion , UrlImagen, E.Descripcion Tipo, D.Descripcion Debilidad, P.Id, P.IdTipo, P.IdDebilidad From POKEMONS P, ELEMENTOS E, ELEMENTOS D where E.Id = P.IdTipo and D.Id = P.IdDebilidad and P.Activo = 1 and ";
                if (campo == "Número")
                {
                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += "Numero > " + filtro;
                            break;
                        case "Menor a":
                            consulta += "Numero < " + filtro;
                            break;
                        default:
                            consulta += "Numero = " + filtro;
                            break;
                    }
                }
                else if (campo == "Nombre")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "Nombre like '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += "Nombre like '%" + filtro + "'";
                            break;
                        default:
                            consulta += "Nombre like '%" + filtro + "%'";
                            break;
                    }
                }
                else
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "P.Descripcion like '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += "P.Descripcion like '%" + filtro + "'";
                            break;
                        default:
                            consulta += "P.Descripcion like '%" + filtro + "%'";
                            break;
                    }
                }
                filtrar.setearConsulta(consulta);
                filtrar.ejecutarLectura();
                while (filtrar.Lector.Read())
                {
                    Pokemons aux = new Pokemons();
                    aux.Id = (int)filtrar.Lector["Id"];
                    aux.Numero = filtrar.Lector.GetInt32(0);
                    aux.Nombre = (string)filtrar.Lector["Nombre"];
                    aux.Descripcion = (string)filtrar.Lector["Descripcion"];

                    if (!(filtrar.Lector["UrlImagen"] is DBNull))
                        aux.UrlImagen = (string)filtrar.Lector["UrlImagen"];

                    aux.Tipo = new Elemento();
                    aux.Tipo.Id = (int)filtrar.Lector["IdTipo"];
                    aux.Tipo.Descripcion = (string)filtrar.Lector["Tipo"];
                    aux.Debilidad = new Elemento();
                    aux.Debilidad.Id = (int)filtrar.Lector["IdDebilidad"];
                    aux.Debilidad.Descripcion = (string)filtrar.Lector["Debilidad"];

                    lista.Add(aux);
                }
                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                filtrar.cerrarConexion();
            }

        }
    } 
}