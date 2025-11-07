namespace WebSuportePim.Models
{
    public class Chamado
    {

        public int Id_Chamado { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }
        public string Prioridade { get; set; }
        public DateTime Data_Abertura { get; set; }
        public DateTime? Data_Resolucao { get; set; }
        public int Id_Usuario { get; set; }
        public int? Id_Tecnico { get; set; }
        public int? Id_Gerente { get; set; }
        public byte[]? Imagem { get; set; }

    }
}
