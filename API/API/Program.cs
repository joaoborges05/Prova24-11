using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDataContext>();

var app = builder.Build();

app.MapGet("/", () => "COLOQUE O SEU NOME");

//ENDPOINTS DE TAREFA
//GET: http://localhost:5273/api/chamado/listar
app.MapGet("/api/chamado/listar", ([FromServices] AppDataContext ctx) =>
{
    if (ctx.Chamados.Any())
    {
        return Results.Ok(ctx.Chamados.ToList());
    }
    return Results.NotFound("Nenhum chamado encontrada");
});

//POST: http://localhost:5273/api/chamado/cadastrar
app.MapPost("/api/chamado/cadastrar", ([FromServices] AppDataContext ctx, [FromBody] Chamado chamado) =>
{
    ctx.Chamados.Add(chamado);
    ctx.SaveChanges();
    return Results.Created("", chamado);
});

//PATCH: http://localhost:5273/chamado/alterar/{id}
app.MapPatch("/api/chamado/alterar/{id}", ([FromServices] AppDataContext ctx, [FromRoute] string id, [FromBody] Chamado chamado) =>
{
   var chamadoExistente = ctx.Chamados.FirstOrDefault(c => c.ChamadoId == chamado.ChamadoId);

    if (chamadoExistente == null)
    {
        return Results.NotFound("Chamado não encontrado.");
    }

    
    if (chamadoExistente.Status == "Aberto")
    {
        chamadoExistente.Status = "Em atendimento";
    }
    else if (chamadoExistente.Status == "Em atendimento")
    {
        chamadoExistente.Status = "Resolvido";
    }
    else
    {
        return Results.BadRequest("O status não pode ser alterado.");
    }

    ctx.SaveChanges();
    
    return Results.Ok(chamadoExistente);
});


//GET: http://localhost:5273/chamado/naoconcluidas
app.MapGet("/api/chamado/naoresolvidos", ([FromServices] AppDataContext ctx) =>

    {
    var chamadosNaoResolvidos = ctx.Chamados
        .Where(c => c.Status == "Aberto" || c.Status == "Em atendimento")
        .ToList();

    if (chamadosNaoResolvidos.Any())
    {
        return Results.Ok(chamadosNaoResolvidos);
    }

    return Results.NotFound("Nenhum chamado não resolvido encontrado.");
});


//GET: http://localhost:5273/chamado/concluidas
app.MapGet("/api/chamado/resolvidos", ([FromServices] AppDataContext ctx) =>
{ 
    var chamadosResolvidos = ctx.Chamados
        .Where(c => c.Status == "Resolvidos")
        .ToList();

    if (chamadosResolvidos.Any())
    {
        return Results.Ok(chamadosResolvidos);
    }

    return Results.NotFound("Nenhum chamado resolvido encontrado.");
});

app.Run();
