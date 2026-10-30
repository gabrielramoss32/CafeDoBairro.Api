using CafeDoBairro.Api.Models;
var builder = WebApplication.CreateBuilder(args);

//Declaração e código das 4 endopoints do projeto.

builder.Services.AddOpenApi();

var app = builder.Build();
var pedidos = new List<Pedido>();
var proximoId = 1;

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

    string statusNormalizado = novoPedido.Status.Trim().ToLower();
    //Remove os caracteres de espaço da string e normaliza para minúsculo, atribuindo à uma nova variável
    //Tal ordem é realizada para comparar se o status recebido é igual a um dos três status permitidos.

    if (statusNormalizado != "recebida" && statusNormalizado != "em preparo" && statusNormalizado != "entregue")
    {
        return Results.BadRequest("O status de entrega não corresponde à nenhum dos tipos permitidos");
    }
    novoPedido.Status = char.ToUpper(statusNormalizado[0]) + statusNormalizado.Substring(1);

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
        return Results.NotFound();
    }

    if (string.IsNullOrWhiteSpace(pedidoAtualizado.NomeCliente) ||
        string.IsNullOrWhiteSpace(pedidoAtualizado.Telefone) ||
        string.IsNullOrWhiteSpace(pedidoAtualizado.Produto) ||
        string.IsNullOrWhiteSpace(pedidoAtualizado.Status))
    {
        return Results.BadRequest("Certifique-se de preencher todos os campos necessários");
    }

    string statusNormalizado = pedidoAtualizado.Status.Trim().ToLower();
    if (statusNormalizado != "recebida" && statusNormalizado != "em preparo" && statusNormalizado != "entregue")
    {
        return Results.BadRequest("O status de entrega não corresponde à nenhum dos tipos permitidos");
    }
    pedidoAtualizado.Status = char.ToUpper(statusNormalizado[0]) + statusNormalizado.Substring(1);

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
        return Results.NotFound();
    }

    pedidos.Remove(pedidoExistente);

    return Results.Ok();
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run(); 