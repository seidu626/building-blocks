        namespace BuildingBlocks.Types;

        // Attribute to map Excel headers to properties
        [AttributeUsage(AttributeTargets.Property)]
        public class ExcelHeaderAttribute : Attribute
        {
            public string Name { get; }
            public ExcelHeaderAttribute(string name) => Name = name;
        }