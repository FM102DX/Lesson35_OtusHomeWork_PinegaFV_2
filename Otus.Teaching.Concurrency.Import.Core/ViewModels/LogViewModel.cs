using System.Collections.Generic;
using Otus.Teaching.Concurrency.Import.Core.Entities;

namespace Otus.Teaching.Concurrency.Import.Core.ViewModels
{
    public class LogViewModel
    {
        public IEnumerable<ConsoleToApiMessage> MyMessageList { get; set; }

    }
}