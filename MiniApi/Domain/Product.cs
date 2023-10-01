﻿namespace MiniApi.Domain
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }

        public Product(string name, string? description)
        {
            Id = Guid.NewGuid();
            Description = description;
        }

        public Product Update(string name, string? description)
        {
            Name = name;
            Description = description;

            return this;
        }
    }
}
