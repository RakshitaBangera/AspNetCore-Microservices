using Microsoft.AspNetCore.Mvc;
using NameService.DTO;
using NameService.Models;
using NameService.Data;
using StackExchange.Redis;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace NameService;

[ApiController]
[Route("api")]
public class NameController : ControllerBase
{
    private readonly NameDbContext _context;
    private readonly IDatabase _redis;

    public NameController(NameDbContext context,IConnectionMultiplexer redis)//indicates that the NameController class has a constructor that takes a NameDbContext parameter. This allows the controller to access the database context and perform operations on the Names table.
    {
        _context = context;
        _redis = redis.GetDatabase();
    }
    


    [HttpPost("concatenate")]
    public async Task<IActionResult> Concatenate(List<NameRequest> requests)
    {
         var names = requests.Select(request => new Name
        {
            FirstName = request.FirstName,
            LastName = request.LastName
        }).ToList();

        _context.Names.AddRange(names); //indicates that the AddRange method is called on the Names DbSet of the NameDbContext. This method adds multiple Name entities to the context, preparing them to be inserted into the database when SaveChangesAsync is called.

        await _context.SaveChangesAsync();
        

        var fullNames = names.Select(name =>
            name.FirstName + " " + name.LastName
        ).ToList();

       
        var cachedProducts = await _redis.StringGetAsync("products");//this line retrieves the cached product data from Redis using the StringGetAsync method. It attempts to get the value associated with the key "products" from Redis. If the data is found in Redis, it will be stored in the cachedProducts variable; otherwise, it will be null.

        List<Product> products;//this line declares a variable named products of type List<Product>. It will be used to store the list of Product entities that will be returned in the response.
        string source;//this line declares a variable named source of type string. It will be used to indicate the source of the product data, whether it came from Redis or the database.
        if (cachedProducts.HasValue)//checks if the cachedProducts variable has a value, indicating that the product data was found in Redis. If it has a value, it means the data is already cached, and the code proceeds to deserialize it into a List<Product> using JsonSerializer.Deserialize. The deserialized products are then assigned to the products variable for further use.
        {
            products = JsonSerializer.Deserialize<List<Product>>(cachedProducts.ToString())!;//here, the cached product data retrieved from Redis is deserialized into a List<Product> using JsonSerializer.Deserialize. The cachedProducts variable is converted to a string using ToString() before deserialization. The exclamation mark (!) at the end indicates that the deserialization result is expected to be non-null, and it is assigned to the products variable for further use.
            source = "Products came from Redis";
            
        }
        else
        {
            products = await _context.Products.ToListAsync();//indicates that the ToListAsync method is called on the Products DbSet of the NameDbContext. This method asynchronously retrieves all Product entities from the database and returns them as a List<Product>.

            var productJson = JsonSerializer.Serialize(products);//this line serializes the list of Product entities into a JSON string using the JsonSerializer.Serialize method. This allows the products to be stored in Redis as a string.

            await _redis.StringSetAsync("products", productJson,TimeSpan.FromSeconds(20));//this line stores the serialized JSON string of products in Redis using the StringSetAsync method. The key used for storing the data is "products". This allows subsequent requests to retrieve the product data from Redis instead of querying the database again.
            source = "Products did not come from Redis";
        }
        return Ok(new
        {
            names = fullNames,
            products = products,
            source = source
        });
    

        
//         the payload structure for the above endpoint is as follows:
// [
//     {
//         "firstName": "Rakshita",
//         "lastName": "Bangera"
//     },
//     {
//         "firstName": "John",
//         "lastName": "Smith"
//     }
// ]
    }
}