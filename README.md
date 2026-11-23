# user-service-demo
Репозиторий-пример merge-конфликтов и конфликтов логической связности 

# Исправленные ошибки

## 1. Ошибка в UsersController.cs

Проблема: после слияния веток валидация в методе Register не проверяла новое поле FullName, добавленное в ветке

Что было:
```csharp
if (string.IsNullOrWhiteSpace(request.Email))
    throw new ArgumentException("Email is required");
if (request.Password is { Length: < 8 })
    throw new ArgumentException("Password too short");

Console.WriteLine($"[register] attempt for {request.Email}");

var dto = await _users.RegisterAsync(request, ct);
return Ok(dto); // ответ: { id, email }
```

Как правильно:
```csharp
if (string.IsNullOrWhiteSpace(request.Email))
    return BadRequest("Email is required");
if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 8)
    return BadRequest("Password too short");
if (string.IsNullOrWhiteSpace(request.FullName))
    return BadRequest("FullName is required");

Console.WriteLine($"[register] attempt for {request.Email}");

var dto = await _users.RegisterAsync(request, ct);
return Ok(dto); // ответ: { id, email, fullName }
```

## 2. Ошибка в UsersController.cs

Проблема: поле _users и конструктор были объявлены после метода Register, который их использует

Что было:
```csharp
public class UsersController : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(...)
    {
        var dto = await _users.RegisterAsync(request, ct); // Ошибка: _users еще не объявлен
        return Ok(dto);
    }

    private readonly IUserService _users;  // Объявление после использования
    public UsersController(IUserService users) => _users = users;
}
```

Как правильно:
```csharp
public class UsersController : ControllerBase
{
    private readonly IUserService _users;
    public UsersController(IUserService users) => _users = users;

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(...)
    {
        var dto = await _users.RegisterAsync(request, ct);
        return Ok(dto);
    }
}
```

## 2. Ошибка в клиенте (client.py)

Проблема: клиент не соответствует API. Бэкенд теперь требует поле fullName и возвращает его в ответе, а клиент его не отправляет и не ожидает

Что было:
```python
EXPECTED = ("id", "email")

def register(email: str, password: str) -> dict | None:
    payload = {"email": email, "password": password}
    ...

r = register("student@example.com", "password123")
```

Как правильно:
```python
EXPECTED = ("id", "email", "fullName")

def register(email: str, password: str, full_name: str) -> dict | None:
    payload = {"email": email, "password": password, "fullName": full_name}
    ...

r = register("student@example.com", "password123", "Ivan Petrov")
```
