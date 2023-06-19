using MongoDB.Driver;

namespace api.gestaopessoal.Services.Transacao
{
    using api.gestaopessoal.Models.Transacao;
    using api.gestaopessoal.Models.ConfiguracaoBD;

    using api.gestaopessoal.Services.Fatura;
    using api.gestaopessoal.Services.TipoPagamento;

    using Aspose.Cells;
    using System.Text;
    using MongoDB.Bson.IO;
    using api.gestaopessoal.Models.Fatura;
    using api.gestaopessoal.Core;

    public class TransacaoService : ITransacaoService
    {
        private readonly IMongoCollection<Transacao> _transacaoCollection;
        private readonly IFaturaService _faturaService;
        private readonly ITipoPagamentoService _tipoPagamentoService;
        public TransacaoService(IGestaoPessoalStoreDatabaseSettings settings, IMongoClient mongoClient, IFaturaService faturaService, ITipoPagamentoService tipoPagamentoService)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _transacaoCollection = database.GetCollection<Transacao>(settings.CollectionNameTransacao);

            _faturaService = faturaService;
            _tipoPagamentoService = tipoPagamentoService;
        }

        public List<Transacao> Create(Transacao transacao)
        {
            if (transacao == null)
                throw new Exception("Transação inválida");

            if (string.IsNullOrEmpty(transacao.FaturaId))
                throw new Exception("Fatura inválida");

            var transacoes = new List<Transacao>();

            transacao.DataCriacao = DateTime.Now;
            transacao.DataModificacao = transacao.DataCriacao;

            if (transacao.QuantidadeParcelas == 1)
            {
                transacao.NumeroParcela = 1;
                transacoes.Add(transacao);
            }
            else if (transacao.QuantidadeParcelas > 1)
            {
                var faturas = _faturaService.Get();
                for (int indice = 1; indice <= transacao.QuantidadeParcelas; indice++)
                {
                    if (indice == 1)
                    {
                        transacao.Fatura = faturas.Find(ft => ft.Id == transacao.FaturaId);
                        transacao.NumeroParcela = 1;
                        transacoes.Add(transacao);
                    }
                    else
                    {
                        if (transacao.Fatura == null)
                            throw new Exception("Fatura inválida");

                        var transacaoSerializada = Newtonsoft.Json.JsonConvert.SerializeObject(transacao);
                        var novaTransacao = Newtonsoft.Json.JsonConvert.DeserializeObject<Transacao>(transacaoSerializada);

                        novaTransacao.NumeroParcela = indice;

                        int ordemProximaFatura = transacao.Fatura.Ordem + (indice - 1);

                        novaTransacao.Fatura = faturas.Find(ft => ft.Ordem == ordemProximaFatura);
                        if (novaTransacao.Fatura == null)
                            throw new Exception(string.Format("A Fatura de ordem '{0}' não existe. Cadastre primeiro esta fatura e depois cadastre novamente esta transação.", ordemProximaFatura));

                        novaTransacao.FaturaId = novaTransacao.Fatura.Id;
                        transacoes.Add(novaTransacao);
                    }
                }
            }

            var transacoesRetorno = new List<Transacao>();

            foreach (var transacaoLoop in transacoes)
            {
                _transacaoCollection.InsertOne(transacaoLoop);
                transacoesRetorno.Add(Get(transacaoLoop.Id));
            }

            return transacoesRetorno;
        }

        public List<Transacao> Get()
        {
            var tiposPagamento = _tipoPagamentoService.Get();
            var faturas = _faturaService.Get();

            var transacoes = _transacaoCollection.Find(tp => true).ToList();

            foreach (var transacao in transacoes)
            {
                transacao.Fatura = faturas.FirstOrDefault(f => f.Id == transacao.FaturaId);
                transacao.TipoPagamento = tiposPagamento.FirstOrDefault(t => t.Id == transacao.TipoPagamentoId);
            }

            return transacoes.OrderByDescending(order => order.DataCriacao).ToList();
        }

        public Transacao Get(string id)
        {
            var transacao = _transacaoCollection.Find(tp => tp.Id == id).FirstOrDefault();

            if (transacao is null)
                return null;

            transacao.TipoPagamento = _tipoPagamentoService.Get(transacao.TipoPagamentoId);
            transacao.Fatura = _faturaService.Get(transacao.FaturaId);

            return transacao;
        }

