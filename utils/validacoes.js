function validarObrigatorios(task) {
  if (!task.nome) throw new Error('Nome obrigatório');
  if (!task.descricao) throw new Error('Descrição obrigatória');
}

function validarLimites(task) {
  if (task.nome.length < 3) throw new Error('Nome deve ter pelo menos 3 caracteres');
  if (task.nome.length > 50) throw new Error('Nome excede 50 caracteres');
}

function validarData(data) {
  if (!data.match(/^\d{4}-\d{2}-\d{2}$/)) throw new Error('Formato de data inválido');
}

module.exports = { validarObrigatorios, validarLimites, validarData };
