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

        var produtos = MainPage.Produtos.OrderBy(p => p.Validade).ToList();
        produtosListView.ItemsSource = produtos;

        var hoje = DateTime.Today;
        var vencidos = produtos.Where(p => p.Validade.HasValue && p.Validade.Value < hoje).ToList();
        var proximos = produtos.Where(p => p.Validade.HasValue && p.Validade.Value <= hoje.AddDays(3) && p.Validade.Value >= hoje).ToList();

        if (vencidos.Any() || proximos.Any())
        {
            alertaLabel.Text = $"⚠️ Atenção: {vencidos.Count} vencido(s), {proximos.Count} prestes a vencer!!!";
        }
        else
        {
            alertaLabel.Text = string.Empty;
        }

        AtualizarResumo();
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
                            p.Categoria != "Eletrônicos" &&
                            p.Categoria != "Vestuário")
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
    private async void OnItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is Produto produto)
        {
            bool confirmar = await DisplayAlert("Remover Produto",
                $"Deseja remover o produto \"{produto.Nome}\"?", "Sim", "Não");

            if (confirmar)
            {
                MainPage.Produtos.Remove(produto);
                ProdutoStorage.SalvarProdutos(MainPage.Produtos);
                produtosListView.ItemsSource = MainPage.Produtos.OrderBy(p => p.Validade).ToList();
                AtualizarResumo();
                OnAppearing();

                ProdutoStorage.SalvarProdutos(MainPage.Produtos);
            }
        }
    }
    private void AtualizarResumo()
    {

        var produtos = produtosListView.ItemsSource as List<Produto>;
        int quantidade = produtos?.Count ?? 0;
        decimal total = produtos?.Sum(p => p.Preco) ?? 0;

        resumoLabel.Text = $"Total: {quantidade} produto(s) - Valor: R$ {total:F2}";
    }
}