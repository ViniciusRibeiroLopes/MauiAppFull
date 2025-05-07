namespace MauiAppComCadastro;

public partial class ListaProdutosPage : ContentPage
{
	public ListaProdutosPage()
	{
        InitializeComponent();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        produtosListView.ItemsSource = MainPage.Produtos
            .OrderBy(p => !p.Validade.HasValue) // true (sem validade) vai pro final
            .ThenBy(p => p.Validade)
            .ToList();
    }

    private void FiltrarPorCategoria(object sender, EventArgs e)
    {
        string categoriaSelecionada = filtroCategoriaPicker.SelectedItem?.ToString() ?? "Todas";

        if (categoriaSelecionada == "Todas")
        {
            produtosListView.ItemsSource = MainPage.Produtos
                .OrderBy(p => !p.Validade.HasValue)
                .ThenBy(p => p.Validade)
                .ToList();
        }
        else if (categoriaSelecionada == "Outros")
        {
            produtosListView.ItemsSource = MainPage.Produtos
                .Where(p => p.Categoria != "Alimentos" &&
                            p.Categoria != "Eletr�nicos" &&
                            p.Categoria != "Vestu�rio")
                .OrderBy(p => !p.Validade.HasValue)
                .ThenBy(p => p.Validade)
                .ToList();
        }
        else
        {
            produtosListView.ItemsSource = MainPage.Produtos
                .Where(p => p.Categoria == categoriaSelecionada)
                .OrderBy(p => !p.Validade.HasValue)
                .ThenBy(p => p.Validade)
                .ToList();
        }
    }
}