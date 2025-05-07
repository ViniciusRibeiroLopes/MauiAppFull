using System.Globalization;
using System.Text;

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
        string categoriaSelecionadaNormalizada = RemoveDiacritics(categoriaSelecionada.ToLowerInvariant());

        if (categoriaSelecionadaNormalizada == "todas")
        {
            produtosListView.ItemsSource = MainPage.Produtos
                .OrderBy(p => !p.Validade.HasValue)
                .ThenBy(p => p.Validade)
                .ToList();
        }
        else if (categoriaSelecionadaNormalizada == "outros")
        {
            produtosListView.ItemsSource = MainPage.Produtos
                .Where(p =>
                    RemoveDiacritics(p.Categoria.ToLowerInvariant()) != "alimentos" &&
                    RemoveDiacritics(p.Categoria.ToLowerInvariant()) != "eletronicos" &&
                    RemoveDiacritics(p.Categoria.ToLowerInvariant()) != "vestuario")
                .OrderBy(p => !p.Validade.HasValue)
                .ThenBy(p => p.Validade)
                .ToList();
        }
        else
        {
            produtosListView.ItemsSource = MainPage.Produtos
                .Where(p => RemoveDiacritics(p.Categoria.ToLowerInvariant()) == categoriaSelecionadaNormalizada)
                .OrderBy(p => !p.Validade.HasValue)
                .ThenBy(p => p.Validade)
                .ToList();
        }
    }
    public static string RemoveDiacritics(string text)
    {
        return new string(text.Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray());
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