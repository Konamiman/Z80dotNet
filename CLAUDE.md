# Coding Conventions

## Brace Placement

- **Top-level structures** (namespaces, classes, interfaces, methods, properties, getters/setters): opening brace on a new line.
  ```csharp
  public class MyClass
  {
      public int Value
      {
          get
          {
              return _value;
          }
      }
  }
  ```

- **Control structures** (if, else, for, while, foreach, switch, try/catch): opening brace on the same line.
  ```csharp
  if (condition) {
      DoSomething();
  }
  else {
      DoSomethingElse();
  }

  for (int i = 0; i < 10; i++) {
      Process(i);
  }
  ```

## Null Handling

- Prefer null-coalescing operators (`??`, `?.`, `??=`) over explicit if/else blocks when it doesn't hurt readability.
  ```csharp
  // Preferred
  var result = cachedValue ?? ComputeValue();

  // Avoid
  var result = cachedValue != null ? cachedValue : ComputeValue();
  ```

## Pattern Matching

- Use [pattern matching](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/pattern-matching) whenever it improves readability.
  ```csharp
  // Preferred
  if (accessMode is MemoryAccessMode.ReadAndWrite or MemoryAccessMode.WriteOnly) {
      DoSomething();
  }

  // Avoid
  if (accessMode == MemoryAccessMode.ReadAndWrite || accessMode == MemoryAccessMode.WriteOnly) {
      DoSomething();
  }
  ```
