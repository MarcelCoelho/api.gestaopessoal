using System.Security.Cryptography;
using System.Text;

namespace api.gestaopessoal.Core
{
    public class Hash
    {
        private readonly HashAlgorithm _algoritimo;
        public Hash(HashAlgorithm algoritimo)
        {
            _algoritimo = algoritimo;
        }

        public string Cripitografar(string senha)
        {
            var valorCodigoficado = Encoding.UTF8.GetBytes(senha);
            var senhaEncripitada = _algoritimo.ComputeHash(valorCodigoficado);

            var sb = new StringBuilder();
            foreach (var caracter in senhaEncripitada)
            {
                sb.Append(caracter.ToString("X2"));
            }

            return sb.ToString();
        }

        public bool ValidarSenha(string senha, string senhaBD)
        {
            if (string.IsNullOrEmpty(senha))
                throw new NullReferenceException("Cadastre uma senha.");

            var senhaEncripitada = _algoritimo.ComputeHash(Encoding.UTF8.GetBytes(senha));

            var sb = new StringBuilder();
            foreach (var caractere in senhaEncripitada)
            {
                sb.Append(caractere.ToString("X2"));
            }

            return sb.ToString().Equals(senhaBD);
        }
    }
}
