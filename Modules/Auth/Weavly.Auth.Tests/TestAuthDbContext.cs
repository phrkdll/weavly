using Microsoft.EntityFrameworkCore;
using Weavly.Auth.Persistence;

namespace Weavly.Auth.Tests;

public class TestAuthDbContext(DbContextOptions options) : AuthDbContext(options);
