using System;

namespace ClassLibraryForUnitTest
{
    public class BankAccount
    {
        protected string owner;
        protected decimal balance;

        public BankAccount(string o, decimal b)
        {
            owner = o;
            balance = b;
        }
        public BankAccount(string o):this(o, 0.0M)
        {}

        public decimal Balance
        {
            get { return balance; }
        }

        public virtual void Withdraw(decimal amount)
        {
            if (amount > balance)
                throw new ArgumentOutOfRangeException("Not enough balance to withdraw");
            else if(amount < 0)
                throw new ArgumentOutOfRangeException("Not enough balance to withdraw");
            else
                balance -= amount;
        }

        public virtual void Deposit(decimal amount)
        {
            if(amount<0)
                   throw new ArgumentOutOfRangeException(nameof(amount));
            balance += amount;
        }

        public void TransferFundsTo(BankAccount otherAccount, decimal amount)
        {
            if (otherAccount == null)
                throw new ArgumentNullException(nameof(otherAccount));
            Withdraw(amount);
            otherAccount.Deposit(amount);
        }

        public override string ToString()
        {
            return $"{owner}'s account holds {balance}";
        }
    }
}
