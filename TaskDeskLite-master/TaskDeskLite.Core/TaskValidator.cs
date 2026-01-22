namespace TaskDeskLite.Core
{
    // Classe responsável por validar regras de negócio relacionadas à TaskItem
    public static class TaskValidator
    {
        // Lista de palavras proibidas no título da tarefa
        private static readonly string[] ForbiddenWords = { "hack", "drop", "delete" };

        // Método usado tanto na criação quanto na atualização de uma tarefa
        public static void ValidateForCreateOrUpdate(TaskItem task)
        {
            // Verifica se o objeto da tarefa é nulo
            if (task is null)
                throw new DomainValidationException("Tarefa inválida.");

            // Obtém o título, garantindo que não seja nulo e removendo espaços extras
            var title = (task.Title ?? "").Trim();

            // Valida se o título está vazio ou contém apenas espaços
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainValidationException("Título é obrigatório.");

            // Valida o tamanho mínimo e máximo do título
            if (title.Length < 3 || title.Length > 40)
                throw new DomainValidationException("Título deve ter entre 3 e 40 caracteres.");

            // Verifica se o título contém alguma palavra proibida (case-insensitive)
            if (ForbiddenWords.Any(w => title.Contains(w, StringComparison.OrdinalIgnoreCase)))
                throw new DomainValidationException("Título contém termo não permitido.");

            // Verifica se a prioridade informada existe no enum TaskPriority
            if (!Enum.IsDefined(typeof(TaskPriority), task.Priority))
                throw new DomainValidationException("Prioridade inválida.");

            // Caso exista descrição, valida se ela não ultrapassa 200 caracteres
            if (task.Description is not null && task.Description.Length > 200)
                throw new DomainValidationException("Descrição deve ter no máximo 200 caracteres.");

            // Caso exista uma data de prazo
            if (task.DueDate.HasValue)
            {
                // Obtém apenas a data (sem horário)
                var due = task.DueDate.Value.Date;
                var today = DateTime.Now.Date;

                // Verifica se o prazo é anterior à data atual
                if (due < today)
                    throw new DomainValidationException("Prazo não pode ser no passado.");
            }
        }
    }
}
