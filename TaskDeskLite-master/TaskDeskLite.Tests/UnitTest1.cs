using TaskDeskLite.Core;

namespace TaskDeskLite.Tests
{
    public class UnitTest1
    {
        // TESTE PARA VERIFICAR SE A DESCRIÇÃO NÃO ULTRAPASSA 200 CARACTERES
        [Fact(DisplayName = "Descricao se limita a 200 caracters")]
        public void Descricao200Caracters()
        {

            // Cria uma string com 201 caracteres para ultrapassar o limite permitido (200)
            var DescricaoInvalida = new string('a', 201); // 201 caracteres da letra 'a'

            // Cria uma tarefa válida em todos os aspectos,
            // exceto pela descrição que é maior que 200 caracteres
            var task = new TaskItem
            {
                Title = "Tarefa válida",          // Título válido para não interferir no teste
                Description = DescricaoInvalida,    // Descrição inválida (maior que 200)
                Priority = TaskPriority.Low       // Prioridade válida
            };


            // Executa o método de validação e verifica se ele lança
            // uma DomainValidationException
            var exception = Assert.Throws<DomainValidationException>(() =>
                TaskValidator.ValidateForCreateOrUpdate(task)
            );

            // Verifica se a mensagem da exceção é exatamente a esperada
            Assert.Equal("Descrição deve ter no máximo 200 caracteres.", exception.Message);
        }

        // TESTE PARA VERIFICAR SE O TÍTULO NÃO CONTÉM PALAVRAS PROIBIDAS
        [Fact(DisplayName = "Palavras Bloqueadas")]
        public void PalavrasBloqueadas()
        {
            // Cria uma tarefa com uma palavra proibida no título
            var task = new TaskItem
            {
                Title = "DROP TABLE tasks", // Palavra proibida "hack"
                Description = "Descrição válida",
                Priority = TaskPriority.Medium
            };
            // Executa o método de validação e verifica se ele lança
            // uma DomainValidationException
            var exception = Assert.Throws<DomainValidationException>(() =>
                TaskValidator.ValidateForCreateOrUpdate(task)
            );
            // Verifica se a mensagem da exceção é exatamente a esperada
            Assert.Equal("Título contém termo não permitido.", exception.Message);
        }

        [Fact(DisplayName = "Tarefa concluida nao pode ser editada")]
        public void TarefaConcluidaNaoPodeSerAlterada()
        {
            // Cria uma instância do serviço de tarefas
            var service = new TaskService();
            // Cria uma nova tarefa válida
            var task = new TaskItem
            {

                Title = "Tarefa para teste",
                Description = "Descrição da tarefa",
                Priority = TaskPriority.Medium,
                Status = Core.TaskStatus.Done
            };
          
            // Verifica se a atualização lança uma BusinessRuleException
            var exception = Assert.Throws<BusinessRuleException>(() =>
                service.Update(task)
            );
            // Verifica se a mensagem da exceção é exatamente a esperada
            Assert.Equal("Tarefa concluída não pode ser editada.", exception.Message);
        }
    }
}