        public List<Transacao> GetByUser(string usuarioId)
        {
            var tiposPagamento = _tipoPagamentoService.Get();
            var faturas = _faturaService.Get();

            var transacoes = _transacaoCollection.Find(tp => tp.UsuarioId.Equals(usuarioId)).ToList();

            foreach (var transacao in transacoes)
            {
                transacao.Fatura = faturas.FirstOrDefault(f => f.Id == transacao.FaturaId);
                transacao.TipoPagamento = tiposPagamento.FirstOrDefault(t => t.Id == transacao.TipoPagamentoId);
            }

            return transacoes.OrderByDescending(order => order.DataCriacao).ToList();
        }

        public void Remove(string id)
        {
            _transacaoCollection.DeleteOne(tp => tp.Id == id);
        }

        public List<Transacao> TransacoesPorFaturaAtual()
        {
            var tiposPagamento = _tipoPagamentoService.Get();
            var faturas = _faturaService.Get();

            var transacoes = _transacaoCollection.Find(tp => true).ToList();

            foreach (var transacao in transacoes)
            {
                transacao.Fatura = faturas.FirstOrDefault(f => f.Id == transacao.FaturaId);
                transacao.TipoPagamento = tiposPagamento.FirstOrDefault(t => t.Id == transacao.TipoPagamentoId);
            }

            transacoes = transacoes.FindAll(tr => tr.Fatura != null && tr.Fatura.Atual).ToList();

            return transacoes.OrderByDescending(order => order.DataCriacao).ToList();
        }

        public List<TotalFatura>? TransacoesPorFaturas(List<string> ids)
        {
            var tiposPagamento = _tipoPagamentoService.Get();
            List<Fatura> faturas = null;

            var transacoes = _transacaoCollection.Find(tp => true).ToList();

            if (ids.Any())
            {
                faturas = _faturaService.Get().FindAll(f => ids.Contains(f.Id));
                transacoes = transacoes.FindAll(tr => ids.Contains(tr.FaturaId)).OrderByDescending(o => o.DataCriacao).ToList();
            }
            else
                //TODO - filtra 5 meses
                faturas = _faturaService.Get();

            foreach (var transacao in transacoes)
            {
                transacao.Fatura = faturas.Find(f => f.Id == transacao.FaturaId);
                transacao.TipoPagamento = tiposPagamento.FirstOrDefault(t => t.Id == transacao.TipoPagamentoId);
                transacao.Editando = false;
            }

            var totalPorFatura = transacoes.OrderBy(o => o.Fatura?.Ordem).ToList().GroupBy(p => p.Fatura?.Id,
                                        (key, transacao) => new TotalFatura
                                        {
                                            Id = key,
                                            Descricao = transacao.FirstOrDefault().Fatura?.Observacao,
                                            DataInicio = transacao.FirstOrDefault().Fatura.DataInicio.Date,
                                            DataFinal = transacao.FirstOrDefault().Fatura.DataFinal.Date,
                                            Atual = transacao.FirstOrDefault().Fatura.Atual,
                                            Fechada = transacao.FirstOrDefault().Fatura.Fechada,
                                            QuantidadeTransacoes = transacao.ToList().Count,
                                            Valor = transacao.ToList().Sum(tr => tr.Valor),
                                            TransacoesVisiveis = false,
                                            Ordem = transacao.FirstOrDefault().Fatura.Ordem,
                                            Transacoes = transacoes.FindAll(tr => tr.FaturaId == key).OrderByDescending(o => o.DataCriacao).ThenBy(ord => ord.Data).ToList()
                                        }).ToList();

            var totalFaturaAtual = totalPorFatura.Find(tf => tf.Atual);
            if (totalFaturaAtual == null) return null;

            var novaListaTotalFatura = new List<TotalFatura>() { totalFaturaAtual };
            foreach (var totalFatura in totalPorFatura.Where(tf => tf.Id != totalFaturaAtual.Id).OrderByDescending(o => o.Ordem))
                novaListaTotalFatura.Add(totalFatura);

            return novaListaTotalFatura;
        }

