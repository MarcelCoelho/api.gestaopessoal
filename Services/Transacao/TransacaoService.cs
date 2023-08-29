using MongoDB.Driver;

namespace api.gestaopessoal.Services.Transacao
{
    using api.gestaopessoal.Models.Categoria;
    using api.gestaopessoal.Models.ConfiguracaoBD;
    using api.gestaopessoal.Models.Configuration;
    using api.gestaopessoal.Models.Email;
    using api.gestaopessoal.Models.Fatura;
    using api.gestaopessoal.Models.TipoPagamento;
    using api.gestaopessoal.Models.Transacao;
    using api.gestaopessoal.Models.Usuario;
    using api.gestaopessoal.Services.Categoria;
    using api.gestaopessoal.Services.Email;
    using api.gestaopessoal.Services.Fatura;
    using api.gestaopessoal.Services.TipoPagamento;
    using api.gestaopessoal.Services.Usuario;
    using Aspose.Cells;
    using Microsoft.AspNetCore.DataProtection.KeyManagement;
    using System.Collections.Generic;
    using System.Security.AccessControl;
    using System.Security.Cryptography;
    using System.Text;

    public class TransacaoService : ITransacaoService
    {
        private readonly IMongoCollection<Transacao> _transacaoCollection;
        private readonly ICategoriaService _categoriaService;
        private readonly IFaturaService _faturaService;
        private readonly ITipoPagamentoService _tipoPagamentoService;
        private readonly IUsuarioService _usuarioService;
        private readonly IEmailService _emailService;

        public TransacaoService(IGestaoPessoalStoreDatabaseSettings settings,
                            IMongoClient mongoClient,
                            ICategoriaService categoriaService,
                            IFaturaService faturaService,
                            ITipoPagamentoService tipoPagamentoService,
                            IUsuarioService usuarioService,
                            IEmailService emailService)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _transacaoCollection = database.GetCollection<Transacao>(settings.CollectionNameTransacao);

            _categoriaService = categoriaService;
            _faturaService = faturaService;
            _tipoPagamentoService = tipoPagamentoService;
            _usuarioService = usuarioService;
            _emailService = emailService;
        }

        public List<Transacao>? Create(Transacao transacao)
        {
            try
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
                    var faturaCartao = _faturaService.GetById(transacao.FaturaId);
                    var faturas = _faturaService.GetByCartao(faturaCartao.CartaoId);

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

                            if (novaTransacao == null)
                                throw new Exception("Transação nao pode ser deserializada!");

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

                EmailSend(transacoes[0], EmailType.NovaTransacao);
                return transacoesRetorno;
            }
            catch (Exception ex)
            {
                _emailService.Send(new EmailTransaction
                {
                    Title = EmailType.Excecao,
                    Observation = string.Format("Erro ao criar uma nova transação: {0}", ex.ToString())
                });
            }

