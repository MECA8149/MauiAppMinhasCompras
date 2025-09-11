using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiApp1.Views;

public partial class ListaProduto : ContentPage
{
    ObservableCollection<Produto> lista = new ObservableCollection<Produto>();
    private string currentFilter = "Todas as Categorias";
    private string currentSearch = "";

    public ListaProduto()
    {
        InitializeComponent();
        lst_produtos.ItemsSource = lista;
    }

    protected async override void OnAppearing()
    {
        try
        {
            await LoadCategories();
            await LoadProducts();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async Task LoadCategories()
    {
        try
        {
            var categorias = await App.Db.GetAllCategories();
            categorias.Insert(0, "Todas as Categorias");

            categoriaFilterPicker.ItemsSource = categorias;

            var currentIndex = categorias.IndexOf(currentFilter);
            categoriaFilterPicker.SelectedIndex = currentIndex >= 0 ? currentIndex : 0;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private async Task LoadProducts()
    {
        try
        {
            lista.Clear();
            List<Produto> produtos;

            if (currentFilter != "Todas as Categorias")
            {
                produtos = await App.Db.GetByCategory(currentFilter);
            }
            else
            {
                produtos = await App.Db.GetAll();
            }

            // Aplicar filtro de pesquisa
            if (!string.IsNullOrEmpty(currentSearch))
            {
                produtos = produtos.Where(p =>
                    p.Descricao.Contains(currentSearch, StringComparison.OrdinalIgnoreCase) ||
                    (p.Categoria != null && p.Categoria.Contains(currentSearch, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            foreach (var produto in produtos)
            {
                lista.Add(produto);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private async void CategoriaFilterPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        currentFilter = picker.SelectedItem?.ToString() ?? "Todas as Categorias";
        await LoadProducts();
    }

    // MÉTODO txt_search_TextChanged CORRIGIDO (APENAS UM)
    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            currentSearch = e.NewTextValue;
            await LoadProducts();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            Navigation.PushAsync(new Views.NovoProduto());
        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
        double soma = lista.Sum(i => i.Total);
        string msg = $"O total é {soma:C}";
        DisplayAlert("Total dos Produtos", msg, "OK");
    }

    // NOVO: Botão para relatório de categorias
    private async void ToolbarItem_Relatorio_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RelatorioCategorias());
    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            MenuItem selecionado = sender as MenuItem;
            Produto p = selecionado.BindingContext as Produto;

            bool confirm = await DisplayAlert(
                "Tem Certeza?", $"Remover {p.Descricao}?", "Sim", "Não");

            if (confirm)
            {
                await App.Db.Delete(p.Id);
                lista.Remove(p);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "Ok");
        }
    }

    private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            Produto p = e.SelectedItem as Produto;
            Navigation.PushAsync(new Views.EditarProduto
            {
                BindingContext = p,
            });
        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "Ok");
        }
    }

    private async void lst_produtos_Refreshing(object sender, EventArgs e)
    {
        try
        {
            await LoadProducts();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
        finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }
}