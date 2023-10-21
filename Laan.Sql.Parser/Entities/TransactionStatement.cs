using System;
using System.Collections.Generic;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Entities
{
    public enum TransactionDescriptor
    {
        None,
        Tran,
        Transaction,
        Work
    }    

    public abstract class TransactionStatement : CustomStatement
    {
        public TransactionStatement()
        {
            Descriptor = TransactionDescriptor.None;
        }

        public TransactionDescriptor Descriptor { get; set; }
        public string Name { get; set; }
    }

    public class BeginTransactionStatement : TransactionStatement
    {
        public bool Distributed { get; set; }
    }

    public class CommitTransactionStatement : TransactionStatement
    {
    }

    public class RollbackTransactionStatement : TransactionStatement
    {
    }
}
