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

        // Executa a validação da tarefa antes de qualquer alteração ou persistência
        // Garante que as regras de negócio sejam respeitadas
        TaskValidator.ValidateForCreateOrUpdate(task);

        // Gera um novo identificador único para a tarefa
        task.Id = Guid.NewGuid();

        // Define o status inicial da tarefa como "Pending" (pendente)
        task.Status = TaskStatus.Pending;

        // Registra a data e hora de criação da tarefa usando UTC
        task.CreatedAt = DateTime.UtcNow;

        // Adiciona a tarefa à coleção de tarefas (ex: lista em memória ou repositório)
        _tasks.Add(task);

        // Retorna a tarefa recém-criada já com os dados preenchidos
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
        // Segundo regra de negocio tarefa pode ser removida
        _tasks.Remove(task);
    }
    // Marca a tarefa como concluída
    public TaskItem MarkAsDone(Guid id)
    {
        var task = GetById(id); // Lança NotFoundException se não existir
        task.Status = TaskStatus.Done;
        return task;
    }
}
