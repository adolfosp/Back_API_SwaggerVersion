using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Helpers;
using MimicAPI.V1.Models;
using MimicAPI.V1.Models.DTO;
using MimicAPI.V1.Repositories.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.V1.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    //[ApiVersion("1.0", Deprecated = true)] versao obsoleta
    //[ApiVersion("1.1")] e em cima dos métodos eu coloco [MapToApiVersion("1.1")]
    public class PalavrasController : ControllerBase
    {
        private readonly IPalavraRepository _palavraRepository;
        private readonly IMapper _mapper;
        public PalavrasController(IPalavraRepository repository, IMapper mapper)
        {
            _palavraRepository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtém todas as palavras 
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Retorna uma lista de palavras</returns>
        [ApiExplorerSettings(GroupName = "v1")]
        [HttpGet("", Name = "ObterTodasPalavras")]
        public IActionResult ObterTodasPalavras([FromQuery] PalavraUrlQuery query)
        {
            ListaDePaginacao<Palavra> item = _palavraRepository.ObterTodasPalavras(query);
            if (item.Resultados.Count == 0)
            {
                return NotFound();
            }

            ListaDePaginacao<PalavraDTO> lista = CriarLinksPalavra(query, item);

            return Ok(lista);
        }

        private ListaDePaginacao<PalavraDTO> CriarLinksPalavra(PalavraUrlQuery query, ListaDePaginacao<Palavra> item)
        {
            ListaDePaginacao<PalavraDTO> lista = _mapper.Map<ListaDePaginacao<Palavra>, ListaDePaginacao<PalavraDTO>>(item);

            foreach (var palavra in lista.Resultados)
            {
                palavra.Links = new List<LinkDTO>();
                palavra.Links.Add(new LinkDTO("self", Url.Link("ObterApenasUmaPalavra", new { id = palavra.Id }), "GET"));
            }

            lista.Links.Add(new LinkDTO("self", Url.Link("ObterTodasPalavras", query), "GET"));

            if (item.Paginacao != null)
            {
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(item.Paginacao));
                if (query.NumeroPaginas + 1 <= item.Paginacao.TotalPaginas)
                {
                    PalavraUrlQuery queryString = new PalavraUrlQuery() { NumeroPaginas = query.NumeroPaginas + 1, QuantidadeDeRegistroInformado = query.QuantidadeDeRegistroInformado, Data = query.Data };
                    lista.Links.Add(new LinkDTO("next", Url.Link("ObterTodasPalavras", queryString), "GET"));

                }
                if (query.NumeroPaginas - 1 > 0)
                {
                    var queryString = new PalavraUrlQuery() { NumeroPaginas = query.NumeroPaginas - 1, QuantidadeDeRegistroInformado = query.QuantidadeDeRegistroInformado, Data = query.Data };
                    lista.Links.Add(new LinkDTO("prev", Url.Link("ObterTodasPalavras", queryString), "GET"));

                }
            }

            return lista;
        }
        /// <summary>
        /// Obtém apenas uma palavra 
        /// </summary>
        /// <param name="id">Filtro para achar uma palavra específica</param>
        /// <returns>Listagem de palavra</returns>
        [ApiExplorerSettings(GroupName = "v1")]
        [HttpGet("{id}", Name = "ObterApenasUmaPalavra")]
        public IActionResult ObterApenasUmaPalavra(int id)
        {
            Palavra apenasUmaPalavra = _palavraRepository.ObterApenasUmaPalavra(id);

            if (apenasUmaPalavra == null)
            {
                return NotFound();
            }

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(apenasUmaPalavra);
            palavraDTO.Links.Add(
                new LinkDTO("self", Url.Link("ObterApenasUmaPalavra", new { id = palavraDTO.Id }), "GET")
                );

            palavraDTO.Links.Add(
               new LinkDTO("update", Url.Link("AtualizarPalavra", new { id = palavraDTO.Id }), "PUT")
               );

            palavraDTO.Links.Add(
               new LinkDTO("delete", Url.Link("DeletarPalavra", new { id = palavraDTO.Id }), "DELETE")
               );

            return Ok(palavraDTO);
        }
        /// <summary>
        /// Cadastra uma palavra
        /// </summary>
        /// <param name="palavra">Objeto de palavra</param>
        /// <returns>Retorna a palavra criada</returns>
        [Route("")]
        [ApiExplorerSettings(GroupName = "v1")]
        [HttpPost]
        public IActionResult CadastrarPalavras([FromBody] Palavra palavra)
        {
            if(palavra == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return  UnprocessableEntity(ModelState);
            }
            palavra.DataCriacao = DateTime.Now;
            palavra.Ativo = true;

            _palavraRepository.CadastrarPalavras(palavra);

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);
            palavraDTO.Links.Add(
                new LinkDTO("self", Url.Link("ObterApenasUmaPalavra", new { id = palavraDTO.Id }), "GET")
                );

            return Created($"/api/palavras/{palavra.Id}", palavraDTO);
        }

        /// <summary>
        /// Atualiza uma palavra
        /// </summary>
        /// <param name="id">Id para encontrar a palavra</param>
        /// <param name="palavra">Objeto palavra com as informações para atualizar</param>
        /// <returns></returns>
        [MapToApiVersion("1.1")]
        [ApiExplorerSettings(GroupName = "v1.1")]
        [HttpPut("{id}", Name = "AtualizarPalavra")]
        public IActionResult AtualizarPalavra(int id, [FromBody] Palavra palavra)
        {
         

            Palavra atualizarUnicaPalavra = _palavraRepository.ObterApenasUmaPalavra(id);

            if (atualizarUnicaPalavra == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            if (palavra == null)
            {
                return NotFound();
            }
            palavra.Id = id;
            palavra.Ativo = atualizarUnicaPalavra.Ativo;
            palavra.DataCriacao = atualizarUnicaPalavra.DataCriacao;
            palavra.Atualizado = DateTime.Now;

            _palavraRepository.AtualizarPalavra(palavra);
            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);
            palavraDTO.Links.Add(
                new LinkDTO("self", Url.Link("ObterApenasUmaPalavra", new { id = palavraDTO.Id }), "GET")
                );
            return NoContent();
        }

        /// <summary>
        /// Deleta uma palavra
        /// </summary>
        /// <param name="id">Id para encontrar qual palavra irá ser desativada do banco</param>
        /// <returns></returns>
        //[Route("{id}")]
        [MapToApiVersion("1.1")]
        [ApiExplorerSettings(GroupName = "v1.1")]
        [HttpDelete("{id}", Name = "DeletarPalavra")]
        public IActionResult DeletarPalavra(int id)
        {

            Palavra palavraRetorno = _palavraRepository.ObterApenasUmaPalavra(id);

            if (palavraRetorno == null)
            {
                return NotFound();
            }
            _palavraRepository.DeletarPalavra(id);
            return NoContent();
        }
    }
}
