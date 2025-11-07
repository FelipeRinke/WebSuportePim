using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.IO;
using System.Collections.Generic;

namespace WebSuportePim.Models
{
    public class ChamadoDAO
    {
        // Copie sua string de conexão do UsuarioDAO.cs
        private string connectionString = "Server=tcp:helpdesk-unip.database.windows.net,1433;Initial Catalog=help-desk-unip;User ID=feliperinke;Password=Unip080403@!;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        // Método para INSERIR um novo chamado
        public bool Inserir(Chamado chamado)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    INSERT INTO Chamado (Titulo, Descricao, Status, Data_Abertura, Id_Usuario, Imagem) 
                    VALUES (@Titulo, @Descricao, @Status, @Data_Abertura, @Id_Usuario, @Imagem)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Estes parâmetros estão OK com AddWithValue
                    cmd.Parameters.AddWithValue("@Titulo", chamado.Titulo);
                    cmd.Parameters.AddWithValue("@Descricao", chamado.Descricao);
                    cmd.Parameters.AddWithValue("@Status", chamado.Status);
                    cmd.Parameters.AddWithValue("@Data_Abertura", chamado.Data_Abertura);
                    cmd.Parameters.AddWithValue("@Id_Usuario", chamado.Id_Usuario);

                    // --- CORREÇÃO AQUI ---
                    // Em vez de AddWithValue, criamos o parâmetro manualmente
                    // para a imagem, especificando o tipo correto (VarBinary).
                    // O '-1' no tamanho significa VARBINARY(MAX).
                    SqlParameter paramImagem = new SqlParameter("@Imagem", SqlDbType.VarBinary, -1);

                    if (chamado.Imagem != null)
                    {
                        paramImagem.Value = chamado.Imagem;
                    }
                    else
                    {
                        paramImagem.Value = DBNull.Value;
                    }

                    // Adicionamos o parâmetro à coleção
                    cmd.Parameters.Add(paramImagem);
                    // --- FIM DA CORREÇÃO ---

                    // Executa o comando
                    int linhasAfetadas = cmd.ExecuteNonQuery();
                    return linhasAfetadas > 0;
                }
            }
        }

        //listar todos os chamados de um usuário específico
        public List<Chamado> ListarPorUsuario(int idUsuario)
        {
            List<Chamado> listaDeChamados = new List<Chamado>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Query SQL que busca os chamados (do mais novo para o mais antigo)
                string query = @"
                    SELECT Id_Chamado, Titulo, Descricao, Status, Prioridade, Data_Abertura, Id_Usuario 
                    FROM Chamado 
                    WHERE Id_Usuario = @Id_Usuario
                    ORDER BY Data_Abertura DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id_Usuario", idUsuario);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Loop para ler todas as linhas que o SELECT retornou
                        while (reader.Read())
                        {
                            // Cria um objeto Chamado para cada linha
                            Chamado chamado = new Chamado
                            {
                                Id_Chamado = Convert.ToInt32(reader["Id_Chamado"]),
                                Titulo = reader["Titulo"].ToString(),
                                Descricao = reader["Descricao"].ToString(),
                                Status = reader["Status"].ToString(),
                                // Verificação de nulo para Prioridade (caso não seja obrigatória)
                                Prioridade = reader["Prioridade"] != DBNull.Value ? reader["Prioridade"].ToString() : "N/A",
                                Data_Abertura = Convert.ToDateTime(reader["Data_Abertura"]),
                                Id_Usuario = Convert.ToInt32(reader["Id_Usuario"])
                            };

                            // Adiciona o chamado na lista
                            listaDeChamados.Add(chamado);
                        }
                    }
                }
            }

            return listaDeChamados; // Retorna a lista (pode estar vazia)
        }

    }
}