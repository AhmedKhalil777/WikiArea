using MongoDB.Driver;
using WikiArea.Core.Entities;
using WikiArea.Core.Enums;
using WikiArea.Core.Interfaces;
using WikiArea.Infrastructure.Data;

namespace WikiArea.Infrastructure.Repositories;

public class UserRepository : MongoRepository<User>, IUserRepository
{
    public UserRepository(WikiAreaMongoContext context) : base(context.Users)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Username == username && !x.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Email == email && !x.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByUsernameOrEmailAsync(string username, string email, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => (x.Username == username || x.Email == email) && !x.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByAdfsIdAsync(string adfsId, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.AdfsId == adfsId && !x.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Role == role && !x.IsDeleted)
            .SortBy(x => x.DisplayName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetByDepartmentAsync(string department, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Department == department && !x.IsDeleted)
            .SortBy(x => x.DisplayName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>.Filter.And(
            Builders<User>.Filter.Or(
                Builders<User>.Filter.Regex(x => x.Username, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                Builders<User>.Filter.Regex(x => x.DisplayName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                Builders<User>.Filter.Regex(x => x.Email, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            ),
            Builders<User>.Filter.Eq(x => x.IsDeleted, false)
        );

        return await _collection.Find(filter)
            .SortBy(x => x.DisplayName)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsUsernameUniqueAsync(string username, string? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>.Filter.And(
            Builders<User>.Filter.Eq(x => x.Username, username),
            Builders<User>.Filter.Eq(x => x.IsDeleted, false)
        );

        if (!string.IsNullOrEmpty(excludeUserId))
        {
            filter = Builders<User>.Filter.And(filter, 
                Builders<User>.Filter.Ne(x => x.Id, excludeUserId));
        }

        var count = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count == 0;
    }

    public async Task<bool> IsEmailUniqueAsync(string email, string? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>.Filter.And(
            Builders<User>.Filter.Eq(x => x.Email, email),
            Builders<User>.Filter.Eq(x => x.IsDeleted, false)
        );

        if (!string.IsNullOrEmpty(excludeUserId))
        {
            filter = Builders<User>.Filter.And(filter, 
                Builders<User>.Filter.Ne(x => x.Id, excludeUserId));
        }

        var count = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count == 0;
    }
} 