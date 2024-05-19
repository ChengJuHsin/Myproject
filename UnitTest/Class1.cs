using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryForUnitTest;
using NUnit.Framework;


namespace Test1
{
    [TestFixture]
    public class BankAccountTests
    {
        private BankAccount account;
        private BankAccount otherAccount;

        [SetUp]
        public void Setup()
        {
            // 在每個測試方法執行前初始化 BankAccount 物件
            account = new BankAccount("John", 1000);
            otherAccount = new BankAccount("Alice", 500);
        }

        [Test]
        public void Withdraw_WithValidAmount_DecreasesBalance()
        {
            // Arrange
            decimal initialBalance = account.Balance;
            decimal amount = 100;

            // Act
            account.Withdraw(amount);

            // Assert
            decimal newBalance = account.Balance;           
            Assert.That(newBalance, Is.EqualTo(initialBalance - amount));
        }

        [Test]
        public void Withdraw_WithNegativeAmount_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => account.Withdraw(-100));
        }

        [Test]
        public void Withdraw_WithInsufficientBalance_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => account.Withdraw(2000));
        }

        [Test]
        public void Deposit_WithValidAmount_IncreasesBalance()
        {
            // Arrange
            decimal initialBalance = account.Balance;
            decimal amount = 100;

            // Act
            account.Deposit(amount);

            // Assert
            decimal newBalance = account.Balance;
            Assert.That(newBalance, Is.EqualTo(initialBalance + amount) );
        }

        [Test]
        public void Deposit_WithNegativeAmount_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => account.Deposit(-100));
        }

        [Test]
        public void TransferFundsTo_WithValidAmount_DecreasesSenderBalanceAndIncreasesReceiverBalance()
        {
            // Arrange
            decimal senderInitialBalance = account.Balance;
            decimal receiverInitialBalance = otherAccount.Balance;
            decimal amount = 200;

            // Act
            account.TransferFundsTo(otherAccount, amount);

            // Assert
            decimal senderNewBalance = account.Balance;
            decimal receiverNewBalance = otherAccount.Balance;
            Assert.That(senderNewBalance, Is.EqualTo(senderInitialBalance - amount));
            Assert.That(receiverNewBalance, Is.EqualTo(receiverInitialBalance + amount));

        }

        [Test]
        public void TransferFundsTo_WithNullOtherAccount_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => account.TransferFundsTo(null, 200));
        }

        [Test]
        public void TransferFundsTo_WithInsufficientBalance_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => account.TransferFundsTo(otherAccount, 2000));
        }
    }
}
