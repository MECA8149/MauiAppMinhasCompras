using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MauiApp1.Views
{
    public partial class RelatorioCategorias : ContentPage, INotifyPropertyChanged
    {
        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public ObservableCollection<KeyValuePair<string, double>> Categorias { get; } = new();
        public Command LoadRelatorioCommand { get; }

        public RelatorioCategorias()
        {
            InitializeComponent();
            BindingContext = this;

            LoadRelatorioCommand = new Command(async () => await LoadRelatorio());
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
                IsRefreshing = true;
                Categorias.Clear();

                var totaisPorCategoria = await App.Db.GetTotalByCategory();

                if (totaisPorCategoria == null || totaisPorCategoria.Count == 0)
                {
                    await DisplayAlert("Info", "Nenhum dado encontrado para exibir o relatório.", "OK");
                    return;
                }

                // Ordenar por total (decrescente)
                var categoriasOrdenadas = totaisPorCategoria
                    .OrderByDescending(x => x.Value)
                    .ThenBy(x => x.Key);

                foreach (var categoria in categoriasOrdenadas)
                {
                    Categorias.Add(categoria);
                }

                // Adicionar total geral
                double totalGeral = totaisPorCategoria.Sum(x => x.Value);
                Categorias.Add(new KeyValuePair<string, double>("TOTAL GERAL", totalGeral));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao carregar relatório: {ex.Message}", "OK");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        // MÉTODO QUE ESTAVA FALTANDO - adicione esta parte
        private async void ToolbarItem_Fechar_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}