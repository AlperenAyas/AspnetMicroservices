using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ICatalogContext _context;

        public ProductRepository(ICatalogContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Creates the new Product
        /// </summary>
        /// <param name="product">Represents Product Entity</param>
        /// <returns></returns>
        public async Task CreateProduct(Product product)
        {
            await _context.Products.InsertOneAsync(product);
        }
        /// <summary>
        /// Removes the Product By Id
        /// </summary>
        /// <param name="id">Represents the Bson Id</param>
        /// <returns>boolean</returns>
        public async Task<bool> DeleteProduct(string id)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Id, id);

            DeleteResult deleteResult = await _context.Products.DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }
        /// <summary>
        /// Gets Products By Bson Id
        /// </summary>
        /// <param name="id"> String represents the Bson Id</param>
        /// <returns> Product </returns>
        public async Task<Product> GetProduct(string id)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Id, id);
            return await _context.Products.Find(filter).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Gets Products by Category
        /// </summary>
        /// <param name="categoryName"> String Category Name</param>
        /// <returns> IEnumerable Products</returns>
        public async Task<IEnumerable<Product>> GetProductByCategory(string categoryName)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Category, categoryName);
            return await _context.Products.Find(filter).ToListAsync();
        }
        /// <summary>
        /// Gets Products by Name
        /// </summary>
        /// <param name="name"> String Product Name</param>
        /// <returns> IEnumerable Products</returns>
        public async Task<IEnumerable<Product>> GetProductByName(string name)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Name, name);
            return await _context.Products.Find(filter).ToListAsync();
        }
        /// <summary>
        /// Gets All Products
        /// </summary>
        /// <returns> IEnumerable Products</returns>
        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _context.Products.Find(p=>true).ToListAsync();
        }
        /// <summary>
        /// Updates the given Product
        /// </summary>
        /// <param name="product"> Represents a Product Entity</param>
        /// <returns>boolean</returns>
        public async Task<bool> UpdateProduct(Product product)
        {
            var updateResult = await _context
                .Products
                .ReplaceOneAsync(g => g.Id == product.Id, product);
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }
    }
}
