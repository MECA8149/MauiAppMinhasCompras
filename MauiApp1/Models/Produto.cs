using SQLite;

namespace MauiAppMinhasCompras.Models
{
    [Table("Produto")]
    public class Produto
    {
        private string _descricao = string.Empty; // VALOR PADRÃO IMPORTANTE

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Descricao
        {
            get => _descricao;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception("Por favor, Preencha a descrição");
                }
                _descricao = value;
            }
        }

        public double Quantidade { get; set; }
        public double Preco { get; set; }

        [Ignore]
        public double Total { get => Quantidade * Preco; }

        public string Categoria { get; set; } = "Geral"; // VALOR PADRÃO
    }
}