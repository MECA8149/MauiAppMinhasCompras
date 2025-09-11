using MauiAppMinhasCompras.Models;

namespace MauiApp1.Views
{
    public partial class EditarProduto : ContentPage
    {
        private Produto _produto;

        public EditarProduto()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Receber o produto da tela anterior
            _produto = BindingContext as Produto;

            if (_produto != null)
            {
                // Preencher os campos
                txt_descricao.Text = _produto.Descricao;
                txt_quantidade.Text = _produto.Quantidade.ToString();
                txt_preco.Text = _produto.Preco.ToString();

                // Selecionar categoria se existir
                if (!string.IsNullOrEmpty(_produto.Categoria))
                {
                    picker_categoria.SelectedItem = _produto.Categoria;
                }
            }
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Validações (igual ao NovoProduto)
                if (string.IsNullOrWhiteSpace(txt_descricao.Text))
                {
                    await DisplayAlert("Erro", "Digite a descrição", "OK");
                    return;
                }

                if (!double.TryParse(txt_quantidade.Text, out double quantidade) || quantidade <= 0)
                {
                    await DisplayAlert("Erro", "Quantidade inválida", "OK");
                    return;
                }

                if (!double.TryParse(txt_preco.Text, out double preco) || preco <= 0)
                {
                    await DisplayAlert("Erro", "Preço inválido", "OK");
                    return;
                }

                // Atualizar o produto existente (diferente do NovoProduto)
                _produto.Descricao = txt_descricao.Text;
                _produto.Quantidade = quantidade;
                _produto.Preco = preco;
                _produto.Categoria = picker_categoria.SelectedItem?.ToString() ?? "Geral";

                await App.Db.Update(_produto); // UPDATE em vez de INSERT

                await DisplayAlert("Sucesso", "Produto atualizado!", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }
    }
}