        public List<TotalFatura> TransacoesPorFaturasNaoSelecionadas(List<string> ids)
        {
            var tiposPagamento = _tipoPagamentoService.Get();
            var faturas = _faturaService.Get().FindAll(f => !ids.Contains(f.Id));

            var transacoes = _transacaoCollection.Find(tp => true).ToList();

            transacoes = transacoes.FindAll(tr => !ids.Contains(tr.FaturaId)).OrderByDescending(o => o.DataCriacao).ToList();

            foreach (var transacao in transacoes)
            {
                transacao.Fatura = faturas.Find(f => f.Id == transacao.FaturaId);
                transacao.TipoPagamento = tiposPagamento.FirstOrDefault(t => t.Id == transacao.TipoPagamentoId);
            }

            var totalPorFatura = transacoes.OrderBy(o => o.Fatura.Ordem).ToList().GroupBy(p => p.Fatura?.Id,
                                        (key, transacao) => new TotalFatura
                                        {
                                            Id = key,
                                            Descricao = transacao.FirstOrDefault().Fatura?.Observacao,
                                            DataInicio = transacao.FirstOrDefault().Fatura.DataInicio.Date,
                                            DataFinal = transacao.FirstOrDefault().Fatura.DataFinal.Date,
                                            Atual = transacao.FirstOrDefault().Fatura.Atual,
                                            Fechada = transacao.FirstOrDefault().Fatura.Fechada,
                                            QuantidadeTransacoes = transacao.ToList().Count,
                                            Valor = transacao.ToList().Sum(tr => tr.Valor),
                                            TransacoesVisiveis = false,
                                            Ordem = transacao.FirstOrDefault().Fatura.Ordem,
                                            ModoInserir = false,
                                            Transacoes = transacoes.FindAll(tr => tr.FaturaId == key).OrderBy(o => o.DataCriacao).ThenBy(ord => ord.Data).ToList()
                                        }).ToList();

            return totalPorFatura;
        }

        public List<TotalFatura> TotalPorFatura()
        {
            var faturas = _faturaService.Get();
            var transacoes = _transacaoCollection.Find(tp => true).ToList();

            foreach (var transacao in transacoes)
                transacao.Fatura = faturas.FirstOrDefault(f => f.Id == transacao.FaturaId);

            var faturaAtual = faturas.Find(f => f.Atual);

            transacoes = transacoes.FindAll(tr => tr.Fatura.Ordem >= faturaAtual.Ordem - 3);

            var totalPorFatura = transacoes.GroupBy(p => p.Fatura?.Id,
                                        (key, transacao) => new TotalFatura
                                        {
                                            Id = key,
                                            Descricao = transacao.FirstOrDefault().Fatura?.Observacao,
                                            Atual = transacao.FirstOrDefault().Fatura.Atual,
                                            Fechada = transacao.FirstOrDefault().Fatura.Fechada,
                                            QuantidadeTransacoes = transacao.ToList().Count,
                                            Valor = transacao.ToList().Sum(tr => tr.Valor)
                                        }).ToList();


            return totalPorFatura;
        }

        public Transacao Update(string id, Transacao transacao)
        {
            _transacaoCollection.ReplaceOne(tp => tp.Id == id, transacao);
            return transacao;
        }

        public void UpdateAll()
        {
            var tiposPagamento = _tipoPagamentoService.Get();
            var faturas = _faturaService.Get();

            var transacoes = _transacaoCollection.Find(tp => tp.UsuarioId.Equals("marcelfillipe@hotmail.com")).ToList();

            foreach (var transacao in transacoes)
            {
                transacao.Fatura = faturas.FirstOrDefault(f => f.Id == transacao.FaturaId);
                transacao.TipoPagamento = tiposPagamento.FirstOrDefault(t => t.Id == transacao.TipoPagamentoId);
                transacao.UsuarioId = "39440678";
                _transacaoCollection.ReplaceOne(tp => tp.Id == transacao.Id, transacao);
            }
        }

        public int GetMaxTransacao()
        {
            var transacoes = _transacaoCollection.Find(tp => true).ToList();

            var registro = _transacaoCollection.Aggregate().SortByDescending(so => so.RegistroId).First();
            if (registro == null)
                return -1;

            return registro.RegistroId;
        }

        public string GetData()
        {
            var workbook = new Workbook(@"c:\\GastosPessoais.xlsx");
            JsonSaveOptions options = new JsonSaveOptions()
            {
                HasHeaderRow = false,
                SortNames = false
            };

            workbook.Save(@"C:\ProgramData\Temp\GastosPessoais.json", options);

            var bytes = File.ReadAllBytes(@"C:\ProgramData\Temp\GastosPessoais.json");

            string jsonStr = Encoding.UTF8.GetString(bytes);

            //var resultado = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Transacao>>(text);
            return jsonStr;
        }

        public List<Transacao> GravarTransacoes(List<Transacao> transacoes)
        {
            foreach (var transacao in transacoes)
            {
                transacao.DataCriacao = DateTime.Now;
                transacao.DataModificacao = transacao.DataCriacao;
                transacao.UsuarioCriacao = "web";
                transacao.UsuarioModificacao = "web";

                _transacaoCollection.InsertOne(transacao);
            }

            return transacoes;
        }
    }
}
