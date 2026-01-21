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

        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public TaskItem MarkAsDone(Guid id)
    {
        var task = GetById(id); // Lança NotFoundException se não existir
        task.Status = TaskStatus.Done;
        return task;
    }
}
