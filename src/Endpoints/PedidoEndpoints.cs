//Endpoints do projeto e suas validações e verificações juntos.
using CafeDoBairro.Api.Models;
namespace CafeDoBairro.Api.Endpoints;

public static class PedidoEndpoints
{
    public static void MapPedidoEndpoints(this WebApplication app)
    {
        var pedidos = new List<Pedido>();
        var proximoId = 1;

        //Coleção dos produtos disponíveis em tipo string
        var produtosDisponiveis = new HashSet<string>
        {
            "café coado",
            "pão de queijo",
            "bolo de chocolate",
            "bolo de cenoura",
            "bolo de milho",
            "bolo de aipim"
        };

        app.MapPost("/pedidos", (Pedido novoPedido) =>
        {
            //Verifica se há um campo vazio no nome, telefone, produto ou status.
            if (string.IsNullOrWhiteSpace(novoPedido.NomeCliente) ||
                string.IsNullOrWhiteSpace(novoPedido.Telefone) ||
                string.IsNullOrWhiteSpace(novoPedido.Produto) ||
                string.IsNullOrWhiteSpace(novoPedido.Status))
            {
                return Results.BadRequest("Certifique-se de preencher todos os campos necessários");
            }

            //Verifica se a data informada é uma data que já passou.
            if (novoPedido.DataEntrega.Date < DateTime.Now.Date)
            {
                return Results.BadRequest("A data de entrega não pode ser uma data anterior ao dia atual");
            }

            string statusNormalizado = novoPedido.Status.Trim().ToLower();
            //Remove os caracteres de espaço da string e normaliza para minúsculo, atribuindo à uma nova variável

            if (statusNormalizado != "recebida" && statusNormalizado != "em preparo" && statusNormalizado != "entregue")
            {
                return Results.BadRequest("O status de entrega não corresponde à nenhum dos tipos permitidos");
            }
            novoPedido.Status = char.ToUpper(statusNormalizado[0]) + statusNormalizado.Substring(1);

            string produtoNormalizado = novoPedido.Produto.Trim().ToLower();

            if (!produtosDisponiveis.Contains(produtoNormalizado))
            {
                return Results.BadRequest("O produto informado não está disponível no catálogo");
            }
            novoPedido.Produto = produtoNormalizado;

            if (novoPedido.Telefone.Length != 11 || !novoPedido.Telefone.All(char.IsDigit))
            {
                return Results.BadRequest("O telefone deve conter exatamente 11 dígitos numéricos");
            }

            var quantidadeNoDia = pedidos.Count(p =>
            p.DataEntrega.Date == novoPedido.DataEntrega.Date);
            //Percorre a quantidade de pedidos cadastrados e contabiliza na variável a quantidade de pedidos daquele dia
            //comparando se cada pedido novo também está naquela data ou não.

            if (quantidadeNoDia >= 5)
            {
                return Results.Conflict("O limite de 5 encomendas para este dia já foi atingido");
            }

            novoPedido.Id = proximoId;
            proximoId++;

            pedidos.Add(novoPedido);

            return Results.Created($"/pedidos/{novoPedido.Id}", novoPedido);
        });

        app.MapGet("/pedidos", () =>
        {
            return Results.Ok(pedidos);
        });

        app.MapPut("/pedidos/{id}", (int id, Pedido pedidoAtualizado) =>
        {
            var pedidoExistente = pedidos.FirstOrDefault(p => p.Id == id);

            if (pedidoExistente is null)
            {
                return Results.NotFound("O ID do pedido selecionado não foi encontrado");
            }

            if (string.IsNullOrWhiteSpace(pedidoAtualizado.NomeCliente) ||
                string.IsNullOrWhiteSpace(pedidoAtualizado.Telefone) ||
                string.IsNullOrWhiteSpace(pedidoAtualizado.Produto) ||
                string.IsNullOrWhiteSpace(pedidoAtualizado.Status))
            {
                return Results.BadRequest("Certifique-se de preencher todos os campos necessários");
            }

            if (pedidoAtualizado.DataEntrega.Date < DateTime.Now.Date)
            {
                return Results.BadRequest("A data de entrega não pode ser uma data anterior ao dia atual");
            }

            string statusNormalizado = pedidoAtualizado.Status.Trim().ToLower();
            if (statusNormalizado != "recebida" && statusNormalizado != "em preparo" && statusNormalizado != "entregue")
            {
                return Results.BadRequest("O status de entrega não corresponde à nenhum dos tipos permitidos");
            }
            pedidoAtualizado.Status = char.ToUpper(statusNormalizado[0]) + statusNormalizado.Substring(1);

            string produtoNormalizado = pedidoAtualizado.Produto.Trim().ToLower();
            if (!produtosDisponiveis.Contains(produtoNormalizado))
            {
                return Results.BadRequest("O produto informado não está disponível no catálogo");
            }
            pedidoAtualizado.Produto = produtoNormalizado;
            
            if (pedidoAtualizado.Telefone.Length != 11 || !pedidoAtualizado.Telefone.All(char.IsDigit))
            {
                return Results.BadRequest("O telefone deve conter exatamente 11 dígitos numéricos (DDD + número).");
            }

            var quantidadeNoDia = pedidos.Count(p => 
            p.DataEntrega.Date == pedidoAtualizado.DataEntrega.Date && p.Id != id
            );
            //Percorre a quantidade de pedidos naquele dia e desconsidera o pedido atual, já que ele está sendo atualizado
            //Isso previne que a pessoa atualize a data de um pedido para uma data com a agenda máxima.

            if (quantidadeNoDia >= 5)
            {
                return Results.Conflict("O limite de 5 encomendas para este dia já foi atingido, coloque para outro dia");
            }

            pedidoExistente.NomeCliente = pedidoAtualizado.NomeCliente;
            pedidoExistente.Telefone = pedidoAtualizado.Telefone;
            pedidoExistente.Produto = pedidoAtualizado.Produto;
            pedidoExistente.DataEntrega = pedidoAtualizado.DataEntrega;
            pedidoExistente.Status = pedidoAtualizado.Status;

            return Results.Ok(pedidoExistente);
        });

        app.MapDelete("/pedidos/{id}", (int id) =>
        {
            var pedidoExistente = pedidos.FirstOrDefault(p => p.Id == id);

            if (pedidoExistente is null)
            {
                return Results.NotFound("O ID do pedido selecionado para exclusão não foi encontrado");
            }

            pedidos.Remove(pedidoExistente);

            return Results.Ok();
        });
    }
}