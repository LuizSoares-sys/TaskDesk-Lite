namespace TaskDeskLite.Core;

public class TaskService : ITaskService
{
    // Persistência em memória
    private readonly List<TaskItem> _tasks = new();

    public IReadOnlyList<TaskItem> GetAll()
        => _tasks.OrderByDescending(t => t.CreatedAt).ToList();

    public TaskItem GetById(Guid id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task is null) throw new NotFoundException("Tarefa não encontrada.");
        return task;
    }

    public TaskItem Create(TaskItem task)
    {
        // TODO: validar
        // TODO: garantir Id novo e Status Pending
        // TODO: adicionar na lista
        // TODO: retornar a tarefa criada

        TaskValidator.ValidateForCreateOrUpdate(task);

        task.Id = Guid.NewGuid();
        task.Status = TaskStatus.Pending;
        task.CreatedAt = DateTime.UtcNow;

        _tasks.Add(task);

        return task;
    }

    public TaskItem Update(TaskItem task)
    {
        if (task is null) throw new DomainValidationException("Tarefa inválida.");

        var existing = GetById(task.Id); // Lança NotFoundException se não existir

        // Regra de negócio: tarefa concluída não pode ser editada
        if (existing.Status == TaskStatus.Done)
            throw new BusinessRuleException("Tarefa concluída não pode ser editada.");

        // Validação completa: título, descrição, prazo, palavras proibidas
        TaskValidator.ValidateForCreateOrUpdate(task);

        // Atualizar apenas campos permitidos
        existing.Title = task.Title;
        existing.Description = task.Description;
        existing.Priority = task.Priority;
        existing.DueDate = task.DueDate;
        // Status, CreatedAt e Id são preservados da tarefa original

        return existing;
    }

    public void Delete(Guid id)
    {
        // TODO: se não existir -> NotFoundException
        // TODO: remover


        // Procura na lista _tasks a primeira tarefa cujo Id seja igual ao id informado
        // FirstOrDefault retorna a tarefa encontrada ou null se não existir
        var task = _tasks.FirstOrDefault(t => t.Id == id);

        // Verifica se nenhuma tarefa foi encontrada com esse Id
        // Se task for null, lança uma exceção indicando que a tarefa não existe
        if (task is null)
            throw new NotFoundException("Tarefa não encontrada.");

        _tasks.Remove(task);
    }

    public TaskItem MarkAsDone(Guid id)
    {
        var task = GetById(id); // Lança NotFoundException se não existir
        task.Status = TaskStatus.Done;
        return task;
    }
}