            return null;
        }

        public List<Transacao> Get()
        {
            var categorias = _categoriaService.Get();
            var tiposPagamento = _tipoPagamentoService.Get();
            var faturas = _faturaService.Get();

            var transacoes = _transacaoCollection.Find(tp => true).ToList();

            foreach (var transacao in transacoes)
            {
                transacao.Categoria = categorias.FirstOrDefault(f => f.Id == transacao.CategoriaId);
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

            transacao.Categoria = _categoriaService.GetById(transacao.CategoriaId);
            transacao.TipoPagamento = _tipoPagamentoService.GetById(transacao.TipoPagamentoId);
            transacao.Fatura = _faturaService.GetById(transacao.FaturaId);

            return transacao;
        }

        public List<Transacao> GetByUser(string usuarioId)
        {
            var categorias = _categoriaService.Get();
            var tiposPagamento = _tipoPagamentoService.Get();
            var faturas = _faturaService.Get();

            var transacoes = _transacaoCollection.Find(tp => tp.UsuarioId.Equals(usuarioId)).ToList();

            foreach (var transacao in transacoes)
            {
                transacao.Categoria = categorias.FirstOrDefault(f => f.Id == transacao.CategoriaId);
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
            var categorias = _categoriaService.Get();
            var tiposPagamento = _tipoPagamentoService.Get();
            var fatura = _faturaService.GetFaturaAtual();

            if (fatura == null)
                throw new Exception("Fatura Atual não encontrada");

            var transacoes = _transacaoCollection.Find(tp => true).ToList();

            var transacoesPorFaturaAtual = new List<Transacao>();

            foreach (var transacao in transacoes.Where(tr => tr.FaturaId == fatura.Id).OrderByDescending(order => order.DataCriacao).ToList())
            {
                transacao.Categoria = categorias.FirstOrDefault(f => f.Id == transacao.CategoriaId);
                transacao.Fatura = fatura;
                transacao.TipoPagamento = tiposPagamento.FirstOrDefault(t => t.Id == transacao.TipoPagamentoId);

                transacoesPorFaturaAtual.Add(transacao);
            }
            //transacoes = transacoes.FindAll(tr => tr.Fatura != null && tr.Fatura.Atual).ToList();
            //return transacoes.OrderByDescending(order => order.DataCriacao).ToList();
            return transacoesPorFaturaAtual;
        }

        public List<TotalFatura> TransacoesPorFaturas(List<string> usuarios)
        {
            if (usuarios == null || usuarios.Count == 0)
                throw new Exception("Parâmetro usuários não informado!");

            var usuarioLogado = _usuarioService.Get().Find(user => usuarios.Contains(user.Email));

            if (usuarioLogado == null)
                throw new Exception(string.Format("Usuário: {0} não encontrado", usuarios[0]));

            //TODO - filtra 5 meses
            List<Fatura>? faturas = _faturaService.Get();
            var tiposPagamento = _tipoPagamentoService.Get();
            var categorias = _categoriaService.Get();

            var transacoes = _transacaoCollection.Find(tp => (tp.UsuarioId == usuarioLogado.Email ||
                                                      (!string.IsNullOrEmpty(usuarioLogado.Dependentes) &&
                                                        usuarioLogado.Dependentes == tp.UsuarioId)) || (tp.UsuarioId != usuarioLogado.Email && tp.UsuarioId != usuarioLogado.Dependentes)).ToList();

            //var transacoes = _transacaoCollection.Find(tc=> true).ToList();

            if (transacoes == null || transacoes.Count == 0)
                throw new Exception(string.Format("Ainda não existem transações cadastradas para o usuário: {0}", usuarioLogado.Login));

            foreach (var transacao in transacoes)
            {
                transacao.Categoria = categorias.FirstOrDefault(f => f.Id == transacao.CategoriaId);
                transacao.Fatura = faturas.Find(f => f.Id == transacao.FaturaId);
                transacao.TipoPagamento = tiposPagamento.FirstOrDefault(t => t.Id == transacao.TipoPagamentoId);
            }

            var totalPorFatura = transacoes.OrderBy(o => o.Fatura?.Ordem).ToList().GroupBy(p => p.Fatura?.Id,
                                        (key, transacao) => new TotalFatura
                                        {
                                            Id = key,
                                            Ano = transacao.FirstOrDefault().Fatura?.Ano,
                                            Mes = transacao.FirstOrDefault().Fatura?.Mes,
                                            Descricao = transacao.FirstOrDefault().Fatura?.Observacao,
                                            DataInicio = transacao.FirstOrDefault().Fatura.DataInicio.Date,
                                            DataFinal = transacao.FirstOrDefault().Fatura.DataFinal.Date,
                                            Atual = transacao.FirstOrDefault().Fatura.Atual,
                                            Fechada = transacao.FirstOrDefault().Fatura.Fechada,
                                            QuantidadeTransacoes = transacao.ToList().Count,
                                            Valor = transacao.ToList().Sum(tr => tr.Valor),
                                            Ordem = transacao.FirstOrDefault().Fatura.Ordem,
                                            Transacoes = transacoes.FindAll(tr => tr.FaturaId == key).OrderByDescending(o => o.DataCriacao).ThenBy(ord => ord.Data).ToList()
                                        }).ToList();

            var totalFaturaAtual = totalPorFatura.Find(tf => tf.Atual);
            if (totalFaturaAtual == null)
                throw new Exception("Fatura Atual não encontrada!");

            var novaListaTotalFatura = new List<TotalFatura>() { totalFaturaAtual };
            foreach (var totalFatura in totalPorFatura.Where(tf => tf.Id != totalFaturaAtual.Id).OrderByDescending(o => o.Ordem))
                novaListaTotalFatura.Add(totalFatura);

            return novaListaTotalFatura;
        }

        public List<TotalFatura> TransacoesPorFaturasNaoSelecionadas(List<string> ids)
        {
            var categorias = _categoriaService.Get();
            var tiposPagamento = _tipoPagamentoService.Get();
            var faturas = _faturaService.Get().FindAll(f => !ids.Contains(f.Id));

            var transacoes = _transacaoCollection.Find(tp => true).ToList();

            transacoes = transacoes.FindAll(tr => !ids.Contains(tr.FaturaId)).OrderByDescending(o => o.DataCriacao).ToList();

            foreach (var transacao in transacoes)
            {
                transacao.Categoria = categorias.FirstOrDefault(f => f.Id == transacao.CategoriaId);
                transacao.Fatura = faturas.Find(f => f.Id == transacao.FaturaId);
                transacao.TipoPagamento = tiposPagamento.FirstOrDefault(t => t.Id == transacao.TipoPagamentoId);
            }

            var totalPorFatura = transacoes.OrderBy(o => o.Fatura?.Ordem).ToList().GroupBy(p => p.Fatura?.Id,
                                        (key, transacao) => new TotalFatura
                                        {
                                            Id = key,
                                            Descricao = transacao.FirstOrDefault().Fatura.Observacao,
                                            DataInicio = transacao.FirstOrDefault().Fatura.DataInicio.Date,
                                            DataFinal = transacao.FirstOrDefault().Fatura.DataFinal.Date,
                                            Atual = transacao.FirstOrDefault().Fatura.Atual,
                                            Fechada = transacao.FirstOrDefault().Fatura.Fechada,
                                            QuantidadeTransacoes = transacao.ToList().Count,
                                            Valor = transacao.ToList().Sum(tr => tr.Valor),
                                            Ordem = transacao.FirstOrDefault().Fatura.Ordem,
                                            Transacoes = transacoes.FindAll(tr => tr.FaturaId == key).OrderBy(o => o.DataCriacao).ThenBy(ord => ord.Data).ToList()
                                        }).ToList();

            return totalPorFatura;
        }

        public List<TotalFatura> TotalPorFatura(List<string> usuarios)
        {

            var usuarioLogado = _usuarioService.Get().Find(user => usuarios.Contains(user.Email));

            if (usuarioLogado == null)
                throw new Exception("Usuário não encontrado!");

            var transacoes = _transacaoCollection.Find(tp => tp.UsuarioId == usuarioLogado.Email ||
                                                      (!string.IsNullOrEmpty(usuarioLogado.Dependentes) &&
                                                        usuarioLogado.Dependentes == tp.UsuarioId)).ToList();

            if (transacoes == null || transacoes.Count == 0)
                throw new Exception(string.Format("Transações não encontradas para o usuario: {0}", usuarios[0]));

            var categorias = _categoriaService.Get();
            var faturas = _faturaService.Get();

            foreach (var transacao in transacoes)
            {
                transacao.Categoria = categorias.FirstOrDefault(f => f.Id == transacao.CategoriaId);
                transacao.Fatura = faturas.FirstOrDefault(f => f.Id == transacao.FaturaId);
            }

            var totalPorFatura = transacoes.GroupBy(p => p.Fatura?.Id,
                                        (key, transacao) => new TotalFatura
                                        {
                                            Id = key,
                                            Descricao = transacao.FirstOrDefault().Fatura?.Observacao,
                                            Ano = transacao.FirstOrDefault().Fatura?.Ano,
                                            Mes = transacao.FirstOrDefault().Fatura?.Mes,
                                            Atual = transacao.FirstOrDefault().Fatura.Atual,
                                            Fechada = transacao.FirstOrDefault().Fatura.Fechada,
                                            QuantidadeTransacoes = transacao.ToList().Count,
                                            Valor = transacao.ToList().Sum(tr => tr.Valor)
                                        }).ToList();


            return totalPorFatura;
        }

        public List<UsuarioFatura>? TotalFaturaPorUsuarios(string ano, List<string> usuariosFiltro)
        {
            var totalFaturas = TransacoesPorFaturas(usuariosFiltro);
            if (totalFaturas == null)
                return null;

            var usuarioLogado = _usuarioService.Get().Find(user => usuariosFiltro.Contains(user.Email));

            if (usuarioLogado == null)
                throw new Exception("Usuário não encontrado!");

            var usuarioDependente = _usuarioService.Get().Find(user => !string.IsNullOrEmpty(usuarioLogado.Dependentes) && user.Email == usuarioLogado.Dependentes);

            var usuariosFaturas = new List<UsuarioFatura>();
            foreach (var totalFatura in totalFaturas.Where(tf => (tf.Ano == ano) && tf.Transacoes != null && tf.Transacoes.Any()))
            {
                var grupoUsuario = totalFatura.Transacoes.Where(trs => usuarioLogado.Email == trs.UsuarioId || (usuarioDependente != null && usuarioDependente.Email == trs.UsuarioId)).GroupBy(tr => tr.UsuarioId,
                                        (key, transacao) => new TotalFatura
                                        {
                                            Id = key,
                                            Ano = transacao.FirstOrDefault().Fatura?.Ano,
                                            Mes = transacao.FirstOrDefault().Fatura?.Mes,
                                            Descricao = transacao.FirstOrDefault().Fatura?.Observacao,
                                            DataInicio = transacao.FirstOrDefault().Fatura.DataInicio.Date,
                                            DataFinal = transacao.FirstOrDefault().Fatura.DataFinal.Date,
                                            Atual = transacao.FirstOrDefault().Fatura.Atual,
                                            Fechada = transacao.FirstOrDefault().Fatura.Fechada,
                                            QuantidadeTransacoes = transacao.ToList().Count,
                                            Valor = transacao.ToList().Sum(tr => tr.Valor),
                                            Ordem = transacao.FirstOrDefault().Fatura.Ordem,
                                            Transacoes = totalFatura.Transacoes.FindAll(tr => tr.UsuarioId == key).OrderByDescending(o => o.DataCriacao).ThenBy(ord => ord.Data).ToList()
                                        }).ToList();

                if (grupoUsuario?.Count > 0)
                {
                    foreach (var gu in grupoUsuario.Where(g => !string.IsNullOrEmpty(g.Id)))
                    {
                        var uf = usuariosFaturas.Find(uft => uft.Usuario?.Email == gu.Id);
                        if (uf != null)
                            uf.TotalFaturas?.Add(gu);
                        else
                        {
                            uf = new UsuarioFatura
                            {
                                Usuario = usuarioLogado.Email == gu.Id ? usuarioLogado : usuarioDependente,
                                TotalFaturas = new List<TotalFatura> { gu },
                            };
                            usuariosFaturas.Add(uf);
                        }
                    }
                }

            }

            return usuariosFaturas;
        }

        public Transacao Update(string id, Transacao transacao)
        {
            _transacaoCollection.ReplaceOne(tp => tp.Id == id, transacao);
            EmailSend(transacao, EmailType.AtualizaTransacao);
            return transacao;
        }

        public void UpdateAll(string obs1, string obs2)
        {
            var categorias = _categoriaService.Get();
            var tiposPagamento = _tipoPagamentoService.Get();
            var faturas = _faturaService.Get();

            var transacoes = _transacaoCollection.Find(tp => tp.Observacao == obs1).ToList();
            //var transacoes = _transacaoCollection.Find(tp => string.IsNullOrEmpty(tp.UsuarioId)).ToList();

            foreach (var transacao in transacoes)
            {
                transacao.Observacao = obs2;
                transacao.Fatura = faturas.FirstOrDefault(f => f.Id == transacao.FaturaId);
                transacao.TipoPagamento = tiposPagamento.FirstOrDefault(t => t.Id == transacao.TipoPagamentoId);
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

        private void EmailSend(Transacao transaction, EmailType emailType)
        {
            var tipoPagamento = _tipoPagamentoService.GetById(transaction.TipoPagamentoId);
            var fatura = _faturaService.GetById(transaction.FaturaId);

            var validationMessage = tipoPagamento == null ? "Tipo Pagamento: " + transaction.TipoPagamentoId + " não encontrado!" : "";

            if (string.IsNullOrEmpty(validationMessage))
                validationMessage = fatura == null ? "Fatura: " + transaction.FaturaId + " não encontrada" : "";

            var emailTransaction = new EmailTransaction
            {
                Title = !string.IsNullOrEmpty(validationMessage) ? EmailType.Validacao : emailType,
                Owner = transaction.UsuarioId,
                Date = transaction.Data,
                Type = tipoPagamento != null ? tipoPagamento.Descricao : "",
                Bill = fatura != null ? fatura.Observacao : "",
                InstallmentsCount = transaction.QuantidadeParcelas,
                Product = transaction.Produto,
                Amount = transaction.Valor,
                Observation = validationMessage
            };

            _emailService.Send(emailTransaction);
        }

        private Fatura? getFatura(List<Fatura> faturas, string faturaId)
        {
            return faturas?.FirstOrDefault(ft => ft.Id == faturaId);
        }

        public List<TotalCategoria>? TotalTransacoesPorCategoria(string cartaoId, List<string> anos, List<string> identificadoresFatura, string usuario, bool verDependentes)
        {

            List<Transacao> transacoes;

            var faturas = _faturaService.GetByCartao(cartaoId);

            transacoes = FiltrarTransacoesPorUsuario(verDependentes, usuario);

            transacoes = FiltrarTransacoesPorFaturaCartao(transacoes, faturas);

            transacoes = FiltrarTransacoesPorFatura(transacoes, faturas, identificadoresFatura, anos);

            var categorias = _categoriaService.Get();
            var tipoPagamentos = _tipoPagamentoService.Get();

            AgregarCategoriaNasTransacoes(ref transacoes, faturas, categorias, tipoPagamentos);
            var totalPorCategoria = AgruparTransacoesPorCategoria(transacoes);

            return totalPorCategoria;
        }

        private static List<TotalCategoria>? AgruparTransacoesPorCategoria(List<Transacao> transacoes)
        {
            var totalPorCategoria = transacoes.GroupBy(p => p.Categoria,
                                                    (key, transacao) => new TotalCategoria
                                                    {
                                                        Categoria = key ?? new Categoria(),
                                                        QuantidadeTransacoes = transacao.ToList().Count,
                                                        ValorTotal = transacao.ToList().Sum(tr => tr.Valor),
                                                        //Transacoes = transacoes.FindAll(tr => (tr.Categoria != null && key != null &&
                                                        //                                      tr.CategoriaId == key.Id) || tr.Categoria == null).OrderByDescending(orden=> orden.Fatura.Ordem).ThenByDescending(o => o.DataCriacao).ThenBy(ord => ord.Data).ToList()

                                                        TotalFatura = transacao.OrderByDescending(o => o.Fatura?.Ordem).ToList().GroupBy(f => f.Fatura?.Id,
                                                                                                       (key2, transacaoFatura) => new TotalFatura
                                                                                                       {
                                                                                                           Id = key2,
                                                                                                           Ano = transacaoFatura.FirstOrDefault().Fatura?.Ano,
                                                                                                           Mes = transacaoFatura.FirstOrDefault().Fatura?.Mes,
                                                                                                           Descricao = transacaoFatura.FirstOrDefault().Fatura?.Observacao,
                                                                                                           DataInicio = transacaoFatura.FirstOrDefault().Fatura.DataInicio.Date,
                                                                                                           DataFinal = transacaoFatura.FirstOrDefault().Fatura.DataFinal.Date,
                                                                                                           Atual = transacaoFatura.FirstOrDefault().Fatura.Atual,
                                                                                                           Fechada = transacaoFatura.FirstOrDefault().Fatura.Fechada,
                                                                                                           QuantidadeTransacoes = transacaoFatura.ToList().Count,
                                                                                                           Valor = transacaoFatura.ToList().Sum(tr => tr.Valor),
                                                                                                           Ordem = transacaoFatura.FirstOrDefault().Fatura.Ordem,
                                                                                                           Transacoes = transacao.Where(tr => tr.FaturaId == key2).OrderByDescending(o => o.DataCriacao).ThenBy(ord => ord.Data).ToList()
                                                                                                       }).ToList()
                                                    }).ToList();

            if (totalPorCategoria?.Count > 0)
            {
                totalPorCategoria = totalPorCategoria.OrderByDescending(tpc => tpc.ValorTotal).ToList();
                return totalPorCategoria;
            }
            return null;
        }

        private static void AgregarCategoriaNasTransacoes(ref List<Transacao> transacoes, List<Fatura> faturas, List<Categoria> categorias, List<TipoPagamento> tipoPagamentos)
        {
            foreach (var transacao in transacoes)
            {
                Categoria? categoria = null;

                if (string.IsNullOrEmpty(transacao.CategoriaId) || categorias.Find(ct => ct.Id == transacao.CategoriaId).Codigo == "semcategoria")
                {
                    if (!string.IsNullOrEmpty(transacao.Observacao))
                    {
                        categoria = categorias.Find(ct => ct.Codigo == transacao.Observacao.Trim().ToLower());
                        if (categoria == null)
                        {
                            var categoriaId = Guid.NewGuid().ToString();
                            categoria = new Categoria { Id = categoriaId, Codigo = transacao.Observacao.Trim().ToLower(), Descricao = transacao.Observacao };
                            categorias.Add(categoria);
                        }
                    }
                    else
                    {
                        categoria = categorias.Find(ct => ct.Id == transacao.TipoPagamentoId);
                        if (categoria == null)
                        {
                            var tipoPagamento = tipoPagamentos.Find(tp => tp.Id == transacao.TipoPagamentoId);
                            categoria = new Categoria
                            {
                                Id = transacao.TipoPagamentoId,
                                Codigo = tipoPagamento.Codigo,
                                Descricao = tipoPagamento.Descricao
                            };
                            categorias.Add(categoria);
                            //categoria = categoria ?? categorias.Find(ct => ct.Codigo == "semcategoria");
                        }
                    }

                    if (categoria != null)
                    {
                        transacao.Categoria = categoria;
                        transacao.CategoriaId = categoria.Id;
                    }
                }

                transacao.Categoria = categorias.FirstOrDefault(f => f.Id == transacao.CategoriaId);
                transacao.Fatura = faturas.FirstOrDefault(f => f.Id == transacao.FaturaId);
            }
        }

        private List<Transacao> FiltrarTransacoesPorUsuario(bool verDependentes, string usuario)
        {
            var usuarioLogado = _usuarioService.Get().Find(user => usuario.Equals(user.Email));

            if (usuarioLogado == null)
                throw new Exception("Usuário não encontrado!");

            List<Transacao> transacoesFiltradas;
            if (verDependentes)
                transacoesFiltradas = _transacaoCollection.Find(tp => tp.UsuarioId == usuarioLogado.Email ||
                                                      (!string.IsNullOrEmpty(usuarioLogado.Dependentes) &&
                                                        usuarioLogado.Dependentes == tp.UsuarioId)).ToList();
            else
                transacoesFiltradas = _transacaoCollection.Find(tp => tp.UsuarioId == usuarioLogado.Email).ToList();

            if (transacoesFiltradas == null || transacoesFiltradas.Count == 0)
                throw new Exception(string.Format("Transações não encontradas para o usuario: {0}", usuario));

            return transacoesFiltradas;
        }

        private List<Transacao> FiltrarTransacoesPorFaturaCartao(List<Transacao> transacoes, List<Fatura> faturas)
        {
            var transacoesFiltradas = new List<Transacao>();
            foreach (var transacao in transacoes)
            {
                if (faturas.Exists(e => e.Id == transacao.FaturaId))
                {
                    transacoesFiltradas.Add(transacao);
                }
            }

            if (transacoesFiltradas == null || transacoesFiltradas.Count == 0)
                throw new Exception("Transações não encontradas para o cartão selecionado.");

            return transacoesFiltradas;
        }

        private List<Transacao> FiltrarTransacoesPorFatura(List<Transacao> transacoes, List<Fatura> faturas, List<string> identificadoresFatura, List<string> anos)
        {
            List<Transacao> transacoesFiltradas = null;

            if (identificadoresFatura?.Count > 0)
                transacoesFiltradas = transacoes.FindAll(tr => identificadoresFatura.Contains(tr.FaturaId)).ToList();
            else if (anos?.Count > 0)
                transacoesFiltradas = transacoes.FindAll(tr => anos.Contains(getFatura(faturas, tr.FaturaId).Ano)).ToList();


            if (transacoesFiltradas == null || transacoesFiltradas.Count == 0)
                throw new Exception("Transações não encontradas para os anos ou faturas selecionados.");

            return transacoesFiltradas;
        }

    }
}
