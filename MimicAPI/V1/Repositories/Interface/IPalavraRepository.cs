using MimicAPI.Helpers;
using MimicAPI.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.V1.Repositories.Interface
{
    public interface IPalavraRepository
    {
        ListaDePaginacao<Palavra> ObterTodasPalavras(PalavraUrlQuery query);
        Palavra ObterApenasUmaPalavra(int id);

        void CadastrarPalavras(Palavra palavra);

        void AtualizarPalavra(Palavra palavra);

        void DeletarPalavra(int id);
    }
}
