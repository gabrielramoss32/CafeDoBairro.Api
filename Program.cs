using CafeDoBairro.Api.Models;
var builder = WebApplication.CreateBuilder(args);

//Declaração e código das 4 endopoints do projeto.

builder.Services.AddOpenApi();

var app = builder.Build();
var pedidos = new List<Pedido>();
var proximoId = 1;


app.MapPost("/pedidos", (Pedido novoPedido) =>
{
    var quantidadeNoDia = pedidos.Count(p =>
    p.DataEntrega.Date == novoPedido.DataEntrega.Date);
    //Percorre a quantidade de pedidos cadastrados e contabiliza na variável a quantidade de pedidos daquele dia, comparando se cada pedido novo também está naquela data ou não.

    if (quantidadeNoDia >= 5)
    //Verifica se no dia x, já existe a quantidade máxima de 5 encomendas.
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