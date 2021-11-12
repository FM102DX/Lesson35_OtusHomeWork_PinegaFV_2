using Otus.Teaching.Concurrency.Import.Core.Abstract;
using System;

namespace Otus.Teaching.Concurrency.Import.Core.Entities
{
    public class ConsoleToApiMessage : KeepableClass
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime TimeStamp { get; set; }

        public override string ToString()
        {
            return $"{Text}      ---- {TimeStamp} ";
        }
    }
}