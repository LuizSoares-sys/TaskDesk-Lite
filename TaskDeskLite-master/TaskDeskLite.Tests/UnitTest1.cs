using TaskDeskLite.Core;

namespace TaskDeskLite.Tests
{
    public class UnitTest1
    {
        [Fact(DisplayName = "Prazo da data do cadastro")]
        public void Create_Deve_Definir_CreatedAt_Com_Data_De_Hoje()
        {
            var service = new TaskService();

            var task = service.Create(new TaskItem
            {
                Title = "Teste",
                Priority = TaskPriority.Medium
            });

            Assert.Equal(DateTime.Today, task.CreatedAt.Date);
        }

    }
}
