using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.V1.Models;
using MimicAPI.V1.Repositories.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MimicAPI.V1.Repositories
{
    public class PalavraRepository : IPalavraRepository
    {
        private readonly MimicContext _banco;
        public PalavraRepository(MimicContext banco)
        {
            _banco = banco;
        }
        public void AtualizarPalavra(Palavra palavra)
        {
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
        }

        public void CadastrarPalavras(Palavra palavra)
        {
            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();
        }

        public void DeletarPalavra(int id)
        {
            Palavra palavraEliminar = ObterApenasUmaPalavra(id);
            palavraEliminar.Ativo = false;
            _banco.Palavras.Update(palavraEliminar);
            _banco.SaveChanges();
        }

        public Palavra ObterApenasUmaPalavra(int id)
        {
            return _banco.Palavras.Where(p => p.Ativo != false).AsNoTracking().FirstOrDefault(a => a.Id == id);
        }

        public ListaDePaginacao<Palavra> ObterTodasPalavras(PalavraUrlQuery query)
        {
            var lista = new ListaDePaginacao<Palavra>();
            var itens = _banco.Palavras.Where(p => p.Ativo != false).AsNoTracking().AsQueryable();


            if (query.Data.HasValue)
            {
                itens = itens.Where(a => a.DataCriacao > query.Data.Value || a.Atualizado > query.Data.Value);
            }

            if (query.NumeroPaginas.HasValue && query.QuantidadeDeRegistroInformado.HasValue)
            {
                int quantidadeTotalDeRegistro = itens.Count();

                itens = itens.Skip((query.NumeroPaginas.Value - 1) * query.QuantidadeDeRegistroInformado.Value).Take(query.QuantidadeDeRegistroInformado.Value);

                Paginacao paginacao = new Paginacao();
                paginacao.NumeroPaginas = query.NumeroPaginas.Value;
                paginacao.RegistrosPorPagina = query.QuantidadeDeRegistroInformado.Value;
                paginacao.TotalRegistros = quantidadeTotalDeRegistro;
                paginacao.TotalPaginas = (int)Math.Ceiling((double)(paginacao.TotalRegistros / query.QuantidadeDeRegistroInformado.Value));

                lista.Paginacao = paginacao;

            }
            lista.Resultados.AddRange(itens.ToList());
            return lista;
        }
    }
}
