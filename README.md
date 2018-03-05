- <a href="https://www.nuget.org/packages/Mulligan/2.0.0">Nuget Package</a>
- <a href="https://github.com/maxinfet/Mulligan/releases">Releases</a>

### Introduction
<p>Mulligan is a library that defines generic retry logic. This code is inspired by the retry code in FlaUI, TestStack.White and some feed back from a co-worker.</p>


### Why would you need this?
<p>The reason for using this is if your code is interacting with an entity that is timing based. For example when working with UI automation the UI will update at different speeds and you would want to be able to retry finding a control.</p>

### Usage
This will retry the action until it passes or 1 second passes. Then it will return a `RetryResult` that contains information about each retry that was attempted.
``` csharp
Action action = () => //Do something
RetryResult retryResult = Retry.While(action, TimeSpan.FromSeconds(1));

int retryCount = retryResult.Count;
bool isCompletedSuccessfully = retryResult.IsCompletedSuccessfully;
```

This will retry the function until the predicate evaluates false. The predicate should return true if you want to retry your function. The `Retry.While` will then return a `RetryResult` that contains the result and information about each retry it attempted.
```csharp
Func<T> function = () => //Do something
Predicate<T> shouldRetry = resultOfFunction => //use the result of the function to evaluate if you should retry
RetryResult<T> retryResult = Retry.While(shouldRetry, function, TimeSpan.FromSeconds(1));

int retryCount = retryResult.Count;
bool isCompletedSuccessfully = retryResult.IsCompletedSuccessfully;
T result = retryResult.Result.Value;
```
