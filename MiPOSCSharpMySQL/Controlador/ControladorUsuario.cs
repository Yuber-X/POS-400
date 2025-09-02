using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace MiPOSCSharpMySQL.Controlador
{
    internal class ControladorUsuario
    {
        // Mostrar todos los usuarios en el DataGridView
        public void MostrarUsuarios(DataGridView tabla)
        {
            Configuracion.CConexion con = new Configuracion.CConexion();
            try
            {
                string sql = @"SELECT 
                    u.idUsuario AS 'ID',
                    u.nombreUsuario AS 'Usuario',
                    u.nombre AS 'Nombre',
                    u.apellido AS 'Apellido',
                    u.password AS 'Contraseña',
                    r.idRol AS 'IdRol',
                    r.nombreRol AS 'Rol'
                    FROM usuario u 
                    JOIN rol r ON u.fkRol = r.idRol";
                MySqlDataAdapter da = new MySqlDataAdapter(sql, con.estableceConexion());
                DataTable dt = new DataTable();
                da.Fill(dt);
                tabla.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar usuarios: " + ex.Message);
            }
            finally
            {
                con.CerrarConexion();
            }
        }

        // Insertar un nuevo usuario
        public void AgregarUsuario(string nombre, string apellido, string username, string password, int rolId)
        {
            Configuracion.CConexion con = new Configuracion.CConexion();
            try
            {
                string sql = @"INSERT INTO usuario (nombre, apellido, nombreUsuario, password, fkRol)
                               VALUES (@n, @a, @u, @p, @r)";
                MySqlCommand cmd = new MySqlCommand(sql, con.estableceConexion());
                cmd.Parameters.AddWithValue("@n", nombre);
                cmd.Parameters.AddWithValue("@a", apellido);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);
                cmd.Parameters.AddWithValue("@r", rolId);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Usuario guardado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar usuario: " + ex.Message);
            }
            finally
            {
                con.CerrarConexion();
            }
        }

        // Modificar usuario existente
        public void ModificarUsuario(long id, string nombre, string apellido, string username, string password, int rolId)
        {
            Configuracion.CConexion con = new Configuracion.CConexion();
            try
            {
                string sql = @"UPDATE usuario 
                       SET nombre=@n, apellido=@a, nombreUsuario=@u, password=@p, fkRol=@r
                       WHERE idUsuario=@id";
                MySqlCommand cmd = new MySqlCommand(sql, con.estableceConexion());
                cmd.Parameters.AddWithValue("@n", nombre);
                cmd.Parameters.AddWithValue("@a", apellido);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);
                cmd.Parameters.AddWithValue("@r", rolId);
                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Usuario modificado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar usuario: " + ex.Message);
            }
            finally
            {
                con.CerrarConexion();
            }
        }

        // Eliminar usuario
        public void EliminarUsuario(long id)
        {
            Configuracion.CConexion con = new Configuracion.CConexion();
            try
            {
                string sql = "DELETE FROM usuario WHERE idUsuario=@id";
                MySqlCommand cmd = new MySqlCommand(sql, con.estableceConexion());
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Usuario eliminado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar usuario: " + ex.Message);
            }
            finally
            {
                con.CerrarConexion();
            }
        }

        // Permisos

        public DataTable ListarPermisos()
        {
            var con = new Configuracion.CConexion();
            try
            {
                string sql = "SELECT idPermiso, nombrePermiso, claveForm FROM permiso ORDER BY idPermiso";
                var da = new MySqlDataAdapter(sql, con.estableceConexion());
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            finally { con.CerrarConexion(); }
        }

        public HashSet<int> PermisosDeUsuarioIds(long idUsuario)
        {
            var con = new Configuracion.CConexion();
            try
            {
                string sql = @"SELECT fkPermiso FROM usuario_permiso WHERE fkUsuario=@u";
                var cmd = new MySqlCommand(sql, con.estableceConexion());
                cmd.Parameters.AddWithValue("@u", idUsuario);
                var set = new HashSet<int>();
                using (var rd = cmd.ExecuteReader())
                    while (rd.Read()) set.Add(Convert.ToInt32(rd[0]));
                return set;
            }
            finally { con.CerrarConexion(); }
        }

        public void GuardarPermisos(int idUsuario, List<int> permisos)
        {
            Configuracion.CConexion con = new Configuracion.CConexion();
            try
            {
                var cx = con.estableceConexion();

                // eliminar permisos previos
                var del = new MySqlCommand("DELETE FROM usuario_permiso WHERE fkUsuario=@u", cx);
                del.Parameters.AddWithValue("@u", idUsuario);
                del.ExecuteNonQuery();

                // insertar los nuevos
                foreach (int idPermiso in permisos)
                {
                    var ins = new MySqlCommand("INSERT INTO usuario_permiso (fkUsuario, fkPermiso) VALUES (@u,@p)", cx);
                    ins.Parameters.AddWithValue("@u", idUsuario);
                    ins.Parameters.AddWithValue("@p", idPermiso);
                    ins.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar permisos: " + ex.Message);
            }
            finally
            {
                con.CerrarConexion();
            }
        }

        public void AsignarPermiso(long idUsuario, int idPermiso)
        {
            var con = new Configuracion.CConexion();
            try
            {
                string sql = @"INSERT IGNORE INTO usuario_permiso (fkUsuario, fkPermiso) VALUES (@u, @p)";
                MySqlCommand cmd = new MySqlCommand(sql, con.estableceConexion());
                cmd.Parameters.AddWithValue("@u", idUsuario);
                cmd.Parameters.AddWithValue("@p", idPermiso);
                cmd.ExecuteNonQuery();
            }
            finally { con.CerrarConexion(); }
        }

        public void RevocarPermiso(long idUsuario, int idPermiso)
        {
            var con = new Configuracion.CConexion();
            try
            {
                string sql = @"DELETE FROM usuario_permiso WHERE fkUsuario=@u AND fkPermiso=@p";
                MySqlCommand cmd = new MySqlCommand(sql, con.estableceConexion());
                cmd.Parameters.AddWithValue("@u", idUsuario);
                cmd.Parameters.AddWithValue("@p", idPermiso);
                cmd.ExecuteNonQuery();
            }
            finally { con.CerrarConexion(); }
        }

    }
}
