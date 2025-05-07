using static MauiAppComCadastro.MainPage;

namespace MauiAppComCadastro
{
    public partial class MainPage : ContentPage
    {
        public static List<Produto> Produtos { get; set; } = ProdutoStorage.CarregarProdutos();

        public MainPage()
        {
            InitializeComponent();
        }

        private void TemValidadeSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            validadeStack.IsVisible = e.Value;
        }

        private void AdicionarProduto_Clicked(object sender, EventArgs e)
        {
            string nome = nomeEntry.Text;
            string precoTexto = precoEntry.Text;
            string descricao = descricaoEntry.Text;
            string categoria = categoriaEntry.Text;
            DateTime? validade = temValidadeSwitch.IsToggled ? validadePicker.Date : null;

            if (!string.IsNullOrWhiteSpace(nome) &&
                !string.IsNullOrWhiteSpace(precoTexto) &&
                decimal.TryParse(precoTexto, out decimal preco))
            {
                Produtos.Add(new Produto
                {
                    Nome = nome,
                    Preco = preco,
                    Descricao = descricao,
                    Categoria = categoria,
                    Validade = validade,
                    CaminhoImagem = caminhoImagemSelecionada
                });

                mensagemLabel.Text = $"Produto '{nome}' adicionado com sucesso!";

                nomeEntry.Text = string.Empty;
                precoEntry.Text = string.Empty;
                descricaoEntry.Text = string.Empty;
                categoriaEntry.Text = string.Empty;
                validadePicker.Date = DateTime.Today;
                temValidadeSwitch.IsToggled = false;
                previewImagem.Source = null;
                caminhoImagemSelecionada = null;

                ProdutoStorage.SalvarProdutos(Produtos);
            }
            else
            {
                mensagemLabel.Text = "Preencha os campos obrigatórios corretamente.";
            }
        }

        private async void IrParaLista_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListaProdutosPage());
        }

        private string caminhoImagemSelecionada;

        private async void SelecionarImagem_Clicked(object sender, EventArgs e)
        {
            var resultado = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Selecione uma imagem",
                FileTypes = FilePickerFileType.Images
            });

            if (resultado != null)
            {
                caminhoImagemSelecionada = resultado.FullPath;
                previewImagem.Source = ImageSource.FromFile(caminhoImagemSelecionada);
            }
        }
    }
    public class Produto
    {
        public bool EstaVencido => Validade.HasValue && Validade.Value < DateTime.Today;
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public DateTime? Validade { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string? CaminhoImagem { get; set; }

        public string ValidadeFormatada => Validade?.ToString("dd/MM/yyyy") ?? "Sem validade";
        public string PrecoFormatado => $"R$ {Preco:F2}";
    }
}