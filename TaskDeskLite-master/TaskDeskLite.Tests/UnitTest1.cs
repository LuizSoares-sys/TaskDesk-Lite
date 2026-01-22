using TaskDeskLite.Core;

namespace TaskDeskLite.Tests
{
    public class UnitTest1
    {
        // TESTE PARA VERIFICAR SE A DESCRIÇÃO NÃO ULTRAPASSA 200 CARACTERES
        [Fact(DisplayName = "Descricao se limita a 201 caracters")]
        public void Descricao200Caracters()
        {

            // Cria uma string com 201 caracteres para ultrapassar o limite permitido (200)
            var DescricaoInvalida = new string('a', 201); // 201 caracteres 'a'

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
    }
}
