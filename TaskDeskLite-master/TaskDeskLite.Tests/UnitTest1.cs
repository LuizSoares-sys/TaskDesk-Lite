using TaskDeskLite.Core;

namespace TaskDeskLite.Tests
{
    public class UnitTest1
    {
        // TESTE PARA VERIFICAR SE A DESCRIÇÃO NÃO ULTRAPASSA 200 CARACTERES TESTE 1
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

        // TESTE PARA VERIFICAR SE O TÍTULO NÃO CONTÉM PALAVRAS PROIBIDAS  // TESTE 2
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
        
        //TESTE DE PRAZO DA DATA DE CADASTRO // TESTE 3
        [Fact(DisplayName = "Prazo da data do cadastro")]
        public void Create_Definir_CreatedAt_Com_Data_De_Hoje()
        {
            // Cria uma instância do serviço responsável por criar tarefas
            var service = new TaskService();

            // Cria uma nova tarefa usando o serviço
            var task = service.Create(new TaskItem
            {
                // Título da tarefa
                Title = "Teste",

                // Prioridade da tarefa (nível médio)
                Priority = TaskPriority.Medium
            });

            // Verifica se a data de criação da tarefa
            // é igual à data de hoje (ignorando horas, minutos e segundos)
            Assert.Equal(DateTime.Today, task.CreatedAt.Date);

        }
        // TESTE PARA VERIFICAR SE O TÍTULO NÃO É NULO, VAZIO OU APENAS ESPAÇOS QUANDO CRIADO TESTE 4
        /// Testa se o método Create rejeita títulos inválidos (nulo, vazio ou apenas espaços).
        /// Este teste valida a regra: "Título não pode ser só espaços".
        /// <param name="invalidTitle">Título inválido a ser testado (null, "", " ", etc.)</param>
        [Theory]
        [InlineData(null)]      // Testa título nulo
        [InlineData("")]        // Testa string vazia
        [InlineData(" ")]       // Testa um espaço
        [InlineData("   ")]     // Testa múltiplos espaços
        [InlineData("\t")]      // Testa tabulação
        [InlineData("\n")]      // Testa quebra de linha
        [InlineData("\r\n")]    // Testa quebra de linha Windows
        public void Create_ComTituloInvalido_ApenasEspacos_DeveLancarExcecao(string? invalidTitle)
        {
            // Arrange: Prepara os dados do teste
            var service = new TaskService();
            var task = new TaskItem
            {
                Title = invalidTitle!,  // Título inválido (nulo, vazio ou só espaços)
                Description = "Descrição válida",
                Priority = TaskPriority.Medium,
                DueDate = DateTime.Now.AddDays(1)
            };

            // Act & Assert: Tenta criar a tarefa e verifica se lança exceção
            // O sistema deve rejeitar e lançar DomainValidationException
            var exception = Assert.Throws<DomainValidationException>(() => service.Create(task));
            Assert.Equal("Título é obrigatório.", exception.Message);
        }

