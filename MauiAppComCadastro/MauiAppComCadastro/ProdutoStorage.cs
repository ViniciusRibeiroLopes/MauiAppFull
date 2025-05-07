using System.Text.Json;
using MauiAppComCadastro;
using Microsoft.Maui.Storage;

public static class ProdutoStorage
{
    const string ProdutosKey = "ProdutosSalvos";

    public static void SalvarProdutos(List<Produto> produtos)
    {
        string json = JsonSerializer.Serialize(produtos);
        Preferences.Set(ProdutosKey, json);
    }

    public static List<Produto> CarregarProdutos()
    {
        string json = Preferences.Get(ProdutosKey, string.Empty);
        return string.IsNullOrEmpty(json) ? new List<Produto>() :
            JsonSerializer.Deserialize<List<Produto>>(json) ?? new List<Produto>();
    }
}