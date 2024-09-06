The GenericRepository<T> class is a generic repository pattern implementation in C#. It provides common CRUD (Create, Read, Update, Delete) operations for entities in a SQL Server database, using the DbContext class for database connection management and SQL commands for execution. Key Features:

Generic CRUD Operations: Supports GetAllAsync, GetByIdAsync, AddAsync, UpdateAsync, and DeleteAsync methods.
Reflection-based Object Mapping: Uses reflection to map SQL records to objects dynamically.
SQL Server Database Interaction: Executes raw SQL queries via SqlCommand.

Prerequisites Microsoft SQL Server Dependency Injection (DI) configured for DbContext

Installation

Clone or add the repository to your solution.

Add necessary dependencies to your project:
    Microsoft.Data.SqlClient for database interactions.
    Configure Dependency Injection for DbContext and GenericRepository.

Add service registrations in your Startup.cs or Program.cs:

    InfraInjectionModule.BindServices(builder.Services, connectionString)

Methods

    GetAllAsync()

Fetches all records from the table corresponding to the class T.



public async Task<IEnumerable<T>> GetAllAsync()

Returns: A list of all records as T objects.

    GetByIdAsync(int id)

Fetches a record by its primary key (Id).



public async Task<T> GetByIdAsync(int id)

Parameters: id - The primary key of the record.
Returns: The corresponding T object.

    AddAsync(T entity)

Inserts a new record into the database.



public async Task AddAsync(T entity)

Parameters: entity - The object to be added.

    UpdateAsync(int id, T entity)

Updates an existing record in the database.



public async Task UpdateAsync(int id, T entity)

Parameters:
    id - The primary key of the record to be updated.
    entity - The updated entity.

    DeleteAsync(int id)

Deletes a record from the database by its Id.

public async Task DeleteAsync(int id)

Parameters: id - The primary key of the record to be deleted.

How It Works

SQL Commands: Raw SQL queries are dynamically generated based on the type T. The class name of T is used as the table name, and properties are assumed to match the column names.
Reflection: The repository uses a ReflectionCache to retrieve the properties of the entity type T, mapping SQL columns to the corresponding object properties.
Dependency Injection: DbContext is injected into the repository for managing SQL connections.

Example Usage

In a Service Layer:

    public class UserService { private readonly IGenericRepository<User> _userRepository;

    public UserService(IGenericRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await _userRepository.GetAllAsync();
    }
    
    public async Task AddUser(User user)
    {
        await _userRepository.AddAsync(user);
    }
    
    // Other CRUD operations...
    
    }

In Startup/Program for Dependency Injection:

    services.AddScoped<DbContext>(provider => new DbContext("YourConnectionString"));
    services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

Error Handling

Each method catches and rethrows exceptions with meaningful messages, using:

    catch (Exception ex) { throw new Exception(ex.Message, ex); }

Contributions

Feel free to submit issues or pull requests for new features or improvements.
