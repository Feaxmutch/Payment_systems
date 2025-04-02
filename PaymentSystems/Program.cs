using System.Security.Cryptography;
using System.Text;

namespace PaymentSystems
{
    class Program
    {
        static void Main(string[] args)
        {
            Random random = new();
            Order order = new(random.Next(0, int.MaxValue), 12000);
            PaymentSystem1 system1 = new(new Hasher(MD5.Create()));
            PaymentSystem2 system2 = new(new Hasher(MD5.Create()));
            PaymentSystem3 system3 = new(new Hasher(SHA1.Create()));

            Console.WriteLine(system1.GetPayingLink(order));
            Console.WriteLine(system2.GetPayingLink(order));
            Console.WriteLine(system3.GetPayingLink(order));
            Console.ReadKey();
        }
    }

    public class Order
    {
        public readonly int Id;
        public readonly int Amount;

        public Order(int id, int amount) => (Id, Amount) = (id, amount);
    }

    public interface IPaymentSystem
    {
        public string GetPayingLink(Order order);
    }

    public interface IHasher
    {
        string GetHash(string[] strings);
    }

    public class Hasher : IHasher
    {
        private readonly HashAlgorithm _hashAlgorithm;

        public Hasher(HashAlgorithm hashAlgorithm)
        {
            _hashAlgorithm = hashAlgorithm;
        }

        public string GetHash(string[] input)
        {
            string allStrings = SumStrings(input);
            byte[] inputBytes = Encoding.Default.GetBytes(allStrings);
            byte[] hashBytes = _hashAlgorithm.ComputeHash(inputBytes);
            string hash = Convert.ToHexString(hashBytes);

            return hash;
        }

        private string SumStrings(string[] strings)
        {
            string allStrings = string.Empty;

            for (int i = 0; i < strings.Length; i++)
            {
                allStrings += strings[i];
            }

            return allStrings;
        }
    }

    public class PaymentSystem1 : IPaymentSystem
    {
        private readonly IHasher _hasher;

        public PaymentSystem1(IHasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher);
            _hasher = hasher;
        }

        public string GetPayingLink(Order order)
        {
            string id = order.Id.ToString();
            string amount = order.Amount.ToString();
            string hash = _hasher.GetHash([id]);
            string link = $"pay.system1.ru/order?amount={amount}RUB&hash={{{hash}}}";

            return link;
        }
    }

    public class PaymentSystem2 : IPaymentSystem
    {
        IHasher _hasher;

        public PaymentSystem2(IHasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher);
            _hasher = hasher;
        }

        public string GetPayingLink(Order order)
        {
            string id = order.Id.ToString();
            string amount = order.Amount.ToString();
            string hash = _hasher.GetHash([id, amount]);
            string link = $"order.system2.ru/pay?hash={{{hash}}}";

            return link;
        }
    }

    public class PaymentSystem3 : IPaymentSystem
    {
        IHasher _hasher;
        Random _random;

        public PaymentSystem3(IHasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher);
            _hasher = hasher;
            _random = new();
        }

        public string GetPayingLink(Order order)
        {
            string secretKey = GetSecretKey();
            string id = order.Id.ToString();
            string amount = order.Amount.ToString();
            string hash = _hasher.GetHash([id, amount]);
            string link = $"system3.com/pay?amount={amount}&curency=RUB&hash={{{hash}{secretKey}}}";

            return link;
        }

        private string GetSecretKey()
        {
            return _random.Next(int.MaxValue).ToString();
        }
    }
}
