using MauiAppMinhasCompras.Models;

namespace MauiApp1.Views
{
    public partial class NovoProduto : ContentPage
    {
       

        public NovoProduto()
        {
            InitializeComponent();
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Validações
                if (string.IsNullOrWhiteSpace(txt_descricao.Text))
                {
                    await DisplayAlert("Erro", "Por favor, digite a descrição do produto", "OK");
                    return;
                }

                if (!double.TryParse(txt_quantidade.Text, out double quantidade) || quantidade <= 0)
                {
                    await DisplayAlert("Erro", "Por favor, digite uma quantidade válida", "OK");
                    return;
                }

                if (!double.TryParse(txt_preco.Text, out double preco) || preco <= 0)
                {
                    await DisplayAlert("Erro", "Por favor, digite um preço válido", "OK");
                    return;
                }

                // Criar novo produto com categoria
                var produto = new Produto
                {
                    Descricao = txt_descricao.Text,
                    Quantidade = quantidade,
                    Preco = preco,
                    Categoria = picker_categoria.SelectedItem?.ToString() ?? "Geral" // Nova categoria
                };

                // Salvar no banco
                await App.Db.Insert(produto);

                await DisplayAlert("Sucesso", "Produto salvo com sucesso!", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao salvar produto: {ex.Message}", "OK");
            }
        }
    }
}