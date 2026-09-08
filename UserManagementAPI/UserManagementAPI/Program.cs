using Microsoft.AspNetCore.Identity;
using UserManagementAPI1;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();
var logger = app.Logger;

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.UseErrorHandling();
app.UseTokenAuthentication();
app.UseRequestResponseLogging();


var users = new Dictionary<int, User>
{
    {1, new User {Id = 1, Name = "Anna", Email = "asd123@gmail.com"}},
    {2, new User {Id = 2, Name = "Bea", Email = "aaaa@gmail.com"}},
    {3, new User {Id = 3, Name = "Jay", Email = "ddjdjd@gmail.com"}}
};

var nextId = users.Keys.Max() + 1;


app.MapGet("/users", (int? page, int? pageSize) =>
{
    const int defaultPageSize = 50;
    const int maximumPageSize = 100;

    int currentPage = page ?? 1;
    int size = pageSize ?? defaultPageSize;

    if (currentPage < 1)
    {
        return Results.BadRequest(new
        {
            error = "Page must be greater than 0."
        });
    }

    if (size < 1 || size > maximumPageSize)
    {
        return Results.BadRequest(new
        {
            error = $"Page size must be between 1 and {maximumPageSize}."
        });
    }

    var pagedUsers = users.Values
        .OrderBy(user => user.Id)
        .Skip((currentPage - 1) * size)
        .Take(size)
        .ToList();

    logger.LogInformation(
        "Fetched page {Page} with {Count} users.",
        currentPage,
        pagedUsers.Count);

    return Results.Ok(new
    {
        page = currentPage,
        pageSize = size,
        totalUsers = users.Count,
        data = pagedUsers
    });
});


app.MapGet("/users/{id:int}", (int id) =>
{
    if (users.TryGetValue(id, out var user))
    {
        logger.LogInformation(
            "User with ID {Id} retrieved successfully.",
            id);

        return Results.Ok(user);
    }

    logger.LogWarning(
        "Lookup failed: User with ID {Id} was not found.",
        id);

    return Results.NotFound(new
    {
        error = $"User with ID {id} not found."
    });
});


app.MapGet("/users/search", (string? name, string? email) =>
{
    IEnumerable<User> query = users.Values;

    if (!string.IsNullOrWhiteSpace(name))
    {
        query = query.Where(user =>
            user.Name.Contains(
                name,
                StringComparison.OrdinalIgnoreCase));
    }

    if (!string.IsNullOrWhiteSpace(email))
    {
        query = query.Where(user =>
            user.Email.Contains(
                email,
                StringComparison.OrdinalIgnoreCase));
    }

    var result = query
        .OrderBy(user => user.Id)
        .ToList();

    logger.LogInformation(
        "User search returned {Count} users.",
        result.Count);

    return Results.Ok(result);
});


app.MapGet("/users/names", () =>
{
    var names = users.Values
        .OrderBy(user => user.Id)
        .Select(user => user.Name)
        .ToList();

    logger.LogInformation(
        "Fetched {Count} user names.",
        names.Count);

    return Results.Ok(names);
});


app.MapPost("/users", (User user) =>
{
    var validationError = UserValidator.Validate(user);

    if (validationError is not null)
    {
        return Results.BadRequest(new
        {
            error = validationError
        });
    }

    var newId = Interlocked.Increment(ref nextId);

    user.Id = newId;

    users[user.Id] = user;

    logger.LogInformation(
        "Created new user with ID {Id}.",
        user.Id);

    return Results.Created(
        $"/users/{user.Id}",
        user);
});


app.MapPut("/users/{id:int}", (int id, User user) =>
{
    if (!users.ContainsKey(id))
    {
        logger.LogWarning(
            "Update failed: User with ID {Id} was not found.",
            id);

        return Results.NotFound(new
        {
            error = $"User with ID {id} not found."
        });
    }

    var validationError = UserValidator.Validate(user);

    if (validationError is not null)
    {
        return Results.BadRequest(new
        {
            error = validationError
        });
    }


    user.Id = id;

    users[id] = user;

    logger.LogInformation(
        "Updated user with ID {Id}.",
        id);

    return Results.Ok(user);
});


app.MapDelete("/users/{id:int}", (int id) =>
{
    if (users.Remove(id, out _))
    {
        logger.LogInformation(
            "Deleted user with ID {Id}.",
            id);

        return Results.NoContent();
    }

    logger.LogWarning(
        "Delete failed: User with ID {Id} was not found.",
        id);

    return Results.NotFound(new
    {
        error = $"User with ID {id} not found."
    });
});


if (app.Environment.IsDevelopment())
{
    app.MapGet("/test-error", () =>
    {
        throw new InvalidOperationException(
            "This is a test exception.");
    });
}

app.Run();


public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
}

