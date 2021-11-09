using Otus.Teaching.Concurrency.Import.Core.Abstract;

namespace Otus.Teaching.Concurrency.Import.Core.Entities
{
    public class Customer: KeepableClass, IKeepable
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public override string ToString()
        {
            return $"{Id} {FullName} {Email} {Phone}";
        }
    }
}