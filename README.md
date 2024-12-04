## Propósito do Sistema
O sistema tem como objetivo principal gerenciar empréstimos de livros em uma biblioteca, fornecendo cadastro, consulta e exclusão de usuários, livros e registros de empréstimos.

## Usuários do Sistema
Bibliotecários: Administram o sistema, gerenciam usuários, livros e empréstimos.

## Requisitos Funcionais

1. Microsserviço Livros:

   - Listar todos os livros.
   - Cadastrar novos livros.
   - Atualizar o status de empréstimo de um livro.
   - Consultar detalhes de um livro específico.
   - Remover livros do sistema.

2. Microsserviço Usuários:

    - Listar todos os usuários.
   - Cadastrar novos usuários.
   - Consultar informações de um usuário específico, incluindo se tem empréstimos ativos.
   - Remover usuários do sistema.

3. Microsserviço Empréstimos:

    - Registrar novos empréstimos.
   - Consultar empréstimos ativos para um usuário.
   - Remover registros de empréstimos.

---

## Microsserviços

### LivrosAPI:

Função: Gerenciar o catálogo de livros.
Endpoints:
- GET /api/livros: Retorna todos os livros cadastrados.
- POST /api/livros: Cadastra um novo livro.
- PATCH /api/livros/{id}: Atualiza o status de empréstimo do livro (emprestado ou disponível).
- GET /api/livros/{id}: Retorna informações de um livro específico.
- DELETE /api/livros/{id}: Remove um livro do sistema.

### UsuariosAPI:

Função: Gerenciar os usuários da biblioteca.
Endpoints:
- GET /api/usuarios: Retorna todos os usuários cadastrados.
- POST /api/usuarios: Cadastra um novo usuário.
- GET /api/usuarios/{id}: Retorna informações detalhadas do usuário, incluindo se tem empréstimos ativos.
- DELETE /api/usuarios/{id}: Remove um usuário do sistema.

# EmprestimosAPI:

Função: Registrar e monitorar empréstimos de livros.
Endpoints:
- GET /api/emprestimos?usuarioId={id}: Retorna os empréstimos ativos de um usuário.
- POST /api/emprestimos: Registra um novo empréstimo. Verifica disponibilidade do livro e existência do usuário.
- DELETE /api/emprestimos/{id}: Remove um registro de empréstimo.

---

## Comunicação entre Microsserviços

EmprestimosAPI → LivrosAPI:

- Verifica se o livro está disponível antes de registrar um empréstimo.
- Atualiza o status do livro para "emprestado" ou "disponível" conforme necessário.

EmprestimosAPI → UsuariosAPI:

- Confirma a existência do usuário antes de registrar um empréstimo.

UsuariosAPI → EmprestimosAPI:

- Consulta empréstimos ativos ao exibir detalhes do usuário.
