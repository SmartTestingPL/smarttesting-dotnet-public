namespace Core.Scoring.Analysis;

public interface IExecutor
{
  Task<TResult> Run<TResult>(Func<Task<TResult>> func);
}

public class TaskBasedExecutor : IExecutor
{
  public async Task<TResult> Run<TResult>(Func<Task<TResult>> func)
  {
    return await Task.Run(func);
  }
}