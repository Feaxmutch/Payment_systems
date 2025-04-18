﻿using System.Security.Cryptography;
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

        public Order(int id, int amount)
        {
            Id = id;
            Amount = amount;
        }
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
            string allStrings = string.Join(string.Empty, input);
            byte[] inputBytes = Encoding.Default.GetBytes(allStrings);
            byte[] hashBytes = _hashAlgorithm.ComputeHash(inputBytes);

            return Convert.ToHexString(hashBytes);
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

            return $"pay.system1.ru/order?amount={amount}RUB&hash={{{hash}}}";
        }
    }

    public class PaymentSystem2 : IPaymentSystem
    {
        private readonly IHasher _hasher;

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

            return $"order.system2.ru/pay?hash={{{hash}}}";
        }
    }

    public class PaymentSystem3 : IPaymentSystem
    {
        private readonly IHasher _hasher;
        private readonly Random _random;

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

            return $"system3.com/pay?amount={amount}&curency=RUB&hash={{{hash}{secretKey}}}";
        }

        private string GetSecretKey()
        {
            return _random.Next(int.MaxValue).ToString();
        }
    }
}
