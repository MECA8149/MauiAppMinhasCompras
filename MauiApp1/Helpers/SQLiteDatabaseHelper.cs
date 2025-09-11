using MauiAppMinhasCompras.Models;
using SQLite;
using System.Linq;

namespace MauiAppMinhasCompras.Helpers
{
    public class SQLiteDatabaseHelper
    {
        readonly SQLiteAsyncConnection _conn;

        public SQLiteDatabaseHelper(string path)
        {
            _conn = new SQLiteAsyncConnection(path);
            _conn.CreateTableAsync<Produto>().Wait();

            // Adicionar coluna Categoria se não existir
            try
            {
                _conn.ExecuteAsync("ALTER TABLE Produto ADD COLUMN Categoria TEXT DEFAULT 'Geral'").Wait();
            }
            catch
            {
                // Coluna já existe
            }
        }

        public Task<int> Insert(Produto p)
        {
            return _conn.InsertAsync(p);
        }

        public Task<int> Update(Produto p)
        {
            return _conn.UpdateAsync(p);
        }

        public Task<int> Delete(int id)
        {
            return _conn.Table<Produto>().DeleteAsync(i => i.Id == id);
        }

        public Task<List<Produto>> GetAll()
        {
            return _conn.Table<Produto>().ToListAsync();
        }

        public Task<List<Produto>> Search(string q)
        {
            string sql = "SELECT * FROM Produto WHERE descricao LIKE '%" + q + "%' OR Categoria LIKE '%" + q + "%'";
            return _conn.QueryAsync<Produto>(sql);
        }

        // NOVOS MÉTODOS PARA CATEGORIAS
        public async Task<List<Produto>> GetByCategory(string categoria)
        {
            if (categoria == "Todas as Categorias")
                return await GetAll();

            return await _conn.Table<Produto>()
                           .Where(p => p.Categoria == categoria)
                           .ToListAsync();
        }

        public async Task<List<string>> GetAllCategories()
        {
            try
            {
                var produtos = await _conn.Table<Produto>().ToListAsync();
                return produtos
                    .Select(p => p.Categoria)
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
                return new List<string>();
            }
        }

        // MÉTODO QUE ESTAVA FALTANDO - GetTotalByCategory
        public async Task<Dictionary<string, double>> GetTotalByCategory()
        {
            try
            {
                var produtos = await GetAll();

                return produtos
                    .Where(p => p != null)
                    .GroupBy(p => string.IsNullOrEmpty(p.Categoria) ? "Geral" : p.Categoria)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(p => p.Quantidade * p.Preco)
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao calcular totais por categoria: {ex.Message}");
                return new Dictionary<string, double>();
            }
        }
    }
}