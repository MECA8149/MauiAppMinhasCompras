
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

        public RelatorioCategorias()
        {
            InitializeComponent();
            BindingContext = this;

            // REMOVA ESTA LINHA: categoriasCollection.ItemsSource = Categorias;
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

                // Ordenar por total (decrescente)
                var categoriasOrdenadas = totaisPorCategoria
                    .OrderByDescending(x => x.Value)
                    .ThenBy(x => x.Key); // CORREÇÃO: Key (maiúsculo)

                foreach (var categoria in categoriasOrdenadas)
                {
                    Categorias.Add(categoria); // CORREÇÃO: Add normal
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}