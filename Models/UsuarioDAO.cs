using Microsoft.Data.SqlClient;


namespace WebSuportePim.Models
{
    public class UsuarioDAO
    {
        private string connectionString = "Server=tcp:helpdesk-unip.database.windows.net,1433;Initial Catalog=help-desk-unip;User ID=feliperinke;Password=Unip080403@!;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public Usuario ValidarLogin(string Email, string Senha)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

               
                string query = "SELECT [Id_Usuario], [Nome], [Departamento], [Email] FROM [Usuario] WHERE [Email] = @Email AND [Senha] = @Senha";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Parameters.AddWithValue("@Senha", Senha);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Retorna o objeto do usuário encontrado
                            return new Usuario
                            {
                                Id = Convert.ToInt32(reader["Id_Usuario"]),
                                Nome = reader["Nome"].ToString(),
                                Email = reader["Email"].ToString(),
                                Departamento = reader["Departamento"].ToString()
                            };
                        }
                    }
                }
            }

            return null; // Login inválido
        }
    }
}