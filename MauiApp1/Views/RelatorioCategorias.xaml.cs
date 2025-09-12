using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiApp1.Views
{
    public partial class RelatorioCategorias : ContentPage
    {
        public ObservableCollection<KeyValuePair<string, double>> Categorias { get; } = new();

        public RelatorioCategorias()
        {
            InitializeComponent();
            categoriasCollection.ItemsSource = Categorias; // Vinculação direta
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadRelatorio();
        }

        private async Task LoadRelatorio()
        {
            try
            {
                Categorias.Clear();
                var totaisPorCategoria = await App.Db.GetTotalByCategory();

                if (totaisPorCategoria == null || totaisPorCategoria.Count == 0)
                {
                    await DisplayAlert("Info", "Nenhum dado encontrado.", "OK");
                    return;
                }

                foreach (var categoria in totaisPorCategoria.OrderByDescending(x => x.Value))
                {
                    Categorias.Add(categoria);
                }

                double totalGeral = totaisPorCategoria.Sum(x => x.Value);
                Categorias.Add(new KeyValuePair<string, double>("TOTAL GERAL", totalGeral));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        private async void ToolbarItem_Fechar_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}