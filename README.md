### Introduction
<p>Mulligan is a library that defines generic retry logic. This code is inspired by the retry code in [FlaUI](https://github.com/Roemer/FlaUI), [TestStack.White](https://github.com/TestStack/White) and some feed back from a co-worker.</p>


### Why would you need this?
<p>The reason to use this is if you have code that is inherently temporal. For example when working with UI automation the UI will update at different speeds and you would want to be able to retry finding a control.</p>

### Usage
This will retry the action until it passes or 1 second passes.
``` csharp
Action action = () => //Do something
Retry.While(function, TimeSpan.FromSeconds(1));
```

This will retry the function until the predicate evaluates false. The predicate should return true if you want to retry your function.
```csharp
Func<T> function = () => //Do something
Predicate<T> shouldRetry = resultOfFunction => //use the result of the function to evaluate if you should retry
Retry.While(shouldRetry, function, TimeSpan.FromSeconds(1));
```
