namespace CafeDoBairro.Api.Models;

//Classe referente às variáveis que compõem a encomenda, em que seu status inicial é "Recebido".

public class Pedido
{
    public int Id { get; set; } 
    public string NomeCliente  { get; set; } = "";
    public string Telefone { get; set; } = "";
    public string Produto { get; set; } = "";
    public DateTime DataEntrega { get; set; }
    public string Status { get; set; } = "Recebida";
}