        // TESTE PARA VERIFICAR SE O TÍTULO NÃO É NULO, VAZIO OU APENAS ESPAÇOS AO ATUALIZAR TESTE 5
        /// <summary>
        /// Testa se o método Update rejeita títulos inválidos (nulo, vazio ou apenas espaços).
        /// Este teste valida a mesma regra do Create: "Título não pode ser só espaços".
        /// </summary>
        /// <param name="invalidTitle">Título inválido a ser testado (null, "", " ", etc.)</param>
        [Theory]
        [InlineData(null)]      // Testa título nulo
        [InlineData("")]        // Testa string vazia
        [InlineData(" ")]       // Testa um espaço
        [InlineData("   ")]     // Testa múltiplos espaços
        [InlineData("\t")]      // Testa tabulação
        [InlineData("\n")]      // Testa quebra de linha
        [InlineData("\r\n")]    // Testa quebra de linha Windows
        public void Update_ComTituloInvalido_ApenasEspacos_DeveLancarExcecao(string? invalidTitle)
        {
            // Arrange: Prepara os dados do teste
            var service = new TaskService();
            
            // Primeiro cria uma tarefa válida para poder testar a atualização
            var validTask = new TaskItem
            {
                Title = "Tarefa válida",
                Description = "Descrição",
                Priority = TaskPriority.Medium,
                DueDate = DateTime.Now.AddDays(1)
            };
            var createdTask = service.Create(validTask);

            // Prepara uma atualização com título inválido
            var taskToUpdate = new TaskItem
            {
                Id = createdTask.Id,
                Title = invalidTitle!,  // Título inválido (nulo, vazio ou só espaços)
                Description = "Descrição atualizada",
                Priority = TaskPriority.High,
                DueDate = DateTime.Now.AddDays(2),
                Status = Core.TaskStatus.Pending,
                CreatedAt = createdTask.CreatedAt
            };

            // Act & Assert: Tenta atualizar a tarefa e verifica se lança exceção
            // O sistema deve rejeitar e lançar DomainValidationException
            var exception = Assert.Throws<DomainValidationException>(() => service.Update(taskToUpdate));
            Assert.Equal("Título é obrigatório.", exception.Message);
        }

        // TESTE PARA VERIFICAR SE UMA TAREFA CONCLUÍDA PODE SER EDITADA TESTE 6

        [Fact(DisplayName = "Tarefa concluida nao pode ser editada")]

        public void TarefaConcluidaNaoPodeSerAlterada()

        {
            var service = new TaskService();

            // Cria a tarefa
            var task = new TaskItem
            {
                Title = "Tarefa para teste",
                Description = "Descrição da tarefa",
                Priority = TaskPriority.Medium
            };

            // Salva a nova tarefa no sistema
            var tarefaNova = service.Create(task);

            // Atualiza o status para concluído
            service.MarkAsDone(tarefaNova.Id);

            //  Tenta editar depois de concluída
            var taskToUpdate = new TaskItem

            {
                Id = tarefaNova.Id,
                Title = "Título alterado",
                Description = "Descrição alterada",
                Priority = TaskPriority.High
            };

            // Verifica se ao tentar atualizar uma tarefa concluída lança a exceção esperada
            var exception = Assert.Throws<BusinessRuleException>(() =>
            service.Update(taskToUpdate)

            );

            Assert.Equal("Tarefa concluída não pode ser editada.", exception.Message);
        }

        // TESTE PARA VERIFICAR SE O SISTEMA ACEITA TITULO COM 2 CARACTERES (LIMITE MINIMO) ESTE 7

        // Método auxiliar para criar uma nova instância do serviço
        // Assim cada teste começa com a lista de tarefas vazia
        private TaskService CreateService() => new();

            [Fact]
            public void Create_TitleWith3Characters_ShouldCreateTask()
            {
                var service = CreateService();
                var task = new TaskItem
                {
                    Title = "Abc" // 3 caracteres
                };

                var result = service.Create(task);

                Assert.NotEqual(Guid.Empty, result.Id);
               
            }

        // TESTE PARA VERIFICAR SE O SISTEMA ACEITA TÍTULO COM 40 CARACTERES (LIMITE MÁXIMO) TESTE 8

        [Fact]
            // Teste para verificar se o sistema aceita título com 40 caracteres (limite máximo)
            public void Criar_TituloCom40Caracteres_DeveCriarTarefa()
            {
                var service = CreateService();

                // Cria uma tarefa com título no limite máximo permitido
                var task = new TaskItem
                {
                    Title = new string('A', 40) // 40 caracteres
                };

                // Executa a criação da tarefa
                var result = service.Create(task);

                // Verifica se o ID foi gerado corretamente
                Assert.NotEqual(Guid.Empty, result.Id);

             
            }
        }
    }

